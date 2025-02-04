# Create the Properties hashtable first
$properties = @{
    "PrimaryNode" = ""
    "PrimaryNodeCheckRetryWaitSecs" = ""
    "PrimaryNodeCheckRetryMax" = ""
    "F5Version" = "$version"
    "PrimaryNodeOnlineRequired" = "false"
    "IgnoreSSLWarning" = "true"
    "ServerUsername" = "$target_kf_proid"
    "ServerPassword" = "$passwd"
    "ServerUseSSL" = "true"
    "cyberarkUsername" = "your_username"
    "cyberarkPassword" = "your_password"
}

# Create the ReenrollmentStatus hashtable
$reenrollmentStatus = @{
    "Data" = $false
    "AgentId" = $null
    "Message" = "Key generation is not supported by the agent for this store."
    "JobProperties" = $null
    "CustomAliasAllowed" = 0
    "EntryParameters" = $null
}

# Create the InventorySchedule hashtable
$inventorySchedule = @{
    "Immediate" = $false
}

# Create the main request body
$requestBody = @{
    "ContainerId" = $container["Id"]
    "ClientMachine" = $hostname
    "Storepath" = "Common"
    "CertStoreType" = $certStoreTypeID
    "Approved" = $true
    "CreateIfMissing" = $true
    "Properties" = $properties | ConvertTo-Json -Compress
    "AgentId" = $agentid
    "AgentAssigned" = $true
    "ContainerName" = $container["Name"]
    "InventorySchedule" = $inventorySchedule
    "ReenrollmentStatus" = $reenrollmentStatus
}

# Convert to JSON
$jsonBody = $requestBody | ConvertTo-Json -Depth 10

# Create the headers
$headers = @{
    "Accept" = "application/json"
    "Content-Type" = "application/json"
    "X-Keyfactor-Api-Version" = "1"
    "X-KeyFactor-Requested-With" = "APIClient"
}

# Make the request
$deployResult = Invoke-RestMethod -Uri "$target_kf/CertificateStores" `
    -Method Post `
    -Body $jsonBody `
    -Headers $headers `
    -Authentication $session.auth `
    -WebSession $session