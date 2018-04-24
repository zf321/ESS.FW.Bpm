using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

    public class MultiTenancyCommandTenantCheckTest
	{


	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
	  protected internal IIdentityService identityService;


	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		identityService = engineRule.IdentityService;

		identityService.SetAuthentication("user", null, null);
	  }


	  public virtual void disableTenantCheckForProcessEngine()
	  {
		// disable tenant check for process engine
		processEngineConfiguration.SetTenantCheckEnabled(false);

		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public object Execute(CommandContext commandContext)
		  {
			// cannot enable tenant check for command when it is disabled for process engine
			commandContext.EnableTenantCheck();
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(false));

			return null;
		  }
	  }


	  public virtual void disableTenantCheckForCommand()
	  {

		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));

		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass2 : ICommand<object>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass2(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public object Execute(CommandContext commandContext)
		  {
			// disable tenant check for the current command
			commandContext.DisableTenantCheck();
			Assert.That(commandContext.TenantCheckEnabled, Is.EqualTo(false));
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(false));

			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass3 : ICommand<object>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass3(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public object Execute(CommandContext commandContext)
		  {
			// Assert that it is enabled again for further commands
			Assert.That(commandContext.TenantCheckEnabled, Is.EqualTo(true));
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(true));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableAndEnableTenantCheckForCommand()
	  public virtual void disableAndEnableTenantCheckForCommand()
	  {

		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass4(this));
	  }

	  private class CommandAnonymousInnerClass4 : ICommand<object>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass4(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public object Execute(CommandContext commandContext)
		  {

			commandContext.DisableTenantCheck();
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(false));

			commandContext.EnableTenantCheck();
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(true));

			return null;
		  }
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void disableTenantCheckForCamundaAdmin()
	  public virtual void disableTenantCheckForCamundaAdmin()
	  {
		//identityService.SetAuthentication("user", Collections.SingletonList(Groups.GroupsFields.CamundaAdmin), null);

		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass5(this));
	  }

	  private class CommandAnonymousInnerClass5 : ICommand<object>
	  {
		  private readonly MultiTenancyCommandTenantCheckTest outerInstance;

		  public CommandAnonymousInnerClass5(MultiTenancyCommandTenantCheckTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public object Execute(CommandContext commandContext)
		  {
			// camunda-admin should access data from all tenants
			Assert.That(commandContext.TenantManager.TenantCheckEnabled, Is.EqualTo(false));

			return null;
		  }
	  }

	}

}