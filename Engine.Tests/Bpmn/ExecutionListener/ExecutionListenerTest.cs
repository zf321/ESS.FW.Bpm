using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Bpmn.Event.Conditional;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ExecutionListener
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ExecutionListenerTest
    {
        private readonly bool InstanceFieldsInitialized;

        public ExecutionListenerTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(processEngineRule);
            //ruleChain = RuleChain.outerRule(processEngineRule).around(testHelper);
        }


        public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(processEngineRule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        protected internal IHistoryService historyService;
        protected internal IManagementService managementService;
        [SetUp]
        public virtual void clearRecorderListener()
        {
            RecorderExecutionListener.Clear();
        }

        [SetUp]
        public virtual void initServices()
        {
            runtimeService = processEngineRule.RuntimeService;
            taskService = processEngineRule.TaskService;
            historyService = processEngineRule.HistoryService;
            managementService = processEngineRule.ManagementService;
        }

        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void assertProcessEnded(final String ProcessInstanceId)
        public virtual void assertProcessEnded(string ProcessInstanceId)
        {
            var processInstance = runtimeService.CreateProcessInstanceQuery(c => c.ProcessDefinitionId == ProcessInstanceId)
                .First();

            if (processInstance != null)
                throw new System.Exception("Expected finished process instance '" + ProcessInstanceId +
                                    "' but it was still in the db");
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenersProcess.bpmn20.xml")]
        public virtual void testExecutionListenersOnAllPossibleElements()
        {
            // Process start executionListener will have executionListener class that sets 2 variables
            var processInstance = runtimeService.StartProcessInstanceByKey("executionListenersProcess", "businessKey123");

            var varSetInExecutionListener =
                (string)runtimeService.GetVariable(processInstance.Id, "variableSetInExecutionListener");
            Assert.NotNull(varSetInExecutionListener);
            Assert.AreEqual("firstValue", varSetInExecutionListener);

            // Check if business key was available in execution listener
            var businessKey = (string)runtimeService.GetVariable(processInstance.Id, "businessKeyInExecution");
            Assert.NotNull(businessKey);
            Assert.AreEqual("businessKey123", businessKey);

            // Transition take executionListener will set 2 variables
            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            varSetInExecutionListener =
                (string)runtimeService.GetVariable(processInstance.Id, "variableSetInExecutionListener");

            Assert.NotNull(varSetInExecutionListener);
            Assert.AreEqual("secondValue", varSetInExecutionListener);

            var myPojo = new ExampleExecutionListenerPojo();
            runtimeService.SetVariable(processInstance.Id, "myPojo", myPojo);

            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            // First usertask uses a method-expression as executionListener: ${myPojo.MyMethod(execution.EventName)}
            var pojoVariable = (ExampleExecutionListenerPojo)runtimeService.GetVariable(processInstance.Id, "myPojo");
            Assert.NotNull(pojoVariable.ReceivedEventName);
            Assert.AreEqual("end", pojoVariable.ReceivedEventName);

            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

            assertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenersStartEndEvent.bpmn20.xml")]
        public virtual void testExecutionListenersOnStartEndEvents()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("executionListenersProcess");
            assertProcessEnded(processInstance.Id);

            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(4, recordedEvents.Count);

            Assert.AreEqual("theStart", recordedEvents[0].ActivityId);
            Assert.AreEqual("Start Event", recordedEvents[0].ActivityName);
            Assert.AreEqual("Start Event Listener", recordedEvents[0].Parameter);
            Assert.AreEqual("end", recordedEvents[0].EventName);
            Assert.That(recordedEvents[0].Canceled, Is.EqualTo(false));

            Assert.AreEqual("noneEvent", recordedEvents[1].ActivityId);
            Assert.AreEqual("None Event", recordedEvents[1].ActivityName);
            Assert.AreEqual("Intermediate Catch Event Listener", recordedEvents[1].Parameter);
            Assert.AreEqual("end", recordedEvents[1].EventName);
            Assert.That(recordedEvents[1].Canceled, Is.EqualTo(false));

            Assert.AreEqual("signalEvent", recordedEvents[2].ActivityId);
            Assert.AreEqual("Signal Event", recordedEvents[2].ActivityName);
            Assert.AreEqual("Intermediate Throw Event Listener", recordedEvents[2].Parameter);
            Assert.AreEqual("start", recordedEvents[2].EventName);
            Assert.That(recordedEvents[2].Canceled, Is.EqualTo(false));

            Assert.AreEqual("theEnd", recordedEvents[3].ActivityId);
            Assert.AreEqual("End Event", recordedEvents[3].ActivityName);
            Assert.AreEqual("End Event Listener", recordedEvents[3].Parameter);
            Assert.AreEqual("start", recordedEvents[3].EventName);
            Assert.That(recordedEvents[3].Canceled, Is.EqualTo(false));
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenersFieldInjectionProcess.bpmn20.xml")]
        public virtual void testExecutionListenerFieldInjection()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["myVar"] = "listening!";

            var processInstance = runtimeService.StartProcessInstanceByKey("executionListenersProcess", variables);

            var varSetByListener = runtimeService.GetVariable(processInstance.Id, "var");
            Assert.NotNull(varSetByListener);
            Assert.True(varSetByListener is string);

            // Result is a concatenation of fixed injected field and injected expression
            Assert.AreEqual("Yes, I am listening!", varSetByListener);
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenersCurrentActivity.bpmn20.xml")]
        public virtual void testExecutionListenerCurrentActivity()
        {
            //CurrentActivityExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("executionListenersProcess");
            assertProcessEnded(processInstance.Id);

            var currentActivities = CurrentActivityExecutionListener.CurrentActivities;
            Assert.AreEqual(3, currentActivities.Count);

            Assert.AreEqual("theStart", currentActivities[0].ActivityId);
            Assert.AreEqual("Start Event", currentActivities[0].ActivityName);

            Assert.AreEqual("noneEvent", currentActivities[1].ActivityId);
            Assert.AreEqual("None Event", currentActivities[1].ActivityName);

            Assert.AreEqual("theEnd", currentActivities[2].ActivityId);
            Assert.AreEqual("End Event", currentActivities[2].ActivityName);
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenerTest.testOnBoundaryEvents.bpmn20.xml")]
        public virtual void testOnBoundaryEvents()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            var firstTimer = managementService.CreateJobQuery()
                /*.Timers()*/
                .First();

            managementService.ExecuteJob(firstTimer.Id);

            var secondTimer = managementService.CreateJobQuery()
                /*.Timers()*/
                .First();

            managementService.ExecuteJob(secondTimer.Id);

            assertProcessEnded(processInstance.Id);

            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(2, recordedEvents.Count);

            Assert.AreEqual("timer1", recordedEvents[0].ActivityId);
            Assert.AreEqual("start boundary listener", recordedEvents[0].Parameter);
            Assert.AreEqual("start", recordedEvents[0].EventName);
            Assert.That(recordedEvents[0].Canceled, Is.EqualTo(false));

            Assert.AreEqual("timer2", recordedEvents[1].ActivityId);
            Assert.AreEqual("end boundary listener", recordedEvents[1].Parameter);
            Assert.AreEqual("end", recordedEvents[1].EventName);
            Assert.That(recordedEvents[1].Canceled, Is.EqualTo(false));
        }

        [Test]
        [Deployment]
        public virtual void testScriptListener()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.True(processInstance.IsEnded);


            if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >=
                ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var query = historyService.CreateHistoricVariableInstanceQuery();
                var Count = query.Count();
                Assert.AreEqual(5, Count);

                IHistoricVariableInstance variableInstance = null;
                string[] variableNames = { "start-start", "start-end", "start-take", "end-start", "end-end" };
                foreach (var VariableName in variableNames)
                {
                    variableInstance = query/*.VariableName(VariableName)*/
                        .First();
                    Assert.NotNull(variableInstance, "Unable ot find variable with name '" + VariableName + "'");
                    Assert.True((bool)variableInstance.Value, "Variable '" + VariableName + "' should be set to true");
                }
            }
        }

        [Test]
        [Deployment(new[] { "resources/bpmn/executionlistener/ExecutionListenerTest.testScriptResourceListener.bpmn20.xml", "resources/bpmn/executionlistener/executionListener.groovy" })]
        public virtual void testScriptResourceListener()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.True(processInstance.IsEnded);

            if (processEngineRule.ProcessEngineConfiguration.HistoryLevel.Id >=
                ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var query = historyService.CreateHistoricVariableInstanceQuery();
                var Count = query.Count();
                Assert.AreEqual(5, Count);

                IHistoricVariableInstance variableInstance = null;
                string[] variableNames = { "start-start", "start-end", "start-take", "end-start", "end-end" };
                foreach (var VariableName in variableNames)
                {
                    variableInstance = query/*.VariableName(VariableName)*/
                        .First();
                    Assert.NotNull(variableInstance, "Unable ot find variable with name '" + VariableName + "'");
                    Assert.True((bool)variableInstance.Value, "Variable '" + VariableName + "' should be set to true");
                }
            }
        }
        [Test]
        [Deployment]
        public virtual void testExecutionListenerOnTerminateEndEvent()
        {
            RecorderExecutionListener.Clear();

            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);

            var recordedEvents = RecorderExecutionListener.RecordedEvents;

            Assert.AreEqual(2, recordedEvents.Count);

            Assert.AreEqual("start", recordedEvents[0].EventName);
            Assert.AreEqual("end", recordedEvents[1].EventName);
        }

        [Test]
        [Deployment("resources/bpmn/executionlistener/ExecutionListenerTest.testOnCancellingBoundaryEvent.bpmn")]
        public virtual void testOnCancellingBoundaryEvents()
        {
            RecorderExecutionListener.Clear();

            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            var timer = managementService.CreateJobQuery()
                /*.Timers()*/
                .First();

            managementService.ExecuteJob(timer.Id);

            assertProcessEnded(processInstance.Id);

            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            //Assert.That(recordedEvents, hasSize(1));

            Assert.AreEqual("UserTask_1", recordedEvents[0].ActivityId);
            Assert.AreEqual("end", recordedEvents[0].EventName);
            Assert.That(recordedEvents[0].Canceled, Is.EqualTo(true));
        }

        private const string MESSAGE = "cancelMessage";
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        public static readonly IBpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent()
                .ParallelGateway("fork")
                .UserTask("userTask1")
                .ServiceTask("sendTask")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, typeof(SendMessageDelegate).FullName)
                //.CamundaExpression("${true}")
                .EndEvent("endEvent")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    typeof(RecorderExecutionListener).FullName)
                //.MoveToLastGateway()
                .UserTask("userTask2")
                .BoundaryEvent("boundaryEvent")
                .Message(MESSAGE)
                .EndEvent("endBoundaryEvent")
                //.MoveToNode("userTask2")
                .EndEvent()
                .Done();

        [Test]
        public virtual void testServiceTaskExecutionListenerCall()
        {
            testHelper.Deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER);
            runtimeService.StartProcessInstanceByKey("Process");
            var task = taskService.CreateTaskQuery(c => c.TaskDefinitionKeyWithoutCascade == "userTask1")
                .First();
            taskService.Complete(task.Id);

            Assert.AreEqual(0, taskService.CreateTaskQuery()
                .Count());
            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(1, recordedEvents.Count);
            Assert.AreEqual("endEvent", recordedEvents[0].ActivityId);
        }

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        public static readonly IBpmnModelInstance PROCESS_SERVICE_TASK_WITH_TWO_EXECUTION_START_LISTENER =
            ModifiableBpmnModelInstance.Modify(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER)
            //.ActivityBuilder("sendTask")
            //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
            //    typeof(RecorderExecutionListener).FullName)
            //.Done()
            ;

        [Test]
        public virtual void testServiceTaskTwoExecutionListenerCall()
        {
            testHelper.Deploy(PROCESS_SERVICE_TASK_WITH_TWO_EXECUTION_START_LISTENER);
            runtimeService.StartProcessInstanceByKey("Process");
            var task = taskService.CreateTaskQuery(c => c.TaskDefinitionKeyWithoutCascade == "userTask1")
                .First();
            taskService.Complete(task.Id);

            Assert.AreEqual(0, taskService.CreateTaskQuery()
                .Count());
            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(2, recordedEvents.Count);
            Assert.AreEqual("sendTask", recordedEvents[0].ActivityId);
            Assert.AreEqual("endEvent", recordedEvents[1].ActivityId);
        }

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        public static readonly IBpmnModelInstance PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS = GetDone();


        private static IBpmnModelInstance GetDone()
        {
            var task = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                      .StartEvent()
                      .UserTask("userTask")
                      .ServiceTask("sendTask");
            task
                    .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                        typeof(SendMessageDelegate).FullName)
                    .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                        typeof(RecorderExecutionListener).FullName);
            var model = task.CamundaExpression("${true}")
             .EndEvent("endEvent")
             .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                 typeof(RecorderExecutionListener).FullName)
             .Done();
            var start = ModifiableBpmnModelInstance.Modify(model)
            .AddSubProcessTo("Process")
             .TriggerByEvent()
             .EmbeddedSubProcess()
             .StartEvent("startSubProcess");
            start.Interrupting(false)
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                typeof(RecorderExecutionListener).FullName);
           return start.Message(MESSAGE)
            .UserTask("subProcessTask")
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                typeof(RecorderExecutionListener).FullName)
            .EndEvent("endSubProcess")
            .Done();

        }

        [Test]
        public virtual void testServiceTaskExecutionListenerCallAndSubProcess()
        {
            testHelper.Deploy(PROCESS_SERVICE_TASK_WITH_EXECUTION_START_LISTENER_AND_SUB_PROCESS);
            runtimeService.StartProcessInstanceByKey("Process");
            var task = taskService.CreateTaskQuery(c => c.TaskDefinitionKeyWithoutCascade == "userTask")
                .First();
            taskService.Complete(task.Id);

            Assert.AreEqual(1, taskService.CreateTaskQuery()
                .Count());

            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(4, recordedEvents.Count);
            Assert.AreEqual("startSubProcess", recordedEvents[0].ActivityId);
            Assert.AreEqual("subProcessTask", recordedEvents[1].ActivityId);
            Assert.AreEqual("sendTask", recordedEvents[2].ActivityId);
            Assert.AreEqual("endEvent", recordedEvents[3].ActivityId);
        }

        public class SendMessageDelegate : IJavaDelegate
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                IRuntimeService runtimeService = ((IDelegateExecution)execution).ProcessEngineServices.RuntimeService;
                runtimeService.CorrelateMessage(MESSAGE);
            }
        }

        [Test]
        public virtual void testEndExecutionListenerIsCalledOnlyOnce()
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
            IBpmnModelInstance modelInstance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("conditionalProcessKey")
                .StartEvent()
                .UserTask()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd, typeof(SetVariableDelegate).FullName)
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RecorderExecutionListener).FullName)
                .EndEvent()
                .Done();

            modelInstance = ModifiableBpmnModelInstance.Modify(modelInstance)
                .AddSubProcessTo("conditionalProcessKey")
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent()
                //.Interrupting(true)
                //.ConditionalEventDefinition()
                //.Condition("${variable == 1}")
                //.ConditionalEventDefinitionDone()
                .EndEvent()
                .Done();

            testHelper.Deploy(modelInstance);

            // given
            var procInst = runtimeService.StartProcessInstanceByKey("conditionalProcessKey");
            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery(c => c.ProcessInstanceId == procInst.Id);

            //when task is completed
            taskService.Complete(taskQuery.First()
                .Id);

            //then end listener sets variable and triggers conditional event
            //end listener should called only once
            Assert.AreEqual(1, RecorderExecutionListener.RecordedEvents.Count);
        }
    }
}