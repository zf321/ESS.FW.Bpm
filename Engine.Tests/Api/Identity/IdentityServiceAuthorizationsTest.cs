using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;


namespace Engine.Tests.Api.Identity
{


    /// <summary>
	/// 
	/// 
	/// </summary>
	public class IdentityServiceAuthorizationsTest : PluggableProcessEngineTestCase
	{

	  private const string jonny2 = "jonny2";

        [TearDown]
	  protected internal void tearDown()
	  {
		processEngineConfiguration.SetAuthorizationEnabled(false);
		cleanupAfterTest();
		base.TearDown();
	  }
        
	  [Test]  public virtual void testUserCreateAuthorizations()
	  {

		// add base permission which allows nobody to create users:
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.User;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.NewUser("jonny1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
          AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.User.ToString()/*.ResourceName()*/, null, info);
		}

		// circumvent auth check to get new transient userobject
		IUser newUser = new UserEntity("jonny1");

		try
		{
		  identityService.SaveUser(newUser);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.User.ToString()/*.ResourceName()*/, null, info);
		}
	  }

	  [Test]  public virtual void testUserDeleteAuthorizations()
	  {

		// crate user while still in god-mode:
		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.User;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Delete); // revoke Delete
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteUser("jonny1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.User.ToString()/*.ResourceName()*/, "jonny1", info);
		}
	  }

	  [Test]  public virtual void testUserUpdateAuthorizations()
	  {

		// crate user while still in god-mode:
		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.User;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Update); // revoke update
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		jonny1 = identityService.CreateUserQuery().First();
		jonny1.FirstName = "Jonny";

		try
		{
		  identityService.SaveUser(jonny1);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Update.ToString(), Resources.User.ToString()/*.ResourceName()*/, "jonny1", info);
		}

		// but I can create a new user:
		IUser jonny3 = identityService.NewUser("jonny3");
		identityService.SaveUser(jonny3);

	  }

	  [Test]  public virtual void testGroupCreateAuthorizations()
	  {

		// add base permission which allows nobody to create groups:
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Group;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.NewGroup("group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Group.ToString()/*.ResourceName()*/, null, info);
		}

		// circumvent auth check to get new transient userobject
		IGroup group = new GroupEntity("group1");

		try
		{
		  identityService.SaveGroup(group);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Group.ToString()/*.ResourceName()*/, null, info);
		}
	  }

	  [Test]  public virtual void testGroupDeleteAuthorizations()
	  {

		// crate group while still in god-mode:
		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Group;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Delete); // revoke Delete
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteGroup("group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.Group.ToString()/*.ResourceName()*/, "group1", info);
		}

	  }


	  [Test]  public virtual void testGroupUpdateAuthorizations()
	  {

		// crate group while still in god-mode:
		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Group;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Update); // revoke update
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		group1 = identityService.CreateGroupQuery().First();
		//group1.ToString() = "IGroup 1";

		try
		{
		  identityService.SaveGroup(group1);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Update.ToString(), Resources.Group.ToString()/*.ResourceName()*/, "group1", info);
		}

		// but I can create a new group:
		IGroup group2 = identityService.NewGroup("group2");
		identityService.SaveGroup(group2);

	  }

	  [Test]  public virtual void testTenantCreateAuthorizations()
	  {

		// add base permission which allows nobody to create tenants:
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Tenant;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.NewTenant("tenant");

		  Assert.Fail("exception expected");
		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Tenant.ToString()/*.ResourceName()*/, null, info);
		}

		// circumvent auth check to get new transient userobject
		ITenant tenant = new TenantEntity("tenant");

		try
		{
		  identityService.SaveTenant(tenant);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Tenant.ToString()/*.ResourceName()*/, null, info);
		}
	  }

	  [Test]  public virtual void testTenantDeleteAuthorizations()
	  {

		// create tenant
		ITenant tenant = new TenantEntity("tenant");
		identityService.SaveTenant(tenant);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Tenant;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Delete); // revoke Delete
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteTenant("tenant");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.Tenant.ToString()/*.ResourceName()*/, "tenant", info);
		}
	  }

	  [Test]  public virtual void testTenantUpdateAuthorizations()
	  {

		// create tenant
		ITenant tenant = new TenantEntity("tenant");
		identityService.SaveTenant(tenant);

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Tenant;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Update); // revoke update
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		// fetch user:
		tenant = identityService.CreateTenantQuery().First();
		tenant.Name = "newName";

		try
		{
		  identityService.SaveTenant(tenant);

		  Assert.Fail("exception expected");
		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Update.ToString(), Resources.Tenant.ToString()/*.ResourceName()*/, "tenant", info);
		}

		// but I can create a new tenant:
		ITenant newTenant = identityService.NewTenant("newTenant");
		identityService.SaveTenant(newTenant);
	  }

	  [Test]  public virtual void testMembershipCreateAuthorizations()
	  {

		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		// add base permission which allows nobody to add users to groups
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.GroupMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'crate'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.CreateMembership("jonny1", "group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.GroupMembership.ToString()/*.ResourceName()*/, "group1", info);
		}
	  }

	  [Test]  public virtual void testMembershipDeleteAuthorizations()
	  {

		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		// add base permission which allows nobody to add users to groups
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.GroupMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'Delete'
		basePerms.RemovePermission(Permissions.Delete);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteMembership("jonny1", "group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.GroupMembership.ToString()/*.ResourceName()*/, "group1", info);
		}
	  }

	  [Test]  public virtual void testTenantUserMembershipCreateAuthorizations()
	  {

		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		ITenant tenant1 = identityService.NewTenant("tenant1");
		identityService.SaveTenant(tenant1);

		// add base permission which allows nobody to create memberships
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.TenantMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.CreateTenantUserMembership("tenant1", "jonny1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.TenantMembership.ToString()/*.ResourceName()*/, "tenant1", info);
		}
	  }

	  [Test]  public virtual void testTenantGroupMembershipCreateAuthorizations()
	  {

		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		ITenant tenant1 = identityService.NewTenant("tenant1");
		identityService.SaveTenant(tenant1);

		// add base permission which allows nobody to create memberships
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.TenantMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.CreateTenantGroupMembership("tenant1", "group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.TenantMembership.ToString()/*.ResourceName()*/, "tenant1", info);
		}
	  }

	  [Test]  public virtual void testTenantUserMembershipDeleteAuthorizations()
	  {

		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		ITenant tenant1 = identityService.NewTenant("tenant1");
		identityService.SaveTenant(tenant1);

		// add base permission which allows nobody to Delete memberships
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.TenantMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'Delete'
		basePerms.RemovePermission(Permissions.Delete);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteTenantUserMembership("tenant1", "jonny1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.TenantMembership.ToString()/*.ResourceName()*/, "tenant1", info);
		}
	  }

	  [Test]  public virtual void testTenanGroupMembershipDeleteAuthorizations()
	  {

		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		ITenant tenant1 = identityService.NewTenant("tenant1");
		identityService.SaveTenant(tenant1);

		// add base permission which allows nobody to Delete memberships
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.TenantMembership;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'Delete'
		basePerms.RemovePermission(Permissions.Delete);
		authorizationService.SaveAuthorization(basePerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  identityService.DeleteTenantGroupMembership("tenant1", "group1");
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.TenantMembership.ToString()/*.ResourceName()*/, "tenant1", info);
		}
	  }

	  [Test]  public virtual void testUserQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);

		// set base permission for all users (no-one has any permissions on users)
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.User;
		basePerms.ResourceId = AuthorizationFields.Any;
		authorizationService.SaveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we cannot fetch the user
		Assert.IsNull(identityService.CreateUserQuery().First());
		Assert.AreEqual(0, identityService.CreateUserQuery().Count());

		processEngineConfiguration.SetAuthorizationEnabled(false);

		// now we add permission for jonny2 to read the user:
		IAuthorization ourPerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = Resources.User;
		ourPerms.ResourceId = AuthorizationFields.Any;
		ourPerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);

		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we can fetch the user
		Assert.NotNull(identityService.CreateUserQuery().First());
		Assert.AreEqual(1, identityService.CreateUserQuery().Count());

		// change the base permission:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		basePerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.User && c.UserId == "*").First();
		basePerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(basePerms);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we can still fetch the user
		Assert.NotNull(identityService.CreateUserQuery().First());
		Assert.AreEqual(1, identityService.CreateUserQuery().Count());


		// revoke permission for jonny2:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		ourPerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.User&& c.UserId == authUserId).First();
		ourPerms.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);

		IAuthorization revoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		revoke.UserId = authUserId;
		revoke.Resource = Resources.User;
		revoke.ResourceId = AuthorizationFields.Any;
		revoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(revoke);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we cannot fetch the user
		Assert.IsNull(identityService.CreateUserQuery().First());
		Assert.AreEqual(0, identityService.CreateUserQuery().Count());


		// Delete our perms
		processEngineConfiguration.SetAuthorizationEnabled(false);
		authorizationService.DeleteAuthorization(ourPerms.Id);
		authorizationService.DeleteAuthorization(revoke.Id);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now the base permission applies and grants us read access
		Assert.NotNull(identityService.CreateUserQuery().First());
		Assert.AreEqual(1, identityService.CreateUserQuery().Count());

	  }

	  [Test]  public virtual void testUserQueryAuthorizationsMultipleGroups()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		IUser demo = identityService.NewUser("demo");
		identityService.SaveUser(demo);

		IUser mary = identityService.NewUser("mary");
		identityService.SaveUser(mary);

		IUser peter = identityService.NewUser("peter");
		identityService.SaveUser(peter);

		IUser john = identityService.NewUser("john");
		identityService.SaveUser(john);

		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);

		IGroup accounting = identityService.NewGroup("accounting");
		identityService.SaveGroup(accounting);

		IGroup management = identityService.NewGroup("management");
		identityService.SaveGroup(management);

		identityService.CreateMembership("demo", "sales");
		identityService.CreateMembership("demo", "accounting");
		identityService.CreateMembership("demo", "management");

		identityService.CreateMembership("john", "sales");
		identityService.CreateMembership("mary", "accounting");
		identityService.CreateMembership("peter", "management");

		IAuthorization demoAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		demoAuth.UserId = "demo";
		demoAuth.Resource = Resources.User;
		demoAuth.ResourceId = "demo";
		demoAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(demoAuth);

		IAuthorization johnAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		johnAuth.UserId = "john";
		johnAuth.Resource = Resources.User;
		johnAuth.ResourceId = "john";
		johnAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(johnAuth);

		IAuthorization maryAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		maryAuth.UserId = "mary";
		maryAuth.Resource = Resources.User;
		maryAuth.ResourceId = "mary";
		maryAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(maryAuth);

		IAuthorization peterAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		peterAuth.UserId = "peter";
		peterAuth.Resource = Resources.User;
		peterAuth.ResourceId = "peter";
		peterAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(peterAuth);

		IAuthorization accAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		accAuth.GroupId = "accounting";
		accAuth.Resource = Resources.Group;
		accAuth.ResourceId = "accounting";
		accAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(accAuth);

		IAuthorization salesAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		salesAuth.GroupId = "sales";
		salesAuth.Resource = Resources.Group;
		salesAuth.ResourceId = "sales";
		salesAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(salesAuth);

		IAuthorization manAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		manAuth.GroupId = "management";
		manAuth.Resource = Resources.Group;
		manAuth.ResourceId = "management";
		manAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(manAuth);

		IAuthorization salesDemoAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		salesDemoAuth.GroupId = "sales";
		salesDemoAuth.Resource = Resources.User;
		salesDemoAuth.ResourceId = "demo";
		salesDemoAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(salesDemoAuth);

		IAuthorization salesJohnAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		salesJohnAuth.GroupId = "sales";
		salesJohnAuth.Resource = Resources.User;
		salesJohnAuth.ResourceId = "john";
		salesJohnAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(salesJohnAuth);

		IAuthorization manDemoAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		manDemoAuth.GroupId = "management";
		manDemoAuth.Resource = Resources.User;
		manDemoAuth.ResourceId = "demo";
		manDemoAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(manDemoAuth);

		IAuthorization manPeterAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		manPeterAuth.GroupId = "management";
		manPeterAuth.Resource = Resources.User;
		manPeterAuth.ResourceId = "peter";
		manPeterAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(manPeterAuth);

		IAuthorization accDemoAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		accDemoAuth.GroupId = "accounting";
		accDemoAuth.Resource = Resources.User;
		accDemoAuth.ResourceId = "demo";
		accDemoAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(accDemoAuth);

		IAuthorization accMaryAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		accMaryAuth.GroupId = "accounting";
		accMaryAuth.Resource = Resources.User;
		accMaryAuth.ResourceId = "mary";
		accMaryAuth.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(accMaryAuth);

		IList<string> groups = new List<string>();
		groups.Add("management");
		groups.Add("accounting");
		groups.Add("sales");

		identityService.SetAuthentication("demo", groups);

		processEngineConfiguration.SetAuthorizationEnabled(true);

		IList<IUser> salesUser = identityService.CreateUserQuery()
                //.MemberOfGroup("sales")
                .ToList();
		Assert.AreEqual(2, salesUser.Count);

		foreach (IUser user in salesUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("john"))
		  {
			Assert.Fail("Unexpected user for group sales: " + user.Id);
		  }
		}

		IList<IUser> accountingUser = identityService.CreateUserQuery()//.MemberOfGroup("accounting")
                .ToList();
		Assert.AreEqual(2, accountingUser.Count);

		foreach (IUser user in accountingUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("mary"))
		  {
			Assert.Fail("Unexpected user for group accounting: " + user.Id);
		  }
		}

		IList<IUser> managementUser = identityService.CreateUserQuery()//.MemberOfGroup("management")
                .ToList();
		Assert.AreEqual(2, managementUser.Count);

		foreach (IUser user in managementUser)
		{
		  if (!user.Id.Equals("demo") && !user.Id.Equals("peter"))
		  {
			Assert.Fail("Unexpected user for group managment: " + user.Id);
		  }
		}
	  }

	  [Test]  public virtual void testGroupQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);
		// create new group
		IGroup group1 = identityService.NewGroup("group1");
		identityService.SaveGroup(group1);

		// set base permission for all users (no-one has any permissions on groups)
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Group;
		basePerms.ResourceId = AuthorizationFields.Any;
		authorizationService.SaveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we cannot fetch the group
		Assert.IsNull(identityService.CreateGroupQuery().First());
		Assert.AreEqual(0, identityService.CreateGroupQuery().Count());

		// now we add permission for jonny2 to read the group:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		IAuthorization ourPerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = Resources.Group;
		ourPerms.ResourceId = AuthorizationFields.Any;
		ourPerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we can fetch the group
		Assert.NotNull(identityService.CreateGroupQuery().First());
		Assert.AreEqual(1, identityService.CreateGroupQuery().Count());

		// change the base permission:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		basePerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.Group&& c.UserId == "*").First();
		basePerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(basePerms);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we can still fetch the group
		Assert.NotNull(identityService.CreateGroupQuery().First());
		Assert.AreEqual(1, identityService.CreateGroupQuery().Count());

		// revoke permission for jonny2:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		ourPerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.Group&& c.UserId == authUserId).First();
		ourPerms.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);

		IAuthorization revoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		revoke.UserId = authUserId;
		revoke.Resource = Resources.Group;
		revoke.ResourceId = AuthorizationFields.Any;
		revoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(revoke);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we cannot fetch the group
		Assert.IsNull(identityService.CreateGroupQuery().First());
		Assert.AreEqual(0, identityService.CreateGroupQuery().Count());

		// Delete our perms
		processEngineConfiguration.SetAuthorizationEnabled(false);
		authorizationService.DeleteAuthorization(ourPerms.Id);
		authorizationService.DeleteAuthorization(revoke.Id);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now the base permission applies and grants us read access
		Assert.NotNull(identityService.CreateGroupQuery().First());
		Assert.AreEqual(1, identityService.CreateGroupQuery().Count());

	  }

	  [Test]  public virtual void testTenantQueryAuthorizations()
	  {
		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new user jonny1
		IUser jonny1 = identityService.NewUser("jonny1");
		identityService.SaveUser(jonny1);
		// create new tenant
		ITenant tenant = identityService.NewTenant("tenant");
		identityService.SaveTenant(tenant);

		// set base permission for all users (no-one has any permissions on tenants)
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Tenant;
		basePerms.ResourceId = AuthorizationFields.Any;
		authorizationService.SaveAuthorization(basePerms);

		// now enable checks
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we cannot fetch the tenants
		Assert.AreEqual(0, identityService.CreateTenantQuery().Count());

		// now we add permission for jonny2 to read the tenants:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		IAuthorization ourPerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		ourPerms.UserId = authUserId;
		ourPerms.Resource = Resources.Tenant;
		ourPerms.ResourceId = AuthorizationFields.Any;
		ourPerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we can fetch the tenants
		Assert.AreEqual(1, identityService.CreateTenantQuery().Count());

		// change the base permission:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		basePerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.Tenant&& c.UserId == "*").First();
		basePerms.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(basePerms);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// we can still fetch the tenants
		Assert.AreEqual(1, identityService.CreateTenantQuery().Count());

		// revoke permission for jonny2:
		processEngineConfiguration.SetAuthorizationEnabled(false);
		ourPerms = authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==Resources.Tenant&& c.UserId == authUserId).First();
		ourPerms.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(ourPerms);

		IAuthorization revoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		revoke.UserId = authUserId;
		revoke.Resource = Resources.Tenant;
		revoke.ResourceId = AuthorizationFields.Any;
		revoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(revoke);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now we cannot fetch the tenants
		Assert.AreEqual(0, identityService.CreateTenantQuery().Count());

		// Delete our permissions
		processEngineConfiguration.SetAuthorizationEnabled(false);
		authorizationService.DeleteAuthorization(ourPerms.Id);
		authorizationService.DeleteAuthorization(revoke.Id);
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// now the base permission applies and grants us read access
		Assert.AreEqual(1, identityService.CreateTenantQuery().Count());
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (IGroup group in identityService.CreateGroupQuery().ToList())
		{
		  identityService.DeleteGroup(group.Id);
		}
		foreach (IUser user in identityService.CreateUserQuery().ToList())
		{
		  identityService.DeleteUser(user.Id);
		}
		foreach (ITenant tenant in identityService.CreateTenantQuery().ToList())
		{
		  identityService.DeleteTenant(tenant.Id);
		}
		foreach (IAuthorization authorization in authorizationService.CreateAuthorizationQuery().ToList())
		{
		  authorizationService.DeleteAuthorization(authorization.Id);
		}
	  }

	}

}