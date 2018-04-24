using System.Collections.Generic;
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
	public class MultiTenancyActivityCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyActivityCmdsTenantCheckTest()
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

	  protected internal static readonly IBpmnModelInstance ONE_TASK_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;

	  protected internal string ProcessInstanceId;

        [SetUp]
	  public virtual void init()
	  {

		testRule.DeployForTenant(TENANT_ONE, ONE_TASK_PROCESS);

		ProcessInstanceId = engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY).Id;
	  }

        [Test]
        public virtual void getActivityInstanceWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
	  }
        [Test]

        public virtual void getActivityInstanceWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId);

	  }

        [Test]
        public virtual void getActivityInstanceWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.NotNull(engineRule.RuntimeService.GetActivityInstance(ProcessInstanceId));
	  }
        [Test]

        public virtual void getActivityIdsWithAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null,new List<string>(){TENANT_ONE});

		// then
		Assert.AreEqual(1, engineRule.RuntimeService.GetActiveActivityIds(ProcessInstanceId).Count);

	  }

        [Test]
        public virtual void getActivityIdsWithNoAuthenticatedTenant()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot read the process instance '" + ProcessInstanceId + "' because it belongs to no authenticated tenant.");
		// when
		engineRule.RuntimeService.GetActiveActivityIds(ProcessInstanceId);

	  }

        [Test]
        public virtual void getActivityIdsWithDisabledTenantCheck()
	  {

		engineRule.IdentityService.SetAuthentication("aUserId", null);
		engineRule.ProcessEngineConfiguration.SetTenantCheckEnabled(false);

		// then
		Assert.AreEqual(1, engineRule.RuntimeService.GetActiveActivityIds(ProcessInstanceId).Count);

	  }

	}

}