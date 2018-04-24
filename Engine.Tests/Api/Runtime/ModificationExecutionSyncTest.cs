using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [TestFixture]
    public class ModificationExecutionSyncTest
    {
        [SetUp]
        public virtual void CreateBpmnModelInstance()
        {
            Instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1")
                .StartEvent("start")
                .UserTask("user1")
                .SequenceFlowId("seq")
                .UserTask("user2")
                .UserTask("user3")
                .EndEvent("end")
                .Done();
        }

        [SetUp]
        public virtual void InitServices()
        {
            RuntimeService = Rule.RuntimeService;
        }

        [TearDown]
        public virtual void RemoveInstanceIds()
        {
            Helper.CurrentProcessInstances = new List<string>();
        }

        private readonly bool _instanceFieldsInitialized;
        protected internal BatchModificationHelper Helper;
        protected internal IBpmnModelInstance Instance;


        protected internal ProcessEngineRule Rule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(rule).around(testRule);
        //public RuleChain ruleChain;

        protected internal IRuntimeService RuntimeService;
        protected internal ProcessEngineTestRule TestRule;

        public ModificationExecutionSyncTest()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            TestRule = new ProcessEngineTestRule(Rule);
            Helper = new BatchModificationHelper(Rule);
            ////ruleChain = RuleChain.outerRule(rule).around(testRule);
        }

        [Test]
        public virtual void CreateSimpleModificationPlan()
        {
            var processDefinition = TestRule.DeployAndGetDefinition(Instance);
            var instances = Helper.StartInstances("process1", 2);
            RuntimeService.CreateModification(processDefinition.Id)
                .StartBeforeActivity("user2")
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(instances)
                .Execute();

            foreach (var instanceId in instances)
            {
                var activeActivityIds = RuntimeService.GetActiveActivityIds(instanceId);
                Assert.AreEqual(1, activeActivityIds.Count);
                Assert.AreEqual(activeActivityIds.GetEnumerator()
                    .Current, "user2");
            }
        }

        [Test]
        public virtual void CreateModificationWithNullProcessInstanceIdsList()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds((IList<string>) null)
                    .Execute();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void CreateModificationUsingProcessInstanceIdsListWithNullValue()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds("foo", null, "bar")
                    .Execute();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void CreateModificationWithEmptyProcessInstanceIdsList()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds(new List<string>())
                    .Execute();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void CreateModificationWithNullProcessDefinitionId()
        {
            try
            {
                RuntimeService.CreateModification(null)
                    .CancelAllForActivity("activityId")
                    .SetProcessInstanceIds("20", "1--0")
                    .Execute();
                Assert.Fail("Should not succed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinitionId is null"));
            }
        }

        [Test]
        public virtual void CreateModificationWithNullProcessInstanceIdsArray()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceIds(null)
                    .Execute();
                Assert.Fail("Should not be able to Modify");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void CreateModificationUsingProcessInstanceIdsArrayWithNullValue()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .CancelAllForActivity("user1")
                    .SetProcessInstanceIds("foo", null, "bar")
                    .Execute();
                Assert.Fail("Should not be able to Modify");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids contains null value"));
            }
        }

        [Test]
        public virtual void TestNullProcessInstanceQuery()
        {
            try
            {
                RuntimeService.CreateModification("processDefinitionId")
                    .StartAfterActivity("user1")
                    .SetProcessInstanceQuery(null)
                    .Execute();
                Assert.Fail("Should not succeed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("Process instance ids is empty"));
            }
        }

        [Test]
        public virtual void CreateModificationWithNotMatchingProcessDefinitionId()
        {
            var deployment = TestRule.Deploy(Instance);
            var deploymentDeployedProcessDefinition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 2);
            try
            {
                RuntimeService.CreateModification("foo")
                    .CancelAllForActivity("activityId")
                    .SetProcessInstanceIds(processInstanceIds)
                    .Execute();
                Assert.Fail("Should not succed");
            }
            catch (ProcessEngineException e)
            {
                Assert.That(e.Message, Does.Contain("processDefinition is null"));
            }
        }

        [Test]
        public virtual void TestStartBefore()
        {
            var deployment = TestRule.Deploy(Instance);
            var definition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 2);

            RuntimeService.CreateModification(definition.Id)
                .StartBeforeActivity("user2")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            foreach (var processInstanceId in processInstanceIds)
            {
                var updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }
        }

        [Test]
        public virtual void TestStartAfter()
        {
            var deployment = TestRule.Deploy(Instance);
            var definition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 2);

            RuntimeService.CreateModification(definition.Id)
                .StartAfterActivity("user2")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            foreach (var processInstanceId in processInstanceIds)
            {
                var updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                        .Activity("user1")
                        .Activity("user3")
                        .Done());
            }
        }

        [Test]
        public virtual void TestStartTransition()
        {
            var deployment = TestRule.Deploy(Instance);
            var definition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 2);

            RuntimeService.CreateModification(definition.Id)
                .StartTransition("seq")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            foreach (var processInstanceId in processInstanceIds)
            {
                var updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                        .Activity("user1")
                        .Activity("user2")
                        .Done());
            }
        }

        [Test]
        public virtual void TestCancelAll()
        {
            var processDefinition = TestRule.DeployAndGetDefinition(Instance);
            var processInstanceIds = Helper.StartInstances("process1", 2);

            RuntimeService.CreateModification(processDefinition.Id)
                .CancelAllForActivity("user1")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            foreach (var processInstanceId in processInstanceIds)
            {
                var updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
                Assert.IsNull(updatedTree);
            }
        }

        [Test]
        public virtual void TestStartBeforeAndCancelAll()
        {
            var deployment = TestRule.Deploy(Instance);
            var definition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 2);

            RuntimeService.CreateModification(definition.Id)
                .CancelAllForActivity("user1")
                .StartBeforeActivity("user2")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            foreach (var processInstanceId in processInstanceIds)
            {
                var updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
                Assert.NotNull(updatedTree);
                Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);

                ActivityInstanceAssert.That(updatedTree)
                    .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                        .Activity("user2")
                        .Done());
            }
        }

        [Test]
        public virtual void TestDifferentStates()
        {
            var deployment = TestRule.Deploy(Instance);
            var definition = deployment.DeployedProcessDefinitions[0];

            var processInstanceIds = Helper.StartInstances("process1", 1);
            var task = Rule.TaskService.CreateTaskQuery()
                .First();
            Rule.TaskService.Complete(task.Id);

            var anotherProcessInstanceIds = Helper.StartInstances("process1", 1);
            ((List<string>) processInstanceIds).AddRange(anotherProcessInstanceIds);

            RuntimeService.CreateModification(definition.Id)
                .StartBeforeActivity("user3")
                .SetProcessInstanceIds(processInstanceIds)
                .Execute();

            IActivityInstance updatedTree = null;
            var processInstanceId = processInstanceIds[0];
            updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                    .Activity("user2")
                    .Activity("user3")
                    .Done());

            processInstanceId = processInstanceIds[1];
            updatedTree = RuntimeService.GetActivityInstance(processInstanceId);
            Assert.NotNull(updatedTree);
            Assert.AreEqual(processInstanceId, updatedTree.ProcessInstanceId);
            ActivityInstanceAssert.That(updatedTree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(definition.Id)
                    .Activity("user1")
                    .Activity("user3")
                    .Done());
        }
    }
}