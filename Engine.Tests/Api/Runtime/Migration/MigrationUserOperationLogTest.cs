using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Migration
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class MigrationUserOperationLogTest
    {
        public const string USER_ID = "userId";
        private readonly bool InstanceFieldsInitialized;

        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
        protected internal MigrationTestRule testHelper;

        public MigrationUserOperationLogTest()
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
        public virtual void testLogCreation()
        {
            // given
            var sourceProcessDefinition = testHelper.DeployAndGetDefinition(ProcessModels.OneTaskProcess);
            var targetProcessDefinition =
                testHelper.DeployAndGetDefinition(ModifiableBpmnModelInstance.Modify(ProcessModels.OneTaskProcess)
                    .ChangeElementId(ProcessModels.ProcessKey, "new" + ProcessModels.ProcessKey));

            var migrationPlan =
                rule.RuntimeService.CreateMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id)
                    .MapEqualActivities()
                    .Build();

            var processInstance = rule.RuntimeService.StartProcessInstanceById(sourceProcessDefinition.Id);

            // when
            rule.IdentityService.AuthenticatedUserId = USER_ID;
            rule.RuntimeService.NewMigration(migrationPlan)
                .ProcessInstanceIds(processInstance.Id)
                .Execute();
            rule.IdentityService.ClearAuthentication();

            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery()
                
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
            Assert.AreEqual("false", asyncEntry.NewValue);

            var numInstanceEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstanceEntry);
            Assert.AreEqual("IProcessInstance", numInstanceEntry.EntityType);
            Assert.AreEqual("Migrate", numInstanceEntry.OperationType);
            Assert.AreEqual(sourceProcessDefinition.Id, numInstanceEntry.ProcessDefinitionId);
            Assert.AreEqual(sourceProcessDefinition.Key, numInstanceEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstanceEntry.ProcessInstanceId);
            Assert.IsNull(numInstanceEntry.OrgValue);
            Assert.AreEqual("1", numInstanceEntry.NewValue);

            Assert.AreEqual(procDefEntry.OperationId, asyncEntry.OperationId);
            Assert.AreEqual(asyncEntry.OperationId, numInstanceEntry.OperationId);
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