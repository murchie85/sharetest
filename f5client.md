## f5 client instantiation 

```c#
public class F5Client
{
    public CertificateStore CertificateStore { get; set; }
    public string ServerUserName { get; set; }
    public string ServerPassword { get; set; }
    public bool UseSSL { get; set; }
    public string PFXPassword { get; set; }
    public bool IgnoreSSLWarning { get; set; }
    public bool UseTokenAuth { get; set; }
    public IEnumerable<PreviousInventoryItem> Inventory { get; set; }
    public RESTHandler REST { get; set; }

    private static ILogger logger;

    public F5Client(CertificateStore certificateStore, string serverUserName, string serverPassword, bool useSSL, string pfxPassword, bool ignoreSSLWarning, bool useTokenAuth, IEnumerable<PreviousInventoryItem> inventory)
    {
        CertificateStore = certificateStore;
        ServerUserName = serverUserName;
        ServerPassword = serverPassword;
        UseSSL = useSSL;
        PFXPassword = pfxPassword;
        IgnoreSSLWarning = ignoreSSLWarning;
        UseTokenAuth = useTokenAuth;
        Inventory = inventory;

        if (logger == null)
        {
            logger = Keyfactor.Logging.LogHandler.GetClassLogger(this.GetType());
        }

        REST = new RESTHandler(certificateStore.ClientMachine, serverUserName, serverPassword, useSSL, IgnoreSSLWarning);

        if (UseTokenAuth)
        {
            REST.ObtainToken();
        }
    }

    public List<CurrentInventoryItem> GetSSLProfiles(int pageSize)
    {
        LogHandlerCommon.MethodEntry(logger, CertificateStore, "GetSSLProfiles");
        string partition = CertificateStore.StorePath;
        string query = $"/mgmt/tm/sys/file/ssl-cert?$filter=partition+eq+{partition}&$select=name,keyType,isBundle&$top={pageSize}&$skip=0";
        F5PagedSSLProfiles pagedProfiles = REST.Get<F5PagedSSLProfiles>(query);
        List<F5SSLProfile> profiles = new List<F5SSLProfile>();
        List<CurrentInventoryItem> inventory = new List<CurrentInventoryItem>();

        if (pagedProfiles.totalItems == 0 || pagedProfiles.items?.Length == 0)
        {
            LogHandlerCommon.Trace(logger, CertificateStore, $"No SSL profiles found in partition '{partition}'");
            LogHandlerCommon.MethodExit(logger, CertificateStore, "GetSSLProfiles");
            return inventory;
        }
        else
        {
            LogHandlerCommon.Trace(logger, CertificateStore, $"Compiling {pagedProfiles.totalPages} pages containing {pagedProfiles.totalItems} total inventory entries");
        }

        // Collected all of the profile entry names
        for (int i = pagedProfiles.pageIndex; i <= pagedProfiles.totalPages; i++)
        {
            profiles.AddRange(pagedProfiles.items);

            // The current paged profile will contain a link to the next set, unless the end has been reached
            if (string.IsNullOrEmpty(pagedProfiles.nextLink)) { break; }

            // Get the next page of profiles
            query = pagedProfiles.nextLink.Replace("https://localhost", "");
            pagedProfiles = REST.Get<F5PagedSSLProfiles>(query);
        }

        // Compile the entries into inventory items
        for (int i = 0; i < profiles.Count; i++)
        {
            try
            {
                // Exclude 'ca-bundle.crt' as that can only be managed by F5
                if (profiles[i].name.Equals("ca-bundle.crt", StringComparison.OrdinalIgnoreCase)
                    || profiles[i].name.Equals("f5-ca-bundle.crt", StringComparison.OrdinalIgnoreCase))
                {
                    LogHandlerCommon.Trace(logger, CertificateStore, $"Skipping '{profiles[i].name}' because it is managed by F5");
                    continue;
                }
                inventory.Add(GetInventoryItem(partition, profiles[i].name, true));
            }
            catch (Exception ex)
            {
                LogHandlerCommon.Error(logger, CertificateStore, ExceptionHandler.FlattenExceptionMessages(ex, $"Unable to process inventory item {profiles[i].name}."));
            }
        }

        LogHandlerCommon.MethodExit(logger, CertificateStore, "GetSSLProfiles");
        return inventory;
    }
}

```


# obtain token 

```c#
public void ObtainToken()
{
    logger.LogTrace("Entered ObtainToken method");

    string url = "/mgmt/shared/authn/login";
    var body = new
    {
        username = User,
        password = Password,
        loginProviderName = "tmos" // or any other appropriate login provider name
    };
    string requestContent = Newtonsoft.Json.JsonConvert.SerializeObject(body);

    using (HttpClient client = new HttpClient(GetHttpClientHandler()))
    {
        ConfigureHttpClient(client);
        HttpContent content = new StringContent(requestContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = client.PostAsync(url, content).Result;
        if (!response.IsSuccessStatusCode)
        {
            throw ProcessFailureResponse(response.StatusCode,
                response.Content.ReadAsStringAsync().Result,
                url,
                true,
                requestContent);
        }

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
        Token = result.token.token;
    }

    logger.LogTrace("Leaving ObtainToken method");
}

```