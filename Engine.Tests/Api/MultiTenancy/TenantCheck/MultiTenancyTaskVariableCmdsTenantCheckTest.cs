using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>

	public class MultiTenancyTaskVariableCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTaskVariableCmdsTenantCheckTest()
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

	  protected internal const string VARIABLE_1 = "testVariable1";
	  protected internal const string VARIABLE_2 = "testVariable2";

	  protected internal const string VARIABLE_VALUE_1 = "test1";
	  protected internal const string VARIABLE_VALUE_2 = "test2";

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal static readonly IBpmnModelInstance ONE_TASK_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask("task").EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;



	  protected internal string taskId;

        [SetUp]
	  public virtual void init()
	  {

		// deploy tenants
		testRule.DeployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue(VARIABLE_1, VARIABLE_VALUE_1).PutValue(VARIABLE_2, VARIABLE_VALUE_2));

		taskId = engineRule.TaskService.CreateTaskQuery().First().Id;

	  }

	   [Test]
	  public virtual void getTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.TaskService.GetVariable(taskId, VARIABLE_1));
	  }


        [Test]
        public virtual void getTaskVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.GetVariable(taskId, VARIABLE_1);
	  }



        [Test]
        public virtual void getTaskVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.TaskService.GetVariable(taskId, VARIABLE_1));
	  }


        [Test]
        public virtual void getTaskVariableTypedWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.TaskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_1).Value);

	  }


        [Test]
        public virtual void getTaskVariableTypedWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_1);
	  }


        [Test]
        public virtual void getTaskVariableTypedWithDisableTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.TaskService.GetVariableTyped<ITypedValue>(taskId, VARIABLE_1).Value);
	  }


        [Test]
        public virtual void getTaskVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(2, engineRule.TaskService.GetVariables(taskId).Count);
	  }


        [Test]
        public virtual void getTaskVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.GetVariables(taskId);

	  }


        [Test]
        public virtual void getTaskVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		Assert.AreEqual(2, engineRule.TaskService.GetVariables(taskId).Count);
	  }



        [Test]
        public virtual void setTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		engineRule.TaskService.SetVariable(taskId, "newVariable", "newValue");

		Assert.AreEqual(3, engineRule.TaskService.GetVariables(taskId).Count);
	  }


        [Test]
        public virtual void setTaskVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the task '" + taskId + "' because it belongs to no authenticated tenant.");
		engineRule.TaskService.SetVariable(taskId, "newVariable", "newValue");

	  }


        [Test]
        public virtual void setTaskVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		engineRule.TaskService.SetVariable(taskId, "newVariable", "newValue");
		Assert.AreEqual(3, engineRule.TaskService.GetVariables(taskId).Count);

	  }


        [Test]
        public virtual void removeTaskVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		engineRule.TaskService.RemoveVariable(taskId, VARIABLE_1);
		// then
		Assert.AreEqual(1, engineRule.TaskService.GetVariables(taskId).Count);
	  }


        [Test]
        public virtual void removeTaskVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the task '" + taskId + "' because it belongs to no authenticated tenant.");

		engineRule.TaskService.RemoveVariable(taskId, VARIABLE_1);
	  }


        [Test]
        public virtual void removeTaskVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		engineRule.TaskService.RemoveVariable(taskId, VARIABLE_1);
		Assert.AreEqual(1, engineRule.TaskService.GetVariables(taskId).Count);
	  }

	}

}