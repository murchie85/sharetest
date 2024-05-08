```powershell
# Set client ID and secret
$CLIENT_ID = 'your_client_id'
$CLIENT_SECRET = 'your_client_secret'

# Encode the client ID and secret into Base64 to prepare for HTTP Basic Authentication
$credentials = "$CLIENT_ID:$CLIENT_SECRET"
$encodedCredentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($credentials))

# Create headers with Basic Authentication
$headers = @{ Authorization = "Basic $encodedCredentials" }

# Data to be sent in the body of the POST request
$body = @{
    scope = 'openid'
    grant_type = 'client_credentials'
}

# Endpoint URL
$url = 'https://endpoint...'

# Send the POST request using Invoke-WebRequest
$response = Invoke-WebRequest -Uri $url -Method Post -Body $body -Headers $headers -ContentType 'application/x-www-form-urlencoded'

# Display the response content
$response.Content

```
