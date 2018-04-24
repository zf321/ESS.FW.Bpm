using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{


    public class DefaultPermissionForTenantMemberTest
    {
        private bool InstanceFieldsInitialized = false;

        public DefaultPermissionForTenantMemberTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        protected internal const string TENANT_ONE = "tenant1";
        protected internal const string TENANT_TWO = "tenant2";
        protected internal const string USER_ID = "user";
        protected internal const string GROUP_ID = "group";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

        protected internal ProcessEngineTestRule testRule;

        protected internal IAuthorizationService authorizationService;
        protected internal IIdentityService identityService;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;
        
        [SetUp]
        public virtual void init()
        {
            identityService = engineRule.IdentityService;
            authorizationService = engineRule.IAuthorizationService;

            createTenant(TENANT_ONE);

            IUser user = identityService.NewUser(USER_ID);
            identityService.SaveUser(user);

            IGroup group = identityService.NewGroup(GROUP_ID);
            identityService.SaveGroup(group);

            engineRule.ProcessEngineConfiguration.AuthorizationEnabled = true;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        [TearDown]
        public virtual void tearDown()
        {
            identityService.ClearAuthentication();

            identityService.DeleteUser(USER_ID);
            identityService.DeleteGroup(GROUP_ID);
            identityService.DeleteTenant(TENANT_ONE);
            identityService.DeleteTenant(TENANT_TWO);

            engineRule.ProcessEngineConfiguration.AuthorizationEnabled = false;
        }


        [Test]
        public virtual void testCreateTenantUserMembership()
        {

            identityService.CreateTenantUserMembership(TENANT_ONE, USER_ID);

            Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== USER_ID&& c.ResourceType==Resources.Tenant&& c.ResourceId==TENANT_ONE)/*.HasPermission(Permissions.Read)*/.Count());

            identityService.AuthenticatedUserId = USER_ID;

            Assert.AreEqual(TENANT_ONE, identityService.CreateTenantQuery().First().Id);
        }

        [Test]
        public virtual void testCreateAndDeleteTenantUserMembership()
        {

            identityService.CreateTenantUserMembership(TENANT_ONE, USER_ID);

            identityService.DeleteTenantUserMembership(TENANT_ONE, USER_ID);

            Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== USER_ID&& c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());

            identityService.AuthenticatedUserId = USER_ID;

            Assert.AreEqual(0, identityService.CreateTenantQuery().Count());
        }

        [Test]
        public virtual void testCreateAndDeleteTenantUserMembershipForMultipleTenants()
        {

            createTenant(TENANT_TWO);

            identityService.CreateTenantUserMembership(TENANT_ONE, USER_ID);
            identityService.CreateTenantUserMembership(TENANT_TWO, USER_ID);

            Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.UserId== USER_ID&& c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());

            identityService.DeleteTenantUserMembership(TENANT_ONE, USER_ID);

            Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== USER_ID&& c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());
        }

        [Test]
        public virtual void testCreateTenantGroupMembership()
        {

            identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ID);

            Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == GROUP_ID&& c.ResourceType==Resources.Tenant&& c.ResourceId==TENANT_ONE)/*.HasPermission(Permissions.Read)*/.Count());

            identityService.SetAuthentication(USER_ID, new List<string>() { GROUP_ID });

            Assert.AreEqual(TENANT_ONE, identityService.CreateTenantQuery().First().Id);
        }

        [Test]
        public virtual void testCreateAndDeleteTenantGroupMembership()
        {

            identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ID);

            identityService.DeleteTenantGroupMembership(TENANT_ONE, GROUP_ID);

            Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == GROUP_ID&& c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());            

            //identityService.SetAuthentication(USER_ID, Collections.singletonList(GROUP_ID));
            identityService.SetAuthentication(USER_ID, new List<string>() {GROUP_ID});

            Assert.AreEqual(0, identityService.CreateTenantQuery().Count());
        }

        [Test]
        public virtual void testCreateAndDeleteTenantGroupMembershipForMultipleTenants()
        {

            createTenant(TENANT_TWO);

            identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ID);
            identityService.CreateTenantGroupMembership(TENANT_TWO, GROUP_ID);

            Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == GROUP_ID && c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());

            identityService.DeleteTenantGroupMembership(TENANT_ONE, GROUP_ID);

            Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == GROUP_ID&& c.ResourceType==Resources.Tenant)/*.HasPermission(Permissions.Read)*/.Count());
        }

        protected internal virtual ITenant createTenant(string tenantId)
        {
            ITenant newTenant = identityService.NewTenant(tenantId);
            identityService.SaveTenant(newTenant);
            return newTenant;
        }
    }

}