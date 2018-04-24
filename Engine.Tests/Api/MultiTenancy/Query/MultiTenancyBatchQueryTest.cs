using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Query
{
    
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancyBatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyBatchQueryTest()
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
	  protected internal IIdentityService identityService;

	  protected internal IBatch sharedBatch;
	  protected internal IBatch tenant1Batch;
	  protected internal IBatch tenant2Batch;


	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		identityService = engineRule.IdentityService;
	  }


	  public virtual void deployProcesses()
	  {
		IProcessDefinition sharedDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
		IProcessDefinition tenant1Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_ONE, ProcessModels.OneTaskProcess);
		IProcessDefinition tenant2Definition = testHelper.DeployForTenantAndGetDefinition(TENANT_TWO, ProcessModels.OneTaskProcess);

		sharedBatch = batchHelper.MigrateProcessInstanceAsync(sharedDefinition, sharedDefinition);
		tenant1Batch = batchHelper.MigrateProcessInstanceAsync(tenant1Definition, tenant1Definition);
		tenant2Batch = batchHelper.MigrateProcessInstanceAsync(tenant2Definition, tenant2Definition);
	  }


	  public virtual void removeBatches()
	  {
		batchHelper.RemoveAllRunningAndHistoricBatches();
	  }
    

	   [Test]   public virtual void testBatchQueryNoAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null, null);

		// then
	      IList<IBatch> batches = managementService.CreateBatchQuery()
	          .ToList();
		Assert.AreEqual(1, batches.Count);
		Assert.AreEqual(sharedBatch.Id, batches[0].Id);

		Assert.AreEqual(1, managementService.CreateBatchQuery().Count());

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchQueryAuthenticatedTenant()
	  {
		// given
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		IList<IBatch> batches = managementService.CreateBatchQuery().ToList();

		// then
		Assert.AreEqual(2, batches.Count);
		AssertBatches<IBatch>(batches, tenant1Batch.Id, sharedBatch.Id);

		Assert.AreEqual(2, managementService.CreateBatchQuery().Count());

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchQueryAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		// when
		IList<IBatch> batches = managementService.CreateBatchQuery().ToList();

		// then
		Assert.AreEqual(3, batches.Count);
		Assert.AreEqual(3, managementService.CreateBatchQuery().Count());

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchStatisticsNoAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null, null);

		// when
		//IList<IBatchStatistics> statistics = managementService.CreateBatchStatisticsQuery().ToList();

		// then
		//Assert.AreEqual(1, statistics.Count);
		//Assert.AreEqual(sharedBatch.Id, statistics[0].Id);

		//Assert.AreEqual(1, managementService.CreateBatchStatisticsQuery().Count);

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchStatisticsAuthenticatedTenant()
	  {
		// given
		//identityService.SetAuthentication("user", null, singletonList(TENANT_ONE));

		// when
		//IList<IBatchStatistics> statistics = managementService.CreateBatchStatisticsQuery().ToList();

		// then
		//Assert.AreEqual(2, statistics.Count);

		//Assert.AreEqual(2, managementService.CreateBatchStatisticsQuery().Count());

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchStatisticsAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		// then
		//IList<IBatchStatistics> statistics = managementService.CreateBatchStatisticsQuery().ToList();
		//Assert.AreEqual(3, statistics.Count);

		//Assert.AreEqual(3, managementService.CreateBatchStatisticsQuery().Count());

		identityService.ClearAuthentication();
	  }


	   [Test]   public virtual void testBatchQueryFilterByTenant()
	  {
		// when
		IBatch returnedBatch = managementService.CreateBatchQuery(c=>c.TenantId == TENANT_ONE).First();

		// then
		Assert.NotNull(returnedBatch);
		Assert.AreEqual(tenant1Batch.Id, returnedBatch.Id);
	  }


	   [Test]   public virtual void testBatchQueryFilterByTenants()
	  {
		// when
		IList<IBatch> returnedBatches = managementService.CreateBatchQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		// then
		Assert.AreEqual(2, returnedBatches.Count);
		Assert.AreEqual(tenant1Batch.Id, returnedBatches[0].Id);
		Assert.AreEqual(tenant2Batch.Id, returnedBatches[1].Id);
	  }


	   [Test]   public virtual void testBatchQueryFilterWithoutTenantId()
	  {
		// when
		IBatch returnedBatch = managementService.CreateBatchQuery(c=>c.TenantId == null).First();

		// then
		Assert.NotNull(returnedBatch);
		Assert.AreEqual(sharedBatch.Id, returnedBatch.Id);
	  }


	   [Test]   public virtual void testBatchQueryFailOnNullTenantIdCase1()
	  {

		string[] tenantIds = null;
		try
		{
		  managementService.CreateBatchQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }


	   [Test]   public virtual void testBatchQueryFailOnNullTenantIdCase2()
	  {

		string[] tenantIds = new string[]{null};
		try
		{
		  managementService.CreateBatchQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }


	   [Test]   public virtual void testOrderByTenantIdAsc()
	  {

		// when
		IList<IBatch> orderedBatches = managementService.CreateBatchQuery()/*.OrderByTenantId()*//*.Asc()*/.ToList();

		// then
		TestOrderingUtil.verifySorting<IBatch>(orderedBatches, TestOrderingUtil.batchByTenantId());
	  }


	   [Test]   public virtual void testOrderByTenantIdDesc()
	  {

		// when
		IList<IBatch> orderedBatches = managementService.CreateBatchQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		// then
		TestOrderingUtil.verifySorting(orderedBatches, TestOrderingUtil.inverted(TestOrderingUtil.batchByTenantId()));
	  }


	   [Test]   public virtual void testBatchStatisticsQueryFilterByTenant()
	  {
		// when
		//IBatchStatistics returnedBatch = managementService.CreateBatchStatisticsQuery(c=>c.TenantId == TENANT_ONE).First();

		// then
		//Assert.NotNull(returnedBatch);
		//Assert.AreEqual(tenant1Batch.Id, returnedBatch.Id);
	  }


	   [Test]   public virtual void testBatchStatisticsQueryFilterByTenants()
	  {
		// when
		//IList<IBatchStatistics> returnedBatches = managementService.CreateBatchStatisticsQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
          //      /*.OrderByTenantId()*//*.Asc()*/.ToList();

		// then
		//Assert.AreEqual(2, returnedBatches.Count);
		//Assert.AreEqual(tenant1Batch.Id, returnedBatches[0].Id);
		//Assert.AreEqual(tenant2Batch.Id, returnedBatches[1].Id);
	  }


	   [Test]   public virtual void testBatchStatisticsQueryFilterWithoutTenantId()
	  {
		// when
		//IBatchStatistics returnedBatch = managementService.CreateBatchStatisticsQuery(c=>c.TenantId == null).First();

		//// then
		//Assert.NotNull(returnedBatch);
		//Assert.AreEqual(sharedBatch.Id, returnedBatch.Id);
	  }


	   [Test]   public virtual void testBatchStatisticsQueryFailOnNullTenantIdCase1()
	  {

		string[] tenantIds = null;
		try
		{
		  managementService.CreateBatchStatisticsQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }


	   [Test]   public virtual void testBatchStatisticsQueryFailOnNullTenantIdCase2()
	  {

		string[] tenantIds = new string[]{null};
		try
		{
		  managementService.CreateBatchStatisticsQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }


	   [Test]   public virtual void testBatchStatisticsQueryOrderByTenantIdAsc()
	  {
		// when
		//IList<IBatchStatistics> orderedBatches = managementService.CreateBatchStatisticsQuery()/*.OrderByTenantId()*//*.Asc()*/.ToList();

		//// then
		//TestOrderingUtil.verifySorting(orderedBatches, TestOrderingUtil.batchStatisticsByTenantId());
	  }


	   [Test]   public virtual void testBatchStatisticsQueryOrderByTenantIdDesc()
	  {
		// when
		//IList<IBatchStatistics> orderedBatches = managementService.CreateBatchStatisticsQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		// then
		//TestOrderingUtil.verifySorting(orderedBatches, TestOrderingUtil.inverted(TestOrderingUtil.batchStatisticsByTenantId()));
	  }

	  protected internal virtual void AssertBatches<T1>(IList<T1> actualBatches, params string[] expectedIds) where T1 : IBatch
	  {
		Assert.AreEqual(expectedIds.Length, actualBatches.Count);

		ISet<string> actualIds = new HashSet<string>();
		foreach (IBatch batch in actualBatches)
		{
		  actualIds.Add(batch.Id);
		}

		foreach (string expectedId in expectedIds)
		{
		  Assert.True(actualIds.Contains(expectedId));
		}
	  }
	}

}