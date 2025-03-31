$username = "REDACTED" # Replace with your username
$password = "REDACTED" | ConvertTo-SecureString -AsPlainText -Force
$uri = 'REDACTED'
$cred = New-Object System.Management.Automation.PSCredential($username, $password)
$s = $null
$s2 = $null

try {
    $r = iwr -uri $uri -UserAgent curl/123 -AllowUnencryptedAuthentication -NoProxy -SessionVariable s -Credential $cred -Headers @{ Accept = "*/*" } -PreserveAuthorizationOnRedirect -MaximumRedirection 0
} catch {
    $r = $_.Exception
}

$next = $r.Response.Headers.Location.OriginalString

try {
    $r2 = iwr -uri $next -UserAgent curl/123 -AllowUnencryptedAuthentication -NoProxy -SessionVariable s2 -Credential $cred -Headers @{ Accept = "*/*" } -PreserveAuthorizationOnRedirect -MaximumRedirection 0
} catch {
    $r2 = $_.Exception
}

$next = $r2.Response.Headers.Location.OriginalString

try {
    $r3 = iwr -uri $next -UserAgent curl/123 -AllowUnencryptedAuthentication -NoProxy -WebSession $s2 -Credential $cred -Headers @{ Accept = "*/*" } -PreserveAuthorizationOnRedirect -MaximumRedirection 0
} catch {
    $r3 = $_.Exception
}

$next = $r3.Response.Headers.Location.OriginalString

try {
    $returnurl = $r3.Response.Headers.Location.OriginalString
    $r4 = iwr -uri $returnurl -UserAgent curl/123 -AllowUnencryptedAuthentication -NoProxy -WebSession $s -Credential $cred -Headers @{ Accept = "*/*" } -PreserveAuthorizationOnRedirect -MaximumRedirection 10
} catch {
    $r4 = $_.Exception
}

$r4
