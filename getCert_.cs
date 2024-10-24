using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public void GetFinalContent(string uri, string username, string password, string clientMachine, string path)
{
    string fullUri = uri + clientMachine + "/getcertificateentry?path=" + path;

    // Initialize the HTTP handler with required settings
    var handler = new HttpClientHandler
    {
        AllowAutoRedirect = false, // Prevent automatic redirection
        UseProxy = false,          // Disable proxy usage
        CookieContainer = new CookieContainer(),
        UseCookies = true
    };

    using (var client = new HttpClient(handler))
    {
        // Set headers
        client.DefaultRequestHeaders.UserAgent.ParseAdd("curl/123");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        // Add Basic Authentication header
        string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        try
        {
            LogHandlerCommon.Info(logger, CertificateStore, $"Starting first request to URI: {uri}");
            var response = client.GetAsync(uri).Result;

            var next = response.Headers.Location?.ToString();
            LogHandlerCommon.Info(logger, CertificateStore, $"Next URL after first request: {next}");

            // Second request
            if (!string.IsNullOrEmpty(next))
            {
                LogHandlerCommon.Info(logger, CertificateStore, $"Starting second request to URL: {next}");
                var response2 = client.GetAsync(next).Result;

                var next2 = response2.Headers.Location?.ToString();
                LogHandlerCommon.Info(logger, CertificateStore, $"Next URL after second request: {next2}");

                // Third request
                if (!string.IsNullOrEmpty(next2))
                {
                    LogHandlerCommon.Info(logger, CertificateStore, $"Starting third request to URL: {next2}");
                    var response3 = client.GetAsync(next2).Result;

                    var returnUrl = response3.Headers.Location?.ToString();
                    LogHandlerCommon.Info(logger, CertificateStore, $"Return URL after third request: {returnUrl}");

                    // Fourth request
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        LogHandlerCommon.Info(logger, CertificateStore, $"Starting fourth request to URL: {returnUrl}");
                        var response4 = client.GetAsync(returnUrl).Result;

                        var finalContent = response4.Content.ReadAsStringAsync().Result.Trim('"');
                        LogHandlerCommon.Info(logger, CertificateStore, $"Final content: {finalContent}");
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
}
