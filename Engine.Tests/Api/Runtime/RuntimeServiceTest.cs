using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Tests.Api.Runtime.Util;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Bpmn.TaskListener.Util;
using Engine.Tests.History;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Type;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class RuntimeServiceTest : PluggableProcessEngineTestCase
    {
        public const string TESTING_INSTANCE_DELETION = "testing instance deletion";
        public const string A_STREAM = "aStream";

        [Test]
        public virtual void testStartProcessInstanceByKeyNullKey()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Expected exception
            }
        }

        [Test]
        public virtual void testStartProcessInstanceByKeyUnexistingKey()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("unexistingkey");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("no processes deployed with key", ae.Message);
            }
        }

        [Test]
        public virtual void testStartProcessInstanceByIdNullId()
        {
            try
            {
                runtimeService.StartProcessInstanceById(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Expected exception
            }
        }

        [Test]
        public virtual void testStartProcessInstanceByIdUnexistingId()
        {
            try
            {
                runtimeService.StartProcessInstanceById("unexistingId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("no deployed process definition found with id", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testStartProcessInstanceByIdNullVariables()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", (IDictionary<string, object>) null);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void startProcessInstanceWithBusinessKey()
        {
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // by key
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "123");
            Assert.NotNull(processInstance);
            Assert.AreEqual("123", processInstance.BusinessKey);
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());

            // by key with variables
            processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "456"
                //CollectionUtil.singletonMap("var", "value")
            );
            Assert.NotNull(processInstance);
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());
            Assert.AreEqual("var", runtimeService.GetVariable(processInstance.Id, "var"));

            // by id
            processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id, "789");
            Assert.NotNull(processInstance);
            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());

            // by id with variables
            processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id, "101123"
                //CollectionUtil.singletonMap("var", "value2")
            );
            Assert.NotNull(processInstance);
            Assert.AreEqual(4, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());
            Assert.AreEqual("var", runtimeService.GetVariable(processInstance.Id, "var"));
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstance()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());

            runtimeService.DeleteProcessInstance(processInstance.Id, TESTING_INSTANCE_DELETION);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());

            // test that the Delete reason of the process instance shows up as Delete reason of the task in history
            // ACT-848
            if (!ProcessEngineConfiguration.HistoryNone.Equals(processEngineConfiguration.History))
            {
                var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery(c=>c.ProcessInstanceId ==processInstance.Id)
                    .First();

                Assert.AreEqual(TESTING_INSTANCE_DELETION, historicTaskInstance.DeleteReason);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstances()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // if we skip the custom listeners,
            runtimeService.DeleteProcessInstances(new[] {processInstance.Id, processInstance2.Id}, null, false, false);

            Assert.That(runtimeService.CreateProcessInstanceQuery()
                .Count(), Is.EqualTo(0l));
        }

        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceWithListeners()
        {
            RecorderExecutionListener.Clear();

            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("nestedParallelGatewayScopeTasks");

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, "");

            // then
            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(10, recordedEvents.Count);

            ISet<RecorderExecutionListener.RecordedEvent> startEvents =
                new HashSet<RecorderExecutionListener.RecordedEvent>();
            ISet<RecorderExecutionListener.RecordedEvent> endEvents =
                new HashSet<RecorderExecutionListener.RecordedEvent>();
            foreach (var @event in recordedEvents)
                if (@event.EventName.Equals(ExecutionListenerFields.EventNameStart))
                    startEvents.Add(@event);
                else if (@event.EventName.Equals(ExecutionListenerFields.EventNameEnd))
                    endEvents.Add(@event);

            //Assert.That(startEvents, hasSize(5));
            //Assert.That(endEvents, hasSize(5));
            foreach (var startEvent in startEvents)
            foreach (var endEvent in endEvents)
                if (startEvent.ActivityId.Equals(endEvent.ActivityId))
                {
                    Assert.That(startEvent.ActivityInstanceId, Is.EqualTo(endEvent.ActivityInstanceId));
                    Assert.That(startEvent.ExecutionId, Is.EqualTo(endEvent.ExecutionId));
                }
            foreach (var recordedEvent in endEvents)
            {
                //Assert.That(recordedEvent.ActivityId,
                //    Is.EqualTo(anyOf(equalTo("innerTask1"), equalTo("innerTask2"), equalTo("outerTask"),
                //        equalTo("subProcess"), null)));
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstanceSkipCustomListenersEnsureHistoryWritten()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // if we skip the custom listeners,
            runtimeService.DeleteProcessInstance(processInstance.Id, null, true);

            // buit-in listeners are still invoked and thus history is written
            if (!ProcessEngineConfiguration.HistoryNone.Equals(processEngineConfiguration.History))
            {
                // verify that all historic activity instances are ended
                var hais = historyService.CreateHistoricActivityInstanceQuery()
                    
                    .ToList();
                foreach (var hai in hais)
                    Assert.NotNull(hai.EndTime);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceSkipCustomListeners()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // if we do not skip the custom listeners,
            runtimeService.DeleteProcessInstance(processInstance.Id, null, false);
            // the custom listener is invoked
            Assert.True(TestExecutionListener.CollectedEvents.Count == 1);
            TestExecutionListener.Reset();

            processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // if we DO skip the custom listeners,
            runtimeService.DeleteProcessInstance(processInstance.Id, null, true);
            // the custom listener is not invoked
            Assert.True(TestExecutionListener.CollectedEvents.Count == 0);
            TestExecutionListener.Reset();
        }

        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceSkipCustomListenersScope()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // if we do not skip the custom listeners,
            runtimeService.DeleteProcessInstance(processInstance.Id, null, false);
            // the custom listener is invoked
            Assert.True(TestExecutionListener.CollectedEvents.Count == 1);
            TestExecutionListener.Reset();

            processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

            // if we DO skip the custom listeners,
            runtimeService.DeleteProcessInstance(processInstance.Id, null, true);
            // the custom listener is not invoked
            Assert.True(TestExecutionListener.CollectedEvents.Count == 0);
            TestExecutionListener.Reset();
        }

        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceSkipCustomTaskListeners()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // and an empty task listener invocation storage
            RecorderTaskListener.clear();

            // if we do not skip the custom listeners
            runtimeService.DeleteProcessInstance(instance.Id, null, false);

            // then the the custom listener is invoked
            Assert.AreEqual(1, RecorderTaskListener.RecordedEvents.Count);
            Assert.AreEqual(TaskListenerFields.EventnameDelete, RecorderTaskListener.RecordedEvents[0].Event);

            // if we do skip the custom listeners
            instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            RecorderTaskListener.clear();

            runtimeService.DeleteProcessInstance(instance.Id, null, true);

            // then the the custom listener is not invoked
            Assert.True(RecorderTaskListener.RecordedEvents.Count == 0);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testDeleteProcessInstanceNullReason()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());

            // Deleting without a reason should be possible
            runtimeService.DeleteProcessInstance(processInstance.Id, null);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("oneTaskProcess")
                .Count());
        }

        [Test]
        public virtual void testDeleteProcessInstanceUnexistingId()
        {
            try
            {
                runtimeService.DeleteProcessInstance("enexistingInstanceId", null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("No process instance found for id", ae.Message);
                Assert.True(ae is BadUserRequestException);
            }
        }


        [Test]
        public virtual void testDeleteProcessInstanceNullId()
        {
            try
            {
                runtimeService.DeleteProcessInstance(null, "test null id Delete");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("ProcessInstanceId is null", ae.Message);
                Assert.True(ae is BadUserRequestException);
            }
        }

        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceWithActiveCompensation()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("compensationProcess");

            var innerTask = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(innerTask.Id);

            var afterSubProcessTask = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("taskAfterSubprocess", afterSubProcessTask.TaskDefinitionKey);
            taskService.Complete(afterSubProcessTask.Id);

            // when
            // there are two compensation tasks
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "outerAfterBoundaryTask")
                .Count());
            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "innerAfterBoundaryTask")
                .Count());

            // when the process instance is deleted
            runtimeService.DeleteProcessInstance(instance.Id, "");

            // then
            AssertProcessEnded(instance.Id);
        }


        [Test]
        [Deployment]
        public virtual void testDeleteProcessInstanceWithVariableOnScopeAndConcurrentExecution()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("task")
                .Execute();

            var executions = runtimeService.CreateExecutionQuery()
                
                .ToList();

            foreach (var execution in executions)
                runtimeService.SetVariableLocal(execution.Id, "foo", "bar");

            // when
            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // then
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testFindActiveActivityIds()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var activities = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.NotNull(activities);
            Assert.AreEqual(1, activities.Count);
        }

        [Test]
        public virtual void testFindActiveActivityIdsUnexistingExecututionId()
        {
            try
            {
                runtimeService.GetActiveActivityIds("unexistingExecutionId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
            }
        }

        [Test]
        public virtual void testFindActiveActivityIdsNullExecututionId()
        {
            try
            {
                runtimeService.GetActiveActivityIds(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        /// <summary>
        ///     Testcase to reproduce ACT-950 (https://jira.codehaus.org/browse/ACT-950)
        /// </summary>
        [Test]
        [Deployment]
        public virtual void testFindActiveActivityIdProcessWithErrorEventAndSubProcess()
        {
            var processInstance = ProcessEngine.RuntimeService.StartProcessInstanceByKey("errorEventSubprocess");

            var activeActivities = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.AreEqual(3, activeActivities.Count);

            var tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            ITask parallelUserTask = null;
            foreach (var task in tasks)
            {
                if (!task.Name.Equals("ParallelUserTask") && !task.Name.Equals("MainUserTask"))
                    Assert.Fail("Expected: <ParallelUserTask> or <MainUserTask> but was <" + task.Name + ">.");
                if (task.Name.Equals("ParallelUserTask"))
                    parallelUserTask = task;
            }
            Assert.NotNull(parallelUserTask);

            taskService.Complete(parallelUserTask.Id);

            var execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == processInstance.Id && c.ActivityId =="subprocess1WaitBeforeError")
                .First();
            runtimeService.Signal(execution.Id);

            activeActivities = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.AreEqual(2, activeActivities.Count);

            tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            ITask beforeErrorUserTask = null;
            foreach (var task in tasks)
            {
                if (!task.Name.Equals("BeforeError") && !task.Name.Equals("MainUserTask"))
                    Assert.Fail("Expected: <BeforeError> or <MainUserTask> but was <" + task.Name + ">.");
                if (task.Name.Equals("BeforeError"))
                    beforeErrorUserTask = task;
            }
            Assert.NotNull(beforeErrorUserTask);

            taskService.Complete(beforeErrorUserTask.Id);

            activeActivities = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.AreEqual(2, activeActivities.Count);

            tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(2, tasks.Count);

            ITask afterErrorUserTask = null;
            foreach (var task in tasks)
            {
                if (!task.Name.Equals("AfterError") && !task.Name.Equals("MainUserTask"))
                    Assert.Fail("Expected: <AfterError> or <MainUserTask> but was <" + task.Name + ">.");
                if (task.Name.Equals("AfterError"))
                    afterErrorUserTask = task;
            }
            Assert.NotNull(afterErrorUserTask);

            taskService.Complete(afterErrorUserTask.Id);

            tasks = taskService.CreateTaskQuery()
                
                .ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("MainUserTask", tasks[0].Name);

            activeActivities = runtimeService.GetActiveActivityIds(processInstance.Id);
            Assert.AreEqual(1, activeActivities.Count);
            Assert.AreEqual("MainUserTask", activeActivities[0]);

            taskService.Complete(tasks[0].Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testSignalUnexistingExecututionId()
        {
            try
            {
                runtimeService.Signal("unexistingExecutionId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
                Assert.True(ae is BadUserRequestException);
            }
        }

        [Test]
        public virtual void testSignalNullExecutionId()
        {
            try
            {
                runtimeService.Signal(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
                Assert.True(ae is BadUserRequestException);
            }
        }

        [Test]
        [Deployment]
        public virtual void testSignalWithProcessVariables()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testSignalWithProcessVariables");
            IDictionary<string, object> processVariables = new Dictionary<string, object>();
            processVariables["variable"] = "value";

            // signal the execution while passing in the variables
            runtimeService.Signal(processInstance.Id, processVariables);

            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(variables, processVariables);
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/RuntimeServiceTest.TestSignalWithProcessVariables.bpmn20.xml"}) ]
        public virtual void testSignalWithSignalNameAndData()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testSignalWithProcessVariables");
            IDictionary<string, object> processVariables = new Dictionary<string, object>();
            processVariables["variable"] = "value";

            // signal the execution while passing in the variables
            runtimeService.Signal(processInstance.Id, "dummySignalName", "SignalData", processVariables);

            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(variables, processVariables);
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/RuntimeServiceTest.TestSignalWithProcessVariables.bpmn20.xml" })]
        public virtual void testSignalWithoutSignalNameAndData()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("testSignalWithProcessVariables");
            IDictionary<string, object> processVariables = new Dictionary<string, object>();
            processVariables["variable"] = "value";

            // signal the execution while passing in the variables
            runtimeService.Signal(processInstance.Id, null, null, processVariables);

            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(processVariables, variables);
        }

        [Test]
        [Deployment]
        public virtual void testSignalInactiveExecution()
        {
            var instance = runtimeService.StartProcessInstanceByKey("testSignalInactiveExecution");

            // there exist two executions: the inactive parent (the process instance) and the child that actually waits in the receive task
            try
            {
                runtimeService.Signal(instance.Id);
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresent("cannot signal execution " + instance.Id + ": it has no current activity", e.Message);
            }
            catch (System.Exception)
            {
                Assert.Fail(
                    "Signalling an inactive execution that has no activity should result in a ProcessEngineException");
            }
        }

        [Test]
        public virtual void testGetVariablesUnexistingExecutionId()
        {
            try
            {
                runtimeService.GetVariables("unexistingExecutionId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
            }
        }

        [Test]
        public virtual void testGetVariablesNullExecutionId()
        {
            try
            {
                runtimeService.GetVariables(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testGetVariableUnexistingExecutionId()
        {
            try
            {
                runtimeService.GetVariables("unexistingExecutionId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
            }
        }

        [Test]
        public virtual void testGetVariableNullExecutionId()
        {
            try
            {
                runtimeService.GetVariables(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariableUnexistingVariableName()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var variableValue = runtimeService.GetVariable(processInstance.Id, "unexistingVariable");
            Assert.IsNull(variableValue);
        }

        [Test]
        public virtual void testSetVariableUnexistingExecutionId()
        {
            try
            {
                runtimeService.SetVariable("unexistingExecutionId", "VariableName", "value");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingExecutionId doesn't exist", ae.Message);
            }
        }

        [Test]
        public virtual void testSetVariableNullExecutionId()
        {
            try
            {
                runtimeService.SetVariable(null, "VariableName", "variableValue");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetVariableNullVariableName()
        {
            try
            {
                var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
                runtimeService.SetVariable(processInstance.Id, null, "variableValue");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("VariableName is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetVariables()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            runtimeService.SetVariables(processInstance.Id, vars);

            Assert.AreEqual("value1", runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesTyped()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);
            var variablesTyped = runtimeService.GetVariablesTyped(processInstance.Id);
            Assert.AreEqual(vars, variablesTyped);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesTypedDeserialize()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("broken", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue("broken")
                    .SerializationDataFormat(ESS.FW.Bpm.Engine.Variable.Variables.SerializationDataFormats.Net.ToString())
                    //.objectTypeName("unexisting")
                    .Create()));

            // this works
            var variablesTyped = runtimeService.GetVariablesTyped(processInstance.Id, false);
            //Assert.NotNull(variablesTyped.GetValueTyped("broken"));
            variablesTyped = runtimeService.GetVariablesTyped(processInstance.Id, new[] {"broken"}, false);
            //Assert.NotNull(variablesTyped.GetValueTyped("broken"));

            // this does not
            try
            {
                runtimeService.GetVariablesTyped(processInstance.Id);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }

            // this does not
            try
            {
                runtimeService.GetVariablesTyped(processInstance.Id, new[] {"broken"}, true);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesLocalTyped()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);
            var variablesTyped = runtimeService.GetVariablesLocalTyped(processInstance.Id);
            Assert.AreEqual(vars, variablesTyped);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesLocalTypedDeserialize()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("broken", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue("broken")
                    //.SerializationDataFormat(Variables.SerializationDataFormats.Java)
                    //.objectTypeName("unexisting")
                    .Create()));

            // this works
            var variablesTyped = runtimeService.GetVariablesLocalTyped(processInstance.Id, false);
            //Assert.NotNull(variablesTyped.GetValueTyped("broken"));
            variablesTyped = runtimeService.GetVariablesLocalTyped(processInstance.Id, new[] {"broken"}, false);
            //Assert.NotNull(variablesTyped.GetValueTyped("broken"));

            // this does not
            try
            {
                runtimeService.GetVariablesLocalTyped(processInstance.Id);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }

            // this does not
            try
            {
                runtimeService.GetVariablesLocalTyped(processInstance.Id, new[] {"broken"}, true);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }
        }

        [Test]
        public virtual void testSetVariablesUnexistingExecutionId()
        {
            try
            {
                runtimeService.SetVariables("unexistingexecution", new Dictionary<string, object>());
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("execution unexistingexecution doesn't exist", ae.Message);
            }
        }

        [Test]
        public virtual void testSetVariablesNullExecutionId()
        {
            try
            {
                runtimeService.SetVariables(null, new Dictionary<string, object>());
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }
        
        private void checkHistoricVariableUpdateEntity(string VariableName, string ProcessInstanceId)
        {
            if (processEngineConfiguration.HistoryLevel.Equals(HistoryLevelFields.HistoryLevelFull))
            {
                var deletedVariableUpdateFound = false;

                var resultSet = historyService.CreateHistoricDetailQuery(c=>c.ProcessInstanceId ==ProcessInstanceId)
                    
                    .ToList();
                foreach (var currentHistoricDetail in resultSet)
                {
                    Assert.True(currentHistoricDetail is HistoricVariableUpdateEventEntity);
                    var historicVariableUpdate = (HistoricVariableUpdateEventEntity) currentHistoricDetail;

                    if (historicVariableUpdate.Name.Equals(VariableName))
                        if (historicVariableUpdate.Value == null)
                            if (deletedVariableUpdateFound)
                                Assert.Fail("Mismatch: A HistoricVariableUpdateEntity with a null value already found");
                            else
                                deletedVariableUpdateFound = true;
                }

                Assert.True(deletedVariableUpdateFound);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariable()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            runtimeService.SetVariables(processInstance.Id, vars);

            runtimeService.RemoveVariable(processInstance.Id, "variable1");

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariableInParentScope()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", vars);
            var currentTask = taskService.CreateTaskQuery()
                .First();

            runtimeService.RemoveVariable(currentTask.ExecutionId, "variable1");

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
        }


        [Test]
        public virtual void testRemoveVariableNullExecutionId()
        {
            try
            {
                runtimeService.RemoveVariable(null, "variable");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariableLocal()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);
            runtimeService.RemoveVariableLocal(processInstance.Id, "variable1");

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
        }

        [Test][Deployment(new string[]{ "resources/api/oneSubProcess.bpmn20.xml"}) ]
        public virtual void testRemoveVariableLocalWithParentScope()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", vars);
            var currentTask = taskService.CreateTaskQuery()
                .First();
            runtimeService.SetVariableLocal(currentTask.ExecutionId, "localVariable", "local value");

            Assert.AreEqual("local value", runtimeService.GetVariableLocal(currentTask.ExecutionId, "localVariable"));

            runtimeService.RemoveVariableLocal(currentTask.ExecutionId, "localVariable");

            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "localVariable"));
            Assert.IsNull(runtimeService.GetVariableLocal(currentTask.ExecutionId, "localVariable"));

            Assert.AreEqual("value1", runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));

            Assert.AreEqual("value1", runtimeService.GetVariable(currentTask.ExecutionId, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(currentTask.ExecutionId, "variable2"));

            checkHistoricVariableUpdateEntity("localVariable", processInstance.Id);
        }


        [Test]
        public virtual void testRemoveLocalVariableNullExecutionId()
        {
            try
            {
                runtimeService.RemoveVariableLocal(null, "variable");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariables()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);
            runtimeService.SetVariable(processInstance.Id, "variable3", "value3");

            runtimeService.RemoveVariables(processInstance.Id, vars.Keys);

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable2"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable2"));

            Assert.AreEqual("value3", runtimeService.GetVariable(processInstance.Id, "variable3"));
            Assert.AreEqual("value3", runtimeService.GetVariableLocal(processInstance.Id, "variable3"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneSubProcess.bpmn20.xml" })]
        public virtual void testRemoveVariablesWithParentScope()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", vars);
            runtimeService.SetVariable(processInstance.Id, "variable3", "value3");

            var currentTask = taskService.CreateTaskQuery()
                .First();

            runtimeService.RemoveVariables(currentTask.ExecutionId, vars.Keys);

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable1"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable2"));
            Assert.IsNull(runtimeService.GetVariableLocal(processInstance.Id, "variable2"));

            Assert.AreEqual("value3", runtimeService.GetVariable(processInstance.Id, "variable3"));
            Assert.AreEqual("value3", runtimeService.GetVariableLocal(processInstance.Id, "variable3"));

            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable1"));
            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable2"));

            Assert.AreEqual("value3", runtimeService.GetVariable(currentTask.ExecutionId, "variable3"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
        }

        [Test]
        public virtual void testRemoveVariablesNullExecutionId()
        {
            try
            {
                runtimeService.RemoveVariables(null, new List<string>());
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneSubProcess.bpmn20.xml" })]
        public virtual void testRemoveVariablesLocalWithParentScope()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", vars);

            var currentTask = taskService.CreateTaskQuery()
                .First();
            IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
            varsToDelete["variable3"] = "value3";
            varsToDelete["variable4"] = "value4";
            varsToDelete["variable5"] = "value5";
            runtimeService.SetVariablesLocal(currentTask.ExecutionId, varsToDelete);
            runtimeService.SetVariableLocal(currentTask.ExecutionId, "variable6", "value6");

            Assert.AreEqual("value3", runtimeService.GetVariable(currentTask.ExecutionId, "variable3"));
            Assert.AreEqual("value3", runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable3"));
            Assert.AreEqual("value4", runtimeService.GetVariable(currentTask.ExecutionId, "variable4"));
            Assert.AreEqual("value4", runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable4"));
            Assert.AreEqual("value5", runtimeService.GetVariable(currentTask.ExecutionId, "variable5"));
            Assert.AreEqual("value5", runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable5"));
            Assert.AreEqual("value6", runtimeService.GetVariable(currentTask.ExecutionId, "variable6"));
            Assert.AreEqual("value6", runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable6"));

            runtimeService.RemoveVariablesLocal(currentTask.ExecutionId, varsToDelete.Keys);

            Assert.AreEqual("value1", runtimeService.GetVariable(currentTask.ExecutionId, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(currentTask.ExecutionId, "variable2"));

            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable3"));
            Assert.IsNull(runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable3"));
            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable4"));
            Assert.IsNull(runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable4"));
            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable5"));
            Assert.IsNull(runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable5"));

            Assert.AreEqual("value6", runtimeService.GetVariable(currentTask.ExecutionId, "variable6"));
            Assert.AreEqual("value6", runtimeService.GetVariableLocal(currentTask.ExecutionId, "variable6"));

            checkHistoricVariableUpdateEntity("variable3", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable4", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable5", processInstance.Id);
        }

        [Test]
        public virtual void testRemoveVariablesLocalNullExecutionId()
        {
            try
            {
                runtimeService.RemoveVariablesLocal(null, new List<string>());
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("executionId is null", ae.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testUpdateVariables()
        {
            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "value1";
            modifications["variable2"] = "value2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable1");

            IDictionary<string, object> initialVariables = new Dictionary<string, object>();
            initialVariables["variable1"] = "initialValue";
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", initialVariables);
            ((RuntimeServiceImpl) runtimeService).UpdateVariables(processInstance.Id, modifications, deletions);

            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable1"));
            Assert.AreEqual("value2", runtimeService.GetVariable(processInstance.Id, "variable2"));
        }

        [Test]
        [Deployment(new string[] { "resources/api/oneSubProcess.bpmn20.xml" })]
        public virtual void testUpdateVariablesLocal()
        {
            IDictionary<string, object> globalVars = new Dictionary<string, object>();
            globalVars["variable4"] = "value4";
            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", globalVars);

            var currentTask = taskService.CreateTaskQuery()
                .First();
            IDictionary<string, object> localVars = new Dictionary<string, object>();
            localVars["variable1"] = "value1";
            localVars["variable2"] = "value2";
            localVars["variable3"] = "value3";
            runtimeService.SetVariablesLocal(currentTask.ExecutionId, localVars);

            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            ((RuntimeServiceImpl) runtimeService).UpdateVariablesLocal(currentTask.ExecutionId, modifications, deletions);

            Assert.AreEqual("anotherValue1", runtimeService.GetVariable(currentTask.ExecutionId, "variable1"));
            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable2"));
            Assert.IsNull(runtimeService.GetVariable(currentTask.ExecutionId, "variable3"));
            Assert.AreEqual("value4", runtimeService.GetVariable(processInstance.Id, "variable4"));
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml", "resources/api/runtime/RuntimeServiceTest.catchPanicSignal.bpmn20.xml" })]
        public virtual void testSignalEventReceived()
        {
            //////  test  SignalEventReceived(String)

            startSignalCatchProcesses();
            // 12, because the signal catch is a scope
            Assert.AreEqual(12, runtimeService.CreateExecutionQuery()
                .Count());
            runtimeService.SignalEventReceived("alert");
            Assert.AreEqual(6, runtimeService.CreateExecutionQuery()
                .Count());
            runtimeService.SignalEventReceived("panic");
            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());

            //////  test  SignalEventReceived(String, String)
            startSignalCatchProcesses();

            // signal the executions one at a time:
            for (var executions = 3; executions > 0; executions--)
            {
                var page = runtimeService.CreateExecutionQuery()
                    //.SignalEventSubscriptionName("alert")
                    /*.ListPage(0, 1)*/
                    .ToList();
                runtimeService.SignalEventReceived("alert", page[0].Id);

                Assert.AreEqual(executions - 1, runtimeService.CreateExecutionQuery()
                   // .SignalEventSubscriptionName("alert")
                    .Count());
            }

            for (var executions = 3; executions > 0; executions--)
            {
                var page = runtimeService.CreateExecutionQuery()
                   // .SignalEventSubscriptionName("panic")
                    /*.ListPage(0, 1)*/
                    .ToList();
                runtimeService.SignalEventReceived("panic", page[0].Id);

                Assert.AreEqual(executions - 1, runtimeService.CreateExecutionQuery()
                   // .SignalEventSubscriptionName("panic")
                    .Count());
            }
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/RuntimeServiceTest.catchAlertMessage.bpmn20.xml", "resources/api/runtime/RuntimeServiceTest.catchPanicMessage.bpmn20.xml" })]
        public virtual void testMessageEventReceived()
        {
            startMessageCatchProcesses();
            // 12, because the signal catch is a scope
            Assert.AreEqual(12, runtimeService.CreateExecutionQuery()
                .Count());

            // signal the executions one at a time:
            for (var executions = 3; executions > 0; executions--)
            {
                var page = runtimeService.CreateExecutionQuery()
                    ////.MessageEventSubscriptionName("alert")
                    ///*.ListPage(0, 1)*/
                    .ToList();
                runtimeService.MessageEventReceived("alert", page[0].Id);

                Assert.AreEqual(executions - 1, runtimeService.CreateExecutionQuery()
                    ////.MessageEventSubscriptionName("alert")
                    .Count());
            }

            for (var executions = 3; executions > 0; executions--)
            {
                var page = runtimeService.CreateExecutionQuery()
                    //.MessageEventSubscriptionName("panic")
                    /*.ListPage(0, 1)*/
                    .ToList();
                runtimeService.MessageEventReceived("panic", page[0].Id);

                Assert.AreEqual(executions - 1, runtimeService.CreateExecutionQuery()
                    //.MessageEventSubscriptionName("panic")
                    .Count());
            }
        }

        [Test]
        public virtual void testSignalEventReceivedNonExistingExecution()
        {
            try
            {
                runtimeService.SignalEventReceived("alert", "nonexistingExecution");
                Assert.Fail("exeception expected");
            }
            catch (ProcessEngineException e)
            {
                // this is good
                Assert.True(e.Message.Contains("Cannot find execution with id 'nonexistingExecution'"));
            }
        }

        [Test]
        public virtual void testMessageEventReceivedNonExistingExecution()
        {
            try
            {
                runtimeService.MessageEventReceived("alert", "nonexistingExecution");
                Assert.Fail("exeception expected");
            }
            catch (ProcessEngineException e)
            {
                // this is good
                Assert.True(
                    e.Message.Contains(
                        "IExecution with id 'nonexistingExecution' does not have a subscription to a message event with name 'alert'"));
            }
        }

        [Test][Deployment(new string[]{ "resources/api/runtime/RuntimeServiceTest.catchAlertSignal.bpmn20.xml" }) ]
        public virtual void testExecutionWaitingForDifferentSignal()
        {
            runtimeService.StartProcessInstanceByKey("catchAlertSignal");
            var execution = runtimeService.CreateExecutionQuery()
                //.SignalEventSubscriptionName("alert")
                .First();
            try
            {
                runtimeService.SignalEventReceived("bogusSignal", execution.Id);
                Assert.Fail("exeception expected");
            }
            catch (ProcessEngineException e)
            {
                // this is good
                Assert.True(e.Message.Contains("has not subscribed to a signal event with name 'bogusSignal'"));
            }
        }

        private void startSignalCatchProcesses()
        {
            for (var i = 0; i < 3; i++)
            {
                runtimeService.StartProcessInstanceByKey("catchAlertSignal");
                runtimeService.StartProcessInstanceByKey("catchPanicSignal");
            }
        }

        private void startMessageCatchProcesses()
        {
            for (var i = 0; i < 3; i++)
            {
                runtimeService.StartProcessInstanceByKey("catchAlertMessage");
                runtimeService.StartProcessInstanceByKey("catchPanicMessage");
            }
        }

        // getActivityInstance Tests //////////////////////////////////

        [Test]
        public virtual void testActivityInstanceForNonExistingProcessInstanceId()
        {
            Assert.IsNull(runtimeService.GetActivityInstance("some-nonexisting-id"));
        }

        [Test]
        public virtual void testActivityInstanceForNullProcessInstanceId()
        {
            try
            {
                runtimeService.GetActivityInstance(null);
                Assert.Fail("PEE expected!");
            }
            catch (ProcessEngineException engineException)
            {
                Assert.True(engineException.Message.Contains("ProcessInstanceId is null"));
            }
        }
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testActivityInstancePopulated()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", "business-key");

            // validate properties of root
            var rootActInstance = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(processInstance.Id, rootActInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.ProcessDefinitionId, rootActInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, rootActInstance.ProcessInstanceId);
            Assert.True(rootActInstance.ExecutionIds[0].Equals(processInstance.Id));
            Assert.AreEqual(rootActInstance.ProcessDefinitionId, rootActInstance.ActivityId);
            Assert.IsNull(rootActInstance.ParentActivityInstanceId);
            Assert.AreEqual("processDefinition", rootActInstance.ActivityType);

            // validate properties of child:
            var task = taskService.CreateTaskQuery()
                .First();
            var childActivityInstance = rootActInstance.ChildActivityInstances[0];
            Assert.AreEqual(processInstance.Id, childActivityInstance.ProcessInstanceId);
            Assert.AreEqual(processInstance.ProcessDefinitionId, childActivityInstance.ProcessDefinitionId);
            Assert.AreEqual(processInstance.Id, childActivityInstance.ProcessInstanceId);
            Assert.True(childActivityInstance.ExecutionIds[0].Equals(task.ExecutionId));
            Assert.AreEqual("theTask", childActivityInstance.ActivityId);
            Assert.AreEqual(rootActInstance.Id, childActivityInstance.ParentActivityInstanceId);
            Assert.AreEqual("userTask", childActivityInstance.ActivityType);
            Assert.NotNull(childActivityInstance.ChildActivityInstances);
            Assert.NotNull(childActivityInstance.ChildTransitionInstances);
            Assert.AreEqual(0, childActivityInstance.ChildActivityInstances.Length);
            Assert.AreEqual(0, childActivityInstance.ChildTransitionInstances.Length);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForAsyncBeforeTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("theTask")
                    .Done());

            var asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
            Assert.AreEqual(processInstance.Id, asyncBeforeTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForConcurrentAsyncBeforeTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("concurrentTasksProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("theTask").Transition("asyncTask").Done());

            var asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
            var asyncExecutionId = managementService.CreateJobQuery()
                .First()
                .ExecutionId;
            Assert.AreEqual(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForAsyncBeforeStartEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("theStart")
                    .Done());

            var asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
            Assert.AreEqual(processInstance.Id, asyncBeforeTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForAsyncAfterTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);


            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Transition("theTask")
                    .Done());

            var asyncAfterTransitionInstance = tree.ChildTransitionInstances[0];
            Assert.AreEqual(processInstance.Id, asyncAfterTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForConcurrentAsyncAfterTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("concurrentTasksProcess");

            var asyncTask = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "asyncTask")
                .First();
            Assert.NotNull(asyncTask);
            taskService.Complete(asyncTask.Id);

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Activity("theTask").Transition("asyncTask").Done());

            var asyncBeforeTransitionInstance = tree.ChildTransitionInstances[0];
            var asyncExecutionId = managementService.CreateJobQuery()
                .First()
                .ExecutionId;
            Assert.AreEqual(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForAsyncAfterEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("asyncEndEventProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).Transition("theEnd").Done());

            var asyncAfterTransitionInstance = tree.ChildTransitionInstances[0];
            Assert.AreEqual(processInstance.Id, asyncAfterTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForNestedAsyncBeforeTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("subProcess").Transition("theTask").Done());

            var asyncBeforeTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
            var asyncExecutionId = managementService.CreateJobQuery()
                .First()
                .ExecutionId;
            Assert.AreEqual(asyncExecutionId, asyncBeforeTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForNestedAsyncBeforeStartEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Transition("theSubProcessStart")
                    .Done());
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForNestedAsyncAfterTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);


            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            //ActivityInstanceAssert.That(tree).HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId).BeginScope("subProcess").Transition("theTask").Done());

            var asyncAfterTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
            var asyncExecutionId = managementService.CreateJobQuery()
                .First()
                .ExecutionId;
            Assert.AreEqual(asyncExecutionId, asyncAfterTransitionInstance.ExecutionId);
        }

        [Test]
        [Deployment]
        public virtual void testActivityInstanceTreeForNestedAsyncAfterEndEvent()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("asyncEndEventProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .BeginScope("subProcess")
                    .Transition("theSubProcessEnd")
                    .Done());

            var asyncAfterTransitionInstance = tree.ChildActivityInstances[0].ChildTransitionInstances[0];
            var asyncExecutionId = managementService.CreateJobQuery()
                .First()
                .ExecutionId;
            Assert.AreEqual(asyncExecutionId, asyncAfterTransitionInstance.ExecutionId);
        }

        /// <summary>
        ///     Test for CAM-3572
        /// </summary>
        [Test]
        [Deployment]
        public virtual void testActivityInstanceForConcurrentSubprocess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("concurrentSubProcess");

            var tree = runtimeService.GetActivityInstance(processInstance.Id);
            Assert.NotNull(tree);

            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                    .Activity("outerTask")
                    .BeginScope("subProcess")
                    .Activity("innerTask")
                    .Done());
        }

        [Test]
        [Deployment]
        public virtual void testGetActivityInstancesForActivity()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");
            var definition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when
            var tree = runtimeService.GetActivityInstance(instance.Id);

            // then
            var processActivityInstances = tree.GetActivityInstances(definition.Id);
            Assert.AreEqual(1, processActivityInstances.Length);
            Assert.AreEqual(tree.Id, processActivityInstances[0].Id);
            Assert.AreEqual(definition.Id, processActivityInstances[0].ActivityId);

            AssertActivityInstances(tree.GetActivityInstances("subProcess#multiInstanceBody"), 1,
                "subProcess#multiInstanceBody");
            AssertActivityInstances(tree.GetActivityInstances("subProcess"), 3, "subProcess");
            AssertActivityInstances(tree.GetActivityInstances("innerTask"), 3, "innerTask");

            var subProcessInstance = tree.ChildActivityInstances[0].ChildActivityInstances[0];
            AssertActivityInstances(subProcessInstance.GetActivityInstances("subProcess"), 1, "subProcess");

            var childInstances = subProcessInstance.GetActivityInstances("innerTask");
            Assert.AreEqual(1, childInstances.Length);
            Assert.AreEqual(subProcessInstance.ChildActivityInstances[0].Id, childInstances[0].Id);
        }

        [Test][Deployment( "resources/api/runtime/RuntimeServiceTest.TestGetActivityInstancesForActivity.bpmn20.xml")]
        public virtual void testGetInvalidActivityInstancesForActivity()
        {
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");

            var tree = runtimeService.GetActivityInstance(instance.Id);

            try
            {
                tree.GetActivityInstances(null);
                Assert.Fail("exception expected");
            }
            catch (NullValueException)
            {
                // happy path
            }
        }

        [Test]
        [Deployment("resources/api/runtime/RuntimeServiceTest.TestGetActivityInstancesForActivity.bpmn20.xml")]
        public virtual void testGetActivityInstancesForNonExistingActivity()
        {
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");

            var tree = runtimeService.GetActivityInstance(instance.Id);

            var instances = tree.GetActivityInstances("aNonExistingActivityId");
            Assert.NotNull(instances);
            Assert.AreEqual(0, instances.Length);
        }

        [Test]
        [Deployment]
        public virtual void testGetTransitionInstancesForActivity()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");

            // complete one async task
            var job = managementService.CreateJobQuery()
               
                .First();
               // .ToList();
            managementService.ExecuteJob(job.Id);
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            // when
            var tree = runtimeService.GetActivityInstance(instance.Id);

            // then
            Assert.AreEqual(0, tree.GetTransitionInstances("subProcess")
                .Length);
            var asyncBeforeInstances = tree.GetTransitionInstances("innerTask");
            Assert.AreEqual(2, asyncBeforeInstances.Length);

            Assert.AreEqual("innerTask", asyncBeforeInstances[0].ActivityId);
            Assert.AreEqual("innerTask", asyncBeforeInstances[1].ActivityId);
            Assert.IsFalse(asyncBeforeInstances[0].Id.Equals(asyncBeforeInstances[1].Id));

            var asyncEndEventInstances = tree.GetTransitionInstances("theSubProcessEnd");
            Assert.AreEqual(1, asyncEndEventInstances.Length);
            Assert.AreEqual("theSubProcessEnd", asyncEndEventInstances[0].ActivityId);
        }

        [Test][Deployment("resources/api/runtime/RuntimeServiceTest.TestGetTransitionInstancesForActivity.bpmn20.xml")]
        public virtual void testGetInvalidTransitionInstancesForActivity()
        {
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");

            var tree = runtimeService.GetActivityInstance(instance.Id);

            try
            {
                tree.GetTransitionInstances(null);
                Assert.Fail("exception expected");
            }
            catch (NullValueException)
            {
                // happy path
            }
        }

        [Test]
        [Deployment("resources/api/runtime/RuntimeServiceTest.TestGetTransitionInstancesForActivity.bpmn20.xml")]
        public virtual void testGetTransitionInstancesForNonExistingActivity()
        {
            var instance = runtimeService.StartProcessInstanceByKey("miSubprocess");

            var tree = runtimeService.GetActivityInstance(instance.Id);

            var instances = tree.GetTransitionInstances("aNonExistingActivityId");
            Assert.NotNull(instances);
            Assert.AreEqual(0, instances.Length);
        }


        protected internal virtual void AssertActivityInstances(IActivityInstance[] instances, int expectedAmount,
            string expectedActivityId)
        {
            Assert.AreEqual(expectedAmount, instances.Length);

            ISet<string> instanceIds = new HashSet<string>();

            foreach (var instance in instances)
            {
                Assert.AreEqual(expectedActivityId, instance.ActivityId);
                instanceIds.Add(instance.Id);
            }

            // ensure that all instances are unique
            Assert.AreEqual(expectedAmount, instanceIds.Count);
        }


        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testChangeVariableType()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var dummy = new DummySerializable();
            runtimeService.SetVariable(instance.Id, "dummy", dummy);

            runtimeService.SetVariable(instance.Id, "dummy", 47);

            var variableInstance = runtimeService.CreateVariableInstanceQuery()
                .First();

            Assert.AreEqual(47, variableInstance.Value);
            Assert.AreEqual(ValueTypeFields.Integer.ToString(), variableInstance.TypeName);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testStartByKeyWithCaseInstanceId()
        {
            var caseInstanceId = "aCaseInstanceId";

            var firstInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", null, caseInstanceId);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // load process instance from db
            firstInstance = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == firstInstance.Id)
                .First();

            Assert.NotNull(firstInstance);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // the second possibility to start a process instance /////////////////////////////////////////////

            var secondInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", null, caseInstanceId, null);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);

            // load process instance from db
            secondInstance = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == secondInstance.Id)
                .First();

            Assert.NotNull(secondInstance);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testStartByIdWithCaseInstanceId()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key =="oneTaskProcess")
                .First()
                .Id;

            var caseInstanceId = "aCaseInstanceId";
            var firstInstance = runtimeService.StartProcessInstanceById(processDefinitionId, null, caseInstanceId);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // load process instance from db
            firstInstance = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId== firstInstance.Id)
                .First();

            Assert.NotNull(firstInstance);

            Assert.AreEqual(caseInstanceId, firstInstance.CaseInstanceId);

            // the second possibility to start a process instance /////////////////////////////////////////////

            var secondInstance = runtimeService.StartProcessInstanceById(processDefinitionId, null, caseInstanceId, null);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);

            // load process instance from db
            secondInstance = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == secondInstance.Id)
                .First();

            Assert.NotNull(secondInstance);

            Assert.AreEqual(caseInstanceId, secondInstance.CaseInstanceId);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testSetAbstractNumberValueFails()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValueTyped("var", ESS.FW.Bpm.Engine.Variable.Variables.NumberValue(42)));
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("cannot serialize value of abstract type number", e.Message);
            }

            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            try
            {
                runtimeService.SetVariable(processInstance.Id, "var", ESS.FW.Bpm.Engine.Variable.Variables.NumberValue(42));
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                // happy path
                AssertTextPresentIgnoreCase("cannot serialize value of abstract type number", e.Message);
            }
        }


        [Test][Deployment("resources/api/runtime/messageStartEvent.bpmn20.xml") ]
        public virtual void testStartProcessInstanceByMessageWithEarlierVersionOfProcessDefinition()
        {
            var deploymentId = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/runtime/messageStartEvent_version2.bpmn20.xml")
                .Deploy()
                .Id;
            var processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 1)
                .First();

            var processInstance = runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("startMessage",
                processDefinition.Id);

            Assert.That(processInstance, Is.Not.Null);
            Assert.That(processInstance.ProcessDefinitionId, Is.EqualTo(processDefinition.Id));

            // clean up
            repositoryService.DeleteDeployment(deploymentId, true);
        }

        [Test]
        [Deployment("resources/api/runtime/messageStartEvent.bpmn20.xml")]
        public virtual void testStartProcessInstanceByMessageWithLastVersionOfProcessDefinition()
        {
            var deploymentId = repositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/runtime/messageStartEvent_version2.bpmn20.xml")
                .Deploy()
                .Id;
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                /*.LatestVersion()*/
                .First();

            var processInstance = runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("newStartMessage",
                processDefinition.Id);

            Assert.That(processInstance, Is.Not.Null);
            Assert.That(processInstance.ProcessDefinitionId, Is.EqualTo(processDefinition.Id));

            // clean up
            repositoryService.DeleteDeployment(deploymentId, true);
        }

        [Test]
        [Deployment("resources/api/runtime/messageStartEvent.bpmn20.xml")]
        public virtual void testStartProcessInstanceByMessageWithNonExistingMessageStartEvent()
        {
            string deploymentId = null;
            try
            {
                deploymentId = repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/runtime/messageStartEvent_version2.bpmn20.xml")
                    .Deploy()
                    .Id;
                var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                   // .ProcessDefinitionVersion(1)
                    .First();

                runtimeService.StartProcessInstanceByMessageAndProcessDefinitionId("newStartMessage",
                    processDefinition.Id);

                Assert.Fail("exeception expected");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Cannot correlate message 'newStartMessage'"));
            }
            finally
            {
                // clean up
                if (!ReferenceEquals(deploymentId, null))
                    repositoryService.DeleteDeployment(deploymentId, true);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testActivityInstanceActivityNameProperty()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("oneTaskProcess")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var activityInstances = tree.GetActivityInstances("theTask");
            Assert.AreEqual(1, activityInstances.Length);

            var task = activityInstances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("my task", task.ActivityName);
        }
        [Test]
        [Deployment]
        public virtual void testTransitionInstanceActivityNamePropertyBeforeTask()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("firstServiceTask");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("First Service Task", task.ActivityName);

            instances = tree.GetTransitionInstances("secondServiceTask");
            task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("Second Service Task", task.ActivityName);
        }

        [Test][Deployment( "resources/api/runtime/RuntimeServiceTest.TestTransitionInstanceActivityNamePropertyBeforeTask.bpmn20.xml") ]
        public virtual void testTransitionInstanceActivityTypePropertyBeforeTask()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("firstServiceTask");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("serviceTask", task.ActivityType);

            instances = tree.GetTransitionInstances("secondServiceTask");
            task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("serviceTask", task.ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testTransitionInstanceActivityNamePropertyAfterTask()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("firstServiceTask");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("First Service Task", task.ActivityName);

            instances = tree.GetTransitionInstances("secondServiceTask");
            task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("Second Service Task", task.ActivityName);
        }

        [Test]
        [Deployment("resources/api/runtime/RuntimeServiceTest.TestTransitionInstanceActivityNamePropertyAfterTask.bpmn20.xml")]
        public virtual void testTransitionInstanceActivityTypePropertyAfterTask()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("firstServiceTask");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("serviceTask", task.ActivityType);

            instances = tree.GetTransitionInstances("secondServiceTask");
            task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("serviceTask", task.ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testTransitionInstanceActivityNamePropertyBeforeStartEvent()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("start");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("The Start Event", task.ActivityName);
        }

        [Test]
        [Deployment("resources/api/runtime/RuntimeServiceTest.TestTransitionInstanceActivityNamePropertyBeforeStartEvent.bpmn20.xml") ]
        public virtual void testTransitionInstanceActivityTypePropertyBeforeStartEvent()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("start");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("startEvent", task.ActivityType);
        }

        [Test]
        [Deployment]
        public virtual void testTransitionInstanceActivityNamePropertyAfterStartEvent()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("start");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityName);
            Assert.AreEqual("The Start Event", task.ActivityName);
        }

        [Test]
        [Deployment("resources/api/runtime/RuntimeServiceTest.TestTransitionInstanceActivityNamePropertyAfterStartEvent.bpmn20.xml")]
        public virtual void testTransitionInstanceActivityTypePropertyAfterStartEvent()
        {
            // given
            var ProcessInstanceId = runtimeService.StartProcessInstanceByKey("process")
                .Id;

            // when
            var tree = runtimeService.GetActivityInstance(ProcessInstanceId);

            // then
            var instances = tree.GetTransitionInstances("start");
            var task = instances[0];
            Assert.NotNull(task);
            Assert.NotNull(task.ActivityType);
            Assert.AreEqual("startEvent", task.ActivityType);
        }

        //Test for a bug: when the process engine is rebooted the
        // cache is cleaned and the deployed process definition is
        // removed from the process cache. This led to problems because
        // the id wasnt fetched from the DB after a redeploy.
        [Test]
        public virtual void testStartProcessInstanceByIdAfterReboot()
        {
            // In case this test is run in a test suite, previous engines might
            // have been initialized and cached.  First we close the
            // existing process engines to make sure that the db is clean
            // and that there are no existing process engines involved.
            ProcessEngines.Destroy();

            // Creating the DB schema (without building a process engine)
            ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneInMemProcessEngineConfiguration();
            processEngineConfiguration.SetProcessEngineName("reboot-test-schema");
            //processEngineConfiguration.JdbcUrl = "jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000";
            var schemaProcessEngine = processEngineConfiguration.BuildProcessEngine();

            // Create process engine and deploy test process
            var processEngine = new StandaloneProcessEngineConfiguration().SetProcessEngineName("reboot-test")
                .SetDatabaseSchemaUpdate(ProcessEngineConfiguration.DbSchemaUpdateFalse)
                .SetJdbcUrl("jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000")
                .SetJobExecutorActivate(false)
                .BuildProcessEngine();

            processEngine.RepositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/oneTaskProcess.bpmn20.xml")
                .Deploy();
            // verify existence of process definition
            var processDefinitions = processEngine.RepositoryService.CreateProcessDefinitionQuery()
                
                .ToList();

            Assert.AreEqual(1, processDefinitions.Count);

            // Start a new Process instance
            var processInstance = processEngine.RuntimeService.StartProcessInstanceById(processDefinitions[0].Id);
            var ProcessInstanceId = processInstance.Id;
            Assert.NotNull(processInstance);

            // Close the process engine
            processEngine.Close();
            Assert.NotNull(processEngine.RuntimeService);

            // Reboot the process engine
            processEngine = new StandaloneProcessEngineConfiguration().SetProcessEngineName("reboot-test")
                .SetDatabaseSchemaUpdate(ProcessEngineConfiguration.DbSchemaUpdateFalse)
                .SetJdbcUrl("jdbc:h2:mem:activiti-reboot-test;DB_CLOSE_DELAY=1000")
                .SetJobExecutorActivate(false)
                .BuildProcessEngine();

            // Check if the existing process instance is still alive
            processInstance = processEngine.RuntimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == ProcessInstanceId)
                .First();

            Assert.NotNull(processInstance);

            // Complete the task.  That will end the process instance
            var taskService = processEngine.TaskService;
            var task = taskService.CreateTaskQuery()
                .First();
               // .ToList();
            taskService.Complete(task.Id);

            // Check if the process instance has really ended.  This means that the process definition has
            // re-loaded into the process definition cache
            processInstance = processEngine.RuntimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId == ProcessInstanceId)
                .First();
            Assert.IsNull(processInstance);

            // Extra check to see if a new process instance can be started as well
            processInstance = processEngine.RuntimeService.StartProcessInstanceById(processDefinitions[0].Id);
            Assert.NotNull(processInstance);

            // close the process engine
            processEngine.Close();

            // Cleanup schema
            schemaProcessEngine.Close();
        }

        [Test]
        [Deployment]
        public virtual void testVariableScope()
        {
            // After starting the process, the task in the subprocess should be active
            IDictionary<string, object> varMap = new Dictionary<string, object>();
            varMap["test"] = "test";
            var pi = runtimeService.StartProcessInstanceByKey("simpleSubProcess", varMap);
            var subProcessTask = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== pi.Id)
                .First();
            Assert.AreEqual("ITask in subprocess", subProcessTask.Name);

            // get variables for execution id user task, should return the new value of variable test --> test2
            Assert.AreEqual("test2", runtimeService.GetVariable(subProcessTask.ExecutionId, "test"));
            Assert.AreEqual("test2", runtimeService.GetVariables(subProcessTask.ExecutionId)["test"]);

            // get variables for process instance id, should return the initial value of variable test --> test
            Assert.AreEqual("test", runtimeService.GetVariable(pi.Id, "test"));
            Assert.AreEqual("test", runtimeService.GetVariables(pi.Id)["test"]);

            runtimeService.SetVariableLocal(subProcessTask.ExecutionId, "test", "test3");

            // get variables for execution id user task, should return the new value of variable test --> test3
            Assert.AreEqual("test3", runtimeService.GetVariable(subProcessTask.ExecutionId, "test"));
            Assert.AreEqual("test3", runtimeService.GetVariables(subProcessTask.ExecutionId)["test"]);

            // get variables for process instance id, should still return the initial value of variable test --> test
            Assert.AreEqual("test", runtimeService.GetVariable(pi.Id, "test"));
            Assert.AreEqual("test", runtimeService.GetVariables(pi.Id)["test"]);

            runtimeService.SetVariable(pi.Id, "test", "test4");

            // get variables for execution id user task, should return the old value of variable test --> test3
            Assert.AreEqual("test3", runtimeService.GetVariable(subProcessTask.ExecutionId, "test"));
            Assert.AreEqual("test3", runtimeService.GetVariables(subProcessTask.ExecutionId)["test"]);

            // get variables for process instance id, should also return the initial value of variable test --> test4
            Assert.AreEqual("test4", runtimeService.GetVariable(pi.Id, "test"));
            Assert.AreEqual("test4", runtimeService.GetVariables(pi.Id)["test"]);

            // After completing the task in the subprocess,
            // the subprocess scope is destroyed and the complete process ends
            taskService.Complete(subProcessTask.Id);
        }

        [Test]
        [Deployment]
        public virtual void testBasicVariableOperations()
        {
            var now = DateTime.Now;
            IList<string> serializable = new List<string>();
            serializable.Add("one");
            serializable.Add("two");
            serializable.Add("three");
            var bytes = "somebytes".GetBytes();
            var streamBytes = "morebytes".GetBytes();

            // Start process instance with different types of variables
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["longVar"] = 928374L;
            variables["shortVar"] = (short) 123;
            variables["integerVar"] = 1234;
            variables["stringVar"] = "coca-cola";
            variables["dateVar"] = now;
            variables["nullVar"] = null;
            variables["serializableVar"] = serializable;
            variables["bytesVar"] = bytes;
            variables["byteStreamVar"] = new MemoryStream(streamBytes);
            var processInstance = runtimeService.StartProcessInstanceByKey("taskAssigneeProcess", variables);

            variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual("coca-cola", variables["stringVar"]);
            Assert.AreEqual(928374L, variables["longVar"]);
            Assert.AreEqual((short) 123, variables["shortVar"]);
            Assert.AreEqual(1234, variables["integerVar"]);
            Assert.AreEqual(now, variables["dateVar"]);
            Assert.AreEqual(null, variables["nullVar"]);
            Assert.AreEqual(serializable, variables["serializableVar"]);
            Assert.AreEqual(bytes, (byte[]) variables["bytesVar"]);
            Assert.AreEqual(streamBytes, (byte[]) variables["byteStreamVar"]);
            Assert.AreEqual(9, variables.Count);

            // Set all existing variables values to null
            runtimeService.SetVariable(processInstance.Id, "longVar", null);
            runtimeService.SetVariable(processInstance.Id, "shortVar", null);
            runtimeService.SetVariable(processInstance.Id, "integerVar", null);
            runtimeService.SetVariable(processInstance.Id, "stringVar", null);
            runtimeService.SetVariable(processInstance.Id, "dateVar", null);
            runtimeService.SetVariable(processInstance.Id, "nullVar", null);
            runtimeService.SetVariable(processInstance.Id, "serializableVar", null);
            runtimeService.SetVariable(processInstance.Id, "bytesVar", null);
            runtimeService.SetVariable(processInstance.Id, "byteStreamVar", null);

            variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(null, variables["longVar"]);
            Assert.AreEqual(null, variables["shortVar"]);
            Assert.AreEqual(null, variables["integerVar"]);
            Assert.AreEqual(null, variables["stringVar"]);
            Assert.AreEqual(null, variables["dateVar"]);
            Assert.AreEqual(null, variables["nullVar"]);
            Assert.AreEqual(null, variables["serializableVar"]);
            Assert.AreEqual(null, variables["bytesVar"]);
            Assert.AreEqual(null, variables["byteStreamVar"]);
            Assert.AreEqual(9, variables.Count);

            // Update existing variable values again, and add a new variable
            runtimeService.SetVariable(processInstance.Id, "new var", "hi");
            runtimeService.SetVariable(processInstance.Id, "longVar", 9987L);
            runtimeService.SetVariable(processInstance.Id, "shortVar", (short) 456);
            runtimeService.SetVariable(processInstance.Id, "integerVar", 4567);
            runtimeService.SetVariable(processInstance.Id, "stringVar", "colgate");
            runtimeService.SetVariable(processInstance.Id, "dateVar", now);
            runtimeService.SetVariable(processInstance.Id, "serializableVar", serializable);
            runtimeService.SetVariable(processInstance.Id, "bytesVar", bytes);
            runtimeService.SetVariable(processInstance.Id, "byteStreamVar", new MemoryStream(streamBytes));

            variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual("hi", variables["new var"]);
            Assert.AreEqual(9987L, variables["longVar"]);
            Assert.AreEqual((short) 456, variables["shortVar"]);
            Assert.AreEqual(4567, variables["integerVar"]);
            Assert.AreEqual("colgate", variables["stringVar"]);
            Assert.AreEqual(now, variables["dateVar"]);
            Assert.AreEqual(null, variables["nullVar"]);
            Assert.AreEqual(serializable, variables["serializableVar"]);
            Assert.Equals(bytes, (byte[]) variables["bytesVar"]);
            Assert.Equals(streamBytes, (byte[]) variables["byteStreamVar"]);
            Assert.AreEqual(10, variables.Count);

            ICollection<string> varFilter = new List<string>(2);
            varFilter.Add("stringVar");
            varFilter.Add("integerVar");

            var filteredVariables = runtimeService.GetVariables(processInstance.Id, varFilter);
            Assert.AreEqual(2, filteredVariables.Count);
            Assert.True(filteredVariables.ContainsKey("stringVar"));
            Assert.True(filteredVariables.ContainsKey("integerVar"));

            // Try setting the value of the variable that was initially created with value 'null'
            runtimeService.SetVariable(processInstance.Id, "nullVar", "a value");
            var newValue = runtimeService.GetVariable(processInstance.Id, "nullVar");
            Assert.NotNull(newValue);
            Assert.AreEqual("a value", newValue);

            // Try setting the value of the serializableVar to an integer value
            runtimeService.SetVariable(processInstance.Id, "serializableVar", 100);
            variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(100, variables["serializableVar"]);

            // Try setting the value of the serializableVar back to a serializable value
            runtimeService.SetVariable(processInstance.Id, "serializableVar", serializable);
            variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(serializable, variables["serializableVar"]);
        }

        [Test][Deployment(new string[] {"resources/api/runtime/RuntimeServiceTest.TestBasicVariableOperations.bpmn20.xml"})] 
        public virtual void testOnlyChangeType()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aVariable"] = 1234;
            var pi = runtimeService.StartProcessInstanceByKey("taskAssigneeProcess", variables);

            var query = runtimeService.CreateVariableInstanceQuery(c=>c.Name == "aVariable");

            var variable = query.First();
            Assert.AreEqual(ValueTypeFields.Integer.Name, variable.TypeName);

            runtimeService.SetVariable(pi.Id, "aVariable", 1234L);
            variable = query.First();
            Assert.AreEqual(ValueTypeFields.Long.Name, variable.TypeName);

            runtimeService.SetVariable(pi.Id, "aVariable", (short) 1234);
            variable = query.First();
            Assert.AreEqual(ValueTypeFields.Short.Name, variable.TypeName);
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/RuntimeServiceTest.TestBasicVariableOperations.bpmn20.xml" })]
        public virtual void testChangeTypeFromSerializableUsingApi()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aVariable"] = new SerializableVariable("foo");
            var pi = runtimeService.StartProcessInstanceByKey("taskAssigneeProcess", variables);

            var query = runtimeService.CreateVariableInstanceQuery(c=>c.Name == "aVariable");

            var variable = query.First();
            Assert.AreEqual(ValueTypeFields.Object.Name, variable.TypeName);

            runtimeService.SetVariable(pi.Id, "aVariable", null);
            variable = query.First();
            Assert.AreEqual(ValueTypeFields.Null.Name, variable.TypeName);
        }

        [Test]
        [Deployment]
        public virtual void testChangeSerializableInsideEngine()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var task = taskService.CreateTaskQuery()
                .First();

            var var = (SerializableVariable) taskService.GetVariable(task.Id, "VariableName");
            Assert.NotNull(var);
        }

        [Test]
        [Deployment(new string[] { "resources/api/runtime/RuntimeServiceTest.TestBasicVariableOperations.bpmn20.xml" })]
        public virtual void testChangeToSerializableUsingApi()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aVariable"] = "test";
            var processInstance = runtimeService.StartProcessInstanceByKey("taskAssigneeProcess", variables);

            var query = runtimeService.CreateVariableInstanceQuery(c=>c.Name == "aVariable");

            var variable = query.First();
            Assert.AreEqual(ValueTypeFields.String.Name, variable.TypeName);

            runtimeService.SetVariable(processInstance.Id, "aVariable", new SerializableVariable("foo"));
            variable = query.First();
            Assert.AreEqual(ValueTypeFields.Object.Name, variable.TypeName);
        }

        [Test]
        [Deployment]
        public virtual void testGetVariableInstancesFromVariableScope()
        {
            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("anIntegerVariable", 1234)
                .PutValue("anObjectValue", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(new SimpleSerializableBean(10))
                    .SerializationDataFormat(ESS.FW.Bpm.Engine.Variable.Variables.SerializationDataFormats.Net.ToString()))
                .PutValue("anUntypedObjectValue", new SimpleSerializableBean(30));

            runtimeService.StartProcessInstanceByKey("testProcess", variables);

            // Assertions are part of the java delegate AssertVariableInstancesDelegate
            // only there we can access the IVariableScope methods
        }

        [Test]
        [Deployment( "resources/api/runtime/RuntimeServiceTest.TestSetVariableInScope.bpmn20.xml")]
        public virtual void testSetVariableInScopeExplicitUpdate()
        {
            // when a process instance is started and the task after the subprocess reached
            runtimeService.StartProcessInstanceByKey("testProcess");
            //new Dictionary<string, ITypedValue>(){{"shouldExplicitlyUpdateVariable", true));

            // then there should be only the "shouldExplicitlyUpdateVariable" variable
            var variableInstance = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(variableInstance);
            Assert.AreEqual("shouldExplicitlyUpdateVariable", variableInstance.Name);
        }

        [Deployment("resources/api/runtime/RuntimeServiceTest.TestSetVariableInScope.bpmn20.xml")]
        public virtual void testSetVariableInScopeImplicitUpdate()
        {
            // when a process instance is started and the task after the subprocess reached
            runtimeService.StartProcessInstanceByKey("testProcess");
            //new Dictionary<string, ITypedValue>(){{"shouldExplicitlyUpdateVariable", true));

            // then there should be only the "shouldExplicitlyUpdateVariable" variable
            var variableInstance = runtimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(variableInstance);
            Assert.AreEqual("shouldExplicitlyUpdateVariable", variableInstance.Name);
        }

        [Test]
        [Deployment]
        public virtual void testUpdateVariableInProcessWithoutWaitstate()
        {
            // when a process instance is started
            runtimeService.StartProcessInstanceByKey("oneScriptTaskProcess");
            //new Dictionary<string, ITypedValue>(){{"var", new SimpleSerializableBean(10)));

            // then it should succeeds successfully
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.IsNull(processInstance);
        }

        [Test]
        [Deployment]
        public virtual void testSetUpdateAndDeleteComplexVariable()
        {
            // when a process instance is started
            runtimeService.StartProcessInstanceByKey("oneUserTaskProcess");
            //new Dictionary<string, ITypedValue>(){{"var", new SimpleSerializableBean(10)));

            // then it should wait at the user task
            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);
        }

        [Test]
        [Deployment]
        public virtual void testRollback()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("RollbackProcess");

                Assert.Fail("Starting the process instance should throw an exception");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("Buzzz", e.Message);
            }

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }
        [Test]
        [Deployment(new string[] { "resources/api/runtime/trivial.bpmn20.xml", "resources/api/runtime/rollbackAfterSubProcess.bpmn20.xml"}) ]
        public virtual void testRollbackAfterSubProcess()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("RollbackAfterSubProcess");

                Assert.Fail("Starting the process instance should throw an exception");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual("Buzzz", e.Message);
            }

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery()
                .Count());
        }

        [Test]
        public virtual void testGetActivityInstanceForCompletedInstanceInDelegate()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var deletingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1")
                .StartEvent()
                .UserTask()
                .ServiceTask()
                .CamundaClass(typeof(DeleteInstanceDelegate).FullName)
                .UserTask()
                .EndEvent()
                .Done();
            var processToDelete = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2")
                .StartEvent()
                .UserTask()
                .EndEvent()
                .Done();

            Deployment(deletingProcess, processToDelete);

            var instanceToDelete = runtimeService.StartProcessInstanceByKey("process2");
            var deletingInstance = runtimeService.StartProcessInstanceByKey("process1", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("instanceToComplete", instanceToDelete.Id));

            var deleteTrigger = taskService.CreateTaskQuery(c=>c.ProcessInstanceId== deletingInstance.Id)
                .First();

            // when
            taskService.Complete(deleteTrigger.Id);

            // then
            var activityInstanceNull = (bool) runtimeService.GetVariable(deletingInstance.Id, "activityInstanceNull");
            Assert.True(activityInstanceNull);
        }

        public class DeleteInstanceDelegate : IJavaDelegate
        {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                var runtimeService = ((IDelegateExecution) execution).ProcessEngineServices.RuntimeService;
                var taskService = ((IDelegateExecution) execution).ProcessEngineServices.TaskService;

                var instanceToDelete = (string) execution.GetVariable("instanceToComplete");
                var taskToTrigger = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instanceToDelete)
                    .First();
                taskService.Complete(taskToTrigger.Id);

                var activityInstance = runtimeService.GetActivityInstance(instanceToDelete);
                execution.SetVariable("activityInstanceNull", activityInstance == null);
            }
        }
        [Deployment(new string[] { "resources/api/runtime/RuntimeServiceTest.TestCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "resources/api/runtime/RuntimeServiceTest.TestCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }) ]
        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void testCascadingDeleteSubprocessInstanceSkipIoMappings()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("callingProcess");

            var instance2 = runtimeService.CreateProcessInstanceQuery()
               // .SetSuperProcessInstanceId(instance.Id)
                .First();

            // when the process instance is deleted and we do skip the io mappings
            runtimeService.DeleteProcessInstance(instance.Id, "test_purposes", false, true /*, true*/);

            // then
            AssertProcessEnded(instance.Id);
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId== instance2.Id)
                
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                ////.VariableName("inputMappingExecuted")
                .Count());
        }

        [Deployment(new string[] { "resources/api/runtime/RuntimeServiceTest.TestCascadingDeleteSubprocessInstanceSkipIoMappings.Calling.bpmn20.xml", "resources/api/runtime/RuntimeServiceTest.TestCascadingDeleteSubprocessInstanceSkipIoMappings.Called.bpmn20.xml" }) ]
        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void testCascadingDeleteSubprocessInstanceWithoutSkipIoMappings()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("callingProcess");

            var instance2 = runtimeService.CreateProcessInstanceQuery()
               // .SetSuperProcessInstanceId(instance.Id)
                .First();

            // when the process instance is deleted and we do not skip the io mappings
            runtimeService.DeleteProcessInstance(instance.Id, "test_purposes", false, true /*, false*/);

            // then
            AssertProcessEnded(instance.Id);
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId== instance2.Id)
                
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                ////.VariableName("inputMappingExecuted")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                ////.VariableName("outputMappingExecuted")
                .Count());
        }
