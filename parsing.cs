var filtered = documents.EnumerateArray()
   .Where(doc => 
       doc.GetProperty("clientMachine").GetString() == "x" && 
       JsonDocument.Parse(doc.GetProperty("Properties").GetString())
           .RootElement.GetProperty("PluginVersion").GetString() == "10"
   );





foreach (var doc in resultDoc.EnumerateArray())
{
    var clientMachine = doc.GetProperty("clientMachine").GetString();
    var properties = doc.GetProperty("Properties").GetString();
    
    LogHandlerCommon.Info(logger, CertificateStore, 
        $"Machine: {clientMachine}, Match: {clientMachine == CertificateStore.ClientMachine}");
}


var filtered = resultDoc.EnumerateArray()
    .Select(doc => new {
        Doc = doc,
        HasClient = doc.TryGetProperty("clientMachine", out var cm),
        ClientMachine = doc.TryGetProperty("clientMachine", out var c) ? c.GetString() : null
    });

foreach (var item in filtered)
{
    LogHandlerCommon.Info(logger, CertificateStore, 
        $"Has clientMachine: {item.HasClient}, Value: {item.ClientMachine}");
}


foreach (var doc in resultDoc.EnumerateArray())
{
   if (doc.TryGetProperty("ClientMachine", out var cm) && 
       cm.GetString() == CertificateStore.ClientMachine)
   {
       LogHandlerCommon.Info(logger, CertificateStore, $"Found matching machine: {cm.GetString()}");
   }
}




```csharp
string TargetCertStoreID = "none";
foreach (var doc in resultDoc.EnumerateArray())
{
    if (doc.TryGetProperty("ClientMachine", out var cm) && 
        cm.GetString() == CertificateStore.ClientMachine)
    {
        LogHandlerCommon.Info(logger, CertificateStore, $"Checking machine: {cm.GetString()}");
        LogHandlerCommon.Info(logger, CertificateStore, $"Doc ID: {doc.GetProperty("Id").GetString()}");

        var propertiesStr = doc.GetProperty("Properties").GetString();
        var propertiesJson = JsonDocument.Parse(propertiesStr).RootElement;

        if (propertiesJson.TryGetProperty("PluginVersion", out var version))
        {
            LogHandlerCommon.Info(logger, CertificateStore, $"Found PluginVersion: {version.GetString()}");
            if (version.GetString() == "10")
            {
                TargetCertStoreID = doc.GetProperty("Id").GetString();
                LogHandlerCommon.Info(logger, CertificateStore, $"Found matching store with ID: {TargetCertStoreID}");
                break;
            }
        }
    }
}
LogHandlerCommon.Info(logger, CertificateStore, $"Final target certstore id: {TargetCertStoreID}");
```