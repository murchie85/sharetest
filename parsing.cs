var filtered = documents.EnumerateArray()
   .Where(doc => 
       doc.GetProperty("clientMachine").GetString() == "x" && 
       JsonDocument.Parse(doc.GetProperty("Properties").GetString())
           .RootElement.GetProperty("PluginVersion").GetString() == "10"
   );