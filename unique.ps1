# URL of the CSV file
$url = "YOUR_URL_HERE"

# Download the CSV content
$csvContent = Invoke-WebRequest -Uri $url | Select-Object -ExpandProperty Content

# Convert the CSV content to objects
$csvData = $csvContent | ConvertFrom-Csv

# Group by "Issued DN" column, count occurrences, and sort by count in descending order
$topIssuedDNs = $csvData | 
    Group-Object -Property "Issued DN" | 
    Select-Object Name, Count | 
    Sort-Object -Property Count -Descending | 
    Select-Object -First 20

# Display the results
$topIssuedDNs | Format-Table -AutoSize