private async Task<string> HandleOidcRedirectsAsync(string initialUrl, string username, string password, HttpClient httpClient = null)
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetCertificateEntry");
    LogHandlerCommon.Info(logger, CertificateStore, $"username: '{username}'");
    LogHandlerCommon.Info(logger, CertificateStore, $"password: '{password}'");
    LogHandlerCommon.Info(logger, CertificateStore, $"initialUrl: '{initialUrl}'");

    var credentials = new NetworkCredential(username, password);
    
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

    var currentUrl = initialUrl;
    for (int redirectCount = 0; redirectCount < 4; redirectCount++) // Limit redirects
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, currentUrl);

        // Add headers
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        request.Headers.Add("UserAgent", "curl/123");
        request.Properties["AllowUnencryptedAuthentication"] = true;

        var response = await httpClient.SendAsync(request);

        // If no redirect (i.e., Location header missing), check for meta-refresh
        if (!response.Headers.Location?.OriginalString.Any() ?? true)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if it's an HTML page with meta-refresh
            var metaRefreshUrl = ExtractMetaRefreshUrl(responseContent);
            if (!string.IsNullOrEmpty(metaRefreshUrl))
            {
                LogHandlerCommon.Info(logger, CertificateStore, $"Meta refresh detected, new URL: '{metaRefreshUrl}'");
                currentUrl = metaRefreshUrl;
                continue; // Retry with the new URL
            }

            // If no meta-refresh or Location, return content
            return responseContent;
        }

        // Follow standard Location-based redirect
        currentUrl = response.Headers.Location.OriginalString;
        LogHandlerCommon.Info(logger, CertificateStore, $"Next location: '{currentUrl}'");
    }

    throw new Exception("Too many redirects");
}

// Helper method to extract the URL from a meta-refresh tag
private string ExtractMetaRefreshUrl(string htmlContent)
{
    const string metaRefreshTag = "<meta http-equiv=\"refresh\" content=\"";
    var startIndex = htmlContent.IndexOf(metaRefreshTag, StringComparison.OrdinalIgnoreCase);
    if (startIndex == -1) return null;

    startIndex += metaRefreshTag.Length;
    var endIndex = htmlContent.IndexOf("\"", startIndex);
    if (endIndex == -1) return null;

    var refreshContent = htmlContent.Substring(startIndex, endIndex - startIndex);
    var parts = refreshContent.Split(new[] { ";url=" }, StringSplitOptions.RemoveEmptyEntries);
    return parts.Length > 1 ? parts[1].Trim(' ', '"', '\'') : null;
}
