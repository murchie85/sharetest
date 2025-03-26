# List certificates in the Current User store
Write-Host "Current User Certificate Stores:" -ForegroundColor Green
Get-ChildItem Cert:\CurrentUser -Recurse | Where-Object { $_.PSIsContainer -eq $false } | 
    Select-Object FriendlyName, Subject, Issuer, NotBefore, NotAfter, Thumbprint |
    Format-Table -AutoSize

# List certificates in the Local Machine store
Write-Host "`nLocal Machine Certificate Stores:" -ForegroundColor Green
Get-ChildItem Cert:\LocalMachine -Recurse | Where-Object { $_.PSIsContainer -eq $false } | 
    Select-Object FriendlyName, Subject, Issuer, NotBefore, NotAfter, Thumbprint |
    Format-Table -AutoSize