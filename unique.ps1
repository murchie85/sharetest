# URL of the CSV file
$url = "YOUR_URL_HERE"

# Download the raw content
$rawContent = (Invoke-WebRequest -Uri $url).Content

# Split into lines
$lines = $rawContent -split "`r?`n"

# Find the header line
$headerLine = $lines[0]
Write-Host "Header line: $headerLine"

# Parse headers
$headers = $headerLine -split ','
for ($i = 0; $i -lt $headers.Count; $i++) {
    $headers[$i] = $headers[$i].Trim('"').Trim()
    Write-Host "Header[$i]: '$($headers[$i])'"
}

# Find the index of "Issued DN" or similar
$issuedDnIndex = -1
for ($i = 0; $i -lt $headers.Count; $i++) {
    if ($headers[$i] -eq "Issued DN" -or $headers[$i] -like "*Issued*DN*") {
        $issuedDnIndex = $i
        Write-Host "Found 'Issued DN' at index $i: '$($headers[$i])'"
        break
    }
}

if ($issuedDnIndex -ge 0) {
    # Process data rows (skip header)
    $issuedDnValues = @()
    
    for ($lineIndex = 1; $lineIndex -lt $lines.Count; $lineIndex++) {
        $line = $lines[$lineIndex]
        if ($line.Trim() -ne "") {
            # Simple CSV parsing (doesn't handle quoted commas correctly but works for basic CSV)
            $fields = $line -split ','
            
            if ($fields.Count -gt $issuedDnIndex) {
                $value = $fields[$issuedDnIndex].Trim('"').Trim()
                if ($value -ne "") {
                    $issuedDnValues += $value
                }
            }
        }
    }
    
    Write-Host "Found $($issuedDnValues.Count) non-empty 'Issued DN' values"
    
    # Count occurrences and get top 20
    $topValues = $issuedDnValues | Group-Object | Sort-Object -Property Count -Descending | Select-Object -First 20
    
    # Display results
    $topValues | Format-Table -Property Name, Count -AutoSize
}
else {
    Write-Host "Could not find 'Issued DN' column in headers"
}