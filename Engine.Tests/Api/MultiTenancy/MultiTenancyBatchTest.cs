using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyBatchTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyBatchTest()
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
			//defaultRuleChin = RuleChain.outerRule(engineRule).around(testHelper);
			batchHelper = new BatchMigrationHelper(engineRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testHelper;



	  protected internal BatchMigrationHelper batchHelper;

	  protected internal IManagementService managementService;
	  protected internal IHistoryService historyService;
	  protected internal IIdentityService identityService;

	  protected internal IProcessDefinition tenant1Definition;
	  protected internal IProcessDefinition tenant2Definition;
	  protected internal IProcessDefinition sharedDefinition;

      [SetUp]
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
	  }

      [Test]
	  public virtual void deployProcesses()
	  {
		sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.OneTaskProcess);
	  }

        
        public virtual void removeBatches()
	  {
		batchHelper.RemoveAllRunningAndHistoricBatches();
	  }

	  

	   [Test]   public virtual void testBatchTenantIdCase1()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(sharedDefinition, sharedDefinition);

		// then
		Assert.IsNull(batch.TenantId);
	  }

	  

	   [Test]   public virtual void testBatchTenantIdCase2()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, sharedDefinition);

		// then
		Assert.AreEqual(TENANT_ONE, batch.TenantId);
	  }

	   
 
	   [Test]   public virtual void testBatchTenantIdCase3()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(sharedDefinition, tenant1Definition);

		// then
		Assert.IsNull(batch.TenantId);
	  }

 
	   [Test]   public virtual void testHistoricBatchTenantId()
	  {
		// given
		batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		IHistoricBatch historicBatch = historyService.CreateHistoricBatchQuery().First();
		Assert.AreEqual(TENANT_ONE, historicBatch.TenantId);
	  }

 
	   [Test]   public virtual void testBatchJobDefinitionsTenantId()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		IJobDefinition migrationJobDefinition = batchHelper.GetExecutionJobDefinition(batch);
		Assert.AreEqual(TENANT_ONE, migrationJobDefinition.TenantId);

		IJobDefinition monitorJobDefinition = batchHelper.GetMonitorJobDefinition(batch);
		Assert.AreEqual(TENANT_ONE, monitorJobDefinition.TenantId);

		IJobDefinition seedJobDefinition = batchHelper.GetSeedJobDefinition(batch);
		Assert.AreEqual(TENANT_ONE, seedJobDefinition.TenantId);
	  }

 
	   [Test]   public virtual void testBatchJobsTenantId()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// then
		IJob seedJob = batchHelper.GetSeedJob(batch);
		Assert.AreEqual(TENANT_ONE, seedJob.TenantId);

		batchHelper.ExecuteSeedJob(batch);

		IList<IJob> migrationJob = batchHelper.GetExecutionJobs(batch);
		Assert.AreEqual(TENANT_ONE, migrationJob[0].TenantId);

		IJob monitorJob = batchHelper.GetMonitorJob(batch);
		Assert.AreEqual(TENANT_ONE, monitorJob.TenantId);
	  }

 
	   [Test]   public virtual void testDeleteBatch()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.DeleteBatch(batch.Id, true);
		identityService.ClearAuthentication();

		// then
		Assert.AreEqual(0, managementService.CreateBatchQuery().Count());
	  }

 
	   [Test]   public virtual void testDeleteBatchFailsWithWrongTenant()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant2Definition, tenant2Definition);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.DeleteBatch(batch.Id, true);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		 // Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot Delete batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.ClearAuthentication();
		}
	  }

 
	   [Test]   public virtual void testSuspendBatch()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.SuspendBatchById(batch.Id);
		identityService.ClearAuthentication();

		// then
		//batch = managementService.CreateBatchQuery().batchId(batch.Id).First();
		Assert.True(batch.Suspended);
	  }

 
	   [Test]   public virtual void testSuspendBatchFailsWithWrongTenant()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant2Definition, tenant2Definition);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.SuspendBatchById(batch.Id);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		 // Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot suspend batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.ClearAuthentication();
		}
	  }

 
	   [Test]   public virtual void testActivateBatch()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);
		managementService.SuspendBatchById(batch.Id);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		managementService.ActivateBatchById(batch.Id);
		identityService.ClearAuthentication();

		// then
		//batch = managementService.CreateBatchQuery().batchId(batch.Id).First();
		Assert.IsFalse(batch.Suspended);
	  }

 
	   [Test]   public virtual void testActivateBatchFailsWithWrongTenant()
	  {
		// given
		IBatch batch = batchHelper.MigrateProcessInstanceAsync(tenant2Definition, tenant2Definition);
		managementService.SuspendBatchById(batch.Id);

		// when
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));
		try
		{
		  managementService.ActivateBatchById(batch.Id);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		  //Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot activate batch '" + batch.Id + "' because it belongs to no authenticated tenant"));
		}
		finally
		{
		  identityService.ClearAuthentication();
		}
	  }

	}

}