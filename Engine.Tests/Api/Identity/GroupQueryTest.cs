//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using ESS.FW.Bpm.Engine.Identity;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;



//namespace ESS.FW.Bpm.Engine.Tests.Api.Identity
//{

//	/// <summary>
//	/// 
//	/// </summary>
//	public class GroupQueryTest : PluggableProcessEngineTestCase
//	{

//        [SetUp]
//	  protected internal void setUp()
//	  {
//		//base.SetUp();

//		createGroup("muppets", "Muppet show characters_", "user");
//		createGroup("frogs", "Famous frogs", "user");
//		createGroup("mammals", "Famous mammals from eighties", "user");
//		createGroup("admin", "Administrators", "security");

//		identityService.SaveUser(identityService.NewUser("kermit"));
//		identityService.SaveUser(identityService.NewUser("fozzie"));
//		identityService.SaveUser(identityService.NewUser("mispiggy"));

//		identityService.SaveTenant(identityService.NewTenant("tenant"));

//		identityService.CreateMembership("kermit", "muppets");
//		identityService.CreateMembership("fozzie", "muppets");
//		identityService.CreateMembership("mispiggy", "muppets");

//		identityService.CreateMembership("kermit", "frogs");

//		identityService.CreateMembership("fozzie", "mammals");
//		identityService.CreateMembership("mispiggy", "mammals");

//		identityService.CreateMembership("kermit", "admin");

//		identityService.CreateTenantGroupMembership("tenant", "frogs");

//	  }

//	  private IGroup createGroup(string id, string name, string type)
//	  {
//		IGroup group = identityService.NewGroup(id);
//		group.Name = name;
//		group.Type = type;
//		identityService.SaveGroup(group);
//		return group;
//	  }

//        [TearDown]
//	  protected internal void tearDown()
//	  {
//		identityService.DeleteUser("kermit");
//		identityService.DeleteUser("fozzie");
//		identityService.DeleteUser("mispiggy");

//		identityService.DeleteGroup("muppets");
//		identityService.DeleteGroup("mammals");
//		identityService.DeleteGroup("frogs");
//		identityService.DeleteGroup("admin");

//		identityService.DeleteTenant("tenant");

//		base.TearDown();
//	  }
//        [Test]
//	  public virtual void testQueryById()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupId("muppets");
//		//verifyQueryResults(query, 1);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidId()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupId("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateGroupQuery().GroupId(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        public virtual void testQueryByIdIn()
//	  {
//		// empty list
//		Assert.True(identityService.CreateGroupQuery(c=>c.GroupId == "a", "b").Count()==0);

//		// collect all ids
//		IList<IGroup> list = identityService.CreateGroupQuery().ToList();
//		string[] ids = new string[list.Count];
//		for (int i = 0; i < ids.Length; i++)
//		{
//		  ids[i] = list[i].Id;
//		}

//		IList<IGroup> idInList = identityService.CreateGroupQuery(c=>c.GroupId == ids).ToList();
//		Assert.AreEqual(list.Count, idInList.Count);
//		foreach (IGroup group in idInList)
//		{
//		  bool found = false;
//		  foreach (IGroup otherGroup in list)
//		  {
//			if (otherGroup.Id.Equals(group.Id))
//			{
//			  found = true;
//			  break;
//			}
//		  }
//		  if (!found)
//		  {
//			Assert.Fail("Expected to find group " + group);
//		  }
//		}
//	  }

//        [Test]
//        public virtual void testQueryByName()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupName("Muppet show characters_");
//		//verifyQueryResults(query, 1);

//		query = identityService.CreateGroupQuery().GroupName("Famous frogs");
//		//verifyQueryResults(query, 1);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidName()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupName("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateGroupQuery().GroupName(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        public virtual void testQueryByNameLike()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupNameLike("%Famous%");
//		//verifyQueryResults(query, 2);

//		query = identityService.CreateGroupQuery().GroupNameLike("Famous%");
//		//verifyQueryResults(query, 2);

