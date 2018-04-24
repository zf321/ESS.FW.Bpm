using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Batch;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    [TestFixture]
    public class BatchPriorityTest
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
        public virtual void saveAndReduceBatchConfiguration()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            defaultBatchJobPriority = configuration.BatchJobPriority;
            // reduce number of batch jobs per seed to not have to create a lot of instances
            configuration.BatchJobsPerSeed = 1;
        }

        private readonly bool InstanceFieldsInitialized;

        public BatchPriorityTest()
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


        public static readonly long CUSTOM_PRIORITY = DefaultJobPriorityProvider.DEFAULT_PRIORITY + 10;

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
        protected internal long defaultBatchJobPriority;
        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        [TearDown]
        public virtual void resetBatchJobsPerSeed()
        {
            var processEngineConfiguration = engineRule.ProcessEngineConfiguration;
            processEngineConfiguration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
            processEngineConfiguration.BatchJobPriority = defaultBatchJobPriority;
        }
        [Test]
        public virtual void seedJobShouldHaveDefaultPriority()
        {
            // when
            var batch = helper.MigrateProcessInstancesAsync(1);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.AreEqual(DefaultJobPriorityProvider.DEFAULT_PRIORITY, seedJob.Priority);
        }

        [Test]
        public virtual void monitorJobShouldHaveDefaultPriority()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.AreEqual(DefaultJobPriorityProvider.DEFAULT_PRIORITY, monitorJob.Priority);
        }

        [Test]
        public virtual void batchExecutionJobShouldHaveDefaultPriority()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var executionJob = helper.GetExecutionJobs(batch)[0];
            Assert.AreEqual(DefaultJobPriorityProvider.DEFAULT_PRIORITY, executionJob.Priority);
        }

        [Test]
        public virtual void seedJobShouldGetPriorityFromProcessEngineConfiguration()
        {
            // given
            BatchJobPriority = CUSTOM_PRIORITY;

            // when
            var batch = helper.MigrateProcessInstancesAsync(1);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, seedJob.Priority);
        }

        [Test]
        public virtual void monitorJobShouldGetPriorityFromProcessEngineConfiguration()
        {
            // given
            BatchJobPriority = CUSTOM_PRIORITY;
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, monitorJob.Priority);
        }

        [Test]
        public virtual void executionJobShouldGetPriorityFromProcessEngineConfiguration()
        {
            // given
            BatchJobPriority = CUSTOM_PRIORITY;
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var executionJob = helper.GetExecutionJobs(batch)[0];
            Assert.AreEqual(CUSTOM_PRIORITY, executionJob.Priority);
        }

        [Test]
        public virtual void seedJobShouldGetPriorityFromOverridingJobDefinitionPriority()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(2);
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            managementService.SetOverridingJobPriorityForJobDefinition(seedJobDefinition.Id, CUSTOM_PRIORITY);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, seedJob.Priority);
        }

        [Test]
        public virtual void seedJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);

            // when
            managementService.SetOverridingJobPriorityForJobDefinition(seedJobDefinition.Id, CUSTOM_PRIORITY, true);

            // then
            var seedJob = helper.GetSeedJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, seedJob.Priority);
        }

        [Test]
        public virtual void monitorJobShouldGetPriorityOverridingJobDefinitionPriority()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            var monitorJobDefinition = helper.GetMonitorJobDefinition(batch);
            managementService.SetOverridingJobPriorityForJobDefinition(monitorJobDefinition.Id, CUSTOM_PRIORITY);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, monitorJob.Priority);
        }

        [Test]
        public virtual void monitorJobShouldGetPriorityOverridingJobDefinitionPriorityWithCascade()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            var monitorJobDefinition = helper.GetMonitorJobDefinition(batch);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.SetOverridingJobPriorityForJobDefinition(monitorJobDefinition.Id, CUSTOM_PRIORITY, true);

            // then
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.AreEqual(CUSTOM_PRIORITY, monitorJob.Priority);
        }

        [Test]
        public virtual void executionJobShouldGetPriorityFromOverridingJobDefinitionPriority()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            var executionJobDefinition = helper.GetExecutionJobDefinition(batch);
            managementService.SetOverridingJobPriorityForJobDefinition(executionJobDefinition.Id, CUSTOM_PRIORITY, true);

            // when
            helper.ExecuteSeedJob(batch);

            // then
            var executionJob = helper.GetExecutionJobs(batch)[0];
            Assert.AreEqual(CUSTOM_PRIORITY, executionJob.Priority);
        }

        [Test]
        public virtual void executionJobShouldGetPriorityFromOverridingJobDefinitionPriorityWithCascade()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);
            var executionJobDefinition = helper.GetExecutionJobDefinition(batch);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.SetOverridingJobPriorityForJobDefinition(executionJobDefinition.Id, CUSTOM_PRIORITY, true);

            // then
            var executionJob = helper.GetExecutionJobs(batch)[0];
            Assert.AreEqual(CUSTOM_PRIORITY, executionJob.Priority);
        }

        protected internal virtual long BatchJobPriority
        {
            set { engineRule.ProcessEngineConfiguration.BatchJobPriority = value; }
        }
    }
}