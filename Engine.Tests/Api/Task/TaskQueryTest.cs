//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Common;
//using ESS.FW.Bpm.Engine.exception;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using ESS.FW.Bpm.Engine.Tests.Api.Runtime;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Bpm.Engine.Variable.Value;
//using ESS.FW.Bpm.Model.Bpmn;
//using NUnit.Framework;
//using PluggableProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.PluggableProcessEngineTestCase;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Task
//{


//    /// <summary>
//    /// 
//    /// 
//    /// 
//    /// </summary>
//    public class TaskQueryTest : PluggableProcessEngineTestCase
//    {

//        private IList<string> taskIds;

//        // The range of Oracle's NUMBER field is limited to ~10e+125
//        // which is below Double.MAX_VALUE, so we only test with the following
//        // max value
//        protected internal const double MAX_DOUBLE_VALUE = 10E+124;

//        [SetUp]
//        public void setUp()
//        {

//            identityService.SaveUser(identityService.NewUser("kermit"));
//            identityService.SaveUser(identityService.NewUser("gonzo"));
//            identityService.SaveUser(identityService.NewUser("fozzie"));

//            identityService.SaveGroup(identityService.NewGroup("management"));
//            identityService.SaveGroup(identityService.NewGroup("accountancy"));

//            identityService.CreateMembership("kermit", "management");
//            identityService.CreateMembership("kermit", "accountancy");
//            identityService.CreateMembership("fozzie", "management");

//            taskIds = generateTestTasks();
//        }

//        [TearDown]
//        public void tearDown()
//        {
//            identityService.DeleteGroup("accountancy");
//            identityService.DeleteGroup("management");
//            identityService.DeleteUser("fozzie");
//            identityService.DeleteUser("gonzo");
//            identityService.DeleteUser("kermit");
//            taskService.DeleteTasks(taskIds, true);
//        }

//        [Test]
//        public virtual void tesBasicTaskPropertiesNotNull()
//        {
//            ITask task = taskService.CreateTaskQuery(c=>c.Id == taskIds[0]).First();
//            Assert.NotNull(task.Description);
//            Assert.NotNull(task.Id);
//            Assert.NotNull(task.Name);
//            Assert.NotNull(task.CreateTime);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryNoCriteria()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            Assert.AreEqual(12, query.Count());
//            Assert.AreEqual(12, query.Count());
//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByTaskId()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Id == taskIds[0]);
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidTaskId()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Id == "invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                taskService.CreateTaskQuery(c=>c.Id == null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByName()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="testTask");
//            Assert.AreEqual(6, query.Count());
//            Assert.AreEqual(6, query.Count());

//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidName()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                taskService.CreateTaskQuery(c=>c.Name ==null).First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNameLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskNameLike("gonzo\\_%");
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidNameLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Name =="1");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                taskService.CreateTaskQuery(c=>c.Name ==null).First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByDescription()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDescription("testTask description");
//            Assert.AreEqual(6, query.Count());
//            Assert.AreEqual(6, query.Count());

//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidDescription()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDescription("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                taskService.CreateTaskQuery().TaskDescription(null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {

//            }
//        }



//        [Test]
//        [Deployment]
//        public virtual void testTaskQueryLookupByNameCaseInsensitive()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            query.TaskName("testTask");


//            IList<ITask> tasks = query.ToList();
//            Assert.NotNull(tasks);
//            Assert.That(tasks.Count, Is.EqualTo(6));

//            query = taskService.CreateTaskQuery();
//            query.TaskName("TeStTaSk");

//            tasks = query.ToList();
//            Assert.NotNull(tasks);
//            Assert.That(tasks.Count, Is.EqualTo(6));
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskQueryLookupByNameLikeCaseInsensitive()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            query.TaskNameLike("%task%");


//            IList<ITask> tasks = query.ToList();
//            Assert.NotNull(tasks);
//            Assert.That(tasks.Count, Is.EqualTo(10));

//            query = taskService.CreateTaskQuery();
//            query.TaskNameLike("%Task%");

//            tasks = query.ToList();
//            Assert.NotNull(tasks);
//            Assert.That(tasks.Count, Is.EqualTo(10));
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByDescriptionLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDescriptionLike("%gonzo\\_%");
//            Assert.NotNull(query.First());
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidDescriptionLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDescriptionLike("invalid");
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            try
//            {
//                taskService.CreateTaskQuery().TaskDescriptionLike(null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {

//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByPriority()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Priority==10);
//            Assert.AreEqual(2, query.Count());
//            Assert.AreEqual(2, query.Count());

//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//            query = taskService.CreateTaskQuery(c=>c.Priority==100);
//            Assert.IsNull(query.First());
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            query = taskService.CreateTaskQuery().TaskMinPriority(50);
//            Assert.AreEqual(3, query.Count());

//            query = taskService.CreateTaskQuery().TaskMinPriority(10);
//            Assert.AreEqual(5, query.Count());

//            query = taskService.CreateTaskQuery().TaskMaxPriority(10);
//            Assert.AreEqual(9, query.Count());

//            query = taskService.CreateTaskQuery().TaskMaxPriority(3);
//            Assert.AreEqual(6, query.Count());

//            query = taskService.CreateTaskQuery().TaskMinPriority(50).TaskMaxPriority(10);
//            Assert.AreEqual(0, query.Count());

//            query = taskService.CreateTaskQuery(c=>c.Priority==30).TaskMaxPriority(10);
//            Assert.AreEqual(0, query.Count());

//            query = taskService.CreateTaskQuery().TaskMinPriority(30).TaskPriority(10);
//            Assert.AreEqual(0, query.Count());

//            query = taskService.CreateTaskQuery().TaskMinPriority(30).TaskPriority(20).TaskMaxPriority(10);
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidPriority()
//        {
//            try
//            {
//                taskService.CreateTaskQuery(c=>c.Priority==null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByAssignee()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery(c=>c.Assignee == "gonzo_");
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//            Assert.NotNull(query.First());

//            query = taskService.CreateTaskQuery(c=>c.Assignee == "kermit");
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//            Assert.IsNull(query.First());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByAssigneeLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskAssigneeLike("gonz%\\_");
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//            Assert.NotNull(query.First());

//            query = taskService.CreateTaskQuery(c=>c.Assignee == "gonz");
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//            Assert.IsNull(query.First());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullAssignee()
//        {
//            try
//            {
//                taskService.CreateTaskQuery(c=>c.Assignee == null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByUnassigned()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskUnassigned();
//            Assert.AreEqual(10, query.Count());
//            Assert.AreEqual(10, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByAssigned()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskAssigned();
//            Assert.AreEqual(2, query.Count());
//            Assert.AreEqual(2, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByCandidateUser()
//        {
//            // kermit is candidate for 12 tasks, two of them are already assigned
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCandidateUser("kermit");
//            Assert.AreEqual(10, query.Count());
//            Assert.AreEqual(10, query.Count());
//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateUser("kermit").IncludeAssignedTasks();
//            Assert.AreEqual(12, query.Count());
//            Assert.AreEqual(12, query.Count());

//            // fozzie is candidate for one task and her groups are candidate for 2 tasks, one of them is already assigned
//            query = taskService.CreateTaskQuery().TaskCandidateUser("fozzie");
//            Assert.AreEqual(2, query.Count());
//            Assert.AreEqual(2, query.Count());
//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateUser("fozzie").IncludeAssignedTasks();
//            Assert.AreEqual(3, query.Count());
//            Assert.AreEqual(3, query.Count());

//            // gonzo is candidate for one task, which is already assinged
//            query = taskService.CreateTaskQuery().TaskCandidateUser("gonzo");
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateUser("gonzo").IncludeAssignedTasks();
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCandidateUser()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().TaskCandidateUser(null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByIncludeAssignedTasksWithMissingCandidateUserOrGroup()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().IncludeAssignedTasks();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByCandidateGroup()
//        {
//            // management group is candidate for 3 tasks, one of them is already assigned
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCandidateGroup("management");
//            Assert.AreEqual(2, query.Count());
//            Assert.AreEqual(2, query.Count());
//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroup("management").IncludeAssignedTasks();
//            Assert.AreEqual(3, query.Count());
//            Assert.AreEqual(3, query.Count());


//            // accountancy group is candidate for 3 tasks, one of them is already assigned
//            query = taskService.CreateTaskQuery().TaskCandidateGroup("accountancy");
//            Assert.AreEqual(2, query.Count());
//            Assert.AreEqual(2, query.Count());

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroup("accountancy").IncludeAssignedTasks();
//            Assert.AreEqual(3, query.Count());
//            Assert.AreEqual(3, query.Count());

//            // sales group is candidate for no tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroup("sales");
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroup("sales").IncludeAssignedTasks();
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryWithCandidateGroups()
//        {
//            // test withCandidateGroups
//            IQueryable<ITask> query = taskService.CreateTaskQuery().WithCandidateGroups();
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query.Count());

//            Assert.AreEqual(5, query.IncludeAssignedTasks().Count());
//            Assert.AreEqual(5, query.IncludeAssignedTasks().Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryWithoutCandidateGroups()
//        {
//            // test withoutCandidateGroups
//            IQueryable<ITask> query = taskService.CreateTaskQuery().WithoutCandidateGroups();
//            Assert.AreEqual(6, query.Count());
//            Assert.AreEqual(6, query.Count());

//            Assert.AreEqual(7, query.IncludeAssignedTasks().Count());
//            Assert.AreEqual(7, query.IncludeAssignedTasks().Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCandidateGroup()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().TaskCandidateGroup(null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByCandidateGroupIn()
//        {
//            IList<string> groups = new List<string>() { "management", "accountancy" };
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCandidateGroupIn(groups);
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query.Count());
//            try
//            {
//                query.First();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroupIn(groups).IncludeAssignedTasks();
//            Assert.AreEqual(5, query.Count());
//            Assert.AreEqual(5, query.Count());

//            // Unexisting groups or groups that don't have candidate tasks shouldn't influence other results
//            groups = new List<string>() { "management", "accountancy", "sales", "unexising" };
//            query = taskService.CreateTaskQuery().TaskCandidateGroupIn(groups);
//            Assert.AreEqual(4, query.Count());
//            Assert.AreEqual(4, query.Count());

//            // test including assigned tasks
//            query = taskService.CreateTaskQuery().TaskCandidateGroupIn(groups).IncludeAssignedTasks();
//            Assert.AreEqual(5, query.Count());
//            Assert.AreEqual(5, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCandidateGroupIn()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().TaskCandidateGroupIn(null).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskCandidateGroupIn(new List<string>()).ToList();
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByDelegationState()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDelegationState(0);
//            Assert.AreEqual(12, query.Count());
//            Assert.AreEqual(12, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Pending);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Resolved);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            string taskId = taskService.CreateTaskQuery(c=>c.Assignee == "gonzo_").First().Id;
//            taskService.DelegateTask(taskId, "kermit");

