using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class ProcessInstanceModificationHistoryTest : PluggableProcessEngineTestCase
    {
        protected internal const string OneTaskProcess = "resources/api/oneTaskProcess.bpmn20.xml";

        protected internal const string EXCLUSIVE_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGateway.bpmn20.xml";

        protected internal const string EXCLUSIVE_GATEWAY_ASYNC_TASK_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGatewayAsyncTask.bpmn20.xml";

        protected internal const string SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.Subprocess.bpmn20.xml";

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS) ]
        public virtual void testStartBeforeWithVariablesInHistory()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .SetVariable("procInstVar", "procInstValue")
                .SetVariableLocal("localVar", "localValue")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);

            var procInstVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("procInstVar")
                .First();

            Assert.NotNull(procInstVariable);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);
            Assert.AreEqual("procInstVar", procInstVariable.Name);
            Assert.AreEqual("procInstValue", procInstVariable.Value);

            var procInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(procInstVariable.Id)
                .First();
            Assert.NotNull(procInstanceVarDetail);
            // when starting before/after an activity instance, the activity instance id of the
            // execution is null and so is the activity instance id of the historic detail
            Assert.IsNull(procInstanceVarDetail.ActivityInstanceId);

            var localVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("localVar")
                .First();

            Assert.NotNull(localVariable);
            Assert.IsNull(localVariable.ActivityInstanceId);
            Assert.AreEqual("localVar", localVariable.Name);
            Assert.AreEqual("localValue", localVariable.Value);

            var localInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(localVariable.Id)
                .First();
            Assert.NotNull(localInstanceVarDetail);
            Assert.IsNull(localInstanceVarDetail.ActivityInstanceId);

            completeTasksInOrder("task1", "task2");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_ASYNC_TASK_PROCESS) ]
        public virtual void testStartBeforeAsyncWithVariablesInHistory()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task2")
                .SetVariable("procInstVar", "procInstValue")
                .SetVariableLocal("localVar", "localValue")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);

            var procInstVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("procInstVar")
                .First();

            Assert.NotNull(procInstVariable);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);
            Assert.AreEqual("procInstVar", procInstVariable.Name);
            Assert.AreEqual("procInstValue", procInstVariable.Value);

            var procInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(procInstVariable.Id)
                .First();
            Assert.NotNull(procInstanceVarDetail);
            // when starting before/after an activity instance, the activity instance id of the
            // execution is null and so is the activity instance id of the historic detail
            Assert.IsNull(procInstanceVarDetail.ActivityInstanceId);

            var localVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("localVar")
                .First();

            Assert.NotNull(localVariable);
            // the following is null because localVariable is local on a concurrent execution
            // but the concurrent execution does not execute an activity at the time the variable is set
            Assert.IsNull(localVariable.ActivityInstanceId);
            Assert.AreEqual("localVar", localVariable.Name);
            Assert.AreEqual("localValue", localVariable.Value);

            var localInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(localVariable.Id)
                .First();
            Assert.NotNull(localInstanceVarDetail);
            Assert.IsNull(localInstanceVarDetail.ActivityInstanceId);

            // end process instance
            completeTasksInOrder("task1");

            var job = managementService.CreateJobQuery()
                .First();
            managementService.ExecuteJob(job.Id);

            completeTasksInOrder("task2");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(SUBPROCESS_PROCESS)]
        public virtual void testStartBeforeScopeWithVariablesInHistory()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("subprocess");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("innerTask")
                .SetVariable("procInstVar", "procInstValue")
                .SetVariableLocal("localVar", "localValue")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);

            var procInstVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("procInstVar")
                .First();

            Assert.NotNull(procInstVariable);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);
            Assert.AreEqual("procInstVar", procInstVariable.Name);
            Assert.AreEqual("procInstValue", procInstVariable.Value);

            var procInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(procInstVariable.Id)
                .First();
            Assert.NotNull(procInstanceVarDetail);
            // when starting before/after an activity instance, the activity instance id of the
            // execution is null and so is the activity instance id of the historic detail
            Assert.IsNull(procInstanceVarDetail.ActivityInstanceId);

            var localVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("localVar")
                .First();

            Assert.NotNull(localVariable);
            Assert.AreEqual(updatedTree.GetActivityInstances("subProcess")[0].Id, localVariable.ActivityInstanceId);
            Assert.AreEqual("localVar", localVariable.Name);
            Assert.AreEqual("localValue", localVariable.Value);

            var localInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(localVariable.Id)
                .First();
            Assert.NotNull(localInstanceVarDetail);
            Assert.IsNull(localInstanceVarDetail.ActivityInstanceId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testStartTransitionWithVariablesInHistory()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exclusiveGateway");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartTransition("flow2")
                .SetVariable("procInstVar", "procInstValue")
                .SetVariableLocal("localVar", "localValue")
                .Execute();

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);

            var procInstVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("procInstVar")
                .First();

            Assert.NotNull(procInstVariable);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);
            Assert.AreEqual("procInstVar", procInstVariable.Name);
            Assert.AreEqual("procInstValue", procInstVariable.Value);

            var procInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(procInstVariable.Id)
                .First();
            Assert.NotNull(procInstanceVarDetail);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);

            var localVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("localVar")
                .First();

            Assert.NotNull(localVariable);
            Assert.AreEqual(updatedTree.Id, procInstVariable.ActivityInstanceId);
            Assert.AreEqual("localVar", localVariable.Name);
            Assert.AreEqual("localValue", localVariable.Value);

            var localInstanceVarDetail = historyService.CreateHistoricDetailQuery()
                //.VariableInstanceId(localVariable.Id)
                .First();
            Assert.NotNull(localInstanceVarDetail);
            // when starting before/after an activity instance, the activity instance id of the
            // execution is null and so is the activity instance id of the historic detail
            Assert.IsNull(localInstanceVarDetail.ActivityInstanceId);

            completeTasksInOrder("task1", "task1");
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment(OneTaskProcess)]
        public virtual void testCancelTaskShouldCancelProcessInstance()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess")
                .Id;

            // when
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .CancelAllForActivity("theTask")
                .Execute(true, false);

            // then
            var instance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.NotNull(instance);

            Assert.AreEqual(ProcessInstanceId, instance.Id);
            Assert.NotNull(instance.EndTime);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSkipCustomListenerEnsureHistoryWritten()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("exclusiveGateway")
                .Id;

            // when creating the task skipping custom listeners
            runtimeService.CreateProcessInstanceModification(ProcessInstanceId)
                .StartBeforeActivity("task2")
                .Execute(true, false);

            // then the task assignment history (which uses a task listener) is written
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();

            var instance = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="task2")
                .First();
            Assert.NotNull(instance);
            Assert.AreEqual(task.Id, instance.TaskId);
            Assert.AreEqual("kermit", instance.Assignee);
        }

        protected internal virtual IActivityInstance getChildInstanceForActivity(IActivityInstance activityInstance,
            string activityId)
        {
            if (activityId.Equals(activityInstance.ActivityId))
                return activityInstance;

            foreach (var childInstance in activityInstance.ChildActivityInstances)
            {
                var instance = getChildInstanceForActivity(childInstance, activityId);
                if (instance != null)
                    return instance;
            }

            return null;
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