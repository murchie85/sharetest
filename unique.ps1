# Define the URL
$csvUrl = "https://example.com/path/to/your.csv"

# Download CSV data
$csvData = Invoke-WebRequest -Uri $csvUrl -UseBasicParsing | Select-Object -ExpandProperty Content

# Convert CSV to PowerShell objects
$csvObjects = $csvData | ConvertFrom-Csv

# Remove blank rows (rows where all columns are empty)
$filteredObjects = $csvObjects | Where-Object { $_."Issued DN" -and ($_."Issued DN" -match '\S') }

# Ensure we have valid rows
if ($filteredObjects.Count -eq 0) {
    Write-Host "No valid data found in CSV after filtering empty rows."
    exit
}

# Group and count occurrences of "Issued DN"
$frequencyCount = $filteredObjects | Group-Object -Property "Issued DN" | Sort-Object Count -Descending

# Select the top 50
$top50 = $frequencyCount | Select-Object -First 50

# Display results
$top50 | Format-Table Name, Count -AutoSize
