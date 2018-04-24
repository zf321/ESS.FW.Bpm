using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using NUnit.Framework;


namespace Engine.Tests.Api.Identity
{
    

	/// <summary>
	/// <para>Test authorizations provided by <seealso cref="DefaultAuthorizationProvider"/></para>
	/// 
	/// 
	/// 
	/// </summary>
	public class DefaultAuthorizationProviderTest : PluggableProcessEngineTestCase
	{

        [SetUp]
	  protected internal virtual void setUp()
	  {
		// we are jonny
		identityService.AuthenticatedUserId = "jonny";
		// make sure we can do stuff:
		IAuthorization jonnyIsGod = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = Resources.User;
		jonnyIsGod.ResourceId = AuthorizationFields.Any;
		jonnyIsGod.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(jonnyIsGod);

		jonnyIsGod = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = Resources.Group;
		jonnyIsGod.ResourceId = AuthorizationFields.Any;
		jonnyIsGod.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(jonnyIsGod);

		jonnyIsGod = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		jonnyIsGod.UserId = "jonny";
		jonnyIsGod.Resource = Resources.Authorization;
		jonnyIsGod.ResourceId = AuthorizationFields.Any;
		jonnyIsGod.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(jonnyIsGod);

		// enable authorizations
		processEngineConfiguration.SetAuthorizationEnabled(true);
		base.InitializeServices();
       //base.SetUp();
            

      }

        [TearDown]
	  protected internal virtual void tearDown()
	  {
		processEngineConfiguration.SetAuthorizationEnabled(false);
	      IList<IAuthorization> jonnysAuths = authorizationService.CreateAuthorizationQuery(c => c.UserId == "jonny")
	          .ToList();
		foreach (IAuthorization authorization in jonnysAuths)
		{
		  authorizationService.DeleteAuthorization(authorization.Id);
		}
		base.TearDown();
	  }
        [Test]
	  public virtual void testCreateUser()
	  {
		// initially there are no authorizations for jonny2:
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "jonny2").Count());

		// create new user
		identityService.SaveUser(identityService.NewUser("jonny2"));

		// now there is an authorization for jonny2 which grants him Permissions.All permissions on himself
		IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.UserId== "jonny2").First();
		Assert.NotNull(authorization);
		Assert.AreEqual(AuthorizationFields.AuthTypeGrant, authorization.AuthorizationType);
		Assert.AreEqual(Resources.User, authorization.ResourceType);//.ResourceType()
            Assert.AreEqual("jonny2", authorization.ResourceId);
		Assert.True(authorization.IsPermissionGranted(Permissions.All));

		// Delete the user
		identityService.DeleteUser("jonny2");

		// the authorization is deleted as well:
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "jonny2").Count());
	  }

        [Test]
        public virtual void testCreateGroup()
	  {
		// initially there are no authorizations for group "sales":
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "sales").Count());

		// create new group
		identityService.SaveGroup(identityService.NewGroup("sales"));
            
		IAuthorization authorization = authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "sales").First();
		Assert.NotNull(authorization);
		Assert.AreEqual(AuthorizationFields.AuthTypeGrant, authorization.AuthorizationType);
		Assert.AreEqual(Resources.Group, authorization.ResourceType);//ResourceType()
            Assert.AreEqual("sales", authorization.ResourceId);
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));

		// Delete the group
		identityService.DeleteGroup("sales");

		// the authorization is deleted as well:
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "sales").Count());
	  }

	}

}