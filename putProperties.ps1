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





################################################################################
# Example Variables
################################################################################
$apiUrl          = "https://YourKeyfactor/KeyfactorAPI"
$storeId         = "11111111-2222-3333-4444-555555555555"  # The store's GUID
$serverUsername  = "myServerUser"
$serverPword     = "myServerPword"
$cyberarkUname   = "myCyberArkUser"
$cyberarkPword   = "myCyberArkPass"

# Authentication headers
$headers = @{
    "X-Keyfactor-Requested-With" = "APIClient"
    "Content-Type"               = "application/json"
    # Add whatever auth is needed, e.g. Basic or Bearer:
    # "Authorization"              = "Bearer <token>"
}

################################################################################
# 1) GET the existing store
################################################################################
$getUrl        = "$apiUrl/CertificateStores/$storeId"
$originalStore = Invoke-RestMethod -Method GET -Uri $getUrl -Headers $headers -UseDefaultCredentials

################################################################################
# 2) Convert the store's Properties from JSON (string) -> PSObject
################################################################################
$propsObject = $originalStore.Properties | ConvertFrom-Json

################################################################################
# 3) Wrap any simple string/bool properties in { value = ... } if needed
################################################################################
foreach ($key in $propsObject.PSObject.Properties.Name) {
    $val = $propsObject.$key
    # If the property is just a string or bool, wrap it
    if ($val -isnot [System.Collections.Hashtable] -and $val -isnot [PSCustomObject]) {
        $propsObject.$key = @{ value = $val }
    }
}

################################################################################
# 4) Override the four credential fields with secret objects
################################################################################
# ServerUsername
$propsObject.ServerUsername = @{
    value = @{
        SecretValue = $serverUsername
    }
}

# ServerPassword
$propsObject.ServerPassword = @{
    value = @{
        SecretValue = $serverPword
    }
}

# cyberarkUsername
$propsObject.cyberarkUsername = @{
    value = @{
        SecretValue = $cyberarkUname
    }
}

# cyberarkPassword
$propsObject.cyberarkPassword = @{
    value = @{
        SecretValue = $cyberarkPword
    }
}

################################################################################
# 5) Put the updated properties back in the store object
################################################################################
$originalStore.Properties = $propsObject

################################################################################
# 6) Convert the entire store to JSON and PUT
################################################################################
$bodyJson = $originalStore | ConvertTo-Json -Depth 10

$putUrl = "$apiUrl/CertificateStores"
$updatedStore = Invoke-RestMethod -Method PUT -Uri $putUrl `
    -Body $bodyJson -Headers $headers -UseDefaultCredentials

Write-Host "Updated store result:"
$updatedStore
