using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class ModificationUserOperationLogTest
    {
        [SetUp]
        public virtual void initServices()
        {
            runtimeService = rule.RuntimeService;
        }

        [SetUp]
        public virtual void setClock()
        {
            ClockUtil.CurrentTime = START_DATE;
        }

        [SetUp]
        public virtual void createBpmnModelInstance()
        {
            instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1")
                .StartEvent("start")
                .UserTask("user1")
                .SequenceFlowId("seq")
                .UserTask("user2")
                .EndEvent("end")
                .Done();
        }

        [TearDown]
        public virtual void resetClock()
        {
            ClockUtil.Reset();
        }

        [TearDown]
        public virtual void removeInstanceIds()
        {
            helper.CurrentProcessInstances = new List<string>();
        }

        [TearDown]
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);
        protected internal BatchModificationHelper helper;
        protected internal IBpmnModelInstance instance;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineTestRule testRule;

        public ModificationUserOperationLogTest()
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
            helper = new BatchModificationHelper(rule);
            ////ruleChain = RuleChain.outerRule(rule).around(testRule);
        }

        [Test]
        public virtual void testLogCreation()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            rule.IdentityService.AuthenticatedUserId = "userId";

            // when
            helper.StartBeforeAsync("process1", 10, "user2", processDefinition.Id);
            rule.IdentityService.ClearAuthentication();

            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery()
                
                .ToList();
            Assert.AreEqual(2, opLogEntries.Count);

            var entries = asMap(opLogEntries);


            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("ModifyProcessInstance", asyncEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, asyncEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("true", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("ModifyProcessInstance", numInstancesEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, numInstancesEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("10", numInstancesEntry.NewValue);

            Assert.AreEqual(asyncEntry.OperationId, numInstancesEntry.OperationId);
        }

        [Test]
        public virtual void testNoCreationOnSyncBatchJobExecution()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            var batch = runtimeService.CreateModification(processDefinition.Id)
                .StartAfterActivity("user2")
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            helper.ExecuteSeedJob(batch);

            // when
            rule.IdentityService.AuthenticatedUserId = "userId";
            helper.ExecuteJobs(batch);
            rule.IdentityService.ClearAuthentication();

            // then
            Assert.AreEqual(0, rule.HistoryService.CreateUserOperationLogQuery()
                .Count());
        }

        [Test]
        public virtual void testNoCreationOnJobExecutorBatchJobExecution()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            runtimeService.CreateModification(processDefinition.Id)
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(processInstance.Id)
                .ExecuteAsync();

            // when
            testRule.WaitForJobExecutorToProcessAllJobs(5000L);

            // then
            Assert.AreEqual(0, rule.HistoryService.CreateUserOperationLogQuery()
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