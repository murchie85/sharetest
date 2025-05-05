
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.F5Orchestrator.Tests.SSLProfile
{
    [TestFixture]
    public class ManagementTests
    {
        private Mock<IPAMSecretResolver> _mockResolver;
        private Mock<F5Client> _mockF5Client;
        
        [SetUp]
        public void Setup()
        {
            _mockResolver = new Mock<IPAMSecretResolver>();
            _mockF5Client = new Mock<F5Client>(MockBehavior.Loose, 
                It.IsAny<CertificateStore>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), 
                It.IsAny<bool>(), It.IsAny<IEnumerable<PreviousInventoryItem>>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>());
        }
        
        [Test]
        public void ProcessJob_SetsCredentials()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "testUsername",
                ServerPassword = "testPassword",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Add,
                JobHistoryId = "test123"
            };

            // Create properties JSON with CyberArk credentials
            config.CertificateStoreDetails.Properties = @"{
                ""cyberarkUsername"": ""cyberTestUser"",
                ""cyberarkPassword"": ""cyberTestPass"",
                ""InventoryType"": ""UPLOADED_ONLY"",
                ""Env"": ""prod"",
                ""ParmOne"": ""testParam""
            }";

            // Configure resolver to return the input value (no PAM resolution in test)
            _mockResolver.Setup(r => r.Resolve(It.IsAny<string>()))
                .Returns<string>(s => s);
            
            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            var credentials = testable.GetCredentials();
            Assert.IsNotNull(credentials);
            Assert.AreEqual("testUsername", credentials.ServerUsername);
            Assert.AreEqual("testPassword", credentials.ServerPassword);
            Assert.AreEqual("cyberTestUser", credentials.CyberarkUsername);
            Assert.AreEqual("cyberTestPass", credentials.CyberarkPassword);
            Assert.AreEqual("testParam", credentials.Parm1);
        }

        [Test]
        public void ProcessJob_WithAdd_CallsPerformAddJob()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "username",
                ServerPassword = "password",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Add,
                JobHistoryId = "test123"
            };

            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            Assert.IsTrue(testable.AddJobCalled);
            Assert.IsFalse(testable.RemoveJobCalled);
        }

        [Test]
        public void ProcessJob_WithRemove_CallsPerformRemoveJob()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "username",
                ServerPassword = "password",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Remove,
                JobHistoryId = "test123"
            };

            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            Assert.IsFalse(testable.AddJobCalled);
            Assert.IsTrue(testable.RemoveJobCalled);
        }
    }
    
    public class TestableManagement : Keyfactor.Extensions.Orchestrator.F5Orchestrator.SSLProfile.Management
    {
        private readonly F5Client _mockClient;
        public bool AddJobCalled { get; private set; }
        public bool RemoveJobCalled { get; private set; }
        
        public TestableManagement(IPAMSecretResolver resolver, F5Client mockClient) : base(resolver)
        {
            _mockClient = mockClient;
        }
        
        internal override F5Client GetTestableF5Client(ManagementJobConfiguration config, string cyberarkUsername, string cyberarkPassword, 
            string InventoryType, string UploadedCerts, string Env_Plugin)
        {
            return _mockClient;
        }

        // Override to expose protected properties for testing
        public class Credentials
        {
            public string ServerUsername { get; set; }
            public string ServerPassword { get; set; }
            public string CyberarkUsername { get; set; }
            public string CyberarkPassword { get; set; }
            public string Parm1 { get; set; }
        }

        public Credentials GetCredentials()
        {
            return new Credentials
            {
                ServerUsername = ServerUserName,
                ServerPassword = ServerPassword,
                CyberarkUsername = GetPrivateField<string>("cyberarkUsername"),
                CyberarkPassword = GetPrivateField<string>("cyberarkPassword"),
                Parm1 = GetPrivateField<string>("parm1")
            };
        }

        private T GetPrivateField<T>(string fieldName)
        {
            var field = GetType().BaseType.GetField(fieldName, 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic);
            return (T)field.GetValue(this);
        }

        // Override methods to track calls
        private new void PerformAddJob(F5Client f5, string certStoreID)
        {
            AddJobCalled = true;
            // Don't call base to avoid real implementation
        }

        private new void PerformRemovalJob(F5Client f5)
        {
            RemoveJobCalled = true;
            // Don't call base to avoid real implementation
        }
    }
}






