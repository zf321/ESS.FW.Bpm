using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{
    

	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyProcessInstanceCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessInstanceCmdsTenantCheckTest()
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

	  protected internal const string PROCESS_DEFINITION_KEY = "oneTaskProcess";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string ProcessInstanceId;



	  protected internal static readonly IBpmnModelInstance ONE_TASK_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask("task").EndEvent().Done();

     [SetUp]
	  public virtual void init()
	  {
		// deploy tenants
		testRule.DeployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;

	  }


        [Test]
        public virtual void deleteProcessInstanceWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		engineRule.RuntimeService.DeleteProcessInstance(ProcessInstanceId, null);

		Assert.AreEqual(0, engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId).Count());
	  }


        [Test]
        public virtual void deleteProcessInstanceWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");

		// when
		engineRule.RuntimeService.DeleteProcessInstance(ProcessInstanceId, null);
	  }


        [Test]
        public virtual void deleteProcessInstanceWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		//then
		engineRule.RuntimeService.DeleteProcessInstance(ProcessInstanceId, null);

		Assert.AreEqual(0, engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId).Count());
	  }


        [Test]
        public virtual void modifyProcessInstanceWithAuthenticatedTenant()
	  {

		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// when
		engineRule.RuntimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();

		Assert.IsNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
	  }


        [Test]
        public virtual void modifyProcessInstanceWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot update the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");

		// when
		engineRule.RuntimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();
	  }


        [Test]
        public virtual void modifyProcessInstanceWithDisabledTenantCheck()
	  {

		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// when
		engineRule.RuntimeService.CreateProcessInstanceModification(ProcessInstanceId).CancelAllForActivity("task").Execute();

		Assert.IsNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
	  }
	}

}