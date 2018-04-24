using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Identity;
using NUnit.Framework;


namespace Engine.Tests.Api.Identity
{
    
	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class AuthorizationServiceWithEnabledAuthorizationTest : PluggableProcessEngineTestCase
	{

        [SetUp]
	  public void setUp()
	  {
		//base.SetUp();
		processEngineConfiguration.SetAuthorizationEnabled(true);
	  }

        [TearDown]
	  public void tearDown()
	  {
		processEngineConfiguration.SetAuthorizationEnabled(false);
		cleanupAfterTest();
		base.TearDown();
	  }

	   [Test]   public virtual void testAuthorizationCheckEmptyDb()
	  {
		TestResource resource1 = new TestResource("resource1",100);
		TestResource resource2 = new TestResource("resource2",101);

		IList<string> jonnysGroups = new List<string>(){ "sales", "marketing" };
		IList<string> someOneElsesGroups = new List<string>() {  "marketing" };// Collections.singletonList("marketing");

		// if no authorizations are in Db, nothing is authorized
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone", someOneElsesGroups, Permissions.Create, (Resources)resource2.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", null, Permissions.Delete, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType(), "someId"));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone", someOneElsesGroups, Permissions.Create, (Resources)resource1.resourceType(), "someId"));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", null, Permissions.Delete, (Resources)resource1.resourceType(), "someOtherId"));
	  }

	   [Test]   public virtual void testUserOverrideGlobalGrantAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1",100);

		// create global authorization which grants all permissions to all users  (on resource1):
		IAuthorization globalGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		globalGrant.Resource = (Resources)resource1.resourceType();
		globalGrant.ResourceId = AuthorizationFields.Any;
		globalGrant.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(globalGrant);

		// revoke Permissions.Read for jonny
		IAuthorization localRevoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		localRevoke.UserId = "jonny";
		localRevoke.Resource = (Resources)resource1.resourceType();
		localRevoke.ResourceId = AuthorizationFields.Any;
		localRevoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(localRevoke);

            IList<string> jonnysGroups = new List<string>() { "sales", "marketing" };
            IList<string> someOneElsesGroups = new List<string>() { "marketing" };

            // jonny does not have Permissions.All permissions
            Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", null, Permissions.All,(Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType()));
		// jonny can't read
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", null, Permissions.Read, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.Read, (Resources)resource1.resourceType()));
		// someone else can
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", someOneElsesGroups, Permissions.Read, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", someOneElsesGroups, Permissions.Read, (Resources)resource1.resourceType()));
		// jonny can still Delete
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.Delete, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.Delete, (Resources)resource1.resourceType()));
	  }

	   [Test]   public virtual void testGroupOverrideGlobalGrantAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1",100);

		// create global authorization which grants all permissions to all users  (on resource1):
		IAuthorization globalGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		globalGrant.Resource = (Resources)resource1.resourceType();
		globalGrant.ResourceId = AuthorizationFields.Any;
		globalGrant.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(globalGrant);

		// revoke Permissions.Read for group "sales"
		IAuthorization groupRevoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		groupRevoke.GroupId = "sales";
		groupRevoke.Resource = (Resources)resource1.resourceType();
		groupRevoke.ResourceId = AuthorizationFields.Any;
		groupRevoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(groupRevoke);

            IList<string> jonnysGroups = new List<string>() { "sales", "marketing" };
            IList<string> someOneElsesGroups = new List<string>() { "marketing" };

            // jonny does not have Permissions.All permissions if queried with groups
            Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType()));
		// if queried without groups he has
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.All, (Resources)resource1.resourceType()));

		// jonny can't read if queried with groups
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.Read, (Resources)resource1.resourceType()));
		// if queried without groups he has
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.Read, (Resources)resource1.resourceType()));

		// someone else who is in group "marketing" but but not "sales" can
		Assert.True(authorizationService.IsUserAuthorized("someone else", someOneElsesGroups, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", someOneElsesGroups, Permissions.Read, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.Read, (Resources)resource1.resourceType()));
		// he could'nt if he were in jonny's groups
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", jonnysGroups, Permissions.All, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", jonnysGroups, Permissions.Read, (Resources)resource1.resourceType()));

		// jonny can still Delete
		Assert.True(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.Delete, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.Delete, (Resources)resource1.resourceType()));
	  }

	   [Test]   public virtual void testUserOverrideGlobalRevokeAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1", 100);

		// create global authorization which revokes all permissions to all users  (on resource1):
		IAuthorization globalGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		globalGrant.Resource = (Resources)resource1.resourceType();
		globalGrant.ResourceId = AuthorizationFields.Any;
		globalGrant.RemovePermission(Permissions.All);
		authorizationService.SaveAuthorization(globalGrant);

		// add Permissions.Read for jonny
		IAuthorization localRevoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		localRevoke.UserId = "jonny";
		localRevoke.Resource = (Resources)resource1.resourceType();
		localRevoke.ResourceId = AuthorizationFields.Any;
		localRevoke.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(localRevoke);

		// jonny does not have Permissions.All permissions
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", null, Permissions.All, (Resources)resource1.resourceType()));
		// jonny can read
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.Read, (Resources)resource1.resourceType()));
		// jonny can't Delete
		Assert.IsFalse(authorizationService.IsUserAuthorized("jonny", null, Permissions.Delete, (Resources)resource1.resourceType()));

		// someone else can't do anything
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", null, Permissions.All, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", null, Permissions.Read, (Resources)resource1.resourceType()));
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", null, Permissions.Delete, (Resources)resource1.resourceType()));
	  }

	   [Test]   public virtual void testNullAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1", 100);
		Assert.IsFalse(authorizationService.IsUserAuthorized(null, null, Permissions.Update, (Resources)resource1.resourceType()));
	  }

	   [Test]   public virtual void testUserOverrideGroupOverrideGlobalAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1",100);

		// create global authorization which grants all permissions to all users  (on resource1):
		IAuthorization globalGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		globalGrant.Resource = (Resources)resource1.resourceType();
		globalGrant.ResourceId = AuthorizationFields.Any;
		globalGrant.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(globalGrant);

		// revoke Permissions.Read for group "sales"
		IAuthorization groupRevoke = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		groupRevoke.GroupId = "sales";
		groupRevoke.Resource = (Resources)resource1.resourceType();
		groupRevoke.ResourceId = AuthorizationFields.Any;
		groupRevoke.RemovePermission(Permissions.Read);
		authorizationService.SaveAuthorization(groupRevoke);

		// add Permissions.Read for jonny
		IAuthorization userGrant = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		userGrant.UserId = "jonny";
		userGrant.Resource = (Resources)resource1.resourceType();
		userGrant.ResourceId = AuthorizationFields.Any;
		userGrant.AddPermission(Permissions.Read);
		authorizationService.SaveAuthorization(userGrant);

            IList<string> jonnysGroups = new List<string>() { "sales", "marketing" };
            IList<string> someOneElsesGroups = new List<string>() { "marketing" };

            // jonny can read
            Assert.True(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.Read, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.Read, (Resources)resource1.resourceType()));

		// someone else in the same groups cannot
		Assert.IsFalse(authorizationService.IsUserAuthorized("someone else", jonnysGroups, Permissions.Read, (Resources)resource1.resourceType()));

		// someone else in different groups can
		Assert.True(authorizationService.IsUserAuthorized("someone else", someOneElsesGroups, Permissions.Read, (Resources)resource1.resourceType()));
	  }

	   [Test]   public virtual void testEnabledAuthorizationCheck()
	  {
		// given
		TestResource resource1 = new TestResource("resource1", 100);

		// when
		bool isAuthorized = authorizationService.IsUserAuthorized("jonny", null, Permissions.Update, (Resources)resource1.resourceType());

		// then
		Assert.IsFalse(isAuthorized);
	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (IUser user in identityService.CreateUserQuery().ToList())
		{
		  identityService.DeleteUser(user.Id);
		}
		foreach (IAuthorization authorization in authorizationService.CreateAuthorizationQuery().ToList())
		{
		  authorizationService.DeleteAuthorization(authorization.Id);
		}
	  }
	}

}