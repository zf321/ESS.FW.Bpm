using System.Linq;
using Engine.Tests.Api.Runtime.Migration;
using Engine.Tests.Bpmn.ExecutionListener;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    ///     Tests for when delegate code synchronously cancels the activity instance it belongs to.
    /// </summary>
    [TestFixture]
    public class SelfCancellationTest
    {
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
        }

        protected internal const string MESSAGE = "Message";

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(processEngineRule).around(testHelper);
        //public RuleChain ruleChain;

        //========================================================================================================================
        //=======================================================MODELS===========================================================
        //========================================================================================================================

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK = PROCESS_WITH_CANCELING_RECEIVE_TASK_Up();
           
        private static IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_Up()
        {
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent();
            start.ParallelGateway("fork")
               .UserTask();
            start.SendTask("sendTask")
               .CamundaClass(typeof(SendMessageDelegate).FullName);
            start.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName)
            .EndEvent("endEvent")
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                typeof(RecorderExecutionListener).FullName)
            .MoveToLastGateway();
            var task = start.ReceiveTask("receiveTask");
            task.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName);
              return task.Message(MESSAGE)
               .EndEvent("terminateEnd")
               .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                   typeof(RecorderExecutionListener).FullName)
               .Done();
        }
        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND =
                ModifiableBpmnModelInstance.Modify(PROCESS_WITH_CANCELING_RECEIVE_TASK)
                    .RemoveFlowNode("endEvent")
            //.ActivityBuilder("sendTask")
            //.UserTask("userTask")
            //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
            //    typeof(RecorderExecutionListener).FullName)
            //.EndEvent()
            //.Done()
            ;

        public static readonly IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND = ModifiableBpmnModelInstance
            .Modify(PROCESS_WITH_CANCELING_RECEIVE_TASK)
            .RemoveFlowNode("endEvent");

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE =
                ModifiableBpmnModelInstance.Modify(PROCESS_WITH_CANCELING_RECEIVE_TASK)
            //.ActivityBuilder("sendTask")
            //.BoundaryEvent("boundary")
            //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
            //    typeof(RecorderExecutionListener).FullName)
            //.TimerWithDuration("PT5S")
            //.EndEvent("endEventBoundary")
            //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
            //    typeof(RecorderExecutionListener).FullName)
            //.Done()
            ;


        public static readonly IBpmnModelInstance PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END = ModifiableBpmnModelInstance
            .Modify(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE)
            .RemoveFlowNode("endEvent");

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_SUBPROCESS_AND_DELEGATE_MSG_SEND =
            ModifiableBpmnModelInstance.Modify(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                    .StartEvent()
                    .SubProcess()
                    //.EmbeddedSubProcess()
                    //.StartEvent()
                    .UserTask()
                    .ServiceTask("sendTask")
                    .CamundaClass(typeof(SendMessageDelegate).FullName)
                    .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                        typeof(RecorderExecutionListener).FullName)
                    .EndEvent("endEventSubProc")
                    .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                        typeof(RecorderExecutionListener).FullName)
                    .SubProcessDone()
                    .EndEvent()
                    .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                        typeof(RecorderExecutionListener).FullName)
                    .Done())
                .AddSubProcessTo("process")
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent("startSubEvent")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RecorderExecutionListener).FullName)
                //.Message(MESSAGE)
                .EndEvent("endEventSubEvent")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RecorderExecutionListener).FullName)
                .Done();

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT = PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT_Up();


        private static IBpmnModelInstance PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT_Up()
        {
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                 .StartEvent();
            start.ParallelGateway("fork")
            .UserTask()
            .EndEvent()
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                typeof(RecorderExecutionListener).FullName)
            .MoveToLastGateway();
            var sendTask = start.SendTask("sendTask");
            sendTask.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName);
            sendTask.CamundaClass(typeof(SignalDelegate).FullName);
            return sendTask.BoundaryEvent("boundary")
            .Message(MESSAGE)
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName)
            .EndEvent("endEventBoundary")
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName)
            .MoveToNode("sendTask")
            .EndEvent("endEvent")
            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                typeof(RecorderExecutionListener).FullName)
            .Done();
        }

        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT = PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT_Up();


        private static IBpmnModelInstance PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT_Up()
        {
            var start = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                 .StartEvent();
            var sendTask = start.SendTask("sendTask");
            sendTask.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                typeof(RecorderExecutionListener).FullName);
            sendTask.CamundaClass(typeof(SignalDelegate).FullName);
               return sendTask.BoundaryEvent("boundary")
                .Message(MESSAGE)
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RecorderExecutionListener).FullName)
                .EndEvent("endEventBoundary")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(RecorderExecutionListener).FullName)
                .MoveToNode("sendTask")
                .EndEvent("endEvent")
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    typeof(RecorderExecutionListener).FullName)
                .Done();
        }
        private readonly bool InstanceFieldsInitialized;

        public ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();

        //========================================================================================================================
        //=======================================================TESTS============================================================
        //========================================================================================================================


        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;
        public ProcessEngineTestRule testHelper;


        //========================================================================================================================
        //=========================================================INIT===========================================================
        //========================================================================================================================

        static SelfCancellationTest()
        {
            initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK, "terminateEnd");
            initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND, "terminateEnd");
            initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE, "terminateEnd");
            initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND, "terminateEnd");
            initEndEvent(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END, "terminateEnd");
        }

        public SelfCancellationTest()
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

        public static void initEndEvent(IBpmnModelInstance modelInstance, string endEventId)
        {
            var endEvent = modelInstance.GetModelElementById/*<IEndEvent>*/(endEventId) as IEndEvent;
            var terminateDefinition =
                modelInstance.NewInstance<ITerminateEventDefinition>(typeof(ITerminateEventDefinition));
            endEvent.AddChildElement(terminateDefinition);
        }
        [Test]
        private void checkRecordedEvents(params string[] activityIds)
        {
            var recordedEvents = RecorderExecutionListener.RecordedEvents;
            Assert.AreEqual(activityIds.Length, recordedEvents.Count);

            for (var i = 0; i < activityIds.Length; i++)
                Assert.AreEqual(activityIds[i], recordedEvents[i].ActivityId);
        }

        [Test]
        private void testParallelTerminationWithSend(IBpmnModelInstance modelInstance)
        {
            // given
            testHelper.Deploy(modelInstance);
            runtimeService.StartProcessInstanceByKey("process");

            var task = taskService.CreateTaskQuery()
                .First();

            // when
            taskService.Complete(task.Id);

            // then
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
            checkRecordedEvents("receiveTask", "sendTask", "terminateEnd");
        }

        [Test]
        public virtual void testTriggerParallelTerminateEndEvent()
        {
            testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK);
        }

        [Test]
        public virtual void testTriggerParallelTerminateEndEventWithUserTask()
        {
            testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_AND_USER_TASK_AFTER_SEND);
        }

        [Test]
        public virtual void testTriggerParallelTerminateEndEventWithoutEndAfterSend()
        {
            testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITHOUT_END_AFTER_SEND);
        }

        [Test]
        public virtual void testTriggerParallelTerminateEndEventWithSendAsScope()
        {
            testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE);
        }

        [Test]
        public virtual void testTriggerParallelTerminateEndEventWithSendAsScopeWithoutEnd()
        {
            testParallelTerminationWithSend(PROCESS_WITH_CANCELING_RECEIVE_TASK_WITH_SEND_AS_SCOPE_WITHOUT_END);
        }

        [Test]
        public virtual void testSendMessageInSubProcess()
        {
            // given
            testHelper.Deploy(PROCESS_WITH_SUBPROCESS_AND_DELEGATE_MSG_SEND);
            runtimeService.StartProcessInstanceByKey("process");

            var task = taskService.CreateTaskQuery()
                .First();

            // when
            taskService.Complete(task.Id);

            // then
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery()
                .Count());
            checkRecordedEvents("sendTask", "startSubEvent", "endEventSubEvent");
        }

        [Test]
        public virtual void testParallelSendTaskWithBoundaryRecieveTask()
        {
            // given
            testHelper.Deploy(PROCESS_WITH_PARALLEL_SEND_TASK_AND_BOUNDARY_EVENT);
            var procInst = runtimeService.StartProcessInstanceByKey("process");

            var activity = runtimeService.CreateExecutionQuery(c => c.ActivityId == "sendTask")
                .First();
            runtimeService.Signal(activity.Id);

            // then
            var activities = runtimeService.GetActiveActivityIds(procInst.Id);
            Assert.NotNull(activities);
            Assert.AreEqual(1, activities.Count);
            checkRecordedEvents("sendTask", "boundary", "endEventBoundary");
        }

        [Test]
        public virtual void testSendTaskWithBoundaryEvent()
        {
            // given
            testHelper.Deploy(PROCESS_WITH_SEND_TASK_AND_BOUNDARY_EVENT);
            runtimeService.StartProcessInstanceByKey("process");

            var activity = runtimeService.CreateExecutionQuery(c => c.ActivityId == "sendTask")
                .First();
            runtimeService.Signal(activity.Id);

            // then
            checkRecordedEvents("sendTask", "boundary", "endEventBoundary");
        }

        //========================================================================================================================
        //===================================================STATIC CLASSES=======================================================
        //========================================================================================================================
        public class SendMessageDelegate : IJavaDelegate
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                var runtimeService = ((IDelegateExecution)execution).ProcessEngineServices.RuntimeService;
                runtimeService.CorrelateMessage(MESSAGE);
            }
        }

        public class SignalDelegate : ISignallableActivityBehavior
        {
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.impl.pvm.Delegate.ActivityExecution execution) throws Exception
            public void Execute(IActivityExecution execution)
            {
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void signal(org.Camunda.bpm.Engine.impl.pvm.Delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
            public void Signal(IActivityExecution execution, string signalEvent, object signalData)
            {
                var runtimeService = execution.ProcessEngineServices.RuntimeService;
                runtimeService.CorrelateMessage(MESSAGE);
            }
        }
    }
}