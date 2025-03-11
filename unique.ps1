# Define the URL
$csvUrl = "https://example.com/path/to/your.csv"

# Download CSV data
$csvData = Invoke-WebRequest -Uri $csvUrl -UseBasicParsing | Select-Object -ExpandProperty Content

# Convert CSV to PowerShell objects
$csvObjects = $csvData | ConvertFrom-Csv

# Count occurrences of unique "Issued DN" values
$frequencyCount = $csvObjects | Group-Object -Property "Issued DN" | Sort-Object Count -Descending

# Select the top 50 most frequent occurrences
$top50 = $frequencyCount | Select-Object -First 50

# Display results in a table format
$top50 | Format-Table Name, Count -AutoSize
