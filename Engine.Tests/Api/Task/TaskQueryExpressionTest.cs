//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Identity;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Task
//{
//    /// <summary>
//    /// </summary>
//    [TestFixture]
//    public class TaskQueryExpressionTest : ResourceProcessEngineTestCase
//    {
//        [SetUp]
//        public virtual void setUp()
//        {
//            group1 = createGroup("group1");
//            var group2 = createGroup("group2");
//            var group3 = createGroup("group3");

//            user = createUser("user", group1.Id, group2.Id);
//            anotherUser = createUser("anotherUser", group3.Id);
//            userWithoutGroups = createUser("userWithoutGroups");

//            //Time = 1427547759000l;
//            task = createTestTask("task");
//            // shift time to force distinguishable create times
//            adjustTime(2 * 60);
//            var anotherTask = createTestTask("anotherTask");
//            var assignedCandidateTask = createTestTask("assignedCandidateTask");


//            taskService.SetOwner(task.Id, user.Id);
//            taskService.SetAssignee(task.Id, user.Id);

//            taskService.AddCandidateUser(anotherTask.Id, user.Id);
//            taskService.AddCandidateGroup(anotherTask.Id, group1.Id);

//            taskService.SetAssignee(assignedCandidateTask.Id, user.Id);
//            taskService.AddCandidateUser(assignedCandidateTask.Id, user.Id);
//            taskService.AddCandidateGroup(assignedCandidateTask.Id, group1.Id);
//        }

//        [TearDown]
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @After public void tearDown()
//        public virtual void tearDown()
//        {
//            foreach (var group in identityService.CreateGroupQuery()
//                .ToList())
//                identityService.DeleteGroup(group.Id);
//            foreach (var user in identityService.CreateUserQuery()
//                .ToList())
//                identityService.DeleteUser(user.Id);
//            foreach (var task in taskService.CreateTaskQuery()
//                .ToList())
//                taskService.DeleteTask(task.Id, true);

//            identityService.ClearAuthentication();
//        }

//        protected internal ITask task;
//        protected internal IUser user;
//        protected internal IUser anotherUser;
//        protected internal IUser userWithoutGroups;
//        protected internal IGroup group1;

//        public TaskQueryExpressionTest() : base("resources/api/task/task-query-expression-test.Camunda.cfg.xml")
//        {
//        }


//        protected internal virtual IQueryable<ITask> taskQuery()
//        {
//            return taskService.CreateTaskQuery();
//        }

//        protected internal virtual void AssertCount(IQueryable<ITask> query, long Count)
//        {
//            Assert.AreEqual(Count, query.Count());
//        }

//        protected internal virtual IUser CurrentUser
//        {
//            set
//            {
//                var groups = identityService.CreateGroupQuery()
//                    .GroupMember(value.Id)
//                    
//                    .ToList();
//                IList<string> groupIds = new List<string>();
//                foreach (var group in groups)
//                    groupIds.Add(group.Id);

//                identityService.SetAuthentication(value.Id, groupIds);
//            }
//        }

//        protected internal virtual IGroup createGroup(string groupId)
//        {
//            var group = identityService.NewGroup(groupId);
//            identityService.SaveGroup(group);
//            return group;
//        }

//        protected internal virtual IUser createUser(string userId, params string[] groupIds)
//        {
//            var user = identityService.NewUser(userId);
//            identityService.SaveUser(user);

//            if (groupIds != null)
//                foreach (var groupId in groupIds)
//                    identityService.CreateMembership(userId, groupId);

//            return user;
//        }

//        protected internal virtual ITask createTestTask(string taskId)
//        {
//            var task = taskService.NewTask(taskId);
//            task.DueDate = task.CreateTime;
//            task.FollowUpDate = task.CreateTime;
//            taskService.SaveTask(task);
//            return task;
//        }

//        protected internal virtual DateTime now()
//        {
//            return ClockUtil.CurrentTime;
//        }

//        protected internal virtual long Time
//        {
//            set { Time = new DateTime().Millisecond; }
//        }

//        //protected internal virtual DateTime CurrentTime
//        //{
//        // set
//        // {
//        //ClockUtil.CurrentTime = value;
//        // }
//        //}

//        /// <summary>
//        ///     Changes the current time about the given amount in seconds.
//        /// </summary>
//        /// <param name="amount"> the amount to adjust the current time </param>
//        protected internal virtual void adjustTime(int amount)
//        {
//            var time = now()
//                           .Ticks + amount * 1000;
//            Time = time;
//        }

//        [Test]
//        public virtual void testExpressionOverrideQuery()
//        {
//            var queryString = "query";
//            var expressionString = "expression";
//            var testStringExpression = "${'" + expressionString + "'}";

