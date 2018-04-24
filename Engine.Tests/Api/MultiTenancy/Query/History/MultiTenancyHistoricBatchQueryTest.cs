using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.History;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.Query.History
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>

	public class MultiTenancyHistoricBatchQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyHistoricBatchQueryTest()
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

	  protected internal IHistoryService historyService;
	  protected internal IIdentityService identityService;

	  protected internal IBatch sharedBatch;
	  protected internal IBatch tenant1Batch;
	  protected internal IBatch tenant2Batch;


	  public virtual void initServices()
	  {
		historyService = engineRule.HistoryService;
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
        [Test]
        public virtual void testHistoricBatchQueryNoAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null, null);

		// when
		IList<IHistoricBatch> batches = historyService.CreateHistoricBatchQuery().ToList();

		// then
		Assert.AreEqual(1, batches.Count);
		Assert.AreEqual(sharedBatch.Id, batches[0].Id);

		Assert.AreEqual(1, historyService.CreateHistoricBatchQuery().Count());

		identityService.ClearAuthentication();
	  }

        [Test]
        public virtual void testHistoricBatchQueryAuthenticatedTenant()
	  {
		// given
		identityService.SetAuthentication("user", null, SingletonList(TENANT_ONE));

		// when
	      IList<IHistoricBatch> batches = historyService.CreateHistoricBatchQuery()
	          .ToList();

		// then
		Assert.AreEqual(2, batches.Count);
		AssertBatches(batches, tenant1Batch.Id, sharedBatch.Id);

		Assert.AreEqual(2, historyService.CreateHistoricBatchQuery().Count());

		identityService.ClearAuthentication();
	  }

        private IList<string> SingletonList(string tENANT_ONE)
        {
            throw new NotImplementedException();
        }

        [Test]
        public virtual void testHistoricBatchQueryAuthenticatedTenants()
	  {
		// given
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		// when
		IList<IHistoricBatch> batches = historyService.CreateHistoricBatchQuery().ToList();

		// then
		Assert.AreEqual(3, batches.Count);

		Assert.AreEqual(3, historyService.CreateHistoricBatchQuery().Count());

		identityService.ClearAuthentication();
	  }

        [Test]
        public virtual void testDeleteHistoricBatch()
	  {
		// given
		identityService.SetAuthentication("user", null, SingletonList(TENANT_ONE));

		// when
		historyService.DeleteHistoricBatch(tenant1Batch.Id);

		// then
		identityService.ClearAuthentication();
		Assert.AreEqual(2, historyService.CreateHistoricBatchQuery().Count());
	  }

        [Test]
        public virtual void testDeleteHistoricBatchFailsWithWrongTenant()
	  {
		// given
		identityService.SetAuthentication("user", null, SingletonList(TENANT_ONE));

		// when
		try
		{
		  historyService.DeleteHistoricBatch(tenant2Batch.Id);
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  // then
		 // Assert.That(e.Message, CoreMatchers.Does.Contain("Cannot Delete historic batch '" + tenant2Batch.Id + "' because it belongs to no authenticated tenant"));
		}

		identityService.ClearAuthentication();
	  }


        [Test]
        public virtual void testHistoricBatchQueryFilterByTenant()
	  {
		// when
		IHistoricBatch returnedBatch = historyService.CreateHistoricBatchQuery(c=>c.TenantId == TENANT_ONE).First();

		// then
		Assert.NotNull(returnedBatch);
		Assert.AreEqual(tenant1Batch.Id, returnedBatch.Id);
	  }

        [Test]
        public virtual void testHistoricBatchQueryFilterByTenants()
	  {
		// when
		IList<IHistoricBatch> returnedBatches = historyService.CreateHistoricBatchQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Asc()*/.ToList();

		// then
		Assert.AreEqual(2, returnedBatches.Count);
		Assert.AreEqual(tenant1Batch.Id, returnedBatches[0].Id);
		Assert.AreEqual(tenant2Batch.Id, returnedBatches[1].Id);
	  }

        [Test]
        public virtual void testHistoricBatchQueryFilterWithoutTenantId()
	  {
		// when
		IHistoricBatch returnedBatch = historyService.CreateHistoricBatchQuery(c=>c.TenantId == null).First();

		// then
		Assert.NotNull(returnedBatch);
		Assert.AreEqual(sharedBatch.Id, returnedBatch.Id);
	  }

        [Test]
        public virtual void testBatchQueryFailOnNullTenantIdCase1()
	  {

		string[] tenantIds = null;
		try
		{
		  historyService.CreateHistoricBatchQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

        [Test]
        public virtual void testBatchQueryFailOnNullTenantIdCase2()
	  {

		string[] tenantIds = new string[]{null};
		try
		{
		  historyService.CreateHistoricBatchQuery(c=>tenantIds.Contains(c.TenantId));
		  Assert.Fail("exception expected");
		}
		catch (NullValueException)
		{
		  // happy path
		}
	  }

        [Test]
        public virtual void testOrderByTenantIdAsc()
	  {

		// when
		IList<IHistoricBatch> orderedBatches = historyService.CreateHistoricBatchQuery()/*.OrderByTenantId()*//*.Asc()*/.ToList();

            // then
           TestOrderingUtil.verifySorting(orderedBatches, TestOrderingUtil.historicBatchByTenantId());
	  }

        [Test]
        public virtual void testOrderByTenantIdDesc()
	  {

		// when
		IList<IHistoricBatch> orderedBatches = historyService.CreateHistoricBatchQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

		// then
		TestOrderingUtil.verifySorting(orderedBatches, TestOrderingUtil.inverted(TestOrderingUtil.historicBatchByTenantId()));
	  }

	  protected internal virtual void AssertBatches(IList<IHistoricBatch> actualBatches, params string[] expectedIds)
	  {
		Assert.AreEqual(expectedIds.Length, actualBatches.Count);

		ISet<string> actualIds = new HashSet<string>();
		foreach (IHistoricBatch batch in actualBatches)
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