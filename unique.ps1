# Define the URL
$csvUrl = "https://example.com/path/to/your.csv"

# Download CSV data
$csvData = Invoke-WebRequest -Uri $csvUrl -UseBasicParsing | Select-Object -ExpandProperty Content

# Convert CSV to PowerShell objects
$csvObjects = $csvData | ConvertFrom-Csv

# Count frequency of "Issued DN" values
$frequencyCount = $csvObjects | Group-Object -Property "Issued DN" | Sort-Object -Property Count -Descending

# Display the top 50
$frequencyCount | Select-Object -First 50
