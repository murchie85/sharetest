# Define the URL
$csvUrl = "https://example.com/path/to/your.csv"

# Download CSV data
$csvData = Invoke-WebRequest -Uri $csvUrl -UseBasicParsing | Select-Object -ExpandProperty Content

# Convert CSV to PowerShell objects
$csvObjects = $csvData | ConvertFrom-Csv

# Ensure the "Issued DN" column exists
if ($csvObjects -and $csvObjects.PSObject.Properties.Name -contains "Issued DN") {
    # Count occurrences of unique "Issued DN" values
    $frequencyCount = $csvObjects | Group-Object -Property "Issued DN" | Sort-Object Count -Descending

    # Select the top 50 most frequent occurrences
    $top50 = $frequencyCount | Select-Object -First 50

    # Display results in a readable table
    $top50 | Format-Table Name, Count -AutoSize
} else {
    Write-Host "Column 'Issued DN' not found in CSV."
}
