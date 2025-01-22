# PowerShell example variables:
$ApiUrl   = "https://YourKeyfactorInstance/KeyfactorAPI"
$StoreId  = "<GUID-of-CertificateStore>"  # e.g. "6d14c2be-76d6-4777-8247-72dcd1c9ab1a"
$Headers  = @{
    # Adjust authentication headers to match your Keyfactor environment
    "X-Certificate"  = "<Your-Keyfactor-API-Secret-Or-Token>"
    "Content-Type"   = "application/json"
}

# 1) GET the existing certificate store (all fields)
$getUrl = "$ApiUrl/CertificateStores/$StoreId"
$originalStore = Invoke-RestMethod -Method GET -Uri $getUrl -Headers $Headers

# $originalStore now contains the store’s data as returned by the GET endpoint.




$propsAsJson = $originalStore.Properties | ConvertFrom-Json

$putUrl = "$ApiUrl/CertificateStores"

# Convert $originalStore to JSON. 
# Make sure -Depth is large enough to handle nested objects.
$bodyJson = $propsAsJson | ConvertTo-Json -Depth 10

$updatedStore = Invoke-RestMethod -Method PUT -Uri $putUrl -Body $bodyJson -Headers $Headers