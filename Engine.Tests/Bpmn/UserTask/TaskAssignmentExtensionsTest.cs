using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{

    /// <summary>
    /// Testcase for the non-spec extensions to the task candidate use case.
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class TaskAssignmentExtensionsTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void setUp()
        {
            identityService.SaveUser(identityService.NewUser("kermit"));
            identityService.SaveUser(identityService.NewUser("gonzo"));
            identityService.SaveUser(identityService.NewUser("fozzie"));

            identityService.SaveGroup(identityService.NewGroup("management"));
            identityService.SaveGroup(identityService.NewGroup("accountancy"));

            identityService.CreateMembership("kermit", "management");
            identityService.CreateMembership("kermit", "accountancy");
            identityService.CreateMembership("fozzie", "management");
        }

        [TearDown]
        public virtual void tearDown()
        {
            identityService.DeleteGroup("accountancy");
            identityService.DeleteGroup("management");
            identityService.DeleteUser("fozzie");
            identityService.DeleteUser("gonzo");
            identityService.DeleteUser("kermit");
        }

        [Test]
        [Deployment]
        public virtual void testAssigneeExtension()
        {
            runtimeService.StartProcessInstanceByKey("assigneeExtension");
            IList<ITask> tasks = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == "kermit").ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("my task", tasks[0].Name);
        }
        [Test]
        [Deployment]
        public virtual void testDuplicateAssigneeDeclaration()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testDuplicateAssigneeDeclaration");
                repositoryService.CreateDeployment().AddClasspathResource(resource).Deploy();
                Assert.Fail("Invalid BPMN 2.0 process should not parse, but it gets parsed sucessfully");
            }
            catch (ProcessEngineException)
            {
                // Exception is to be expected
            }
        }
        [Test]
        [Deployment]
        public virtual void testCandidateUsersExtension()
        {
            runtimeService.StartProcessInstanceByKey("candidateUsersExtension");
            var tasks = taskService.GetTaskManager().FindTasksByCandidateUser("kermit");
            Assert.AreEqual(1, tasks.Count);
            tasks = taskService.GetTaskManager().FindTasksByCandidateUser("gonzo");
            Assert.AreEqual(1, tasks.Count);
        }

        [Test]
        [Deployment]
        public virtual void testCandidateGroupsExtension()
        {
            runtimeService.StartProcessInstanceByKey("candidateGroupsExtension");
            var taskManager = taskService.GetTaskManager();
            // Bugfix check: potentially the query could return 2 tasks since
            // kermit is a member of the two candidate groups
            var tasks = taskManager.FindTasksByCandidateUser("kermit");
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("make profit", tasks[0].Name);

            tasks = taskManager.FindTasksByCandidateUser("fozzie");
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("make profit", tasks[0].Name);

            // Test the task query find-by-candidate-group operation
            Assert.AreEqual(1, taskManager.FindTasksByCandidateGroup("management").Count());
            Assert.AreEqual(1, taskManager.FindTasksByCandidateGroup("accountancy").Count());
        }

        // Test where the candidate IUser extension is used together
        // with the spec way of defining candidate users
        [Test]
        [Deployment]
        public virtual void testMixedCandidateUserDefinition()
        {
            runtimeService.StartProcessInstanceByKey("mixedCandidateUser");

            var taskManager = taskService.GetTaskManager();
            var tasks = taskManager.FindTasksByCandidateUser("kermit");
            Assert.AreEqual(1, tasks.Count);

             tasks = taskManager.FindTasksByCandidateUser("fozzie");
            Assert.AreEqual(1, tasks.Count);

             tasks = taskManager.FindTasksByCandidateUser("gonzo");
            Assert.AreEqual(1, tasks.Count);

             tasks = taskManager.FindTasksByCandidateUser("mispiggy");
            Assert.AreEqual(0, tasks.Count);
        }

    }

}