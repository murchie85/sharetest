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
$apiUrl             = "https://YourKeyfactor/KeyfactorAPI"
$storeId            = "11111111-2222-3333-4444-555555555555"  # The store's GUID
$serverUsername     = "myServerUser"
$serverPword        = "myServerPassword"
$cyberarkUname      = "myCyberArkUser"
$cyberarkPword      = "myCyberArkPassword"

# Authentication headers
$headers = @{
    "X-Keyfactor-Requested-With" = "APIClient"
    "Content-Type"               = "application/json"
    # Add whatever auth is needed, e.g. Basic or Bearer
}

################################################################################
# 1) GET the existing store
################################################################################
$getUrl        = "$apiUrl/CertificateStores/$storeId"
$originalStore = Invoke-RestMethod -Method GET -Uri $getUrl -Headers $headers -UseDefaultCredentials

################################################################################
# 2) Convert the store's 'Properties' from JSON (string) -> PSObject
################################################################################
$propsObject = $originalStore.Properties | ConvertFrom-Json

################################################################################
# 3) Overwrite each property according to its type
#
#   - BOOLEANS: "true" or "false" strings under { "value": "..." }
#   - STRINGS, MULTIPLE CHOICE: also pass them as { "value": "<string>" }
#   - SECRETS: pass as { "value": { "SecretValue": "<secret>" } }
################################################################################

# "Primary Node Online Required" (Bool)
$propsObject.'Primary Node Online Required' = @{ value = "false" }

# "Primary Node" (String)
$propsObject.'Primary Node' = @{ value = "my-primary-node" }

# "Primary Node Check Retry Wait Secs" (String)
$propsObject.'Primary Node Check Retry Wait Secs' = @{ value = "120" }

# "Primary Node Check Retry Max" (String)
$propsObject.'Primary Node Check Retry Max' = @{ value = "3" }

# "Version of F5" (MultipleChoice) - supply one valid choice, e.g. "v12" or "v13"
$propsObject.'Version of F5' = @{ value = "v12" }

# "Ignore SSL Warning" (Bool)
$propsObject.'Ignore SSL Warning' = @{ value = "true" }

# "Server Username" (Secret)
$propsObject.'Server Username' = @{
    value = @{
        SecretValue = $serverUsername
    }
}

# "Server Password" (Secret)
$propsObject.'Server Password' = @{
    value = @{
        SecretValue = $serverPword
    }
}

# "Use SSL" (Bool)
$propsObject.'Use SSL' = @{ value = "true" }

# "cyberark Username" (Secret)
$propsObject.'cyberark Username' = @{
    value = @{
        SecretValue = $cyberarkUname
    }
}

# "cyberark Password" (Secret)
$propsObject.'cyberark Password' = @{
    value = @{
        SecretValue = $cyberarkPword
    }
}

################################################################################
# 4) Put the updated 'Properties' back into the store object
################################################################################
$originalStore.Properties = $propsObject

################################################################################
# 5) Convert the entire store to JSON and PUT
################################################################################
$bodyJson = $originalStore | ConvertTo-Json -Depth 10

$putUrl = "$apiUrl/CertificateStores"
$updatedStore = Invoke-RestMethod -Method PUT -Uri $putUrl `
    -Body $bodyJson -Headers $headers -UseDefaultCredentials

Write-Host "Updated store result:"
$updatedStore




{
  "Id": "00000000-1111-2222-3333-444444444444",
  "ClientMachine": "your-host-here",
  "StorePath": "Common",
  "CertStoreType": 117,
  "AgentId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "Approved": true,
  "CreateIfMissing": false,
  "Properties": {
    "PrimaryNodeCheckRetryMax": {
      "value": "4" 
    }
  }
}
