using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class TaskCandidateTest : PluggableProcessEngineTestCase
    {
        private const string KERMIT = "kermit";

        private const string GONZO = "gonzo";

        [SetUp]
        public virtual void runBare()
        {
            var accountants = identityService.NewGroup("accountancy");
            identityService.SaveGroup(accountants);
            var managers = identityService.NewGroup("management");
            identityService.SaveGroup(managers);
            var sales = identityService.NewGroup("sales");
            identityService.SaveGroup(sales);

            var kermit = identityService.NewUser(KERMIT);
            identityService.SaveUser(kermit);
            identityService.CreateMembership(KERMIT, "accountancy");

            var gonzo = identityService.NewUser(GONZO);
            identityService.SaveUser(gonzo);
            identityService.CreateMembership(GONZO, "management");
            identityService.CreateMembership(GONZO, "accountancy");
            identityService.CreateMembership(GONZO, "sales");
        }
        [TearDown]
        public virtual void tearDown()
        {
            identityService.DeleteUser(KERMIT);
            identityService.DeleteUser(GONZO);
            identityService.DeleteGroup("sales");
            identityService.DeleteGroup("accountancy");
            identityService.DeleteGroup("management");
            
        }

        [Test]
        [Deployment]
        public virtual void testMixedCandidateUserAndGroup()
        {
            runtimeService.StartProcessInstanceByKey("mixedCandidateUserAndGroupExample");

            var taskManager = taskService.GetTaskManager();
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(GONZO).Count );
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(KERMIT).Count);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleCandidateGroups()
        {
            // Deploy and start process
            var processInstance = runtimeService.StartProcessInstanceByKey("multipleCandidatesGroup");

            // ITask should not yet be assigned to anyone
            var taskManager = taskService.GetTaskManager();
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == KERMIT)
                .ToList();

            Assert.True(tasks.Count == 0);
            tasks = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == GONZO)
                .ToList();

            Assert.True(tasks.Count == 0);

            // The task should be visible in the candidate task list of Gonzo and Kermit
            // and anyone in the management/accountancy group
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(KERMIT).Count);
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(GONZO).Count);
            Assert.AreEqual(1, taskManager.FindTasksByCandidateGroup("management").Count());
            Assert.AreEqual(1, taskManager.FindTasksByCandidateGroup("accountancy").Count());
            Assert.AreEqual(0, taskManager.FindTasksByCandidateGroup("sales").Count());

            // Gonzo claims the task
            var taskss = taskManager.FindTasksByCandidateUser(GONZO);
            ITask task = taskss[0];
            Assert.AreEqual("Approve expenses", task.Name);
            taskService.Claim(task.Id, GONZO);

            //// The task must now be gone from the candidate task lists
            Assert.True(!taskManager.FindTasksByCandidateUser(KERMIT).ToList().Any());
            Assert.True(!taskManager.FindTasksByCandidateUser(GONZO).ToList().Any());
            Assert.AreEqual(0, taskManager.FindTasksByCandidateGroup("management").Count());

            // The task will be visible on the personal task list of Gonzo
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == GONZO)
                .Count());

            // But not on the personal task list of (for example) Kermit
            Assert.AreEqual(0, taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == KERMIT)
                .Count());

            // Completing the task ends the process
            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMultipleCandidateUsers()
        {
            runtimeService.StartProcessInstanceByKey("multipleCandidateUsersExample");

            var taskManager = taskService.GetTaskManager();
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(GONZO).Count);
            Assert.AreEqual(1, taskManager.FindTasksByCandidateUser(KERMIT).Count);
        }

        [Test]
        [Deployment]
        public virtual void testSingleCandidateGroup()
        {
            // Deploy and start process
            var processInstance = runtimeService.StartProcessInstanceByKey("singleCandidateGroup");

            // ITask should not yet be assigned to kermit
            IList<ITask> taskss = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == KERMIT)
                .ToList();
            Assert.True(taskss.Count == 0);

            // The task should be visible in the candidate task list
            var taskManager = taskService.GetTaskManager();
            var tasks = taskManager.FindTasksByCandidateUser(KERMIT).ToList();
            Assert.AreEqual(1, tasks.Count);
            var task = tasks[0];
            Assert.AreEqual("Pay out expenses", task.Name);

            // Claim the task
            taskService.Claim(task.Id, KERMIT);

            // The task must now be gone from the candidate task list
            tasks = taskManager.FindTasksByCandidateUser(KERMIT).ToList();
            Assert.True(tasks.Count == 0);

            // The task will be visible on the personal task list
            taskss = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == KERMIT)
                .ToList();
            Assert.AreEqual(1, taskss.Count);
            task = (TaskEntity) taskss[0];
            Assert.AreEqual("Pay out expenses", task.Name);

            // Completing the task ends the process
            taskService.Complete(task.Id);

            AssertProcessEnded(processInstance.Id);
        }
    }
}