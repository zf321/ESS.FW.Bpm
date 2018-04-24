using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;
using ProcessEngineBootstrapRule = Engine.Tests.Util.ProcessEngineBootstrapRule;

namespace Engine.Tests.Api.MultiTenancy.Suspensionstate
{
    
	public class MultiTenancyProcessDefinitionSuspensionStateTenantIdProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyProcessDefinitionSuspensionStateTenantIdProviderTest()
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


	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal const string PROCESS_DEFINITION_KEY = "testProcess";

	  protected internal static readonly IBpmnModelInstance PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_DEFINITION_KEY).StartEvent().UserTask().CamundaAsyncBefore().EndEvent().Done();


	  protected internal static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : Util.ProcessEngineBootstrapRule
	  {
		  public ProcessEngineBootstrapRuleAnonymousInnerClass()
		  {
		  }

		  public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
		  {

			ITenantIdProvider tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ONE);
			configuration.TenantIdProvider = tenantIdProvider;

			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

      [SetUp]
	  public virtual void setUp()
	  {

		testRule.Deploy(PROCESS);
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void suspendProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  public virtual void suspendProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  {
		// given active process instances with tenant id of process definition without tenant id
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Execute();

		//IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == null).First();

		// IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionIdEf==processDefinition.Id);
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));

		//// suspend all instances of process definition
		//engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).IncludeProcessInstances(true).Suspend();

		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void activateProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  public virtual void activateProcessDefinitionByIdIncludeInstancesFromAllTenants()
	  {
		// given suspended process instances with tenant id of process definition without tenant id
		engineRule.RuntimeService.CreateProcessInstanceByKey(PROCESS_DEFINITION_KEY).ProcessDefinitionWithoutTenantId().Execute();

		engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionKey(PROCESS_DEFINITION_KEY).IncludeProcessInstances(true).Suspend();

		//IProcessDefinition processDefinition = engineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.TenantId == null).First();

		// IQueryable<IProcessInstance> query = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionIdEf==processDefinition.Id);
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(0L));

		//// activate all instance of process definition
		//engineRule.RepositoryService.UpdateProcessDefinitionSuspensionState().ByProcessDefinitionId(processDefinition.Id).IncludeProcessInstances(true).Activate();

		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode).Count(), Is.EqualTo(0L));
		//Assert.That(query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode).Count(), Is.EqualTo(1L));
		//Assert.That(query.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode &&c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }
	}

}