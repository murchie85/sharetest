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
$body = @{
    username = '<your_username>'
    password = '<your_password>'
    loginProviderName = 'tmos'
} | ConvertTo-Json

try {{
    $response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType 'application/json' -SkipCertificateCheck
    Write-Output 'First request completed.'
}} catch {{
    $response = $_.Exception
    Write-Output 'First request failed: $($response.Message)'
}}

$token = $response.token.token
$secureURL = 'https://<your_endpoint>' + '{query}'
$headers = @{
    'X-F5-Auth-Token' = $token
    'Content-Type' = 'application/json'
}

$certs = Invoke-RestMethod -Uri $secureURL -Method Get -Headers $headers -SkipCertificateCheck
$filteredCerts = $certs.items | Select-Object name, isBundle, keyType

$f5PagedSSLProfiles = [PSCustomObject]@{
    items = $filteredCerts
    totalItems = $certs.totalItems
    totalPages = $certs.totalPages
    pageIndex = $certs.pageIndex
    nextLink = $certs.nextLink
}

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