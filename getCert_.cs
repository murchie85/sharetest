using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

public async Task<string> HandleOidcRedirectsAsync(string initialUrl, string username, string password, HttpClient httpClient = null)
{
    // Create credentials
    var credentials = new NetworkCredential(username, password);

    // If no HttpClient provided, create one with credentials
    if (httpClient == null)
    {
        var handler = new HttpClientHandler
        {
            Credentials = credentials
        };
        httpClient = new HttpClient(handler);
    }
    
    var headers = new Dictionary<string, string>
    {
        { "Accept", "*/*" }
    };

    try 
    {
        var currentUrl = initialUrl;
        for (int redirectCount = 0; redirectCount < 4; redirectCount++) // Limit redirects
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, currentUrl);
            
            // Add headers
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // Configure request
            request.Headers.Add("UserAgent", "curl/123");
            request.Properties["AllowUnencryptedAuthentication"] = true;

            var response = await httpClient.SendAsync(request);
            
            // If no redirect, return content
            if (!response.Headers.Location?.OriginalString?.Any() ?? true)
            {
                return await response.Content.ReadAsStringAsync();
            }

            // Update URL for next redirect
            currentUrl = response.Headers.Location.OriginalString;
            Console.WriteLine($"Next location: {currentUrl}");
        }

        throw new Exception("Too many redirects");
    }
    catch (Exception ex)
    {
        throw new Exception($"OIDC redirect failed: {ex.Message}", ex);
    }
}


// Example usage:
var result = await HandleOidcRedirectsAsync(
    "https://your-initial-url.com",
    "your-username",
    "your-password"
);