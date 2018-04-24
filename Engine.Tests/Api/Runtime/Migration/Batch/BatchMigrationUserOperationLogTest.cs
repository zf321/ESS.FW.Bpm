using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration.Batch
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class BatchMigrationUserOperationLogTest
    {
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(migrationRule);
        //public RuleChain ruleChain;

        [TearDown]
        public virtual void removeBatches()
        {
            batchHelper.RemoveAllRunningAndHistoricBatches();
        }

        public const string USER_ID = "userId";

        protected internal BatchMigrationHelper batchHelper;

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;
        protected internal MigrationTestRule migrationRule;

        public BatchMigrationUserOperationLogTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            migrationRule = new MigrationTestRule(engineRule);
            batchHelper = new BatchMigrationHelper(engineRule, migrationRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(migrationRule);
        }

        [Test]
        public virtual void testLogCreation()
        {
            // given
            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            engineRule.IdentityService.AuthenticatedUserId = USER_ID;
            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();
            engineRule.IdentityService.ClearAuthentication();

            // then
            var opLogEntries = engineRule.HistoryService.CreateUserOperationLogQuery()
                
                .ToList();
            Assert.AreEqual(3, opLogEntries.Count);

            var entries = asMap(opLogEntries);

            var procDefEntry = entries["processDefinitionId"];
            Assert.NotNull(procDefEntry);
            Assert.AreEqual("IProcessInstance", procDefEntry.EntityType);
            Assert.AreEqual("Migrate", procDefEntry.OperationType);
            Assert.AreEqual(sourceProcessDefinition.Id, procDefEntry.ProcessDefinitionId);
            Assert.AreEqual(sourceProcessDefinition.Key, procDefEntry.ProcessDefinitionKey);
            Assert.IsNull(procDefEntry.ProcessInstanceId);
            Assert.AreEqual(sourceProcessDefinition.Id, procDefEntry.OrgValue);
            Assert.AreEqual(targetProcessDefinition.Id, procDefEntry.NewValue);

            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("Migrate", asyncEntry.OperationType);
            Assert.AreEqual(sourceProcessDefinition.Id, asyncEntry.ProcessDefinitionId);
            Assert.AreEqual(sourceProcessDefinition.Key, asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("true", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("Migrate", numInstancesEntry.OperationType);
            Assert.AreEqual(sourceProcessDefinition.Id, numInstancesEntry.ProcessDefinitionId);
            Assert.AreEqual(sourceProcessDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("1", numInstancesEntry.NewValue);

            Assert.AreEqual(procDefEntry.OperationId, asyncEntry.OperationId);
            Assert.AreEqual(asyncEntry.OperationId, numInstancesEntry.OperationId);
        }

        [Test]
        public virtual void testNoCreationOnSyncBatchJobExecution()
        {
            // given
            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            var batch = engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();
            batchHelper.ExecuteSeedJob(batch);

            // when
            engineRule.IdentityService.AuthenticatedUserId = USER_ID;
            batchHelper.ExecuteJobs(batch);
            engineRule.IdentityService.ClearAuthentication();

            // then
            Assert.AreEqual(0, engineRule.HistoryService.CreateUserOperationLogQuery()
                .Count());
        }

        [Test]
        public virtual void testNoCreationOnJobExecutorBatchJobExecution()
        {
            // given
            var sourceProcessDefinition = migrationRule.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                migrationRule.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                engineRule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = engineRule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);
            engineRule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            // when
            migrationRule.WaitForJobExecutorToProcessAllJobs(5000L);

            // then
            Assert.AreEqual(0, engineRule.HistoryService.CreateUserOperationLogQuery()
                .Count());
        }

        protected internal virtual IDictionary<string, IUserOperationLogEntry> asMap(
            IList<IUserOperationLogEntry> logEntries)
        {
            IDictionary<string, IUserOperationLogEntry> map = new Dictionary<string, IUserOperationLogEntry>();

            foreach (var entry in logEntries)
            {
                var previousValue = map[entry.Property] = entry;
                if (previousValue != null)
                    Assert.Fail("expected only entry for every property");
            }

            return map;
        }
    }
}