//		query = identityService.CreateGroupQuery().GroupNameLike("%show%");
//		//verifyQueryResults(query, 1);

//		query = identityService.CreateGroupQuery().GroupNameLike("%ters\\_");
//		//verifyQueryResults(query, 1);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidNameLike()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupNameLike("%invalid%");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateGroupQuery().GroupNameLike(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        public virtual void testQueryByType()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupType("user");
//		//verifyQueryResults(query, 3);

//		query = identityService.CreateGroupQuery().GroupType("admin");
//		//verifyQueryResults(query, 0);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidType()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupType("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateGroupQuery().GroupType(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        public virtual void testQueryByMember()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupMember("fozzie");
//		//verifyQueryResults(query, 2);

//		query = identityService.CreateGroupQuery().GroupMember("kermit");
//		//verifyQueryResults(query, 3);

//		query = query.OrderByGroupId()/*.Asc()*/;
//		IList<IGroup> groups = query.ToList();
//		Assert.AreEqual(3, groups.Count);
//		Assert.AreEqual("admin", groups[0].Id);
//		Assert.AreEqual("frogs", groups[1].Id);
//		Assert.AreEqual("muppets", groups[2].Id);

//		query = query.GroupType("user");
//		groups = query.ToList();
//		Assert.AreEqual(2, groups.Count);
//		Assert.AreEqual("frogs", groups[0].Id);
//		Assert.AreEqual("muppets", groups[1].Id);
//	  }

//        [Test]
//        public virtual void testQueryByInvalidMember()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().GroupMember("invalid");
//		//verifyQueryResults(query, 0);

//		try
//		{
//		  identityService.CreateGroupQuery().GroupMember(null).ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }

//        [Test]
//        public virtual void testQueryByMemberOfTenant()
//	  {
//		IGroupQuery query = identityService.CreateGroupQuery().MemberOfTenant("nonExisting");
//		//verifyQueryResults(query, 0);

//		query = identityService.CreateGroupQuery().MemberOfTenant("tenant");
//		//verifyQueryResults(query, 1);

//		IGroup group = query.First();
//		Assert.AreEqual("frogs", group.Id);
//	  }

//        [Test]
//        public virtual void testQuerySorting()
//	  {
//		// asc
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupId()/*.Asc()*/.Count());
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupName()/*.Asc()*/.Count());
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupType()/*.Asc()*/.Count());

//		// Desc
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupId()/*.Desc()*/.Count());
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupName()/*.Desc()*/.Count());
//		Assert.AreEqual(4, identityService.CreateGroupQuery().OrderByGroupType()/*.Desc()*/.Count());

//		// Multiple sortings
//		IGroupQuery query = identityService.CreateGroupQuery().OrderByGroupType()/*.Asc()*/.OrderByGroupName()/*.Desc()*/;
//		IList<IGroup> groups = query.ToList();
//		Assert.AreEqual(4, query.Count());

//		Assert.AreEqual("security", groups[0].Type);
//		Assert.AreEqual("user", groups[1].Type);
//		Assert.AreEqual("user", groups[2].Type);
//		Assert.AreEqual("user", groups[3].Type);

//		Assert.AreEqual("admin", groups[0].Id);
//		Assert.AreEqual("muppets", groups[1].Id);
//		Assert.AreEqual("mammals", groups[2].Id);
//		Assert.AreEqual("frogs", groups[3].Id);
//	  }

//        [Test]
//        public virtual void testQueryInvalidSortingUsage()
//	  {
//		try
//		{
//		  identityService.CreateGroupQuery().OrderByGroupId().ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}

//		try
//		{
//		  identityService.CreateGroupQuery().OrderByGroupId().OrderByGroupName().ToList();
//		  Assert.Fail();
//		}
//		catch (ProcessEngineException)
//		{
//		}
//	  }
//        private void verifyQueryResults(IGroupQuery query, int countExpected)
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

//	  private void verifySingleResultFails(IGroupQuery query)
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

//	}

//}