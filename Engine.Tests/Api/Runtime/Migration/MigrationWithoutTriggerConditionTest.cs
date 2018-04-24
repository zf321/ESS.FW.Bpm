using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Bpmn.Event.Conditional;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationWithoutTriggerConditionTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationWithoutTriggerConditionTest()
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
        public virtual void testIntermediateConditionalEventWithSetVariableOnEndListener()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .StartEvent()
                .SubProcess()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    typeof(SetVariableDelegate).FullName)
                //.EmbeddedSubProcess()
                //.StartEvent()
                .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
                .ConditionalEventDefinition()
                //.Condition(ConditionalModels.VAR_CONDITION)
                //.ConditionalEventDefinitionDone()
                //.UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                //.EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done());
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .StartEvent()
                .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
                .ConditionalEventDefinition()
                //.Condition(ConditionalModels.VAR_CONDITION)
                //.ConditionalEventDefinitionDone()
                //.UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                //.EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is removed, end listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID,
                null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.IsNull(rule.TaskService.CreateTaskQuery()
                .First());

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }


        [Test]
        public virtual void testIntermediateConditionalEventWithSetVariableOnStartListener()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .StartEvent()
                .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
                .ConditionalEventDefinition()
                //.Condition(ConditionalModels.VAR_CONDITION)
                //.ConditionalEventDefinitionDone()
                //.UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                //.EndEvent()
                .Done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                .StartEvent()
                .SubProcess()
                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    typeof(SetVariableDelegate).FullName)
                //.EmbeddedSubProcess()
                //.StartEvent()
                .IntermediateCatchEvent(ConditionalModels.CONDITION_ID)
                .ConditionalEventDefinition()
                //.Condition(ConditionalModels.VAR_CONDITION)
                //.ConditionalEventDefinitionDone()
                //.UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                //.EndEvent()
                .SubProcessDone()
                .EndEvent()
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is added, start listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.CONDITION_ID, ConditionalModels.CONDITION_ID,
                null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.IsNull(rule.TaskService.CreateTaskQuery()
                .First());

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testBoundaryConditionalEventWithSetVariableOnStartListener()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .UserTaskBuilder(ConditionalModels.USER_TASK_ID)
                    .BoundaryEvent(ConditionalModels.BOUNDARY_ID)
                    .Condition(ConditionalModels.VAR_CONDITION)
                    .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                    .EndEvent()
                    .Done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess()
                            .StartEvent()
                            .SubProcess(ConditionalModels.SUB_PROCESS_ID)
                            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                                typeof(SetVariableDelegate).FullName)
                            //.EmbeddedSubProcess()
                            //.StartEvent()
                            .UserTask(ConditionalModels.USER_TASK_ID)
                            .EndEvent()
                            .SubProcessDone()
                            .EndEvent()
                            .Done())
                        .UserTaskBuilder(ConditionalModels.USER_TASK_ID)
                        .BoundaryEvent(ConditionalModels.BOUNDARY_ID)
                        .Condition(ConditionalModels.VAR_CONDITION)
                        .EndEvent()
                        //.MoveToActivity(ConditionalModels.SUB_PROCESS_ID)
                        //.BoundaryEvent()
                        //.Condition(ConditionalModels.VAR_CONDITION)
                        .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                        .EndEvent()
                        .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.USER_TASK_ID, ConditionalModels.USER_TASK_ID)
                    .MapActivities(ConditionalModels.BOUNDARY_ID, ConditionalModels.BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is added, start listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.BOUNDARY_ID, ConditionalModels.BOUNDARY_ID,
                null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.AreEqual(ConditionalModels.USER_TASK_ID, rule.TaskService.CreateTaskQuery()
                .First()
                .TaskDefinitionKey);

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testBoundaryConditionalEventWithSetVariableOnEndListener()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(
                            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalModels.PROC_DEF_KEY)
                                .StartEvent()
                                .SubProcess(ConditionalModels.SUB_PROCESS_ID)
                                .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                                    typeof(SetVariableDelegate).FullName)
                                //.EmbeddedSubProcess()
                                //.StartEvent()
                                .UserTask(ConditionalModels.USER_TASK_ID)
                                .EndEvent()
                                .SubProcessDone()
                                .EndEvent()
                                .Done())
                        .UserTaskBuilder(ConditionalModels.USER_TASK_ID)
                        .BoundaryEvent(ConditionalModels.BOUNDARY_ID)
                        .Condition(ConditionalModels.VAR_CONDITION)
                        .EndEvent()
                        //.MoveToActivity(ConditionalModels.SUB_PROCESS_ID)
                        //.BoundaryEvent()
                        //.Condition(ConditionalModels.VAR_CONDITION)
                        .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                        .EndEvent()
                        .Done());
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .UserTaskBuilder(ConditionalModels.USER_TASK_ID)
                    .BoundaryEvent(ConditionalModels.BOUNDARY_ID)
                    .Condition(ConditionalModels.VAR_CONDITION)
                    .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                    .EndEvent()
                    .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.USER_TASK_ID, ConditionalModels.USER_TASK_ID)
                    .MapActivities(ConditionalModels.BOUNDARY_ID, ConditionalModels.BOUNDARY_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is removed, end listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(ConditionalModels.BOUNDARY_ID, ConditionalModels.BOUNDARY_ID,
                null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.AreEqual(ConditionalModels.USER_TASK_ID, rule.TaskService.CreateTaskQuery()
                .First()
                .TaskDefinitionKey);

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testConditionalEventSubProcessWithSetVariableOnStartListener()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .AddSubProcessTo(ConditionalModels.PROC_DEF_KEY)
                    .TriggerByEvent()
                    //.EmbeddedSubProcess()
                    //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                    //.Condition(ConditionalModels.VAR_CONDITION)
                    .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                    .EndEvent()
                    .Done());

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetModel =
                ModifiableBpmnModelInstance.Modify(
                        ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalModels.PROC_DEF_KEY)
                            .StartEvent()
                            .SubProcess(ConditionalModels.SUB_PROCESS_ID)
                            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                                typeof(SetVariableDelegate).FullName)
                            //.EmbeddedSubProcess()
                            //.StartEvent()
                            .UserTask(ConditionalModels.USER_TASK_ID)
                            .EndEvent()
                            .SubProcessDone()
                            .EndEvent()
                            .Done())
                    .AddSubProcessTo(ConditionalModels.SUB_PROCESS_ID)
                    .TriggerByEvent()
                    //.EmbeddedSubProcess()
                    //.StartEvent()
                    //.Condition(ConditionalModels.VAR_CONDITION)
                    .EndEvent()
                    .Done();

            targetModel = ModifiableBpmnModelInstance.Modify(targetModel)
                .AddSubProcessTo(ConditionalModels.PROC_DEF_KEY)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                //.Condition(ConditionalModels.VAR_CONDITION)
                .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                .EndEvent()
                .Done();
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetModel);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.USER_TASK_ID, ConditionalModels.USER_TASK_ID)
                    .MapActivities(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID,
                        MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is added, start listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID,
                MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID, null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.AreEqual(ConditionalModels.USER_TASK_ID, rule.TaskService.CreateTaskQuery()
                .First()
                .TaskDefinitionKey);

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testConditionalEventSubProcessWithSetVariableOnEndListener()
        {
            // given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceModel =
                ModifiableBpmnModelInstance.Modify(
                        ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ConditionalModels.PROC_DEF_KEY)
                            .StartEvent()
                            .SubProcess(ConditionalModels.SUB_PROCESS_ID)
                            .CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                                typeof(SetVariableDelegate).FullName)
                            //.EmbeddedSubProcess()
                            //.StartEvent()
                            .UserTask(ConditionalModels.USER_TASK_ID)
                            .EndEvent()
                            .SubProcessDone()
                            .EndEvent()
                            .Done())
                    .AddSubProcessTo(ConditionalModels.PROC_DEF_KEY)
                    .TriggerByEvent()
                    //.EmbeddedSubProcess()
                    //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                    //.Condition(ConditionalModels.VAR_CONDITION)
                    .EndEvent()
                    .Done();

            sourceModel = ModifiableBpmnModelInstance.Modify(sourceModel)
                .AddSubProcessTo(ConditionalModels.SUB_PROCESS_ID)
                .TriggerByEvent()
                //.EmbeddedSubProcess()
                //.StartEvent()
                //.Condition(ConditionalModels.VAR_CONDITION)
                .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                .EndEvent()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceModel);

            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .AddSubProcessTo(ConditionalModels.PROC_DEF_KEY)
                    .TriggerByEvent()
                    //.EmbeddedSubProcess()
                    //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                    //.Condition(ConditionalModels.VAR_CONDITION)
                    .UserTask(AbstractConditionalEventTestCase.TaskAfterConditionId)
                    .EndEvent()
                    .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities(ConditionalModels.USER_TASK_ID, ConditionalModels.USER_TASK_ID)
                    .MapActivities(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID,
                        MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                    .UpdateEventTrigger()
                    .Build();

            //when sub process is removed, end listener is called and sets variable
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);
            testHelper.AssertEventSubscriptionMigrated(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID,
                MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID, null);
            Assert.AreEqual(1, rule.RuntimeService.GetVariable(processInstance.Id, ConditionalModels.VARIABLE_NAME));

            //then conditional event is not triggered
            Assert.AreEqual(ConditionalModels.USER_TASK_ID, rule.TaskService.CreateTaskQuery()
                .First()
                .TaskDefinitionKey);

            //when any var is set
            testHelper.AnyVariable = processInstance.Id;

            //then condition is satisfied, since variable is already set which satisfies condition
            testHelper.CompleteTask(AbstractConditionalEventTestCase.TaskAfterConditionId);
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}