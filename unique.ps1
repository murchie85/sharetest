# URL of the CSV file
$url = "YOUR_URL_HERE"

# Download the CSV content
$rawContent = Invoke-WebRequest -Uri $url | Select-Object -ExpandProperty Content

# Save raw content to a file for inspection (optional)
# $rawContent | Out-File -FilePath "raw_data.txt"

# Check if content looks like a CSV
Write-Host "First 100 characters of raw content:"
$rawContent.Substring(0, [Math]::Min(100, $rawContent.Length))

# Inspect for potential delimiters
$possibleDelimiters = @(',', ';', "`t", '|')
foreach ($delimiter in $possibleDelimiters) {
    $count = ($rawContent.ToCharArray() | Where-Object { $_ -eq $delimiter[0] }).Count
    Write-Host "Potential delimiter '$delimiter' count: $count"
}

# Try to parse with explicit parameters
$csvData = $rawContent | ConvertFrom-Csv

# Show the count of objects
Write-Host "Total CSV objects: $($csvData.Count)"

# Examine what properties are available in the first few objects
Write-Host "Properties in the first object:"
$csvData[0] | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name

# Let's inspect the first few rows to see the pattern
Write-Host "`nFirst 5 rows data inspection:"
for ($i = 0; $i -lt [Math]::Min(5, $csvData.Count); $i++) {
    Write-Host "`nRow $i property values:"
    $row = $csvData[$i]
    $properties = $row | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
    foreach ($prop in $properties) {
        $value = $row.$prop
        $valuePresent = if ([string]::IsNullOrWhiteSpace($value)) { "EMPTY" } else { "PRESENT" }
        Write-Host "  $prop`: $valuePresent"
    }
}

# Attempt to find the "Issued DN" property with case-insensitive search
$issuedDnProperty = $csvData[0] | Get-Member -MemberType NoteProperty | 
                    Where-Object { $_.Name -like "*issued*dn*" } | 
                    Select-Object -First 1 -ExpandProperty Name

if ($issuedDnProperty) {
    Write-Host "`nFound property that might be 'Issued DN': $issuedDnProperty"
    
    # Filter non-empty rows based on the discovered property
    $csvDataFiltered = $csvData | Where-Object { 
        $_ -ne $null -and $_.$issuedDnProperty -ne $null -and $_.$issuedDnProperty -ne "" 
    }
    
    # Count filtered results
    Write-Host "Filtered rows count: $($csvDataFiltered.Count)"
    
    # Process top 20 if we have filtered data
    if ($csvDataFiltered.Count -gt 0) {
        # Group by found property, count occurrences, and sort by count in descending order
        $topIssuedDNs = $csvDataFiltered | 
            Group-Object -Property $issuedDnProperty | 
            Select-Object Name, Count | 
            Sort-Object -Property Count -Descending | 
            Select-Object -First 20
        
        # Display the results
        Write-Host "`nTop 20 most common values:"
        $topIssuedDNs | Format-Table -AutoSize
    }
    else {
        Write-Host "No valid rows found after filtering."
    }
}
else {
    Write-Host "Could not find a property matching 'Issued DN'. Available properties:"
    $csvData[0] | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
}