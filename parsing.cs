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