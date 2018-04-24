using System.Collections.Generic;
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
	public class MultiTenancyExecutionVariableCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyExecutionVariableCmdsTenantCheckTest()
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

	  protected internal static readonly IBpmnModelInstance ONE_TASK_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string ProcessInstanceId;

        [SetUp]
	  public virtual void init()
	  {

		// deploy tenants
		testRule.DeployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue(VARIABLE_1, VARIABLE_VALUE_1).PutValue(VARIABLE_2, VARIABLE_VALUE_2)).Id;
	  }

        [Test]
        public virtual void getExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.RuntimeService.GetVariable(ProcessInstanceId, VARIABLE_1));
	  }

        [Test]
        public virtual void getExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.GetVariable(ProcessInstanceId, VARIABLE_1);

	  }

        [Test]
        public virtual void getExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.RuntimeService.GetVariable(ProcessInstanceId, VARIABLE_1));

	  }

        [Test]
        public virtual void getExecutionVariableTypedWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.RuntimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_1).Value);
	  }

        [Test]
        public virtual void getExecutionVariableTypedWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// then
		engineRule.RuntimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_1);
	  }

        [Test]
        public virtual void getExecutionVariableTypedWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// if
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(VARIABLE_VALUE_1, engineRule.RuntimeService.GetVariableTyped<ITypedValue>(ProcessInstanceId, VARIABLE_1).Value);

	  }

        [Test]
        public virtual void getExecutionVariablesWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(2, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);
	  }

        [Test]
        public virtual void getExecutionVariablesWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		//engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count;
        Assert.AreEqual(2, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);

        }

        [Test]
        public virtual void getExecutionVariablesWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(2, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);

	  }

        [Test]
        public virtual void setExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		engineRule.RuntimeService.SetVariable(ProcessInstanceId, "newVariable", "newValue");
		Assert.AreEqual(3, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);
	  }

        [Test]
        public virtual void setExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.SetVariable(ProcessInstanceId, "newVariable", "newValue");

	  }

        [Test]
        public virtual void setExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);
		engineRule.RuntimeService.SetVariable(ProcessInstanceId, "newVariable", "newValue");
		Assert.AreEqual(3, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);
	  }

        [Test]
        public virtual void removeExecutionVariableWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		engineRule.RuntimeService.RemoveVariable(ProcessInstanceId, VARIABLE_1);

		// then
		Assert.AreEqual(1, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);
	  }

        [Test]
        public virtual void removeExecutionVariableWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		// then
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.RemoveVariable(ProcessInstanceId, VARIABLE_1);

	  }

        [Test]
        public virtual void removeExecutionVariableWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		engineRule.RuntimeService.RemoveVariable(ProcessInstanceId, VARIABLE_1);

		// then
		Assert.AreEqual(1, engineRule.RuntimeService.GetVariables(ProcessInstanceId).Count);
	  }
	}

}