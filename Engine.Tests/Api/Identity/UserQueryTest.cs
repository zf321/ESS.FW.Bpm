//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Identity;
//using ESS.FW.Bpm.Engine.Identity.Db;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.Api.Identity
//{
    

//	/// <summary>
//	/// 
//	/// </summary>
//	public class UserQueryTest : PluggableProcessEngineTestCase
//	{

//        [SetUp]
//	  protected internal void setUp()
//	  {
//		//base.SetUp();

//		createUser("kermit", "Kermit_", "The_frog", "kermit_@muppetshow.com");
//		createUser("fozzie", "Fozzie", "Bear", "fozzie@muppetshow.com");
//		createUser("gonzo", "Gonzo", "The great", "gonzo@muppetshow.com");

//		identityService.SaveGroup(identityService.NewGroup("muppets"));
//		identityService.SaveGroup(identityService.NewGroup("frogs"));

//		identityService.SaveTenant(identityService.NewTenant("tenant"));

//		identityService.CreateMembership("kermit", "muppets");
//		identityService.CreateMembership("kermit", "frogs");
//		identityService.CreateMembership("fozzie", "muppets");
//		identityService.CreateMembership("gonzo", "muppets");

//		identityService.CreateTenantUserMembership("tenant", "kermit");
//	  }

//	  private IUser createUser(string id, string firstName, string lastName, string email)
//	  {
//		IUser user = identityService.NewUser(id);
//		user.FirstName = firstName;
//		user.LastName = lastName;
//		user.Email = email;
//		identityService.SaveUser(user);
//		return user;
//	  }

//        [TearDown]
//	  protected internal void tearDown()
//	  {
//		identityService.DeleteUser("kermit");
//		identityService.DeleteUser("fozzie");
//		identityService.DeleteUser("gonzo");

//		identityService.DeleteGroup("muppets");
//		identityService.DeleteGroup("frogs");

//		identityService.DeleteTenant("tenant");

//		base.TearDown();
//	  }

//	    [Test]  public virtual void testQueryByNoCriteria()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery();
//		//verifyQueryResults(query, 3);
//	  }

//	    [Test]  public virtual void testQueryById()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery(c=>c.Id == "kermit");
//		//verifyQueryResults(query, 1);
//	  }

//	    [Test]  public virtual void testQueryByInvalidId()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery(c=>c.Id == "invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery(c=>c.Id == null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByFirstName()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery(c=>c.FirstName == "Gonzo");
//		//verifyQueryResults(query, 1);

//		IUser result = query.First();
//		Assert.AreEqual("gonzo", result.Id);
//	  }

//	    [Test]  public virtual void testQueryByInvalidFirstName()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery(c=>c.FirstName == "invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery(c=>c.FirstName == null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByFirstNameLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserFirstNameLike("%o%");
//		//verifyQueryResults(query, 2);

//		query = identityService.CreateUserQuery().UserFirstNameLike("Ker%");
//		//verifyQueryResults(query, 1);

//		identityService.CreateUserQuery().UserFirstNameLike("%mit\\_");
//		//verifyQueryResults(query, 1);
//	  }

//	    [Test]  public virtual void testQueryByInvalidFirstNameLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserFirstNameLike("%mispiggy%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery().UserFirstNameLike(null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByLastName()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserLastName("Bear");
//		//verifyQueryResults(query, 1);

//		IUser result = query.First();
//		Assert.AreEqual("fozzie", result.Id);
//	  }

//	    [Test]  public virtual void testQueryByInvalidLastName()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserLastName("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery().UserLastName(null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByLastNameLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserLastNameLike("%\\_frog%");
//		//verifyQueryResults(query, 1);

//		query = identityService.CreateUserQuery().UserLastNameLike("%ea%");
//		//verifyQueryResults(query, 2);
//	  }

//	    [Test]  public virtual void testQueryByInvalidLastNameLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserLastNameLike("%invalid%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery().UserLastNameLike(null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByEmail()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserEmail("kermit_@muppetshow.com");
//		//verifyQueryResults(query, 1);
//	  }

//	    [Test]  public virtual void testQueryByInvalidEmail()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserEmail("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery().UserEmail(null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByEmailLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserEmailLike("%muppetshow.com");
//		//verifyQueryResults(query, 3);

//		query = identityService.CreateUserQuery().UserEmailLike("%kermit\\_%");
//		//verifyQueryResults(query, 1);
//	  }

//	    [Test]  public virtual void testQueryByInvalidEmailLike()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserEmailLike("%invalid%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateUserQuery().UserEmailLike(null).First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQuerySorting()
//	  {
//		// asc
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserId()/*.Asc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserEmail()/*.Asc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserFirstName()/*.Asc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserLastName()/*.Asc()*/.Count());

//		// Desc
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserId()/*.Desc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserEmail()/*.Desc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserFirstName()/*.Desc()*/.Count());
//		Assert.AreEqual(3, identityService.CreateUserQuery().OrderByUserLastName()/*.Desc()*/.Count());

