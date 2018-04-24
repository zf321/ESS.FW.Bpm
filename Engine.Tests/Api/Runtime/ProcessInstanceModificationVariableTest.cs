using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    [TestFixture]
    public class ProcessInstanceModificationVariableTest
    {
        [SetUp]
        public virtual void initialize()
        {
            runtimeService = engineRule.RuntimeService;
            taskService = engineRule.TaskService;
        }

        protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain ruleChain;

        internal IRuntimeService runtimeService;
        internal ITaskService taskService;
        protected internal ProcessEngineTestRule testHelper;

        public ProcessInstanceModificationVariableTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            ////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
        }

        [Test]
        public virtual void modifyAProcessInstanceWithLocalVariableCreation()
        {
            // given a process that sets a local variable when entering the user task
            var instance = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("Process")
                .StartEvent()
                .UserTask("userTask")
                .CamundaTaskListenerClass("create", "api.Runtime.util.CreateLocalVariableEventListener")
                .EndEvent()
                .Done();

            testHelper.DeployAndGetDefinition(instance);
            var processInstance = runtimeService.StartProcessInstanceByKey("Process");

            var updatedTree = runtimeService.GetActivityInstance(processInstance.Id);

            // when I start another activity and Delete the old one
            runtimeService.CreateProcessInstanceModification(processInstance.Id)
                .StartBeforeActivity("userTask")
                .CancelActivityInstance(updatedTree.GetActivityInstances("userTask")[0].Id)
                .Execute(false, false);

            // then migration was successful and I can finish the process
            var task = taskService.CreateTaskQuery()
                .First();
            taskService.Complete(task.Id);
            testHelper.AssertProcessEnded(processInstance.Id);
        }
    }
}