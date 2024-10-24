using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public string GetFinalContent(string uri, string username, string password, string clientMachine, string path)
{
    string finalContent = null;
    string fullUri = uri + clientMachine + "/getcertificateentry?path=" + path;

    // Initialize handlers with separate CookieContainers to mimic $s and $s2
    var handler1 = new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseProxy = false,
        CookieContainer = new CookieContainer(),
        UseCookies = true
    };

    var handler2 = new HttpClientHandler
    {
        AllowAutoRedirect = false,
        UseProxy = false,
        CookieContainer = new CookieContainer(),
        UseCookies = true
    };

    // Create separate HttpClient instances for different sessions
    using (var client1 = new HttpClient(handler1))
    using (var client2 = new HttpClient(handler2))
    {
        // Set common headers
        string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

        // First Request - Store session in handler1 (equivalent to $s)
        try
        {
            LogHandlerCommon.Info(logger, CertificateStore, $"Starting first request to URI: {fullUri}");
            var request1 = new HttpRequestMessage(HttpMethod.Get, fullUri);
            request1.Headers.UserAgent.ParseAdd("curl/123");
            request1.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            request1.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response1 = client1.SendAsync(request1).Result;
            var nextUrl1 = response1.Headers.Location?.ToString();
            LogHandlerCommon.Info(logger, CertificateStore, $"Next URL after first request: {nextUrl1}");

            // Second Request - Store session in handler2 (equivalent to $s2)
            if (!string.IsNullOrEmpty(nextUrl1))
            {
                LogHandlerCommon.Info(logger, CertificateStore, $"Starting second request to URL: {nextUrl1}");
                var request2 = new HttpRequestMessage(HttpMethod.Get, nextUrl1);
                request2.Headers.UserAgent.ParseAdd("curl/123");
                request2.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                request2.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var response2 = client2.SendAsync(request2).Result;
                var nextUrl2 = response2.Headers.Location?.ToString();
                LogHandlerCommon.Info(logger, CertificateStore, $"Next URL after second request: {nextUrl2}");

                // Third Request - Use session from handler2 (equivalent to $s2)
                if (!string.IsNullOrEmpty(nextUrl2))
                {
                    LogHandlerCommon.Info(logger, CertificateStore, $"Starting third request to URL: {nextUrl2}");
                    var request3 = new HttpRequestMessage(HttpMethod.Get, nextUrl2);
                    request3.Headers.UserAgent.ParseAdd("curl/123");
                    request3.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    request3.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                    // Use client2 to maintain the session
                    var response3 = client2.SendAsync(request3).Result;
                    var returnUrl = response3.Headers.Location?.ToString();
                    LogHandlerCommon.Info(logger, CertificateStore, $"Return URL after third request: {returnUrl}");

                    // Fourth Request - Use session from handler1 (equivalent to $s)
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        LogHandlerCommon.Info(logger, CertificateStore, $"Starting fourth request to URL: {returnUrl}");
                        var request4 = new HttpRequestMessage(HttpMethod.Get, returnUrl);
                        request4.Headers.UserAgent.ParseAdd("curl/123");
                        request4.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                        request4.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                        // Use client1 to maintain the session from the first request
                        var response4 = client1.SendAsync(request4).Result;
                        finalContent = response4.Content.ReadAsStringAsync().Result.Trim('"');
                        LogHandlerCommon.Info(logger, CertificateStore, $"Final content retrieved.");
                    }
                    else
                    {
                        LogHandlerCommon.Info(logger, CertificateStore, "Return URL is null after third request.");
                    }
                }
                else
                {
                    LogHandlerCommon.Info(logger, CertificateStore, "Next URL is null after second request.");
                }
            }
            else
            {
                LogHandlerCommon.Info(logger, CertificateStore, "Next URL is null after first request.");
            }
        }
        catch (Exception ex)
        {
            LogHandlerCommon.Info(logger, CertificateStore, $"Exception occurred: {ex.Message}");
        }
    }

    return finalContent;
}
