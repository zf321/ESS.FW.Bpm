using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class UserOperationLogWithoutUserTest : PluggableProcessEngineTestCase
    {
        protected internal const string PROCESS_PATH = "resources/api/oneTaskProcess.bpmn20.xml";
        protected internal const string PROCESS_KEY = "oneTaskProcess";

        [Test]
        public virtual void testCreateTask()
        {
            // when
            var task = taskService.NewTask("a-task-id");
            taskService.SaveTask(task);

            // then
            verifyNoUserOperationLogged();

            taskService.DeleteTask("a-task-id", true);
        }
        [Test]
        [Deployment( "resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testActivateJobDefinition()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneFailingServiceTaskProcess");
            var id = managementService.CreateJobDefinitionQuery()
                .First()
                .Id;

            // when
            managementService.ActivateJobByJobDefinitionId(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSuspendJobDefinition()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneFailingServiceTaskProcess");
            var id = managementService.CreateJobDefinitionQuery()
                .First()
                .Id;

            // when
            managementService.SuspendJobByJobDefinitionId(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testActivateJob()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneFailingServiceTaskProcess");
            var id = managementService.CreateJobQuery()
                .First()
                .Id;

            // when
            managementService.ActivateJobById(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSuspendJob()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneFailingServiceTaskProcess");
            var id = managementService.CreateJobQuery()
                .First()
                .Id;

            // when
            managementService.SuspendJobById(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment("resources/api/runtime/oneFailingServiceProcess.bpmn20.xml")]
        public virtual void testSetJobRetries()
        {
            // given
            runtimeService.StartProcessInstanceByKey("oneFailingServiceTaskProcess");
            var id = managementService.CreateJobQuery()
                .First()
                .Id;

            // when
            managementService.SetJobRetries(id, 5);

            // then
            verifyNoUserOperationLogged();
        }

        protected internal virtual void verifyNoUserOperationLogged()
        {
            var query = historyService.CreateUserOperationLogQuery();
            Assert.AreEqual(0, query.Count());
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testActivateProcessDefinition()
        {
            // when
            repositoryService.ActivateProcessDefinitionByKey(PROCESS_KEY);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testActivateProcessInstance()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey(PROCESS_KEY)
                .Id;

            // when
            runtimeService.ActivateProcessInstanceById(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testAssignTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.SetAssignee(taskId, "demo");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testClaimTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.Claim(taskId, "demo");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testCompleteTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.Complete(taskId);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testDelegateTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.DelegateTask(taskId, "demo");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testModifyProcessInstance()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey(PROCESS_KEY)
                .Id;

            // when
            runtimeService.CreateProcessInstanceModification(id)
                .CancelAllForActivity("theTask")
                .Execute();

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testRemoveVariable()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey(PROCESS_KEY)
                .Id;
            runtimeService.SetVariable(id, "aVariable", "aValue");

            // when
            runtimeService.RemoveVariable(id, "aVariable");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testResolveTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.ResolveTask(taskId);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testSetOwnerTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.SetOwner(taskId, "demo");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testSetPriorityTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.SetPriority(taskId, 60);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testSetVariable()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey(PROCESS_KEY)
                .Id;

            // when
            runtimeService.SetVariable(id, "aVariable", "aValue");

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testSuspendProcessDefinition()
        {
            // when
            repositoryService.SuspendProcessDefinitionByKey(PROCESS_KEY);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testSuspendProcessInstance()
        {
            // given
            var id = runtimeService.StartProcessInstanceByKey(PROCESS_KEY)
                .Id;

            // when
            runtimeService.SuspendProcessInstanceById(id);

            // then
            verifyNoUserOperationLogged();
        }

        [Test]
        [Deployment(PROCESS_PATH)]
        public virtual void testUpdateTask()
        {
            // given
            runtimeService.StartProcessInstanceByKey(PROCESS_KEY);
            var task = taskService.CreateTaskQuery()
                .First();
            task.CaseInstanceId = "a-case-instance-id";

            // when
            taskService.SaveTask(task);

            // then
            verifyNoUserOperationLogged();
        }
    }
}