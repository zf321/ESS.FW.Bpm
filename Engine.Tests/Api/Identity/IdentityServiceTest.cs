using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Identity;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{
    

	/// <summary>
	/// 
	/// </summary>
	public class IdentityServiceTest
	{


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();



	  protected internal IIdentityService identityService;

        [SetUp]
	  public virtual void init()
	  {
		identityService = engineRule.IdentityService;
	  }

        [TearDown]
	  public virtual void cleanUp()
	  {
		foreach (IUser user in identityService.CreateUserQuery().ToList())
		{
		  identityService.DeleteUser(user.Id);
		}
		foreach (IGroup group in identityService.CreateGroupQuery().ToList())
		{
		  identityService.DeleteGroup(group.Id);
		}
	  }

        [Test]
	  public virtual void testIsReadOnly()
	  {
		Assert.IsFalse(identityService.ReadOnly);
	  }


        [Test]
        public virtual void testUserInfo()
	  {
		IUser user = identityService.NewUser("testuser");
		identityService.SaveUser(user);

		identityService.SetUserInfo("testuser", "myinfo", "myvalue");
		Assert.AreEqual("myvalue", identityService.GetUserInfo("testuser", "myinfo"));

		identityService.SetUserInfo("testuser", "myinfo", "myvalue2");
		Assert.AreEqual("myvalue2", identityService.GetUserInfo("testuser", "myinfo"));

		identityService.DeleteUserInfo("testuser", "myinfo");
		Assert.IsNull(identityService.GetUserInfo("testuser", "myinfo"));

		identityService.DeleteUser(user.Id);
	  }


        [Test]
        public virtual void testUserAccount()
	  {
		IUser user = identityService.NewUser("testuser");
		identityService.SaveUser(user);

		identityService.SetUserAccount("testuser", "123", "google", "mygoogleusername", "mygooglepwd", null);
		IAccount googleAccount = identityService.GetUserAccount("testuser", "123", "google");
		Assert.AreEqual("google", googleAccount.Name);
		Assert.AreEqual("mygoogleusername", googleAccount.Username);
		Assert.AreEqual("mygooglepwd", googleAccount.Password);

		identityService.SetUserAccount("testuser", "123", "google", "mygoogleusername2", "mygooglepwd2", null);
		googleAccount = identityService.GetUserAccount("testuser", "123", "google");
		Assert.AreEqual("google", googleAccount.Name);
		Assert.AreEqual("mygoogleusername2", googleAccount.Username);
		Assert.AreEqual("mygooglepwd2", googleAccount.Password);

		identityService.SetUserAccount("testuser", "123", "alfresco", "myalfrescousername", "myalfrescopwd", null);
		identityService.SetUserInfo("testuser", "myinfo", "myvalue");
		identityService.SetUserInfo("testuser", "myinfo2", "myvalue2");

		IList<string> expectedUserAccountNames = new List<string>();
		expectedUserAccountNames.Add("google");
		expectedUserAccountNames.Add("alfresco");
		IList<string> userAccountNames = identityService.GetUserAccountNames("testuser");
		AssertListElementsMatch(expectedUserAccountNames, userAccountNames);

		identityService.DeleteUserAccount("testuser", "google");

		expectedUserAccountNames.Remove("google");

		userAccountNames = identityService.GetUserAccountNames("testuser");
		AssertListElementsMatch(expectedUserAccountNames, userAccountNames);

		identityService.DeleteUser(user.Id);
	  }

	  private void AssertListElementsMatch(IList<string> list1, IList<string> list2)
	  {
		if (list1 != null)
		{
		  Assert.NotNull(list2);
		  Assert.AreEqual(list1.Count, list2.Count);
		  foreach (string value in list1)
		  {
			Assert.True(list2.Contains(value));
		  }
		}
		else
		{
		  Assert.IsNull(list2);
		}

	  }


        [Test]
        public virtual void testUserAccountDetails()
	  {
		IUser user = identityService.NewUser("testuser");
		identityService.SaveUser(user);

		IDictionary<string, string> accountDetails = new Dictionary<string, string>();
		accountDetails["server"] = "localhost";
		accountDetails["port"] = "35";
		identityService.SetUserAccount("testuser", "123", "google", "mygoogleusername", "mygooglepwd", accountDetails);
		IAccount googleAccount = identityService.GetUserAccount("testuser", "123", "google");
		Assert.AreEqual(accountDetails, googleAccount.Details);

		identityService.DeleteUser(user.Id);
	  }


        [Test]
        public virtual void testCreateExistingUser()
	  {
		IUser user = identityService.NewUser("testuser");
		identityService.SaveUser(user);

		IUser secondUser = identityService.NewUser("testuser");

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.SaveUser(secondUser);
	  }


        [Test]
        public virtual void testUpdateUser()
	  {
		// First, create a new user
		IUser user = identityService.NewUser("johndoe");
		user.FirstName = "John";
		user.LastName = "Doe";
		user.Email = "johndoe@alfresco.com";
		user.Password = "s3cret";
		identityService.SaveUser(user);

		// Fetch and update the user
		user = identityService.CreateUserQuery(c=>c.Id == "johndoe").First();
		user.Email = "updated@alfresco.com";
		user.FirstName = "Jane";
		user.LastName = "Donnel";
		identityService.SaveUser(user);

		user = identityService.CreateUserQuery(c=>c.Id == "johndoe").First();
		Assert.AreEqual("Jane", user.FirstName);
		Assert.AreEqual("Donnel", user.LastName);
		Assert.AreEqual("updated@alfresco.com", user.Email);
		Assert.True(identityService.CheckPassword("johndoe", "s3cret"));

		identityService.DeleteUser(user.Id);
	  }


        [Test]
        public virtual void testUserPicture()
	  {
		// First, create a new user
		IUser user = identityService.NewUser("johndoe");
		identityService.SaveUser(user);
		string userId = user.Id;

		Picture picture = new Picture("niceface".GetBytes(), "image/string");
		identityService.SetUserPicture(userId, picture);

		picture = identityService.GetUserPicture(userId);

		// Fetch and update the user
		user = identityService.CreateUserQuery(c=>c.Id == "johndoe").First();
		Assert.True( Array.Equals("niceface".GetBytes(), picture.Bytes));//"byte arrays differ",
            Assert.AreEqual("image/string", picture.MimeType);

		identityService.DeleteUserPicture("johndoe");
		// this is ignored
		identityService.DeleteUserPicture("someone-else-we-dont-know");

		// picture does not exist
		picture = identityService.GetUserPicture("johndoe");
		Assert.IsNull(picture);

		// add new picture
		picture = new Picture("niceface".GetBytes(), "image/string");
		identityService.SetUserPicture(userId, picture);

		// makes the picture go away
		identityService.DeleteUser(user.Id);
	  }


        [Test]
        public virtual void testUpdateGroup()
	  {
		IGroup group = identityService.NewGroup("sales");
		group.Name = "Sales";
		identityService.SaveGroup(group);

		group = identityService.CreateGroupQuery().First();//.GroupId("sales").First();
		group.Name = "Updated";
		identityService.SaveGroup(group);

		group = identityService.CreateGroupQuery().First();//.GroupId("sales").First();
		Assert.AreEqual("Updated", group.Name);

		identityService.DeleteGroup(group.Id);
	  }


        [Test]
        public virtual void findUserByUnexistingId()
	  {
		IUser user = identityService.CreateUserQuery(c=>c.Id == "unexistinguser").First();
		Assert.IsNull(user);
	  }


        [Test]
        public virtual void findGroupByUnexistingId()
	  {
		IGroup group = identityService.CreateGroupQuery()//.GroupId("unexistinggroup")
                .First();
		Assert.IsNull(group);
	  }


        [Test]
        public virtual void testCreateMembershipUnexistingGroup()
	  {
		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.CreateMembership(johndoe.Id, "unexistinggroup");
	  }


        [Test]
        public virtual void testCreateMembershipUnexistingUser()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.CreateMembership("unexistinguser", sales.Id);
	  }


        [Test]
        public virtual void testCreateMembershipAlreadyExisting()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);
		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);

		// Create the membership
		identityService.CreateMembership(johndoe.Id, sales.Id);

		//thrown.Expect(typeof(ProcessEngineException));

		identityService.CreateMembership(johndoe.Id, sales.Id);
	  }


        [Test]
        public virtual void testSaveGroupNullArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("group is null");

		identityService.SaveGroup(null);
	  }


        [Test]
        public virtual void testSaveUserNullArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("user is null");

		identityService.SaveUser(null);
	  }


        [Test]
        public virtual void testFindGroupByIdNullArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("id is null");

		identityService.CreateGroupQuery()//.GroupId(null)
                .First();
	  }


        [Test]
        public virtual void testCreateMembershipNullUserArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("userId is null");

		identityService.CreateMembership(null, "group");
	  }


        [Test]
        public virtual void testCreateMembershipNullGroupArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("groupId is null");

		identityService.CreateMembership("userId", null);
	  }


        [Test]
        public virtual void testFindGroupsByUserIdNullArguments()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("userId is null");

		identityService.CreateGroupQuery()//.GroupMember(null)
                .First();
	  }


        [Test]
        public virtual void testFindUsersByGroupUnexistingGroup()
	  {
		IList<IUser> users = identityService.CreateUserQuery()//.MemberOfGroup("unexistinggroup")
                .ToList();
		Assert.NotNull(users);
		Assert.True(users.Count == 0);
	  }


        [Test]
        public virtual void testDeleteGroupNullArguments()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("groupId is null");

		identityService.DeleteGroup(null);
	  }


        [Test]
        public virtual void testDeleteMembership()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);

		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);
		// Add membership
		identityService.CreateMembership(johndoe.Id, sales.Id);

		IList<IGroup> groups = identityService.CreateGroupQuery()//.GroupMember(johndoe.Id)
                .ToList();
		Assert.True(groups.Count == 1);
		Assert.AreEqual("sales", groups[0].Id);

		// Delete the membership and check members of sales group
		identityService.DeleteMembership(johndoe.Id, sales.Id);
		groups = identityService.CreateGroupQuery()//.GroupMember(johndoe.Id)
                .ToList();
		Assert.True(groups.Count == 0);

		identityService.DeleteGroup("sales");
		identityService.DeleteUser("johndoe");
	  }


        [Test]
        public virtual void testDeleteMembershipWhenUserIsNoMember()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);

		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);

		// Delete the membership when the user is no member
		identityService.DeleteMembership(johndoe.Id, sales.Id);

		identityService.DeleteGroup("sales");
		identityService.DeleteUser("johndoe");
	  }


        [Test]
        public virtual void testDeleteMembershipUnexistingGroup()
	  {
		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);
		// No exception should be thrown when group doesn't exist
		identityService.DeleteMembership(johndoe.Id, "unexistinggroup");
		identityService.DeleteUser(johndoe.Id);
	  }


        [Test]
        public virtual void testDeleteMembershipUnexistingUser()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);
		// No exception should be thrown when user doesn't exist
		identityService.DeleteMembership("unexistinguser", sales.Id);
		identityService.DeleteGroup(sales.Id);
	  }


        [Test]
        public virtual void testDeleteMemberschipNullUserArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("userId is null");

		identityService.DeleteMembership(null, "group");
	  }


        [Test]
        public virtual void testDeleteMemberschipNullGroupArgument()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("groupId is null");

		identityService.DeleteMembership("user", null);
	  }


        [Test]
        public virtual void testDeleteUserNullArguments()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("userId is null");

		identityService.DeleteUser(null);
	  }


        [Test]
        public virtual void testDeleteUserUnexistingUserId()
	  {
		// No exception should be thrown. Deleting an unexisting user should
		// be ignored silently
		identityService.DeleteUser("unexistinguser");
	  }


        [Test]
        public virtual void testCheckPassword()
	  {

		// store user with password
		IUser user = identityService.NewUser("secureUser");
		user.Password = "s3cret";
		identityService.SaveUser(user);

		Assert.True(identityService.CheckPassword(user.Id, "s3cret"));
		Assert.IsFalse(identityService.CheckPassword(user.Id, "wrong"));

		identityService.DeleteUser(user.Id);

	  }


        [Test]
        public virtual void testUpdatePassword()
	  {

		// store user with password
		IUser user = identityService.NewUser("secureUser");
		user.Password = "s3cret";
		identityService.SaveUser(user);

		Assert.True(identityService.CheckPassword(user.Id, "s3cret"));

		user.Password = "new-password";
		identityService.SaveUser(user);

		Assert.True(identityService.CheckPassword(user.Id, "new-password"));

		identityService.DeleteUser(user.Id);

	  }


        [Test]
        public virtual void testCheckPasswordNullSafe()
	  {
		Assert.IsFalse(identityService.CheckPassword("userId", null));
		Assert.IsFalse(identityService.CheckPassword(null, "passwd"));
		Assert.IsFalse(identityService.CheckPassword(null, null));
	  }


        [Test]
        public virtual void testUserOptimisticLockingException()
	  {
		IUser user = identityService.NewUser("kermit");
		identityService.SaveUser(user);

		IUser user1 = identityService.CreateUserQuery().First();
		IUser user2 = identityService.CreateUserQuery().First();

		user1.FirstName = "name one";
		identityService.SaveUser(user1);

		//thrown.Expect(typeof(OptimisticLockingException));

		user2.FirstName = "name two";
		identityService.SaveUser(user2);
	  }


        [Test]
        public virtual void testGroupOptimisticLockingException()
	  {
		IGroup group = identityService.NewGroup("group");
		identityService.SaveGroup(group);

		IGroup group1 = identityService.CreateGroupQuery().First();
		IGroup group2 = identityService.CreateGroupQuery().First();

		group1.Name = "name one";
		identityService.SaveGroup(group1);

		//thrown.Expect(typeof(OptimisticLockingException));

		group2.Name = "name two";
		identityService.SaveGroup(group2);
	  }


        [Test]
        public virtual void testSaveUserWithGenericResourceId()
	  {
		IUser user = identityService.NewUser("*");

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		identityService.SaveUser(user);
	  }


        [Test]
        public virtual void testSaveGroupWithGenericResourceId()
	  {
		IGroup group = identityService.NewGroup("*");

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("has an invalid id: id cannot be *. * is a reserved identifier.");

		identityService.SaveGroup(group);
	  }


        [Test]
        public virtual void testSetAuthenticatedIdToGenericId()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Invalid user id provided: id cannot be *. * is a reserved identifier.");

		identityService.AuthenticatedUserId = "*";
	  }


        [Test]
        public virtual void testSetAuthenticationUserIdToGenericId()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("invalid group id provided: id cannot be *. * is a reserved identifier.");

		identityService.SetAuthentication("aUserId",new List<string>(){ "*" } );
	  }


        [Test]
        public virtual void testSetAuthenticatedTenantIdToGenericId()
	  {
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("invalid tenant id provided: id cannot be *. * is a reserved identifier.");

		identityService.SetAuthentication(null, null, new List<string>() { "*" });
	  }


        [Test]
        public virtual void testSetAuthenticatedUserId()
	  {
		identityService.AuthenticatedUserId = "john";

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		Assert.NotNull(currentAuthentication);
		Assert.AreEqual("john", currentAuthentication.UserId);
		Assert.IsNull(currentAuthentication.GroupIds);
		Assert.IsNull(currentAuthentication.TenantIds);
	  }


        [Test]
        public virtual void testSetAuthenticatedUserAndGroups()
	  {
		IList<string> groups = new List<string>() { "sales", "development" };

		identityService.SetAuthentication("john", groups);

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		Assert.NotNull(currentAuthentication);
		Assert.AreEqual("john", currentAuthentication.UserId);
		Assert.AreEqual(groups, currentAuthentication.GroupIds);
		Assert.IsNull(currentAuthentication.TenantIds);
	  }


        [Test]
        public virtual void testSetAuthenticatedUserGroupsAndTenants()
	  {
		IList<string> groups = new List<string>() { "sales", "development" };
		IList<string> tenants = new List<string>() { "tenant1", "tenant2" };

		identityService.SetAuthentication("john", groups, tenants);

		Authentication currentAuthentication = identityService.CurrentAuthentication;

		Assert.NotNull(currentAuthentication);
		Assert.AreEqual("john", currentAuthentication.UserId);
		Assert.AreEqual(groups, currentAuthentication.GroupIds);
		Assert.AreEqual(tenants, currentAuthentication.TenantIds);
	  }


        [Test]
        public virtual void testAuthentication()
	  {
		IUser user = identityService.NewUser("johndoe");
		user.Password = "xxx";
		identityService.SaveUser(user);

		Assert.True(identityService.CheckPassword("johndoe", "xxx"));
		Assert.IsFalse(identityService.CheckPassword("johndoe", "invalid pwd"));

		identityService.DeleteUser("johndoe");
	  }


        [Test]
        public virtual void testFindGroupsByUserAndType()
	  {
		IGroup sales = identityService.NewGroup("sales");
		sales.Type = "hierarchy";
		identityService.SaveGroup(sales);

		IGroup development = identityService.NewGroup("development");
		development.Type = "hierarchy";
		identityService.SaveGroup(development);

		IGroup admin = identityService.NewGroup("admin");
		admin.Type = "security-role";
		identityService.SaveGroup(admin);

		IGroup user = identityService.NewGroup("user");
		user.Type = "security-role";
		identityService.SaveGroup(user);

		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);

		IUser joesmoe = identityService.NewUser("joesmoe");
		identityService.SaveUser(joesmoe);

		IUser jackblack = identityService.NewUser("jackblack");
		identityService.SaveUser(jackblack);

		identityService.CreateMembership("johndoe", "sales");
		identityService.CreateMembership("johndoe", "user");
		identityService.CreateMembership("johndoe", "admin");

		identityService.CreateMembership("joesmoe", "user");

		IList<IGroup> groups = identityService.CreateGroupQuery()//.GroupMember("johndoe").GroupType("security-role")
                .ToList();
		ISet<string> groupIds = getGroupIds(groups);
		ISet<string> expectedGroupIds = new HashSet<string>();
		expectedGroupIds.Add("user");
		expectedGroupIds.Add("admin");
		Assert.AreEqual(expectedGroupIds, groupIds);

		groups = identityService.CreateGroupQuery()//.GroupMember("joesmoe").GroupType("security-role")
                .ToList();
		groupIds = getGroupIds(groups);
		expectedGroupIds = new HashSet<string>();
		expectedGroupIds.Add("user");
		Assert.AreEqual(expectedGroupIds, groupIds);

		groups = identityService.CreateGroupQuery()//.GroupMember("jackblack").GroupType("security-role")
                .ToList();
		Assert.True(groups.Count == 0);

		identityService.DeleteGroup("sales");
		identityService.DeleteGroup("development");
		identityService.DeleteGroup("admin");
		identityService.DeleteGroup("user");
		identityService.DeleteUser("johndoe");
		identityService.DeleteUser("joesmoe");
		identityService.DeleteUser("jackblack");
	  }


        [Test]
        public virtual void testUser()
	  {
		IUser user = identityService.NewUser("johndoe");
		user.FirstName = "John";
		user.LastName = "Doe";
		user.Email = "johndoe@alfresco.com";
		identityService.SaveUser(user);

		user = identityService.CreateUserQuery(c=>c.Id == "johndoe").First();
		Assert.AreEqual("johndoe", user.Id);
		Assert.AreEqual("John", user.FirstName);
		Assert.AreEqual("Doe", user.LastName);
		Assert.AreEqual("johndoe@alfresco.com", user.Email);

		identityService.DeleteUser("johndoe");
	  }


        [Test]
        public virtual void testGroup()
	  {
		IGroup group = identityService.NewGroup("sales");
		group.Name = "Sales division";
		identityService.SaveGroup(group);

		group = identityService.CreateGroupQuery().First();//.GroupId("sales").First();
		Assert.AreEqual("sales", group.Id);
		Assert.AreEqual("Sales division", group.Name);

		identityService.DeleteGroup("sales");
	  }


        [Test]
        public virtual void testMembership()
	  {
		IGroup sales = identityService.NewGroup("sales");
		identityService.SaveGroup(sales);

		IGroup development = identityService.NewGroup("development");
		identityService.SaveGroup(development);

		IUser johndoe = identityService.NewUser("johndoe");
		identityService.SaveUser(johndoe);

		IUser joesmoe = identityService.NewUser("joesmoe");
		identityService.SaveUser(joesmoe);

		IUser jackblack = identityService.NewUser("jackblack");
		identityService.SaveUser(jackblack);

		identityService.CreateMembership("johndoe", "sales");
		identityService.CreateMembership("joesmoe", "sales");

		identityService.CreateMembership("joesmoe", "development");
		identityService.CreateMembership("jackblack", "development");

		IList<IGroup> groups = identityService.CreateGroupQuery()//.GroupMember("johndoe")
                .ToList();
		Assert.AreEqual(createStringSet("sales"), getGroupIds(groups));

		groups = identityService.CreateGroupQuery()//.GroupMember("joesmoe")
                .ToList();
		Assert.AreEqual(createStringSet("sales", "development"), getGroupIds(groups));

		groups = identityService.CreateGroupQuery()//.GroupMember("jackblack")
                .ToList();
		Assert.AreEqual(createStringSet("development"), getGroupIds(groups));

		IList<IUser> users = identityService.CreateUserQuery()//.MemberOfGroup("sales")
                .ToList();
		Assert.AreEqual(createStringSet("johndoe", "joesmoe"), getUserIds(users));

		users = identityService.CreateUserQuery()//.MemberOfGroup("development")
                .ToList();
		Assert.AreEqual(createStringSet("joesmoe", "jackblack"), getUserIds(users));

		identityService.DeleteGroup("sales");
		identityService.DeleteGroup("development");

		identityService.DeleteUser("jackblack");
		identityService.DeleteUser("joesmoe");
		identityService.DeleteUser("johndoe");
	  }

	  private object createStringSet(params string[] strings)
	  {
		ISet<string> stringSet = new HashSet<string>();
		foreach (string @string in strings)
		{
		  stringSet.Add(@string);
		}
		return stringSet;
	  }

	  protected internal virtual ISet<string> getGroupIds(IList<IGroup> groups)
	  {
		ISet<string> groupIds = new HashSet<string>();
		foreach (IGroup group in groups)
		{
		  groupIds.Add(group.Id);
		}
		return groupIds;
	  }

	  protected internal virtual ISet<string> getUserIds(IList<IUser> users)
	  {
		ISet<string> userIds = new HashSet<string>();
		foreach (IUser user in users)
		{
		  userIds.Add(user.Id);
		}
		return userIds;
	  }

	}

}