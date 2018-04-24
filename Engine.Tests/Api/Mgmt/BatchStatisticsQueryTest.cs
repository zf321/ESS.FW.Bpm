using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.exception;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class BatchStatisticsQueryTest
    {
        [SetUp]
        public virtual void initServices()
        {
            managementService = engineRule.ManagementService;
        }

        [SetUp]
        public virtual void saveAndReduceBatchJobsPerSeed()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            // reduce number of batch jobs per seed to not have to create a lot of instances
            configuration.BatchJobsPerSeed = 10;
        }

        private readonly bool InstanceFieldsInitialized;

        public BatchStatisticsQueryTest()
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

        protected internal IManagementService managementService;
        protected internal int defaultBatchJobsPerSeed;
        [TearDown]
        public virtual void resetBatchJobsPerSeed()
        {
            engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }
        [Test]
        public virtual void testQuery()
        {
            var statistics = managementService.CreateBatchStatisticsQuery()
                
                .ToList();
            Assert.AreEqual(0, statistics.Count);

            var batch1 = helper.CreateMigrationBatchWithSize(1);

            statistics = managementService.CreateBatchStatisticsQuery()
                
                .ToList();
            Assert.AreEqual(1, statistics.Count);
            Assert.AreEqual(batch1.Id, statistics[0].Id);

            var batch2 = helper.CreateMigrationBatchWithSize(1);
            var batch3 = helper.CreateMigrationBatchWithSize(1);

            statistics = managementService.CreateBatchStatisticsQuery()
                
                .ToList();
            Assert.AreEqual(3, statistics.Count);

            helper.CompleteBatch(batch1);
            helper.CompleteBatch(batch3);

            statistics = managementService.CreateBatchStatisticsQuery()
                
                .ToList();
            Assert.AreEqual(1, statistics.Count);
            Assert.AreEqual(batch2.Id, statistics[0].Id);

            helper.CompleteBatch(batch2);

            statistics = managementService.CreateBatchStatisticsQuery()
                
                .ToList();
            Assert.AreEqual(0, statistics.Count);
        }

        [Test]
        public virtual void testQueryCount()
        {
            var Count = managementService.CreateBatchStatisticsQuery()
                .Count();
            Assert.AreEqual(0, Count);

            var batch1 = helper.CreateMigrationBatchWithSize(1);

            Count = managementService.CreateBatchStatisticsQuery()
                .Count();
            Assert.AreEqual(1, Count);

            var batch2 = helper.CreateMigrationBatchWithSize(1);
            var batch3 = helper.CreateMigrationBatchWithSize(1);

            Count = managementService.CreateBatchStatisticsQuery()
                .Count();
            Assert.AreEqual(3, Count);

            helper.CompleteBatch(batch1);
            helper.CompleteBatch(batch3);

            Count = managementService.CreateBatchStatisticsQuery()
                .Count();
            Assert.AreEqual(1, Count);

            helper.CompleteBatch(batch2);

            Count = managementService.CreateBatchStatisticsQuery()
                .Count();
            Assert.AreEqual(0, Count);
        }

        [Test]
        public virtual void testQueryById()
        {
            // given
            helper.CreateMigrationBatchWithSize(1);
            var batch = helper.CreateMigrationBatchWithSize(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery(c=>c.Id== batch.Id)
                .First();

            // then
            Assert.AreEqual(batch.Id, statistics.Id);
        }

        [Test]
        public virtual void testQueryByNullId()
        {
            try
            {
                managementService.CreateBatchStatisticsQuery(c=>c.Id== null)
                    .First();
                Assert.Fail("exception expected");
            }
            catch (NullValueException e)
            {
                Assert.That(e.Message, Does.Contain("IBatch id is null"));
            }
        }

        [Test]
        public virtual void testQueryByUnknownId()
        {
            // given
            helper.CreateMigrationBatchWithSize(1);
            helper.CreateMigrationBatchWithSize(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery(c=>c.Id == "unknown")
                
                .ToList();

            // then
            Assert.AreEqual(0, statistics.Count);
        }

        [Test]
        public virtual void testQueryByType()
        {
            // given
            helper.CreateMigrationBatchWithSize(1);
            helper.CreateMigrationBatchWithSize(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery(c=>c.Type == BatchFields.TypeProcessInstanceMigration)
                
                .ToList();

            // then
            Assert.AreEqual(2, statistics.Count);
        }

        [Test]
        public virtual void testQueryByNullType()
        {
            try
            {
                managementService.CreateBatchStatisticsQuery(c=>c.Type ==null)
                    
                    .ToList();
                Assert.Fail("exception expected");
            }
            catch (NullValueException e)
            {
                Assert.That(e.Message, Does.Contain("Type is null"));
            }
        }

        [Test]
        public virtual void testQueryByUnknownType()
        {
            // given
            helper.CreateMigrationBatchWithSize(1);
            helper.CreateMigrationBatchWithSize(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery(c=>c.Type=="unknown")
                
                .ToList();

            // then
            Assert.AreEqual(0, statistics.Count);
        }

        [Test]
        public virtual void testQueryOrderByIdAsc()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery()
                ////.OrderById()
                /*.Asc()*/
                
                .ToList();

            // then
            //verifySorting(statistics, batchStatisticsById());
        }

        [Test]
        public virtual void testQueryOrderByIdDec()
        {
            // given
            helper.MigrateProcessInstancesAsync(1);
            helper.MigrateProcessInstancesAsync(1);

            // when
            var statistics = managementService.CreateBatchStatisticsQuery()
                ////.OrderById()
                /*.Desc()*/
                
                .ToList();

            // then
            //verifySorting(statistics, inverted(batchStatisticsById()));
        }

        [Test]
        public virtual void testQueryOrderingPropertyWithoutOrder()
        {
            try
            {
                managementService.CreateBatchStatisticsQuery()
                    ////.OrderById()
                    
                    .ToList();
                Assert.Fail("exception expected");
            }
            catch (NotValidException e)
            {
                Assert.That(e.Message,
                    Does.Contain("Invalid query: " + "call asc() or Desc() after using orderByXX()"));
            }
        }

        [Test]
        public virtual void testQueryOrderWithoutOrderingProperty()
        {
            try
            {
                managementService.CreateBatchStatisticsQuery()
                    /*.Asc()*/
                    
                    .ToList();
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
        public virtual void testStatisticsNoExecutionJobsGenerated()
        {
            // given
            helper.CreateMigrationBatchWithSize(3);

            // when
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            // then
            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(0, batchStatistics.JobsCreated);
            Assert.AreEqual(3, batchStatistics.RemainingJobs);
            Assert.AreEqual(0, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsMostExecutionJobsGenerated()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(13);

            // when
            helper.ExecuteJob(helper.GetSeedJob(batch));

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(13, batchStatistics.TotalJobs);
            Assert.AreEqual(10, batchStatistics.JobsCreated);
            Assert.AreEqual(13, batchStatistics.RemainingJobs);
            Assert.AreEqual(0, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsAllExecutionJobsGenerated()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(3);

            // when
            helper.CompleteSeedJobs(batch);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(3, batchStatistics.RemainingJobs);
            Assert.AreEqual(0, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsOneCompletedJob()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(3);

            // when
            helper.CompleteSeedJobs(batch);
            helper.CompleteJobs(batch, 1);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(2, batchStatistics.RemainingJobs);
            Assert.AreEqual(1, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsOneFailedJob()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(3);

            // when
            helper.CompleteSeedJobs(batch);
            helper.FailExecutionJobs(batch, 1);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(3, batchStatistics.RemainingJobs);
            Assert.AreEqual(0, batchStatistics.CompletedJobs);
            Assert.AreEqual(1, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsOneCompletedAndOneFailedJob()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(3);

            // when
            helper.CompleteSeedJobs(batch);
            helper.CompleteJobs(batch, 1);
            helper.FailExecutionJobs(batch, 1);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(2, batchStatistics.RemainingJobs);
            Assert.AreEqual(1, batchStatistics.CompletedJobs);
            Assert.AreEqual(1, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsRetriedFailedJobs()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(3);

            // when
            helper.CompleteSeedJobs(batch);
            helper.FailExecutionJobs(batch, 3);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(3, batchStatistics.RemainingJobs);
            Assert.AreEqual(0, batchStatistics.CompletedJobs);
            Assert.AreEqual(3, batchStatistics.FailedJobs);

            // when
            helper.SetRetries(batch, 3, 1);
            helper.CompleteJobs(batch, 3);

            // then
            batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(3, batchStatistics.TotalJobs);
            Assert.AreEqual(3, batchStatistics.JobsCreated);
            Assert.AreEqual(0, batchStatistics.RemainingJobs);
            Assert.AreEqual(3, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsWithDeletedJobs()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(13);

            // when
            helper.ExecuteJob(helper.GetSeedJob(batch));
            deleteMigrationJobs(batch);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(13, batchStatistics.TotalJobs);
            Assert.AreEqual(10, batchStatistics.JobsCreated);
            Assert.AreEqual(3, batchStatistics.RemainingJobs);
            Assert.AreEqual(10, batchStatistics.CompletedJobs);
            Assert.AreEqual(0, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testStatisticsWithNotAllGeneratedAndAlreadyCompletedAndFailedJobs()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(13);

            // when
            helper.ExecuteJob(helper.GetSeedJob(batch));
            helper.CompleteJobs(batch, 2);
            helper.FailExecutionJobs(batch, 2);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery()
                .First();

            Assert.AreEqual(13, batchStatistics.TotalJobs);
            Assert.AreEqual(10, batchStatistics.JobsCreated);
            Assert.AreEqual(11, batchStatistics.RemainingJobs);
            Assert.AreEqual(2, batchStatistics.CompletedJobs);
            Assert.AreEqual(2, batchStatistics.FailedJobs);
        }

        [Test]
        public virtual void testMultipleBatchesStatistics()
        {
            // given
            var batch1 = helper.CreateMigrationBatchWithSize(3);
            var batch2 = helper.CreateMigrationBatchWithSize(13);
            var batch3 = helper.CreateMigrationBatchWithSize(15);

            // when
            helper.ExecuteJob(helper.GetSeedJob(batch2));
            helper.CompleteJobs(batch2, 2);
            helper.FailExecutionJobs(batch2, 3);

            helper.ExecuteJob(helper.GetSeedJob(batch3));
            deleteMigrationJobs(batch3);
            helper.ExecuteJob(helper.GetSeedJob(batch3));
            helper.CompleteJobs(batch3, 2);
            helper.FailExecutionJobs(batch3, 3);

            // then
            var batchStatisticsList = managementService.CreateBatchStatisticsQuery()
                
                .ToList();

            foreach (var batchStatistics in batchStatisticsList)
                if (batch1.Id.Equals(batchStatistics.Id))
                {
                    // batch 1
                    Assert.AreEqual(3, batchStatistics.TotalJobs);
                    Assert.AreEqual(0, batchStatistics.JobsCreated);
                    Assert.AreEqual(3, batchStatistics.RemainingJobs);
                    Assert.AreEqual(0, batchStatistics.CompletedJobs);
                    Assert.AreEqual(0, batchStatistics.FailedJobs);
                }
                else if (batch2.Id.Equals(batchStatistics.Id))
                {
                    // batch 2
                    Assert.AreEqual(13, batchStatistics.TotalJobs);
                    Assert.AreEqual(10, batchStatistics.JobsCreated);
                    Assert.AreEqual(11, batchStatistics.RemainingJobs);
                    Assert.AreEqual(2, batchStatistics.CompletedJobs);
                    Assert.AreEqual(3, batchStatistics.FailedJobs);
                }
                else if (batch3.Id.Equals(batchStatistics.Id))
                {
                    // batch 3
                    Assert.AreEqual(15, batchStatistics.TotalJobs);
                    Assert.AreEqual(15, batchStatistics.JobsCreated);
                    Assert.AreEqual(3, batchStatistics.RemainingJobs);
                    Assert.AreEqual(12, batchStatistics.CompletedJobs);
                    Assert.AreEqual(3, batchStatistics.FailedJobs);
                }
        }

        [Test]
        public virtual void testStatisticsSuspend()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            managementService.SuspendBatchById(batch.Id);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery(c=>c.Id== batch.Id)
                .First();

            Assert.True(batchStatistics.Suspended);
        }

        [Test]
        public virtual void testStatisticsActivate()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            managementService.ActivateBatchById(batch.Id);

            // then
            var batchStatistics = managementService.CreateBatchStatisticsQuery(c=>c.Id== batch.Id)
                .First();

            Assert.IsFalse(batchStatistics.Suspended);
        }

        [Test]
        public virtual void testStatisticsQueryBySuspendedBatches()
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
            //var query = managementService.CreateBatchStatisticsQuery()
            //    .Suspended();
            //Assert.AreEqual(1, query.Count());
            //Assert.AreEqual(1, query
            ////    .Count());
            //Assert.AreEqual(batch2.Id, query.First()
            //    .Id);
        }

        [Test]
        public virtual void testStatisticsQueryByActiveBatches()
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
            var query = managementService.CreateBatchStatisticsQuery()
                //.Active()
                ;
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query
                .Count());

            IList<string> foundIds = new List<string>();
            foreach (IBatch batch in query
                .ToList())
                foundIds.Add(batch.Id);
            //Assert.That(foundIds, hasItems(batch1.Id, batch3.Id));
        }

        protected internal virtual void deleteMigrationJobs(IBatch batch)
        {
            foreach (var migrationJob in helper.GetExecutionJobs(batch))
                managementService.DeleteJob(migrationJob.Id);
        }
    }
}