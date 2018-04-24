using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Bpmn.MultiInstance;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationAddSubprocessTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationAddSubprocessTest()
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
        public virtual void testScopeUserTaskMigration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskSubprocessProcess);

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
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
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
        public virtual void testConcurrentScopeUserTaskMigration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelScopeTasksSubProcess);

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
                    .Scope()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask1")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask1"))
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask2")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("userTask2"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
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
        public virtual void testUserTaskMigration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

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
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
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
        public virtual void testConcurrentUserTaskMigration()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

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
                    .Scope()
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
                    .BeginScope("subProcess")
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
        public virtual void testConcurrentThreeUserTaskMigration()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewayProcess)
                    //.GetBuilderForElementById("fork", typeof(ParallelGatewayBuilder))
                    //.UserTask("userTask3")
                    //.EndEvent()
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(ProcessModels.ParallelGatewaySubprocessProcess)
                    //.GetBuilderForElementById("fork", typeof(ParallelGatewayBuilder))
                    //.UserTask("userTask3")
                    //.EndEvent()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask2")
                    .MapActivities("userTask2", "userTask3")
                    .MapActivities("userTask3", "userTask1")
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
                    .Scope()
                    .Child("userTask1")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask2")
                    .Concurrent()
                    .NoScope()
                    .Up()
                    .Child("userTask3")
                    .Concurrent()
                    .NoScope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask3")
                        .Id)
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Activity("userTask3", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(3, migratedTasks.Count);

            foreach (var migratedTask in migratedTasks)
                Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testNestedScopesMigration1()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.DoubleSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("subProcess", "outerSubProcess")
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
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("subProcess"))
                    .Child("userTask")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("outerSubProcess", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                        .Id)
                    .BeginScope("innerSubProcess")
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
        public virtual void testNestedScopesMigration2()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.DoubleSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("subProcess", "innerSubProcess")
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
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("subProcess"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("outerSubProcess")
                    .BeginScope("innerSubProcess", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                        .Id)
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
        public virtual void testMultipleInstancesOfScope()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.DoubleSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .MapActivities("subProcess", "outerSubProcess")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("subProcess")
                .StartBeforeActivity("subProcess")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Up()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Done());

            var activityInstance = testHelper.SnapshotBeforeMigration.ActivityTree;
            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("outerSubProcess", activityInstance.GetActivityInstances("subProcess")[0].Id)
                    .BeginScope("innerSubProcess")
                    .Activity("userTask",
                        activityInstance.GetActivityInstances("subProcess")[0].GetActivityInstances("userTask")[0].Id)
                    .EndScope()
                    .EndScope()
                    .BeginScope("outerSubProcess", activityInstance.GetActivityInstances("subProcess")[1].Id)
                    .BeginScope("innerSubProcess")
                    .Activity("userTask",
                        activityInstance.GetActivityInstances("subProcess")[1].GetActivityInstances("userTask")[0].Id)
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
        public virtual void testChangeActivityId()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask2")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask1")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("userTask2")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask2");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testChangeScopeActivityId()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelScopeTasksSubProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask2")
                    .Build();

            var processInstance = rule.RuntimeService.CreateProcessInstanceById(sourceProcessDefinition.Id)
                .StartBeforeActivity("userTask1")
                .Execute();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Child("userTask2")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask2");
            Assert.NotNull(migratedTask);
            Assert.AreEqual(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testListenerInvocationForNewlyCreatedScope()
        {
            // given
            DelegateEvent.ClearEvents();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    //    typeof(DelegateExecutionListener).FullName)
                    //.Done()
                );

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
            Assert.AreEqual(targetProcessDefinition.Id, @event.ProcessDefinitionId);
            Assert.AreEqual("subProcess", @event.CurrentActivityId);

            DelegateEvent.ClearEvents();
        }

        [Test]
        public virtual void testSkipListenerInvocationForNewlyCreatedScope()
        {
            // given
            DelegateEvent.ClearEvents();

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart,
                    //    typeof(DelegateExecutionListener).FullName)
                    //.Done()
                );

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
        public virtual void testIoMappingInvocationForNewlyCreatedScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaInputParameter("foo", "bar")
                    //.Done()
                );

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

            var activityInstance = rule.RuntimeService.GetActivityInstance(processInstance.Id);
            Assert.AreEqual(activityInstance.GetActivityInstances("subProcess")[0].Id, inputVariable.ActivityInstanceId);
        }

        [Test]
        public virtual void testSkipIoMappingInvocationForNewlyCreatedScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaInputParameter("foo", "bar")
                    //.Done()
                );

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
        public virtual void testDeleteMigratedInstance()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ScopeTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelScopeTasksSubProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then it is possible to Delete the process instance
            var ProcessInstanceId = testHelper.SnapshotBeforeMigration.ProcessInstanceId;
            rule.RuntimeService.DeleteProcessInstance(ProcessInstanceId, null);
            testHelper.AssertProcessEnded(ProcessInstanceId);
        }

        /// <summary>
        ///     Readd when we implement migration for multi-instance
        /// </summary>
        [Test]
        public virtual void testAddParentScopeToMultiInstance()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition((ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .GetModelElementById/*<IUserTask>*/("userTask") as IUserTask)
                    .Builder()
                    .MultiInstance()
                    .Parallel()
                    .CamundaCollection("collectionVar")
                    .CamundaElementVariable("elementVar")
                    .Done());
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition((ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    .GetModelElementById/*<IUserTask>*/("userTask") as IUserTask)
                    .Builder()
                    .MultiInstance()
                    .Parallel()
                    .CamundaCollection("collectionVar")
                    .CamundaElementVariable("elementVar")
                    .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask#multiInstanceBody", "userTask#multiInstanceBody")
                    .MapActivities("userTask", "userTask")
                    .Build();

            IList<string> miElements = new List<string>();
            miElements.Add("a");
            miElements.Add("b");
            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id
                //,
                //Variable.Variables.CreateVariables()
                //    .PutValue("collectionVar", miElements)
            );

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .BeginMiBody("userTask")
                    .Activity("userTask")
                    .Activity("userTask")
                    .Activity("userTask")
                    .Done());

            // the element variables still exist
            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);

            IList<string> collectedElementsVars = new List<string>();
            foreach (var migratedTask in migratedTasks)
                collectedElementsVars.Add((string) rule.TaskService.GetVariable(migratedTask.Id, "elementVar"));

            Assert.True(collectedElementsVars.Contains("a"));
            Assert.True(collectedElementsVars.Contains("b"));

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);

            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testAddTwoScopes()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.DoubleSubprocessProcess);

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
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("outerSubProcess")
                    .BeginScope("innerSubProcess")
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
        public virtual void testAddTwoConcurrentScopes()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewayProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.DoubleParallelSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask1", "userTask1")
                    .MapActivities("userTask2", "userTask2")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then there is only one instance of outerSubProcess
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
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
                    .BeginScope("outerSubProcess")
                    .BeginScope("innerSubProcess1")
                    .Activity("userTask1", testHelper.GetSingleActivityInstanceBeforeMigration("userTask1")
                        .Id)
                    .EndScope()
                    .BeginScope("innerSubProcess2")
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .Done());

            var migratedTasks = testHelper.SnapshotAfterMigration.Tasks;
            Assert.AreEqual(2, migratedTasks.Count);

            // and it is possible to successfully complete the migrated instance
            foreach (var migratedTask in migratedTasks)
                rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testCanMigrateParentScopeWayTooHigh()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.TripleSubprocessProcess);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("subProcess", "subProcess1")
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then there is only one instance of outerSubProcess
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("subProcess"))
                    .Child(null)
                    .Scope()
                    .Child("userTask")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess1", testHelper.GetSingleActivityInstanceBeforeMigration("subProcess")
                        .Id)
                    .BeginScope("subProcess2")
                    .BeginScope("subProcess3")
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            var migratedTask = testHelper.SnapshotAfterMigration.GetTaskForKey("userTask");
            Assert.NotNull(migratedTask);

            // and it is possible to successfully complete the migrated instance
            rule.TaskService.Complete(migratedTask.Id);
            testHelper.AssertProcessEnded(testHelper.SnapshotBeforeMigration.ProcessInstanceId);
        }

        [Test]
        public virtual void testMoveConcurrentActivityIntoSiblingScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelTaskAndSubprocessProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ProcessModels.ParallelGatewaySubprocessProcess);

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
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask2")
                    .Scope()
                    .Up()
                    .Up()
                    .Child(null)
                    .Concurrent()
                    .NoScope()
                    .Child("userTask1")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask2", testHelper.GetSingleActivityInstanceBeforeMigration("userTask2")
                        .Id)
                    .EndScope()
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

        [Test]
        public virtual void testAddScopeDoesNotBecomeAsync()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.SubprocessProcess)
                    //.ActivityBuilder("subProcess")
                    //.CamundaAsyncBefore()
                    //.Done()
                );

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("userTask", "userTask")
                    .Build();

            // when
            testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the async flag for the subprocess was not relevant for instantiation
            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("userTask", testHelper.GetSingleActivityInstanceBeforeMigration("userTask")
                        .Id)
                    .Done());

            Assert.AreEqual(0, testHelper.SnapshotAfterMigration.Jobs.Count);
        }
    }
}