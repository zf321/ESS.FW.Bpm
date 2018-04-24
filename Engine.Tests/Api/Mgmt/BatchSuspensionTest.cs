using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class BatchSuspensionTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
            identityService = engineRule.IdentityService;
        }

        [SetUp]
        public virtual void saveAndReduceBatchJobsPerSeed()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            // reduce number of batch jobs per seed to not have to create a lot of instances
            configuration.BatchJobsPerSeed = 1;
        }

        private readonly bool InstanceFieldsInitialized;

        public BatchSuspensionTest()
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


        public const string USER_ID = "userId";

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule migrationRule;
        protected internal BatchMigrationHelper helper;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal IManagementService managementService;
        protected internal IHistoryService historyService;
        protected internal IIdentityService identityService;

        protected internal int defaultBatchJobsPerSeed;
        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }
        [TearDown]
        public virtual void resetBatchJobsPerSeed()
        {
            engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
        }

        [Test]
        public virtual void shouldSuspendBatch()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            managementService.SuspendBatchById(batch.Id);

            // then
            batch = managementService.CreateBatchQuery()
               // .SetBatchId(batch.Id)
                .First();
            Assert.True(batch.Suspended);
        }

        [Test]
        public virtual void shouldFailWhenSuspendingUsingUnknownId()
        {
            try
            {
                managementService.SuspendBatchById("unknown");
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("IBatch for id 'unknown' cannot be found"));
            }
        }

        [Test]
        public virtual void shouldFailWhenSuspendingUsingNullId()
        {
            try
            {
                managementService.SuspendBatchById(null);
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("batch id is null"));
            }
        }

        [Test]
        public virtual void shouldSuspendSeedJobAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            managementService.SuspendBatchById(batch.Id);

            // then
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            Assert.True(seedJobDefinition.Suspended);

            var seedJob = helper.GetSeedJob(batch);
            Assert.True(seedJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateSuspendedSeedJob()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(2);
            managementService.SuspendBatchById(batch.Id);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.True(seedJob.Suspended);
        }

        [Test]
        public virtual void shouldSuspendMonitorJobAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.SuspendBatchById(batch.Id);

            // then
            var monitorJobDefinition = helper.GetMonitorJobDefinition(batch);
            Assert.True(monitorJobDefinition.Suspended);

            var monitorJob = helper.GetMonitorJob(batch);
            Assert.True(monitorJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateSuspendedMonitorJob()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.True(monitorJob.Suspended);
        }

        [Test]
        public virtual void shouldSuspendExecutionJobsAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.SuspendBatchById(batch.Id);

            // then
            var migrationJobDefinition = helper.GetExecutionJobDefinition(batch);
            Assert.True(migrationJobDefinition.Suspended);

            var migrationJob = helper.GetExecutionJobs(batch)[0];
            Assert.True(migrationJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateSuspendedExecutionJobs()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var migrationJob = helper.GetExecutionJobs(batch)[0];
            Assert.True(migrationJob.Suspended);
        }

        [Test][RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
        public virtual void shouldCreateUserOperationLogForBatchSuspension()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            identityService.AuthenticatedUserId = USER_ID;
            managementService.SuspendBatchById(batch.Id);
            identityService.ClearAuthentication();

            // then
            var entry = historyService.CreateUserOperationLogQuery()
                .First();

            Assert.NotNull(entry);
            Assert.AreEqual(batch.Id, entry.BatchId);
            Assert.AreEqual(AbstractSetBatchStateCmd.SuspensionStateProperty, entry.Property);
            Assert.IsNull(entry.OrgValue);
            Assert.AreEqual(SuspensionStateFields.Suspended.Name, entry.NewValue);
        }

        [Test]
        public virtual void shouldActivateBatch()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            managementService.ActivateBatchById(batch.Id);

            // then
            batch = managementService.CreateBatchQuery()
               // .SetBatchId(batch.Id)
                .First();
            Assert.IsFalse(batch.Suspended);
        }

        [Test]
        public virtual void shouldFailWhenActivatingUsingUnknownId()
        {
            try
            {
                managementService.ActivateBatchById("unknown");
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("IBatch for id 'unknown' cannot be found"));
            }
        }

        [Test]
        public virtual void shouldFailWhenActivatingUsingNullId()
        {
            try
            {
                managementService.ActivateBatchById(null);
                Assert.Fail("Exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("batch id is null"));
            }
        }

        [Test]
        public virtual void shouldActivateSeedJobAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            managementService.ActivateBatchById(batch.Id);

            // then
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            Assert.IsFalse(seedJobDefinition.Suspended);

            var seedJob = helper.GetSeedJob(batch);
            Assert.IsFalse(seedJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateActivatedSeedJob()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(2);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.IsFalse(seedJob.Suspended);
        }

        [Test]
        public virtual void shouldActivateMonitorJobAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.ActivateBatchById(batch.Id);

            // then
            var monitorJobDefinition = helper.GetMonitorJobDefinition(batch);
            Assert.IsFalse(monitorJobDefinition.Suspended);

            var monitorJob = helper.GetMonitorJob(batch);
            Assert.IsFalse(monitorJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateActivatedMonitorJob()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.IsFalse(monitorJob.Suspended);
        }

        [Test]
        public virtual void shouldActivateExecutionJobsAndDefinition()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.ActivateBatchById(batch.Id);

            // then
            var migrationJobDefinition = helper.GetExecutionJobDefinition(batch);
            Assert.IsFalse(migrationJobDefinition.Suspended);

            var migrationJob = helper.GetExecutionJobs(batch)[0];
            Assert.IsFalse(migrationJob.Suspended);
        }

        [Test]
        public virtual void shouldCreateActivatedExecutionJobs()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var migrationJob = helper.GetExecutionJobs(batch)[0];
            Assert.IsFalse(migrationJob.Suspended);
        }

        [Test][RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void shouldCreateUserOperationLogForBatchActivation()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            managementService.SuspendBatchById(batch.Id);

            // when
            identityService.AuthenticatedUserId = USER_ID;
            managementService.ActivateBatchById(batch.Id);
            identityService.ClearAuthentication();

            // then
            var entry = historyService.CreateUserOperationLogQuery()
                .First();

            Assert.NotNull(entry);
            Assert.AreEqual(batch.Id, entry.BatchId);
            Assert.AreEqual(AbstractSetBatchStateCmd.SuspensionStateProperty, entry.Property);
            Assert.IsNull(entry.OrgValue);
            Assert.AreEqual(SuspensionStateFields.Active.Name, entry.NewValue);
        }

        [Test][RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
        public virtual void testUserOperationLogQueryByBatchEntityType()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);

            // when
            identityService.AuthenticatedUserId = USER_ID;
            managementService.SuspendBatchById(batch1.Id);
            managementService.SuspendBatchById(batch2.Id);
            managementService.ActivateBatchById(batch1.Id);
            identityService.ClearAuthentication();

            // then
            var query = historyService.CreateUserOperationLogQuery();
                //.EntityType(EntityTypes.Batch);
            Assert.AreEqual(3, query.Count());
            Assert.AreEqual(3, query
                .Count());
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void testUserOperationLogQueryByBatchId()
        {
            // given
            var batch1 = helper.MigrateProcessInstancesAsync(1);
            var batch2 = helper.MigrateProcessInstancesAsync(1);

            // when
            identityService.AuthenticatedUserId = USER_ID;
            managementService.SuspendBatchById(batch1.Id);
            managementService.SuspendBatchById(batch2.Id);
            managementService.ActivateBatchById(batch1.Id);
            identityService.ClearAuthentication();

            // then
            var query = historyService.CreateUserOperationLogQuery()
                ;//.BatchId(batch1.Id);
            Assert.AreEqual(2, query.Count());
            Assert.AreEqual(2, query
                .Count());

            query = historyService.CreateUserOperationLogQuery()
                ;//.BatchId(batch2.Id);
            Assert.AreEqual(1, query.Count());
            Assert.AreEqual(1, query
                .Count());
        }
    }
}