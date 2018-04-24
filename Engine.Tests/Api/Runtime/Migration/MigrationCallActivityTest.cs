using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MigrationCallActivityTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        [SetUp]
        public virtual void deployOneTaskProcess()
        {
            testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                .ChangeElementId(ProcessModels.ProcessKey, "oneTaskProcess"));
        }

        [SetUp]
        public virtual void deployOneTaskCase()
        {
            testHelper.Deploy("resources/api/cmmn/oneTaskCase.cmmn");
        }

        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationCallActivityTest()
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
        public virtual void testCallBpmnProcessSimpleMigration()
        {
            // given
            var model = CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // and it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCallCmmnCaseSimpleMigration()
        {
            // given
            var model = CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // and it is possible to complete the called case instance
            var caseExecution = rule.CaseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First();

            testHelper.CompleteTask("PI_HumanTask_1");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);

            // and close the called case instance
            rule.CaseService.WithCaseExecution(caseExecution.CaseInstanceId)
                .Close();
            testHelper.AssertCaseEnded(caseExecution.CaseInstanceId);
        }

        [Test]
        public virtual void testCallBpmnProcessAddParentScope()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.subProcessBpmnCallActivityProcess("oneTaskProcess"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Child("callActivity")
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginScope("subProcess")
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // and it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCallBpmnProcessParallelMultiInstance()
        {
            // given
            IBpmnModelInstance model =
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"))
                //.ActivityBuilder("callActivity")
                //.MultiInstance()
                //.Parallel()
                //.Cardinality("1")
                //.Done()
                ;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity#multiInstanceBody", "callActivity#multiInstanceBody")
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity#multiInstanceBody"))
                    .Child("callActivity")
                    .Concurrent()
                    .NoScope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("callActivity")
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // and the link between calling and called instance is maintained correctly

            testHelper.AssertSuperExecutionOfProcessInstance(rule.RuntimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("oneTaskProcess")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // and it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCallCmmnCaseParallelMultiInstance()
        {
            // given
            IBpmnModelInstance model =
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"))
                /*.ActivityBuilder("callActivity").MultiInstance().Parallel().Cardinality("1").Done()*/;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(model);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity#multiInstanceBody", "callActivity#multiInstanceBody")
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child(null)
                    .Scope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity#multiInstanceBody"))
                    .Child("callActivity")
                    .Concurrent()
                    .NoScope()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("callActivity"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .BeginMiBody("callActivity")
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // and the link between calling and called instance is maintained correctly
            testHelper.AssertSuperExecutionOfCaseInstance(rule.CaseService.CreateCaseInstanceQuery()
                //.CaseDefinitionKey("oneTaskCase")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // and it is possible to complete the called case instance
            var caseExecution = rule.CaseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First();

            testHelper.CompleteTask("PI_HumanTask_1");

            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);

            // and close the called case instance
            rule.CaseService.WithCaseExecution(caseExecution.CaseInstanceId)
                .Close();
            testHelper.AssertCaseEnded(caseExecution.CaseInstanceId);
        }

        [Test]
        public virtual void testCallBpmnProcessParallelMultiInstanceRemoveMiBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"))
                    //.ActivityBuilder("callActivity")
                    //.MultiInstance()
                    //.Parallel()
                    //.Cardinality("1")
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // then the link between calling and called instance is maintained correctly

            testHelper.AssertSuperExecutionOfProcessInstance(rule.RuntimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("oneTaskProcess")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // then it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCallCmmnCaseParallelMultiInstanceRemoveMiBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"))
                    //.ActivityBuilder("callActivity")
                    //.MultiInstance()
                    //.Parallel()
                    //.Cardinality("1")
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());


            // and the link between calling and called instance is maintained correctly
            testHelper.AssertSuperExecutionOfCaseInstance(rule.CaseService.CreateCaseInstanceQuery()
                //.CaseDefinitionKey("oneTaskCase")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // and it is possible to complete the called case instance
            var caseExecution = rule.CaseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First();

            testHelper.CompleteTask("PI_HumanTask_1");

            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);

            // and close the called case instance
            rule.CaseService.WithCaseExecution(caseExecution.CaseInstanceId)
                .Close();
            testHelper.AssertCaseEnded(caseExecution.CaseInstanceId);
        }

        [Test]
        public virtual void testCallBpmnProcessSequentialMultiInstanceRemoveMiBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"))
                    //.ActivityBuilder("callActivity")
                    //.MultiInstance()
                    //.Sequential()
                    //.Cardinality("1")
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // then the link between calling and called instance is maintained correctly

            testHelper.AssertSuperExecutionOfProcessInstance(rule.RuntimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("oneTaskProcess")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // then it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCallCmmnCaseSequentialMultiInstanceRemoveMiBody()
        {
            // given
            var sourceProcessDefinition =
                testHelper.DeployAndGetDefinition(
                    ModifiableBpmnModelInstance.Modify(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"))
                    //.ActivityBuilder("callActivity")
                    //.MultiInstance()
                    //.Sequential()
                    //.Cardinality("1")
                    //.Done()
                );
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(CallActivityModels.oneCmmnCallActivityProcess("oneTaskCase"));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("callActivity")
                    .Scope()
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("callActivity", testHelper.GetSingleActivityInstanceBeforeMigration("callActivity")
                        .Id)
                    .Done());

            // then the link between calling and called instance is maintained correctly
            testHelper.AssertSuperExecutionOfCaseInstance(rule.CaseService.CreateCaseInstanceQuery()
                //.CaseDefinitionKey("oneTaskCase")
                .First()
                .Id, testHelper.GetSingleExecutionIdForActivityAfterMigration("callActivity"));

            // and it is possible to complete the called case instance
            var caseExecution = rule.CaseService.CreateCaseExecutionQuery(c=> c.ActivityId =="PI_HumanTask_1")
                .First();

            testHelper.CompleteTask("PI_HumanTask_1");

            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);

            // and close the called case instance
            rule.CaseService.WithCaseExecution(caseExecution.CaseInstanceId)
                .Close();
            testHelper.AssertCaseEnded(caseExecution.CaseInstanceId);
        }

        [Test]
        public virtual void testCallBpmnProcessReconfigureCallActivity()
        {
            // given
            var model = CallActivityModels.oneBpmnCallActivityProcess("oneTaskProcess");

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(model);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(model)
                .CallActivityBuilder("callActivity")
                .CalledElement("foo")
                .Done());

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("callActivity", "callActivity")
                    .Build();

            // when
            var processInstance = testHelper.CreateProcessInstanceAndMigrate(migrationPlan);

            // then the called instance has not changed (e.g. not been migrated to a different process definition)
            var calledInstance = rule.RuntimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("oneTaskProcess")
                .First();
            Assert.NotNull(calledInstance);

            // and it is possible to complete the called process instance
            testHelper.CompleteTask("userTask");
            // and the calling process instance
            testHelper.CompleteTask("userTask");

            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}