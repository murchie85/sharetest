

protected internal virtual IF5Client CreateF5Client(CertificateStore store, string user, string pass, bool ssl, bool ignoreSsl, bool useToken, IEnumerable<PreviousInventoryItem> inventory, string cyberarkUser = null, string cyberarkPass = null, string inventoryType = null, string uploadedCerts = null, string env = null);





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