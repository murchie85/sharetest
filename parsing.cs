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




public string CallKeyfactorPutJson(string url, string jsonPayload)
{
    LogHandlerCommon.MethodEntry(logger, CertificateStore, "MakeApiCall");
    try
    {
        var byteArray = Encoding.ASCII.GetBytes(@"testuser:testpass");
        var base64Credentials = Convert.ToBase64String(byteArray);
        var handler = new HttpClientHandler();
        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic", base64Credentials);

        LogHandlerCommon.Info(logger, CertificateStore, $"Making API PUT call to: '{url}'");

        using (client)
        {
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = client.PutAsync(url, content).Result;
            LogHandlerCommon.Info(logger, CertificateStore, $"Received response. Status: {response.StatusCode}, ReasonPhrase: {response.ReasonPhrase}");

            response.EnsureSuccessStatusCode();
            var responseContent = response.Content.ReadAsStringAsync().Result;

            LogHandlerCommon.Info(logger, CertificateStore, "Successfully read response content");
            return responseContent;
        }
    }
    catch (Exception ex)
    {
        LogHandlerCommon.Error(logger, CertificateStore, $"Exception occurred: {ex.Message}");
        throw;
    }
}



using Newtonsoft.Json;

var originalStore = JsonDocument.Parse(result).RootElement;
var propertiesStr = originalStore.GetProperty("Properties").GetString();
propertiesStr = System.Text.RegularExpressions.Regex.Unescape(propertiesStr);

var existingProps = JsonConvert.DeserializeObject<Dictionary<string, object>>(propertiesStr);

// Update specific properties
existingProps["ServerUsername"] = new { value = new { SecretValue = serverUsername } };
existingProps["ServerPassword"] = new { value = new { SecretValue = serverPword } };
existingProps["cyberarkUsername"] = new { value = new { SecretValue = cyberarkUname } };
existingProps["cyberarkPassword"] = new { value = new { SecretValue = cyberarkPword } };
existingProps["PrimaryNodeCheckRetryMax"] = new { value = "22" };

string newPropsJson = JsonConvert.SerializeObject(existingProps, Formatting.Indented);

var storeObj = new
{
    Id = originalStore.GetProperty("Id").GetString(),
    ContainerId = originalStore.GetProperty("ContainerId").GetRawText(),
    DisplayName = originalStore.GetProperty("DisplayName").GetString(),
    ClientMachine = originalStore.GetProperty("ClientMachine").GetString(),
    Properties = newPropsJson
};

string finalBody = JsonConvert.SerializeObject(storeObj, Formatting.Indented);





var originalStore = JsonDocument.Parse(result).RootElement;
var propertiesStr = originalStore.GetProperty("Properties").GetString();
propertiesStr = System.Text.RegularExpressions.Regex.Unescape(propertiesStr);

// Create store object preserving all original fields
var storeObj = new
{
   Id = originalStore.GetProperty("Id").GetString(),
   ContainerId = originalStore.GetProperty("ContainerId").GetRawText(),
   DisplayName = originalStore.GetProperty("DisplayName").GetString(),
   ClientMachine = originalStore.GetProperty("ClientMachine").GetString(),
   Storepath = originalStore.GetProperty("Storepath").GetString(),
   CertStoreInventoryJobId = originalStore.GetProperty("CertStoreInventoryJobId").GetRawText(),
   CertStoreType = originalStore.GetProperty("CertStoreType").GetRawText(),
   Approved = originalStore.GetProperty("Approved").GetBoolean(),
   CreateIfMissing = originalStore.GetProperty("CreateIfMissing").GetBoolean(),
   AgentId = originalStore.GetProperty("AgentId").GetString(),
   AgentAssigned = originalStore.GetProperty("AgentAssigned").GetBoolean(),
   ContainerName = originalStore.GetProperty("ContainerName").GetString(),
   InventorySchedule = originalStore.GetProperty("InventorySchedule").GetRawText(),
   ReenrollmentStatus = originalStore.GetProperty("ReenrollmentStatus").GetRawText(),
   SetNewPasswordAllowed = originalStore.GetProperty("SetNewPasswordAllowed").GetBoolean(),
   Password = originalStore.GetProperty("Password").GetRawText(),
   Properties = newPropsJson
};

string finalBody = JsonConvert.SerializeObject(storeObj, Formatting.Indented);