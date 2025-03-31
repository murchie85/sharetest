# Define the API endpoint
$ApiUrl = "https://your-api-endpoint.com/endpoint"

# Make the API call and store the response
$response = Invoke-RestMethod -Uri $ApiUrl -Method Get

# Initialize an array to store processed data
$ProcessedData = @()

# Loop through the list of dictionaries in the response
foreach ($item in $response) {
    # Extract the id and metadata
    $id = $item.id
    $eonId = if ($item.metadata.EON_ID) { $item.metadata.EON_ID } else { $null }

    # Create a custom object to store the extracted data
    $ProcessedData += [PSCustomObject]@{
        ID = $id
        EON_ID = $eonId
    }
}

# Define the CSV file path
$CsvFilePath = "C:\\path\\to\\output.csv"

# Export the processed data to a CSV file
$ProcessedData | Export-Csv -Path $CsvFilePath -NoTypeInformation -Force

Write-Output "Data has been exported to $CsvFilePath"
