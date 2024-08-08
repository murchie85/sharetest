```
using System.Management.Automation;
using System.Collections.Generic;

public F5PagedSSLProfiles getPagedProfiles(string query)
{
    // Initialize PowerShell instance
    using (PowerShell ps = PowerShell.Create())
    {
        // PowerShell script to execute
        string script = @"
# PowerShell script to obtain the SSL profiles
param (
    [string]$query
)
$url = 'https://<your_endpoint>/mgmt/shared/authn/login'
$body = @{
    username = '<your_username>'
    password = '<your_password>'
    loginProviderName = 'tmos'
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri $url -Method Post -Body $body -ContentType 'application/json' -skipCertificateCheck
$token = $response.token.token

$secureURL = 'https://<your_endpoint>' + $query
$headers = @{
    'X-F5-Auth-Token' = $token
    'Content-Type' = 'application/json'
}

$certs = Invoke-RestMethod -Uri $secureURL -Method Get -Headers $headers -skipCertificateCheck
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

        // Add the PowerShell script and parameters
        ps.AddScript(script).AddParameter("query", query);

        // Execute the script and retrieve the result
        var result = ps.Invoke();

        // Check for errors
        if (ps.HadErrors)
        {
            throw new Exception("Error occurred while executing PowerShell script: " + string.Join(", ", ps.Streams.Error));
        }

        // Convert the result to F5PagedSSLProfiles
        if (result.Count > 0)
        {
            string jsonString = result[0].ToString();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<F5PagedSSLProfiles>(jsonString);
        }

        return null;
    }
}

```