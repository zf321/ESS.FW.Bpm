using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Identity
{

    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class ReadOnlyIdentityServiceTest
	{

	  protected internal const string CONFIGURATION_RESOURCE = "resources/api/identity/read.only.identity.service.Camunda.cfg.xml";


	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule(CONFIGURATION_RESOURCE);


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);



	  protected internal IIdentityService identityService;

    [SetUp]
	  public virtual void setUp()
	  {
		identityService = engineRule.IdentityService;

		Assert.True(identityService.ReadOnly);
	  }

    
	  public virtual void newUser()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.NewUser("user");
	  }


	  public virtual void SaveUser()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.SaveUser(null);
	  }


	  public virtual void deleteUser()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteUser("user");
	  }

	  public virtual void newGroup()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.NewGroup("group");
	  }


	  public virtual void SaveGroup()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.SaveGroup(null);
	  }


	  public virtual void DeleteGroup()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteGroup("group");
	  }


	  public virtual void newTenant()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.NewTenant("tenant");
	  }
         
	  public virtual void saveTenant()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.SaveTenant(null);
	  }


	  public virtual void DeleteTenant()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteTenant("tenant");
	  }


	  public virtual void createGroupMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.CreateMembership("user", "group");
	  }


	  public virtual void deleteGroupMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteMembership("user", "group");
	  }


	  public virtual void CreateTenantUserMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.CreateTenantUserMembership("tenant", "user");
	  }


	  public virtual void CreateTenantGroupMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.CreateTenantGroupMembership("tenant", "group");
	  }


	  public virtual void DeleteTenantUserMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteTenantUserMembership("tenant", "user");
	  }


	  public virtual void DeleteTenantGroupMembership()
	  {
		//thrown.Expect(typeof(System.NotSupportedException));
		//thrown.ExpectMessage("This identity service implementation is read-only.");

		identityService.DeleteTenantGroupMembership("tenant", "group");
	  }


	  public virtual void checkPassword()
	  {
		identityService.CheckPassword("user", "password");
	  }


	  public virtual void createQuery()
	  {
	      identityService.CreateUserQuery()
	          .ToList();
	      identityService.CreateGroupQuery()
	          .ToList();
	      identityService.CreateTenantQuery()
	          .ToList();
	  }

	}

}