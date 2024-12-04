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