// -----

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.F5Orchestrator.Tests.SSLProfile
{
    [TestFixture]
    public class ManagementTests
    {
        private Mock<IPAMSecretResolver> _mockResolver;
        private Mock<F5Client> _mockF5Client;
        
        [SetUp]
        public void Setup()
        {
            _mockResolver = new Mock<IPAMSecretResolver>();
            _mockF5Client = new Mock<F5Client>(MockBehavior.Loose, 
                It.IsAny<CertificateStore>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), 
                It.IsAny<bool>(), It.IsAny<IEnumerable<PreviousInventoryItem>>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>());
        }
        
        [Test]
        public void ProcessJob_SetsCredentials()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "testUsername",
                ServerPassword = "testPassword",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Add,
                JobHistoryId = "test123"
            };

            // Create properties JSON with CyberArk credentials
            config.CertificateStoreDetails.Properties = @"{
                ""cyberarkUsername"": ""cyberTestUser"",
                ""cyberarkPassword"": ""cyberTestPass"",
                ""InventoryType"": ""UPLOADED_ONLY"",
                ""Env"": ""prod""
            }";

            // Configure resolver to return the input value (no PAM resolution in test)
            _mockResolver.Setup(r => r.Resolve(It.IsAny<string>()))
                .Returns<string>(s => s);
            
            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            var credentials = testable.GetCredentials();
            Assert.IsNotNull(credentials);
            Assert.AreEqual("testUsername", credentials.ServerUsername);
            Assert.AreEqual("testPassword", credentials.ServerPassword);
            Assert.AreEqual("cyberTestUser", credentials.CyberarkUsername);
            Assert.AreEqual("cyberTestPass", credentials.CyberarkPassword);
        }

        [Test]
        public void ProcessJob_WithAdd_CallsPerformAddJob()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "username",
                ServerPassword = "password",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Add,
                JobHistoryId = "test123"
            };

            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            Assert.IsTrue(testable.AddJobCalled);
            Assert.IsFalse(testable.RemoveJobCalled);
        }

        [Test]
        public void ProcessJob_WithRemove_CallsPerformRemoveJob()
        {
            // Arrange
            var config = new ManagementJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "username",
                ServerPassword = "password",
                JobCertificate = new JobCertificate { Alias = "testCert" },
                OperationType = CertStoreOperationType.Remove,
                JobHistoryId = "test123"
            };

            var testable = new TestableManagement(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            testable.ProcessJob(config);
            
            // Assert
            Assert.IsFalse(testable.AddJobCalled);
            Assert.IsTrue(testable.RemoveJobCalled);
        }
    }
    
    public class TestableManagement : SSLProfile.Management
    {
        private readonly F5Client _mockClient;
        public bool AddJobCalled { get; private set; }
        public bool RemoveJobCalled { get; private set; }
        
        public TestableManagement(IPAMSecretResolver resolver, F5Client mockClient) : base(resolver)
        {
            _mockClient = mockClient;
        }
        
        internal virtual F5Client GetTestableF5Client(ManagementJobConfiguration config)
        {
            return _mockClient;
        }

        // Override to expose protected properties for testing
        public class Credentials
        {
            public string ServerUsername { get; set; }
            public string ServerPassword { get; set; }
            public string CyberarkUsername { get; set; }
            public string CyberarkPassword { get; set; }
        }

        public Credentials GetCredentials()
        {
            return new Credentials
            {
                ServerUsername = ServerUserName,
                ServerPassword = ServerPassword,
                CyberarkUsername = GetPrivateField<string>("cyberarkUsername"),
                CyberarkPassword = GetPrivateField<string>("cyberarkPassword")
            };
        }

        private T GetPrivateField<T>(string fieldName)
        {
            var field = GetType().BaseType.GetField(fieldName, 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic);
            return (T)field.GetValue(this);
        }

        // Override methods to track calls
        protected new void PerformAddJob(F5Client f5, string certStoreID)
        {
            AddJobCalled = true;
            // Don't call base to avoid real implementation
        }

        protected new void PerformRemovalJob(F5Client f5)
        {
            RemoveJobCalled = true;
            // Don't call base to avoid real implementation
        }
    }
}





