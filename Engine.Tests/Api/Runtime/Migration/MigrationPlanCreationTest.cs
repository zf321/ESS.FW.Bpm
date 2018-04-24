using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Migration;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationPlanCreationTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
        }

        public const string MESSAGE_NAME = "Message";
        public const string SIGNAL_NAME = "Signal";
        public const string ERROR_CODE = "Error";
        public const string ESCALATION_CODE = "Escalation";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal MigrationTestRule testHelper;

        public MigrationPlanCreationTest()
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

        [Test]
        public virtual void testExplicitInstructionGeneration()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMigrateNonExistingSourceDefinition()
        {
            var processDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan("aNonExistingProcDefId", processDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (BadUserRequestException e)
            {
                AssertExceptionMessage(e, "Source process definition with id 'aNonExistingProcDefId' does not exist");
            }
        }

        [Test]
        public virtual void testMigrateNullSourceDefinition()
        {
            var processDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(null, processDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (BadUserRequestException e)
            {
                AssertExceptionMessage(e, "Source process definition id is null");
            }
        }

        [Test]
        public virtual void testMigrateNonExistingTargetDefinition()
        {
            var processDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            try
            {
                runtimeService.CreateMigrationPlan(processDefinition.Id, "aNonExistingProcDefId")
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (BadUserRequestException e)
            {
                AssertExceptionMessage(e, "Target process definition with id 'aNonExistingProcDefId' does not exist");
            }
        }

        [Test]
        public virtual void testMigrateNullTargetDefinition()
        {
            var processDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(processDefinition.Id, null)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (BadUserRequestException e)
            {
                AssertExceptionMessage(e, "Target process definition id is null");
            }
        }

        [Test]
        public virtual void testMigrateNonExistingSourceActivityId()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("thisActivityDoesNotExist", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                //MigrationPlanValidationReportAssert.That(e.ValidationReport)
                //    .HasInstructionFailures("thisActivityDoesNotExist",
                //        "Source activity 'thisActivityDoesNotExist' does not exist");
            }
        }

        [Test]
        public virtual void testMigrateNullSourceActivityId()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities(null, "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures(null, "Source activity id is null");
            }
        }

        [Test]
        public virtual void testMigrateNonExistingTargetActivityId()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("userTask", "thisActivityDoesNotExist")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask", "Target activity 'thisActivityDoesNotExist' does not exist");
            }
        }

        [Test]
        public virtual void testMigrateNullTargetActivityId()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("userTask", null)
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask", "Target activity id is null");
            }
        }

        [Test]
        public virtual void testMigrateTaskToHigherScope()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var migrationPlan = runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapActivities("userTask", "userTask")
                .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceDefinition)
                .HasTargetProcessDefinition(targetDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMigrateToUnsupportedActivityType()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneReceiveTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("userTask", "receiveTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Activities have incompatible types (UserTaskActivityBehavior is not compatible with ReceiveTaskActivityBehavior)");
            }
        }

        [Test]
        public virtual void testNotMigrateActivitiesOfDifferentType()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    .SwapElementIds("userTask", "subProcess"));

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Activities have incompatible types (UserTaskActivityBehavior is not " +
                        "compatible with SubProcessActivityBehavior)");
            }
        }

        [Test]
        public virtual void testNotMigrateBoundaryEventsOfDifferentType()
        {
            var sourceDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    ////.Message(MESSAGE_NAME)
                    //.Done()
                ));
            var targetDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    //.ActivityBuilder("userTask")
                    //.BoundaryEvent("boundary")
                    //.Signal(SIGNAL_NAME)
                    //.Done()
                );

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("boundary",
                        "Events are not of the same type (boundaryMessage != boundarySignal)");
            }
        }

        [Test]
        public virtual void testMigrateSubProcessToProcessDefinition()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                    .MapActivities("subProcess", targetDefinition.Id)
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("subProcess", "Target activity '" + targetDefinition.Id + "' does not exist");
            }
        }

        [Test]
        public virtual void testMapEqualActivitiesWithParallelMultiInstance()
        {
            // given
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.GetBuilderForElementById("userTask", typeof(UserTaskBuilder))
                //.MultiInstance()
                //.Parallel()
                //.Cardinality("3")
                //.MultiInstanceDone()
                //.Done()
                ;
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            // when
            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Target activity 'userTask' is a descendant of multi-instance body 'userTask#multiInstanceBody' " +
                        "that is not mapped from the source process definition.");
            }
        }

        [Test]
        public virtual void testMapEqualBoundaryEvents()
        {
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("boundary")
                    .To("boundary"));
        }

        [Test]
        public virtual void testMapBoundaryEventsWithDifferentId()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("boundary", "newBoundary");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "newBoundary")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("boundary")
                    .To("newBoundary"));
        }

        [Test]
        public virtual void testMapBoundaryToMigratedActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(sourceProcess)
                .ChangeElementId("userTask", "newUserTask");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "newUserTask")
                    .MapActivities("boundary", "boundary")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("newUserTask"), MigrationPlanAssert.Migrate("boundary")
                    .To("boundary"));
        }

        [Test]
        public virtual void testMapBoundaryToParallelActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask1")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                //.ActivityBuilder("userTask2")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .MapActivities("boundary", "boundary")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("boundary",
                        "The source activity's event scope (userTask1) must be mapped to the target activity's event scope (userTask2)");
            }
        }

        [Test]
        public virtual void testMapBoundaryToHigherScope()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("boundary")
                    .To("boundary"));
        }
        [Test]
        public virtual void testMapBoundaryToLowerScope()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("boundary")
                    .To("boundary"));
        }

        [Test]
        public virtual void testMapBoundaryToChildActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("boundary",
                        "The source activity's event scope (subProcess) must be mapped to the target activity's event scope (userTask)");
            }
        }

        [Test]
        public virtual void testMapBoundaryToParentActivity()
        {
            IBpmnModelInstance sourceProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("userTask")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;
            IBpmnModelInstance targetProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("boundary")
                ////.Message(MESSAGE_NAME)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(sourceProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(targetProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .MapActivities("boundary", "boundary")
                    .Build();

                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("boundary",
                        "The source activity's event scope (userTask) must be mapped to the target activity's event scope (subProcess)",
                        "The closest mapped ancestor 'subProcess' is mapped to scope 'subProcess' which is not an ancestor of target scope 'boundary'");
            }
        }

        [Test]
        public virtual void testMapAllBoundaryEvents()
        {
            IBpmnModelInstance testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                //.ActivityBuilder("subProcess")
                //.BoundaryEvent("error")
                //.Error(ERROR_CODE)
                //.MoveToActivity("subProcess")
                //.BoundaryEvent("escalation")
                //.Escalation(ESCALATION_CODE)
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("error", "error")
                    .MapActivities("escalation", "escalation")
                    .MapActivities("userTask", "userTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("error")
                    .To("error"), MigrationPlanAssert.Migrate("escalation")
                    .To("escalation"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapProcessDefinitionWithEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .AddSubProcessTo(ProcessModels.ProcessKey)
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent()
                ////.Message(MESSAGE_NAME)
                .EndEvent()
                .SubProcessDone()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapSubProcessWithEventSubProcess()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.NESTED_EVENT_SUB_PROCESS_PROCESS);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask", "userTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("subProcess")
                    .To("subProcess"), MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapActivityWithUnmappedParentWhichHasAEventSubProcessChild()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo("subProcess")
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent()
                ////.Message(MESSAGE_NAME)
                .EndEvent()
                .SubProcessDone()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"));
        }

        [Test]
        public virtual void testMapUserTaskInEventSubProcess()
        {
            var testProcess = ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                .AddSubProcessTo("subProcess")
                .TriggerByEvent()
                ////.EmbeddedSubProcess()
                //.StartEvent()
                ////.Message(MESSAGE_NAME)
                .UserTask("innerTask")
                .EndEvent()
                .SubProcessDone()
                .Done();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(testProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("innerTask", "innerTask")
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("userTask")
                    .To("userTask"), MigrationPlanAssert.Migrate("innerTask")
                    .To("innerTask"));
        }

        [Test]
        public virtual void testNotMapActivitiesMoreThanOnce()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask1", "userTask2")
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask1",
                        "There are multiple mappings for source activity id 'userTask1'",
                        "There are multiple mappings for source activity id 'userTask1'");
            }
        }

        [Test]
        public virtual void testCannotUpdateEventTriggerForNonEvent()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .UpdateEventTrigger()
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask",
                        "Cannot update event trigger because the activity does not define a persistent event trigger");
            }
        }

        [Test]
        public virtual void testCannotUpdateEventTriggerForEventSubProcess()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

            try
            {
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcess", "eventSubProcess")
                    .UpdateEventTrigger()
                    .Build();
                Assert.Fail("Should not succeed");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("eventSubProcess",
                        "Cannot update event trigger because the activity does not define a persistent event trigger");
            }
        }

        [Test]
        public virtual void testCanUpdateEventTriggerForEventSubProcessStartEvent()
        {
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(EventSubProcessModels.TIMER_EVENT_SUBPROCESS_PROCESS);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("eventSubProcessStart", "eventSubProcessStart")
                    .UpdateEventTrigger()
                    .Build();

            MigrationPlanAssert.That(migrationPlan)
                .HasSourceProcessDefinition(sourceProcessDefinition)
                .HasTargetProcessDefinition(targetProcessDefinition)
                .HasInstructions(MigrationPlanAssert.Migrate("eventSubProcessStart")
                    .To("eventSubProcessStart")
                    .UpdateEventTrigger(true));
        }

        protected internal virtual void AssertExceptionMessage(System.Exception e, string message)
        {
            Assert.That(e.Message, Does.Contain(message));
        }

        private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass2 :
            MigrationNestedEventSubProcessTest.MigrationEventSubProcessTestConfiguration
        {
            private readonly MigrationNestedEventSubProcessTest outerInstance;

            public MigrationEventSubProcessTestConfigurationAnonymousInnerClass2(
                MigrationNestedEventSubProcessTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public override IBpmnModelInstance SourceProcess
            {
                get
                {
                    return ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                        .AddSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID)
                        .TriggerByEvent()
                        ////.EmbeddedSubProcess()
                        //.StartEvent(MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                        //.Signal(EventSubProcessModels.SIGNAL_NAME)
                        .UserTask(MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_TASK_ID)
                        .EndEvent()
                        .SubProcessDone()
                        .Done();
                }
            }

            public override string EventName
            {
                get { return EventSubProcessModels.SIGNAL_NAME; }
            }

            public override void triggerEventSubProcess(MigrationTestRule testHelper)
            {
                testHelper.SendSignal(EventSubProcessModels.SIGNAL_NAME);
            }

            public override string ToString()
            {
                return "MigrateSignalEventSubProcess";
            }
        }

        private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass3 :
            MigrationNestedEventSubProcessTest.MigrationEventSubProcessTestConfiguration
        {
            private readonly MigrationNestedEventSubProcessTest outerInstance;

            public MigrationEventSubProcessTestConfigurationAnonymousInnerClass3(
                MigrationNestedEventSubProcessTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public override IBpmnModelInstance SourceProcess
            {
                get
                {
                    return ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                        .AddSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID)
                        .TriggerByEvent()
                        ////.EmbeddedSubProcess()
                        //.StartEvent(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                        //.TimerWithDate(MigrationAddBoundaryEventsTest.TIMER_DATE)
                        .UserTask(MigrationEventSubProcessTest.EVENT_SUB_PROCESS_TASK_ID)
                        .EndEvent()
                        .SubProcessDone()
                        .Done();
                }
            }

            public override string EventName
            {
                get { return null; }
            }

            public override void AssertMigration(MigrationTestRule testHelper)
            {
                testHelper.AssertEventSubProcessTimerJobRemoved(
                    MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_START_ID);
                testHelper.AssertEventSubProcessTimerJobCreated(
                    MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_START_ID);
            }

            public override void triggerEventSubProcess(MigrationTestRule testHelper)
            {
                testHelper.TriggerTimer();
            }

            public override string ToString()
            {
                return "MigrateTimerEventSubProcess";
            }
        }

        private class MigrationEventSubProcessTestConfigurationAnonymousInnerClass4 :
            MigrationNestedEventSubProcessTest.MigrationEventSubProcessTestConfiguration
        {
            private readonly MigrationNestedEventSubProcessTest outerInstance;

            public MigrationEventSubProcessTestConfigurationAnonymousInnerClass4(
                MigrationNestedEventSubProcessTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public override IBpmnModelInstance SourceProcess
            {
                get
                {
                    return ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                        .AddSubProcessTo(EventSubProcessModels.SUB_PROCESS_ID)
                        .TriggerByEvent()
                        ////.EmbeddedSubProcess()
                        //.StartEvent(MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_START_ID)
                        //.Condition(EventSubProcessModels.VAR_CONDITION)
                        .UserTask(MigrationNestedEventSubProcessTest.EVENT_SUB_PROCESS_TASK_ID)
                        .EndEvent()
                        .SubProcessDone()
                        .Done();
                }
            }

            public override string EventName
            {
                get { return null; }
            }

            public override void triggerEventSubProcess(MigrationTestRule testHelper)
            {
                testHelper.AnyVariable = testHelper.SnapshotAfterMigration.ProcessInstanceId;
            }

            public override string ToString()
            {
                return "MigrateConditionalEventSubProcess";
            }
        }
    }
}