//            query = taskService.CreateTaskQuery().TaskDelegationState(0);
//            Assert.AreEqual(11, query.Count());
//            Assert.AreEqual(11, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Pending);
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Resolved);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());

//            taskService.ResolveTask(taskId);

//            query = taskService.CreateTaskQuery().TaskDelegationState(0);
//            Assert.AreEqual(11, query.Count());
//            Assert.AreEqual(11, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Pending);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//            query = taskService.CreateTaskQuery().TaskDelegationState(DelegationState.Resolved);
//            Assert.AreEqual(1, query.Count());
//            Assert.AreEqual(1, query.Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryCreatedOn()
//        {

//            // Exact matching of createTime, should result in 6 tasks
//            DateTime createTime = DateTime.Parse("2001/01/01 01:01:01.000");

//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCreatedOn(createTime);
//            Assert.AreEqual(6, query.Count());
//            Assert.AreEqual(6, query.Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryCreatedBefore()
//        {

//            // Should result in 7 tasks
//            DateTime before = DateTime.Parse("2002/02/03 02:02:02.000");

//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCreatedBefore(before);
//            Assert.AreEqual(7, query.Count());
//            Assert.AreEqual(7, query.Count());

//            before = DateTime.Parse("2001/01/01 01:01:01.000");
//            query = taskService.CreateTaskQuery().TaskCreatedBefore(before);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryCreatedAfter()
//        {
//            // Should result in 3 tasks
//            DateTime after = DateTime.Parse("2003/03/03 03:03:03.000");

//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCreatedAfter(after);
//            Assert.AreEqual(3, query.Count());
//            Assert.AreEqual(3, query.Count());

//            after = DateTime.Parse("2005/05/05 05:05:05.000");
//            query = taskService.CreateTaskQuery().TaskCreatedAfter(after);
//            Assert.AreEqual(0, query.Count());
//            Assert.AreEqual(0, query.Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testCreateTimeCombinations()
//        {
//            // Exact matching of createTime, should result in 6 tasks
//            DateTime createTime = DateTime.Parse("2001/01/01 01:01:01.000");

//            DateTime oneHourAgo = new DateTime(createTime.Ticks - 60 * 60 * 1000);
//            DateTime oneHourLater = new DateTime(createTime.Ticks + 60 * 60 * 1000);

//            Assert.AreEqual(6, taskService.CreateTaskQuery().TaskCreatedAfter(oneHourAgo).TaskCreatedOn(createTime).TaskCreatedBefore(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskCreatedAfter(oneHourLater).TaskCreatedOn(createTime).TaskCreatedBefore(oneHourAgo).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskCreatedAfter(oneHourLater).TaskCreatedOn(createTime).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskCreatedOn(createTime).TaskCreatedBefore(oneHourAgo).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDefinitionKey()
//        {

//            // Start process instance, 2 tasks will be available
//            runtimeService.StartProcessInstanceByKey("taskDefinitionKeyProcess");

//            // 1 task should exist with key "taskKey_1"
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="taskKey_1").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("taskKey_1", tasks[0].TaskDefinitionKey);

//            // No task should be found with unexisting key
//            long? Count = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="unexistingKey").Count();
//            Assert.AreEqual(0L, Count.Value);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDefinitionKeyLike()
//        {

//            // Start process instance, 2 tasks will be available
//            runtimeService.StartProcessInstanceByKey("taskDefinitionKeyProcess");

//            // Ends with matching, TaskKey_1 and TaskKey_123 match
//            IList<ITask> tasks = taskService.CreateTaskQuery().TaskDefinitionKeyLike("taskKey\\_1%")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(2, tasks.Count);

//            Assert.AreEqual("taskKey_1", tasks[0].TaskDefinitionKey);
//            Assert.AreEqual("taskKey_123", tasks[1].TaskDefinitionKey);

//            // Starts with matching, TaskKey_123 matches
//            tasks = taskService.CreateTaskQuery().TaskDefinitionKeyLike("%\\_123")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("taskKey_123", tasks[0].TaskDefinitionKey);

//            // Contains matching, TaskKey_123 matches
//            tasks = taskService.CreateTaskQuery().TaskDefinitionKeyLike("%Key\\_12%")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("taskKey_123", tasks[0].TaskDefinitionKey);


//            // No task should be found with unexisting key
//            long? Count = taskService.CreateTaskQuery().TaskDefinitionKeyLike("%unexistingKey%").Count();
//            Assert.AreEqual(0L, Count.Value);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDefinitionKeyIn()
//        {

//            // Start process instance, 2 tasks will be available
//            runtimeService.StartProcessInstanceByKey("taskDefinitionKeyProcess");

//            // 1 ITask should be found with TaskKey1
//            IList<ITask> tasks = taskService.CreateTaskQuery().TaskDefinitionKeyIn("taskKey_1").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("taskKey_1", tasks[0].TaskDefinitionKey);

//            // 2 Tasks should be found with TaskKey_1 and TaskKey_123
//            tasks = taskService.CreateTaskQuery().TaskDefinitionKeyIn("taskKey_1", "taskKey_123")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(2, tasks.Count);

//            Assert.AreEqual("taskKey_1", tasks[0].TaskDefinitionKey);
//            Assert.AreEqual("taskKey_123", tasks[1].TaskDefinitionKey);

//            // 2 Tasks should be found with TaskKey1, TaskKey123 and UnexistingKey
//            tasks = taskService.CreateTaskQuery().TaskDefinitionKeyIn("taskKey_1", "taskKey_123", "unexistingKey")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(2, tasks.Count);

//            Assert.AreEqual("taskKey_1", tasks[0].TaskDefinitionKey);
//            Assert.AreEqual("taskKey_123", tasks[1].TaskDefinitionKey);

//            // No task should be found with UnexistingKey
//            long? Count = taskService.CreateTaskQuery().TaskDefinitionKeyIn("unexistingKey").Count();
//            Assert.AreEqual(0L, Count.Value);

//            Count = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey =="unexistingKey").TaskDefinitionKeyIn("taskKey1").Count();
//            Assert.AreEqual(0l, Count.Value);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskVariableValueEquals()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // No task should be found for an unexisting var
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("unexistingVar", "value").Count());

//            // Create a map with a variable for all default types
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["longVar"] = 928374L;
//            variables["shortVar"] = (short)123;
//            variables["integerVar"] = 1234;
//            variables["stringVar"] = "stringValue";
//            variables["booleanVar"] = true;
//            DateTime date = new DateTime();
//            variables["dateVar"] = date;
//            variables["nullVar"] = null;

//            taskService.SetVariablesLocal(task.Id, variables);

//            // Test query matches
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("longVar", 928374L).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("shortVar", (short)123).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("integerVar", 1234).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("stringVar", "stringValue").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("booleanVar", true).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("dateVar", date).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("nullVar", null).Count());

//            // Test query for other values on existing variables
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("longVar", 999L).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("shortVar", (short)999).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("integerVar", 999).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("stringVar", "999").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("booleanVar", false).Count());
//            DateTime otherDate = new DateTime();
//            otherDate.AddYears(1);
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("dateVar", otherDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("nullVar", "999").Count());

//            // Test query for not equals
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueNotEquals("longVar", 999L).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueNotEquals("shortVar", (short)999).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueNotEquals("integerVar", 999).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueNotEquals("stringVar", "999").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueNotEquals("booleanVar", false).Count());

//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskVariableValueLike()
//        {

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "stringValue";

//            taskService.SetVariablesLocal(task.Id, variables);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "stringVal%").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "%ngValue").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "%ngVal%").Count());

//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "stringVar%").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "%ngVar").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "%ngVar%").Count());

//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", "stringVal").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLike("nonExistingVar", "string%").Count());

//            // test with null value
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueLike("stringVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskVariableValueCompare()
//        {

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["numericVar"] = 928374;
//            DateTime date = (new GregorianCalendar()).ToDateTime(2014, 2, 2, 2, 2, 2, 0);
//            variables["dateVar"] = date;
//            variables["stringVar"] = "ab";
//            variables["nullVar"] = null;

//            taskService.SetVariablesLocal(task.Id, variables);

//            // test compare methods with numeric values
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("numericVar", 928373).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("numericVar", 928375).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("numericVar", 928373).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("numericVar", 928375).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThan("numericVar", 928375).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("numericVar", 928373).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("numericVar", 928375).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("numericVar", 928373).Count());

//            // test compare methods with date values
//            DateTime before = (new GregorianCalendar()).ToDateTime(2014, 2, 2, 2, 2, 1, 0);
//            DateTime after = (new GregorianCalendar()).ToDateTime(2014, 2, 2, 2, 2, 1, 0);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("dateVar", before).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("dateVar", after).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("dateVar", before).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("dateVar", after).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThan("dateVar", after).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("dateVar", before).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("dateVar", after).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("dateVar", before).Count());

//            //test with string values
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("stringVar", "aa").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThan("stringVar", "ba").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("stringVar", "aa").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("stringVar", "ba").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThan("stringVar", "ba").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThan("stringVar", "aa").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("stringVar", "ba").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("stringVar", "aa").Count());

//            // test with null value
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueGreaterThan("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }

//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueLessThan("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }

//            // test with boolean value
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueGreaterThan("nullVar", true).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueGreaterThanOrEquals("nullVar", false).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueLessThan("nullVar", true).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("nullVar", false).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }

//            // test non existing variable
//            Assert.AreEqual(0, taskService.CreateTaskQuery().TaskVariableValueLessThanOrEquals("nonExisting", 123).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessVariableValueEquals()
//        {
//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["longVar"] = 928374L;
//            variables["shortVar"] = (short)123;
//            variables["integerVar"] = 1234;
//            variables["stringVar"] = "stringValue";
//            variables["booleanVar"] = true;
//            DateTime date = new DateTime();
//            variables["dateVar"] = date;
//            variables["nullVar"] = null;

//            // Start process-instance with all types of variables
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // Test query matches
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("longVar", 928374L).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("shortVar", (short)123).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("integerVar", 1234).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("stringVar", "stringValue").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("booleanVar", true).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("dateVar", date).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("nullVar", null).Count());

//            // Test query for other values on existing variables
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("longVar", 999L).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("shortVar", (short)999).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("integerVar", 999).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("stringVar", "999").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("booleanVar", false).Count());
//            DateTime otherDate = new DateTime();
//            otherDate.AddYears(1);
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("dateVar", otherDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("nullVar", "999").Count());

//            // Test querying for task variables don't match the process-variables
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("longVar", 928374L).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("shortVar", (short)123).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("integerVar", 1234).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("stringVar", "stringValue").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("booleanVar", true).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.TaskVariableValueEquals("nullVar", null).Count());

//            // Test querying for task variables not equals
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("longVar", 999L).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("shortVar", (short)999).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("integerVar", 999).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("stringVar", "999").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("booleanVar", false).Count());

//            // and query for the existing variable with NOT shoudl result in nothing found:
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("longVar", 928374L).Count());

