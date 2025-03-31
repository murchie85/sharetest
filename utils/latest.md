```c#
/// <summary>
/// Get certificate from store by thumbprint
/// </summary>
/// <param name="thumbprint">The certificate thumbprint</param>
/// <returns>The certificate matching the thumbprint</returns>
private X509Certificate2 GetCertificateFromStore(string thumbprint)
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetCertificateFromStore");
    
    // Open the LocalMachine/My store
    using (X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
    {
        store.Open(OpenFlags.ReadOnly);
        
        // Find the certificate by thumbprint
        X509Certificate2Collection certCollection = store.Certificates.Find(
            X509FindType.FindByThumbprint, 
            thumbprint, 
            false); // Don't validate certificate
        
        if (certCollection.Count == 0)
        {
            LogHandler.Error(logger, CertificateStore, $"Certificate with thumbprint {thumbprint} not found");
            throw new Exception($"Certificate with thumbprint {thumbprint} not found");
        }
        
        LogHandler.Info(logger, CertificateStore, $"Found certificate: {certCollection[0].Subject}");
        LogHandlerCommon.MethodExit(logger, CertificateStore, "GetCertificateFromStore");
        
        return certCollection[0];
    }
}

/// <summary>
/// Get certificate from store by Common Name (CN)
/// </summary>
/// <param name="commonName">The Common Name (CN) to search for</param>
/// <param name="storeLocation">The certificate store location (default: LocalMachine)</param>
/// <returns>The certificate matching the common name</returns>
private X509Certificate2 GetCertificateByCommonName(string commonName, StoreLocation storeLocation = StoreLocation.LocalMachine)
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetCertificateByCommonName");
    
    // Open the specified store location, My (Personal) store
    using (X509Store store = new X509Store(StoreName.My, storeLocation))
    {
        store.Open(OpenFlags.ReadOnly);
        
        // Find certificates by subject name containing the CN
        X509Certificate2Collection certCollection = store.Certificates;
        X509Certificate2Collection result = new X509Certificate2Collection();
        
        // Loop through all certificates to find ones with matching CN
        foreach (X509Certificate2 cert in certCollection)
        {
            // Extract the CN from the subject
            string subject = cert.Subject;
            if (subject.Contains($"CN={commonName}") || 
                subject.Contains($"CN = {commonName}") ||
                subject.Contains($"CN= {commonName}") ||
                subject.Contains($"CN ={commonName}"))
            {
                result.Add(cert);
                LogHandler.Info(logger, CertificateStore, $"Found matching certificate: {cert.Subject}, Thumbprint: {cert.Thumbprint}");
            }
        }
        
        if (result.Count == 0)
        {
            LogHandler.Error(logger, CertificateStore, $"No certificates found with Common Name (CN) containing '{commonName}'");
            throw new Exception($"No certificates found with Common Name (CN) containing '{commonName}'");
        }
        
        // If multiple matches, use the first valid one with a private key
        foreach (X509Certificate2 cert in result)
        {
            if (cert.HasPrivateKey && DateTime.Now < cert.NotAfter && DateTime.Now > cert.NotBefore)
            {
                LogHandler.Info(logger, CertificateStore, $"Selected certificate: {cert.Subject}, Thumbprint: {cert.Thumbprint}");
                LogHandlerCommon.MethodExit(logger, CertificateStore, "GetCertificateByCommonName");
                return cert;
            }
        }
        
        // If no preferred certificate found, return the first one
        LogHandler.Warning(logger, CertificateStore, $"No ideal certificate found. Using first match: {result[0].Subject}");
        LogHandlerCommon.MethodExit(logger, CertificateStore, "GetCertificateByCommonName");
        return result[0];
    }
}
```


```c#
/// <summary>
/// Lists all certificates available to the current user and local machine
/// </summary>
public void ListAvailableCertificates()
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "ListAvailableCertificates");

    // List certificates from the current user store
    ListCertificatesFromStore(StoreLocation.CurrentUser, "Current User Certificates");

    // List certificates from the local machine store
    ListCertificatesFromStore(StoreLocation.LocalMachine, "Local Machine Certificates");

    LogHandlerCommon.MethodExit(logger, CertificateStore, "ListAvailableCertificates");
}

private void ListCertificatesFromStore(StoreLocation location, string storeDescription)
{
    LogHandler.Info(logger, CertificateStore, $"Listing certificates from {storeDescription}");

    // Open different store types that might contain certificates
    string[] storeNames = { "My", "Root", "CA", "TrustedPeople", "TrustedPublisher" };

    foreach (string storeName in storeNames)
    {
        try
        {
            using (X509Store store = new X509Store(storeName, location))
            {
                store.Open(OpenFlags.ReadOnly);
                
                LogHandler.Info(logger, CertificateStore, $"Store: {location}/{storeName} - Found {store.Certificates.Count} certificates");
                
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    LogHandler.Info(logger, CertificateStore, 
                        $"Certificate: Subject={cert.Subject}, " +
                        $"Issuer={cert.Issuer}, " + 
                        $"Thumbprint={cert.Thumbprint}, " +
                        $"HasPrivateKey={cert.HasPrivateKey}, " +
                        $"NotBefore={cert.NotBefore}, " +
                        $"NotAfter={cert.NotAfter}");
                }
            }
        }
        catch (Exception ex)
        {
            LogHandler.Error(logger, CertificateStore, $"Error accessing {location}/{storeName} store: {ex.Message}");
        }
    }
}
```