LogHandlerCommon.Info(logger, config.CertificateStoreDetails, 
    $"DIRECT: Creating F5Client with params: " +
    $"Store={config.CertificateStoreDetails?.StorePath}, " +
    $"Username={ServerUserName}, " +
    $"Password={(ServerPassword != null ? "set" : "null")}, " +
    $"UseSSL={config.UseSSL}, " +
    $"PfxPwd={null}, " +
    $"IgnoreSSL={IgnoreSSLWarning}, " +
    $"UseToken={UseTokenAuth}, " +
    $"Inventory Count={(config.LastInventory != null ? "has items" : "null")}, " +
    $"CybArkUser={cyberarkUsername}, " +
    $"CybArkPass={(cyberarkPassword != null ? "set" : "null")}, " +
    $"InvType={InventoryType}, " +
    $"UploadedCerts={UploadedCerts}, " +
    $"Env={Env_Plugin}, " +
    $"F5Version={base.F5Version}");

// For method-based F5Client creation
LogHandlerCommon.Info(logger, config.CertificateStoreDetails, 
    $"METHOD: Creating F5Client with params: " +
    $"Store={config.CertificateStoreDetails?.StorePath}, " +
    $"Username={ServerUserName}, " +
    $"Password={(ServerPassword != null ? "set" : "null")}, " +
    $"UseSSL={config.UseSSL}, " +
    $"PfxPwd=null, " +
    $"IgnoreSSL={IgnoreSSLWarning}, " +
    $"UseToken={UseTokenAuth}, " +
    $"Inventory={(config.LastInventory != null ? "has items" : "null")}, " +
    $"CybArkUser={cyberarkUsername}, " +
    $"CybArkPass={(cyberarkPassword != null ? "set" : "null")}, " +
    $"InvType={InventoryType}, " +
    $"UploadedCerts={UploadedCerts}, " +
    $"Env={Env_Plugin}, " +
    $"F5Version={base.F5Version}");                 
protected internal virtual F5Client GetTestableF5Client(InventoryJobConfiguration config, string cyberarkUsername, string cyberarkPassword, 
    string InventoryType, string UploadedCerts, string Env_Plugin)
{
    return new F5Client(config.CertificateStoreDetails, ServerUserName, ServerPassword, config.UseSSL, null,
        IgnoreSSLWarning, UseTokenAuth, config.LastInventory, cyberarkUsername, cyberarkPassword,
        InventoryType, UploadedCerts, Env_Plugin) { F5Version = base.F5Version };
}

F5Client f5 = GetTestableF5Client(config, cyberarkUsername, cyberarkPassword, InventoryType, UploadedCerts, Env_Plugin);



public class TestableInventory : SSLProfile.Inventory
{
    private readonly F5Client _mockClient;
    
    public TestableInventory(IPAMSecretResolver resolver, F5Client mockClient) : base(resolver)
    {
        _mockClient = mockClient;
    }
    
    protected internal override F5Client GetTestableF5Client(InventoryJobConfiguration config, string cyberarkUsername, string cyberarkPassword, 
        string InventoryType, string UploadedCerts, string Env_Plugin)
    {
        return _mockClient;
    }
    
    public InventoryJobConfiguration GetJobConfig()
    {
        return base.JobConfig;
    }
}


