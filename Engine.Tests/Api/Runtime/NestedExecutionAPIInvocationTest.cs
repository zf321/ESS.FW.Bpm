using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class NestedExecutionAPIInvocationTest
    {
        [SetUp]
        public virtual void init()
        {
            StartProcessOnAnotherEngineDelegate.engine = engine2BootstrapRule.ProcessEngine;
            NestedProcessStartDelegate.engine = engineRule1.ProcessEngine;

            // given
            var deployment1 = engineRule1.RepositoryService.CreateDeployment()
                .AddModelInstance("foo.bpmn", PROCESS_MODEL)
                .Deploy();

            var deployment2 = engineRule1.RepositoryService.CreateDeployment()
                .AddModelInstance("boo.bpmn", PROCESS_MODEL_2)
                .Deploy();

            engineRule1.ManageDeployment(deployment1);
            engineRule1.ManageDeployment(deployment2);

            var deployment3 = engineRule2.ProcessEngine.RepositoryService.CreateDeployment()
                .AddModelInstance("joo.bpmn", ONE_TASK_PROCESS_MODEL)
                .Deploy();

            engineRule2.ManageDeployment(deployment3);
        }

        [TearDown]
        public virtual void clearEngineReference()
        {
            StartProcessOnAnotherEngineDelegate.engine = null;
            NestedProcessStartDelegate.engine = null;
        }

        public const string PROCESS_KEY_1 = "process";

        public const string PROCESS_KEY_2 = "multiEngineProcess";

        public const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static util.ProcessEngineBootstrapRule engine2BootstrapRule = new util.ProcessEngineBootstrapRule("camunda.cfg.prefix_extended.xml");
        public static ProcessEngineBootstrapRule engine2BootstrapRule =
            new ProcessEngineBootstrapRule("camunda.cfg.prefix_extended.xml");

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_MODEL = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_KEY_1)
            .StartEvent()
            .UserTask("waitState")
            .ServiceTask("startProcess")
            .CamundaClass(typeof(NestedProcessStartDelegate).FullName)
            .EndEvent()
            .Done();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        public static readonly IBpmnModelInstance PROCESS_MODEL_2 =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(PROCESS_KEY_2)
                .StartEvent()
                .UserTask("waitState")
                .ServiceTask("startProcess")
                .CamundaClass(typeof(StartProcessOnAnotherEngineDelegate).FullName)
                .EndEvent()
                .Done();

        public static readonly IBpmnModelInstance ONE_TASK_PROCESS_MODEL =
            ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ONE_TASK_PROCESS_KEY)
                .StartEvent()
                .UserTask("waitState")
                .EndEvent()
                .Done();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule1 = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule engineRule1 = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule engineRule2 = new util.ProvidedProcessEngineRule(engine2BootstrapRule);
        public ProcessEngineRule engineRule2 = new ProvidedProcessEngineRule(engine2BootstrapRule);

        [Test]
        public virtual void testWaitStateIsReachedOnNestedInstantiation()
        {
            engineRule1.RuntimeService.StartProcessInstanceByKey(PROCESS_KEY_1);
            var taskId = engineRule1.TaskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            engineRule1.TaskService.Complete(taskId);
        }

        [Test]
        public virtual void testWaitStateIsReachedOnMultiEngine()
        {
            engineRule1.RuntimeService.StartProcessInstanceByKey(PROCESS_KEY_2);
            var taskId = engineRule1.TaskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            engineRule1.TaskService.Complete(taskId);
        }

        public class StartProcessOnAnotherEngineDelegate : IJavaDelegate
        {
            public static IProcessEngine engine;
            
            public void Execute(IBaseDelegateExecution execution)
            {
                var runtimeService = engine.RuntimeService;

                var processInstance = runtimeService.StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);

                // then the wait state is reached immediately after instantiation
                var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
                var activityInstances = activityInstance.GetActivityInstances("waitState");
                Assert.AreEqual(1, activityInstances.Length);
            }
        }

        public class NestedProcessStartDelegate : IJavaDelegate
        {
            public static IProcessEngine engine;
            public void Execute(IBaseDelegateExecution execution)
            {
                var runtimeService = engine.RuntimeService;

                var processInstance = runtimeService.StartProcessInstanceByKey("process");

                // then the wait state is reached immediately after instantiation
                var activityInstance = runtimeService.GetActivityInstance(processInstance.Id);
                var activityInstances = activityInstance.GetActivityInstances("waitState");
                Assert.AreEqual(1, activityInstances.Length);
            }
        }
    }
}