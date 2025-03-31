$storeId = "your-store-guid"  # Replace with actual store ID
$url = "https://keyfactor.../CertificateStores"

$headers = @{
    "Accept" = "application/json"
    "Content-Type" = "application/json"
}

# First, let's get the existing store data so we don't lose anything
try {
    $existingStore = Invoke-RestMethod -Uri "$url/$storeId" -Method Get -Headers $headers

    # Now create our PUT request with all required fields plus our test property
    $body = @{
        Id = $storeId
        ClientMachine = $existingStore.ClientMachine
        Storepath = $existingStore.Storepath
        CertStoreType = $existingStore.CertStoreType
        AgentId = $existingStore.AgentId
        Properties = "{\"TestProperty\":{\"value\":\"test123\"}}"
        # Include other existing fields to prevent data loss
        ContainerId = $existingStore.ContainerId
        Approved = $existingStore.Approved
        CreateIfMissing = $existingStore.CreateIfMissing
        AgentAssigned = $existingStore.AgentAssigned
    }

    $json = $body | ConvertTo-Json

    # Make the PUT request
    $response = Invoke-RestMethod -Uri $url -Method Put -Headers $headers -Body $json
    $response | ConvertTo-Json -Depth 10
}
catch {
    Write-Host "Error occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message
    Write-Host "Response:" $_.Exception.Response
}