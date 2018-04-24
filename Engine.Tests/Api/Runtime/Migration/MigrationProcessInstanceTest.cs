using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    [TestFixture]
    public class MigrationProcessInstanceTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
        }

        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testHelper);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal MigrationTestRule testHelper;

        public MigrationProcessInstanceTest()
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
        public virtual void testMigrateWithIdVarargsArray()
        {
            var sourceDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan = runtimeService.CreateMigrationPlan(sourceDefinition.Id, targetDefinition.Id)
                .MapEqualActivities()
                .Build();

            var processInstance1 = runtimeService.StartProcessInstanceById(sourceDefinition.Id);
            var processInstance2 = runtimeService.StartProcessInstanceById(sourceDefinition.Id);

            // when
            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .Execute();

            // then
            Assert.AreEqual(2, runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId == targetDefinition.Id)
                .Count());
        }

        [Test]
        public virtual void testNullMigrationPlan()
        {
            try
            {
                runtimeService.NewMigration(null)
                    .ProcessInstanceIds(new List<string>())
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("migration plan is null"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceIdsList()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds((IList<string>) null)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }
        [Test]
        public virtual void testProcessInstanceIdsListWithNullValue()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds("foo", null, "bar")
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void testNullProcessInstanceIdsArray()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(null)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testProcessInstanceIdsArrayWithNullValue()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds("foo", null, "bar")
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids contains null value"));
            }
        }
        [Test]
        public virtual void testEmptyProcessInstanceIdsList()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(new List<string>())
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testEmptyProcessInstanceIdsArray()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds()
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testNotMigrateProcessInstanceOfWrongProcessDefinition()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var wrongProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            var processInstance = runtimeService.StartProcessInstanceById(wrongProcessDefinition.Id);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(processInstance.Id)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("ENGINE-23002"));
            }
        }

        [Test]
        public virtual void testNotMigrateUnknownProcessInstance()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds("unknown")
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("ENGINE-23003"));
            }
        }

        [Test]
        public virtual void testNotMigrateNullProcessInstance()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceIds(null)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void testMigrateProcessInstanceQuery()
        {
            var processInstanceCount = 10;

            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            for (var i = 0; i < processInstanceCount; i++)
                runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            var targetProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId ==targetProcessDefinition.Id);

            Assert.AreEqual(processInstanceCount, sourceProcessInstanceQuery.Count());
            Assert.AreEqual(0, targetProcessInstanceQuery.Count());


            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            Assert.AreEqual(0, sourceProcessInstanceQuery.Count());
            Assert.AreEqual(processInstanceCount, targetProcessInstanceQuery.Count());
        }

        [Test]
        public virtual void testNullProcessInstanceQuery()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceQuery(null)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testEmptyProcessInstanceQuery()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            var emptyProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery();
            Assert.AreEqual(0, emptyProcessInstanceQuery.Count());

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceQuery(emptyProcessInstanceQuery)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("process instance ids is empty"));
            }
        }

        [Test]
        public virtual void testProcessInstanceQueryOfWrongProcessDefinition()
        {
            var testProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var wrongProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.SubprocessProcess);

            runtimeService.StartProcessInstanceById(wrongProcessDefinition.Id);

            var migrationPlan = runtimeService.CreateMigrationPlan(testProcessDefinition.Id, testProcessDefinition.Id)
                .MapEqualActivities()
                .Build();

            var wrongProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==wrongProcessDefinition.Id);
            Assert.AreEqual(1, wrongProcessInstanceQuery.Count());

            try
            {
                runtimeService.NewMigration(migrationPlan)
                    .ProcessInstanceQuery(wrongProcessInstanceQuery)
                    .Execute();
                Assert.Fail("Should not be able to migrate");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("ENGINE-23002"));
            }
        }

        [Test]
        public virtual void testProcessInstanceIdsAndQuery()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance1 = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var processInstance2 = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=> c.ProcessDefinitionId== processInstance2.Id);
            var targetProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==targetProcessDefinition.Id);

            Assert.AreEqual(0, targetProcessInstanceQuery.Count());

            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance1.Id)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            Assert.AreEqual(2, targetProcessInstanceQuery.Count());
        }

        [Test]
        public virtual void testOverlappingProcessInstanceIdsAndQuery()
        {
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);

            var migrationPlan =
                runtimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance1 = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var processInstance2 = runtimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            var sourceProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==sourceProcessDefinition.Id);
            var targetProcessInstanceQuery = runtimeService.CreateProcessInstanceQuery(c=>c.ProcessDefinitionId==targetProcessDefinition.Id);

            Assert.AreEqual(0, targetProcessInstanceQuery.Count());

            runtimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ProcessInstanceQuery(sourceProcessInstanceQuery)
                .Execute();

            Assert.AreEqual(2, targetProcessInstanceQuery.Count());
        }
    }
}