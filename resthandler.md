```c#
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

// Token storage model
public class TokenInfo
{
    public string Token { get; set; }
    public long ExpirationMicros { get; set; }
    public int Timeout { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RESTHandler
{
    private const string TOKEN_FILE_PATH = "f5_token.json";
    private const int BUFFER_TIME_SECONDS = 300; // 5 minute buffer before expiration
    private const int EXTENDED_TIMEOUT = 36000; // Maximum 10 hours

    // Modified GetToken method with persistence
    private string GetToken(string userName, string userPassword)
    {
        LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetToken");
        
        // Try to get stored token first
        var existingToken = GetStoredToken();
        if (existingToken != null && IsTokenValid(existingToken))
        {
            LogHandlerCommon.MethodExit(logger, CertificateStore, "GetToken - Using stored token");
            return existingToken.Token;
        }

        // Create new token if none exists or the existing one is invalid
        F5LoginRequest request = new F5LoginRequest() { username = userName, password = userPassword, loginProviderName = "tmos" };
        F5LoginResponse loginResponse = REST.Post<F5LoginResponse>($"{endpoint}/mgmt/shared/authn/login", JsonConvert.SerializeObject(request));
        
        if (loginResponse?.token?.token != null)
        {
            // Store token information
            var tokenInfo = new TokenInfo
            {
                Token = loginResponse.token.token,
                ExpirationMicros = loginResponse.token.expirationMicros,
                Timeout = loginResponse.token.timeout,
                UserName = userName,
                CreatedAt = DateTime.UtcNow
            };

            // Extend token timeout to maximum allowed (if not already)
            if (tokenInfo.Timeout < EXTENDED_TIMEOUT)
            {
                ExtendTokenTimeout(tokenInfo.Token, EXTENDED_TIMEOUT);
                tokenInfo.Timeout = EXTENDED_TIMEOUT;
                // Update expiration time (current microseconds + extended timeout in microseconds)
                long currentMicros = DateTimeToMicroseconds(DateTime.UtcNow);
                tokenInfo.ExpirationMicros = currentMicros + (EXTENDED_TIMEOUT * 1000000L);
            }

            // Save token to file
            StoreToken(tokenInfo);
        }

        LogHandlerCommon.MethodExit(logger, CertificateStore, "GetToken");
        return loginResponse.token.token;
    }

    private TokenInfo GetStoredToken()
    {
        try
        {
            if (File.Exists(TOKEN_FILE_PATH))
            {
                string json = File.ReadAllText(TOKEN_FILE_PATH);
                return JsonSerializer.Deserialize<TokenInfo>(json);
            }
        }
        catch (Exception ex)
        {
            // Log error but continue to get a new token
            LogHandlerCommon.Error(logger, CertificateStore, $"Error reading token file: {ex.Message}");
        }
        return null;
    }

    private void StoreToken(TokenInfo tokenInfo)
    {
        try
        {
            string json = JsonSerializer.Serialize(tokenInfo, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TOKEN_FILE_PATH, json);
        }
        catch (Exception ex)
        {
            LogHandlerCommon.Error(logger, CertificateStore, $"Error saving token file: {ex.Message}");
        }
    }

    private bool IsTokenValid(TokenInfo tokenInfo)
    {
        if (tokenInfo == null || string.IsNullOrEmpty(tokenInfo.Token))
            return false;

        // Convert expiration micros to DateTime
        DateTime expirationTime = MicrosecondsToDateTime(tokenInfo.ExpirationMicros);
        
        // Check if token is still valid with a buffer time
        DateTime currentTime = DateTime.UtcNow;
        TimeSpan bufferTimeSpan = TimeSpan.FromSeconds(BUFFER_TIME_SECONDS);
        
        return expirationTime > currentTime.Add(bufferTimeSpan);
    }

    private void ExtendTokenTimeout(string token, int timeoutSeconds)
    {
        try
        {
            var extendRequest = new { timeout = timeoutSeconds };
            REST.Patch($"{endpoint}/mgmt/shared/authz/tokens/{token}", 
                       JsonConvert.SerializeObject(extendRequest), 
                       token);
        }
        catch (Exception ex)
        {
            LogHandlerCommon.Error(logger, CertificateStore, $"Error extending token timeout: {ex.Message}");
        }
    }

    private static long DateTimeToMicroseconds(DateTime dateTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(dateTime - epoch).TotalMilliseconds * 1000;
    }

    private static DateTime MicrosecondsToDateTime(long microseconds)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddMilliseconds(microseconds / 1000);
    }
}
```