using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Bpmn.MultiInstance;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [TestFixture]
    public class ModificationExecutionAsyncTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
        }

        [SetUp]
        public virtual void setClock()
        {
            ClockUtil.CurrentTime = START_DATE;
        }

        [SetUp]
        public virtual void createBpmnModelInstance()
        {
            instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1")
                .StartEvent("start")
                .UserTask("user1")
                .SequenceFlowId("seq")
                .UserTask("user2")
                .EndEvent("end")
                .Done();
        }

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [SetUp]
        public virtual void storeEngineSettings()
        {
            var configuration = rule.ProcessEngineConfiguration;
            defaultBatchJobsPerSeed = configuration.BatchJobsPerSeed;
            defaultInvocationsPerBatchJob = configuration.InvocationsPerBatchJob;
        }

        [TearDown]
        public virtual void restoreEngineSettings()
        {
            var configuration = rule.ProcessEngineConfiguration;
            configuration.BatchJobsPerSeed = defaultBatchJobsPerSeed;
            configuration.InvocationsPerBatchJob = defaultInvocationsPerBatchJob;
        }

        [TearDown]
        public virtual void removeInstanceIds()
        {
            helper.CurrentProcessInstances = new List<string>();
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);
        private int defaultBatchJobsPerSeed;
        private int defaultInvocationsPerBatchJob;
        protected internal BatchModificationHelper helper;
        protected internal IBpmnModelInstance instance;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineTestRule testRule;

        public ModificationExecutionAsyncTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(rule);
            helper = new BatchModificationHelper(rule);
            ////ruleChain = RuleChain.outerRule(rule).around(testRule);
        }

        [Test]
        public virtual void createBatchModification()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var processInstanceIds = helper.StartInstances("process1", 2);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartAfterActivity("user2")
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();

            AssertBatchCreated(batch, 2);
        }

        [Test]
        public virtual void createModificationWithNullProcessInstanceIdsListAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds((IList<string>) null)
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void createModificationWithNullProcessDefinitionId()
        {
            try
            {
                runtimeService.CreateModification(null)
                    .CancelAllForActivity("activityId")
                    .SetProcessInstanceIds("20", "1--0")
                    .ExecuteAsync();
                Assert.Fail("Should not succed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinitionId is null"));
            }
        }


        [Test]
        public virtual void createModificationUsingProcessInstanceIdsListWithNullValueAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds("foo", null, "bar")
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void createModificationWithEmptyProcessInstanceIdsListAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds(new List<string>())
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void createModificationWithNullProcessInstanceIdsArrayAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds(null)
                    .ExecuteAsync();
                Assert.Fail("Should not be able to Modify");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void createModificationUsingProcessInstanceIdsArrayWithNullValueAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .CancelAllForActivity("user1")
                    .SetProcessInstanceIds("foo", null, "bar")
                    .ExecuteAsync();
                Assert.Fail("Should not be able to Modify");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceQueryAsync()
        {
            try
            {
                runtimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceQuery(null)
                    .ExecuteAsync();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void createModificationWithNonExistingProcessDefinitionId()
        {
            var deployment = testRule.Deploy(instance);
            var deploymentDeployedProcessDefinition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = helper.StartInstances("process1", 2);
            try
            {
                runtimeService.CreateModification("foo")
                    .CancelAllForActivity("activityId")
                    .SetProcessInstanceIds(processInstanceIds)
                    .ExecuteAsync();
                Assert.Fail("Should not succed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinition is null"));
            }
        }

        [Test]
        public virtual void createSeedJob()
        {
            // when
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartAfterAsync("process1", 3, "user1", processDefinition.Id);

            // then there exists a seed job definition with the batch id as
            // configuration
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            Assert.NotNull(seedJobDefinition);
            Assert.AreEqual(batch.Id, seedJobDefinition.JobConfiguration);
            Assert.AreEqual(BatchSeedJobHandler.TYPE, seedJobDefinition.JobType);

            // and there exists a modification job definition
            var modificationJobDefinition = helper.GetExecutionJobDefinition(batch);
            Assert.NotNull(modificationJobDefinition);
            Assert.AreEqual(BatchFields.TypeProcessInstanceModification, modificationJobDefinition.JobType);

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

            // but no modification jobs where created
            var modificationJobs = helper.GetExecutionJobs(batch);
            Assert.AreEqual(0, modificationJobs.Count);
        }

        [Test]
        public virtual void createModificationJobs()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            rule.ProcessEngineConfiguration.BatchJobsPerSeed = 10;
            var batch = helper.StartAfterAsync("process1", 20, "user1", processDefinition.Id);
            var seedJobDefinition = helper.GetSeedJobDefinition(batch);
            var modificationJobDefinition = helper.GetExecutionJobDefinition(batch);

            helper.ExecuteSeedJob(batch);

            var modificationJobs = helper.GetJobsForDefinition(modificationJobDefinition);
            Assert.AreEqual(10, modificationJobs.Count);

            foreach (var modificationJob in modificationJobs)
            {
                Assert.AreEqual(modificationJobDefinition.Id, modificationJob.JobDefinitionId);
                Assert.IsNull(modificationJob.Duedate);
                Assert.IsNull(modificationJob.ProcessDefinitionId);
                Assert.IsNull(modificationJob.ProcessDefinitionKey);
                Assert.IsNull(modificationJob.ProcessInstanceId);
                Assert.IsNull(modificationJob.ExecutionId);
            }

            // and the seed job still exists
            var seedJob = helper.GetJobForDefinition(seedJobDefinition);
            Assert.NotNull(seedJob);
        }

        [Test]
        public virtual void createMonitorJob()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartAfterAsync("process1", 10, "user1", processDefinition.Id);

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
        public virtual void executeModificationJobsForStartAfter()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];

            var batch = helper.StartAfterAsync("process1", 10, "user1", processDefinition.Id);
            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForStartBefore()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];

            var batch = helper.StartBeforeAsync("process1", 10, "user2", processDefinition.Id);
            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForStartTransition()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];

            var batch = helper.StartTransitionAsync("process1", 10, "seq", processDefinition.Id);
            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForCancelAll()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.CancelAllAsync("process1", 10, "user1", processDefinition.Id);
            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.IsNull(updatedTree);
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForStartAfterAndCancelAll()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];
            var instances = helper.StartInstances("process1", 10);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartAfterActivity("user1")
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(instances)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user2")
                        .Done());
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForStartBeforeAndCancelAll()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var instances = helper.StartInstances("process1", 10);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartBeforeActivity("user1")
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(instances)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.IsNull(updatedTree);
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForStartTransitionAndCancelAll()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];

            var instances = helper.StartInstances("process1", 10);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartTransition("seq")
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(instances)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user2")
                        .Done());
            }

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void executeModificationJobsForProcessInstancesWithDifferentStates()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = helper.StartInstances("process1", 1);
            var task = rule.TaskService.CreateTaskQuery()
                .First();
            rule.TaskService.Complete(task.Id);

            var anotherProcessInstanceIds = helper.StartInstances("process1", 1);
            ((List<string>) processInstanceIds).AddRange(anotherProcessInstanceIds);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartBeforeActivity("user2")
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            var modificationJobs = helper.GetExecutionJobs(batch);

            // when
            foreach (var modificationJob in modificationJobs)
                helper.ExecuteJob(modificationJob);

            // then all process instances where modified
            IActivityInstance updatedTree = null;
            var ProcessInstanceId = processInstanceIds[0];
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .Activity("user2")
                    .Activity("user2")
                    .Done());

            ProcessInstanceId = processInstanceIds[1];
            updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
            Assert.NotNull(updatedTree);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                    .Activity("user1")
                    .Activity("user2")
                    .Done());

            // and the no modification jobs exist
            Assert.AreEqual(0, helper.GetExecutionJobs(batch)
                .Count);

            // but a monitor job exists
            Assert.NotNull(helper.GetMonitorJob(batch));
        }

        [Test]
        public virtual void testMonitorJobPollingForCompletion()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartAfterAsync("process1", 3, "user1", processDefinition.Id);

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
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartBeforeAsync("process1", 10, "user2", processDefinition.Id);
            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);

            // when
            helper.ExecuteMonitorJob(batch);

            // then the batch was completed and removed
            Assert.AreEqual(0, rule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed jobs was removed
            Assert.AreEqual(0, rule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithCascade()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartTransitionAsync("process1", 10, "seq", processDefinition.Id);
            helper.ExecuteSeedJob(batch);

            // when
            rule.ManagementService.DeleteBatch(batch.Id, true);

            // then the batch was deleted
            Assert.AreEqual(0, rule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed and modification job definition were deleted
            Assert.AreEqual(0, rule.ManagementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and modification jobs were deleted
            Assert.AreEqual(0, rule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithoutCascade()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartBeforeAsync("process1", 10, "user2", processDefinition.Id);
            helper.ExecuteSeedJob(batch);

            // when
            rule.ManagementService.DeleteBatch(batch.Id, false);

            // then the batch was deleted
            Assert.AreEqual(0, rule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed and modification job definition were deleted
            Assert.AreEqual(0, rule.ManagementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and modification jobs were deleted
            Assert.AreEqual(0, rule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.CancelAllAsync("process1", 2, "user1", processDefinition.Id);

            // create incident
            var seedJob = helper.GetSeedJob(batch);
            rule.ManagementService.SetJobRetries(seedJob.Id, 0);

            // when
            rule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = rule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedModificationJobDeletionWithCascade()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartAfterAsync("process1", 2, "user1", processDefinition.Id);
            helper.ExecuteSeedJob(batch);

            // create incidents
            var modificationJobs = helper.GetExecutionJobs(batch);
            foreach (var modificationJob in modificationJobs)
                rule.ManagementService.SetJobRetries(modificationJob.Id, 0);

            // when
            rule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = rule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
        {
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var batch = helper.StartBeforeAsync("process1", 2, "user2", processDefinition.Id);
            helper.ExecuteSeedJob(batch);

            // create incident
            var monitorJob = helper.GetMonitorJob(batch);
            rule.ManagementService.SetJobRetries(monitorJob.Id, 0);

            // when
            rule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = rule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testModificationJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
        {
            var processEngineConfiguration = rule.ProcessEngineConfiguration;

            processEngineConfiguration.SetAuthorizationEnabled(true);
            var processDefinition = testRule.DeployForTenantAndGetDefinition("tenantId", instance);

            try
            {
                var batch = helper.StartAfterAsync("process1", 10, "user1", processDefinition.Id);
                helper.ExecuteSeedJob(batch);

                testRule.WaitForJobExecutorToProcessAllJobs();

                // then all process instances where modified
                foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
                {
                    var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                    Assert.NotNull(updatedTree);
                    Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                    ActivityInstanceAssert.That(updatedTree)
                        .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                            .Activity("user1")
                            .Activity("user2")
                            .Done());
                }
            }
            finally
            {
                processEngineConfiguration.SetAuthorizationEnabled(false);
            }
        }

        [Test]
        public virtual void testBatchExecutionFailureWithMissingProcessInstance()
        {
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];
            var batch = helper.StartAfterAsync("process1", 2, "user1", processDefinition.Id);
            helper.ExecuteSeedJob(batch);

            var processInstances = runtimeService.CreateProcessInstanceQuery()
                
                .ToList();
            var deletedProcessInstanceId = processInstances[0].Id;

            // when
            runtimeService.DeleteProcessInstance(deletedProcessInstanceId, "test");
            helper.ExecuteJobs(batch);

            // then the remaining process instance was modified
            foreach (var ProcessInstanceId in helper.CurrentProcessInstances)
            {
                if (ProcessInstanceId.Equals(helper.CurrentProcessInstances[0]))
                {
                    var u = runtimeService.GetActivityInstance(ProcessInstanceId);
                    Assert.IsNull(u);
                    continue;
                }

                var updatedTree = runtimeService.GetActivityInstance(ProcessInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(ProcessInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }

            // and one batch job failed and has 2 retries left
            var modificationJobs = helper.GetExecutionJobs(batch);
            Assert.AreEqual(1, modificationJobs.Count);

            var failedJob = modificationJobs[0];
            Assert.AreEqual(2, failedJob.Retries);
            Assert.That(failedJob.ExceptionMessage, Does.Contain("ENGINE-13036"));
            Assert.That(failedJob.ExceptionMessage,
                Does.Contain("Process instance '" + deletedProcessInstanceId + "' cannot be modified"));
        }

        [Test]
        public virtual void testBatchCreationWithProcessInstanceQuery()
        {
            var processInstanceCount = 15;
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];
            helper.StartInstances("process1", 15);

            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionId(processDefinition.Id)
                ;
            Assert.AreEqual(processInstanceCount, processInstanceQuery.Count());

            // when
            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartAfterActivity("user1")
                .SetProcessInstanceQuery(processInstanceQuery)
                .ExecuteAsync();

            // then a batch is created
            AssertBatchCreated(batch, processInstanceCount);
        }

        [Test]
        public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
        {
            var processInstanceCount = 15;
            var deployment = testRule.Deploy(instance);
            var processDefinition = deployment.DeployedProcessDefinitions[0];
            var processInstanceIds = helper.StartInstances("process1", 15);

            var processInstanceQuery = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionId(processDefinition.Id)
                ;
            Assert.AreEqual(processInstanceCount, processInstanceQuery.Count());

            // when
            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartTransition("seq")
                .SetProcessInstanceIds(processInstanceIds)
                .SetProcessInstanceQuery(processInstanceQuery)
                .ExecuteAsync();

            // then a batch is created
            AssertBatchCreated(batch, processInstanceCount);
        }

        [Test]
        public virtual void testListenerInvocation()
        {
            // given
            DelegateEvent.ClearEvents();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var processDefinition = testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(instance)
                //.ActivityBuilder("user2")
                //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                //    typeof(DelegateExecutionListener).FullName)
                //.Done()
            );

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartBeforeActivity("user2")
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            var recordedEvents = DelegateEvent.Events;
            Assert.AreEqual(1, recordedEvents.Count);

            var @event = recordedEvents[0];
            Assert.AreEqual(processDefinition.Id, @event.ProcessDefinitionId);
            Assert.AreEqual("user2", @event.CurrentActivityId);

            DelegateEvent.ClearEvents();
        }

        [Test]
        public virtual void testSkipListenerInvocationF()
        {
            // given
            DelegateEvent.ClearEvents();
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var processDefinition = testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(instance)
                //.ActivityBuilder("user2")
                //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                //    typeof(DelegateExecutionListener).FullName)
                //.Done()
            );

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .CancelAllForActivity("user2")
                .SetProcessInstanceIds(processInstance.Id)
                .SetSkipCustomListeners()
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            Assert.AreEqual(0, DelegateEvent.Events.Count);
        }

        [Test]
        public virtual void testIoMappingInvocation()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(instance)
                //.ActivityBuilder("user1")
                //.CamundaInputParameter("foo", "bar")
                //.Done()
            );

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartAfterActivity("user2")
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            var inputVariable = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(inputVariable);
            Assert.AreEqual("foo", inputVariable.Name);
            Assert.AreEqual("bar", inputVariable.Value);

            var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(activityInstance.GetActivityInstances("user1")[0].Id, inputVariable.ActivityInstanceId);
        }

        [Test]
        public virtual void testSkipIoMappingInvocation()
        {
            // given

            var processDefinition = testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(instance)
                //.ActivityBuilder("user2")
                //.CamundaInputParameter("foo", "bar")
                //.Done()
            );


            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartBeforeActivity("user2")
                .SetProcessInstanceIds(processInstance.Id)
                .SetSkipIoMappings()
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            helper.ExecuteJobs(batch);

            // then
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());
        }

        protected internal virtual void AssertBatchCreated(IBatch batch, int processInstanceCount)
        {
            Assert.NotNull(batch);
            Assert.NotNull(batch.Id);
            Assert.AreEqual("instance-modification", batch.Type);
            Assert.AreEqual(processInstanceCount, batch.TotalJobs);
            Assert.AreEqual(defaultBatchJobsPerSeed, batch.BatchJobsPerSeed);
            Assert.AreEqual(defaultInvocationsPerBatchJob, batch.InvocationsPerBatchJob);
        }
    }
}