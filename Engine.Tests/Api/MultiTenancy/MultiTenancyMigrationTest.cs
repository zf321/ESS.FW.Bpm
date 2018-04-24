using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyMigrationTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationTest()
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
			////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;


        [Test]
        public virtual void cannotCreateMigrationPlanBetweenDifferentTenants()
	  {
		// given
		IProcessDefinition tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.OneTaskProcess);

		// when
		try
		{
		  engineRule.RuntimeService.CreateMigrationPlan(tenant1Definition.Id, tenant2Definition.Id).MapEqualActivities().Build();
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		//  Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot migrate process instances between processes of different tenants ('tenant1' != 'tenant2')"));
		}
	  }


        [Test]
        public virtual void canCreateMigrationPlanFromTenantToNoTenant()
	  {
		// given
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition tenantDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);


		// when
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(tenantDefinition.Id, sharedDefinition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);
	  }


        [Test]
        public virtual void canCreateMigrationPlanFromNoTenantToTenant()
	  {
		// given
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition tenantDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);


		// when
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sharedDefinition.Id, tenantDefinition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);
	  }


        [Test]
        public virtual void canCreateMigrationPlanForNoTenants()
	  {
		// given
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);


		// when
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sharedDefinition.Id, sharedDefinition.Id).MapEqualActivities().Build();

		// then
		Assert.NotNull(migrationPlan);
	  }


        [Test]
        public virtual void canMigrateInstanceBetweenSameTenantCase1()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		// when
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertMigratedTo(processInstance, targetDefinition);
	  }


        [Test]
        public virtual void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		// when
		try
		{
		  engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		 // Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot migrate process instance '" + processInstance.Id + "' without tenant to a process definition with a tenant ('tenant1')"));
		}
	  }


        [Test]
        public virtual void canMigrateInstanceWithTenantIdToDefinitionWithoutTenantId()
	  {
		// given
		IProcessDefinition sourceDefinition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		IMigrationPlan migrationPlan = engineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		// when
		engineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).Execute();

		// then
		AssertMigratedTo(processInstance, targetDefinition);
	  }

	  protected internal virtual void AssertMigratedTo(IProcessInstance processInstance, IProcessDefinition targetDefinition)
	  {
		Assert.AreEqual(1, engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ProcessDefinitionId == targetDefinition.Id)
            .Count());
	  }
	}

}