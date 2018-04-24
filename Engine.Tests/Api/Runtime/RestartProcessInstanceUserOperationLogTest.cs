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
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class RestartProcessInstanceUserOperationLogTest
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
        public virtual void removeBatches()
        {
            helper.RemoveAllRunningAndHistoricBatches();
        }

        protected internal static readonly DateTime START_DATE = new DateTime(1457326800000L);
        protected internal BatchRestartHelper helper;
        protected internal IBpmnModelInstance instance;
        private readonly bool InstanceFieldsInitialized;


        protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService runtimeService;
        protected internal ProcessEngineTestRule testRule;

        public RestartProcessInstanceUserOperationLogTest()
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
            helper = new BatchRestartHelper(rule);
            ////ruleChain = RuleChain.outerRule(rule).around(testRule);
        }

        [Test]
        public virtual void testLogCreationAsync()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            rule.IdentityService.AuthenticatedUserId = "userId";

            var processInstance1 = runtimeService.StartProcessInstanceByKey("process1");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("process1");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartAfterActivity("user1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .ExecuteAsync();
            rule.IdentityService.ClearAuthentication();

            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery(c=>c.OperationType == "RestartProcessInstance")
                
                .ToList();
            Assert.AreEqual(2, opLogEntries.Count);

            var entries = asMap(opLogEntries);


            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("RestartProcessInstance", asyncEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, asyncEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("true", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("RestartProcessInstance", numInstancesEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, numInstancesEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("2", numInstancesEntry.NewValue);

            Assert.AreEqual(asyncEntry.OperationId, numInstancesEntry.OperationId);
        }

        [Test]
        public virtual void testLogCreationSync()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);
            rule.IdentityService.AuthenticatedUserId = "userId";

            var processInstance1 = runtimeService.StartProcessInstanceByKey("process1");
            var processInstance2 = runtimeService.StartProcessInstanceByKey("process1");

            runtimeService.DeleteProcessInstance(processInstance1.Id, "test");
            runtimeService.DeleteProcessInstance(processInstance2.Id, "test");

            // when
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartAfterActivity("user1")
                .SetProcessInstanceIds(processInstance1.Id, processInstance2.Id)
                .Execute();
            rule.IdentityService.ClearAuthentication();

            // then
            var opLogEntries = rule.HistoryService.CreateUserOperationLogQuery(c=>c.OperationType == "RestartProcessInstance")
                
                .ToList();
            Assert.AreEqual(2, opLogEntries.Count);

            var entries = asMap(opLogEntries);


            var asyncEntry = entries["async"];
            Assert.NotNull(asyncEntry);
            Assert.AreEqual("IProcessInstance", asyncEntry.EntityType);
            Assert.AreEqual("RestartProcessInstance", asyncEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, asyncEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, asyncEntry.ProcessDefinitionKey);
            Assert.IsNull(asyncEntry.ProcessInstanceId);
            Assert.IsNull(asyncEntry.OrgValue);
            Assert.AreEqual("false", asyncEntry.NewValue);

            var numInstancesEntry = entries["nrOfInstances"];
            Assert.NotNull(numInstancesEntry);
            Assert.AreEqual("IProcessInstance", numInstancesEntry.EntityType);
            Assert.AreEqual("RestartProcessInstance", numInstancesEntry.OperationType);
            Assert.AreEqual(processDefinition.Id, numInstancesEntry.ProcessDefinitionId);
            Assert.AreEqual(processDefinition.Key, numInstancesEntry.ProcessDefinitionKey);
            Assert.IsNull(numInstancesEntry.ProcessInstanceId);
            Assert.IsNull(numInstancesEntry.OrgValue);
            Assert.AreEqual("2", numInstancesEntry.NewValue);

            Assert.AreEqual(asyncEntry.OperationId, numInstancesEntry.OperationId);
        }

        [Test]
        public virtual void testNoCreationOnSyncBatchJobExecution()
        {
            // given
            var processDefinition = testRule.DeployAndGetDefinition(instance);

            var processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
            var batch = runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartBeforeActivity("user1")
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
            runtimeService.DeleteProcessInstance(processInstance.Id, "test");
            runtimeService.RestartProcessInstances(processDefinition.Id)
                .StartAfterActivity("user1")
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