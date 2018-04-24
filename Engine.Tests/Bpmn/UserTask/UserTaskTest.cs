using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class UserTaskTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void setUp()
        {
            identityService.SaveUser(identityService.NewUser("fozzie"));
            identityService.SaveUser(identityService.NewUser("kermit"));

            identityService.SaveGroup(identityService.NewGroup("accountancy"));
            identityService.SaveGroup(identityService.NewGroup("management"));

            identityService.CreateMembership("fozzie", "accountancy");
            identityService.CreateMembership("kermit", "management");
        }

        [TearDown]
        public virtual void tearDown()
        {
            identityService.DeleteUser("fozzie");
            identityService.DeleteUser("kermit");
            identityService.DeleteGroup("accountancy");
            identityService.DeleteGroup("management");
        }

        [Test]
        [Deployment]
        public virtual void testCompleteAfterParallelGateway()
        {

            // start the process
            runtimeService.StartProcessInstanceByKey("ForkProcess");
            IList<ITask> taskList = taskService.CreateTaskQuery()
                .ToList();
            Assert.NotNull(taskList);
            Assert.AreEqual(2, taskList.Count);

            // make sure IUser task exists
            var task = taskService.CreateTaskQuery(c => c.TaskDefinitionKey == "SimpleUser")
                .First();
            Assert.NotNull(task);

            // attempt to complete the task and get PersistenceException pointing to "referential integrity constraint violation"
            taskService.Complete(task.Id);
        }


        [Test]
        [Deployment]
        public virtual void testComplexScenarioWithSubprocessesAndParallelGateways()
        {
            runtimeService.StartProcessInstanceByKey("processWithSubProcessesAndParallelGateways");

            var taskList = taskService.CreateTaskQuery()
                .ToList();
            Assert.NotNull(taskList);
            Assert.AreEqual(13, taskList.Count);
        }

        [Test]
        [Deployment]
        public virtual void testQuerySortingWithParameter()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            Assert.AreEqual(1, taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id)
                .Count());
        }


        [Test]
        [Deployment]
        public virtual void testSimpleProcess()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("financialReport");

            var taskManager = taskService.GetTaskManager();
            var tasks = taskManager.FindTasksByCandidateUser("fozzie");
            Assert.AreEqual(1, tasks.Count);
            var task = tasks[0];
            Assert.AreEqual("Write monthly financial report", task.Name);

            taskService.Claim(task.Id, "fozzie");
            var taskss = taskService.CreateTaskQuery(c => c.AssigneeWithoutCascade == "fozzie")
                .ToList();

            Assert.AreEqual(1, taskss.Count);
            taskService.Complete(task.Id);

            tasks = taskManager.FindTasksByCandidateUser("fozzie").ToList();
            Assert.AreEqual(0, tasks.Count);
            tasks = taskManager.FindTasksByCandidateUser("kermit").ToList();
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Verify monthly financial report", tasks[0]
                .Name);
            taskService.Complete(tasks[0]
                .Id);

            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testTaskPropertiesNotNull()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var activeActivityIds = runtimeService.GetActiveActivityIds(processInstance.Id);

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task.Id);
            Assert.AreEqual("my task", task.Name);
            Assert.AreEqual("Very important", task.Description);
            Assert.True(task.Priority > 0);
            Assert.AreEqual("kermit", task.Assignee);
            Assert.AreEqual(processInstance.Id, task.ProcessInstanceId);
            Assert.AreEqual(processInstance.Id, task.ExecutionId);
            Assert.NotNull(task.ProcessDefinitionId);
            Assert.NotNull(task.TaskDefinitionKey);
            Assert.NotNull(task.CreateTime);

            // the next test verifies that if an execution creates a task, that no events are created during creation of the task.
            if (processEngineConfiguration.HistoryLevel.Id >=
                HistoryLevelFields.HistoryLevelActivity.Id) //HISTORYLEVEL_ACTIVITY)
                Assert.AreEqual(0, taskService.GetTaskEvents(task.Id)
                    .Count);

        }



        [Test]
        [Deployment]
        public virtual void testTaskJztWf()
        {
            identityService.AuthenticatedUserId = "bono";
            
            IDictionary<string, ITypedValue> varMap = new Dictionary<string, ITypedValue>();
            var domain = new DomainClass()
            {
                Details = new List<DomainDetail>()
                {
                    new DomainDetail(){UserId="fozzie"},
                    new DomainDetail(){UserId="zf"}
                }
            };

            varMap.Add("domain", new ObjectValueImpl(domain));
            varMap.Add("userapi", new ObjectValueImpl(new UserApi()));

            var processInstance = runtimeService.StartProcessInstanceByKey("wf", varMap);

            var task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == processInstance.Id)
                .First();
            Assert.NotNull(task.Id);
            Assert.AreEqual("上级审批", task.Name);
            Assert.AreEqual("kermit", task.Assignee);

            taskService.Complete(task.Id);

            
            var tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();

            Assert.AreEqual(2, tasks.Count());
            foreach (var t in tasks)
            {
                Assert.AreEqual("单据明细审批", t.Name);
                Assert.That(t.Assignee,Is.EqualTo("zf").Or.EqualTo("fozzie"));
            }

            taskService.Complete(tasks[0].Id);
            taskService.Complete(tasks[1].Id);


            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();
            Assert.AreEqual(1, tasks.Count());

            Assert.AreEqual("委托", tasks[0].Name);
            Assert.AreEqual("zf", tasks[0].Assignee);
            taskService.DelegateTask(tasks[0].Id, "zz");

            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();
            Assert.AreEqual(1, tasks.Count());

            Assert.AreEqual("委托", tasks[0].Name);
            Assert.AreEqual("zz", tasks[0].Assignee);
            Assert.AreEqual("zf", tasks[0].Owner);
            taskService.ResolveTask(tasks[0].Id);
            taskService.Complete(tasks[0].Id,new Dictionary<string, object>() { { "approved", true } } );

            tasks = taskService.CreateTaskQuery(c => c.ProcessInstanceId == processInstance.Id).ToList();
            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual("制单人审批", tasks[0].Name);
            Assert.AreEqual("bono", tasks[0].Assignee);
            taskService.Complete(tasks[0].Id);

            AssertProcessEnded(processInstance.Id);
            identityService.AuthenticatedUserId = null;
        }
    }
    [System.Serializable]
    public class DomainClass
    {
        public List<DomainDetail> Details { get; set; }
    }

    [System.Serializable]
    public class DomainDetail
    {
        public string UserId { get; set; }
    }
    
    public interface IUserApi
    {
        string GetUpUser();
    }

    [System.Serializable]
    public class UserApi
    {
        public string GetUpUser(string startor,string test)
        {
            return "kermit";
        }
    }
}