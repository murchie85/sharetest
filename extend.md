```c#
private void ExtendTokenTimeout(string token, int timeoutSeconds)
{
    try
    {
        LogHandlerCommon.Info(logger, CertificateStore, $"Extending token lifetime to {timeoutSeconds} seconds");
        
        // Create a temporary REST handler that uses basic auth instead of token auth
        RESTHandler tempREST = new RESTHandler(CertificateStore.ClientMachine, ServerUserName, ServerPassword, UseSSL, IgnoreSSLWarning);
        // Don't set a token on this handler
        
        var extendRequest = new { timeout = timeoutSeconds };
        tempREST.Patch($"/mgmt/shared/authz/tokens/{token}", 
                      JsonConvert.SerializeObject(extendRequest));
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"Error extending token timeout: {ex.Message}");
        // Continue with the current token even if extension fails
    }
}
```