[Deployment(new string[]{ "resources/api/oneTaskProcessWithIoMappings.bpmn20.xml" }) ]   

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void testDeleteProcessInstanceSkipIoMappings()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("ioMappingProcess");

            // when the process instance is deleted and we do skip the io mappings
            runtimeService.DeleteProcessInstance(instance.Id, null, false, true /*, true*/);

            // then
            AssertProcessEnded(instance.Id);
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId== instance.Id)
                
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("inputMappingExecuted")
                .Count());
        }
        [Deployment(new string[] { "resources/api/oneTaskProcessWithIoMappings.bpmn20.xml" }) ]
        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        public virtual void testDeleteProcessInstanceWithoutSkipIoMappings()
        {
            // given a process instance
            var instance = runtimeService.StartProcessInstanceByKey("ioMappingProcess");

            // when the process instance is deleted and we do not skip the io mappings
            runtimeService.DeleteProcessInstance(instance.Id, null, false, true /*, false*/);

            // then
            AssertProcessEnded(instance.Id);
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery(c=>c.ProcessInstanceId== instance.Id)
                
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("inputMappingExecuted")
                .Count());
            Assert.AreEqual(1, historyService.CreateHistoricVariableInstanceQuery()
                //.VariableName("outputMappingExecuted")
                .Count());
        }
    }
}