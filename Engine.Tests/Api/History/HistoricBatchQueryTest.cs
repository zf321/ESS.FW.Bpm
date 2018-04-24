using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.History
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]

    [TestFixture]
    public class HistoricBatchQueryTest
    {
        private readonly bool InstanceFieldsInitialized;

        public HistoricBatchQueryTest()
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

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [Test]
        public virtual void testBatchQuery()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);

            // when
            var list = historyService.CreateHistoricBatchQuery()
                
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
        public virtual void testBatchQueryResult()
        {
            var startDate = new DateTime(10000L);
            var endDate = new DateTime(40000L);

            // given
            ClockUtil.CurrentTime = startDate;
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);

            ClockUtil.CurrentTime = endDate;
            helper.ExecuteMonitorJob(batch);

            // when
            var resultBatch = historyService.CreateHistoricBatchQuery()
                .First();

            // then
            Assert.NotNull(resultBatch);

            Assert.AreEqual(batch.Id, resultBatch.Id);
            Assert.AreEqual(batch.BatchJobDefinitionId, resultBatch.BatchJobDefinitionId);
            Assert.AreEqual(batch.MonitorJobDefinitionId, resultBatch.MonitorJobDefinitionId);
            Assert.AreEqual(batch.SeedJobDefinitionId, resultBatch.SeedJobDefinitionId);
            Assert.AreEqual(batch.TenantId, resultBatch.TenantId);
            Assert.AreEqual(batch.Type, resultBatch.Type);
            Assert.AreEqual(batch.BatchJobsPerSeed, resultBatch.BatchJobsPerSeed);
            Assert.AreEqual(batch.InvocationsPerBatchJob, resultBatch.InvocationsPerBatchJob);
            Assert.AreEqual(batch.TotalJobs, resultBatch.TotalJobs);
            Assert.AreEqual(startDate, resultBatch.StartTime);
            Assert.AreEqual(endDate, resultBatch.EndTime);
        }

        [Test]
        public virtual void testBatchQueryById()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var resultBatch = historyService.CreateHistoricBatchQuery()
                //.BatchId(batch1.Id)
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
                historyService.CreateHistoricBatchQuery()
                    //.BatchId(null)
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NullValueException e)
            {
                Assert.That(e.Message, Does.Contain("IBatch id is null"));
            }
        }

        [Test]
        public virtual void testBatchQueryByType()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var Count = historyService.CreateHistoricBatchQuery()
                //.Type(batch1.Type)
                .Count();

            // then
            Assert.AreEqual(2, Count);
        }

        [Test]
        public virtual void testBatchQueryByNonExistingType()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);

            // when
            var Count = historyService.CreateHistoricBatchQuery()
                //.Type("foo")
                .Count();

            // then
            Assert.AreEqual(0, Count);
        }

        [Test]
        public virtual void testBatchByState()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);

            helper.CompleteBatch(batch1);

            // when
            var historicBatch = historyService.CreateHistoricBatchQuery()
               // .Completed(true)
                .First();

            // then
            Assert.AreEqual(batch1.Id, historicBatch.Id);

            // when
            historicBatch = historyService.CreateHistoricBatchQuery()
                //.Completed(false)
                .First();

            // then
            Assert.AreEqual(batch2.Id, historicBatch.Id);
        }

        [Test]
        public virtual void testBatchQueryByTypeNull()
        {
            try
            {
                historyService.CreateHistoricBatchQuery()
                    //.Type(null)
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
            var Count = historyService.CreateHistoricBatchQuery()
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
            var orderedBatches = historyService.CreateHistoricBatchQuery()
                //.OrderById()
                /*.Asc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, historicBatchById());
        }

        [Test]
        public virtual void testBatchQueryOrderByIdDec()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var orderedBatches = historyService.CreateHistoricBatchQuery()
                //.OrderById()
                /*.Desc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, inverted(historicBatchById()));
        }

        [Test]
        public virtual void testBatchQueryOrderByStartTimeAsc()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();
            helper.MigrateProcessInstancesAsync(1);
            ClockTestUtil.IncrementClock(1000);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var orderedBatches = historyService.CreateHistoricBatchQuery()
               // .OrderByStartTime()
                /*.Asc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, historicBatchByStartTime());
        }

        [Test]
        public virtual void testBatchQueryOrderByStartTimeDec()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();
            helper.MigrateProcessInstancesAsync(1);
            ClockTestUtil.IncrementClock(1000);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var orderedBatches = historyService.CreateHistoricBatchQuery()
               // .OrderByStartTime()
                /*.Desc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, inverted(historicBatchByStartTime()));
        }

        [Test]
        public virtual void testBatchQueryOrderByEndTimeAsc()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.CompleteBatch(batch1);

            ClockTestUtil.IncrementClock(1000);
            var batch2 = helper.MigrateProcessInstancesAsync(1);
            helper.CompleteBatch(batch2);

            // when
            var orderedBatches = historyService.CreateHistoricBatchQuery()
                //.OrderByEndTime()
                /*.Asc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, historicBatchByEndTime());
        }

        [Test]
        public virtual void testBatchQueryOrderByEndTimeDec()
        {
            // given
            ClockTestUtil.SetClockToDateWithoutMilliseconds();
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            helper.CompleteBatch(batch1);

            ClockTestUtil.IncrementClock(1000);
            var batch2 = helper.MigrateProcessInstancesAsync(1);
            helper.CompleteBatch(batch2);

            // when
            var orderedBatches = historyService.CreateHistoricBatchQuery()
                //.OrderByEndTime()
                /*.Desc()*/
                
                .ToList();

            // then
            //verifySorting(orderedBatches, inverted(historicBatchByEndTime()));
        }

        [Test]
        public virtual void testBatchQueryOrderingPropertyWithoutOrder()
        {
            try
            {
                historyService.CreateHistoricBatchQuery()
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
                historyService.CreateHistoricBatchQuery()
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
    }
}