/// all failed belwo 
-----
protected internal virtual IF5Client GetTestableF5Client()
{
    return new F5Client(config.CertificateStoreDetails, ServerUserName, ServerPassword, config.UseSSL, null, 
        IgnoreSSLWarning, UseTokenAuth, config.LastInventory, cyberarkUsername, cyberarkPassword, 
        InventoryType, UploadedCerts, Env_Plugin) { F5Version = base.F5Version };
}



protected internal override IF5Client GetTestableF5Client()
{
    return _mockClient;
}


IF5Client f5 = GetTestableF5Client();




---

protected internal virtual IF5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null);


protected internal virtual IF5Client CreateF5Client(CertificateStore store, string user, 
    string pass, bool ssl, string pfxPassword, bool ignoreSsl, 
    bool useToken, IEnumerable<PreviousInventoryItem> inventory, 
    string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, 
    string uploadedCerts = null, string env = null)
{
    return new F5Client(store, user, pass, ssl, pfxPassword, ignoreSsl, useToken, inventory, 
        cyberarkUser, cyberarkPass, inventoryType, uploadedCerts, env) 
        { F5Version = base.F5Version };
}

// semi factory approach
protected virtual F5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null)
{
    return new F5Client(store, user, pass, ssl, null, ignoreSsl, useToken, inventory, cyberarkUser, cyberarkPass, inventoryType, uploadedCerts, env) { F5Version = base.F5Version };
}

// 
F5Client f5 = CreateF5Client(config.CertificateStoreDetails, ServerUserName, ServerPassword, config.UseSSL, IgnoreSSLWarning, UseTokenAuth, config.LastInventory, cyberarkUsername, cyberarkPassword, InventoryType, UploadedCerts, Env_Plugin);



namespace Keyfactor.Extensions.Orchestrator.F5Orchestrator
{
    public interface IF5Client
    {
        List<CurrentInventoryItem> GetSSLProfiles(int pageSize);
    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.F5Orchestrator.Tests.SSLProfile
{
    [TestFixture]
    public class InventoryTests
    {
        private Mock<IPAMSecretResolver> _mockResolver;
        private Mock<IF5Client> _mockF5Client;
        
        [SetUp]
        public void Setup()
        {
            _mockResolver = new Mock<IPAMSecretResolver>();
            _mockF5Client = new Mock<IF5Client>();
        }
        
        [Test]
        public void ProcessJob_SetsJobConfig()
        {
            // Arrange
            var mockInventoryItems = new List<CurrentInventoryItem>();
            
            _mockF5Client.Setup(c => c.GetSSLProfiles(It.IsAny<int>()))
                .Returns((mockInventoryItems, "success"));
                
            _mockF5Client.Setup(c => c.ValidateF5Version());
            
            var config = new InventoryJobConfiguration 
            { 
                CertificateStoreDetails = new CertificateStore(),
                ServerUsername = "username",
                ServerPassword = "password",
                UseSSL = true,
                JobHistoryId = "test123" 
            };
            
            var testable = new TestableInventory(_mockResolver.Object, _mockF5Client.Object);
            
            // Act
            bool submitCalled = false;
            testable.ProcessJob(config, items => { submitCalled = true; });
            
            // Assert
            Assert.IsNotNull(testable.GetJobConfig());
            Assert.AreEqual("test123", testable.GetJobConfig().JobHistoryId);
            Assert.IsTrue(submitCalled, "Submit inventory should be called");
        }
    }
    
    public class TestableInventory : SSLProfile.Inventory
    {
        private readonly IF5Client _mockClient;
        
        public TestableInventory(IPAMSecretResolver resolver, IF5Client mockClient) : base(resolver)
        {
            _mockClient = mockClient;
        }

        protected override IF5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, string pfxPassword, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null)
        {
            return _mockClient;
        }
        
        
        public InventoryJobConfiguration GetJobConfig()
        {
            return base.JobConfig;
        }
    }
}