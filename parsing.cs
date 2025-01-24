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