//            // Test combination of task-variable and process-variable
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
//            taskService.SetVariableLocal(task.Id, "taskVar", "theValue");
//            taskService.SetVariableLocal(task.Id, "longVar", 928374L);

//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("longVar", 928374L)//.TaskVariableValueEquals("taskVar", "theValue").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("longVar", 928374L)//.TaskVariableValueEquals("longVar", 928374L).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessVariableValueLike()
//        {

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["stringVar"] = "stringValue";
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "stringVal%").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "%ngValue").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "%ngVal%").Count());

//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "stringVar%").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "%ngVar").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "%ngVar%").Count());

//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", "stringVal").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLike("nonExistingVar", "string%").Count());

//            // test with null value
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueLike("stringVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessVariableValueCompare()
//        {

//            IDictionary<string, object> variables = new Dictionary<string, object>();
//            variables["numericVar"] = 928374;
//            DateTime date = new GregorianCalendar().ToDateTime(2014, 2, 2, 2, 2, 2, 0);
//            variables["dateVar"] = date;
//            variables["stringVar"] = "ab";
//            variables["nullVar"] = null;

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", variables);

//            // test compare methods with numeric values
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("numericVar", 928373).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("numericVar", 928375).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("numericVar", 928373).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("numericVar", 928375).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThan("numericVar", 928375).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("numericVar", 928373).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("numericVar", 928375).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("numericVar", 928374).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("numericVar", 928373).Count());

//            // test compare methods with date values
//            DateTime before = (new GregorianCalendar()).ToDateTime(2014, 2, 2, 2, 2, 1, 0);
//            DateTime after = (new GregorianCalendar()).ToDateTime(2014, 2, 2, 2, 2, 3, 0);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("dateVar", before).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("dateVar", after).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("dateVar", before).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("dateVar", after).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThan("dateVar", after).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("dateVar", before).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("dateVar", after).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("dateVar", date).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("dateVar", before).Count());

//            //test with string values
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("stringVar", "aa").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("stringVar", "ba").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("stringVar", "aa").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("stringVar", "ba").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThan("stringVar", "ba").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("stringVar", "aa").Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("stringVar", "ba").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("stringVar", "ab").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("stringVar", "aa").Count());

//            // test with null value
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueLessThan("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("nullVar", null).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }

//            // test with boolean value
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("nullVar", true).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("nullVar", false).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueLessThan("nullVar", true).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }
//            try
//            {
//                taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("nullVar", false).Count();
//                Assert.Fail("expected exception");
//            }
//            //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//            //ORIGINAL LINE: catch (final org.Camunda.bpm.Engine.ProcessEngineException e)
//            catch (ProcessEngineException)
//            {
//            }

//            // test non existing variable
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("nonExisting", 123).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessVariableValueEqualsNumber()
//        {
//            // long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123L));

//            // non-matching long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 12345L));

//            // short
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", (short)123));

//            // double
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123.0d));

//            // integer
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123));

//            // untyped null (should not match)
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", null));

//            // typed null (should not match)
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", Variable.Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "123"));

//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(123L)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue((short)123)).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(0)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessVariableValueNumberComparison()
//        {
//            // long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123L));

//            // non-matching long
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 12345L));

//            // short
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", (short)123));

//            // double
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123.0d));

//            // integer
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 123));

//            // untyped null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", null));

//            // typed null
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", Variable.Variables.LongValue(null)));

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "123"));

//            Assert.AreEqual(4, taskService.CreateTaskQuery().ProcessVariableValueNotEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessVariableValueGreaterThan("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(5, taskService.CreateTaskQuery().ProcessVariableValueGreaterThanOrEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessVariableValueLessThan("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery().ProcessVariableValueLessThanOrEquals("var", Variable.Variables.NumberValue(123)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskVariableValueEqualsNumber()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").ToList();
//            Assert.AreEqual(8, tasks.Count);
//            taskService.SetVariableLocal(tasks[0].Id, "var", 123L);
//            taskService.SetVariableLocal(tasks[1].Id, "var", 12345L);
//            taskService.SetVariableLocal(tasks[2].Id, "var", (short)123);
//            taskService.SetVariableLocal(tasks[3].Id, "var", 123.0d);
//            taskService.SetVariableLocal(tasks[4].Id, "var", 123);
//            taskService.SetVariableLocal(tasks[5].Id, "var", null);
//            taskService.SetVariableLocal(tasks[6].Id, "var", Variable.Variables.LongValue(null));
//            taskService.SetVariableLocal(tasks[7].Id, "var", "123");

//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.TaskVariableValueEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.TaskVariableValueEquals("var", Variable.Variables.NumberValue(123L)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.TaskVariableValueEquals("var", Variable.Variables.NumberValue(123)).Count());
//            Assert.AreEqual(4, taskService.CreateTaskQuery()//.TaskVariableValueEquals("var", Variable.Variables.NumberValue((short)123)).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.TaskVariableValueEquals("var", Variable.Variables.NumberValue(0)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testVariableEqualsNumberMax()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", MAX_DOUBLE_VALUE));
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", long.MaxValue));

//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.DoubleValue(MAX_DOUBLE_VALUE)).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(long.MaxValue)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testVariableEqualsNumberLongValueOverflow()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", MAX_DOUBLE_VALUE));

//            // this results in an overflow
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", (double)MAX_DOUBLE_VALUE));

//            // the query should not find the long variable
//            Assert.AreEqual(1, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.DoubleValue(MAX_DOUBLE_VALUE)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testVariableEqualsNumberNonIntegerDoubleShouldNotMatchInteger()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", 42).PutValue("var2", 52.4d));

//            // querying by 42.4 should not match the integer variable 42
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.DoubleValue(42.4d)).Count());

//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 42.4d));

//            // querying by 52 should not find the double variable 52.4
//            Assert.AreEqual(0, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", Variable.Variables.NumberValue(52)).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessDefinitionId()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionId ==processInstance.ProcessDefinitionId).ToList();
//            Assert.AreEqual(1, tasks.Count);
//            Assert.AreEqual(processInstance.Id, tasks[0].ProcessInstanceId);

//            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.ProcessDefinitionId =="unexisting").Count());
//        }



//        [Test]
//        [Deployment]
//        public virtual void testProcessDefinitionKey()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").ToList();
//            Assert.AreEqual(1, tasks.Count);
//            Assert.AreEqual(processInstance.Id, tasks[0].ProcessInstanceId);

//            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="unexisting").Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessDefinitionKeyIn()
//        {

//            // Start for each deployed process definition a process instance
//            runtimeService.StartProcessInstanceByKey("taskDefinitionKeyProcess");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // 1 task should be found with oneTaskProcess
//            IList<ITask> tasks = taskService.CreateTaskQuery().ProcessDefinitionKeyIn("oneTaskProcess").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("theTask", tasks[0].TaskDefinitionKey);

//            // 2 Tasks should be found with both process definition keys
//            tasks = taskService.CreateTaskQuery().ProcessDefinitionKeyIn("oneTaskProcess", "taskDefinitionKeyProcess").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(3, tasks.Count);

//            ISet<string> keysFound = new HashSet<string>();
//            foreach (ITask task in tasks)
//            {
//                keysFound.Add(task.TaskDefinitionKey);
//            }
//            Assert.True(keysFound.Contains("taskKey_123"));
//            Assert.True(keysFound.Contains("theTask"));
//            Assert.True(keysFound.Contains("taskKey_1"));

//            // 1 Tasks should be found with oneTaskProcess,and NonExistingKey
//            tasks = taskService.CreateTaskQuery().ProcessDefinitionKeyIn("oneTaskProcess", "NonExistingKey")/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("theTask", tasks[0].TaskDefinitionKey);

//            // No task should be found with NonExistingKey
//            long? Count = taskService.CreateTaskQuery().ProcessDefinitionKeyIn("NonExistingKey").Count();
//            Assert.AreEqual(0L, Count.Value);

//            Count = taskService.CreateTaskQuery().ProcessDefinitionKeyIn("oneTaskProcess").ProcessDefinitionKey("NonExistingKey").Count();
//            Assert.AreEqual(0L, Count.Value);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessDefinitionName()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            IList<ITask> tasks = taskService.CreateTaskQuery().ProcessDefinitionName("The%One%Task%Process").ToList();
//            Assert.AreEqual(1, tasks.Count);
//            Assert.AreEqual(processInstance.Id, tasks[0].ProcessInstanceId);

//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessDefinitionName("unexisting").Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessDefinitionNameLike()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            IList<ITask> tasks = taskService.CreateTaskQuery().ProcessDefinitionNameLike("The\\%One\\%Task%").ToList();
//            Assert.AreEqual(1, tasks.Count);
//            Assert.AreEqual(processInstance.Id, tasks[0].ProcessInstanceId);

//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessDefinitionNameLike("The One Task").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessDefinitionNameLike("The Other Task%").Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessInstanceBusinessKey()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessDefinitionName("The%One%Task%Process").ProcessInstanceBusinessKey("BUSINESS-KEY-1").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessInstanceBusinessKey("BUSINESS-KEY-1").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessInstanceBusinessKey("NON-EXISTING").Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessInstanceBusinessKeyIn()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-2");

//            // 1 task should be found with BUSINESS-KEY-1
//            IList<ITask> tasks = taskService.CreateTaskQuery().ProcessInstanceBusinessKeyIn("BUSINESS-KEY-1").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(1, tasks.Count);

//            Assert.AreEqual("theTask", tasks[0].TaskDefinitionKey);

//            // 2 tasks should be found with BUSINESS-KEY-1 and BUSINESS-KEY-2
//            tasks = taskService.CreateTaskQuery().ProcessInstanceBusinessKeyIn("BUSINESS-KEY-1", "BUSINESS-KEY-2").ToList();
//            Assert.NotNull(tasks);
//            Assert.AreEqual(2, tasks.Count);

//            foreach (ITask item in tasks)
//            {
//                Assert.AreEqual("theTask", item.TaskDefinitionKey);
//            }

//            // 1 tasks should be found with BUSINESS-KEY-1 and NON-EXISTING-KEY
//            ITask task = taskService.CreateTaskQuery().ProcessInstanceBusinessKeyIn("BUSINESS-KEY-1", "NON-EXISTING-KEY").First();

//            Assert.NotNull(tasks);
//            Assert.AreEqual("theTask", task.TaskDefinitionKey);

//            long Count = taskService.CreateTaskQuery().ProcessInstanceBusinessKeyIn("BUSINESS-KEY-1").ProcessInstanceBusinessKey("NON-EXISTING-KEY").Count();
//            Assert.AreEqual(0l, Count);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testProcessInstanceBusinessKeyLike()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess", "BUSINESS-KEY-1");

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessDefinitionName("The%One%Task%Process").ProcessInstanceBusinessKey("BUSINESS-KEY-1").Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().ProcessInstanceBusinessKeyLike("BUSINESS-KEY%").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessInstanceBusinessKeyLike("BUSINESS-KEY").Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().ProcessInstanceBusinessKeyLike("BUZINESS-KEY%").Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDueDate()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // Set due-date on task
//            DateTime dueDate = DateTime.Parse("2003/02/01 01:12:13");
//            task.DueDate = dueDate;
//            taskService.SaveTask(task);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueDate(dueDate).Count());

