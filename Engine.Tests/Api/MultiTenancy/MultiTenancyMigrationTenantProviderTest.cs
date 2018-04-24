using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;
using ProcessEngineBootstrapRule = Engine.Tests.Util.ProcessEngineBootstrapRule;

namespace Engine.Tests.Api.MultiTenancy
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyMigrationTenantProviderTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationTenantProviderTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new ProcessEngineTestRule(engineRule);
			//tenantRuleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);
	  protected internal ProcessEngineTestRule testHelper;


	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : Util.ProcessEngineBootstrapRule
	  {
		  public ProcessEngineBootstrapRuleAnonymousInnerClass()
		  {
		  }

		  public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
		  {

		  ITenantIdProvider tenantIdProvider = new VariableBasedTenantIdProvider();
		  configuration.TenantIdProvider = tenantIdProvider;

		  return configuration;
		  }
	  }


        [Test]
        public virtual void cannotMigrateInstanceBetweenDifferentTenants()
	  {
		// given
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition tenantDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = startInstanceForTenant(sharedDefinition, TENANT_ONE);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sharedDefinition.Id, tenantDefinition.Id).MapEqualActivities().Build();

		// when
		try
		{
		  engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		 // Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot migrate process instance '" + processInstance.Id + "' " + "to a process definition of a different tenant ('tenant1' != 'tenant2')"));
		}

		// then
		Assert.NotNull(migrationPlan);
	  }


        [Test]
        public virtual void canMigrateInstanceBetweenSameTenantCase2()
	  {
		// given
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = startInstanceForTenant(sharedDefinition, TENANT_ONE);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sharedDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		// when
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertInstanceOfDefinition(processInstance, targetDefinition);
	  }


        [Test]
        public virtual void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance1 = startInstanceForTenant(sourceDefinition, TENANT_ONE);
		IProcessInstance processInstance2 = startInstanceForTenant(sourceDefinition, TENANT_TWO);

		// when
		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceQuery(engineRule.RuntimeService.CreateProcessInstanceQuery()).Execute();
		engineRule.IdentityService.ClearAuthentication();

		// then
		AssertInstanceOfDefinition(processInstance1, targetDefinition);
		AssertInstanceOfDefinition(processInstance2, sourceDefinition);
	  }


        [Test]
        public virtual void canMigrateWithProcessInstanceQueryAllInstancesOfAuthenticatedTenants()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IProcessInstance processInstance1 = startInstanceForTenant(sourceDefinition, TENANT_ONE);
		IProcessInstance processInstance2 = startInstanceForTenant(sourceDefinition, TENANT_TWO);

		// when
		engineRule.IdentityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceQuery(engineRule.RuntimeService.CreateProcessInstanceQuery()).Execute();
		engineRule.IdentityService.ClearAuthentication();

		// then
		AssertInstanceOfDefinition(processInstance1, targetDefinition);
		AssertInstanceOfDefinition(processInstance2, targetDefinition);
	  }

	  protected internal virtual void AssertInstanceOfDefinition(IProcessInstance processInstance, IProcessDefinition targetDefinition)
	  {
		Assert.AreEqual(1, engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ProcessDefinitionId == targetDefinition.Id).Count());
	  }

	  protected internal virtual IProcessInstance startInstanceForTenant(IProcessDefinition processDefinition, string tenantId)
	  {
		return engineRule.RuntimeService.StartProcessInstanceById(processDefinition.Id, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
            .PutValue(VariableBasedTenantIdProvider.TENANT_VARIABLE, tenantId) as IDictionary<string, ITypedValue>);
	  }

	  public class VariableBasedTenantIdProvider : ITenantIdProvider
	  {
		public const string TENANT_VARIABLE = "tenantId";

		public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
		{
		  return (string) ctx.Variables.GetValue(TENANT_VARIABLE, typeof(string));
		}

		public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
            {
                return (string)ctx.Variables.GetValue(TENANT_VARIABLE, typeof(string));
            }

		public string ProvideTenantIdForHistoricDecisionInstance(TenantIdProviderHistoricDecisionInstanceContext ctx)
		{
		  return null;
		}

           
        }
	}

}