```ps
# Prompt for credentials
$cred = Get-Credential

# Make the request using the credentials
$r = Invoke-WebRequest -Uri "https://yourapiendpoint.com/devices/getcertificateentry?path=/config/filestore/files" `
    -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy -Credential $cred `
    -PreserveAuthorizationOnRedirect -MaximumRedirection 0 -Verbose

# Check the response and handle redirects if necessary
if ($r.StatusCode -eq 302 -or $r.StatusCode -eq 301) {
    $next = $r.Headers.Location
    $r2 = Invoke-WebRequest -Uri $next -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
        -Credential $cred -PreserveAuthorizationOnRedirect -Verbose

    if ($r2.StatusCode -eq 302 -or $r2.StatusCode -eq 301) {
        $next2 = $r2.Headers.Location
        $r3 = Invoke-WebRequest -Uri $next2 -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
            -Credential $cred -PreserveAuthorizationOnRedirect -Verbose

        if ($r3.StatusCode -eq 302 -or $r3.StatusCode -eq 301) {
            $returnurl = $r3.Headers.Location
            $r4 = Invoke-WebRequest -Uri $returnurl -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
                -Credential $cred -PreserveAuthorizationOnRedirect -Verbose
            $r4.RawContent
        } else {
            $r3.RawContent
        }
    } else {
        $r2.RawContent
    }
} else {
    $r.RawContent
}

```


without prompt 

```ps
# Define username and password
$username = "yourusername"
$password = "yourpassword" | ConvertTo-SecureString -AsPlainText -Force

# Create a PSCredential object
$cred = New-Object System.Management.Automation.PSCredential($username, $password)

# Make the request using the credentials
$r = Invoke-WebRequest -Uri "https://yourapiendpoint.com/devices/getcertificateentry?path=/config/filestore/files" `
    -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy -Credential $cred `
    -PreserveAuthorizationOnRedirect -MaximumRedirection 0 -Verbose

# Check the response and handle redirects if necessary
if ($r.StatusCode -eq 302 -or $r.StatusCode -eq 301) {
    $next = $r.Headers.Location
    $r2 = Invoke-WebRequest -Uri $next -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
        -Credential $cred -PreserveAuthorizationOnRedirect -Verbose

    if ($r2.StatusCode -eq 302 -or $r2.StatusCode -eq 301) {
        $next2 = $r2.Headers.Location
        $r3 = Invoke-WebRequest -Uri $next2 -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
            -Credential $cred -PreserveAuthorizationOnRedirect -Verbose

        if ($r3.StatusCode -eq 302 -or $r3.StatusCode -eq 301) {
            $returnurl = $r3.Headers.Location
            $r4 = Invoke-WebRequest -Uri $returnurl -UserAgent "curl/123" -AllowUnencryptedAuthentication -NoProxy `
                -Credential $cred -PreserveAuthorizationOnRedirect -Verbose
            $r4.RawContent
        } else {
            $r3.RawContent
        }
    } else {
        $r2.RawContent
    }
} else {
    $r.RawContent
}

```


get creds

```ps
# Get the default credentials of the current user
$defaultCreds = [System.Net.CredentialCache]::DefaultNetworkCredentials

# Display the credentials
$defaultCreds

```