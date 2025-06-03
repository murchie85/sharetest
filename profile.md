```ps
# Get directories based on their most recent file activity
function recent-files-dirs($count = 5, $path = ".", $depth = 3) {
    Write-Host "Scanning for recent file activity..." -ForegroundColor Yellow
    
    Get-ChildItem -Path $path -Directory -Recurse -Depth $depth | 
        ForEach-Object {
            $latestFile = Get-ChildItem -Path $_.FullName -File -Recurse -Depth 1 -ErrorAction SilentlyContinue | 
                Sort-Object LastWriteTime -Descending | 
                Select-Object -First 1
            
            if ($latestFile) {
                [PSCustomObject]@{
                    Rank = 0
                    Directory = $_.Name
                    Modified = $latestFile.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                    LatestFile = $latestFile.Name
                    Path = Resolve-Path -Path $_.FullName -Relative
                }
            }
        } |
        Sort-Object { [DateTime]::ParseExact($_.Modified, "yyyy-MM-dd HH:mm", $null) } -Descending |
        Select-Object -First $count |
        ForEach-Object -Begin { $i = 1 } -Process { 
            $_.Rank = $i++
            $_
        } |
        Format-Table -AutoSize
}
```