using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationPlanGenerationTest
    {
        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string TIMER_DATE = "2016-02-11T12:13:14Z";
        public const string ERROR_CODE = "Error";
        public const string ESCALATION_CODE = "Escalation";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationPlanGenerationTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new MigrationTestRule(rule);
            //ruleChain = RuleChain.outerRule(rule).around(testHelper);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [Test]
        public virtual void testMapEqualActivitiesInProcessDefinitionScope()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            var targetProcess = ProcessModels.OneTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEqualActivitiesInSameSubProcessScope()
        {
            var sourceProcess = ProcessModels.SubprocessProcess;
            var targetProcess = ProcessModels.SubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToSubProcessScope()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            var targetProcess = ProcessModels.SubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesToNestedSubProcessScope()
        {
            var sourceProcess = ProcessModels.SubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.DoubleSubprocessProcess)
                .ChangeElementId("outerSubProcess", "subProcess");
            // make ID match with subprocess ID of source definition

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToSurroundingSubProcessScope()
        {
            var sourceProcess = ProcessModels.SubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.DoubleSubprocessProcess)
                .ChangeElementId("innerSubProcess", "subProcess");
            // make ID match with subprocess ID of source definition

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesToDeeplyNestedSubProcessScope()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            var targetProcess = ProcessModels.DoubleSubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesToSiblingScope()
        {
            var sourceProcess = ProcessModels.ParallelSubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.ParallelSubprocessProcess)
                .SwapElementIds("userTask1", "userTask2");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess1")
                    .To("subProcess1"), MigrationPlanAssert.Migrate("subProcess2")
                    .To("subProcess2"), MigrationPlanAssert.Migrate("fork")
                    .To("fork"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToNestedSiblingScope()
        {
            var sourceProcess = ProcessModels.ParallelDoubleSubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.ParallelDoubleSubprocessProcess)
                .SwapElementIds("userTask1", "userTask2");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess1")
                    .To("subProcess1"), MigrationPlanAssert.Migrate("nestedSubProcess1")
                    .To("nestedSubProcess1"), MigrationPlanAssert.Migrate("subProcess2")
                    .To("subProcess2"), MigrationPlanAssert.Migrate("nestedSubProcess2")
                    .To("nestedSubProcess2"), MigrationPlanAssert.Migrate("fork")
                    .To("fork"));
        }

        [Test]
        public virtual void testMapEqualActivitiesWhichBecomeScope()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            var targetProcess = ProcessModels.ScopeTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEqualActivitiesWithParallelMultiInstance()
        {
            var testProcess = (ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .GetModelElementById/*<IUserTask>*/("userTask") as IUserTask)
                .Builder()
                .MultiInstance()
                .Parallel()
                .Cardinality("3")
                //.MultiInstanceDone()
                .Done();

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("userTask#multiInstanceBody")
                    .To("userTask#multiInstanceBody"));
        }

        [Test]
        public virtual void testMapEqualActivitiesIgnoreUnsupportedActivities()
        {
            var sourceProcess = ProcessModels.UnsupportedActivities;
            var targetProcess = ProcessModels.UnsupportedActivities;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualUnsupportedAsyncBeforeActivities()
        {
            IBpmnModelInstance testModel = ModifiableBpmnModelInstance.Modify(ProcessModels.UnsupportedActivities)
                //.FlowNodeBuilder("startEvent")
                //.CamundaAsyncBefore()
                //.MoveToNode("decisionTask")
                //.CamundaAsyncBefore()
                //.MoveToNode("throwEvent")
                //.CamundaAsyncAfter()
                //.MoveToNode("serviceTask")
                //.CamundaAsyncBefore()
                //.MoveToNode("sendTask")
                //.CamundaAsyncBefore()
                //.MoveToNode("scriptTask")
                //.CamundaAsyncBefore()
                //.MoveToNode("endEvent")
                //.CamundaAsyncBefore()
                //.Done()
                ;

            AssertGeneratedMigrationPlan(testModel, testModel)
                .HasInstructions(MigrationPlanAssert.Migrate("startEvent")
                    .To("startEvent"), MigrationPlanAssert.Migrate("decisionTask")
                    .To("decisionTask"), MigrationPlanAssert.Migrate("throwEvent")
                    .To("throwEvent"), MigrationPlanAssert.Migrate("serviceTask")
                    .To("serviceTask"), MigrationPlanAssert.Migrate("sendTask")
                    .To("sendTask"), MigrationPlanAssert.Migrate("scriptTask")
                    .To("scriptTask"), MigrationPlanAssert.Migrate("endEvent")
                    .To("endEvent"));
        }

        [Test]
        public virtual void testMapEqualUnsupportedAsyncAfterActivities()
        {
            IBpmnModelInstance testModel = ModifiableBpmnModelInstance.Modify(ProcessModels.UnsupportedActivities)
                //.FlowNodeBuilder("startEvent")
                //.CamundaAsyncAfter()
                //.MoveToNode("decisionTask")
                //.CamundaAsyncAfter()
                //.MoveToNode("throwEvent")
                //.CamundaAsyncAfter()
                //.MoveToNode("serviceTask")
                //.CamundaAsyncAfter()
                //.MoveToNode("sendTask")
                //.CamundaAsyncAfter()
                //.MoveToNode("scriptTask")
                //.CamundaAsyncAfter()
                //.MoveToNode("endEvent")
                //.CamundaAsyncAfter()
                //.Done()
                ;

            AssertGeneratedMigrationPlan(testModel, testModel)
                .HasInstructions(MigrationPlanAssert.Migrate("startEvent")
                    .To("startEvent"), MigrationPlanAssert.Migrate("decisionTask")
                    .To("decisionTask"), MigrationPlanAssert.Migrate("throwEvent")
                    .To("throwEvent"), MigrationPlanAssert.Migrate("serviceTask")
                    .To("serviceTask"), MigrationPlanAssert.Migrate("sendTask")
                    .To("sendTask"), MigrationPlanAssert.Migrate("scriptTask")
                    .To("scriptTask"), MigrationPlanAssert.Migrate("endEvent")
                    .To("endEvent"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToParentScope()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.DoubleSubprocessProcess)
                .ChangeElementId("outerSubProcess", "subProcess");
            var targetProcess = ProcessModels.SubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"));
        }

        [Test]
        public virtual void testMapEqualActivitiesFromScopeToProcessDefinition()
        {
            var sourceProcess = ProcessModels.SubprocessProcess;
            var targetProcess = ProcessModels.OneTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesFromDoubleScopeToProcessDefinition()
        {
            var sourceProcess = ProcessModels.DoubleSubprocessProcess;
            var targetProcess = ProcessModels.OneTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesFromTripleScopeToProcessDefinition()
        {
            var sourceProcess = ProcessModels.TripleSubprocessProcess;
            var targetProcess = ProcessModels.OneTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesFromTripleScopeToSingleNewScope()
        {
            var sourceProcess = ProcessModels.TripleSubprocessProcess;
            var targetProcess = ProcessModels.SubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesFromTripleScopeToTwoNewScopes()
        {
            var sourceProcess = ProcessModels.TripleSubprocessProcess;
            var targetProcess = ProcessModels.DoubleSubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }
        [Test]
        public virtual void testMapEqualActivitiesToNewScopes()
        {
            var sourceProcess = ProcessModels.DoubleSubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    ProcessModels.DoubleSubprocessProcess)
                .ChangeElementId("outerSubProcess", "newOuterSubProcess")
                .ChangeElementId("innerSubProcess", "newInnerSubProcess");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapEqualActivitiesOutsideOfScope()
        {
            var sourceProcess = ProcessModels.ParallelGatewaySubprocessProcess;
            var targetProcess = ProcessModels.ParallelTaskAndSubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask1")
                    .To("userTask1"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToHorizontalScope()
        {
            var sourceProcess = ProcessModels.ParallelTaskAndSubprocessProcess;
            var targetProcess = ProcessModels.ParallelGatewaySubprocessProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask1")
                    .To("userTask1"));
        }

        [Test]
        public virtual void testMapEqualActivitiesFromTaskWithBoundaryEvent()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent(null)
                //.Message("Message")
                //.Done()
                ;
            var targetProcess = ProcessModels.OneTaskProcess;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEqualActivitiesToTaskWithBoundaryEvent()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent(null)
                //.Message("Message")
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEqualActivitiesWithBoundaryEvent()
        {
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("messageBoundary")
                ////.Message(MESSAGE_NAME)
                //.MoveToActivity("userTask")
                //.BoundaryEvent("signalBoundary")
                //.Signal(SIGNAL_NAME)
                //.MoveToActivity("userTask")
                //.BoundaryEvent("timerBoundary")
                //.TimerWithDate(TIMER_DATE)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("messageBoundary")
                    .To("messageBoundary"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("signalBoundary")
                    .To("signalBoundary"), MigrationPlanAssert.Migrate("timerBoundary")
                    .To("timerBoundary"));
        }

        [Test]
        public virtual void testNotMapBoundaryEventsWithDifferentIds()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("message")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("message", "newMessage");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testIgnoreNotSupportedBoundaryEvents()
        {
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("messageBoundary")
                ////.Message(MESSAGE_NAME)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("errorBoundary")
                //.Error(ERROR_CODE)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("escalationBoundary")
                //.Escalation(ESCALATION_CODE)
                //.MoveToActivity("userTask")
                //.BoundaryEvent("signalBoundary")
                //.Signal(SIGNAL_NAME)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("messageBoundary")
                    .To("messageBoundary"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("signalBoundary")
                    .To("signalBoundary"));
        }

        [Test]
        public virtual void testNotMigrateBoundaryToParallelActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask1")
                //.BoundaryEvent("message")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask2")
                //.BoundaryEvent("message")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask1")
                    .To("userTask1"), MigrationPlanAssert.Migrate("userTask2")
                    .To("userTask2"), MigrationPlanAssert.Migrate("fork")
                    .To("fork"));
        }

        [Test]
        public virtual void testNotMigrateBoundaryToChildActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("message")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("message")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMigrateProcessInstanceWithEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                //.Id("eventSubProcess")
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent("eventSubProcessStart")
                //.Message(MESSAGE_NAME)
                .EndEvent()
                .SubProcessDone()
                .Done();

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(testProcess, ProcessModels.OneTaskProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(ProcessModels.OneTaskProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMigrateSubProcessWithEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo("subProcess")
                //.Id("eventSubProcess")
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent("eventSubProcessStart")
                //.Message(MESSAGE_NAME)
                .EndEvent()
                .SubProcessDone()
                .Done();

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(testProcess, ProcessModels.SubprocessProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(ProcessModels.SubprocessProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }
        [Test]
        public virtual void testMigrateUserTaskInEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo("subProcess")
                //.Id("eventSubProcess")
                .TriggerByEvent()
                //////.EmbeddedSubProcess()
                ////.StartEvent("eventSubProcessStart")
                ////.Message(MESSAGE_NAME)
                .UserTask("innerTask")
                .EndEvent()
                .SubProcessDone()
                .Done();

            AssertGeneratedMigrationPlan(testProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("innerTask")
                    .To("innerTask"), MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(testProcess, ProcessModels.SubprocessProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));

            AssertGeneratedMigrationPlan(ProcessModels.SubprocessProcess, testProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testNotMigrateActivitiesOfDifferentType()
        {
            var sourceProcess = ProcessModels.OneTaskProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .SwapElementIds("userTask", "subProcess");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testNotMigrateBoundaryEventsOfDifferentType()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                //.Signal(SIGNAL_NAME)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testNotMigrateMultiInstanceOfDifferentType()
        {
            var sourceProcess = MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS;
            var targetProcess = MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testNotMigrateBoundaryEventsWithInvalidEventScopeInstruction()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                //.Message("foo")
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneReceiveTaskProcess)
                //.ChangeElementId("receiveTask", "userTask")
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                //.Message("foo")
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapReceiveTasks()
        {
            AssertGeneratedMigrationPlan(MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS,
                    MessageReceiveModels.ONE_RECEIVE_TASK_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("receiveTask")
                    .To("receiveTask"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapMessageCatchEvents()
        {
            AssertGeneratedMigrationPlan(MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS,
                    MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("messageCatch")
                    .To("messageCatch"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapCallActivitiesToBpmnTest()
        {
            AssertGeneratedMigrationPlan(CallActivityModels.oneBpmnCallActivityProcess("foo"),
                    CallActivityModels.oneBpmnCallActivityProcess("foo"))
                .HasInstructions(MigrationPlanAssert.Migrate("callActivity")
                    .To("callActivity"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapCallActivitiesToCmmnTest()
        {
            AssertGeneratedMigrationPlan(CallActivityModels.oneCmmnCallActivityProcess("foo"),
                    CallActivityModels.oneCmmnCallActivityProcess("foo"))
                .HasInstructions(MigrationPlanAssert.Migrate("callActivity")
                    .To("callActivity"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapCallActivitiesFromBpmnToCmmnTest()
        {
            AssertGeneratedMigrationPlan(CallActivityModels.oneBpmnCallActivityProcess("foo"),
                    CallActivityModels.oneCmmnCallActivityProcess("foo"))
                .HasInstructions(MigrationPlanAssert.Migrate("callActivity")
                    .To("callActivity"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapCallActivitiesFromCmmnToBpmnTest()
        {
            AssertGeneratedMigrationPlan(CallActivityModels.oneCmmnCallActivityProcess("foo"),
                    CallActivityModels.oneBpmnCallActivityProcess("foo"))
                .HasInstructions(MigrationPlanAssert.Migrate("callActivity")
                    .To("callActivity"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEventBasedGateway()
        {
            AssertGeneratedMigrationPlan(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS,
                    EventBasedGatewayModels.SIGNAL_EVENT_BASED_GW_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("eventBasedGateway")
                    .To("eventBasedGateway"));
        }

        [Test]
        public virtual void testMapEventBasedGatewayWithIdenticalFollowingEvents()
        {
            AssertGeneratedMigrationPlan(EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS,
                    EventBasedGatewayModels.TIMER_EVENT_BASED_GW_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("eventBasedGateway")
                    .To("eventBasedGateway"), MigrationPlanAssert.Migrate("timerCatch")
                    .To("timerCatch"), MigrationPlanAssert.Migrate("afterTimerCatch")
                    .To("afterTimerCatch"));
        }

        // event sub process

        [Test]
        public virtual void testMapTimerEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapMessageEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapSignalEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapEscalationEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.ESCALATION_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapErrorEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.ERROR_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapCompensationEventSubProcessAndStartEvent()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"));
        }

        [Test]
        public virtual void testNotMapEventSubProcessStartEventOfDifferentType()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.SIGNAL_EVENT_SUBPROCESS_PROCESS)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcess")
                    .To("eventSubProcess"), MigrationPlanAssert.Migrate("eventSubProcessTask")
                    .To("eventSubProcessTask"));
        }

        [Test]
        public virtual void testMapEventSubProcessStartEventWhenSubProcessesAreNotEqual()
        {
            var sourceModel = EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS;
            IBpmnModelInstance targetModel = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS)
                .ChangeElementId("eventSubProcess", "newEventSubProcess");

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart"));
        }

        [Test]
        public virtual void testMapEventSubProcessToEmbeddedSubProcess()
        {
            IBpmnModelInstance sourceModel = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS)
                .ChangeElementId("eventSubProcess", "subProcess");
            var targetModel = ProcessModels.SubprocessProcess;

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"));
        }

        [Test]
        public virtual void testMapEmbeddedSubProcessToEventSubProcess()
        {
            var sourceModel = ProcessModels.SubprocessProcess;
            IBpmnModelInstance targetModel = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS)
                .ChangeElementId("eventSubProcess", "subProcess");

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"));
        }

        [Test]
        public virtual void testMapExternalServiceTask()
        {
            var sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
            var targetModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasInstructions(MigrationPlanAssert.Migrate("externalTask")
                    .To("externalTask"));
        }

        [Test]
        public virtual void testMapExternalServiceToDifferentType()
        {
            var sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
            var targetModel = ProcessModels.NewModel()
                .StartEvent()
                .SendTask("externalTask")
                .CamundaType("external")
                .CamundaTopic("foo")
                .EndEvent()
                .Done();

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasInstructions(MigrationPlanAssert.Migrate("externalTask")
                    .To("externalTask"));
        }

        [Test]
        public virtual void testNotMapExternalToClassDelegateServiceTask()
        {
            var sourceModel = ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS;
            IBpmnModelInstance targetModel = ModifiableBpmnModelInstance.Modify(
                    ServiceTaskModels.oneClassDelegateServiceTask("foo.Bar"))
                .ChangeElementId("serviceTask", "externalTask");

            AssertGeneratedMigrationPlan(sourceModel, targetModel)
                .HasEmptyInstructions();
        }

        [Test]
        public virtual void testMapParallelGateways()
        {
            var model = GatewayModels.PARALLEL_GW;

            AssertGeneratedMigrationPlan(model, model)
                .HasInstructions(MigrationPlanAssert.Migrate("fork")
                    .To("fork"), MigrationPlanAssert.Migrate("join")
                    .To("join"), MigrationPlanAssert.Migrate("parallel1")
                    .To("parallel1"), MigrationPlanAssert.Migrate("parallel2")
                    .To("parallel2"), MigrationPlanAssert.Migrate("afterJoin")
                    .To("afterJoin"));
        }

        [Test]
        public virtual void testMapInclusiveGateways()
        {
            var model = GatewayModels.INCLUSIVE_GW;

            AssertGeneratedMigrationPlan(model, model)
                .HasInstructions(MigrationPlanAssert.Migrate("fork")
                    .To("fork"), MigrationPlanAssert.Migrate("join")
                    .To("join"), MigrationPlanAssert.Migrate("parallel1")
                    .To("parallel1"), MigrationPlanAssert.Migrate("parallel2")
                    .To("parallel2"), MigrationPlanAssert.Migrate("afterJoin")
                    .To("afterJoin"));
        }

        [Test]
        public virtual void testNotMapParallelToInclusiveGateway()
        {
            AssertGeneratedMigrationPlan(GatewayModels.PARALLEL_GW, GatewayModels.INCLUSIVE_GW)
                .HasInstructions(MigrationPlanAssert.Migrate("parallel1")
                    .To("parallel1"), MigrationPlanAssert.Migrate("parallel2")
                    .To("parallel2"), MigrationPlanAssert.Migrate("afterJoin")
                    .To("afterJoin"));
        }

        [Test]
        public virtual void testMapTransaction()
        {
            AssertGeneratedMigrationPlan(TransactionModels.ONE_TASK_TRANSACTION, TransactionModels.ONE_TASK_TRANSACTION)
                .HasInstructions(MigrationPlanAssert.Migrate("transaction")
                    .To("transaction"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapEmbeddedSubProcessToTransaction()
        {
            var sourceProcess = ProcessModels.SubprocessProcess;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(TransactionModels.ONE_TASK_TRANSACTION)
                .ChangeElementId("transaction", "subProcess");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapTransactionToEventSubProcess()
        {
            var sourceProcess = TransactionModels.ONE_TASK_TRANSACTION;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(
                    EventSubProcessModels.MESSAGE_EVENT_SUBPROCESS_PROCESS)
                .ChangeElementId("eventSubProcess", "transaction")
                .ChangeElementId("userTask", "foo")
                .ChangeElementId("eventSubProcessTask", "userTask");

            AssertGeneratedMigrationPlan(sourceProcess, targetProcess)
                .HasInstructions(MigrationPlanAssert.Migrate("transaction")
                    .To("transaction"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapNoUpdateEventTriggers()
        {
            var model = MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS;

            AssertGeneratedMigrationPlan(model, model, false)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("messageCatch")
                    .To("messageCatch")
                    .UpdateEventTrigger(false));
        }

        [Test]
        public virtual void testMapUpdateEventTriggers()
        {
            var model = MessageReceiveModels.ONE_MESSAGE_CATCH_PROCESS;

            AssertGeneratedMigrationPlan(model, model, true)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("messageCatch")
                    .To("messageCatch")
                    .UpdateEventTrigger(true));
        }

        [Test]
        public virtual void testMigrationPlanCreationWithEmptyDeploymentCache()
        {
            // given
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            rule.ProcessEngineConfiguration.DeploymentCache.DiscardProcessDefinitionCache();

            // when
            var migrationPlan = rule.RuntimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapEqualActivities()
                .Build();

            // then
            Assert.NotNull(migrationPlan);
        }

        [Test]
        public virtual void testMapCompensationBoundaryEvents()
        {
            AssertGeneratedMigrationPlan(CompensationModels.ONE_COMPENSATION_TASK_MODEL,
                    CompensationModels.ONE_COMPENSATION_TASK_MODEL, true)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask1")
                    .To("userTask1")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("userTask2")
                    .To("userTask2")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("compensationBoundary")
                    .To("compensationBoundary")
                    .UpdateEventTrigger(false));
        }

        [Test]
        public virtual void testMapCompensationStartEvents()
        {
            AssertGeneratedMigrationPlan(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL,
                    CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL, true)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("userTask1")
                    .To("userTask1")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("userTask2")
                    .To("userTask2")
                    .UpdateEventTrigger(false), MigrationPlanAssert.Migrate("compensationBoundary")
                    .To("compensationBoundary")
                    .UpdateEventTrigger(false));

            // should not map eventSubProcess because it active compensation is not supported
        }

        [Test]
        public virtual void testMapIntermediateConditionalEvent()
        {
            var sourceProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalModels.CONDITIONAL_PROCESS_KEY)
                .StartEvent()
                .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
                .Condition(EventSubProcessModels.VAR_CONDITION)
                .UserTask(EventSubProcessModels.USER_TASK_ID)
                .EndEvent()
                .Done();

            AssertGeneratedMigrationPlan(sourceProcess, sourceProcess, false)
                .HasInstructions(MigrationPlanAssert.Migrate(ConditionalModels.CONDITION_ID)
                    .To(ConditionalModels.CONDITION_ID)
                    .UpdateEventTrigger(true), MigrationPlanAssert.Migrate(EventSubProcessModels.USER_TASK_ID)
                    .To(EventSubProcessModels.USER_TASK_ID)
                    .UpdateEventTrigger(false));
        }

        [Test]
        public virtual void testMapConditionalEventSubProcess()
        {
            AssertGeneratedMigrationPlan(EventSubProcessModels.FALSE_CONDITIONAL_EVENT_SUBPROCESS_PROCESS,
                    EventSubProcessModels.CONDITIONAL_EVENT_SUBPROCESS_PROCESS, false)
                .HasInstructions(MigrationPlanAssert.Migrate(EventSubProcessModels.EVENT_SUB_PROCESS_START_ID)
                        .To(EventSubProcessModels.EVENT_SUB_PROCESS_START_ID)
                        .UpdateEventTrigger(true), MigrationPlanAssert.Migrate(EventSubProcessModels.EVENT_SUB_PROCESS_ID)
                        .To(EventSubProcessModels.EVENT_SUB_PROCESS_ID)
                        .UpdateEventTrigger(false),
                    MigrationPlanAssert.Migrate(EventSubProcessModels.EVENT_SUB_PROCESS_TASK_ID)
                        .To(EventSubProcessModels.EVENT_SUB_PROCESS_TASK_ID),
                    MigrationPlanAssert.Migrate(EventSubProcessModels.USER_TASK_ID)
                        .To(EventSubProcessModels.USER_TASK_ID));
        }

        [Test]
        public virtual void testMapConditionalBoundaryEvents()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder(EventSubProcessModels.USER_TASK_ID)
                //.BoundaryEvent(ConditionalModels.BOUNDARY_ID)
                //.Condition(EventSubProcessModels.VAR_CONDITION)
                //.Done()
                ;

            AssertGeneratedMigrationPlan(sourceProcess, sourceProcess, false)
                .HasInstructions(MigrationPlanAssert.Migrate(ConditionalModels.BOUNDARY_ID)
                    .To(ConditionalModels.BOUNDARY_ID)
                    .UpdateEventTrigger(true), MigrationPlanAssert.Migrate(EventSubProcessModels.USER_TASK_ID)
                    .To(EventSubProcessModels.USER_TASK_ID)
                    .UpdateEventTrigger(false));
        }

        // helper

        protected internal virtual MigrationPlanAssert AssertGeneratedMigrationPlan(IBpmnModelInstance sourceProcess,
            IBpmnModelInstance targetProcess)
        {
            return AssertGeneratedMigrationPlan(sourceProcess, targetProcess, false);
        }

        protected internal virtual MigrationPlanAssert AssertGeneratedMigrationPlan(IBpmnModelInstance sourceProcess,
            IBpmnModelInstance targetProcess, bool updateEventTriggers)
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationInstructionsBuilder =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities();

            if (updateEventTriggers)
                migrationInstructionsBuilder.UpdateEventTriggers();

            var migrationPlan = migrationInstructionsBuilder.Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition);

            return MigrationPlanAssert.That(migrationPlan);
        }
    }
}