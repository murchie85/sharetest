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


$jsonBody = @'
{
  "Id": "00000000-1111-2222-3333-444444444444",
  "ContainerId": 24,
  "ClientMachine": "my-f5-lab.example.com",
  "StorePath": "Common",
  "CertStoreType": 117,
  "AgentId": "aaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  "Approved": true,
  "CreateIfMissing": false,
  "Properties": {
    "PrimaryNodeOnlineRequired": { "value": "false" },
    "PrimaryNode": { "value": "myPrimaryNode" },
    "PrimaryNodeCheckRetryWaitSecs": { "value": "120" },
    "PrimaryNodeCheckRetryMax": { "value": "3" },
    "F5Version": { "value": "v12" },
    "IgnoreSSLWarning": { "value": "true" },
    "ServerUsername": { "value": { "SecretValue": "myF5User" } },
    "ServerPassword": { "value": { "SecretValue": "myF5Pass" } },
    "ServerUseSSL": { "value": "true" },
    "cyberarkUsername": { "value": { "SecretValue": "myCyberUser" } },
    "cyberarkPassword": { "value": { "SecretValue": "myCyberPass" } }
  }
}
'@





# Then call:
Invoke-RestMethod -Method PUT `
    -Uri "https://<KeyfactorServer>/KeyfactorAPI/CertificateStores" `
    -Headers @{
        "X-Keyfactor-Requested-With" = "APIClient"
        "Content-Type"               = "application/json"
        # Add auth here, e.g. "Authorization" = "Bearer <token>"
    } `
    -Body $jsonBody





################################################################################
# 1) GET the existing store
################################################################################
$ApiUrl   = "https://YourKeyfactor/KeyfactorAPI"
$StoreId  = "00000000-1111-2222-3333-444444444444"

$Headers  = @{
    "X-Keyfactor-Requested-With" = "APIClient"
    "Content-Type"               = "application/json"
    # Add your auth, e.g. "Authorization" = "Bearer <token>" or use -UseDefaultCredentials below
}

$getUrl = "$ApiUrl/CertificateStores/$StoreId"
$originalStore = Invoke-RestMethod -Method GET -Uri $getUrl -Headers $Headers -UseDefaultCredentials

################################################################################
# 2) Convert the existing store's .Properties string -> a PSObject
################################################################################
# Keyfactor in your environment apparently stores .Properties as a single JSON string.
# So parse that string so we can modify specific fields without losing others.

$propsObject = $originalStore.Properties | ConvertFrom-Json

# If .Properties is empty or null, create a new hashtable so we can add fields.
if (-not $propsObject) {
    $propsObject = @{}
}

################################################################################
# 3) Overwrite just the fields you want to change (or add).
#    For secrets, we supply { "value": { "SecretValue": ... } }
#    For booleans or strings, we supply { "value": "true"/"some string" }
################################################################################

# Example: Overwrite "ServerUsername" with a new secret
$propsObject.ServerUsername = @{
    value = @{
        SecretValue = "myF5User"
    }
}

# Example: Overwrite "ServerPassword" with a new secret
$propsObject.ServerPassword = @{
    value = @{
        SecretValue = "myF5Pass"
    }
}

# Example: Overwrite "cyberarkUsername" if needed
$propsObject.cyberarkUsername = @{
    value = @{
        SecretValue = "myCyberArkUser"
    }
}

# Example: Overwrite "cyberarkPassword"
$propsObject.cyberarkPassword = @{
    value = @{
        SecretValue = "myCyberArkPass"
    }
}

# If you also want to overwrite e.g. "ServerUseSsl" (bool = "true"/"false"):
$propsObject.ServerUseSsl = @{
    value = "true"
}

################################################################################
# 4) Re-convert the updated properties object -> an escaped JSON string
################################################################################
$propertiesJsonString = $propsObject | ConvertTo-Json -Depth 10

# This string will look something like:
# "{ \"ServerUsername\": { \"value\": { \"SecretValue\": \"myF5User\" } }, \"ServerPassword\" : ... }"

$originalStore.Properties = $propertiesJsonString

################################################################################
# 5) PUT the entire store object back.
#    Convert the entire store to JSON (which includes Properties as a string),
#    then call the API
################################################################################
$bodyJson = $originalStore | ConvertTo-Json -Depth 10

$putUrl = "$ApiUrl/CertificateStores"
$response = Invoke-RestMethod -Method PUT -Uri $putUrl -Body $bodyJson -Headers $Headers -UseDefaultCredentials

Write-Host "PUT response:"
$response
