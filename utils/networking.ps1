[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }
[System.Net.WebRequest]::DefaultWebProxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
$traceSource = New-Object System.Diagnostics.TraceSource "System.Net"
$traceSource.Switch.Level = [System.Diagnostics.SourceLevels]::Verbose
$traceSource.Listeners.Add((New-Object System.Diagnostics.TextWriterTraceListener "C:\Path\To\NetworkTrace.log"))
$traceSource.Listeners[0].TraceOutputOptions = [System.Diagnostics.TraceOptions]::DateTime
$traceSource.Listeners[0].TraceOutputOptions = [System.Diagnostics.TraceOptions]::Callstack
