# API endpoint
$url = "https://keyfactor....CertificateStores/Schedule"  # Replace with your full URL

# Request headers
$headers = @{
    "Accept" = "application/json, text/plain, */*"
    "Content-Type" = "application/json"
    # You'll likely need an authorization header - let me know if you need help with this
}

# Request body
$body = @{
    StoreIds = @("xxx-xxx-xxx")  # Replace with your actual store ID
    Schedule = @{
        Immediate = $true
    }
} | ConvertTo-Json

# Make the API call
try {
    $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $body
    Write-Host "Success! Response:" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    Write-Host "Response:" $_.Exception.Response
}