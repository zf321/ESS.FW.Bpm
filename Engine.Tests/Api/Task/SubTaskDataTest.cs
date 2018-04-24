using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SubTaskDataTest: ProvidedProcessEngineRule
    {
        protected internal IRepositoryService repositoryService;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        //public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;

        [SetUp]
        public virtual void init()
        {
            repositoryService = this.RepositoryService;
            runtimeService = this.RuntimeService;
            taskService = this.TaskService;
        }

        [Test]
        [Deployment]
        public virtual void testSubTaskData()
        {
            //given simple process with user task
            var processInstance = runtimeService.StartProcessInstanceByKey("subTaskTest");
            var task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .First();

            // when set variable to user task
            taskService.SetVariable(task.Id, "testVariable", "testValue");

            // then variable is set in the scope of execution
            Assert.AreEqual("testValue", runtimeService.GetVariable(task.ExecutionId, "testVariable"));

            // when sub task is created create subtask for user task
            var subTask = taskService.NewTask("123456789");
            subTask.ParentTaskId = task.Id;
            subTask.Name = "Test Subtask";
            taskService.SaveTask(subTask);

            // and variable is update
            taskService.SetVariable(subTask.Id, "testVariable", "newTestValue");

            //then variable is also updated in the scope execution
            Assert.AreEqual("newTestValue", runtimeService.GetVariable(task.ExecutionId, "testVariable"));
        }
    }
}