using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Identity;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{

	public class IdentityServiceTenantTest
	{

	  protected internal const string USER_ONE = "user1";
	  protected internal const string USER_TWO = "user2";

	  protected internal const string GROUP_ONE = "group1";
	  protected internal const string GROUP_TWO = "group2";

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();



	  protected internal IIdentityService identityService;

        [SetUp]
	  public virtual void initService()
	  {
		identityService = engineRule.IdentityService;
	  }

        [TearDown]
	  public virtual void cleanUp()
	  {
		identityService.DeleteTenant(TENANT_ONE);
		identityService.DeleteTenant(TENANT_TWO);

		identityService.DeleteGroup(GROUP_ONE);
		identityService.DeleteGroup(GROUP_TWO);

		identityService.DeleteUser(USER_ONE);
		identityService.DeleteUser(USER_TWO);
	  }

        [Test]
	  public virtual void createTenant()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		tenant.Name = "Tenant";
		identityService.SaveTenant(tenant);

		tenant = identityService.CreateTenantQuery().First();
		Assert.That(tenant!=null);
		Assert.That(tenant.Id, Is.EqualTo(TENANT_ONE));
		Assert.That(tenant.Name, Is.EqualTo("Tenant"));
	  }


        [Test]
        public virtual void updateTenant()
	  {
		// create
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		tenant.Name = "Tenant";
		identityService.SaveTenant(tenant);

		// update
		tenant = identityService.CreateTenantQuery().First();
		Assert.That(tenant!=null);

		tenant.Name = "newName";
		identityService.SaveTenant(tenant);

		tenant = identityService.CreateTenantQuery().First();
		Assert.AreEqual("newName", tenant.Name);
	  }


        [Test]
        public virtual void DeleteTenant()
	  {
		// create
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IQueryable<ITenant> query = identityService.CreateTenantQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenant("nonExisting");
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenant(TENANT_ONE);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void updateTenantOptimisticLockingException()
	  {
		// create
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		ITenant tenant1 = identityService.CreateTenantQuery().First();
		ITenant tenant2 = identityService.CreateTenantQuery().First();

		// update
		tenant1.Name = "name";
		identityService.SaveTenant(tenant1);

            //thrown.Expect(typeof(ProcessEngineException));

		// Assert.Fail to update old revision
		tenant2.Name = "other name";
		identityService.SaveTenant(tenant2);
	  }


        [Test]
        public virtual void createTenantWithGenericResourceId()
	  {
		ITenant tenant = identityService.NewTenant("*");

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		identityService.SaveTenant(tenant);
	  }


        [Test]
        public virtual void createTenantMembershipUnexistingTenant()
	  {
		IUser user = identityService.NewUser(USER_ONE);
		identityService.SaveUser(user);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("No tenant found with id 'nonExisting'.");

		identityService.CreateTenantUserMembership("nonExisting", user.Id);
	  }


        [Test]
        public virtual void createTenantMembershipUnexistingUser()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("No user found with id 'nonExisting'.");

		identityService.CreateTenantUserMembership(tenant.Id, "nonExisting");
	  }


        [Test]
        public virtual void createTenantMembershipUnexistingGroup()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("No group found with id 'nonExisting'.");

		identityService.CreateTenantGroupMembership(tenant.Id, "nonExisting");
	  }


        [Test]
        public virtual void createTenantUserMembershipAlreadyExisting()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IUser user = identityService.NewUser(USER_ONE);
		identityService.SaveUser(user);

		identityService.CreateTenantUserMembership(TENANT_ONE, USER_ONE);

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.CreateTenantUserMembership(TENANT_ONE, USER_ONE);
	  }


        [Test]
        public virtual void createTenantGroupMembershipAlreadyExisting()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IGroup group = identityService.NewGroup(GROUP_ONE);
		identityService.SaveGroup(group);

		identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ONE);
	  }


        [Test]
        public virtual void DeleteTenantUserMembership()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IUser user = identityService.NewUser(USER_ONE);
		identityService.SaveUser(user);

		identityService.CreateTenantUserMembership(TENANT_ONE, USER_ONE);

		IQueryable<ITenant> query = identityService.CreateTenantQuery()/*.UserMember(USER_ONE)*/;
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantUserMembership("nonExisting", USER_ONE);
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantUserMembership(TENANT_ONE, "nonExisting");
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantUserMembership(TENANT_ONE, USER_ONE);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void DeleteTenantGroupMembership()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IGroup group = identityService.NewGroup(GROUP_ONE);
		identityService.SaveGroup(group);

		identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		IQueryable<ITenant> query = identityService.CreateTenantQuery()/*.GroupMember(GROUP_ONE)*/;
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantGroupMembership("nonExisting", GROUP_ONE);
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantGroupMembership(TENANT_ONE, "nonExisting");
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteTenantGroupMembership(TENANT_ONE, GROUP_ONE);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteTenantMembershipsWileDeleteUser()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IUser user = identityService.NewUser(USER_ONE);
		identityService.SaveUser(user);

		identityService.CreateTenantUserMembership(TENANT_ONE, USER_ONE);

		IQueryable<ITenant> query = identityService.CreateTenantQuery()/*.UserMember(USER_ONE)*/;
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteUser(USER_ONE);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteTenantMembershipsWhileDeleteGroup()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IGroup group = identityService.NewGroup(GROUP_ONE);
		identityService.SaveGroup(group);

		identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		IQueryable<ITenant> query = identityService.CreateTenantQuery()/*.GroupMember(GROUP_ONE)*/;
		Assert.That(query.Count(), Is.EqualTo(1L));

		identityService.DeleteGroup(GROUP_ONE);
		Assert.That(query.Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void deleteTenantMembershipsOfTenant()
	  {
		ITenant tenant = identityService.NewTenant(TENANT_ONE);
		identityService.SaveTenant(tenant);

		IUser user = identityService.NewUser(USER_ONE);
		identityService.SaveUser(user);

		IGroup group = identityService.NewGroup(GROUP_ONE);
		identityService.SaveGroup(group);

		identityService.CreateTenantUserMembership(TENANT_ONE, USER_ONE);
		identityService.CreateTenantGroupMembership(TENANT_ONE, GROUP_ONE);

		//IQueryable<IUser> userQuery = identityService.CreateUserQuery().MemberOfTenant(TENANT_ONE);
		//IGroupQuery  groupQuery = identityService.CreateGroupQuery().MemberOfTenant(TENANT_ONE);
		//Assert.That(userQuery.Count(), Is.EqualTo(1L));
		//Assert.That(groupQuery.Count(), Is.EqualTo(1L));

		//identityService.DeleteTenant(TENANT_ONE);
		//Assert.That(userQuery.Count(), Is.EqualTo(0L));
		//Assert.That(groupQuery.Count(), Is.EqualTo(0L));
	  }

	}

}