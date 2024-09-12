```
$url = "https://example.com/api/endpoint"
$formData = @{
    "key1" = "value1"
    "key2" = "value2"
}

# Convert the form data to a URL-encoded string
$formEncoded = [System.Web.HttpUtility]::UrlEncode([string]::Join("&", $formData.GetEnumerator() | ForEach-Object { "$($_.Key)=$($_.Value)" }))


# Send the POST request with the appropriate Content-Type header
$response = Invoke-WebRequest -Uri $url -Method POST -ContentType "application/x-www-form-urlencoded" -Body $formEncoded

# Output the response
$response.Content




$formData = @{
    filename = $filePath
    filedata = $sillystring
}
$body = ($formData.GetEnumerator() | ForEach-Object { "$($_.Key)=$([System.Web.HttpUtility]::UrlEncode($_.Value))" }) -join '&'
```