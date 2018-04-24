using System.Linq;
using Engine.Tests.Api.Authorization.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{
    

	/// <summary>
	/// <para>Ensures authorizations are properly
	/// enforced by the <seealso cref="IAuthorizationService"/></para>
	/// 
	/// 
	/// 
	/// </summary>
	public class AuthorizationServiceAuthorizationsTest : PluggableProcessEngineTestCase
	{

	  private const string jonny2 = "jonny2";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
	  protected internal  void tearDown()
	  {
		processEngineConfiguration.SetAuthorizationEnabled(false);
		cleanupAfterTest();
		base.TearDown();
	  }

	  public virtual void testCreateAuthorization()
	  {

		// add base permission which allows nobody to create authorizations
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Authorization;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All); // add all then remove 'create'
		basePerms.RemovePermission(Permissions.Create);
		authorizationService.SaveAuthorization(basePerms);

		// now enable authorizations:
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  // we cannot create another authorization
		  authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Authorization.ToString()/*.ResourceName()*/, null, info);
		}

		// circumvent auth check to get new transient object
		IAuthorization authorization = new AuthorizationEntity(AuthorizationFields.AuthTypeRevoke);
		authorization.UserId = "someUserId";
		authorization.Resource = Resources.Application;

		try
		{
		  authorizationService.SaveAuthorization(authorization);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
          AuthorizationTestUtil.AssertExceptionInfo(Permissions.Create.ToString(), Resources.Authorization.ToString()/*.ResourceName()*/, null, info);
		}
	  }

	  public virtual void testDeleteAuthorization()
	  {

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Authorization;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Delete); // revoke Delete
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		try
		{
		  // try to Delete authorization
		  authorizationService.DeleteAuthorization(basePerms.Id);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Delete.ToString(), Resources.Authorization.ToString()/*.ResourceName()*/, basePerms.Id, info);
		}
	  }

	  public virtual void testUserUpdateAuthorizations()
	  {

		// create global auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Authorization;
		basePerms.ResourceId = AuthorizationFields.Any;
		basePerms.AddPermission(Permissions.All);
		basePerms.RemovePermission(Permissions.Update); // revoke update
		authorizationService.SaveAuthorization(basePerms);

		// turn on authorization
		processEngineConfiguration.SetAuthorizationEnabled(true);
		identityService.AuthenticatedUserId = jonny2;

		// fetch authhorization
		basePerms = authorizationService.CreateAuthorizationQuery().First();
		// make some change to the perms
		basePerms.AddPermission(Permissions.All);

		try
		{
		  authorizationService.SaveAuthorization(basePerms);
		  Assert.Fail("exception expected");

		}
		catch (AuthorizationException e)
		{
		  Assert.AreEqual(1, e.MissingAuthorizations.Count);
		  MissingAuthorization info = e.MissingAuthorizations[0];
		  Assert.AreEqual(jonny2, e.UserId);
		  AuthorizationTestUtil.AssertExceptionInfo(Permissions.Update.ToString(), Resources.Authorization.ToString()/*.ResourceName()*/, basePerms.Id, info);
		}

		// but we can create a new auth
		IAuthorization newAuth = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
		newAuth.UserId = "jonny2";
		newAuth.Resource = Resources.Authorization;
		newAuth.ResourceId = AuthorizationFields.Any;
		newAuth.AddPermission(Permissions.All);
		authorizationService.SaveAuthorization(newAuth);

	  }

	  public virtual void testAuthorizationQueryAuthorizations()
	  {

		// we are jonny2
		string authUserId = "jonny2";
		identityService.AuthenticatedUserId = authUserId;

		// create new auth wich revokes read access on auth
		IAuthorization basePerms = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGlobal);
		basePerms.Resource = Resources.Authorization;
		basePerms.ResourceId = AuthorizationFields.Any;
		authorizationService.SaveAuthorization(basePerms);

		// I can see it
		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery().Count());

		// now enable checks
		processEngineConfiguration.SetAuthorizationEnabled(true);

		// I can't see it
		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery().Count());

	  }

	  protected internal virtual void cleanupAfterTest()
	  {
		foreach (IAuthorization authorization in authorizationService.CreateAuthorizationQuery().ToList())
		{
		  authorizationService.DeleteAuthorization(authorization.Id);
		}
	  }

	}

}