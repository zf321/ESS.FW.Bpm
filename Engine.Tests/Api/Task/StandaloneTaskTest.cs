using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class StandaloneTaskTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void setUp()
        {;
            identityService.SaveUser(identityService.NewUser("kermit"));
            identityService.SaveUser(identityService.NewUser("gonzo"));
        }

        [TearDown]
        public virtual void tearDown()
        {
            identityService.DeleteUser("kermit");
            identityService.DeleteUser("gonzo");
        }

        [Test]
        public virtual void testCreateToComplete()
        {
            // Create and save task
            var task = taskService.NewTask();
            task.Name = "testTask";
            taskService.SaveTask(task);
            var taskId = task.Id;

            // Add user as candidate user
            taskService.AddCandidateUser(taskId, "kermit");
            taskService.AddCandidateUser(taskId, "gonzo");

            // Retrieve task list for jbarrez
            var taskManager = taskService.GetTaskManager();
            var tasks = taskManager.FindTasksByCandidateUser("kermit")
                .ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("testTask", tasks[0].Name);

            //// Retrieve task list for tbaeyens
            tasks = taskManager.FindTasksByCandidateUser("gonzo").ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("testTask", tasks[0].Name);

            //// Claim task
            taskService.Claim(taskId, "kermit");

            //// Tasks shouldn't appear in the candidate tasklists anymore
            Assert.True(!taskManager.FindTasksByCandidateUser("kermit").Any());
            Assert.True(!taskManager.FindTasksByCandidateUser("gonzo").Any());

            // Complete task
            taskService.DeleteTask(taskId, true);

            // ITask should be removed from runtime data
            // TODO: check for historic data when implemented!
            Assert.IsNull(taskService.CreateTaskQuery(c => c.Id == taskId)
                .FirstOrDefault());
        }

        [Test]
        public virtual void testOptimisticLockingThrownOnMultipleUpdates()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            // first modification
            var task1 = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            var task2 = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();

            task1.Description = "first modification";
            taskService.SaveTask(task1);

            // second modification on the initial instance
            task2.Description = "second modification";
            try
            {
                taskService.SaveTask(task2);
                Assert.Fail("should get an exception here as the task was modified by someone else.");
            }
            catch (OptimisticLockingException)
            {
                //  exception was thrown as expected
            }

            taskService.DeleteTask(taskId, true);
        }

        // See http://jira.codehaus.org/browse/ACT-1290
        [Test]
        public virtual void testRevisionUpdatedOnSave()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            Assert.AreEqual(1, ((TaskEntity) task).Revision);

            task.Description = "first modification";
            taskService.SaveTask(task);
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .FirstOrDefault();
            Assert.AreEqual(2, ((TaskEntity) task).Revision);

            task.Description = "second modification";
            taskService.SaveTask(task);
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .FirstOrDefault();
            Assert.AreEqual(3, ((TaskEntity) task).Revision);

            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testSaveTaskWithGenericResourceId()
        {
            var task = taskService.NewTask("*");
            try
            {
                taskService.SaveTask(task);
                Assert.Fail("it should not be possible to save a task with the generic resource id *");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Entity Task[*] has an invalid id: id cannot be *. * is a reserved identifier",
                    e.Message);
            }
        }
    }
}