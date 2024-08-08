```
using System.Management.Automation;
using System.Collections.Generic;

public F5PagedSSLProfiles getPagedProfiles(string query)
{
    string powerShellExe = @"C:\Program Files\PowerShell\7\pwsh.exe";
    string psScript = $@"
$s = $null
$s2 = $null

Write-Output 'Starting PowerShell script execution...'

# Authenticate and get token
$url = 'https://<your_endpoint>/mgmt/shared/authn/login'
$body = @{{ username = '<your_username>'; password = '<your_password>'; loginProviderName = 'tmos' }} | ConvertTo-Json

try {{
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType 'application/json' -SkipCertificateCheck
    Write-Output 'First request completed.'
}} catch {{
    $response = $_.Exception
    Write-Output 'First request failed: $($response.Message)'
}}

$token = $response.token.token
$secureURL = 'https://<your_endpoint>' + '{query}'
$headers = @{{ 'X-F5-Auth-Token' = $token; 'Content-Type' = 'application/json' }}

$certs = Invoke-RestMethod -Uri $secureURL -Method Get -Headers $headers -SkipCertificateCheck
$filteredCerts = $certs.items | Select-Object name, isBundle, keyType

$f5PagedSSLProfiles = [PSCustomObject]@{{
    items = $filteredCerts
    totalItems = $certs.totalItems
    totalPages = $certs.totalPages
    pageIndex = $certs.pageIndex
    nextLink = $certs.nextLink
}}

$f5PagedSSLProfiles | ConvertTo-Json -Depth 3
";

    string output = string.Empty;
    string error = string.Empty;

    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = powerShellExe,
        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process process = new Process())
    {
        process.StartInfo = startInfo;
        process.Start();

        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        process.WaitForExit();
    }

    if (!string.IsNullOrEmpty(error))
    {
        throw new Exception($"PowerShell error: {error}");
    }

    return Newtonsoft.Json.JsonConvert.DeserializeObject<F5PagedSSLProfiles>(output);
}

```	



```
    try
    {
        F5PagedSSLProfiles result = Newtonsoft.Json.JsonConvert.DeserializeObject<F5PagedSSLProfiles>(output);
        LogHandlerCommon.Trace(logger, CertificateStore, $"Deserialized result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
        return result;
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"Error deserializing output: {ex.Message}");
        throw;
    }
}
```


```
try {{
    $certs = Invoke-RestMethod -Uri $secureURL -Method Get -Headers $headers -SkipCertificateCheck
    Write-Output 'Second request completed.'
    Write-Output 'Certificates obtained: ' + ($certs | ConvertTo-Json -Depth 3)
}} catch {{
    $certs = $_.Exception
    Write-Output 'Second request failed: $($certs.Message)'
}}
```


```
public F5PagedSSLProfiles getPagedProfiles(string query)
{
    string powerShellExe = @"C:\Program Files\PowerShell\7\pwsh.exe";
    string psScript = $@"
Write-Output 'Starting PowerShell script execution...'

# Authenticate and get token
$url = 'https://<your_endpoint>/mgmt/shared/authn/login'
$body = @{{ 
    username = '<your_username>'
    password = '<your_password>'
    loginProviderName = 'tmos'
}} | ConvertTo-Json

Write-Output 'Token request body: ' + $body

try {{
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType 'application/json' -SkipCertificateCheck
    Write-Output 'First request completed.'
}} catch {{
    $response = $_.Exception
    Write-Output 'First request failed: $($response.Message)'
    exit
}}

if ($response -is [System.Management.Automation.PSObject]) {{
    $token = $response.token.token
    Write-Output -NoNewline 'Token obtained: '
    Write-Output $token
}} else {{
    Write-Output 'Failed to obtain token'
    exit
}}

$secureURL = 'https://<your_endpoint>' + '{query}'
Write-Output 'Secure URL: ' + $secureURL

$headers = @{{ 
    'X-F5-Auth-Token' = $token
    'Content-Type' = 'application/json'
}}

Write-Output 'Request headers: ' + ($headers | ConvertTo-Json -Depth 3)

try {{
    $certs = Invoke-RestMethod -Uri $secureURL -Method Get -Headers $headers -SkipCertificateCheck
    Write-Output 'Second request completed.'
    Write-Output 'Certificates obtained: ' + ($certs | ConvertTo-Json -Depth 3)
}} catch {{
    $certs = $_.Exception
    Write-Output 'Second request failed: $($certs.Message)'
    Write-Output 'Call output: ' + $certs
    exit
}}

if ($certs -is [System.Management.Automation.PSObject]) {{
    $filteredCerts = $certs.items | Select-Object name, isBundle, keyType

    $f5PagedSSLProfiles = [PSCustomObject]@{{
        items = $filteredCerts
        totalItems = $certs.totalItems
        totalPages = $certs.totalPages
        pageIndex = $certs.pageIndex
        nextLink = $certs.nextLink
    }}

    Write-Output 'Final result: ' + ($f5PagedSSLProfiles | ConvertTo-Json -Depth 3)
}} else {{
    Write-Output 'Failed to obtain certificates'
    exit
}}
";

    string output = string.Empty;
    string error = string.Empty;

    ProcessStartInfo startInfo = new ProcessStartInfo
    {
        FileName = powerShellExe,
        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{psScript}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (Process process = new Process())
    {
        process.StartInfo = startInfo;
        process.Start();

        output = process.StandardOutput.ReadToEnd();
        error = process.StandardError.ReadToEnd();

        process.WaitForExit();
    }

    LogHandlerCommon.Trace(logger, CertificateStore, $"PowerShell output: {output}");
    LogHandlerCommon.Trace(logger, CertificateStore, $"PowerShell error: {error}");

    if (!string.IsNullOrEmpty(error))
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"PowerShell error: {error}");
        throw new Exception($"PowerShell error: {error}");
    }

    try
    {
        F5PagedSSLProfiles result = Newtonsoft.Json.JsonConvert.DeserializeObject<F5PagedSSLProfiles>(output);
        LogHandlerCommon.Trace(logger, CertificateStore, $"Deserialized result: {Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
        return result;
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"Error deserializing output: {ex.Message}");
        throw;
    }
}

```