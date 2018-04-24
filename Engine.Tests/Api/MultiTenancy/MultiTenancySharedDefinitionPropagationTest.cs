using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;
using ProcessEngineBootstrapRule = Engine.Tests.Util.ProcessEngineBootstrapRule;

namespace Engine.Tests.Api.MultiTenancy
{
    

	public class MultiTenancySharedDefinitionPropagationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySharedDefinitionPropagationTest()
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
			//ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule).around(testRule);
		}


	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal const string TENANT_ID = "tenant1";


	  protected static internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : Util.ProcessEngineBootstrapRule
	  {
		  public ProcessEngineBootstrapRuleAnonymousInnerClass()
		  {
		  }

		  public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
		  {

			ITenantIdProvider tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ID);
			configuration.TenantIdProvider = tenantIdProvider;

			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void propagateTenantIdToProcessInstance()
	  {
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().EndEvent().Done());

		engineRule.RuntimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY);

		IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery().First();
		Assert.That(processInstance!=null);
		// get the tenant id from the provider
		Assert.That(processInstance.TenantId, Is.EqualTo(TENANT_ID));
	  }


        [Test]
        public virtual void propagateTenantIdToIntermediateTimerJob()
	  {
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().IntermediateCatchEvent().TimerWithDuration("PT1M").EndEvent().Done());

		engineRule.RuntimeService.StartProcessInstanceByKey("process");

		// the job is created when the timer event is reached
		IJob job = engineRule.ManagementService.CreateJobQuery().First();
		Assert.That(job!=null);
		// inherit the tenant id from execution
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ID));
	  }


        [Test]
        public virtual void propagateTenantIdToAsyncJob()
	  {
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().CamundaAsyncBefore().EndEvent().Done());

		engineRule.RuntimeService.StartProcessInstanceByKey("process");

		// the job is created when the asynchronous activity is reached
		IJob job = engineRule.ManagementService.CreateJobQuery().First();
		Assert.That(job!=null);
		// inherit the tenant id from execution
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ID));
	  }

	}

}