//            DateTime otherDate = new DateTime();
//            otherDate.AddYears(1);
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueDate(otherDate).Count());

//            DateTime priorDate = new DateTime();
//            priorDate = new DateTime(dueDate.Millisecond);
//            priorDate = priorDate.AddYears(-1);
//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueAfter(priorDate).Count());

//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueBefore(otherDate).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDueBefore()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // Set due-date on task
//            DateTime dueDateCal = new DateTime();
//            task.DueDate = dueDateCal;
//            taskService.SaveTask(task);

//            DateTime oneHourAgo = new DateTime();
//            oneHourAgo = new DateTime(dueDateCal.Millisecond);
//            oneHourAgo = oneHourAgo.AddHours(-1);

//            DateTime oneHourLater = new DateTime();
//            oneHourLater = new DateTime(dueDateCal.Millisecond);
//            oneHourLater = oneHourLater.AddHours(1);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueBefore(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueBefore(oneHourAgo).Count());

//            // Update due-date to null, shouldn't show up anymore in query that matched before
//            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
//            task.DueDate = DateTime.MinValue;
//            taskService.SaveTask(task);

//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueBefore(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueBefore(oneHourAgo).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testTaskDueAfter()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // Set due-date on task
//            DateTime dueDateCal = new DateTime();
//            task.DueDate = dueDateCal;
//            taskService.SaveTask(task);

//            DateTime oneHourAgo = new DateTime();
//            oneHourAgo = new DateTime(dueDateCal.Millisecond);
//            oneHourAgo = oneHourAgo.AddHours(-1);

//            DateTime oneHourLater = new DateTime();
//            oneHourLater = new DateTime(dueDateCal.Millisecond);
//            oneHourLater = oneHourLater.AddHours(1);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueAfter(oneHourAgo).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueAfter(oneHourLater).Count());

//            // Update due-date to null, shouldn't show up anymore in query that matched before
//            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();
//            task.DueDate = DateTime.Now;
//            taskService.SaveTask(task);

//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueAfter(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueAfter(oneHourAgo).Count());
//        }


//        public virtual void testTaskDueDateCombinations()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // Set due-date on task
//            DateTime dueDate = DateTime.Parse("2003/02/01 01:12:13");
//            task.DueDate = dueDate;
//            taskService.SaveTask(task);

//            DateTime oneHourAgo = new DateTime(dueDate.Ticks - 60 * 60 * 1000);
//            DateTime oneHourLater = new DateTime(dueDate.Ticks + 60 * 60 * 1000);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().DueAfter(oneHourAgo).DueDate(dueDate).DueBefore(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueAfter(oneHourLater).DueDate(dueDate).DueBefore(oneHourAgo).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueAfter(oneHourLater).DueDate(dueDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().DueDate(dueDate).DueBefore(oneHourAgo).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testFollowUpDate()
//        {
//            DateTime otherDate = new DateTime();

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // do not find any task instances with follow up date
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpDate(otherDate).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).FollowUpBeforeOrNotExistent(otherDate).Count());
//            // we might have tasks from other test cases - so we limit to the current PI

//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // set follow-up date on task
//            DateTime followUpDate = DateTime.Parse("2003/02/01 01:12:13");
//            task.FollowUpDate = followUpDate;
//            taskService.SaveTask(task);

//            Assert.AreEqual(followUpDate, taskService.CreateTaskQuery(c=>c.Id == task.Id).First().FollowUpDate);
//            Assert.AreEqual(1, taskService.CreateTaskQuery().FollowUpDate(followUpDate).Count());

//            otherDate = new DateTime(followUpDate.Millisecond);

//            otherDate.AddYears(1);
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpDate(otherDate).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery().FollowUpBefore(otherDate).Count());
//            Assert.AreEqual(1, taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).FollowUpBeforeOrNotExistent(otherDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpAfter(otherDate).Count());

//            otherDate.AddYears(-2);
//            Assert.AreEqual(1, taskService.CreateTaskQuery().FollowUpAfter(otherDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpBefore(otherDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).FollowUpBeforeOrNotExistent(otherDate).Count());

//            taskService.Complete(task.Id);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testFollowUpDateCombinations()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            // Set follow-up date on task
//            DateTime dueDate = DateTime.Parse("2003/02/01 01:12:13");
//            task.FollowUpDate = dueDate;
//            taskService.SaveTask(task);

//            DateTime oneHourAgo = new DateTime(dueDate.Ticks - 60 * 60 * 1000);
//            DateTime oneHourLater = new DateTime(dueDate.Ticks + 60 * 60 * 1000);

//            Assert.AreEqual(1, taskService.CreateTaskQuery().FollowUpAfter(oneHourAgo).FollowUpDate(dueDate).FollowUpBefore(oneHourLater).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpAfter(oneHourLater).FollowUpDate(dueDate).FollowUpBefore(oneHourAgo).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpAfter(oneHourLater).FollowUpDate(dueDate).Count());
//            Assert.AreEqual(0, taskService.CreateTaskQuery().FollowUpDate(dueDate).FollowUpBefore(oneHourAgo).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByActivityInstanceId()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            string activityInstanceId = runtimeService.GetActivityInstance(processInstance.Id).ChildActivityInstances[0].Id;

//            Assert.AreEqual(1, taskService.CreateTaskQuery().ActivityInstanceIdIn(activityInstanceId).Count());
//        }


//        public virtual void testQueryByMultipleActivityInstanceIds()
//        {
//            IProcessInstance processInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            string activityInstanceId1 = runtimeService.GetActivityInstance(processInstance1.Id).ChildActivityInstances[0].Id;

//            IProcessInstance processInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            string activityInstanceId2 = runtimeService.GetActivityInstance(processInstance2.Id).ChildActivityInstances[0].Id;

//            IList<ITask> result1 = taskService.CreateTaskQuery().ActivityInstanceIdIn(activityInstanceId1).ToList();
//            Assert.AreEqual(1, result1.Count);
//            Assert.AreEqual(processInstance1.Id, result1[0].ProcessInstanceId);

//            IList<ITask> result2 = taskService.CreateTaskQuery().ActivityInstanceIdIn(activityInstanceId2).ToList();
//            Assert.AreEqual(1, result2.Count);
//            Assert.AreEqual(processInstance2.Id, result2[0].ProcessInstanceId);

//            Assert.AreEqual(2, taskService.CreateTaskQuery().ActivityInstanceIdIn(activityInstanceId1, activityInstanceId2).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByInvalidActivityInstanceId()
//        {
//            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            Assert.AreEqual(0, taskService.CreateTaskQuery().ActivityInstanceIdIn("anInvalidActivityInstanceId").Count());
//        }

//        public virtual void testQueryPaging()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskCandidateUser("kermit");

//            Assert.AreEqual(10, query.ListPage(0, int.MaxValue).Count());

//            // Verifying the un-paged results
//            Assert.AreEqual(10, query.Count());
//            Assert.AreEqual(10, query.Count());

//            // Verifying paged results
//            Assert.AreEqual(2, query/*.ListPage(0, 2)*/.Count());
//            Assert.AreEqual(2, query.ListPage(2, 2).Count());
//            Assert.AreEqual(3, query.ListPage(4, 3).Count());
//            Assert.AreEqual(1, query.ListPage(9, 3).Count());
//            Assert.AreEqual(1, query.ListPage(9, 1).Count());

//            // Verifying odd usages
//            Assert.AreEqual(0, query.ListPage(-1, -1).Count());
//            Assert.AreEqual(0, query.ListPage(10, 2).Count()); // 9 is the last index with a result
//            Assert.AreEqual(10, query.ListPage(0, 15).Count()); // there are only 10 tasks
//        }

//        public virtual void testQuerySorting()
//        {
//            // default ordering is by id
//            int expectedCount = 12;
//            //API RUNTIME 屏蔽
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskId(), expectedCount, TestOrderingUtil.taskById());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskId()/*.Asc()*/, expectedCount, TestOrderingUtil.taskById());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/, expectedCount, TestOrderingUtil.taskByName());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskPriority()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByPriority());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskAssignee()*//*.Asc()*/, expectedCount, TestOrderingUtil.taskByAssignee());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskDescription()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByDescription());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByProcessInstanceId()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByProcessInstanceId());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByExecutionId()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByExecutionId());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskCreateTime()*//*.Asc()*/, expectedCount, TestOrderingUtil.taskByCreateTime());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery().OrderByDueDate()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByDueDate());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery().OrderByFollowUpDate()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByFollowUpDate());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByCaseInstanceId()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByCaseInstanceId());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByCaseExecutionId()/*.Asc()*/, expectedCount, TestOrderingUtil.taskByCaseExecutionId());

//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskId()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskById()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByName()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskPriority()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByPriority()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskAssignee()*//*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByAssignee()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskDescription()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByDescription()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByProcessInstanceId()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByProcessInstanceId()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByExecutionId()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByExecutionId()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()/*.OrderByTaskCreateTime()*//*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByCreateTime()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery().OrderByDueDate()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByDueDate()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery().OrderByFollowUpDate()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByFollowUpDate()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByCaseInstanceId()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByCaseInstanceId()));
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByCaseExecutionId()/*.Desc()*/, expectedCount, TestOrderingUtil.inverted(TestOrderingUtil.taskByCaseExecutionId()));

//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskId().TaskName("testTask")/*.Asc()*/, 6, TestOrderingUtil.taskById());
//            //TestOrderingUtil.VerifySortingAndCount(taskService.CreateTaskQuery()//.OrderByTaskId().TaskName("testTask")/*.Desc()*/, 6, TestOrderingUtil.inverted(TestOrderingUtil.taskById()));

//        }

//        public virtual void testQuerySortingByNameShouldBeCaseInsensitive()
//        {
//            // create task with capitalized name
//            ITask task = taskService.NewTask("caseSensitiveTestTask");
//            task.Name = "CaseSensitiveTestTask";
//            taskService.SaveTask(task);

//            // create task filter
//            IFilter filter = filterService.NewTaskFilter("taskNameOrdering");
//            filterService.SaveFilter(filter);

//            IList<string> sortedNames = getTaskNamesFromTasks(taskService.CreateTaskQuery().ToList());
//            sortedNames = sortedNames.OrderBy(o => o).ToList();

//            // ascending ordering
//            IQueryable<ITask> taskQuery = taskService.CreateTaskQuery().OrderByTaskNameCaseInsensitive()/*.Asc()*/;
//            IList<string> ascNames = getTaskNamesFromTasks(taskQuery.ToList());
//            Assert.AreEqual(sortedNames, ascNames);