//            var queryDate = DateTime.Now.AddDays(-1);
//            var testDateExpression = "${now()}";

//            var taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskAssignee(queryString)
//                .TaskAssigneeExpression(testStringExpression)
//                .TaskAssigneeLike(queryString)
//                .TaskAssigneeLikeExpression(testStringExpression)
//                .TaskOwnerExpression(queryString)
//                .TaskOwnerExpression(expressionString)
//                .TaskInvolvedUser(queryString)
//                .TaskInvolvedUserExpression(expressionString)
//                .TaskCreatedBefore(queryDate)
//                .TaskCreatedBeforeExpression(testDateExpression)
//                .TaskCreatedOn(queryDate)
//                .TaskCreatedOnExpression(testDateExpression)
//                .TaskCreatedAfter(queryDate)
//                .TaskCreatedAfterExpression(testDateExpression)
//                .DueBefore(queryDate)
//                .DueBeforeExpression(testDateExpression)
//                .DueDate(queryDate)
//                .DueDateExpression(testDateExpression)
//                .DueAfter(queryDate)
//                .DueAfterExpression(testDateExpression)
//                .FollowUpBefore(queryDate)
//                .FollowUpBeforeExpression(testDateExpression)
//                .FollowUpDate(queryDate)
//                .FollowUpDateExpression(testDateExpression)
//                .FollowUpAfter(queryDate)
//                .FollowUpAfterExpression(testDateExpression);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(expressionString, taskQuery.Assignee);
//            Assert.AreEqual(expressionString, taskQuery.AssigneeLike);
//            Assert.AreEqual(expressionString, taskQuery.Owner);
//            Assert.AreEqual(expressionString, taskQuery.InvolvedUser);
//            Assert.True(taskQuery.CreateTimeBefore > queryDate);
//            Assert.True(taskQuery.CreateTime > queryDate);
//            Assert.True(taskQuery.CreateTimeAfter > queryDate);
//            Assert.True(taskQuery.DueBefore > queryDate);
//            Assert.True(taskQuery.DueDate > queryDate);
//            Assert.True(taskQuery.DueAfter > queryDate);
//            Assert.True(taskQuery.FollowUpBefore > queryDate);
//            Assert.True(taskQuery.FollowUpDate > queryDate);
//            Assert.True(taskQuery.FollowUpAfter > queryDate);

//            // candidates has to be tested separately cause they have to be set exclusively

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateGroup(queryString)
//                .TaskCandidateGroupExpression(testStringExpression);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(expressionString, taskQuery.CandidateGroup);

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateUser(queryString)
//                .TaskCandidateUserExpression(testStringExpression);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(expressionString, taskQuery.CandidateUser);

//            CurrentUser = user;
//            IList<string> queryList = new List<string> {"query"};
//            var testGroupsExpression = "${currentUserGroups()}";

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateGroupIn(queryList)
//                .TaskCandidateGroupInExpression(testGroupsExpression);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(2, taskService.CreateTaskQuery()
//                .WithCandidateGroups()
//                .Count());
//        }

//        [Test]
//        public virtual void testQueryByAssigneeExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskAssigneeExpression("${'" + user.Id + "'}"), 2);
//            AssertCount(taskQuery()
//                .TaskAssigneeExpression("${'" + anotherUser.Id + "'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskAssigneeExpression("${currentUser()}"), 2);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskAssigneeExpression("${currentUser()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByAssigneeLikeExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskAssigneeLikeExpression("${'%" + user.Id.Substring(2) + "'}"), 2);
//            AssertCount(taskQuery()
//                .TaskAssigneeLikeExpression("${'%" + anotherUser.Id.Substring(2) + "'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskAssigneeLikeExpression("${'%'.concat(currentUser())}"), 2);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskAssigneeLikeExpression("${'%'.concat(currentUser())}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByCandidateGroupExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskCandidateGroupExpression("${'" + group1.Id + "'}"), 1);
//            AssertCount(taskQuery()
//                .TaskCandidateGroupExpression("${'unknown'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskCandidateGroupExpression("${currentUserGroups()[0]}"), 1);
//            AssertCount(taskQuery()
//                .TaskCandidateGroupExpression("${currentUserGroups()[0]}")
//                .IncludeAssignedTasks(), 2);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskCandidateGroupExpression("${currentUserGroups()[0]}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByCandidateGroupsExpression()
//        {
//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskCandidateGroupInExpression("${currentUserGroups()}"), 1);
//            AssertCount(taskQuery()
//                .TaskCandidateGroupInExpression("${currentUserGroups()}")
//                .IncludeAssignedTasks(), 2);

