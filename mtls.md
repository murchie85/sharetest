
# connecting to api with certt 

```c#
public string MakeApiCall(string url, string certPath, string certPassword, ILogger logger, string certificateStore)
{
    LogHandlerCommon.MethodEntry(logger, certificateStore, "MakeApiCall");
    LogHandlerCommon.Info(logger, certificateStore, $"Loading certificate from path: '{certPath}'");

    try
    {
        // Load the certificate
        var certificate = new X509Certificate2(
            certPath,
            certPassword,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable
        );
        
        LogHandlerCommon.Info(logger, certificateStore, $"Certificate loaded successfully. Thumbprint: '{certificate.Thumbprint}'");

        // Create handler with certificate
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);
        
        LogHandlerCommon.Info(logger, certificateStore, $"Making API call to: '{url}'");

        // Create client and make request
        using (var client = new HttpClient(handler))
        {
            // Make the request and wait for result
            var response = client.GetAsync(url).Result;
            
            LogHandlerCommon.Info(logger, certificateStore, 
                $"Received response. Status: {response.StatusCode}, ReasonPhrase: '{response.ReasonPhrase}'");
            
            // Ensure we got success status code
            response.EnsureSuccessStatusCode();
            
            var content = response.Content.ReadAsStringAsync().Result;
            LogHandlerCommon.Info(logger, certificateStore, "Successfully read response content");
            
            return content;
        }
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Info(logger, certificateStore, $"Error in MakeApiCall: {ex.Message}");
        if (ex.InnerException != null)
        {
            LogHandlerCommon.Info(logger, certificateStore, $"Inner Exception: {ex.InnerException.Message}");
        }
        throw; // Re-throw to maintain original stack trace
    }
}
```



ls
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12



```ps
try {
    $response = Invoke-WebRequest -Uri "https://your-dev-server-url/whoami" -Certificate $cert -Verbose
    $response.Content
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Inner Error: $($_.Exception.InnerException.Message)"
}

```


# Create a self-signed certificate
$Cert = New-SelfSignedCertificate -DnsName "keyfactor" -CertStoreLocation Cert:\CurrentUser\My -KeyExportPolicy Exportable

# Export the certificate (without private key) to share with the server (optional)
Export-Certificate -Cert $Cert -FilePath "ms.crt"

# Export the certificate with the private key (PFX) for your use
$Password = ConvertTo-SecureString -String "test" -Force -AsPlainText
Export-PfxCertificate -Cert $Cert -FilePath "ms.pfx" -Password $Password




$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" | Where-Object { $_.Subject -like "*adam.mcmurchie@mycompany.com*" }
$cert | Format-List Subject, HasPrivateKey, NotAfter, EnhancedKeyUsageList

# List all certs with their full details
Get-ChildItem -Path "Cert:\CurrentUser\My" | Select-Object Subject, Thumbprint, NotAfter | Format-Table -AutoSize


# Once we have the thumbprint, we can get the cert directly like this:
$cert = Get-ChildItem -Path "Cert:\CurrentUser\My" | Where-Object { $_.Thumbprint -eq "PASTE_THUMBPRINT_HERE" }




#====CERT AND KEY

```ps
# First load both files
$certPath = "path\to\companyname.cert"
$keyPath = "path\to\companyname.key"

# Create an X509Certificate2 object with both cert and key
$cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($certPath)

try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    $response = Invoke-WebRequest -Uri "https://your-dev-server-url/whoami" `
        -Certificate $cert `
        -Verbose
    $response.Content
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Inner Error: $($_.Exception.InnerException.Message)"
}

```

create pfx from cert and key file 

```ps

# Set a password for the PFX (you'll need this for import)
$pfxPassword = "test!"
$securePassword = ConvertTo-SecureString -String $pfxPassword -Force -AsPlainText

# Create PFX using OpenSSL (run this from the directory with your files)
openssl pkcs12 -export -out client.pfx -inkey companyname.key -in companyname.cert -password pass:$pfxPassword



# Import the PFX
$cert = Import-PfxCertificate -FilePath "client.pfx" -CertStoreLocation Cert:\CurrentUser\My -Password $securePassword

# Show the thumbprint (save this for later use)
Write-Host "Certificate Thumbprint: $($cert.Thumbprint)"


$certThumbprint = "PASTE_THUMBPRINT_HERE"
$cert = Get-ChildItem -Path "Cert:\CurrentUser\My\$certThumbprint"

# Make the request
try {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    $response = Invoke-WebRequest -Uri "https://your-dev-server-url/whoami" -Certificate $cert -Verbose
    $response.Content
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Inner Error: $($_.Exception.InnerException.Message)"
}

```