//            // test filter merging
//            ascNames = getTaskNamesFromTasks(filterService.List<ITask>(filter.Id));//,taskQuery
//            Assert.AreEqual(sortedNames, ascNames);

//            // descending ordering

//            // reverse sorted names to test descending ordering
//            sortedNames.Reverse();

//            taskQuery = taskService.CreateTaskQuery().OrderByTaskNameCaseInsensitive()/*.Desc()*/;
//            IList<string> descNames = getTaskNamesFromTasks(taskQuery.ToList());
//            Assert.AreEqual(sortedNames, descNames);

//            // test filter merging
//            descNames = getTaskNamesFromTasks(filterService.List<ITask>(filter.Id));//taskQuery
//            Assert.AreEqual(sortedNames, descNames);

//            // Delete test task
//            taskService.DeleteTask(task.Id, true);

//            // Delete filter
//            filterService.DeleteFilter(filter.Id);
//        }

//        public virtual void testQueryOrderByTaskName()
//        {

//            // asc
//            IList<ITask> tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Asc()*/.ToList();
//            Assert.AreEqual(12, tasks.Count);


//            IList<string> taskNames = getTaskNamesFromTasks(tasks);
//            Assert.AreEqual("accountancy description", taskNames[0]);
//            Assert.AreEqual("accountancy description", taskNames[1]);
//            Assert.AreEqual("gonzo_Task", taskNames[2]);
//            Assert.AreEqual("managementAndAccountancyTask", taskNames[3]);
//            Assert.AreEqual("managementTask", taskNames[4]);
//            Assert.AreEqual("managementTask", taskNames[5]);
//            Assert.AreEqual("testTask", taskNames[6]);
//            Assert.AreEqual("testTask", taskNames[7]);
//            Assert.AreEqual("testTask", taskNames[8]);
//            Assert.AreEqual("testTask", taskNames[9]);
//            Assert.AreEqual("testTask", taskNames[10]);
//            Assert.AreEqual("testTask", taskNames[11]);

//            // Desc
//            tasks = taskService.CreateTaskQuery()/*.OrderByTaskName()*//*.Desc()*/.ToList();
//            Assert.AreEqual(12, tasks.Count);

//            taskNames = getTaskNamesFromTasks(tasks);
//            Assert.AreEqual("testTask", taskNames[0]);
//            Assert.AreEqual("testTask", taskNames[1]);
//            Assert.AreEqual("testTask", taskNames[2]);
//            Assert.AreEqual("testTask", taskNames[3]);
//            Assert.AreEqual("testTask", taskNames[4]);
//            Assert.AreEqual("testTask", taskNames[5]);
//            Assert.AreEqual("managementTask", taskNames[6]);
//            Assert.AreEqual("managementTask", taskNames[7]);
//            Assert.AreEqual("managementAndAccountancyTask", taskNames[8]);
//            Assert.AreEqual("gonzo_Task", taskNames[9]);
//            Assert.AreEqual("accountancy description", taskNames[10]);
//            Assert.AreEqual("accountancy description", taskNames[11]);
//        }

//        public virtual IList<string> getTaskNamesFromTasks(IList<ITask> tasks)
//        {
//            IList<string> names = new List<string>();
//            foreach (ITask task in tasks)
//            {
//                names.Add(task.Name);
//            }
//            return names;
//        }

//        public virtual void testNativeQuery()
//        {
//            string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//            Assert.AreEqual(tablePrefix + "ACT_RU_TASK", managementService.GetTableName(typeof(ITask)));
//            Assert.AreEqual(tablePrefix + "ACT_RU_TASK", managementService.GetTableName(typeof(TaskEntity)));
//            Assert.AreEqual(12, taskService.CreateNativeTaskQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(ITask))).Count());
//            Assert.AreEqual(12, taskService.CreateNativeTaskQuery().Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(ITask))).Count());

//            Assert.AreEqual(144, taskService.CreateNativeTaskQuery().Sql("SELECT Count(*) FROM " + tablePrefix + "ACT_RU_TASK T1, " + tablePrefix + "ACT_RU_TASK T2").Count());

//            // join task and variable instances
//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery().Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(ITask)) + " T1, " + managementService.GetTableName(typeof(VariableInstanceEntity)) + " V1 WHERE V1.TASK_ID_ = T1.ID_").Count());
//            IList<ITask> tasks = taskService.CreateNativeTaskQuery().Sql("SELECT T1.* FROM " + managementService.GetTableName(typeof(ITask)) + " T1, " + managementService.GetTableName(typeof(VariableInstanceEntity)) + " V1 WHERE V1.TASK_ID_ = T1.ID_").ToList();
//            Assert.AreEqual(1, tasks.Count);
//            Assert.AreEqual("gonzo_Task", tasks[0].Name);

//            // select with distinct
//            Assert.AreEqual(12, taskService.CreateNativeTaskQuery().Sql("SELECT DISTINCT T1.* FROM " + tablePrefix + "ACT_RU_TASK T1").Count());

//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery().Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(ITask)) + " T WHERE T.NAME_ = 'gonzo_Task'").Count());
//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(ITask)) + " T WHERE T.NAME_ = 'gonzo_Task'").Count());

//            // use parameters
//            Assert.AreEqual(1, taskService.CreateNativeTaskQuery().Sql("SELECT Count(*) FROM " + managementService.GetTableName(typeof(ITask)) + " T WHERE T.NAME_ = #{taskName}").Parameter("taskName", "gonzo_Task").Count());
//        }

//        public virtual void testNativeQueryPaging()
//        {
//            string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
//            Assert.AreEqual(tablePrefix + "ACT_RU_TASK", managementService.GetTableName(typeof(ITask)));
//            Assert.AreEqual(tablePrefix + "ACT_RU_TASK", managementService.GetTableName(typeof(TaskEntity)));
//            Assert.AreEqual(5, taskService.CreateNativeTaskQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(ITask))).ListPage(0, 5).Count());
//            Assert.AreEqual(2, taskService.CreateNativeTaskQuery().Sql("SELECT * FROM " + managementService.GetTableName(typeof(ITask))).ListPage(10, 12).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseDefinitionId()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionId(caseDefinitionId);

//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseDefinitionId()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionId(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseDefinitionKey()
//        {
//            string CaseDefinitionKey = repositoryService.CreateCaseDefinitionQuery().First().Key;

//            caseService.WithCaseDefinitionByKey(CaseDefinitionKey).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionKey(CaseDefinitionKey);

//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseDefinitionKey()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionKey("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionKey(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseDefinitionName()
//        {
//            ICaseDefinition caseDefinition = repositoryService.CreateCaseDefinitionQuery().First();

//            string caseDefinitionId = caseDefinition.Id;
//            string caseDefinitionName = caseDefinition.Name;

//            caseService.WithCaseDefinition(caseDefinitionId).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionName(caseDefinitionName);

//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseDefinitionName()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionName("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionName(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseDefinitionNameLike()
//        {
//            IList<string> caseDefinitionIds = CaseDefinitionIds;

//            foreach (string caseDefinitionId in caseDefinitionIds)
//            {
//                caseService.WithCaseDefinition(caseDefinitionId).Create();
//            }
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionNameLike("One T%");
//            //verifyQueryResults(query, 1);

//            query.CaseDefinitionNameLike("%ITask Case");
//            //verifyQueryResults(query, 1);

//            query.CaseDefinitionNameLike("%Task%");
//            //verifyQueryResults(query, 1);

//            query.CaseDefinitionNameLike("%z\\_");
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseDefinitionNameLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseDefinitionNameLike("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseDefinitionNameLike(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseInstanceId()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            string caseInstanceId = caseService.WithCaseDefinition(caseDefinitionId).Create().Id;

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceId(caseInstanceId);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseInstanceIdHierarchy()
//        {
//            // given
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string processTaskId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_ProcessTask_1").First().Id;

//            // then

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceId(caseInstanceId);

//            //verifyQueryResults(query, 2);

//            foreach (ITask task in query.ToList())
//            {
//                Assert.AreEqual(caseInstanceId, task.CaseInstanceId);
//                taskService.Complete(task.Id);
//            }

//            //verifyQueryResults(query, 1);
//            Assert.AreEqual(caseInstanceId, query.First().CaseInstanceId);

//            taskService.Complete(query.First().Id);

//            //verifyQueryResults(query, 0);
//        }

//        public virtual void testQueryByInvalidCaseInstanceId()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseInstanceId(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseInstanceBusinessKey()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            string businessKey = "aBusinessKey";

//            caseService.WithCaseDefinition(caseDefinitionId).BusinessKey(businessKey).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceBusinessKey(businessKey);

//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseInstanceBusinessKey()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceBusinessKey("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseInstanceBusinessKey(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseInstanceBusinessKeyLike()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            string businessKey = "aBusiness_Key";

//            caseService.WithCaseDefinition(caseDefinitionId).BusinessKey(businessKey).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceBusinessKeyLike("aBus%");
//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKeyLike("%siness\\_Key");
//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKeyLike("%sines%");
//            //verifyQueryResults(query, 1);

//            query.CaseInstanceBusinessKeyLike("%sines%");
//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseInstanceBusinessKeyLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceBusinessKeyLike("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseInstanceBusinessKeyLike(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByCaseExecutionId()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).Create();

//            string humanTaskExecutionId = startDefaultCaseExecutionManually();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseExecutionId(humanTaskExecutionId);

//            //verifyQueryResults(query, 1);
//        }

//        public virtual void testQueryByInvalidCaseExecutionId()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseExecutionId("invalid");

//            //verifyQueryResults(query, 0);

//            try
//            {
//                query.CaseExecutionId(null);
//                Assert.Fail("expected exception");
//            }
//            catch (ProcessEngineException)
//            {
//                // OK
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aNullValue", null);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aBooleanValue", true);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aShortValue", (short)123);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aLongValue", (long)789);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aDateValue", now);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueEquals("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueEquals("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableValueEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueEquals("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueEquals()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueEquals(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }


//        [Test]
//        [Deployment]
//        protected internal virtual void startDefaultCaseWithVariable(object variableValue, string VariableName)
//        {
//            string caseDefinitionId = CaseDefinitionId;
//            createCaseWithVariable(caseDefinitionId, variableValue, VariableName);
//        }

//        /// <returns> the case definition id if only one case is deployed. </returns>
//        protected internal virtual string CaseDefinitionId
//        {
//            get
//            {
//                string caseDefinitionId = repositoryService.CreateCaseDefinitionQuery().First().Id;
//                return caseDefinitionId;
//            }
//        }

//        /// <returns> the case definition ids </returns>
//        protected internal virtual IList<string> CaseDefinitionIds
//        {
//            get
//            {
//                IList<string> caseDefinitionIds = new List<string>();
//                IList<ICaseDefinition> caseDefinitions = repositoryService.CreateCaseDefinitionQuery().ToList();
//                foreach (ICaseDefinition caseDefinition in caseDefinitions)
//                {
//                    caseDefinitionIds.Add(caseDefinition.Id);
//                }
//                return caseDefinitionIds;
//            }
//        }

//        protected internal virtual void createCaseWithVariable(string caseDefinitionId, object variableValue, string VariableName)
//        {
//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable(VariableName, variableValue).Create();
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aStringValue", "abd");

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aBooleanValue", false);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aShortValue", (short)124);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aLongValue", (long)790);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            DateTime before = new DateTime(now.Ticks - 100000);

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aDateValue", before);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueNotEquals("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueNotEquals()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            try
//            {
//                query.CaseInstanceVariableValueNotEquals(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }

