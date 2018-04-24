using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class ProcessInstantiationAtActivitiesHistoryTest : PluggableProcessEngineTestCase
    {
        protected internal const string PARALLEL_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ParallelGateway.bpmn20.xml";

        protected internal const string EXCLUSIVE_GATEWAY_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGateway.bpmn20.xml";

        protected internal const string SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.Subprocess.bpmn20.xml";

        protected internal const string ASYNC_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.ExclusiveGatewayAsyncTask.bpmn20.xml";

        [Test][Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testHistoricProcessInstanceForSingleActivityInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task1")
                .Execute();

            // then
            var historicInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.NotNull(historicInstance);
            Assert.AreEqual(instance.Id, historicInstance.Id);
            Assert.NotNull(historicInstance.StartTime);
            Assert.IsNull(historicInstance.EndTime);

            // should be the first activity started
            Assert.AreEqual("task1", historicInstance.StartActivityId);

            var historicActivityInstance = historyService.CreateHistoricActivityInstanceQuery()
                .First();
            Assert.NotNull(historicActivityInstance);
            Assert.AreEqual("task1", historicActivityInstance.ActivityId);
            Assert.NotNull(historicActivityInstance.Id);
            Assert.IsFalse(instance.Id.Equals(historicActivityInstance.Id));
            Assert.NotNull(historicActivityInstance.StartTime);
            Assert.IsNull(historicActivityInstance.EndTime);
        }

        [Test][Deployment(SUBPROCESS_PROCESS) ]
        public virtual void testHistoricActivityInstancesForSubprocess()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("subprocess")
                .StartBeforeActivity("innerTask")
                .StartBeforeActivity("theSubProcessStart")
                .Execute();

            // then
            var historicInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.NotNull(historicInstance);
            Assert.AreEqual(instance.Id, historicInstance.Id);
            Assert.NotNull(historicInstance.StartTime);
            Assert.IsNull(historicInstance.EndTime);

            // should be the first activity started
            Assert.AreEqual("innerTask", historicInstance.StartActivityId);

            // subprocess, subprocess start event, two innerTasks
            Assert.AreEqual(4, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            var subProcessInstance = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="subProcess")
                .First();
            Assert.NotNull(subProcessInstance);
            Assert.AreEqual("subProcess", subProcessInstance.ActivityId);
            Assert.NotNull(subProcessInstance.Id);
            Assert.IsFalse(instance.Id.Equals(subProcessInstance.Id));
            Assert.NotNull(subProcessInstance.StartTime);
            Assert.IsNull(subProcessInstance.EndTime);

            var startEventInstance = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="theSubProcessStart")
                .First();
            Assert.NotNull(startEventInstance);
            Assert.AreEqual("theSubProcessStart", startEventInstance.ActivityId);
            Assert.NotNull(startEventInstance.Id);
            Assert.IsFalse(instance.Id.Equals(startEventInstance.Id));
            Assert.NotNull(startEventInstance.StartTime);
            Assert.NotNull(startEventInstance.EndTime);

            var innerTaskInstances = historyService.CreateHistoricActivityInstanceQuery(c=> c.ActivityId =="innerTask")
                
                .ToList();

            Assert.AreEqual(2, innerTaskInstances.Count);

            foreach (var innerTaskInstance in innerTaskInstances)
            {
                Assert.NotNull(innerTaskInstance);
                Assert.AreEqual("innerTask", innerTaskInstance.ActivityId);
                Assert.NotNull(innerTaskInstance.Id);
                Assert.IsFalse(instance.Id.Equals(innerTaskInstance.Id));
                Assert.NotNull(innerTaskInstance.StartTime);
                Assert.IsNull(innerTaskInstance.EndTime);
            }
        }

        [Test][Deployment( ASYNC_PROCESS) ]
        public virtual void testHistoricProcessInstanceAsyncStartEvent()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task2")
                .SetVariable("aVar", "aValue")
                .Execute();

            // then
            var historicInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.NotNull(historicInstance);
            Assert.AreEqual(instance.Id, historicInstance.Id);
            Assert.NotNull(historicInstance.StartTime);
            Assert.IsNull(historicInstance.EndTime);

            // should be the first activity started
            Assert.AreEqual("task2", historicInstance.StartActivityId);

            // task2 wasn't entered yet
            Assert.AreEqual(0, historyService.CreateHistoricActivityInstanceQuery()
                .Count());

            // history events for variables exist already
            var activityInstance = runtimeService.GetActivityInstance(instance.Id);

            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("aVar")
                .First();

            Assert.NotNull(historicVariable);
            Assert.AreEqual(instance.Id, historicVariable.ProcessInstanceId);
            Assert.AreEqual(activityInstance.Id, historicVariable.ActivityInstanceId);
            Assert.AreEqual("aVar", historicVariable.Name);
            Assert.AreEqual("aValue", historicVariable.Value);

            var historicDetail = historyService.CreateHistoricDetailQuery()
                ////.VariableInstanceId(historicVariable.Id)
                .First();
            Assert.AreEqual(instance.Id, historicDetail.ProcessInstanceId);
            Assert.NotNull(historicDetail);
            // TODO: fix if this is not ok due to CAM-3886
            Assert.IsNull(historicDetail.ActivityInstanceId);
            Assert.True(historicDetail is IHistoricVariableUpdate);
            Assert.AreEqual("aVar", ((IHistoricVariableUpdate) historicDetail).VariableName);
            Assert.AreEqual("aValue", ((IHistoricVariableUpdate) historicDetail).Value);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testHistoricVariableInstanceForSingleActivityInstantiation()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartBeforeActivity("task1")
                .SetVariable("aVar", "aValue")
                .Execute();

            var activityInstance = runtimeService.GetActivityInstance(instance.Id);

            // then
            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("aVar")
                .First();

            Assert.NotNull(historicVariable);
            Assert.AreEqual(instance.Id, historicVariable.ProcessInstanceId);
            Assert.AreEqual(activityInstance.Id, historicVariable.ActivityInstanceId);
            Assert.AreEqual("aVar", historicVariable.Name);
            Assert.AreEqual("aValue", historicVariable.Value);

            var historicDetail = historyService.CreateHistoricDetailQuery()
                ////.VariableInstanceId(historicVariable.Id)
                .First();
            Assert.AreEqual(instance.Id, historicDetail.ProcessInstanceId);
            Assert.NotNull(historicDetail);
            // TODO: fix if this is not ok due to CAM-3886
            Assert.IsNull(historicDetail.ActivityInstanceId);
            Assert.True(historicDetail is IHistoricVariableUpdate);
            Assert.AreEqual("aVar", ((IHistoricVariableUpdate) historicDetail).VariableName);
            Assert.AreEqual("aValue", ((IHistoricVariableUpdate) historicDetail).Value);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testHistoricVariableInstanceSetOnProcessInstance()
        {
            // when
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .SetVariable("aVar", "aValue")
                .StartBeforeActivity("task1")
                .Execute();
            // set the variables directly one the instance

            var activityInstance = runtimeService.GetActivityInstance(instance.Id);

            // then
            var historicVariable = historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("aVar")
                .First();

            Assert.NotNull(historicVariable);
            Assert.AreEqual(instance.Id, historicVariable.ProcessInstanceId);
            Assert.AreEqual(activityInstance.Id, historicVariable.ActivityInstanceId);
            Assert.AreEqual("aVar", historicVariable.Name);
            Assert.AreEqual("aValue", historicVariable.Value);

            var historicDetail = historyService.CreateHistoricDetailQuery()
                ////.VariableInstanceId(historicVariable.Id)
                .First();
            Assert.AreEqual(instance.Id, historicDetail.ProcessInstanceId);
            Assert.NotNull(historicDetail);
            // TODO: fix if this is not ok due to CAM-3886
            Assert.AreEqual(instance.Id, historicDetail.ActivityInstanceId);
            Assert.True(historicDetail is IHistoricVariableUpdate);
            Assert.AreEqual("aVar", ((IHistoricVariableUpdate) historicDetail).VariableName);
            Assert.AreEqual("aValue", ((IHistoricVariableUpdate) historicDetail).Value);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testHistoricProcessInstanceForSynchronousCompletion()
        {
            // when the process instance ends immediately
            var instance = runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
                .StartAfterActivity("task1")
                .Execute();

            // then
            var historicInstance = historyService.CreateHistoricProcessInstanceQuery()
                .First();
            Assert.NotNull(historicInstance);
            Assert.AreEqual(instance.Id, historicInstance.Id);
            Assert.NotNull(historicInstance.StartTime);
            Assert.NotNull(historicInstance.EndTime);

            Assert.AreEqual("join", historicInstance.StartActivityId);
        }

        [Test]
        [Deployment(EXCLUSIVE_GATEWAY_PROCESS)]
        public virtual void testSkipCustomListenerEnsureHistoryWritten()
        {
            // when creating the task skipping custom listeners
            runtimeService.CreateProcessInstanceByKey("exclusiveGateway")
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