# PowerShell example variables:
$ApiUrl   = "https://YourKeyfactorInstance/KeyfactorAPI"
$StoreId  = "<GUID-of-CertificateStore>"  # e.g. "6d14c2be-76d6-4777-8247-72dcd1c9ab1a"
$Headers  = @{
    # Adjust authentication headers to match your Keyfactor environment
    "X-Keyfactor-Requested-With" = "APIClient"
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







# 1) GET the store into an object
$originalStore = Invoke-RestMethod -Method GET -Uri "$apiUrl/CertificateStores/$storeId" -Headers $headers

# 2) If $originalStore.Properties is a JSON string, parse it.
#    After GET, it might be a simple string like:
#      "Properties": "{\"ServerUseSsl\":\"true\"}"
#    We want it to be a PSObject or Hashtable for easy manipulation.
if ($originalStore.Properties -is [string]) {
    # Convert that JSON string into an object
    $propsObject = $originalStore.Properties | ConvertFrom-Json
} else {
    # It's already an object (some Keyfactor versions return an object directly).
    $propsObject = $originalStore.Properties
}

# 3) Wrap these properties in the { "value": ... } format if needed
#    Because PUT requires them in that shape. 
foreach ($key in $propsObject.PSObject.Properties.Name) {
    $val = $propsObject.$key
    # If not already in the { value = ... } format, fix it
    if ($val -isnot [hashtable] -and $val -isnot [PSCustomObject]) {
        $propsObject.$key = @{ value = $val }
    }
}

# 4) Update the specific property you wanted
#    For example, set 'ServerUseSsl' to "false"
$propsObject.ServerUseSsl.value = "false"

#    Then put the updated properties back into the original store object
$originalStore.Properties = $propsObject

# 5) Convert the entire store to JSON
$bodyJson = $originalStore | ConvertTo-Json -Depth 10

# 6) PUT the entire updated store record
Invoke-RestMethod -Method PUT -Uri "$apiUrl/CertificateStores" -Headers $headers -Body $bodyJson
