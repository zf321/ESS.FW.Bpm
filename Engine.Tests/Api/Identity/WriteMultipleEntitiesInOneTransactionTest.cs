using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Identity;
using NUnit.Framework;

/// 
namespace Engine.Tests.Api.Identity
{

    /// <summary>
	/// 
	/// 
	/// </summary>
	public class WriteMultipleEntitiesInOneTransactionTest : ResourceProcessEngineTestCase
	{

	  public WriteMultipleEntitiesInOneTransactionTest() : base("resources/api/identity/WriteMultipleEntitiesInOneTransactionTest.Camunda.cfg.xml")
	  {
	  }
        [Test]
	  public virtual void testWriteMultipleEntitiesInOneTransaction()
	  {

		// the identity service provider registered with the engine creates a user, a group, and a membership
		// in the following call:
		Assert.True(identityService.CheckPassword("multipleEntities", "inOneStep"));
		IUser user = identityService.CreateUserQuery(c=>c.Id == "multipleEntities").First();

		Assert.NotNull(user);
		Assert.AreEqual("multipleEntities", user.Id);
		Assert.AreEqual("{SHA}pfdzmt+49nwknTy7xhZd7ZW5suI=", user.Password);

		// It is expected, that the IUser is in exactly one Group
	      IList<IGroup> groups = this.identityService.CreateGroupQuery()
	          //.GroupMember("multipleEntities")
	          .ToList();
		Assert.AreEqual(1, groups.Count);

		IGroup group = groups[0];
		Assert.AreEqual("multipleEntities_group", group.Id);

		// clean the Db
		identityService.DeleteMembership("multipleEntities", "multipleEntities_group");
		identityService.DeleteGroup("multipleEntities_group");
		identityService.DeleteUser("multipleEntities");
	  }
	}

}