using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    
	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>

	public class MultiTenancyFormVariablesCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyFormVariablesCmdsTenantCheckTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal const string VARIABLE_1 = "testVariable1";
	  protected internal const string VARIABLE_2 = "testVariable2";

	  protected internal const string VARIABLE_VALUE_1 = "test1";
	  protected internal const string VARIABLE_VALUE_2 = "test2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;



	  protected internal IProcessInstance instance;



	  protected internal const string START_FORM_RESOURCE = "resources/api/form/FormServiceTest.StartFormFields.bpmn20.xml";

        [SetUp]
	  public virtual void init()
	  {

		// deploy tenants
		testRule.DeployForTenant(TENANT_ONE, START_FORM_RESOURCE);
		instance = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue(VARIABLE_1, VARIABLE_VALUE_1).PutValue(VARIABLE_2, VARIABLE_VALUE_2));
	  }

	  
	   [Test]   public virtual void testGetStartFormVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		Assert.AreEqual(4, engineRule.FormService.GetStartFormVariables(instance.ProcessDefinitionId).Count);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesWithNoAuthenticatedTenant()
	   [Test]   public virtual void testGetStartFormVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the process definition '" + instance.ProcessDefinitionId + "' because it belongs to no authenticated tenant.");

		engineRule.FormService.GetStartFormVariables(instance.ProcessDefinitionId);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStartFormVariablesWithDisabledTenantCheck()
	   [Test]   public virtual void testGetStartFormVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		Assert.AreEqual(4, engineRule.FormService.GetStartFormVariables(instance.ProcessDefinitionId).Count);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithAuthenticatedTenant()
	   [Test]   public virtual void testGetTaskFormVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		ITask task = engineRule.TaskService.CreateTaskQuery().First();

		Assert.AreEqual(2, engineRule.FormService.GetTaskFormVariables(task.Id).Count);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithNoAuthenticatedTenant()
	   [Test]   public virtual void testGetTaskFormVariablesWithNoAuthenticatedTenant()
	  {

		ITask task = engineRule.TaskService.CreateTaskQuery().First();

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + task.Id + "' because it belongs to no authenticated tenant.");

		engineRule.FormService.GetTaskFormVariables(task.Id);

	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetTaskFormVariablesWithDisabledTenantCheck()
	   [Test]   public virtual void testGetTaskFormVariablesWithDisabledTenantCheck()
	  {

		ITask task = engineRule.TaskService.CreateTaskQuery().First();

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		Assert.AreEqual(2, engineRule.FormService.GetTaskFormVariables(task.Id).Count);

	  }
	}

}