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
    public class MigrationGatewayTest
    {
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationGatewayTest()
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
        public virtual void testParallelGatewayContinueExecution()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            testHelper.CompleteTask("parallel1");
            testHelper.CompleteTask("afterJoin");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testParallelGatewayAssertTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("parallel1")
                    .NoScope()
                    .Concurrent()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("parallel1"))
                    .Up()
                    .Child("join")
                    .NoScope()
                    .Concurrent()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("join"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("parallel1", testHelper.GetSingleActivityInstanceBeforeMigration("parallel1")
                        .Id)
                    .Activity("join", testHelper.GetSingleActivityInstanceBeforeMigration("join")
                        .Id)
                    .Done());
        }

        [Test]
        public virtual void testParallelGatewayAddScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW_IN_SUBPROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            testHelper.CompleteTask("parallel1");
            testHelper.CompleteTask("afterJoin");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testInclusiveGatewayContinueExecution()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            testHelper.CompleteTask("parallel1");
            testHelper.CompleteTask("afterJoin");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testInclusiveGatewayAssertTrees()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            testHelper.AssertExecutionTreeAfterMigration()
                .HasProcessDefinitionId(targetProcessDefinition.Id)
                .Matches(ExecutionAssert.DescribeExecutionTree(null)
                    .Scope()
                    .Id(testHelper.SnapshotBeforeMigration.ProcessInstanceId)
                    .Child("parallel1")
                    .NoScope()
                    .Concurrent()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("parallel1"))
                    .Up()
                    .Child("join")
                    .NoScope()
                    .Concurrent()
                    .Id(testHelper.GetSingleExecutionIdForActivityBeforeMigration("join"))
                    .Done());

            testHelper.AssertActivityTreeAfterMigration()
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(targetProcessDefinition.Id)
                    .Activity("parallel1", testHelper.GetSingleActivityInstanceBeforeMigration("parallel1")
                        .Id)
                    .Activity("join", testHelper.GetSingleActivityInstanceBeforeMigration("join")
                        .Id)
                    .Done());
        }

        [Test]
        public virtual void testInclusiveGatewayAddScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW_IN_SUBPROCESS);

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            testHelper.CompleteTask("parallel1");
            testHelper.CompleteTask("afterJoin");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        [Test]
        public virtual void testCannotMigrateParallelToInclusiveGateway()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("join", "join")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("join",
                        "Activities have incompatible types " +
                        "(ParallelGatewayActivityBehavior is not compatible with InclusiveGatewayActivityBehavior)");
            }
        }

        [Test]
        public virtual void testCannotMigrateInclusiveToParallelGateway()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("join", "join")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("join",
                        "Activities have incompatible types " +
                        "(InclusiveGatewayActivityBehavior is not compatible with ParallelGatewayActivityBehavior)");
            }
        }

        /// <summary>
        ///     Ensures that situations are avoided in which more tokens end up at the target gateway
        ///     than it has incoming flows
        /// </summary>
        [Test]
        public virtual void testCannotRemoveGatewayIncomingSequenceFlow()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(GatewayModels.PARALLEL_GW)
                    .RemoveFlowNode("parallel2"));

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("join", "join")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("join",
                        "The target gateway must have at least the same number of incoming sequence flows that the source gateway has");
            }
        }

        /// <summary>
        ///     Ensures that situations are avoided in which more tokens end up at the target gateway
        ///     than it has incoming flows
        /// </summary>
        [Test]
        public virtual void testAddGatewayIncomingSequenceFlow()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(GatewayModels.PARALLEL_GW)
                    //.FlowNodeBuilder("fork")
                    //.UserTask("parallel3")
                    //.ConnectTo("join")
                    //.Done()
                );

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            testHelper.CompleteTask("parallel2");

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("parallel1", "parallel1")
                    .MapActivities("join", "join")
                    .Build();

            // when
            testHelper.MigrateProcessInstance(migrationPlan, processInstance);

            // then
            Assert.AreEqual(1, rule.TaskService.CreateTaskQuery()
                .Count());
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            rule.RuntimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("join")
                .Execute();
            Assert.AreEqual(0, rule.TaskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "afterJoin")
                .Count());

            testHelper.CompleteTask("parallel1");
            testHelper.CompleteTask("afterJoin");
            testHelper.AssertProcessEnded(processInstance.Id);
        }

        /// <summary>
        ///     Ensures that situations are avoided in which more tokens end up at the target gateway
        ///     than it has incoming flows
        /// </summary>
        [Test]
        public virtual void testCannotRemoveParentScope()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW_IN_SUBPROCESS);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("join", "join")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("join", "The gateway's flow scope 'subProcess' must be mapped");
            }
        }

        /// <summary>
        ///     Ensures that situations are avoided in which more tokens end up at the target gateway
        ///     than it has incoming flows
        /// </summary>
        [Test]
        public virtual void testCannotMapMultipleGatewaysToOne()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(GatewayModels.PARALLEL_GW);

            try
            {
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapActivities("join", "join")
                    .MapActivities("fork", "join")
                    .Build();
                Assert.Fail("exception expected");
            }
            catch (MigrationPlanValidationException e)
            {
                // then
                MigrationPlanValidationReportAssert.That(e.ValidationReport)
                    .HasInstructionFailures("join", "Only one gateway can be mapped to gateway 'join'");
            }
        }
    }
}