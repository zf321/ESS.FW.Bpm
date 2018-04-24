using System.Linq;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessInstantiationAtActivitiesTest : PluggableProcessEngineTestCase
    {
        protected internal const string PARALLEL_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ParallelGateway.bpmn20.xml";

        protected internal const string EXCLUSIVE_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGateway.bpmn20.xml";

        protected internal const string SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.Subprocess.bpmn20.xml";

        protected internal const string LISTENERS_PROCESS =
            "resources/api/runtime/ProcessInstantiationAtActivitiesTest.listeners.bpmn20.xml";

        protected internal const string IO_PROCESS =
            "resources/api/runtime/ProcessInstantiationAtActivitiesTest.ioMappings.bpmn20.xml";

        protected internal const string ASYNC_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGatewayAsyncTask.bpmn20.xml";

        protected internal const string SYNC_PROCESS =
            "resources/api/runtime/ProcessInstantiationAtActivitiesTest.synchronous.bpmn20.xml";

        [Test][Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSingleActivityInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task1")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task1")
                    .Done());

            // and it is possible to end the process
            completeTasksInOrder("task1");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSingleActivityInstantiationById()
        {
            // given
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            // when
            var instance = runtimeService.CreateProcessInstanceById(processDefinitionId)
                .StartBeforeActivity("task1")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task1")
                    .Done());

            // and it is possible to end the process
            completeTasksInOrder("task1");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSingleActivityInstantiationSetBusinessKey()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .SetBusinessKey("businessKey")
                .StartBeforeActivity("task1")
                .Execute();

            // then
            Assert.NotNull(instance);
            Assert.AreEqual("businessKey", instance.BusinessKey);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSingleActivityInstantiationSetCaseInstanceId()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .SetCaseInstanceId("caseInstanceId")
                .StartBeforeActivity("task1")
                .Execute();

            // then
            Assert.NotNull(instance);
            Assert.AreEqual("caseInstanceId", instance.CaseInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartEventInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("theStart")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task1")
                    .Done());

            // and it is possible to end the process
            completeTasksInOrder("task1");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartEventInstantiationWithVariables()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("theStart")
                .SetVariable("aVariable", "aValue")
                .Execute();

            // then
            Assert.NotNull(instance);

            Assert.AreEqual("aValue", runtimeService.GetVariable(instance.Id, "aVariable"));
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartWithInvalidInitialActivity()
        {
            try
            {
                // when
                runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                    .StartBeforeActivity("someNonExistingActivity")
                    .Execute();
                Assert.Fail("should not succeed");
            }
            catch (NotValidException e)
            {
                // then
                AssertTextPresentIgnoreCase("element 'someNonExistingActivity' does not exist in process ", e.Message);
            }
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testMultipleActivitiesInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task1")
                .StartBeforeActivity("task2")
                .StartBeforeActivity("task1")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("task1")
                    .Activity("task2")
                    .Activity("task1")
                    .Done());

            // and it is possible to end the process
            completeTasksInOrder("task1", "task2", "task1");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testMultipleActivitiesInstantiationWithVariables()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task1")
                .SetVariableLocal("aVar1", "aValue1")
                .StartBeforeActivity("task2")
                .SetVariableLocal("aVar2", "aValue2")
                .Execute();

            // then
            // variables for task2's execution
            var task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2")
                .First();
            Assert.NotNull(task2Execution);
            Assert.IsNull(runtimeService.GetVariableLocal(task2Execution.Id, "aVar1"));
            Assert.AreEqual("aValue2", runtimeService.GetVariableLocal(task2Execution.Id, "aVar2"));

            // variables for task1's execution
            var task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1")
                .First();
            Assert.NotNull(task1Execution);

            Assert.IsNull(runtimeService.GetVariableLocal(task1Execution.Id, "aVar2"));

            // this variable is not a local variable on execution1 due to tree expansion
            Assert.IsNull(runtimeService.GetVariableLocal(task1Execution.Id, "aVar1"));
            Assert.AreEqual("aValue1", runtimeService.GetVariable(task1Execution.Id, "aVar1"));
        }

        [Test]
        [Deployment(SUBPROCESS_PROCESS) ]
        public virtual void testNestedActivitiesInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("subprocess")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("outerTask")
                .StartBeforeActivity("innerTask")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Activity("innerTask")
                    .Done());

            // and it is possible to end the process
            completeTasksInOrder("innerTask", "innerTask", "outerTask", "innerTask");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        public virtual void testStartNonExistingProcessDefinition()
        {
            try
            {
                runtimeService.CreateProcessInstanceById("I don't exist")
                    .StartBeforeActivity("start")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("no deployed process definition found with id", e.Message);
            }

            try
            {
                runtimeService.CreateProcessInstanceByKey("I don't exist either")
                    .StartBeforeActivity("start")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("no processes deployed with key", e.Message);
            }
        }

        [Test]
        public virtual void testStartNullProcessDefinition()
        {
            try
            {
                runtimeService.CreateProcessInstanceById(null)
                    .StartBeforeActivity("start")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }

            try
            {
                runtimeService.CreateProcessInstanceByKey(null)
                    .StartBeforeActivity("start")
                    .Execute();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException)
            {
                // happy path
            }
        }
        [Test]
        [Deployment(LISTENERS_PROCESS)]
        public virtual void testListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            // when
            var instance = runtimeService.CreateProcessInstanceByKey("listenerProcess")
                .StartBeforeActivity("innerTask")
                .Execute();

            // then
            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            var events = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(3, events.Count);

            var processStartEvent = events[0];
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, processStartEvent.EventName);
            Assert.AreEqual("innerTask", processStartEvent.ActivityId);

            var subProcessStartEvent = events[1];
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, subProcessStartEvent.EventName);
            Assert.AreEqual("subProcess", subProcessStartEvent.ActivityId);

            var innerTaskStartEvent = events[2];
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, innerTaskStartEvent.EventName);
            Assert.AreEqual("innerTask", innerTaskStartEvent.ActivityId);
        }

        [Test]
        [Deployment(LISTENERS_PROCESS)]
        public virtual void testSkipListenerInvocation()
        {
            RecorderExecutionListener.Clear();

            // when
            var instance = runtimeService.CreateProcessInstanceByKey("listenerProcess")
                .StartBeforeActivity("innerTask")
                .Execute(true, true);

            // then
            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());

            var events = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(0, events.Count);
        }

        [Test]
        [Deployment(IO_PROCESS)]
        public virtual void testIoMappingInvocation()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("ioProcess")
                .StartBeforeActivity("innerTask")
                .Execute();

            // then no io mappings have been executed
            var variables = runtimeService.CreateVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/
                
                .ToList();
            Assert.AreEqual(2, variables.Count);

            var innerTaskExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "innerTask")
                .First();
            var innerTaskVariable = variables[0];
            Assert.AreEqual("innerTaskVariable", innerTaskVariable.Name);
            Assert.AreEqual("innerTaskValue", innerTaskVariable.Value);
            Assert.AreEqual(innerTaskExecution.Id, innerTaskVariable.ExecutionId);

            var subProcessVariable = variables[1];
            Assert.AreEqual("subProcessVariable", subProcessVariable.Name);
            Assert.AreEqual("subProcessValue", subProcessVariable.Value);
            Assert.AreEqual(((ExecutionEntity) innerTaskExecution).ParentId, subProcessVariable.ExecutionId);
        }

        [Test]
        [Deployment(IO_PROCESS)]
        public virtual void testSkipIoMappingInvocation()
        {
            // when
            runtimeService.CreateProcessInstanceByKey("ioProcess")
                .StartBeforeActivity("innerTask")
                .Execute(true, true);

            // then no io mappings have been executed
            Assert.AreEqual(0, runtimeService.CreateVariableInstanceQuery()
                .Count());
        }

        [Test]
        [Deployment(SUBPROCESS_PROCESS)]
        public virtual void testSetProcessInstanceVariable()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("subprocess")
                .SetVariable("aVariable1", "aValue1")
                .SetVariableLocal("aVariable2", "aValue2")
                //.SetVariables(Variable.Variables.CreateVariables()
                //    .PutValue("aVariable3", "aValue3"))
                //.SetVariablesLocal(Variable.Variables.CreateVariables()
                //    .PutValue("aVariable4", "aValue4"))
                .StartBeforeActivity("innerTask")
                .Execute();

            // then
            var variables = runtimeService.CreateVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(4, variables.Count);
            Assert.AreEqual("aVariable1", variables[0].Name);
            Assert.AreEqual("aValue1", variables[0].Value);
            Assert.AreEqual(instance.Id, variables[0].ExecutionId);

            Assert.AreEqual("aVariable2", variables[1].Name);
            Assert.AreEqual("aValue2", variables[1].Value);
            Assert.AreEqual(instance.Id, variables[1].ExecutionId);

            Assert.AreEqual("aVariable3", variables[2].Name);
            Assert.AreEqual("aValue3", variables[2].Value);
            Assert.AreEqual(instance.Id, variables[2].ExecutionId);

            Assert.AreEqual("aVariable4", variables[3].Name);
            Assert.AreEqual("aValue4", variables[3].Value);
            Assert.AreEqual(instance.Id, variables[3].ExecutionId);
        }

        [Test]
        [Deployment(ASYNC_PROCESS)]
        public virtual void testStartAsyncTask()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task2")
                .Execute();

            // then
            Assert.NotNull(instance);

            var updatedTree = runtimeService.GetActivityInstance(instance.Id);
            Assert.NotNull(updatedTree);

            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(instance.ProcessDefinitionId)
                    .Transition("task2")
                    .Done());

            // and it is possible to end the process
            var job = managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            managementService.ExecuteJob(job.Id);

            completeTasksInOrder("task2");
            AssertProcessEnded(instance.Id);
        }

        [Test]
        [Deployment(SYNC_PROCESS)]
        public virtual void testStartMultipleTasksInSyncProcess()
        {
            RecorderExecutionListener.Clear();

            // when
            var instance = runtimeService.CreateProcessInstanceByKey("syncProcess")
                .StartBeforeActivity("syncTask")
                .StartBeforeActivity("syncTask")
                .StartBeforeActivity("syncTask")
                .Execute();

            // then the request was successful even though the process instance has already ended
            Assert.NotNull(instance);
            AssertProcessEnded(instance.Id);

            // and the execution listener was invoked correctly
            var events = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(8, events.Count);

            // process start event
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, events[0].EventName);
            Assert.AreEqual("syncTask", events[0].ActivityId);

            // start instruction 1
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, events[1].EventName);
            Assert.AreEqual("syncTask", events[1].ActivityId);
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, events[2].EventName);
            Assert.AreEqual("syncTask", events[2].ActivityId);

            // start instruction 2
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, events[3].EventName);
            Assert.AreEqual("syncTask", events[3].ActivityId);
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, events[4].EventName);
            Assert.AreEqual("syncTask", events[4].ActivityId);

            // start instruction 3
            Assert.AreEqual(ExecutionListenerFields.EventNameStart, events[5].EventName);
            Assert.AreEqual("syncTask", events[5].ActivityId);
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, events[6].EventName);
            Assert.AreEqual("syncTask", events[6].ActivityId);

            // process end event
            Assert.AreEqual(ExecutionListenerFields.EventNameEnd, events[7].EventName);
            Assert.AreEqual("end", events[7].ActivityId);
        }

        [Test]
        [Deployment]
        public virtual void testInitiatorVariable()
        {
            // given
            identityService.AuthenticatedUserId = "kermit";

            // when
            var instance = runtimeService.CreateProcessInstanceByKey("initiatorProcess")
                .StartBeforeActivity("task")
                .Execute();

            // then
            var initiator = (string) runtimeService.GetVariable(instance.Id, "initiator");
            Assert.AreEqual("kermit", initiator);

            identityService.ClearAuthentication();
        }

        protected internal virtual void completeTasksInOrder(params string[] taskNames)
        {
            foreach (var taskName in taskNames)
            {
                // complete any task with that name
                var tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey==taskName)
                    /*.ListPage(0, 1)*/
                    .ToList();
                Assert.True(tasks.Count > 0, "task for activity " + taskName + " does not exist");
                taskService.Complete(tasks[0].Id);
            }
        }
    }
}