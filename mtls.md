# Create a self-signed certificate
$Cert = New-SelfSignedCertificate -DnsName "keyfactor" -CertStoreLocation Cert:\CurrentUser\My -KeyExportPolicy Exportable

# Export the certificate (without private key) to share with the server (optional)
Export-Certificate -Cert $Cert -FilePath "ms.crt"

# Export the certificate with the private key (PFX) for your use
$Password = ConvertTo-SecureString -String "test" -Force -AsPlainText
Export-PfxCertificate -Cert $Cert -FilePath "ms.pfx" -Password $Password
