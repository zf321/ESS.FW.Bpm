using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Bpmn.MultiInstance;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.Batch
{
    [TestFixture]
    public class BatchMigrationTest
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

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [SetUp]
        public virtual void storeEngineSettings()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
        }

        [TearDown]
        public virtual void restoreEngineSettings()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
            configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
        }

        private readonly bool InstanceFieldsInitialized;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal ProcessEngineConfigurationImpl configuration;

        protected internal int defaultBatchJobsPerSeed;
        protected internal int defaultInvocationsPerBatchJob;


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal BatchMigrationHelper helper;
        protected internal IHistoryService historyService;
        protected internal IManagementService managementService;
        protected internal MigrationTestRule migrationRule;
        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineTestRule testRule;

        public BatchMigrationTest()
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
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(migrationRule).around(testRule);
        }


        [Test]
        public virtual void testNullMigrationPlan()
        {
            try
            {
                runtimeService.NewMigration(null)
                    .ProcessInstanceIds(new List<string> {"process"})
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("migration plan is null"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceIdsList()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds((IList<string>) null)
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testProcessInstanceIdsListWithNullValue()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds("foo", null, "bar")
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void testEmptyProcessInstanceIdsList()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(new List<string>())
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceIdsArray()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(null)
                    .ExecuteAsync();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testProcessInstanceIdsArrayWithNullValue()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds("foo", null, "bar")
                    .ExecuteAsync();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceQuery()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceQuery(null)
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }
        [Test]
        public virtual void testEmptyProcessInstanceQuery()
        {
            var testProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            var emptyProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery();
            Assert.AreEqual(0, emptyProcessInstanceQuery.Count());

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceQuery(emptyProcessInstanceQuery)
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testBatchCreation()
        {
            // when
            var batch = helper.MigrateProcessInstancesAsync(15);

            // then a batch is created
            AssertBatchCreated(batch, 15);
        }

        [Test]
        public virtual void testSeedJobCreation()
        {
            // when
            var batch = helper.MigrateProcessInstancesAsync(10);

            // then there exists a seed job definition with the batch id as configuration
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            Assert.NotNull(seedJobDefinition);
            Assert.AreEqual(batch.Id, seedJobDefinition.JobConfiguration);
            Assert.AreEqual(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

            // and there exists a migration job definition
            var migrationJobDefinition = helper.GetExecutionJobDefinition(batch);
            Assert.NotNull(migrationJobDefinition);
            Assert.AreEqual(BatchFields.TypeProcessInstanceMigration, migrationJobDefinition.JobType);

            // and a seed job with no relation to a process or execution etc.
            var seedJob = helper.GetSeedJob(batch);
            Assert.NotNull(seedJob);
            Assert.AreEqual(seedJobDefinition.Id, seedJob.JobDefinitionId);
            Assert.IsNull(seedJob.Duedate);
            Assert.IsNull(seedJob.DeploymentId);
            Assert.IsNull(seedJob.ProcessDefinitionId);
            Assert.IsNull(seedJob.ProcessDefinitionKey);
            Assert.IsNull(seedJob.ProcessInstanceId);
            Assert.IsNull(seedJob.ExecutionId);

            // but no migration jobs where created
            var migrationJobs = helper.GetExecutionJobs(batch);
            Assert.AreEqual(0, migrationJobs.Count);
        }

        [Test]
        public virtual void testMigrationJobsCreation()
        {
            // reduce number of batch jobs per seed to not have to create a lot of instances
            engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

            var batch = helper.MigrateProcessInstancesAsync(20);
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            var migrationJobDefinition = helper.GetExecutionJobDefinition(batch);
            var sourceDeploymentId = helper.SourceProcessDefinition.DeploymentId;

            // when
            helper.ExecuteSeedJob(batch);

            // then there exist migration jobs
            var migrationJobs = helper.GetJobsForDefinition(migrationJobDefinition);
            Assert.AreEqual(10, migrationJobs.Count);

            foreach (var migrationJob in migrationJobs)
            {
                Assert.AreEqual(migrationJobDefinition.Id, migrationJob.JobDefinitionId);
                Assert.IsNull(migrationJob.Duedate);
                Assert.AreEqual(sourceDeploymentId, migrationJob.DeploymentId);
                Assert.IsNull(migrationJob.ProcessDefinitionId);
                Assert.IsNull(migrationJob.ProcessDefinitionKey);
                Assert.IsNull(migrationJob.ProcessInstanceId);
                Assert.IsNull(migrationJob.ExecutionId);
            }

            // and the seed job still exists
            var seedJob = helper.GetJobForDefinition(seedJobDefinition);
            Assert.NotNull(seedJob);
        }

        [Test]
        public virtual void testMonitorJobCreation()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);

            // when
            helper.ExecuteSeedJob(batch);

            // then the seed job definition still exists but the seed job is removed
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            Assert.NotNull(seedJobDefinition);

            var seedJob = helper.GetSeedJob(batch);
            Assert.IsNull(seedJob);

            // and a monitor job definition and job exists
            var monitorJobDefinition = helper.GetMonitorJobDefinition(batch);
            Assert.NotNull(monitorJobDefinition);

            var monitorJob = helper.GetMonitorJob(batch);
            Assert.NotNull(monitorJob);
        }

        [Test]
        public virtual void testMigrationJobsExecution()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);
            helper.ExecuteSeedJob(batch);
            var migrationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var migrationJob in migrationJobs)
                helper.ExecuteJob(migrationJob);

            // then all process instances where migrated
            Assert.AreEqual(0, helper.CountSourceProcessInstances());
            Assert.AreEqual(10, helper.CountTargetProcessInstances());

            // and the no migration jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void testMigrationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
        {
            var processEngineConfiguration = engineRule.ProcessEngineConfiguration;

            processEngineConfiguration.SetAuthorizationEnabled(true);

            try
            {
                var batch = helper.MigrateProcessInstancesAsyncForTenant(10, "someTenantId");
                helper.ExecuteSeedJob(batch);

                testRule.WaitForJobExecutorToProcessAllJobs();

                // then all process instances where migrated
                Assert.AreEqual(0, helper.CountSourceProcessInstances());
                Assert.AreEqual(10, helper.CountTargetProcessInstances());
            }
            finally
            {
                processEngineConfiguration.SetAuthorizationEnabled(false);
            }
        }

        [Test]
        public virtual void testNumberOfJobsCreatedBySeedJobPerInvocation()
        {
            // reduce number of batch jobs per seed to not have to create a lot of instances
            var batchJobsPerSeed = 10;
            engineRule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;

            var batch = helper.MigrateProcessInstancesAsync(batchJobsPerSeed * 2 + 4);

            // when
            helper.ExecuteSeedJob(batch);

            // then the default number of jobs was created
            Assert.AreEqual(batch.BatchJobsPerSeed, helper.GetExecutionJobs(batch)
                .Count);

            // when the seed job is executed a second time
            helper.ExecuteSeedJob(batch);

            // then the same amount of jobs was created
            Assert.AreEqual(2 * batch.BatchJobsPerSeed, helper.GetExecutionJobs(batch)
                .Count);

            // when the seed job is executed a third time
            helper.ExecuteSeedJob(batch);

            // then the all jobs where created
            Assert.AreEqual(2 * batch.BatchJobsPerSeed + 4, helper.GetExecutionJobs(batch)
                .Count);

            // and the seed job is removed
            Assert.IsNull(helper.GetSeedJob(batch));
        }

        [Test]
        public virtual void testDefaultBatchConfiguration()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            Assert.AreEqual(100, configuration.BatchJobsPerSeed);
            Assert.AreEqual(1, configuration.InvocationsPerBatchJob);
            Assert.AreEqual(30, configuration.BatchPollTime);
        }

        [Test]
        public virtual void testCustomNumberOfJobsCreateBySeedJob()
        {
            var configuration = engineRule.ProcessEngineConfiguration;
            configuration.BatchJobsPerSeed = 2;
            configuration.InvocationsPerBatchJob = 5;

            // when
            var batch = helper.MigrateProcessInstancesAsync(20);

            // then the configuration was saved in the batch job
            Assert.AreEqual(2, batch.BatchJobsPerSeed);
            Assert.AreEqual(5, batch.InvocationsPerBatchJob);

            // and the Count was correctly calculated
            Assert.AreEqual(4, batch.TotalJobs);

            // when the seed job is executed
            helper.ExecuteSeedJob(batch);

            // then there exist the first batch of migration jobs
            Assert.AreEqual(2, helper.GetExecutionJobs(batch)
                .Count);

            // when the seed job is executed a second time
            helper.ExecuteSeedJob(batch);

            // then the full batch of migration jobs exist
            Assert.AreEqual(4, helper.GetExecutionJobs(batch)
                .Count);

            // and the seed job is removed
            Assert.IsNull(helper.GetSeedJob(batch));
        }

        [Test]
        public virtual void testMonitorJobPollingForCompletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);

            // when the seed job creates the monitor job
            var createDate = ClockTestUtil.SetClockToDateWithoutMilliseconds();
            helper.ExecuteSeedJob(batch);

            // then the monitor job has a no due date set
            var monitorJob = helper.GetMonitorJob(batch);
            Assert.NotNull(monitorJob);
            Assert.IsNull(monitorJob.Duedate);

            // when the monitor job is executed
            helper.ExecuteMonitorJob(batch);

            // then the monitor job has a due date of the default batch poll time
            monitorJob = helper.GetMonitorJob(batch);
            var dueDate = helper.AddSeconds(createDate, 30);
            Assert.AreEqual(dueDate, monitorJob.Duedate);
        }

        [Test]
        public virtual void testMonitorJobRemovesBatchAfterCompletion()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);
            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);

            // when
            helper.ExecuteMonitorJob(batch);

            // then the batch was completed and removed
            Assert.AreEqual(0, managementService.CreateBatchQuery()
                .Count());

            // and the seed jobs was removed
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithCascade()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.DeleteBatch(batch.Id, true);

            // then the batch was deleted
            Assert.AreEqual(0, managementService.CreateBatchQuery()
                .Count());

            // and the seed and migration job definition were deleted
            Assert.AreEqual(0, managementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and migration jobs were deleted
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithoutCascade()
        {
            var batch = helper.MigrateProcessInstancesAsync(10);
            helper.ExecuteSeedJob(batch);

            // when
            managementService.DeleteBatch(batch.Id, false);

            // then the batch was deleted
            Assert.AreEqual(0, managementService.CreateBatchQuery()
                .Count());

            // and the seed and migration job definition were deleted
            Assert.AreEqual(0, managementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and migration jobs were deleted
            Assert.AreEqual(0, managementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
        {
            var batch = helper.MigrateProcessInstancesAsync(2);

            // create incident
            var seedJob = helper.GetSeedJob(batch);
            managementService.SetJobRetries(seedJob.Id, 0);

            // when
            managementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedMigrationJobDeletionWithCascade()
        {
            var batch = helper.MigrateProcessInstancesAsync(2);
            helper.ExecuteSeedJob(batch);

            // create incidents
            var migrationJobs = helper.GetExecutionJobs(batch);
            foreach (var migrationJob in migrationJobs)
                managementService.SetJobRetries(migrationJob.Id, 0);

            // when
            managementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
        {
            var batch = helper.MigrateProcessInstancesAsync(2);
            helper.ExecuteSeedJob(batch);

            // create incident
            var monitorJob = helper.GetMonitorJob(batch);
            managementService.SetJobRetries(monitorJob.Id, 0);

            // when
            managementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = historyService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchExecutionFailureWithMissingProcessInstance()
        {
            var batch = helper.MigrateProcessInstancesAsync(2);
            helper.ExecuteSeedJob(batch);

            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            var deletedProcessInstanceId = processInstances[0].Id;

            // when
            runtimeService.DeleteProcessInstance(deletedProcessInstanceId, "test");
            helper.ExecuteJobs(batch);

            // then the remaining process instance was migrated
            Assert.AreEqual(0, helper.CountSourceProcessInstances());
            Assert.AreEqual(1, helper.CountTargetProcessInstances());

            // and one batch job failed and has 2 retries left
            var migrationJobs = helper.GetExecutionJobs(batch);
            Assert.AreEqual(1, migrationJobs.Count);

            var failedJob = migrationJobs[0];
            Assert.AreEqual(2, failedJob.Retries);
            Assert.That(failedJob.ExceptionMessage, Does.Contain("ENGINE-23003"));
            Assert.That(failedJob.ExceptionMessage,
                Does.Contain("Process instance '" + deletedProcessInstanceId + "' cannot be migrated"));
        }
        [Test]
        public virtual void testBatchCreationWithProcessInstanceQuery()
        {
            var runtimeService = engineRule.RuntimeService;
            var processInstanceCount = 15;

            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            for (var i = 0; i < processInstanceCount; i++)
                runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            Assert.AreEqual(processInstanceCount, sourceProcessInstanceQuery.Count());

            // when
            var batch = runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .ExecuteAsync();

            // then a batch is created
            AssertBatchCreated(batch, processInstanceCount);
        }

        [Test]
        public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
        {
            var runtimeService = engineRule.RuntimeService;
            var processInstanceCount = 15;

            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            IList<string> processInstanceIds = new List<string>();
            for (var i = 0; i < processInstanceCount; i++)
                processInstanceIds.Add(runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id)
                    .Id);

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == sourceProcessDefinition.Id);
            Assert.AreEqual(processInstanceCount, sourceProcessInstanceQuery.Count());

            // when
            var batch = runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstanceIds)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .ExecuteAsync();

            // then a batch is created
            AssertBatchCreated(batch, processInstanceCount);
        }

        [Test]
        public virtual void testListenerInvocationForNewlyCreatedScope()
        {
            // given
            DelegateEvent.ClearEvents();

            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    /*.ActivityBuilder("subProcess").CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, typeof(DelegateExecutionListener).FullName).Done()*/);

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var batch = engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();
            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            var recordedEvents = DelegateEvent.Events;
            Assert.AreEqual(1, recordedEvents.Count);

            var @event = recordedEvents[0];
            Assert.AreEqual(targetProcessDefinition.Id, @event.ProcessDefinitionId);
            Assert.AreEqual("subProcess", @event.CurrentActivityId);

            DelegateEvent.ClearEvents();
        }

        [Test]
        public virtual void testSkipListenerInvocationForNewlyCreatedScope()
        {
            // given
            DelegateEvent.ClearEvents();

            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    //    typeof(DelegateExecutionListener).FullName)
                    /*.Done()*/);

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var batch = engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .SkipCustomListeners()
                .ExecuteAsync();
            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            Assert.AreEqual(0, DelegateEvent.Events.Count);
        }

        [Test]
        public virtual void testIoMappingInvocationForNewlyCreatedScope()
        {
            // given
            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.camundaInputParameter("foo", "bar")
                    //.Done()
                );

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var batch = engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();
            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            var inputVariable = engineRule.RuntimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(inputVariable);
            Assert.AreEqual("foo", inputVariable.Name);
            Assert.AreEqual("bar", inputVariable.Value);

            var activityInstance = engineRule.RuntimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(activityInstance.GetActivityInstances("subProcess")[0].Id, inputVariable.ActivityInstanceId);
        }

        [Test]
        public virtual void testSkipIoMappingInvocationForNewlyCreatedScope()
        {
            // given
            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.camundaInputParameter("foo", "bar")
                    //.Done()
                );

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var batch = engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .SkipIoMappings()
                .ExecuteAsync();
            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            Assert.AreEqual(0, engineRule.RuntimeService.CreateVariableInstanceQuery()
                .Count());
        }

        [Test]
        public virtual void testUpdateEventTrigger()
        {
            // given
            var newMessageName = "newMessage";

            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneReceiveTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(
                        ProcessModels.OneReceiveTaskProcess)
                    .RenameMessage("Message", newMessageName));

            var processInstance = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .UpdateEventTriggers()
                    .Build();

            var batch = runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(new List<string> {processInstance.Id})
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then the message event subscription's event name was changed
            var eventSubscription = runtimeService.CreateEventSubscriptionQuery()
                .First();
            Assert.AreEqual(newMessageName, eventSubscription.EventName);
        }

        [Test]
        public virtual void testDeleteBatchJobManually()
        {
            // given
            var batch = helper.CreateMigrationBatchWithSize(1);
            helper.ExecuteSeedJob(batch);

            var migrationJob = (JobEntity) helper.GetExecutionJobs(batch)[0];
            var byteArrayId = migrationJob.JobHandlerConfigurationRaw;

            var byteArrayEntity =
                engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new GetByteArrayCommand(this,
                    byteArrayId));
            Assert.NotNull(byteArrayEntity);

            // when
            managementService.DeleteJob(migrationJob.Id);

            // then
            byteArrayEntity =
                engineRule.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(new GetByteArrayCommand(this,
                    byteArrayId));
            Assert.IsNull(byteArrayEntity);
        }

        [Test]
        public virtual void testMigrateWithVarargsArray()
        {
            var sourceDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan = runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapEqualActivities()
                .Build();

            var processInstance1 = runtimeService.StartProcessInstanceById(sourceDefinition.Id);
            var processInstance2 = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

            // when
            var batch = runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);
            helper.ExecuteMonitorJob(batch);

            // then
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==targetDefinition.Id)
                .Count());
        }

        protected internal virtual void AssertBatchCreated(IBatch batch, int processInstanceCount)
        {
            Assert.NotNull(batch);
            Assert.NotNull(batch.Id);
            Assert.AreEqual("instance-migration", batch.Type);
            Assert.AreEqual(processInstanceCount, batch.TotalJobs);
            Assert.AreEqual(defaultBatchJobsPerSeed, batch.BatchJobsPerSeed);
            Assert.AreEqual(defaultInvocationsPerBatchJob, batch.InvocationsPerBatchJob);
        }

        public class GetByteArrayCommand : ICommand<ResourceEntity>
        {
            private readonly BatchMigrationTest outerInstance;


            protected internal string byteArrayId;

            public GetByteArrayCommand(BatchMigrationTest outerInstance, string byteArrayId)
            {
                this.outerInstance = outerInstance;
                this.byteArrayId = byteArrayId;
            }

            public virtual ResourceEntity Execute(CommandContext commandContext)
            {
                return (ResourceEntity) commandContext.ByteArrayManager.Get(byteArrayId);
            }
        }
    }
}