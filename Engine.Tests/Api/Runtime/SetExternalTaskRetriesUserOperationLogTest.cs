using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class SetExternalTaskRetriesUserOperationLogTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
            externalTaskService = rule.ExternalTaskService;
            managementService = rule.ManagementService;
        }

        [SetUp]
        public virtual void deployTestProcesses()
        {
            var deployment = rule.RepositoryService.CreateDeployment()
                .AddClasspathResource("resources/api/externaltask/oneExternalTaskProcess.bpmn20.xml")
                .AddClasspathResource("resources/api/externaltask/externalTaskPriorityExpression.bpmn20.xml")
                .Deploy();

            rule.ManageDeployment(deployment);

            var runtimeService = rule.RuntimeService;
            processInstanceIds = new List<string>();
            for (var i = 0; i < 4; i++)
                processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY, i + "")
                    .Id);
            processInstanceIds.Add(runtimeService.StartProcessInstanceByKey(PROCESS_DEFINITION_KEY_2)
                .Id);
        }

        [SetUp]
        public virtual void setClock()
        {
            ClockUtil.CurrentTime = START_DATE;
        }

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [TearDown]
        public virtual void removeAllRunningAndHistoricBatches()
        {
            var historyService = rule.HistoryService;
            var managementService = rule.ManagementService;
            foreach (var batch in managementService.CreateBatchQuery()
                
                .ToList())
                managementService.DeleteBatch(batch.Id, true);
            // remove history of completed batches
            foreach (var historicBatch in historyService.CreateHistoricBatchQuery()
                
                .ToList())
                historyService.DeleteHistoricBatch(historicBatch.Id);
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testRule);
        //public RuleChain ruleChain;

        private static readonly string PROCESS_DEFINITION_KEY = "oneExternalTaskProcess";
        private static readonly string PROCESS_DEFINITION_KEY_2 = "twoExternalTaskWithPriorityProcess";
        protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);
        protected internal IExternalTaskService externalTaskService;
        private readonly bool InstanceFieldsInitialized;
        protected internal IManagementService managementService;

        protected internal IList<string> processInstanceIds;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineTestRule testRule;

        public SetExternalTaskRetriesUserOperationLogTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(rule);
            ////ruleChain = RuleChain.outerRule(rule).around(testRule);
        }
        [Test]
        public virtual void testLogCreationForOneExternalTaskId()
        {
            // given
            rule.IdentityService.AuthenticatedUserId = "userId";

            // when
            var externalTask = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId== processInstanceIds[0])
                .First();
            externalTaskService.SetRetries(externalTask.Id, 5);
            rule.IdentityService.ClearAuthentication();
            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery()
                
                .ToList();
            Assert.AreEqual(1, opLogEntries.Count);

            var entries = asMap(opLogEntries);

            var retriesEntry = entries["retries"];
            Assert.NotNull(retriesEntry);
            Assert.AreEqual("IProcessInstance", retriesEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", retriesEntry.OperationType);
            Assert.AreEqual(externalTask.ProcessInstanceId, retriesEntry.ProcessInstanceId);
            Assert.AreEqual(externalTask.ProcessDefinitionId, retriesEntry.ProcessDefinitionId);
            Assert.AreEqual(externalTask.ProcessDefinitionKey, retriesEntry.ProcessDefinitionKey);
            Assert.IsNull(retriesEntry.OrgValue);
            Assert.AreEqual("5", retriesEntry.NewValue);
        }

        [Test]
        public virtual void testLogCreationSync()
        {
            // given
            rule.IdentityService.AuthenticatedUserId = "userId";
            var list = externalTaskService.CreateExternalTaskQuery()
                
                .ToList();
            IList<string> externalTaskIds = new List<string>();

            foreach (var task in list)
                externalTaskIds.Add(task.Id);

            // when
            externalTaskService.SetRetries(externalTaskIds, 5);
            rule.IdentityService.ClearAuthentication();
            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery()
                
                .ToList();
            Assert.AreEqual(3, opLogEntries.Count);

            var entries = asMap(opLogEntries);

            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", asyncEntry.OperationType);
            Assert.IsNull(asyncEntry.ProcessDefinitionId);
            Assert.IsNull(asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("false", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", numInstancesEntry.OperationType);
            Assert.IsNull(numInstancesEntry.ProcessDefinitionId);
            Assert.IsNull(numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("6", numInstancesEntry.NewValue);

            var retriesEntry = entries["retries"];
            Assert.NotNull(retriesEntry);
            Assert.AreEqual("IProcessInstance", retriesEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", retriesEntry.OperationType);
            Assert.IsNull(retriesEntry.ProcessDefinitionId);
            Assert.IsNull(retriesEntry.ProcessDefinitionKey);
            Assert.IsNull(retriesEntry.ProcessInstanceId);
            Assert.IsNull(retriesEntry.OrgValue);
            Assert.AreEqual("5", retriesEntry.NewValue);
            Assert.AreEqual(asyncEntry.OperationId, retriesEntry.OperationId);
        }

        [Test]
        public virtual void testLogCreationAsync()
        {
            // given
            rule.IdentityService.AuthenticatedUserId = "userId";

            // when
            externalTaskService.SetRetriesAsync(null, externalTaskService.CreateExternalTaskQuery(), 5);
            rule.IdentityService.ClearAuthentication();
            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery()
                
                .ToList();
            Assert.AreEqual(3, opLogEntries.Count);

            var entries = asMap(opLogEntries);

            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", asyncEntry.OperationType);
            Assert.IsNull(asyncEntry.ProcessDefinitionId);
            Assert.IsNull(asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("true", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", numInstancesEntry.OperationType);
            Assert.IsNull(numInstancesEntry.ProcessDefinitionId);
            Assert.IsNull(numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("6", numInstancesEntry.NewValue);

            var retriesEntry = entries["retries"];
            Assert.NotNull(retriesEntry);
            Assert.AreEqual("IProcessInstance", retriesEntry.EntityType);
            Assert.AreEqual("SetExternalTaskRetries", retriesEntry.OperationType);
            Assert.IsNull(retriesEntry.ProcessDefinitionId);
            Assert.IsNull(retriesEntry.ProcessDefinitionKey);
            Assert.IsNull(retriesEntry.ProcessInstanceId);
            Assert.IsNull(retriesEntry.OrgValue);
            Assert.AreEqual("5", retriesEntry.NewValue);
            Assert.AreEqual(asyncEntry.OperationId, retriesEntry.OperationId);
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