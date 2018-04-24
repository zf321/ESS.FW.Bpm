using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Bpmn.MultiInstance;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Migration;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationRemoveSubprocessTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationRemoveSubprocessTest()
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
        public virtual void testRemoveScopeForNonScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveScopeForScopeActivity()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveScopeForConcurrentNonScopeActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask2")
                    .Concurrent()
                    .NoScope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveScopeForConcurrentScopeActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelScopeTasksSubProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask1")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask2")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testRemoveConcurrentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask")
                    .MapActivities("userTask2", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask")
                    .Concurrent()
                    .NoScope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveConcurrentScope2()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelTaskAndSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask1")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .BeginScope("subProcess", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess1")
                        .Id)
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testRemoveScopeAndMoveToConcurrentActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelTaskAndSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask1")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .BeginScope("subProcess", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                        .Id)
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);
            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        /// <summary>
        ///     Remove when implementing CAM-5407
        /// </summary>
        [Test]
        public virtual void testCannotRemoveScopeAndMoveToConcurrentActivity()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelTaskAndSubprocessProcess);

            // when
            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess")
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

                Assert.Fail("should not validate");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("userTask2",
                        "The closest mapped ancestor 'subProcess' is mapped to scope 'subProcess' which is not an ancestor of target scope 'userTask2'");
            }
        }

        [Test]
        public virtual void testRemoveMultipleScopes()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.DoubleSubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree("userTask")
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Done());


            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }


        [Test]
        public virtual void testEndListenerInvocationForRemovedScope()
        {
            // given
            DelegateEvent.ClearEvents();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    //    typeof(DelegateExecutionListener).FullName)
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            var recordedEvents = DelegateEvent.Events;
            Assert.AreEqual(1, recordedEvents.Count);

            var @event = recordedEvents[0];
            Assert.AreEqual(sourceProcessDefinition.Id, @event.ProcessDefinitionId);
            Assert.AreEqual("subProcess", @event.CurrentActivityId);
            Assert.AreEqual(testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                .Id, @event.ActivityInstanceId);

            DelegateEvent.ClearEvents();
        }

        [Test]
        public virtual void testSkipListenerInvocationForRemovedScope()
        {
            // given
            DelegateEvent.ClearEvents();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameEnd,
                    //    typeof(DelegateExecutionListener).FullName)
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .SkipCustomListeners()
                .Execute();

            // then
            var recordedEvents = DelegateEvent.Events;
            Assert.AreEqual(0, recordedEvents.Count);

            DelegateEvent.ClearEvents();
        }

        [Test]
        public virtual void testIoMappingInvocationForRemovedScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaOutputParameter("foo", "bar")
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();

            // then
            var inputVariable = rule.RuntimeService.CreateVariableInstanceQuery()
                .First();
            Assert.NotNull(inputVariable);
            Assert.AreEqual("foo", inputVariable.Name);
            Assert.AreEqual("bar", inputVariable.Value);
            Assert.AreEqual(processInstance.Id, inputVariable.ActivityInstanceId);
        }

        [Test]
        public virtual void testSkipIoMappingInvocationForRemovedScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaOutputParameter("foo", "bar")
                    //.Done()
                );
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            var processInstance = rule.RuntimeService.StartProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .SkipIoMappings()
                .Execute();

            // then
            Assert.AreEqual(0, rule.RuntimeService.CreateVariableInstanceQuery()
                .Count());
        }


        [Test]
        public virtual void testCannotRemoveParentScopeAndMoveOutOfGrandParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TripleSubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TripleSubprocessProcess);

            // when
            try
            {
                // subProcess2 is not migrated
                // subProcess 3 is moved out of the subProcess1 scope (by becoming a subProcess1 itself)
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess1", "subProcess1")
                    .MapActivities("subProcess3", "subProcess1")
                    .MapActivities("userTask", "userTask")
                    .Build();

                Assert.Fail("should not validate");
            }
            catch (MigrationPlanValidationException e)
            {
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("subProcess3",
                        "The closest mapped ancestor 'subProcess1' is mapped to scope 'subProcess1' which is not an ancestor of target scope 'subProcess1'");
            }
        }
    }
}