```cs
// Replace the existing GetToken method in F5Client class with this implementation:

private string GetToken(string userName, string userPassword)
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetToken");
    
    // Create token manager
    TokenManager tokenManager = new TokenManager(logger);
    
    // Try to get stored token first
    var existingToken = tokenManager.GetStoredToken();
    if (existingToken != null && tokenManager.IsTokenValid(existingToken))
    {
        LogHandlerCommon.Trace(logger, CertificateStore, "Using stored token");
        return existingToken.Token;
    }

    // Create new token if none exists or the existing one is invalid
    LogHandlerCommon.Trace(logger, CertificateStore, "Stored token not found or expired - requesting new token");
    F5LoginRequest request = new F5LoginRequest() { username = userName, password = userPassword, loginProviderName = "tmos" };
    F5LoginResponse loginResponse = REST.Post<F5LoginResponse>($"/mgmt/shared/authn/login", JsonConvert.SerializeObject(request));
    
    if (loginResponse?.token?.token != null)
    {
        // Store token information
        var tokenInfo = new F5TokenInfo
        {
            Token = loginResponse.token.token,
            ExpirationMicros = loginResponse.token.expirationMicros,
            Timeout = loginResponse.token.timeout,
            UserName = userName,
            CreatedAt = DateTime.UtcNow
        };

        // Extend token timeout to maximum allowed (if not already)
        if (tokenInfo.Timeout < TokenManager.EXTENDED_TIMEOUT)
        {
            LogHandlerCommon.Trace(logger, CertificateStore, $"Extending token lifetime from {tokenInfo.Timeout}s to {TokenManager.EXTENDED_TIMEOUT}s");
            ExtendTokenTimeout(tokenInfo.Token, TokenManager.EXTENDED_TIMEOUT);
            tokenInfo.Timeout = TokenManager.EXTENDED_TIMEOUT;
            
            // Update expiration time (current microseconds + extended timeout in microseconds)
            long currentMicros = TokenManager.DateTimeToMicroseconds(DateTime.UtcNow);
            tokenInfo.ExpirationMicros = currentMicros + (TokenManager.EXTENDED_TIMEOUT * 1000000L);
        }

        // Save token to file
        tokenManager.StoreToken(tokenInfo);
    }

    LogHandlerCommon.MethodExit(logger, CertificateStore, "GetToken");
    return loginResponse.token.token;
}

// Add this method to the F5Client class:
private void ExtendTokenTimeout(string token, int timeoutSeconds)
{
    try
    {
        LogHandlerCommon.Trace(logger, CertificateStore, $"Extending token timeout to {timeoutSeconds} seconds");
        var extendRequest = new { timeout = timeoutSeconds };
        REST.Patch($"/mgmt/shared/authz/tokens/{token}", 
                   JsonConvert.SerializeObject(extendRequest), 
                   token);
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"Error extending token timeout: {ex.Message}");
        // Continue with the current token even if extension fails
    }
}







/// token info
// Add this class to your project namespace
using System;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.F5Orchestrator
{
    // Token storage model
    public class F5TokenInfo
    {
        public string Token { get; set; }
        public long ExpirationMicros { get; set; }
        public int Timeout { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Add this class for token management
    public class TokenManager
    {
        public const string TOKEN_FILE_PATH = "f5_token.json";
        public const int BUFFER_TIME_SECONDS = 300; // 5 minute buffer before expiration
        public const int EXTENDED_TIMEOUT = 36000; // Maximum 10 hours (in seconds)
        private ILogger logger;

        public TokenManager(ILogger logger)
        {
            this.logger = logger;
        }

        public F5TokenInfo GetStoredToken()
        {
            try
            {
                if (File.Exists(TOKEN_FILE_PATH))
                {
                    string json = File.ReadAllText(TOKEN_FILE_PATH);
                    return JsonConvert.DeserializeObject<F5TokenInfo>(json);
                }
            }
            catch (Exception ex)
            {
                // Log error but continue to get a new token
                logger?.LogError($"Error reading token file: {ex.Message}");
            }
            return null;
        }

        public void StoreToken(F5TokenInfo tokenInfo)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tokenInfo, Formatting.Indented);
                File.WriteAllText(TOKEN_FILE_PATH, json);
            }
            catch (Exception ex)
            {
                logger?.LogError($"Error saving token file: {ex.Message}");
            }
        }

        public bool IsTokenValid(F5TokenInfo tokenInfo)
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

        public static long DateTimeToMicroseconds(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epoch).TotalMilliseconds * 1000;
        }

        public static DateTime MicrosecondsToDateTime(long microseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(microseconds / 1000);
        }
    }
}
```