//		// Combined with criteria
//		IQueryable<IUser> query = identityService.CreateUserQuery().UserLastNameLike("%ea%").OrderByUserFirstName()/*.Asc()*/;
//		IList<IUser> users = query.ToList();
//		Assert.AreEqual(2,users.Count);
//		Assert.AreEqual("Fozzie", users[0].FirstName);
//		Assert.AreEqual("Gonzo", users[1].FirstName);
//	  }

//	    [Test]  public virtual void testQueryInvalidSortingUsage()
//	  {
//		try
//		{
//		    identityService.CreateUserQuery()
//		        //.OrderByUserId()
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}

//		try
//		{
//		    identityService.CreateUserQuery()
//		        //.OrderByUserId()
//		        //.OrderByUserEmail()
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByMemberOfGroup()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().MemberOfGroup("muppets");
//		//verifyQueryResults(query, 3);

//		query = identityService.CreateUserQuery().MemberOfGroup("frogs");
//		//verifyQueryResults(query, 1);

//		IUser result = query.First();
//		Assert.AreEqual("kermit", result.Id);
//	  }

//	    [Test]  public virtual void testQueryByInvalidMemberOfGoup()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().MemberOfGroup("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		    identityService.CreateUserQuery()
//		        .MemberOfGroup(null)
//		        .ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByMemberOfTenant()
//	  {
//		IQueryable<IUser> query = identityService.CreateUserQuery().MemberOfTenant("nonExisting");
//		//verifyQueryResults(query, 0);

//		query = identityService.CreateUserQuery().MemberOfTenant("tenant");
//		//verifyQueryResults(query, 1);

//		IUser result = query.First();
//		Assert.AreEqual("kermit", result.Id);
//	  }

//	  private void verifyQueryResults(IQueryable<IUser> query, int countExpected)
//	  {
//		Assert.AreEqual(countExpected, query.Count());
//		Assert.AreEqual(countExpected, query.Count());

//		if (countExpected == 1)
//		{
//		  Assert.NotNull(query.First());
//		}
//		else if (countExpected > 1)
//		{
//		  verifySingleResultFails(query);
//		}
//		else if (countExpected == 0)
//		{
//		  Assert.IsNull(query.First());
//		}
//	  }

//	  private void verifySingleResultFails(IQueryable<IUser> query)
//	  {
//		try
//		{
//		  query.First();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//	    [Test]  public virtual void testQueryByIdIn()
//	  {

//		// empty list
//		Assert.True(identityService.CreateUserQuery(c=>c.UserId== "a", "b").Count()==0);


//		// collect all ids
//	      IList<IUser> list = identityService.CreateUserQuery()
//	          .ToList();
//		string[] ids = new string[list.Count];
//		for (int i = 0; i < ids.Length; i++)
//		{
//		  ids[i] = list[i].Id;
//		}

//	      IList<IUser> idInList = identityService.CreateUserQuery()
//	          .UserIdIn(ids)
//	          .ToList();
//		foreach (IUser user in idInList)
//		{
//		  bool found = false;
//		  foreach (IUser otherUser in list)
//		  {
//			if (otherUser.Id.Equals(user.Id))
//			{
//			  found = true;
//			  break;
//			}
//		  }
//		  if (!found)
//		  {
//			Assert.Fail("Expected to find user " + user);
//		  }
//		}
//	  }

//	    private DbReadOnlyIdentityServiceProvider provider=new DbIdentityServiceProvider();

//	      [Test]  public virtual void testNativeQuery()
//	  {
//		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//		// just test that the query will be constructed and executed, details are tested in the TaskQueryTest
//		Assert.AreEqual(tablePrefix + "ACT_ID_USER", managementService.GetTableName(typeof(UserEntity)));

//		long userCount = identityService.CreateUserQuery().Count();

//		Assert.AreEqual(userCount, provider.CreateNativeUserQuery()
//            .Sql("SELECT * FROM " + managementService.GetTableName(typeof(UserEntity))).Count());
//            Assert.AreEqual(userCount, provider.CreateNativeUserQuery().Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(UserEntity))).Count());
//          //  Assert.AreEqual(userCount, identityService.CreateNativeUserQuery().sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(UserEntity))).Count());
//        }

//          [Test]  public virtual void testNativeQueryOrLike()
//	  {
//		string searchPattern = "%frog";

//		string fromWhereClauses = string.Format("FROM {0} WHERE FIRST_ LIKE #{{searchPattern}} OR LAST_ LIKE #{{searchPattern}} OR EMAIL_ LIKE #{{searchPattern}}", managementService.GetTableName(typeof(UserEntity)));

//		Assert.AreEqual(1, provider.CreateNativeUserQuery().Sql("SELECT * " + fromWhereClauses)
//            .Parameter("searchPattern", searchPattern).Count());
//		Assert.AreEqual(1, provider.CreateNativeUserQuery().Sql("SELECT Count(*) " + fromWhereClauses).Parameter("searchPattern", searchPattern).Count());
//	  }
      
//	    [Test]  public virtual void testNativeQueryPaging()
//	  {
//		Assert.AreEqual(2, provider.CreateNativeUserQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(UserEntity))).ListPage(1, 2).Count);
//		Assert.AreEqual(1, provider.CreateNativeUserQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(UserEntity))).ListPage(2, 1).Count);
//	  }

//	}

//}