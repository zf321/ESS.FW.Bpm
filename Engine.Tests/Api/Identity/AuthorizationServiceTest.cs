using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
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
	public class AuthorizationServiceTest : PluggableProcessEngineTestCase
	{

	//private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal void tearDown()
	  {
		cleanupAfterTest();
		base.TearDown();
	  }

	  public virtual void testGlobalAuthorizationType()
	  {
		IAuthorization globalAuthorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		// I can set userId = null
		globalAuthorization.UserId = null;
		// I can set userId = AuthorizationFields.Any
		globalAuthorization.UserId = AuthorizationFields.Any;

		try
		{
		  // I cannot set anything else:
		  globalAuthorization.UserId = "something";
		  Assert.Fail("exception expected");

		}
		catch (System.Exception e)
		{
		  AssertTextPresent("ENGINE-03028 Illegal value 'something' for userId for GLOBAL authorization. Must be '*'", e.Message);

		}

		// I can set groupId = null
		globalAuthorization.GroupId = null;

		try
		{
		  // I cannot set anything else:
		  globalAuthorization.GroupId = "something";
		  Assert.Fail("exception expected");

		}
		catch (System.Exception e)
		{
		  AssertTextPresent("ENGINE-03027 Cannot use 'groupId' for GLOBAL authorization", e.Message);
		}
	  }

	  public virtual void testGrantAuthorizationType()
	  {
		IAuthorization grantAuthorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		// I can set userId = null
		grantAuthorization.UserId = null;
		// I can set userId = AuthorizationFields.Any
		grantAuthorization.UserId = AuthorizationFields.Any;
		// I can set anything else:
		grantAuthorization.UserId = "something";
		// I can set groupId = null
		grantAuthorization.GroupId = null;
		// I can set anything else:
		grantAuthorization.GroupId = "something";
	  }

	  public virtual void testRevokeAuthorizationType()
	  {
		IAuthorization revokeAuthorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		// I can set userId = null
		revokeAuthorization.UserId = null;
		// I can set userId = AuthorizationFields.Any
		revokeAuthorization.UserId = AuthorizationFields.Any;
		// I can set anything else:
		revokeAuthorization.UserId = "something";
		// I can set groupId = null
		revokeAuthorization.GroupId = null;
		// I can set anything else:
		revokeAuthorization.GroupId = "something";
	  }

	  public virtual void testDeleteNonExistingAuthorization()
	  {

		try
		{
		  authorizationService.DeleteAuthorization("nonExisiting");
		  Assert.Fail();
		}
		catch (System.Exception e)
		{
		  AssertTextPresent("IAuthorization for Id 'nonExisiting' does not exist: authorization is null", e.Message);
		}

	  }

	  public virtual void testCreateAuthorizationWithUserId()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		// initially, no authorization exists:
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery()/*.Count()*/);

		// simple create / Delete with userId
		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.UserId = "aUserId";
		//authorization.Resource = resource1;

		// save the authorization
		authorizationService.SaveAuthorization(authorization);
		// authorization exists
		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery()/*.Count()*/);
		// Delete the authorization
		authorizationService.DeleteAuthorization(authorization.Id);
		// it's gone
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery().Count());

	  }

	  public virtual void testCreateAuthorizationWithGroupId()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		// initially, no authorization exists:
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery());

		// simple create / Delete with userId
		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.GroupId = "aGroupId";
		////authorization.Resource = resource1;

		// save the authorization
		authorizationService.SaveAuthorization(authorization);
		// authorization exists
		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery()/*.Count()*/);
		// Delete the authorization
		authorizationService.DeleteAuthorization(authorization.Id);
		// it's gone
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery()/*.Count()*/);

	  }

	  public virtual void testInvalidCreateAuthorization()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		// case 1: no user id & no group id ////////////

		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		//authorization.Resource = resource1;

		try
		{
		  authorizationService.SaveAuthorization(authorization);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.True(e.Message.Contains("IAuthorization must either have a 'userId' or a 'groupId'."));
		}

		// case 2: both user id & group id ////////////

		authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.GroupId = "someId";
		authorization.UserId = "someOtherId";
		//authorization.Resource = resource1;

		try
		{
		  authorizationService.SaveAuthorization(authorization);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("IAuthorization must either have a 'userId' or a 'groupId'.", e.Message);
		}

		// case 3: no resourceType ////////////

		authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.UserId = "someId";

		try
		{
		  authorizationService.SaveAuthorization(authorization);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.True(e.Message.Contains("IAuthorization 'resourceType' cannot be null."));
		}

		// case 4: no permissions /////////////////

		authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);
		authorization.UserId = "someId";

		try
		{
		  authorizationService.SaveAuthorization(authorization);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  Assert.True(e.Message.Contains("IAuthorization 'resourceType' cannot be null."));
		}
	  }

	  public virtual void testUniqueUserConstraints()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		IAuthorization authorization1 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		IAuthorization authorization2 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);

		//authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";
		authorization1.UserId = "someUser";

		//authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";
		authorization2.UserId = "someUser";

		// the first one can be saved
		authorizationService.SaveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.SaveAuthorization(authorization2);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException)
		{
		  //expected
		}

		// but I can add a AuthorizationFields.AuthTypeRevoke auth

		IAuthorization authorization3 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);

		//authorization3.Resource = resource1;
		authorization3.ResourceId = "someId";
		authorization3.UserId = "someUser";

		authorizationService.SaveAuthorization(authorization3);

		// but not a second

		IAuthorization authorization4 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);

		//authorization4.Resource = resource1;
		authorization4.ResourceId = "someId";
		authorization4.UserId = "someUser";

		try
		{
		  authorizationService.SaveAuthorization(authorization4);
		  Assert.Fail("exception expected");
		}
		catch (System.Exception)
		{
		  //expected
		}
	  }

	  public virtual void testUniqueGroupConstraints()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		IAuthorization authorization1 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		IAuthorization authorization2 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);

		//authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";
		authorization1.GroupId = "someGroup";

		//authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";
		authorization2.GroupId = "someGroup";

		// the first one can be saved
		authorizationService.SaveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.SaveAuthorization(authorization2);
		  Assert.Fail("exception expected");
		}
		catch (System.Exception)
		{
		  //expected
		}

		// but I can add a AuthorizationFields.AuthTypeRevoke auth

		IAuthorization authorization3 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);

		//authorization3.Resource = resource1;
		authorization3.ResourceId = "someId";
		authorization3.GroupId = "someGroup";

		authorizationService.SaveAuthorization(authorization3);

		// but not a second

		IAuthorization authorization4 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeRevoke);

		//authorization4.Resource = resource1;
		authorization4.ResourceId = "someId";
		authorization4.GroupId = "someGroup";

		try
		{
		  authorizationService.SaveAuthorization(authorization4);
		  Assert.Fail("exception expected");
		}
		catch (System.Exception)
		{
		  //expected
		}

	  }

	  public virtual void testGlobalUniqueConstraints()
	  {

		TestResource resource1 = new TestResource("resource1",100);

		IAuthorization authorization1 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		IAuthorization authorization2 = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);

		////authorization1.Resource = resource1;
		authorization1.ResourceId = "someId";

		////authorization2.Resource = resource1;
		authorization2.ResourceId = "someId";

		// the first one can be saved
		authorizationService.SaveAuthorization(authorization1);

		// the second one cannot
		try
		{
		  authorizationService.SaveAuthorization(authorization2);
		  Assert.Fail("exception expected");
		}
		catch (System.Exception)
		{
		  //expected
		}
	  }

	  public virtual void testUpdateNewAuthorization()
	  {

		TestResource resource1 = new TestResource("resource1",100);
		TestResource resource2 = new TestResource("resource1",101);

		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.UserId = "aUserId";
		//authorization.Resource = resource1;
		authorization.ResourceId = "aResourceId";
		authorization.AddPermission(Permissions.Access);

		// save the authorization
		authorizationService.SaveAuthorization(authorization);

		// validate authorization
		IAuthorization savedAuthorization = authorizationService.CreateAuthorizationQuery().First();
		Assert.AreEqual("aUserId", savedAuthorization.UserId);
		//Assert.AreEqual(resource1.resourceType(), savedAuthorization.ResourceType);
		Assert.AreEqual("aResourceId", savedAuthorization.ResourceId);
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Access));

		// update authorization
		authorization.UserId = "anotherUserId";
		//authorization.Resource = resource2;
		authorization.ResourceId = "anotherResourceId";
		authorization.AddPermission(Permissions.Delete);
		authorizationService.SaveAuthorization(authorization);

		// validate authorization updated
		savedAuthorization = authorizationService.CreateAuthorizationQuery().First();
		Assert.AreEqual("anotherUserId", savedAuthorization.UserId);
		//Assert.AreEqual(resource2.resourceType(), savedAuthorization.ResourceType);
		Assert.AreEqual("anotherResourceId", savedAuthorization.ResourceId);
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Access));
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Delete));

	  }

	  public virtual void testUpdatePersistentAuthorization()
	  {

		TestResource resource1 = new TestResource("resource1",100);
		TestResource resource2 = new TestResource("resource1",101);

		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		authorization.UserId = "aUserId";
		//authorization.Resource = resource1;
		authorization.ResourceId = "aResourceId";
		authorization.AddPermission(Permissions.Access);

		// save the authorization
		authorizationService.SaveAuthorization(authorization);

		// validate authorization
		IAuthorization savedAuthorization = authorizationService.CreateAuthorizationQuery().First();
		Assert.AreEqual("aUserId", savedAuthorization.UserId);
		Assert.AreEqual(resource1.resourceType(), savedAuthorization.ResourceType);
		Assert.AreEqual("aResourceId", savedAuthorization.ResourceId);
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Access));

		// update authorization
		savedAuthorization.UserId = "anotherUserId";
		//savedAuthorization.Resource = resource2;
		savedAuthorization.ResourceId = "anotherResourceId";
		savedAuthorization.AddPermission(Permissions.Delete);
		authorizationService.SaveAuthorization(savedAuthorization);

		// validate authorization updated
		savedAuthorization = authorizationService.CreateAuthorizationQuery().First();
		Assert.AreEqual("anotherUserId", savedAuthorization.UserId);
		Assert.AreEqual(resource2.resourceType(), savedAuthorization.ResourceType);
		Assert.AreEqual("anotherResourceId", savedAuthorization.ResourceId);
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Access));
		Assert.True(savedAuthorization.IsPermissionGranted(Permissions.Delete));

	  }

	  public virtual void testPermissions()
	  {

		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);

		Assert.AreEqual(1, authorization.GetPermissions(authorization.GetPermissions(new Permissions[]{})).Length);

		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Access));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Read));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

		authorization.AddPermission(Permissions.Access);
		Assert.True(authorization.IsPermissionGranted(Permissions.Access));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Read));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

		authorization.AddPermission(Permissions.Delete);
		Assert.True(authorization.IsPermissionGranted(Permissions.Access));
		Assert.True(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Read));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

		authorization.AddPermission(Permissions.Read);
		Assert.True(authorization.IsPermissionGranted(Permissions.Access));
		Assert.True(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

		authorization.AddPermission(Permissions.Update);
		Assert.True(authorization.IsPermissionGranted(Permissions.Access));
		Assert.True(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.Update));

		authorization.RemovePermission(Permissions.Access);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Access));
		Assert.True(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.Update));

		authorization.RemovePermission(Permissions.Delete);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Access));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.Update));

		authorization.RemovePermission(Permissions.Read);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Access));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.Update));

		authorization.RemovePermission(Permissions.Update);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Access));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Delete));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Read));
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.Update));

	  }

	  public virtual void testGrantAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AuthorizationFields.AuthTypeGrant);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.All));
		Assert.True(authorization.IsPermissionGranted(Permissions.None));
		IList<Permissions> perms = (authorization.GetPermissions(authorization.GetPermissions(new Permissions[] { })));
		Assert.True(perms.Contains(Permissions.None));
		Assert.AreEqual(1, perms.Count);

		authorization.AddPermission(Permissions.Read);
		perms = (authorization.GetPermissions(authorization.GetPermissions(new Permissions[] { })));
		Assert.True(perms.Contains(Permissions.None));
		Assert.True(perms.Contains(Permissions.Read));
		Assert.AreEqual(2, perms.Count);
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.None)); // (none is always granted => you are always authorized to do nothing)

		try
		{
		  authorization.IsPermissionRevoked(Permissions.Read);
		  Assert.Fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  AssertTextPresent("ENGINE-03026 Method 'isPermissionRevoked' cannot be used for authorization with type 'GRANT'.", e.Message);
		}

	  }

	  public virtual void testGlobalAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AuthorizationFields.AuthTypeGrant);
		Assert.IsFalse(authorization.IsPermissionGranted(Permissions.All));
		Assert.True(authorization.IsPermissionGranted(Permissions.None));
		IList<Permissions> perms = (authorization.GetPermissions(new Permissions[] { }));
		Assert.True(perms.Contains(Permissions.None));
		Assert.AreEqual(1, perms.Count);

		authorization.AddPermission(Permissions.Read);
		perms = (authorization.GetPermissions(new Permissions[] { }));
		Assert.True(perms.Contains(Permissions.None));
		Assert.True(perms.Contains(Permissions.Read));
		Assert.AreEqual(2, perms.Count);
		Assert.True(authorization.IsPermissionGranted(Permissions.Read));
		Assert.True(authorization.IsPermissionGranted(Permissions.None)); // (none is always granted => you are always authorized to do nothing)

		try
		{
		  authorization.IsPermissionRevoked(Permissions.Read);
		  Assert.Fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  AssertTextPresent("ENGINE-03026 Method 'isPermissionRevoked' cannot be used for authorization with type 'GRANT'.", e.Message);
		}

	  }

	  public virtual void testRevokeAuthPermissions()
	  {

		AuthorizationEntity authorization = new AuthorizationEntity(AuthorizationFields.AuthTypeRevoke);
		Assert.IsFalse(authorization.IsPermissionRevoked(Permissions.All));
		IList<Permissions> perms = (authorization.GetPermissions(new Permissions[] { }));
		Assert.AreEqual(0, perms.Count);

		authorization.RemovePermission(Permissions.Read);
		perms = (authorization.GetPermissions(new Permissions[] { }));
		Assert.True(perms.Contains(Permissions.Read));
		Assert.True(perms.Contains(Permissions.All));
		Assert.AreEqual(2, perms.Count);

		try
		{
		  authorization.IsPermissionGranted(Permissions.Read);
		  Assert.Fail("Exception expected");
		}
		catch (System.InvalidOperationException e)
		{
		  AssertTextPresent("ENGINE-03026 Method 'isPermissionGranted' cannot be used for authorization with type 'REVOKE'.", e.Message);
		}

	  }

	  public virtual void testGlobalGrantAuthorizationCheck()
	  {
		TestResource resource1 = new TestResource("resource1",100);

		// create global authorization which grants all permissions to all users (on resource1):
		IAuthorization globalAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		globalAuth.Resource = (Resources)resource1.resourceType();
		globalAuth.ResourceId = AuthorizationFields.Any;
		globalAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(globalAuth);

		IList<string> jonnysGroups = (new string[]{"sales", "marketing"});
		IList<string> someOneElsesGroups = (new string[]{"marketing"});

		// this authorizes any user to do anything in this resource:
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone", null, Permissions.Create, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone", someOneElsesGroups, Permissions.Create, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.Delete, (Resources)resource1.resourceType()));
		Assert.True(authorizationService.IsUserAuthorized("jonny", null, Permissions.All, (Resources)resource1.resourceType(), "someId"));
		Assert.True(authorizationService.IsUserAuthorized("jonny", jonnysGroups, Permissions.All, (Resources)resource1.resourceType(), "someId"));
		Assert.True(authorizationService.IsUserAuthorized("someone", null, Permissions.Create, (Resources)resource1.resourceType(), "someId"));
		Assert.True(authorizationService.IsUserAuthorized("someone else", null, Permissions.Delete, (Resources)resource1.resourceType(), "someOtherId"));
	  }

	  public virtual void testDisabledAuthorizationCheck()
	  {
		// given
		TestResource resource1 = new TestResource("resource1", 100);

		// when
		bool isAuthorized = authorizationService.IsUserAuthorized("jonny", null, Permissions.Update, (Resources)resource1.resourceType());

		// then
		Assert.True(isAuthorized);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testConcurrentIsUserAuthorized() throws Exception
	 // public virtual void testConcurrentIsUserAuthorized()
	 // {
		//int threadCount = 2;
		//int invocationCount = 500;
		////var executorService = Executors.NewFixedThreadPool(threadCount);

		//try
		//{
		//  List<Func<Exception>> callables = new List<Func<Exception>>();

  //              for (int i = 0; i < invocationCount; i++)
  //              {
  //                  callables.Add(() =>
  //                  {
  //                      try
  //                      {
  //                          authorizationService.IsUserAuthorized(null, null, Permissions.None, Resources.Application, null);
  //                      }
  //                      catch (Exception e)
  //                      {
  //                          return e;
  //                      }
  //                      return null;
  //                  });
  //              }

  //              IList<Func<Exception>> futures = executorService.invokeAll(callables);

  //              foreach (Func<Exception> future in futures)
  //              {
  //                  Exception exception = future.Get();
  //                  if (exception != null)
  //                  {
  //                      Assert.Fail("No exception expected: " + exception.Message);
  //                  }
  //              }

  //          }
		//finally
		//{
		//  // reset original logging level
		////  executorService.shutdownNow();
		// // executorService.awaitTermination(10, TimeUnit.SECONDS);
		//}

	 // }


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