var filtered = documents.EnumerateArray()
   .Where(doc => 
       doc.GetProperty("clientMachine").GetString() == "x" && 
       JsonDocument.Parse(doc.GetProperty("Properties").GetString())
           .RootElement.GetProperty("PluginVersion").GetString() == "10"
   );





var filtered = resultDoc.EnumerateArray()
    .Where(doc => doc.GetProperty("clientMachine").GetString() == CertificateStore.ClientMachine);

LogHandlerCommon.Info(logger, CertificateStore, $"Found matches: {filtered.Count()}");

foreach (var doc in filtered)
{
    LogHandlerCommon.Info(logger, CertificateStore, $"Match found: {doc}");
}