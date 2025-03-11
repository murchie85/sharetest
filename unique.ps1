# URL of the CSV file
$url = "YOUR_URL_HERE"

# Download the CSV content
$csvContent = Invoke-WebRequest -Uri $url | Select-Object -ExpandProperty Content

# Convert the CSV content to objects
$csvData = $csvContent | ConvertFrom-Csv

# Filter out empty rows and check the data
$csvDataFiltered = $csvData | Where-Object { 
    # Check if the "Issued DN" property has a value
    $_."Issued DN" -ne $null -and $_."Issued DN" -ne "" 
}

# Output sample to confirm data structure
Write-Host "First 3 rows after filtering:"
$csvDataFiltered | Select-Object -First 3 | Format-Table

# Count total rows for verification
Write-Host "Total filtered rows: $($csvDataFiltered.Count)"

# Group by "Issued DN" column, count occurrences, and sort by count in descending order
$topIssuedDNs = $csvDataFiltered | 
    Group-Object -Property "Issued DN" | 
    Select-Object Name, Count | 
    Sort-Object -Property Count -Descending | 
    Select-Object -First 20

# Display the results
$topIssuedDNs | Format-Table -AutoSize