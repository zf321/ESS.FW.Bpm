//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Repository;
//using NUnit.Framework;
////using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Identity
//{
    

//	/// <summary>
//	/// 
//	/// 
//	/// </summary>
//	public class AuthorizationQueryTest : PluggableProcessEngineTestCase
//	{

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: @Override protected void setUp() throws Exception
//    [SetUp]
//	  protected internal void setUp()
//	  {
//            //base.SetUp();
//        var resource1  = new TestResource("resource1", 100) ;
//		var resource2 = new TestResource("resource2", 101) ;

//		createAuthorization("user1", null, (Resources)resource1.resourceType(), "resource1-1",new []{ Permissions.Access } );
//		createAuthorization("user1", null, (Resources)resource2.resourceType(), "resource2-1", new[] { Permissions.Delete } );
//		createAuthorization("user2", null, (Resources)resource1.resourceType(), "resource1-2");
//		createAuthorization("user3", null, (Resources)resource2.resourceType(), "resource2-1", Permissions.Read, Permissions.Update);

//		createAuthorization(null, "group1", (Resources)resource1.resourceType(), "resource1-1");
//		createAuthorization(null, "group1", (Resources)resource1.resourceType(), "resource1-2", new[] { Permissions.Update } );
//		createAuthorization(null, "group2", (Resources)resource2.resourceType(), "resource2-2", Permissions.Read, Permissions.Update);
//		createAuthorization(null, "group3", (Resources)resource2.resourceType(), "resource2-3", new[] { Permissions.Delete } );

//	  }
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: protected void tearDown() throws Exception
//	  protected internal virtual void tearDown()
//	  {
//		IList<IAuthorization> list = authorizationService.CreateAuthorizationQuery().ToList();
//		foreach (IAuthorization authorization in list)
//		{
//		  authorizationService.DeleteAuthorization(authorization.Id);
//		}
//		base.TearDown();
//	  }

//	  protected internal virtual void createAuthorization(string userId, string groupId
//          , ESS.FW.Bpm.Engine.Authorization.Resources resourceType, string resourceId
//          , params Permissions[] permissions)
//	  {

//		IAuthorization authorization = authorizationService.CreateNewAuthorization(AuthorizationFields.AuthTypeGrant);
//		authorization.UserId = userId;
//		authorization.GroupId = groupId;
//		authorization.Resource = resourceType;
//		authorization.ResourceId = resourceId;

//		foreach (Permissions permission in permissions)
//		{
//		  authorization.AddPermission(permission);
//		}

//		authorizationService.SaveAuthorization(authorization);
//	  }

//	  public virtual void testValidQueryCounts()
//	  {

//		var resource1 = new TestResource("resource1", 100);
//		var resource2 = new TestResource("resource2", 101);
//		var nonExisting = new TestResource("non-existing", 102);

//		// query by user id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user2").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user3").Count());
//		Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1", "user2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "non-existing").Count());

//		// query by group id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group2").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group3").Count());
//		Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1", "group2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "non-existing").Count());

//		// query by resource type
//		Assert.AreEqual(4, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==resource1.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==nonExisting.resourceType()).Count());
//		Assert.AreEqual(4, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==resource1.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==nonExisting.resourceType()).Count());

//		// query by resource id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == "resource1-2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == "non-existing").Count());

//		// query by permission
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Access).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Delete).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.Count());
//		Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Update).Count());
//		// multiple permissions at the same time
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.HasPermission(Permissions.Update).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Update)/*.HasPermission(Permissions.Read)*/.Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.HasPermission(Permissions.Access).Count());

//		// user id & resource type
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1"&& c.ResourceType==resource1.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1"&& c.ResourceType==nonExisting.resourceType()).Count());

//		// group id & resource type
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group2"&& c.ResourceType==resource2.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1"&& c.ResourceType==nonExisting.resourceType()).Count());
//	  }

//	  public virtual void testValidQueryLists()
//	  {

//		var resource1 = new TestResource("resource1", 100);
//		var resource2 = new TestResource("resource2", 101);
//		var nonExisting = new TestResource("non-existing", 102);

//		// query by user id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.UserId == "user1").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user2").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user3").Count());
//		//Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1", "user2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "non-existing").Count());

//		// query by group id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group2").Count());
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group3").Count());
//		//Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1", "group2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "non-existing").Count());

//		// query by resource type
//		Assert.AreEqual(4, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==resource1.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.ResourceType ==nonExisting.resourceType()).Count());

//		// query by resource id
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == "resource1-2").Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.ResourceId == "non-existing").Count());

//		// query by permission
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Access).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Delete).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.Count());
//		Assert.AreEqual(3, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Update).Count());
//		// multiple permissions at the same time
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.HasPermission(Permissions.Update).Count());
//		Assert.AreEqual(2, authorizationService.CreateAuthorizationQuery().HasPermission(Permissions.Update)/*.HasPermission(Permissions.Read)*/.Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery()/*.HasPermission(Permissions.Read)*/.HasPermission(Permissions.Access).Count());

