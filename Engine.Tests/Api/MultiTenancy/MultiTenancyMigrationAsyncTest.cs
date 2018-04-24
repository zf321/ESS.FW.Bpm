using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Batch;
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
	public class MultiTenancyMigrationAsyncTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyMigrationAsyncTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			defaultTestRule = new ProcessEngineTestRule(defaultEngineRule);
			migrationRule = new MigrationTestRule(defaultEngineRule);
			//defaultRuleChin = RuleChain.outerRule(defaultEngineRule).around(defaultTestRule).around(migrationRule);
			batchHelper = new BatchMigrationHelper(defaultEngineRule, migrationRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule defaultEngineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule defaultTestRule;
	  protected internal MigrationTestRule migrationRule;



	  protected internal BatchMigrationHelper batchHelper;


	  public virtual void removeBatches()
	  {
		batchHelper.RemoveAllRunningAndHistoricBatches();
	  }
        
	  public virtual void canMigrateInstanceBetweenSameTenantCase1()
	  {
		// given
		IProcessDefinition sourceDefinition = defaultTestRule.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = defaultTestRule.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = defaultEngineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		IMigrationPlan migrationPlan = defaultEngineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IBatch batch = defaultEngineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();

		batchHelper.ExecuteSeedJob(batch);

		// when
		batchHelper.ExecuteJobs(batch);

		// then
		AssertMigratedTo(processInstance, targetDefinition);
	  }


	  public virtual void cannotMigrateInstanceWithoutTenantIdToDifferentTenant()
	  {
		// given
		IProcessDefinition sourceDefinition = defaultTestRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition targetDefinition = defaultTestRule.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);

		IProcessInstance processInstance = defaultEngineRule.RuntimeService.StartProcessInstanceById(sourceDefinition.Id);
		IMigrationPlan migrationPlan = defaultEngineRule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id).MapEqualActivities().Build();

		IBatch batch = defaultEngineRule.RuntimeService.NewMigration(migrationPlan).ProcessInstanceIds((processInstance.Id)).ExecuteAsync();

		batchHelper.ExecuteSeedJob(batch);

		// when
		batchHelper.ExecuteJobs(batch);

		// then
		IJob migrationJob = batchHelper.GetExecutionJobs(batch)[0];
		//Assert.That(migrationJob.ExceptionMessage, CoreMatchers.Does.Contain("Cannot migrate process instance '" + processInstance.Id + "' without tenant to a process definition with a tenant ('tenant1')"));
	  }

	  protected internal virtual void AssertMigratedTo(IProcessInstance processInstance, IProcessDefinition targetDefinition)
	  {
		Assert.AreEqual(1, defaultEngineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id&& c.ProcessDefinitionId == targetDefinition.Id).Count());
	  }
	}

}