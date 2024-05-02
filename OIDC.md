# Variables for OIDC
$clientId = 'your-client-id'
$clientSecret = 'your-client-secret'
$tokenEndpoint = 'https://your-oidc-provider.com/token'

# Basic Auth Header
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($clientId):$($clientSecret)"))

# Get OIDC Token
$tokenResponse = Invoke-RestMethod -Uri $tokenEndpoint -Method Post -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -Body @{grant_type="client_credentials"} -ContentType "application/x-www-form-urlencoded"
$accessToken = $tokenResponse.access_token
2. Use the Token to Access the API
Now that you have the OIDC access token, you can use it to authenticate API requests. Here’s how you might do this with cURL in PowerShell:

powershell
Copy code
# API URL
$apiUrl = 'https://your-api-endpoint.com/data'

# cURL Command in PowerShell
curl -Uri $apiUrl -Headers @{Authorization="Bearer $accessToken"} -Method Get
3. Integrating Kerberos Authentication
If your environment also requires Kerberos authentication (for example, if the API or the OIDC provider uses Kerberos), you’ll need to ensure your Windows credentials are valid and can obtain a Kerberos ticket. PowerShell generally handles this seamlessly if you're on a domain-joined machine and the service supports Kerberos.

Notes:
Ensure your machine is part of the domain and has access to a Key Distribution Center (KDC) when using Kerberos.
Use the klist command in PowerShell to check your Kerberos tickets.
If your API endpoint specifically requires Kerberos, ensure the endpoint supports and advertises it via WWW-Authenticate headers.
Example with Kerberos (if needed):
If the API specifically needs a Kerberos ticket, you might use something like the following, ensuring that the environment is properly set up for Kerberos:

powershell
Copy code
# Ensure Kerberos ticket
klist

# Use curl with Kerberos
curl -Uri $apiUrl -Headers @{Authorization="Bearer $accessToken"} -Method Get --negotiate -u : 
In this setup, the --negotiate -u : part tells cURL to use the Negotiate authentication method, which can include Kerberos.