//        /// <summary>
//        /// @return
//        /// </summary>
//        protected internal virtual IFileValue createDefaultFileValue()
//        {
//            //IFileValue fileValue = Variable.Variables.FileValue("tst.Txt").File(File.ReadAllBytes("somebytes".GetBytes())).Create();
//            //return fileValue;
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Starts the case execution for oneTaskCase.cmmn<para>
//        /// Only works for testcases, which deploy that process.
//        /// 
//        /// </para>
//        /// </summary>
//        /// <returns> the execution id for the activity PI_HumanTask_1 </returns>
//        protected internal virtual string startDefaultCaseExecutionManually()
//        {
//            string humanTaskExecutionId = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").First().Id;

//            caseService.WithCaseExecution(humanTaskExecutionId).ManualStart();
//            return humanTaskExecutionId;
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueNotEquals("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueNotEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueNotEquals("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aNullValue", null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThan("aStringValue", "ab");

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aBooleanValue", false).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThan("aShortValue", (short)122);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThan("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThan("aLongValue", (long)788);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            DateTime before = new DateTime(now.Ticks - 100000);

//            query.CaseInstanceVariableValueGreaterThan("aDateValue", before);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThan("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableGreaterThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueGreaterThan()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            startDefaultCaseExecutionManually();
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThan(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEquals("aNullValue", null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aStringValue", "ab");

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEquals("aBooleanValue", false).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aShortValue", (short)122);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aShortValue", (short)123);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueGreaterThanOrEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("anIntegerValue", 455);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aLongValue", (long)788);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aLongValue", (long)789);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            DateTime before = new DateTime(now.Ticks - 100000);

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aDateValue", before);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aDateValue", now);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aDoubleValue", 1.4);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueGreaterThanOrEquals("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEquals("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableGreaterThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEquals("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueGreaterThanOrEqual()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueGreaterThanOrEquals(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aNullValue", null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThan("aStringValue", "abd");

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aBooleanValue", false).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThan("aShortValue", (short)124);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThan("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThan("aLongValue", (long)790);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            DateTime after = new DateTime(now.Ticks + 100000);

//            query.CaseInstanceVariableValueLessThan("aDateValue", after);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThan("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableLessThan()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThan("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueLessThan()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            try
//            {
//                query.CaseInstanceVariableValueLessThan(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEquals("aNullValue", null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aStringValue", "abd");

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aStringValue", "abc");

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByBooleanCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aBooleanValue", true).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEquals("aBooleanValue", false).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByShortCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aShortValue", (short)123).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aShortValue", (short)124);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aShortValue", (short)123);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByIntegerCaseInstanceVariableValueLessThanOrEquals()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("anIntegerValue", 456).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("anIntegerValue", 457);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("anIntegerValue", 456);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByLongCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aLongValue", (long)789).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aLongValue", (long)790);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aLongValue", (long)789);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDateCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            DateTime now = DateTime.Now;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDateValue", now).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            DateTime after = new DateTime(now.Ticks + 100000);

//            query.CaseInstanceVariableValueLessThanOrEquals("aDateValue", after);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aDateValue", now);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByDoubleCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aDoubleValue", 1.5).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aDoubleValue", 1.6);

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLessThanOrEquals("aDoubleValue", 1.5);

//            //verifyQueryResults(query, 1);

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByByteArrayCaseInstanceVariableValueLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            byte[] bytes = "somebytes".GetBytes();

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aByteArrayValue", bytes).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEquals("aByteArrayValue", bytes).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryBySerializableCaseInstanceVariableLessThanOrEqual()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            IList<string> serializable = new List<string>();
//            serializable.Add("one");
//            serializable.Add("two");
//            serializable.Add("three");

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aSerializableValue", serializable).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEquals("aSerializableValue", serializable).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByFileCaseInstanceVariableValueLessThanOrEqual()
//        {
//            IFileValue fileValue = createDefaultFileValue();
//            string VariableName = "aFileValue";

//            startDefaultCaseWithVariable(fileValue, VariableName);
//            IQueryable<ITask> query = taskService.CreateTaskQuery();
//            try
//            {
//                query.CaseInstanceVariableValueLessThanOrEquals(VariableName, fileValue).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException e)
//            {
//                Assert.That(e.Message, Does.Contain("Variables of type File cannot be used to query"));
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByNullCaseInstanceVariableValueLike()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aNullValue", null).Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            try
//            {
//                query.CaseInstanceVariableValueLike("aNullValue", null).ToList();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }

//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByStringCaseInstanceVariableValueLike()
//        {
//            string caseDefinitionId = CaseDefinitionId;

//            caseService.WithCaseDefinition(caseDefinitionId).SetVariable("aStringValue", "abc").Create();

//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "ab%");

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "%bc");

//            //verifyQueryResults(query, 1);

//            query = taskService.CreateTaskQuery();

//            query.CaseInstanceVariableValueLike("aStringValue", "%b%");

//            //verifyQueryResults(query, 1);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryByVariableInParallelBranch()
//        {
//            runtimeService.StartProcessInstanceByKey("parallelGateway");

//            // when there are two process variables of the same name but different types
//            IExecution task1Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task1").First();
//            runtimeService.SetVariableLocal(task1Execution.Id, "var", 12345L);
//            IExecution task2Execution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task2").First();
//            runtimeService.SetVariableLocal(task2Execution.Id, "var", 12345);

//            // then the task query should be able to filter by both variables and return both tasks
//            Assert.AreEqual(2, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", 12345).Count());
//            Assert.AreEqual(2, taskService.CreateTaskQuery()//.ProcessVariableValueEquals("var", 12345L).Count());
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByProcessVariables()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "bValue"));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "cValue"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "aValue"));

//            // when I make a task query with ascending variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(3, tasks.Count);
//            // then in alphabetical order
//            Assert.AreEqual(instance3.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance1.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance2.Id, tasks[2].ProcessInstanceId);

//            // when I make a task query with descending variable ordering by String values
//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Desc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(3, tasks.Count);
//            // then in alphabetical order
//            Assert.AreEqual(instance2.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance1.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance3.Id, tasks[2].ProcessInstanceId);


//            // when I make a task query with variable ordering by Integer values
//            IList<ITask> unorderedTasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            // then the tasks are in no particular ordering
//            Assert.AreEqual(3, unorderedTasks.Count);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByExecutionVariables()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("parallelGateway", Collections.SingletonMap<string, object>("var", "aValue"));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("parallelGateway", Collections.SingletonMap<string, object>("var", "bValue"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("parallelGateway", Collections.SingletonMap<string, object>("var", "cValue"));

//            // and some local variables on the tasks
//            ITask task1 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance1.Id).First();
//            runtimeService.SetVariableLocal(task1.ExecutionId, "var", "cValue");

//            ITask task2 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance2.Id).First();
//            runtimeService.SetVariableLocal(task2.ExecutionId, "var", "bValue");

//            ITask task3 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance3.Id).First();
//            runtimeService.SetVariableLocal(task3.ExecutionId, "var", "aValue");

//            // when I make a task query with ascending variable ordering by tasks variables
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="parallelGateway").OrderByExecutionVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly by their local variables
//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance3.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance2.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance1.Id, tasks[2].ProcessInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByTaskVariables()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "aValue"));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "bValue"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "cValue"));

//            // and some local variables on the tasks
//            ITask task1 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance1.Id).First();
//            taskService.SetVariableLocal(task1.Id, "var", "cValue");

//            ITask task2 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance2.Id).First();
//            taskService.SetVariableLocal(task2.Id, "var", "bValue");

//            ITask task3 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance3.Id).First();
//            taskService.SetVariableLocal(task3.Id, "var", "aValue");

//            // when I make a task query with ascending variable ordering by tasks variables
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByTaskVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly by their local variables
//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance3.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance2.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance1.Id, tasks[2].ProcessInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByCaseInstanceVariables()
//        {
//            // given three tasks with String case instance variables
//            ICaseInstance instance1 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "cValue"));
//            ICaseInstance instance2 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "aValue"));
//            ICaseInstance instance3 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "bValue"));

//            // when I make a task query with ascending variable ordering by tasks variables
//            IList<ITask> tasks = taskService.CreateTaskQuery().CaseDefinitionKey("oneTaskCase").OrderByCaseInstanceVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly by their local variables
//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance2.Id, tasks[0].CaseInstanceId);
//            Assert.AreEqual(instance3.Id, tasks[1].CaseInstanceId);
//            Assert.AreEqual(instance1.Id, tasks[2].CaseInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByCaseExecutionVariables()
//        {
//            // given three tasks with String case instance variables
//            ICaseInstance instance1 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "cValue"));
//            ICaseInstance instance2 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "aValue"));
//            ICaseInstance instance3 = caseService.CreateCaseInstanceByKey("oneTaskCase", Collections.SingletonMap<string, object>("var", "bValue"));

//            // and local case execution variables
//            ICaseExecution caseExecution1 = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").CaseInstanceId(instance1.Id).First();

//            caseService.WithCaseExecution(caseExecution1.Id).SetVariableLocal("var", "aValue").ManualStart();

//            ICaseExecution caseExecution2 = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").CaseInstanceId(instance2.Id).First();

//            caseService.WithCaseExecution(caseExecution2.Id).SetVariableLocal("var", "bValue").ManualStart();

//            ICaseExecution caseExecution3 = caseService.CreateCaseExecutionQuery(c=>c.ActivityId == "PI_HumanTask_1").CaseInstanceId(instance3.Id).First();

//            caseService.WithCaseExecution(caseExecution3.Id).SetVariableLocal("var", "cValue").ManualStart();

//            // when I make a task query with ascending variable ordering by tasks variables
//            IList<ITask> tasks = taskService.CreateTaskQuery().CaseDefinitionKey("oneTaskCase").OrderByCaseExecutionVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly by their local variables
//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance1.Id, tasks[0].CaseInstanceId);
//            Assert.AreEqual(instance2.Id, tasks[1].CaseInstanceId);
//            Assert.AreEqual(instance3.Id, tasks[2].CaseInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByVariablesWithNullValues()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "bValue"));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "cValue"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "aValue"));
//            IProcessInstance instance4 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            ITask firstTask = tasks[0];

