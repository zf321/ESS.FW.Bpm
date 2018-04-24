using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationCompensationAddSubProcessTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationCompensationAddSubProcessTest()
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
        public virtual void testCase1()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        /// <summary>
        ///     The guarantee given by the API is: Compensation can be triggered in the scope that it could be triggered before
        ///     migration. Thus, it should not be possible to trigger compensation from the new sub process instance but only from
        ///     the
        ///     parent scope, i.E. the process definition instance
        /// </summary>
        [Test]
        public virtual void testCase1CannotTriggerCompensationInNewScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL)
                        .EndEventBuilder("subProcessEnd")
                        //.CompensateEventDefinition()
                        //.WaitForCompletion(true)
                        //.CompensateEventDefinitionDone()
                        .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then compensation is only caught outside of the subProcess
            testHelper.CompleteTask("userTask2");

            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .BeginScope("subProcess")
                    .Activity("compensationHandler")
                    .Done()); // note: this is not subProcess and subProcessEnd
        }

        [Test]
        public virtual void testCase1AssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_TWO_TASKS_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then the execution tree is correct
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask2")
                    .Scope()
                    .Up()
                    .Child("subProcess")
                    .Scope()
                    .EventScope()
                    .Done());
        }

        [Test]
        public virtual void testCase2()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertEventSubscriptionMigrated("compensationHandler", "compensationHandler", null);

            // and the compensation can be triggered and completed
            testHelper.CompleteTask("userTask2");
            testHelper.CompleteTask("compensationHandler");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCase2AssertExecutionTree()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask2")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("subProcess")
                    .Scope()
                    .EventScope()
                    .Done());
        }

        [Test]
        public virtual void testCase2AssertActivityInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when throwing compensation
            testHelper.CompleteTask("userTask2");

            // then the activity instance tree is correct
            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);

            ActivityInstanceAssert.That(activityInstance)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("compensationEvent")
                    .BeginScope("subProcess")
                    .Activity("compensationHandler")
                    .Done());
        }

        [Test]
        public virtual void testNoListenersCalled()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerExpression(ExecutionListenerFields.EventNameStart,
                    //    "${execution.SetVariable('foo', 'bar')}")
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
        }

        [Test]
        public virtual void testNoInputMappingExecuted()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.ONE_COMPENSATION_TASK_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL)
                    //.ActivityBuilder("subProcess")
                    //.CamundaInputParameter("foo", "bar")
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            testHelper.CompleteTask("userTask1");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.GetVariables()
                .Count);
        }

        [Test]
        public virtual void testVariablesInParentEventScopeStillAccessible()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(CompensationModels.DOUBLE_SUBPROCESS_MODEL);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "outerSubProcess")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var subProcessExecution = rule.RuntimeService.CreateExecutionQuery(c=>c.ActivityId == "userTask1")
                .First();
            rule.RuntimeService.SetVariableLocal(subProcessExecution.Id, "foo", "bar");

            testHelper.CompleteTask("userTask1");

            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // when throwing compensation
            testHelper.CompleteAnyTask("userTask2");

            // then the variable snapshot is available
            var compensationTask = rule.TaskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("bar", rule.TaskService.GetVariable(compensationTask.Id, "foo"));
        }

        [Test]
        public virtual void testCannotAddScopeOnTopOfEventSubProcess()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CompensationModels.COMPENSATION_EVENT_SUBPROCESS_MODEL);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CompensationModels.DOUBLE_SUBPROCESS_MODEL)
                        .AddSubProcessTo("innerSubProcess")
                        //.Id("eventSubProcess")
                        .TriggerByEvent()
                        //.EmbeddedSubProcess()
                        //.StartEvent("eventSubProcessStart")
                        //.CompensateEventDefinition()
                        //.CompensateEventDefinitionDone()
                        .EndEvent()
                        .Done());


            try
            {
                // when
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "outerSubProcess")
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .MapActivities("compensationBoundary", "compensationBoundary")
                    .MapActivities("userTask2", "userTask2")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("eventSubProcessStart",
                        "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (innerSubProcess)");
            }
        }
    }
}