//            CurrentUser = anotherUser;

//            AssertCount(taskQuery()
//                .TaskCandidateGroupInExpression("${currentUserGroups()}"), 0);

//            CurrentUser = userWithoutGroups;
//            try
//            {
//                taskQuery()
//                    .TaskCandidateGroupInExpression("${currentUserGroups()}")
//                    .Count();
//                Assert.Fail("Exception expected");
//            }
//            catch (ProcessEngineException)
//            {
//                // expected because currentUserGroups will return null
//            }
//        }

//        [Test]
//        public virtual void testQueryByCandidateUserExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${'" + user.Id + "'}"), 1);
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${'" + user.Id + "'}")
//                .IncludeAssignedTasks(), 2);
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${'" + anotherUser.Id + "'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${currentUser()}"), 1);
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${currentUser()}")
//                .IncludeAssignedTasks(), 2);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskCandidateUserExpression("${currentUser()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByDueAfterExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .DueAfterExpression("${now()}"), 0);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .DueAfterExpression("${now()}"), 3);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .DueAfterExpression("${dateTime().plusMonths(2)}"), 0);

//            AssertCount(taskQuery()
//                .DueAfterExpression("${dateTime().MinusYears(1)}"), 3);
//        }

//        [Test]
//        public virtual void testQueryByDueBeforeExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .DueBeforeExpression("${now()}"), 3);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .DueBeforeExpression("${now()}"), 0);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .DueBeforeExpression("${dateTime().plusMonths(2)}"), 3);

//            AssertCount(taskQuery()
//                .DueBeforeExpression("${dateTime().MinusYears(1)}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByDueDateExpression()
//        {
//            Time = task.DueDate.Millisecond;
//            AssertCount(taskQuery()
//                .DueDateExpression("${now()}"), 1);

//            adjustTime(10);

//            AssertCount(taskQuery()
//                .DueDateExpression("${dateTime().MinusSeconds(10)}"), 1);

//            AssertCount(taskQuery()
//                .DueDateExpression("${now()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByFollowUpAfterExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .FollowUpAfterExpression("${now()}"), 0);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .FollowUpAfterExpression("${now()}"), 3);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .FollowUpAfterExpression("${dateTime().plusMonths(2)}"), 0);

//            AssertCount(taskQuery()
//                .FollowUpAfterExpression("${dateTime().MinusYears(1)}"), 3);
//        }

//        [Test]
//        public virtual void testQueryByFollowUpBeforeExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .FollowUpBeforeExpression("${now()}"), 3);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .FollowUpBeforeExpression("${now()}"), 0);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .FollowUpBeforeExpression("${dateTime().plusMonths(2)}"), 3);

//            AssertCount(taskQuery()
//                .FollowUpBeforeExpression("${dateTime().MinusYears(1)}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByFollowUpDateExpression()
//        {
//            Time = task.FollowUpDate.Millisecond;
//            AssertCount(taskQuery()
//                .FollowUpDateExpression("${now()}"), 1);

//            adjustTime(10);

//            AssertCount(taskQuery()
//                .FollowUpDateExpression("${dateTime().MinusSeconds(10)}"), 1);

//            AssertCount(taskQuery()
//                .FollowUpDateExpression("${now()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByInvolvedUserExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskInvolvedUserExpression("${'" + user.Id + "'}"), 2);
//            AssertCount(taskQuery()
//                .TaskInvolvedUserExpression("${'" + anotherUser.Id + "'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskInvolvedUserExpression("${currentUser()}"), 2);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskInvolvedUserExpression("${currentUser()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByOwnerExpression()
//        {
//            AssertCount(taskQuery()
//                .TaskOwnerExpression("${'" + user.Id + "'}"), 1);
//            AssertCount(taskQuery()
//                .TaskOwnerExpression("${'" + anotherUser.Id + "'}"), 0);

//            CurrentUser = user;
//            AssertCount(taskQuery()
//                .TaskOwnerExpression("${currentUser()}"), 1);

//            CurrentUser = anotherUser;
//            AssertCount(taskQuery()
//                .TaskOwnerExpression("${currentUser()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByTaskCreatedAfterExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .TaskCreatedAfterExpression("${now()}"), 0);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .TaskCreatedAfterExpression("${now()}"), 3);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .TaskCreatedAfterExpression("${dateTime().plusMonths(2)}"), 0);

//            AssertCount(taskQuery()
//                .TaskCreatedAfterExpression("${dateTime().MinusYears(1)}"), 3);
//        }

//        [Test]
//        public virtual void testQueryByTaskCreatedBeforeExpression()
//        {
//            adjustTime(1);

//            AssertCount(taskQuery()
//                .TaskCreatedBeforeExpression("${now()}"), 3);

//            adjustTime(-5 * 60);

//            AssertCount(taskQuery()
//                .TaskCreatedBeforeExpression("${now()}"), 0);

//            Time = task.CreateTime.Millisecond;

//            AssertCount(taskQuery()
//                .TaskCreatedBeforeExpression("${dateTime().plusMonths(2)}"), 3);

//            AssertCount(taskQuery()
//                .TaskCreatedBeforeExpression("${dateTime().MinusYears(1)}"), 0);
//        }

//        [Test]
//        public virtual void testQueryByTaskCreatedOnExpression()
//        {
//            Time = task.CreateTime.Millisecond;
//            AssertCount(taskQuery()
//                .TaskCreatedOnExpression("${now()}"), 1);

//            adjustTime(10);

//            AssertCount(taskQuery()
//                .TaskCreatedOnExpression("${dateTime().MinusSeconds(10)}"), 1);

//            AssertCount(taskQuery()
//                .TaskCreatedOnExpression("${now()}"), 0);
//        }

//        [Test]
//        public virtual void testQueryOverrideExpression()
//        {
//            var queryString = "query";
//            var expressionString = "expression";
//            var testStringExpression = "${'" + expressionString + "'}";

//            var queryDate = DateTime.Now.AddDays(-1);
//            var testDateExpression = "${now()}";

//            var taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskAssigneeExpression(testStringExpression)
//                .TaskAssignee(queryString)
//                .TaskAssigneeLikeExpression(testStringExpression)
//                .TaskAssigneeLike(queryString)
//                .TaskOwnerExpression(expressionString)
//                .TaskOwner(queryString)
//                .TaskInvolvedUserExpression(expressionString)
//                .TaskInvolvedUser(queryString)
//                .TaskCreatedBeforeExpression(testDateExpression)
//                .TaskCreatedBefore(queryDate)
//                .TaskCreatedOnExpression(testDateExpression)
//                .TaskCreatedOn(queryDate)
//                .TaskCreatedAfterExpression(testDateExpression)
//                .TaskCreatedAfter(queryDate)
//                .DueBeforeExpression(testDateExpression)
//                .DueBefore(queryDate)
//                .DueDateExpression(testDateExpression)
//                .DueDate(queryDate)
//                .DueAfterExpression(testDateExpression)
//                .DueAfter(queryDate)
//                .FollowUpBeforeExpression(testDateExpression)
//                .FollowUpBefore(queryDate)
//                .FollowUpDateExpression(testDateExpression)
//                .FollowUpDate(queryDate)
//                .FollowUpAfterExpression(testDateExpression)
//                .FollowUpAfter(queryDate);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(queryString, taskQuery.Assignee);
//            Assert.AreEqual(queryString, taskQuery.AssigneeLike);
//            Assert.AreEqual(queryString, taskQuery.Owner);
//            Assert.AreEqual(queryString, taskQuery.InvolvedUser);
//            Assert.True(taskQuery.CreateTimeBefore.Equals(queryDate));
//            Assert.True(taskQuery.CreateTime.Equals(queryDate));
//            Assert.True(taskQuery.CreateTimeAfter.Equals(queryDate));
//            Assert.True(taskQuery.DueBefore.Equals(queryDate));
//            Assert.True(taskQuery.DueDate.Equals(queryDate));
//            Assert.True(taskQuery.DueAfter.Equals(queryDate));
//            Assert.True(taskQuery.FollowUpBefore.Equals(queryDate));
//            Assert.True(taskQuery.FollowUpDate.Equals(queryDate));
//            Assert.True(taskQuery.FollowUpAfter.Equals(queryDate));

//            // candidates has to be tested separately cause they have to be set exclusively

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateGroupExpression(testStringExpression)
//                .TaskCandidateGroup(queryString);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(queryString, taskQuery.CandidateGroup);

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateUserExpression(testStringExpression)
//                .TaskCandidateUser(queryString);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(queryString, taskQuery.CandidateUser);

//            CurrentUser = user;
//            IList<string> queryList = new List<string> {"query"};
//            var testGroupsExpression = "${currentUserGroups()}";

//            taskQuery = (TaskQueryImpl) taskService.CreateTaskQuery()
//                .TaskCandidateGroupInExpression(testGroupsExpression)
//                .TaskCandidateGroupIn(queryList);

//            // execute query so expression will be evaluated
//            taskQuery.Count();

//            Assert.AreEqual(1, taskQuery.CandidateGroups.Count);
//        }
//    }
//}