//            // the null-valued task should be either first or last
//            if (firstTask.ProcessInstanceId.Equals(instance4.Id))
//            {
//                // then the others in ascending order
//                Assert.AreEqual(instance3.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance1.Id, tasks[2].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[3].ProcessInstanceId);
//            }
//            else
//            {
//                Assert.AreEqual(instance3.Id, tasks[0].ProcessInstanceId);
//                Assert.AreEqual(instance1.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[2].ProcessInstanceId);
//                Assert.AreEqual(instance4.Id, tasks[3].ProcessInstanceId);
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByVariablesWithMixedTypes()
//        {
//            // given three tasks with String and Integer process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 42));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "cValue"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "aValue"));

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            ITask firstTask = tasks[0];

//            // the numeric-valued task should be either first or last
//            if (firstTask.ProcessInstanceId.Equals(instance1.Id))
//            {
//                // then the others in ascending order
//                Assert.AreEqual(instance3.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[2].ProcessInstanceId);
//            }
//            else
//            {
//                Assert.AreEqual(instance3.Id, tasks[0].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance1.Id, tasks[2].ProcessInstanceId);
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByStringVariableWithMixedCase()
//        {
//            // given three tasks with String and Integer process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "a"));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "B"));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "c"));

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(3, tasks.Count);
//            // first the numeric valued task (since it is treated like null-valued)
//            Assert.AreEqual(instance1.Id, tasks[0].ProcessInstanceId);
//            // then the others in alphabetical order
//            Assert.AreEqual(instance2.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance3.Id, tasks[2].ProcessInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByVariablesOfAllPrimitiveTypes()
//        {
//            // given three tasks with String and Integer process instance variables
//            IProcessInstance booleanInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", true));
//            IProcessInstance shortInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", (short)16));
//            IProcessInstance longInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 500L));
//            IProcessInstance intInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 400));
//            IProcessInstance stringInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", "300"));
//            IProcessInstance dateInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", new DateTime(1000L)));
//            IProcessInstance doubleInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 42.5d));

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Short)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, booleanInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Short)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, shortInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Long)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, longInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, intInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, stringInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Date)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, dateInstance);

//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Double)/*.Asc()*/.ToList();

//            verifyFirstOrLastTask(tasks, doubleInstance);
//        }

//        public virtual void testQueryByUnsupportedValueTypes()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.Bytes);
//                Assert.Fail("this type is not supported");
//            }
//            catch (ProcessEngineException e)
//            {
//                // happy path
//                AssertTextPresent("Cannot order by variables of type byte", e.Message);
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.Null);
//                Assert.Fail("this type is not supported");
//            }
//            catch (ProcessEngineException e)
//            {
//                // happy path
//                AssertTextPresent("Cannot order by variables of type null", e.Message);
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.Number);
//                Assert.Fail("this type is not supported");
//            }
//            catch (ProcessEngineException e)
//            {
//                // happy path
//                AssertTextPresent("Cannot order by variables of type number", e.Message);
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.Object);
//                Assert.Fail("this type is not supported");
//            }
//            catch (ProcessEngineException e)
//            {
//                // happy path
//                AssertTextPresent("Cannot order by variables of type object", e.Message);
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.File);
//                Assert.Fail("this type is not supported");
//            }
//            catch (ProcessEngineException e)
//            {
//                // happy path
//                AssertTextPresent("Cannot order by variables of type file", e.Message);
//            }
//        }

//        /// <summary>
//        /// verify that either the first or the last task of the list belong to the given process instance
//        /// </summary>
//        protected internal virtual void verifyFirstOrLastTask(IList<ITask> tasks, IProcessInstance belongingProcessInstance)
//        {
//            if (tasks.Count == 0)
//            {
//                Assert.Fail("no tasks given");
//            }

//            int numTasks = tasks.Count;
//            bool matches = tasks[0].ProcessInstanceId.Equals(belongingProcessInstance.Id);
//            matches = matches || tasks[numTasks - 1].ProcessInstanceId.Equals(belongingProcessInstance.Id);

//            Assert.True(matches, "neither first nor last task belong to process instance " + belongingProcessInstance.Id);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByVariablesWithMixedTypesAndSameColumn()
//        {
//            // given three tasks with Integer and Long process instance variables
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 42));
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 800));
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Collections.SingletonMap<string, object>("var", 500L));

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(3, tasks.Count);

//            ITask firstTask = tasks[0];

//            // the Long-valued task should be either first or last
//            if (firstTask.ProcessInstanceId.Equals(instance3.Id))
//            {
//                // then the others in ascending order
//                Assert.AreEqual(instance1.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[2].ProcessInstanceId);
//            }
//            else
//            {
//                Assert.AreEqual(instance1.Id, tasks[0].ProcessInstanceId);
//                Assert.AreEqual(instance2.Id, tasks[1].ProcessInstanceId);
//                Assert.AreEqual(instance3.Id, tasks[2].ProcessInstanceId);
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByTwoVariables()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance bInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "b").PutValue("var2", 14));
//            IProcessInstance bInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "b").PutValue("var2", 30));
//            IProcessInstance cInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "c").PutValue("var2", 50));
//            IProcessInstance cInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "c").PutValue("var2", 30));
//            IProcessInstance aInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "a").PutValue("var2", 14));
//            IProcessInstance aInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "a").PutValue("var2", 50));

//            // when I make a task query with variable primary ordering by var values
//            // and secondary ordering by var2 values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Desc()*/.OrderByProcessVariable("var2", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(6, tasks.Count);
//            // var = c; var2 = 30
//            Assert.AreEqual(cInstance2.Id, tasks[0].ProcessInstanceId);
//            // var = c; var2 = 50
//            Assert.AreEqual(cInstance1.Id, tasks[1].ProcessInstanceId);
//            // var = b; var2 = 14
//            Assert.AreEqual(bInstance1.Id, tasks[2].ProcessInstanceId);
//            // var = b; var2 = 30
//            Assert.AreEqual(bInstance2.Id, tasks[3].ProcessInstanceId);
//            // var = a; var2 = 14
//            Assert.AreEqual(aInstance1.Id, tasks[4].ProcessInstanceId);
//            // var = a; var2 = 50
//            Assert.AreEqual(aInstance2.Id, tasks[5].ProcessInstanceId);

//            // when I make a task query with variable primary ordering by var2 values
//            // and secondary ordering by var values
//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var2", ValueTypeFields.Integer)/*.Desc()*/.OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(6, tasks.Count);
//            // var = a; var2 = 50
//            Assert.AreEqual(aInstance2.Id, tasks[0].ProcessInstanceId);
//            // var = c; var2 = 50
//            Assert.AreEqual(cInstance1.Id, tasks[1].ProcessInstanceId);
//            // var = b; var2 = 30
//            Assert.AreEqual(bInstance2.Id, tasks[2].ProcessInstanceId);
//            // var = c; var2 = 30
//            Assert.AreEqual(cInstance2.Id, tasks[3].ProcessInstanceId);
//            // var = a; var2 = 14
//            Assert.AreEqual(aInstance1.Id, tasks[4].ProcessInstanceId);
//            // var = b; var2 = 14
//            Assert.AreEqual(bInstance1.Id, tasks[5].ProcessInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingByVariablesWithSecondaryOrderingByProcessInstanceId()
//        {
//            // given three tasks with String process instance variables
//            IProcessInstance bInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "b"));
//            IProcessInstance bInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "b"));
//            IProcessInstance cInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "c"));
//            IProcessInstance cInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "c"));
//            IProcessInstance aInstance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "a"));
//            IProcessInstance aInstance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", "a"));

//            // when I make a task query with variable ordering by String values
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.String)/*.Asc()*///.OrderByProcessInstanceId()/*.Asc()*/.ToList();

//            // then the tasks are ordered correctly
//            Assert.AreEqual(6, tasks.Count);

//            // var = a
//            verifyTasksSortedByProcessInstanceId(new List<IProcessInstance>() { aInstance1, aInstance2 }, tasks.Skip(2).Take(2).ToList());

//            // var = b
//            verifyTasksSortedByProcessInstanceId(new List<IProcessInstance>() { bInstance1, bInstance2 }, tasks.Skip(2).Take(4).ToList());

//            // var = c
//            verifyTasksSortedByProcessInstanceId(new List<IProcessInstance>() { cInstance1, cInstance2 }, tasks.Skip(4).Take(2).ToList());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryResultOrderingWithInvalidParameters()
//        {
//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable(null, ValueTypeFields.String)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByProcessVariable("var", ValueTypeFields.Null)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByExecutionVariable(null, ValueTypeFields.String)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByExecutionVariable("var", ValueTypeFields.Null)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByTaskVariable(null, ValueTypeFields.String)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByTaskVariable("var", null)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByCaseInstanceVariable(null, ValueTypeFields.String)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByCaseInstanceVariable("var", null)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByCaseExecutionVariable(null, ValueTypeFields.String)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }

//            try
//            {
//                taskService.CreateTaskQuery().OrderByCaseExecutionVariable("var", null)/*.Asc()*/.ToList();
//                Assert.Fail("should not succeed");
//            }
//            catch (NullValueException)
//            {
//                // happy path
//            }
//        }

//        protected internal virtual void verifyTasksSortedByProcessInstanceId(IList<IProcessInstance> expectedProcessInstances, IList<ITask> actualTasks)
//        {

//            Assert.AreEqual(expectedProcessInstances.Count, actualTasks.Count);
//            IList<IProcessInstance> instances = new List<IProcessInstance>(expectedProcessInstances);

//            // instances.OrderBy(o => new ComparatorAnonymousInnerClass(o));
//            instances = instances.OrderBy(o => o.Id).ToList();

//            for (int i = 0; i < instances.Count; i++)
//            {
//                Assert.AreEqual(instances[i].Id, actualTasks[i].ProcessInstanceId);
//            }
//        }

//        private class ComparatorAnonymousInnerClass : IComparer<IProcessInstance>
//        {
//            private readonly TaskQueryTest outerInstance;

//            public ComparatorAnonymousInnerClass(TaskQueryTest outerInstance)
//            {
//                this.outerInstance = outerInstance;
//            }

//            public virtual int Compare(IProcessInstance p1, IProcessInstance p2)
//            {
//                return p1.Id.CompareTo(p2.Id);
//            }
//        }

//        private void verifyQueryResults(IQueryable<ITask> query, int countExpected)
//        {
//            Assert.AreEqual(countExpected, query.Count());
//            Assert.AreEqual(countExpected, query.Count());

//            if (countExpected == 1)
//            {
//                Assert.NotNull(query.First());
//            }
//            else if (countExpected > 1)
//            {
//                verifySingleResultFails(query);
//            }
//            else if (countExpected == 0)
//            {
//                Assert.IsNull(query.First());
//            }
//        }

