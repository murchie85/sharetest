// semi factory approach
protected virtual F5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null)
{
    return new F5Client(store, user, pass, ssl, null, ignoreSsl, useToken, inventory, cyberarkUser, cyberarkPass, inventoryType, uploadedCerts, env) { F5Version = base.F5Version };
}

// 
F5Client f5 = CreateF5Client(config.CertificateStoreDetails, ServerUserName, ServerPassword, config.UseSSL, IgnoreSSLWarning, UseTokenAuth, config.LastInventory, cyberarkUsername, cyberarkPassword, InventoryType, UploadedCerts, Env_Plugin);



public class TestableInventory : Inventory
{
    private readonly F5Client _mockClient;
    
    public TestableInventory(IPAMSecretResolver resolver, F5Client mockClient) : base(resolver)
    {
        _mockClient = mockClient;
    }
    
    protected override F5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null)
    {
        return _mockClient;
    }
    
    public InventoryJobConfiguration GetJobConfig()
    {
        return base.JobConfig;
    }
}