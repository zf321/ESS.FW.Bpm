using System;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.Batch
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BatchMigrationHistoryTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            managementService = engineRule.ManagementService;
            historyService = engineRule.HistoryService;
        }

        [SetUp]
        public virtual void setClock()
        {
            ClockUtil.CurrentTime = START_DATE;
        }

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule);
        ////public RuleChain ruleChain;

        protected internal ProcessEngineConfigurationImpl configuration;

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal BatchMigrationHelper helper;
        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        protected internal IManagementService managementService;
        protected internal MigrationTestRule migrationRule;
        protected internal IRuntimeService runtimeService;

        protected internal IProcessDefinition sourceProcessDefinition;
        protected internal IProcessDefinition targetProcessDefinition;

        public BatchMigrationHistoryTest()
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
        [Test]
        public virtual void testHistoricBatchCreation()
        {
            // when
            var batch = helper.MigrateProcessInstancesAsync(10);

            // then a historic batch was created
            var historicBatch = helper.GetHistoricBatch(batch);
            Assert.NotNull(historicBatch);
            Assert.AreEqual(batch.Id, historicBatch.Id);
            Assert.AreEqual(batch.Type, historicBatch.Type);
            Assert.AreEqual(batch.TotalJobs, historicBatch.TotalJobs);
            Assert.AreEqual(batch.BatchJobsPerSeed, historicBatch.BatchJobsPerSeed);
            Assert.AreEqual(batch.InvocationsPerBatchJob, historicBatch.InvocationsPerBatchJob);
            Assert.AreEqual(batch.SeedJobDefinitionId, historicBatch.SeedJobDefinitionId);
            Assert.AreEqual(batch.MonitorJobDefinitionId, historicBatch.MonitorJobDefinitionId);
            Assert.AreEqual(batch.BatchJobDefinitionId, historicBatch.BatchJobDefinitionId);
            Assert.AreEqual(START_DATE, historicBatch.StartTime);
            Assert.IsNull(historicBatch.EndTime);
        }

        [Test]
        public virtual void testHistoricBatchCompletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);

            var endDate = helper.AddSecondsToClock(12);

            // when
            helper.ExecuteMonitorJob(batch);

            // then the historic batch has an end time set
            var historicBatch = helper.GetHistoricBatch(batch);
            Assert.NotNull(historicBatch);
            Assert.AreEqual(endDate, historicBatch.EndTime);
        }

        [Test]
        public virtual void testHistoricSeedJobLog()
        {
            // when
            var batch = helper.MigrateProcessInstancesAsync(1);

            // then a historic job log exists for the seed job
            var jobLog = helper.GetHistoricSeedJobLog(batch)[0];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.CreationLog);
            Assert.AreEqual(batch.SeedJobDefinitionId, jobLog.JobDefinitionId);
            Assert.AreEqual(BatchSeedJobHandler.TYPE, jobLog.JobDefinitionType);
            Assert.AreEqual(batch.Id, jobLog.JobDefinitionConfiguration);
            Assert.AreEqual(START_DATE, jobLog.TimeStamp);
            Assert.IsNull(jobLog.DeploymentId);
            Assert.IsNull(jobLog.ProcessDefinitionId);
            Assert.IsNull(jobLog.ExecutionId);
            Assert.IsNull(jobLog.JobDueDate);

            // when the seed job is executed
            var executionDate = helper.AddSecondsToClock(12);
            helper.ExecuteSeedJob(batch);

            // then a new historic job log exists for the seed job
            jobLog = helper.GetHistoricSeedJobLog(batch)[1];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.SuccessLog);
            Assert.AreEqual(batch.SeedJobDefinitionId, jobLog.JobDefinitionId);
            Assert.AreEqual(BatchSeedJobHandler.TYPE, jobLog.JobDefinitionType);
            Assert.AreEqual(batch.Id, jobLog.JobDefinitionConfiguration);
            Assert.AreEqual(executionDate, jobLog.TimeStamp);
            Assert.IsNull(jobLog.DeploymentId);
            Assert.IsNull(jobLog.ProcessDefinitionId);
            Assert.IsNull(jobLog.ExecutionId);
            Assert.IsNull(jobLog.JobDueDate);
        }

        [Test]
        public virtual void testHistoricMonitorJobLog()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when the seed job is executed
            helper.ExecuteSeedJob(batch);

            var monitorJob = helper.GetMonitorJob(batch);
            var jobLogs = helper.GetHistoricMonitorJobLog(batch, monitorJob);
            Assert.AreEqual(1, jobLogs.Count);

            // then a creation historic job log exists for the monitor job without due date
            var jobLog = jobLogs[0];
            AssertCommonMonitorJobLogProperties(batch, jobLog);
            Assert.True(jobLog.CreationLog);
            Assert.AreEqual(START_DATE, jobLog.TimeStamp);
            Assert.IsNull(jobLog.JobDueDate);

            // when the monitor job is executed
            var executionDate = helper.AddSecondsToClock(15);
            var monitorJobDueDate = helper.AddSeconds(executionDate, 30);
            helper.ExecuteMonitorJob(batch);

            jobLogs = helper.GetHistoricMonitorJobLog(batch, monitorJob);
            Assert.AreEqual(2, jobLogs.Count);

            // then a success job log was created for the last monitor job
            jobLog = jobLogs[1];
            AssertCommonMonitorJobLogProperties(batch, jobLog);
            Assert.True(jobLog.SuccessLog);
            Assert.AreEqual(executionDate, jobLog.TimeStamp);
            Assert.IsNull(jobLog.JobDueDate);

            // and a creation job log for the new monitor job was created with due date
            monitorJob = helper.GetMonitorJob(batch);
            jobLogs = helper.GetHistoricMonitorJobLog(batch, monitorJob);
            Assert.AreEqual(1, jobLogs.Count);

            jobLog = jobLogs[0];
            AssertCommonMonitorJobLogProperties(batch, jobLog);
            Assert.True(jobLog.CreationLog);
            Assert.AreEqual(executionDate, jobLog.TimeStamp);
            Assert.AreEqual(monitorJobDueDate, jobLog.JobDueDate);

            // when the migration and monitor jobs are executed
            executionDate = helper.AddSecondsToClock(15);
            helper.ExecuteJobs(batch);
            helper.ExecuteMonitorJob(batch);

            jobLogs = helper.GetHistoricMonitorJobLog(batch, monitorJob);
            Assert.AreEqual(2, jobLogs.Count);

            // then a success job log was created for the last monitor job
            jobLog = jobLogs[1];
            AssertCommonMonitorJobLogProperties(batch, jobLog);
            Assert.True(jobLog.SuccessLog);
            Assert.AreEqual(executionDate, jobLog.TimeStamp);
            Assert.AreEqual(monitorJobDueDate, jobLog.JobDueDate);
        }

        [Test]
        public virtual void testHistoricBatchJobLog()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);

            var sourceDeploymentId = helper.SourceProcessDefinition.DeploymentId;

            // when
            var executionDate = helper.AddSecondsToClock(12);
            helper.ExecuteJobs(batch);

            // then a historic job log exists for the batch job
            var jobLog = helper.GetHistoricBatchJobLog(batch)[0];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.CreationLog);
            Assert.AreEqual(batch.BatchJobDefinitionId, jobLog.JobDefinitionId);
            Assert.AreEqual(BatchFields.TypeProcessInstanceMigration, jobLog.JobDefinitionType);
            Assert.AreEqual(batch.Id, jobLog.JobDefinitionConfiguration);
            Assert.AreEqual(START_DATE, jobLog.TimeStamp);
            Assert.AreEqual(sourceDeploymentId, jobLog.DeploymentId);
            Assert.IsNull(jobLog.ProcessDefinitionId);
            Assert.IsNull(jobLog.ExecutionId);
            Assert.IsNull(jobLog.JobDueDate);

            jobLog = helper.GetHistoricBatchJobLog(batch)[1];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.SuccessLog);
            Assert.AreEqual(batch.BatchJobDefinitionId, jobLog.JobDefinitionId);
            Assert.AreEqual(BatchFields.TypeProcessInstanceMigration, jobLog.JobDefinitionType);
            Assert.AreEqual(batch.Id, jobLog.JobDefinitionConfiguration);
            Assert.AreEqual(executionDate, jobLog.TimeStamp);
            Assert.AreEqual(sourceDeploymentId, jobLog.DeploymentId);
            Assert.IsNull(jobLog.ProcessDefinitionId);
            Assert.IsNull(jobLog.ExecutionId);
            Assert.IsNull(jobLog.JobDueDate);
        }

        [Test]
        public virtual void testHistoricBatchForBatchDeletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            var deletionDate = helper.AddSecondsToClock(12);
            managementService.DeleteBatch(batch.Id, false);

            // then the end time was set for the historic batch
            var historicBatch = helper.GetHistoricBatch(batch);
            Assert.NotNull(historicBatch);
            Assert.AreEqual(deletionDate, historicBatch.EndTime);
        }

        [Test]
        public virtual void testHistoricSeedJobLogForBatchDeletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);

            // when
            var deletionDate = helper.AddSecondsToClock(12);
            managementService.DeleteBatch(batch.Id, false);

            // then a deletion historic job log was added
            var jobLog = helper.GetHistoricSeedJobLog(batch)[1];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.DeletionLog);
            Assert.AreEqual(deletionDate, jobLog.TimeStamp);
        }

        [Test]
        public virtual void testHistoricMonitorJobLogForBatchDeletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);

            // when
            var deletionDate = helper.AddSecondsToClock(12);
            managementService.DeleteBatch(batch.Id, false);

            // then a deletion historic job log was added
            var jobLog = helper.GetHistoricMonitorJobLog(batch)[1];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.DeletionLog);
            Assert.AreEqual(deletionDate, jobLog.TimeStamp);
        }

        [Test]
        public virtual void testHistoricBatchJobLogForBatchDeletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);

            // when
            var deletionDate = helper.AddSecondsToClock(12);
            managementService.DeleteBatch(batch.Id, false);

            // then a deletion historic job log was added
            var jobLog = helper.GetHistoricBatchJobLog(batch)[1];
            Assert.NotNull(jobLog);
            Assert.True(jobLog.DeletionLog);
            Assert.AreEqual(deletionDate, jobLog.TimeStamp);
        }

        [Test]
        public virtual void testDeleteHistoricBatch()
        {
            var batch = helper.MigrateProcessInstancesAsync(1);
            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);
            helper.ExecuteMonitorJob(batch);

            // when
            var historicBatch = helper.GetHistoricBatch(batch);
            historyService.DeleteHistoricBatch(historicBatch.Id);

            // then the historic batch was removed and all job logs
            Assert.IsNull(helper.GetHistoricBatch(batch));
            Assert.True(helper.GetHistoricSeedJobLog(batch)
                            .Count == 0);
            Assert.True(helper.GetHistoricMonitorJobLog(batch)
                            .Count == 0);
            Assert.True(helper.GetHistoricBatchJobLog(batch)
                            .Count == 0);
        }

        [Test]
        public virtual void testHistoricSeedJobIncidentDeletion()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            var seedJob = helper.GetSeedJob(batch);
            managementService.SetJobRetries(seedJob.Id, 0);

            managementService.DeleteBatch(batch.Id, false);

            // when
            historyService.DeleteHistoricBatch(batch.Id);

            // then the historic incident was deleted
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testHistoricMonitorJobIncidentDeletion()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(1);

            helper.ExecuteSeedJob(batch);
            var monitorJob = helper.GetMonitorJob(batch);
            managementService.SetJobRetries(monitorJob.Id, 0);

            managementService.DeleteBatch(batch.Id, false);

            // when
            historyService.DeleteHistoricBatch(batch.Id);

            // then the historic incident was deleted
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testHistoricBatchJobLogIncidentDeletion()
        {
            // given
            var batch = helper.MigrateProcessInstancesAsync(3);

            helper.ExecuteSeedJob(batch);
            helper.FailExecutionJobs(batch, 3);

            managementService.DeleteBatch(batch.Id, false);

            // when
            historyService.DeleteHistoricBatch(batch.Id);

            // then the historic incident was deleted
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        protected internal virtual void AssertCommonMonitorJobLogProperties(IBatch batch, IHistoricJobLog jobLog)
        {
            Assert.NotNull(jobLog);
            Assert.AreEqual(batch.MonitorJobDefinitionId, jobLog.JobDefinitionId);
            Assert.AreEqual(BatchMonitorJobHandler.TYPE, jobLog.JobDefinitionType);
            Assert.AreEqual(batch.Id, jobLog.JobDefinitionConfiguration);
            Assert.IsNull(jobLog.DeploymentId);
            Assert.IsNull(jobLog.ProcessDefinitionId);
            Assert.IsNull(jobLog.ExecutionId);
        }
    }
}