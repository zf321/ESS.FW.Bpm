using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class BatchQueryTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        private readonly bool InstanceFieldsInitialized;

        public BatchQueryTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            migrationRule = new MigrationTestRule(engineRule);
            helper = new BatchMigrationHelper(engineRule, migrationRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(migrationRule);
        }


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule migrationRule;
        protected internal BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal IManagementService managementService;
        protected internal IHistoryService historyService;

        [Test]
        public virtual void testBatchQuery()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);

            // when
            var list = managementService.CreateBatchQuery()
                
                .ToList();

            // then
            Assert.AreEqual(2, list.Count);

            IList<string> batchIds = new List<string>();
            foreach (var resultBatch in list)
                batchIds.Add(resultBatch.Id);

            Assert.True(batchIds.Contains(batch1.Id));
            Assert.True(batchIds.Contains(batch2.Id));
        }

        [Test]
        public virtual void testBatchQueryByActiveBatches()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);
            var batch3 = helper.MigrateProcessInstancesAsync(1);

            // when
            managementService.SuspendBatchById(batch1.Id);
            managementService.SuspendBatchById(batch2.Id);
            managementService.ActivateBatchById(batch1.Id);

            // then
            //var query = managementService.CreateBatchQuery()
            //    .Active();
            //Assert.AreEqual(2, query.Count());
            //Assert.AreEqual(2, query
            //    .Count());

            //IList<string> foundIds = new List<string>();
            //foreach (var batch in query
            //    .ToList())
            //    foundIds.Add(batch.Id);
            //Assert.That(foundIds, hasItems(batch1.Id, batch3.Id));
        }

        [Test]
        public virtual void testBatchQueryById()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var resultBatch = managementService.CreateBatchQuery(c=>c.Id== batch1.Id)
                .First();

            // then
            Assert.NotNull(resultBatch);
            Assert.AreEqual(batch1.Id, resultBatch.Id);
        }

        [Test]
        public virtual void testBatchQueryByIdNull()
        {
            try
            {
                managementService.CreateBatchQuery(c=>c.Id== null)
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NullValueException e)
            {
                Assert.That(e.Message, Does.Contain("IBatch id is null"));
            }
        }

        [Test]
        public virtual void testBatchQueryByNonExistingType()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);

            // when
            var Count = managementService.CreateBatchQuery(c=>c.Type== "foo")
                .Count();

            // then
            Assert.AreEqual(0, Count);
        }

        [Test]
        public virtual void testBatchQueryBySuspendedBatches()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            managementService.SuspendBatchById(batch1.Id);
            managementService.SuspendBatchById(batch2.Id);
            managementService.ActivateBatchById(batch1.Id);

            // then
            //var query = managementService.CreateBatchQuery()
            //    .Suspended();
            //Assert.AreEqual(1, query.Count());
            //Assert.AreEqual(1, query
            //    .Count());
            //Assert.AreEqual(batch2.Id, query.First()
            //    .Id);
        }

        [Test]
        public virtual void testBatchQueryByType()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var Count = managementService.CreateBatchQuery(c=>c.Type== batch1.Type)
                .Count();

            // then
            Assert.AreEqual(2, Count);
        }

        [Test]
        public virtual void testBatchQueryByTypeNull()
        {
            try
            {
                managementService.CreateBatchQuery(c=>c.Type ==null)
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NullValueException e)
            {
                Assert.That(e.Message, Does.Contain("Type is null"));
            }
        }

        [Test]
        public virtual void testBatchQueryCount()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var Count = managementService.CreateBatchQuery()
                .Count();

            // then
            Assert.AreEqual(2, Count);
        }

        [Test]
        public virtual void testBatchQueryOrderByIdAsc()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var orderedBatches = managementService.CreateBatchQuery()
                //.OrderById()
                /*.Asc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, batchById());
        }

        [Test]
        public virtual void testBatchQueryOrderByIdDec()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var orderedBatches = managementService.CreateBatchQuery()
                //.OrderById()
                /*.Desc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, inverted(batchById()));
        }

        [Test]
        public virtual void testBatchQueryOrderingPropertyWithoutOrder()
        {
            try
            {
                managementService.CreateBatchQuery()
                    //.OrderById()
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NotValidException e)
            {
                Assert.That(e.Message,
                    Does.Contain("Invalid query: " + "call asc() or Desc() after using orderByXX()"));
            }
        }

        [Test]
        public virtual void testBatchQueryOrderWithoutOrderingProperty()
        {
            try
            {
                managementService.CreateBatchQuery()
                    /*.Asc()*/
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NotValidException e)
            {
                Assert.That(e.Message,
                    Does.Contain("You should call any of the orderBy methods " +
                                        "first before specifying a direction"));
            }
        }

        [Test]
        public virtual void testBatchQueryResult()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            var resultBatch = managementService.CreateBatchQuery()
                .First();

            // then
            Assert.NotNull(batch);

            Assert.AreEqual(batch.Id, resultBatch.Id);
            Assert.AreEqual(batch.BatchJobDefinitionId, resultBatch.BatchJobDefinitionId);
            Assert.AreEqual(batch.MonitorJobDefinitionId, resultBatch.MonitorJobDefinitionId);
            Assert.AreEqual(batch.SeedJobDefinitionId, resultBatch.SeedJobDefinitionId);
            Assert.AreEqual(batch.TenantId, resultBatch.TenantId);
            Assert.AreEqual(batch.Type, resultBatch.Type);
            Assert.AreEqual(batch.BatchJobsPerSeed, resultBatch.BatchJobsPerSeed);
            Assert.AreEqual(batch.InvocationsPerBatchJob, resultBatch.InvocationsPerBatchJob);
            Assert.AreEqual(batch.TotalJobs, resultBatch.TotalJobs);
            Assert.AreEqual(batch.JobsCreated, resultBatch.JobsCreated);
            Assert.AreEqual(batch.Suspended, resultBatch.Suspended);
        }
    }
}