//        private void verifySingleResultFails(IQueryable<ITask> query)
//        {
//            try
//            {
//                query.First();
//                Assert.Fail();
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testInitializeFormKeys()
//        {
//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("testProcess");

//            // if initializeFormKeys
//            ITask task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).InitializeFormKeys().First();

//            // then the form key is present
//            Assert.AreEqual("exampleFormKey", task.FormKey);

//            // if NOT initializeFormKeys
//            task = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).First();

//            try
//            {
//                // then the form key is not retrievable
//                // task.FormKey;
//                Assert.AreEqual("exampleFormKey", task.FormKey);
//                Assert.Fail("exception expected.");
//            }
//            catch (BadUserRequestException e)
//            {
//                Assert.AreEqual("ENGINE-03052 The form key is not initialized. You must call initializeFormKeys() on the task query before you can retrieve the form key.", e.Message);
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryOrderByProcessVariableInteger()
//        {
//            IProcessInstance instance500 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", 500));
//            IProcessInstance instance1000 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", 1000));
//            IProcessInstance instance250 = runtimeService.StartProcessInstanceByKey("oneTaskProcess", Variable.Variables.CreateVariables().PutValue("var", 250));

//            // asc
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance250.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance500.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance1000.Id, tasks[2].ProcessInstanceId);

//            // Desc
//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Desc()*/.ToList();

//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(instance1000.Id, tasks[0].ProcessInstanceId);
//            Assert.AreEqual(instance500.Id, tasks[1].ProcessInstanceId);
//            Assert.AreEqual(instance250.Id, tasks[2].ProcessInstanceId);
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryOrderByTaskVariableInteger()
//        {
//            IProcessInstance instance1 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            IProcessInstance instance2 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
//            IProcessInstance instance3 = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

//            ITask task500 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance1.Id).First();
//            taskService.SetVariableLocal(task500.Id, "var", 500);

//            ITask task250 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance2.Id).First();
//            taskService.SetVariableLocal(task250.Id, "var", 250);

//            ITask task1000 = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==instance3.Id).First();
//            taskService.SetVariableLocal(task1000.Id, "var", 1000);

//            // asc
//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByTaskVariable("var", ValueTypeFields.Integer)/*.Asc()*/.ToList();

//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(task250.Id, tasks[0].Id);
//            Assert.AreEqual(task500.Id, tasks[1].Id);
//            Assert.AreEqual(task1000.Id, tasks[2].Id);

//            // Desc
//            tasks = taskService.CreateTaskQuery(c=>c.ProcessDefinitionKey=="oneTaskProcess").OrderByProcessVariable("var", ValueTypeFields.Integer)/*.Desc()*/.ToList();

//            Assert.AreEqual(3, tasks.Count);
//            Assert.AreEqual(task1000.Id, tasks[0].Id);
//            Assert.AreEqual(task500.Id, tasks[1].Id);
//            Assert.AreEqual(task250.Id, tasks[2].Id);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByParentTaskId()
//        {
//            string parentTaskId = "parentTask";
//            ITask parent = taskService.NewTask(parentTaskId);
//            taskService.SaveTask(parent);

//            ITask sub1 = taskService.NewTask("subTask1");
//            sub1.ParentTaskId = parentTaskId;
//            taskService.SaveTask(sub1);

//            ITask sub2 = taskService.NewTask("subTask2");
//            sub2.ParentTaskId = parentTaskId;
//            taskService.SaveTask(sub2);

//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskParentTaskId(parentTaskId);

//            //verifyQueryResults(query, 2);

//            taskService.DeleteTask(parentTaskId, true);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testExtendTaskQueryList_ProcessDefinitionKeyIn()
//        {
//            // given
//            string processDefinitionKey = "invoice";
//            IQueryable<ITask> query = taskService.CreateTaskQuery().ProcessDefinitionKeyIn(processDefinitionKey);

//            IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//            // when
//            var result = ((TaskQueryImpl)query).Extend(extendingQuery.First());

//            // then
//            string[] processDefinitionKeys = ((TaskQueryImpl)result).ProcessDefinitionKeys;
//            Assert.AreEqual(1, processDefinitionKeys.Length);
//            Assert.AreEqual(processDefinitionKey, processDefinitionKeys[0]);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testExtendingTaskQueryList_ProcessDefinitionKeyIn()
//        {
//            // given
//            string processDefinitionKey = "invoice";
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery().ProcessDefinitionKeyIn(processDefinitionKey);

//            // when
//            var result = ((TaskQueryImpl)query).Extend(extendingQuery.First());

//            // then
//            string[] processDefinitionKeys = ((TaskQueryImpl)result).ProcessDefinitionKeys;
//            Assert.AreEqual(1, processDefinitionKeys.Length);
//            Assert.AreEqual(processDefinitionKey, processDefinitionKeys[0]);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testExtendTaskQueryList_TaskDefinitionKeyIn()
//        {
//            // given
//            string TaskDefinitionKey = "assigneApprover";
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskDefinitionKeyIn(TaskDefinitionKey);

//            IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery();

//            // when
//            var result = ((TaskQueryImpl)query).Extend(extendingQuery.First());

//            // then
//            string[] key = ((TaskQueryImpl)result).Keys;
//            Assert.AreEqual(1, key.Length);
//            Assert.AreEqual(TaskDefinitionKey, key[0]);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testExtendingTaskQueryList_TaskDefinitionKeyIn()
//        {
//            // given
//            string TaskDefinitionKey = "assigneApprover";
//            IQueryable<ITask> query = taskService.CreateTaskQuery();

//            IQueryable<ITask> extendingQuery = taskService.CreateTaskQuery().TaskDefinitionKeyIn(TaskDefinitionKey);

//            // when
//            var result = ((TaskQueryImpl)query).Extend(extendingQuery.First());

//            // then
//            string[] key = ((TaskQueryImpl)result).Keys;
//            Assert.AreEqual(1, key.Length);
//            Assert.AreEqual(TaskDefinitionKey, key[0]);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryWithCandidateUsers()
//        {
//            IBpmnModelInstance process = Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().CamundaCandidateUsers("anna").EndEvent().Done();

//            Deployment(process);

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).WithCandidateUsers().ToList();
//            Assert.AreEqual(1, tasks.Count);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryWithoutCandidateUsers()
//        {
//            IBpmnModelInstance process = Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().CamundaCandidateGroups("sales").EndEvent().Done();

//            Deployment(process);

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

//            IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).WithoutCandidateUsers().ToList();
//            Assert.AreEqual(1, tasks.Count);
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryAssignedTasksWithCandidateUsers()
//        {
//            IBpmnModelInstance process = Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().CamundaCandidateGroups("sales").EndEvent().Done();

//            Deployment(process);

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

//            try
//            {
//                taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).IncludeAssignedTasks().WithCandidateUsers().ToList();
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }


//        [Test]
//        [Deployment]
//        public virtual void testQueryAssignedTasksWithoutCandidateUsers()
//        {
//            IBpmnModelInstance process = Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().UserTask().CamundaCandidateGroups("sales").EndEvent().Done();

//            Deployment(process);

//            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

//            try
//            {
//                taskService.CreateTaskQuery(c=>c.ProcessInstanceId ==processInstance.Id).IncludeAssignedTasks().WithoutCandidateUsers().ToList();
//                Assert.Fail("exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//            }
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNameNotEqual()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskNameNotEqual("gonzo_Task");
//            Assert.AreEqual(11, query.Count());
//        }

//        [Test]
//        [Deployment]
//        public virtual void testQueryByNameNotLike()
//        {
//            IQueryable<ITask> query = taskService.CreateTaskQuery().TaskNameNotLike("management%");
//            Assert.AreEqual(9, query.Count());
//            Assert.AreEqual(9, query.Count());

//            query = taskService.CreateTaskQuery().TaskNameNotLike("gonzo\\_%");
//            Assert.AreEqual(11, query.Count());
//            Assert.AreEqual(11, query.Count());
//        }

//        /// <summary>
//        /// Generates some test tasks.
//        /// - 6 tasks where kermit is a candidate
//        /// - 1 tasks where gonzo is assignee and kermit and gonzo are candidates
//        /// - 2 tasks assigned to management group
//        /// - 2 tasks assigned to accountancy group
//        /// - 1 task assigned to fozzie and to both the management and accountancy group
//        /// </summary>
//        private IList<string> generateTestTasks()
//        {
//            IList<string> ids = new List<string>();

//            // 6 tasks for kermit
//            ClockUtil.CurrentTime = DateTime.Parse("2001/01/01 01:01:01.000");
//            for (int i = 0; i < 6; i++)
//            {
//                ITask curTask = taskService.NewTask();
//                curTask.Name = "testTask";
//                curTask.Description = "testTask description";
//                curTask.Priority = 3;
//                taskService.SaveTask(curTask);
//                ids.Add(curTask.Id);
//                taskService.AddCandidateUser(curTask.Id, "kermit");
//            }

//            ClockUtil.CurrentTime = DateTime.Parse("2002/02/02 02:02:02.000");
//            // 1 task for gonzo
//            ITask task = taskService.NewTask();
//            task.Name = "gonzo_Task";
//            task.Description = "gonzo_description";
//            task.Priority = 4;
//            taskService.SaveTask(task);
//            taskService.SetAssignee(task.Id, "gonzo_");
//            taskService.SetVariable(task.Id, "testVar", "someVariable");
//            taskService.AddCandidateUser(task.Id, "kermit");
//            taskService.AddCandidateUser(task.Id, "gonzo");
//            ids.Add(task.Id);

//            ClockUtil.CurrentTime = DateTime.Parse("2003/03/03  03:03:03.000");
//            // 2 tasks for management group
//            for (int i = 0; i < 2; i++)
//            {
//                task = taskService.NewTask();
//                task.Name = "managementTask";
//                task.Priority = 10;
//                taskService.SaveTask(task);
//                taskService.AddCandidateGroup(task.Id, "management");
//                ids.Add(task.Id);
//            }

//            ClockUtil.CurrentTime = DateTime.Parse("2004/04/04  04:04:04.000");
//            // 2 tasks for accountancy group
//            for (int i = 0; i < 2; i++)
//            {
//                task = taskService.NewTask();
//                task.Name = "accountancyTask";
//                task.Name = "accountancy description";
//                taskService.SaveTask(task);
//                taskService.AddCandidateGroup(task.Id, "accountancy");
//                ids.Add(task.Id);
//            }

//            ClockUtil.CurrentTime = DateTime.Parse("2005/05/05  05:05:05.000");
//            // 1 task assigned to management and accountancy group
//            task = taskService.NewTask();
//            task.Name = "managementAndAccountancyTask";
//            taskService.SaveTask(task);
//            taskService.SetAssignee(task.Id, "fozzie");
//            taskService.AddCandidateGroup(task.Id, "management");
//            taskService.AddCandidateGroup(task.Id, "accountancy");
//            ids.Add(task.Id);

//            return ids;
//        }

//    }

//}