//		// user id & resource type
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1"&& c.ResourceType==resource1.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.UserId== "user1"&& c.ResourceType==nonExisting.resourceType()).Count());

//		// group id & resource type
//		Assert.AreEqual(1, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group2"&& c.ResourceType==resource2.resourceType()).Count());
//		Assert.AreEqual(0, authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "group1"&& c.ResourceType==nonExisting.resourceType()).Count());
//	  }

//	  public virtual void testOrderByQueries()
//	  {

//		var resource1 = new TestResource("resource1", 100);
//		var resource2 = new TestResource("resource2", 101);

//	      IList<IAuthorization> list = authorizationService.CreateAuthorizationQuery()
//	              .OrderByResourceType()
//	              /*.Asc()*/
//	              .ToList();
//	          ;Assert.AreEqual(resource1.resourceType(), list[0].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[1].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[2].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[3].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[4].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[5].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[6].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[7].ResourceType);

//	      list = authorizationService.CreateAuthorizationQuery()
//	              .OrderByResourceType()
//	              /*.Desc()*/
//	              .ToList();
//	          ;Assert.AreEqual(resource2.resourceType(), list[0].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[1].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[2].ResourceType);
//		Assert.AreEqual(resource2.resourceType(), list[3].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[4].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[5].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[6].ResourceType);
//		Assert.AreEqual(resource1.resourceType(), list[7].ResourceType);

//	      list = authorizationService.CreateAuthorizationQuery()
//	          .OrderByResourceId()
//	          /*.Asc()*/
//	          .ToList();
//		Assert.AreEqual("resource1-1", list[0].ResourceId);
//		Assert.AreEqual("resource1-1", list[1].ResourceId);
//		Assert.AreEqual("resource1-2", list[2].ResourceId);
//		Assert.AreEqual("resource1-2", list[3].ResourceId);
//		Assert.AreEqual("resource2-1", list[4].ResourceId);
//		Assert.AreEqual("resource2-1", list[5].ResourceId);
//		Assert.AreEqual("resource2-2", list[6].ResourceId);
//		Assert.AreEqual("resource2-3", list[7].ResourceId);

//		list = authorizationService.CreateAuthorizationQuery().OrderByResourceId()/*.Desc()*/.ToList();
//		Assert.AreEqual("resource2-3", list[0].ResourceId);
//		Assert.AreEqual("resource2-2", list[1].ResourceId);
//		Assert.AreEqual("resource2-1", list[2].ResourceId);
//		Assert.AreEqual("resource2-1", list[3].ResourceId);
//		Assert.AreEqual("resource1-2", list[4].ResourceId);
//		Assert.AreEqual("resource1-2", list[5].ResourceId);
//		Assert.AreEqual("resource1-1", list[6].ResourceId);
//		Assert.AreEqual("resource1-1", list[7].ResourceId);

//	  }

//	  public virtual void testInvalidOrderByQueries()
//	  {
//		try
//		{
//		  authorizationService.CreateAuthorizationQuery().OrderByResourceType().ToList();
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Invalid query: call asc() or Desc() after using orderByXX()", e.Message);
//		}

//		try
//		{
//		    authorizationService.CreateAuthorizationQuery()
//		        .OrderByResourceId()
//		        .ToList();
//		        ;Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Invalid query: call asc() or Desc() after using orderByXX()", e.Message);
//		}

//		try
//		{
//		  authorizationService.CreateAuthorizationQuery().OrderByResourceId().OrderByResourceType().ToList();
//		  Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Invalid query: call asc() or Desc() after using orderByXX()", e.Message);
//		}

//		try
//		{
//		    authorizationService.CreateAuthorizationQuery()
//		        .OrderByResourceType()
//		        .OrderByResourceId()
//		        .ToList();
//		        ;Assert.Fail("Exception expected");
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Invalid query: call asc() or Desc() after using orderByXX()", e.Message);
//		}
//	  }

//	  public virtual void testInvalidQueries()
//	  {

//		// cannot query for user id and group id at the same time

//		try
//		{
//		  authorizationService.CreateAuthorizationQuery(c=>c.GroupId == "a"&& c.UserId == "b").Count();
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Cannot query for user and group authorizations at the same time.", e.Message);
//		}

//		try
//		{
//		  authorizationService.CreateAuthorizationQuery(c=>c.UserId== "b").GroupIdIn("a").Count();
//		}
//		catch (ProcessEngineException e)
//		{
//		  AssertTextPresent("Cannot query for user and group authorizations at the same time.", e.Message);
//		}

//	  }

//	}

//}