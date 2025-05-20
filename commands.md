```ps1
# Find files containing specific text (like grep)
function Find-TextInFiles {
    param(
        [Parameter(Mandatory=$true)][string]$Pattern,
        [string]$Path = ".",
        [string]$Filter = "*.*"
    )
    Get-ChildItem -Path $Path -Filter $Filter -Recurse | Select-String -Pattern $Pattern -List | Select-Object Path,LineNumber,Line
}
Set-Alias -Name ftf -Value Find-TextInFiles

# Quick system information dashboard
function Get-SystemInfo {
    $os = Get-CimInstance Win32_OperatingSystem
    $proc = Get-CimInstance Win32_Processor
    $mem = Get-CimInstance Win32_PhysicalMemory | Measure-Object -Property Capacity -Sum
    $disk = Get-CimInstance Win32_LogicalDisk -Filter "DeviceID='C:'" | Select-Object Size,FreeSpace
    
    [PSCustomObject]@{
        'OS' = $os.Caption
        'OS Version' = $os.Version
        'CPU' = $proc.Name
        'RAM (GB)' = [math]::Round($mem.Sum / 1GB, 2)
        'Disk Size (GB)' = [math]::Round($disk.Size / 1GB, 2)
        'Free Space (GB)' = [math]::Round($disk.FreeSpace / 1GB, 2)
        'Uptime' = (Get-Date) - $os.LastBootUpTime
    }
}
Set-Alias -Name sysinfo -Value Get-SystemInfo

# Super quick directory size calculation
function Get-DirectorySize {
    param([string]$Path = ".")
    
    $size = Get-ChildItem -Path $Path -Recurse -Force -ErrorAction SilentlyContinue | 
            Measure-Object -Property Length -Sum
    
    [PSCustomObject]@{
        'Path' = (Resolve-Path $Path).Path
        'Size (MB)' = [math]::Round($size.Sum / 1MB, 2)
        'Size (GB)' = [math]::Round($size.Sum / 1GB, 2)
    }
}
Set-Alias -Name dirsize -Value Get-DirectorySize

# Kill processes matching a pattern
function Stop-ProcessByName {
    param([Parameter(Mandatory=$true)][string]$Pattern)
    
    Get-Process | Where-Object { $_.ProcessName -like "*$Pattern*" } | 
    ForEach-Object {
        Write-Host "Stopping: $($_.ProcessName) (ID: $($_.Id))"
        $_ | Stop-Process -Force
    }
}
Set-Alias -Name killp -Value Stop-ProcessByName

# Quick network connections overview
function Get-NetworkConnections {
    Get-NetTCPConnection -State Established | 
    Select-Object LocalAddress,LocalPort,RemoteAddress,RemotePort,State,
    @{Name="ProcessName";Expression={(Get-Process -Id $_.OwningProcess).ProcessName}},
    @{Name="PID";Expression={$_.OwningProcess}} |
    Sort-Object ProcessName
}
Set-Alias -Name netconn -Value Get-NetworkConnections

```