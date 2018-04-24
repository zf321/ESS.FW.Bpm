using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class RestartProcessInstanceAsyncTest
    {
        [SetUp]
        public virtual void init()
        {
            runtimeService = engineRule.RuntimeService;
            taskService = engineRule.TaskService;
            historyService = engineRule.HistoryService;
            managementService = engineRule.ManagementService;
            defaultTenantIdProvider = engineRule.ProcessEngineConfiguration.TenantIdProvider;
        }

        [TearDown]
        public virtual void reset()
        {
            helper.RemoveAllRunningAndHistoricBatches();
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(defaultTenantIdProvider);
        }

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        protected internal ITenantIdProvider defaultTenantIdProvider;


        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal BatchRestartHelper helper;
        protected internal IHistoryService historyService;
        private readonly bool InstanceFieldsInitialized;
        protected internal IManagementService managementService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal ProcessEngineTestRule testRule;

        public RestartProcessInstanceAsyncTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            helper = new BatchRestartHelper(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }

        [Test]
        public virtual void createBatchRestart()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            IList<string> processInstanceIds = new[] {processInstance1.Id, processInstance2.Id};

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartAfterActivity("userTask2")
                .SetProcessInstanceIds(processInstanceIds)
                .ExecuteAsync();

            // then
            AssertBatchCreated(batch, 2);
        }

        [Test]
        public virtual void restartProcessInstanceWithNullProcessDefinitionId()
        {
            try
            {
                runtimeService.RestartProcessInstances(null)
                    .ExecuteAsync();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinitionId is null"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithoutInstructions()
        {
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            try
            {
                var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                    .SetProcessInstanceIds(processInstance.Id)
                    .ExecuteAsync();
                helper.CompleteBatch(batch);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("instructions is empty"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithoutProcessInstanceIds()
        {
            try
            {
                runtimeService.RestartProcessInstances("foo")
                    .StartAfterActivity("bar")
                    .ExecuteAsync();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("processInstanceIds is empty"));
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithNullProcessInstanceId()
        {
            try
            {
                runtimeService.RestartProcessInstances("foo")
                    .StartAfterActivity("bar")
                    .SetProcessInstanceIds((string) null)
                    .ExecuteAsync();
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("processInstanceIds contains null value"));
            }
        }

        [Test]
        public virtual void restartNotExistingProcessInstance()
        {
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("bar")
                .SetProcessInstanceIds("aaa")
                .ExecuteAsync();
            helper.ExecuteSeedJob(batch);
            try
            {
                helper.ExecuteJobs(batch);
                Assert.Fail("exception expected");
            }
            catch (BadUserRequestException e)
            {
                Assert.That(e.Message, Does.Contain("Historic process instance cannot be found"));
            }
        }

        [Test]
        public virtual void shouldRestartProcessInstance()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            var task1 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            var task2 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance2.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance = restartedProcessInstances[0];
            var restartedTask = engineRule.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(task1.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

            restartedProcessInstance = restartedProcessInstances[1];
            restartedTask = engineRule.TaskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(task2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithParallelGateway()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .StartBeforeActivity("userTask2")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            foreach (var restartedProcessInstance in restartedProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .Activity("userTask1")
                        .Activity("userTask2")
                        .Done());
            }
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithSubProcess()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("subProcess")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            foreach (var restartedProcessInstance in restartedProcessInstances)
            {
                var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                        .BeginScope("subProcess")
                        .Activity("userTask")
                        .Done());
            }
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithInitialVariables()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent()
                .UserTask("userTask1")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RestartProcessInstanceSyncTest.SetVariableExecutionListenerImpl).FullName)
                .UserTask("userTask2")
                .EndEvent()
                .Done();

            var processDefinition = testRule.DeployAndGetDefinition(instance);

            // initial variables
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "bar"));
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("var", "bar"));

            // variables update
            var tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // Delete process instances
            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .InitialSetOfVariables()
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var variableInstance1 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstances[0].Id)
                .First();
            var variableInstance2 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstances[1].Id)
                .First();

            Assert.AreEqual(variableInstance1.ExecutionId, restartedProcessInstances[0].Id);
            Assert.AreEqual(variableInstance2.ExecutionId, restartedProcessInstances[1].Id);
            Assert.AreEqual("var", variableInstance1.Name);
            Assert.AreEqual("bar", variableInstance1.Value);
            Assert.AreEqual("var", variableInstance2.Name);
            Assert.AreEqual("bar", variableInstance2.Value);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithVariables()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent()
                .UserTask("userTask1")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RestartProcessInstanceSyncTest.SetVariableExecutionListenerImpl).FullName)
                .UserTask("userTask2")
                .EndEvent()
                .Done();

            var processDefinition = testRule.DeployAndGetDefinition(instance);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            // variables are set at the beginning
            runtimeService.SetVariable(processInstance1.Id, "var", "bar");
            runtimeService.SetVariable(processInstance2.Id, "var", "bb");

            // variables are changed
            var tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);

            // process instances are deleted
            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance = restartedProcessInstances[0];
            var variableInstance1 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                .First();
            Assert.AreEqual(variableInstance1.ExecutionId, restartedProcessInstance.Id);

            restartedProcessInstance = restartedProcessInstances[1];
            var variableInstance2 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstance.Id)
                .First();
            Assert.AreEqual(variableInstance2.ExecutionId, restartedProcessInstance.Id);
            Assert.True(variableInstance1.Name.Equals(variableInstance2.Name));
            Assert.AreEqual("var", variableInstance1.Name);
            Assert.True(variableInstance1.Value.Equals(variableInstance2.Value));
            Assert.AreEqual("foo", variableInstance2.Value);
        }

        [Test]
        public virtual void shouldNotSetLocalVariables()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            var subProcess1 = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance1.Id&& c.ActivityId =="userTask")
                .First();
            var subProcess2 = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance2.Id&& c.ActivityId =="userTask")
                .First();
            runtimeService.SetVariableLocal(subProcess1.Id, "local", "foo");
            runtimeService.SetVariableLocal(subProcess2.Id, "local", "foo");

            runtimeService.SetVariable(processInstance1.Id, "var", "bar");
            runtimeService.SetVariable(processInstance2.Id, "var", "bar");


            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");


            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var variables1 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstances[0].Id)
                
                .ToList();
            Assert.AreEqual(1, variables1.Count);
            Assert.AreEqual("var", variables1[0].Name);
            Assert.AreEqual("bar", variables1[0].Value);
            var variables2 = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstances[1].Id)
                
                .ToList();
            Assert.AreEqual(1, variables1.Count);
            Assert.AreEqual("var", variables2[0].Name);
            Assert.AreEqual("bar", variables2[0].Value);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceUsingHistoricProcessInstanceQuery()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var task1 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance1.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();

            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");
            var task2 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== processInstance2.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var historicProcessInstanceQuery = engineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id);

            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetHistoricProcessInstanceQuery(historicProcessInstanceQuery)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance = restartedProcessInstances[0];
            var restartedTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(task1.TaskDefinitionKey, restartedTask.TaskDefinitionKey);

            restartedProcessInstance = restartedProcessInstances[1];
            restartedTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== restartedProcessInstance.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(task2.TaskDefinitionKey, restartedTask.TaskDefinitionKey);
        }

        [Test]
        public virtual void testBatchCreationWithOverlappingProcessInstanceIdsAndQuery()
        {
            // given
            var processInstanceCount = 2;
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var processInstanceQuery = engineRule.HistoryService.CreateHistoricProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id);

            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .SetHistoricProcessInstanceQuery(processInstanceQuery)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id)
                
                .ToList();
            Assert.AreEqual(restartedProcessInstances.Count, processInstanceCount);
        }

        [Test]
        public virtual void testMonitorJobPollingForCompletion()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

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
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);
            helper.ExecuteJobs(batch);

            helper.ExecuteMonitorJob(batch);

            // then the batch was completed and removed
            Assert.AreEqual(0, engineRule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed jobs was removed
            Assert.AreEqual(0, engineRule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithCascade()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            engineRule.ManagementService.DeleteBatch(batch.Id, true);

            // then the batch was deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed and execution job definition were deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and execution jobs were deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchDeletionWithoutCascade()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            engineRule.ManagementService.DeleteBatch(batch.Id, false);

            // then the batch was deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateBatchQuery()
                .Count());

            // and the seed and execution job definition were deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateJobDefinitionQuery()
                .Count());

            // and the seed job and execution jobs were deleted
            Assert.AreEqual(0, engineRule.ManagementService.CreateJobQuery()
                .Count());
        }

        [Test]
        public virtual void testBatchWithFailedSeedJobDeletionWithCascade()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            // create incident
            var seedJob = helper.GetSeedJob(batch);
            engineRule.ManagementService.SetJobRetries(seedJob.Id, 0);

            engineRule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = engineRule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedExecutionJobDeletionWithCascade()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // create incidents
            var executionJobs = helper.GetExecutionJobs(batch);
            foreach (var executionJob in executionJobs)
                engineRule.ManagementService.SetJobRetries(executionJob.Id, 0);

            engineRule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = engineRule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testBatchWithFailedMonitorJobDeletionWithCascade()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartTransition("flow1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // create incident
            var monitorJob = helper.GetMonitorJob(batch);
            engineRule.ManagementService.SetJobRetries(monitorJob.Id, 0);

            engineRule.ManagementService.DeleteBatch(batch.Id, true);

            // then the no historic incidents exists
            var historicIncidents = engineRule.HistoryService.CreateHistoricIncidentQuery()
                .Count();
            Assert.AreEqual(0, historicIncidents);
        }

        [Test]
        public virtual void testJobsExecutionByJobExecutorWithAuthorizationEnabledAndTenant()
        {
            // given
            var processEngineConfiguration = engineRule.ProcessEngineConfiguration;

            processEngineConfiguration.SetAuthorizationEnabled(true);
            var processDefinition = testRule.DeployForTenantAndGetDefinition("tenantId", ProcessModels.TwoTasksProcess);

            try
            {
                var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
                var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

                IList<string> list = new[] {processInstance1.Id, processInstance2.Id};

                runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
                runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

                // when
                var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                    .StartTransition("flow1")
                    .SetProcessInstanceIds(list)
                    .ExecuteAsync();
                helper.ExecuteSeedJob(batch);

                testRule.WaitForJobExecutorToProcessAllJobs();

                var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id)
                    
                    .ToList();
                // then all process instances were restarted
                foreach (var restartedProcessInstance in restartedProcessInstances)
                {
                    var updatedTree = runtimeService.GetActivityInstance(restartedProcessInstance.Id);
                    Assert.NotNull(updatedTree);
                    Assert.AreEqual(restartedProcessInstance.Id, updatedTree.ProcessInstanceId);
                    Assert.AreEqual("tenantId", restartedProcessInstance.TenantId);

                    ActivityInstanceAssert.That(updatedTree)
                        .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processDefinition.Id)
                            .Activity("userTask2")
                            .Done());
                }
            }
            finally
            {
                processEngineConfiguration.SetAuthorizationEnabled(false);
            }
        }

        [Test]
        public virtual void restartProcessInstanceWithNotMatchingProcessDefinition()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");
            runtimeService.DeleteProcessInstance(processInstance.Id, null);
            var instance2 = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .Done();
            var processDefinition2 = testRule.DeployAndGetDefinition(instance2);

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition2.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            try
            {
                helper.CompleteBatch(batch);
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                // then
                Assert.That(e.Message,
                    Does.Contain("Its process definition '" + processDefinition.Id +
                                        "' does not match given process definition '" + processDefinition2.Id + "'"));
            }
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithoutBusinessKey()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process", "businessKey1", (string) null);
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process", "businessKey2", (string) null);

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .SetWithoutBusinessKey()
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance1 = restartedProcessInstances[0];
            var restartedProcessInstance2 = restartedProcessInstances[1];
            Assert.IsNull(restartedProcessInstance1.BusinessKey);
            Assert.IsNull(restartedProcessInstance2.BusinessKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithBusinessKey()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process", "businessKey1", (string) null);
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process", "businessKey2", (string) null);

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance1 = restartedProcessInstances[0];
            var restartedProcessInstance2 = restartedProcessInstances[1];
            Assert.NotNull(restartedProcessInstance1.BusinessKey);
            Assert.NotNull(restartedProcessInstance2.BusinessKey);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithoutCaseInstanceId()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process", null, "caseInstanceId1");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process", null, "caseInstanceId2");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==processDefinition.Id && c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            var restartedProcessInstance1 = restartedProcessInstances[0];
            var restartedProcessInstance2 = restartedProcessInstances[1];
            Assert.IsNull(restartedProcessInstance1.CaseInstanceId);
            Assert.IsNull(restartedProcessInstance2.CaseInstanceId);
        }

        [Test]
        public virtual void shouldRestartProcessInstanceWithTenant()
        {
            // given
            var processDefinition = testRule.DeployForTenantAndGetDefinition("tenantId", ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");


            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id&& c.SuspensionState == SuspensionStateFields.Active.StateCode)
                
                .ToList();
            Assert.NotNull(restartedProcessInstances[0].TenantId);
            Assert.NotNull(restartedProcessInstances[1].TenantId);
            Assert.AreEqual("tenantId", restartedProcessInstances[0].TenantId);
            Assert.AreEqual("tenantId", restartedProcessInstances[1].TenantId);
        }

        [Test]
        public virtual void shouldSkipCustomListeners()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var processDefinition =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
                    //.ActivityBuilder("userTask1")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    //    typeof(IncrementCounterListener).FullName)
                    //.Done()
                );
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            IncrementCounterListener.Counter = 0;
            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .SetSkipCustomListeners()
                .ExecuteAsync();

            helper.CompleteBatch(batch);
            // then
            Assert.AreEqual(0, IncrementCounterListener.Counter);
        }

        [Test]
        public virtual void shouldSkipIoMappings()
        {
            // given
            var processDefinition =
                testRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.TwoTasksProcess)
                    //.ActivityBuilder("userTask1")
                    //.CamundaInputParameter("foo", "bar")
                    //.Done()
                );
            var processInstance1 = runtimeService.StartProcessInstanceByKey("Process");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("Process");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .SetSkipIoMappings()
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==processDefinition.Id)
                
                .ToList();
            var task1Execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == restartedProcessInstances[0].Id&& c.ActivityId =="userTask1")
                .First();
            Assert.NotNull(task1Execution);
            Assert.IsNull(runtimeService.GetVariable(task1Execution.Id, "foo"));

            task1Execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == restartedProcessInstances[1].Id&& c.ActivityId =="userTask1")
                .First();
            Assert.NotNull(task1Execution);
            Assert.IsNull(runtimeService.GetVariable(task1Execution.Id, "foo"));
        }

        [Test]
        public virtual void shouldRetainTenantIdOfSharedProcessDefinition()
        {
            // given
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new TestTenantIdProvider());

            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity(ProcessModels.UserTaskId)
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId == processDefinition.Id)
                .First();

            Assert.NotNull(restartedInstance);
            Assert.AreEqual(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
        }

        [Test]
        public virtual void shouldSkipTenantIdProviderOnRestart()
        {
            // given
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new TestTenantIdProvider());

            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            Assert.AreEqual(processInstance.TenantId, TestTenantIdProvider.TENANT_ID);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");

            // set tenant id provider to Assert.Fail to verify it is not called during instantiation
            engineRule.ProcessEngineConfiguration.SetTenantIdProvider(new FailingTenantIdProvider());

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity(ProcessModels.UserTaskId)
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedInstance = runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode&& c.ProcessDefinitionId== processDefinition.Id)
                .First();

            Assert.NotNull(restartedInstance);
            Assert.AreEqual(restartedInstance.TenantId, TestTenantIdProvider.TENANT_ID);
        }

        [Test]
        public virtual void shouldNotSetInitialVariablesIfThereIsNoUniqueStartActivity()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(ProcessModels.TwoTasksProcess);
            var processInstance1 = runtimeService.CreateProcessInstanceById(processDefinition.Id)
                .StartBeforeActivity("userTask2")
                .StartBeforeActivity("userTask1")
                .Execute();

            var processInstance2 = runtimeService.CreateProcessInstanceById(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .StartBeforeActivity("userTask2")
                .SetVariable("foo", "bar")
                .Execute();

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("userTask1")
                .InitialSetOfVariables()
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();

            helper.CompleteBatch(batch);

            // then
            var restartedProcessInstances = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == processDefinition.Id)
                
                .ToList();
            //var variables = runtimeService.CreateVariableInstanceQuery(c=> c.ProcessInstanceId== restartedProcessInstances[0].Id, restartedProcessInstances[1].Id)
                
                //.ToList();
            //Assert.AreEqual(0, variables.Count);
        }

        protected internal virtual void AssertBatchCreated(IBatch batch, int processInstanceCount)
        {
            Assert.NotNull(batch);
            Assert.NotNull(batch.Id);
            Assert.AreEqual("instance-restart", batch.Type);
            Assert.AreEqual(processInstanceCount, batch.TotalJobs);
        }

        public class TestTenantIdProvider : FailingTenantIdProvider
        {
            internal const string TENANT_ID = "testTenantId";

            public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
            {
                return TENANT_ID;
            }
        }

        public class FailingTenantIdProvider : ITenantIdProvider
        {
            public string ProvideTenantIdForProcessInstance(TenantIdProviderProcessInstanceContext ctx)
            {
                throw new NotSupportedException();
            }

            public string ProvideTenantIdForCaseInstance(TenantIdProviderCaseInstanceContext ctx)
            {
                throw new NotSupportedException();
            }

            public string ProvideTenantIdForHistoricDecisionInstance(
                TenantIdProviderHistoricDecisionInstanceContext ctx)
            {
                throw new NotSupportedException();
            }
        }
    }
}