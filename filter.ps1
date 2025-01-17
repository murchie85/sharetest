# Import the necessary module for working with Excel
Import-Module -Name ImportExcel -ErrorAction Stop

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

# Define the Excel file path
$ExcelFilePath = "C:\path\to\output.xlsx"

# Export the processed data to an Excel file
$ProcessedData | Export-Excel -Path $ExcelFilePath -AutoSize

Write-Output "Data has been exported to $ExcelFilePath"