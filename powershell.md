```powershell
# Set client ID and secret
$CLIENT_ID = 'your_client_id'
$CLIENT_SECRET = 'your_client_secret'

# Base64 encode the client ID and secret
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$CLIENT_ID:$CLIENT_SECRET"))

# Create headers with Basic Authentication
$headers = @{ Authorization = "Basic $base64AuthInfo" }

# Data to be sent in the body of the POST request
$body = @{ 
    scope = 'openid'
    grant_type = 'client_credentials'
}

# Endpoint URL
$url = 'https://endpoint...'

# Send the POST request
$response = Invoke-WebRequest -Uri $url -Method Post -Body $body -Headers $headers -ContentType 'application/x-www-form-urlencoded'

# Display the response
$response.Content
```
