using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Task
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class TaskDecoratorTest : PluggableProcessEngineTestCase
    {
        [SetUp]
        public virtual void SetUp()
        {
            Task = (TaskEntity) taskService.NewTask();
            taskService.SaveTask(Task);

            ExpressionManager = processEngineConfiguration.ExpressionManager;

            TaskDefinition = new TaskDefinition(null);
            TaskDecorator = new TaskDecorator(TaskDefinition, ExpressionManager);
        }

        [TearDown]
        public virtual void tearDown()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new DeleteTaskCommand(this, Task));
        }

        protected internal TaskEntity Task;
        protected internal TaskDefinition TaskDefinition;
        protected internal TaskDecorator TaskDecorator;
        protected internal ExpressionManager ExpressionManager;

        protected internal virtual void Decorate(TaskEntity task, TaskDecorator decorator)
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new DecorateTaskCommand(this, task, decorator));
        }
        [Test]
        public virtual void TestDecoratePriorityFromVariable()
        {
            // given
            var aPriority = 10;
            taskService.SetVariable(Task.Id, "priority", aPriority);

            var priorityExpression = ExpressionManager.CreateExpression("${priority}");
            TaskDefinition.PriorityExpression = priorityExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aPriority, Task.Priority);
        }

        [Test]
        public virtual void TestDecorateAssigneeFromVariable()
        {
            // given
            var aAssignee = "john";
            taskService.SetVariable(Task.Id, "assignee", aAssignee);

            var assigneeExpression = ExpressionManager.CreateExpression("${assignee}");
            TaskDefinition.AssigneeExpression = assigneeExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aAssignee, Task.Assignee);
        }

        protected internal class DecorateTaskCommand : ICommand<object>
        {
            private readonly TaskDecoratorTest _outerInstance;
            protected internal TaskDecorator Decorator;


            protected internal TaskEntity Task;

            public DecorateTaskCommand(TaskDecoratorTest outerInstance, TaskEntity task, TaskDecorator decorator)
            {
                _outerInstance = outerInstance;
                Task = task;
                Decorator = decorator;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                Decorator.Decorate(Task, Task);
                return null;
            }
        }

        protected internal class DeleteTaskCommand : ICommand<object>
        {
            private readonly TaskDecoratorTest _outerInstance;


            protected internal TaskEntity Task;

            public DeleteTaskCommand(TaskDecoratorTest outerInstance, TaskEntity task)
            {
                _outerInstance = outerInstance;
                Task = task;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.TaskManager.DeleteTask(Task, null, true, false);

                return null;
            }
        }

        [Test]
        public virtual void TestDecorateAssignee()
        {
            // given
            var aAssignee = "john";
            var assigneeExpression = ExpressionManager.CreateExpression(aAssignee);
            TaskDefinition.AssigneeExpression = assigneeExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aAssignee, Task.Assignee);
        }

        [Test]
        public virtual void TestDecorateCandidateGroups()
        {
            // given
            IList<string> aCandidateGroupList = new List<string>();
            aCandidateGroupList.Add("management");
            aCandidateGroupList.Add("accounting");
            aCandidateGroupList.Add("backoffice");

            foreach (var candidateGroup in aCandidateGroupList)
            {
                var candidateGroupExpression = ExpressionManager.CreateExpression(candidateGroup);
                TaskDefinition.AddCandidateGroupIdExpression(candidateGroupExpression);
            }

            // when
            Decorate(Task, TaskDecorator);

            // then
            var candidates = Task.Candidates;
            Assert.AreEqual(3, candidates.Count);

            foreach (var identityLink in candidates)
            {
                var taskId = identityLink.TaskId;
                Assert.AreEqual(Task.Id, taskId);

                Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

                var groupId = identityLink.GroupId;
                if ("management".Equals(groupId))
                    Assert.AreEqual("management", groupId);
                else if ("accounting".Equals(groupId))
                    Assert.AreEqual("accounting", groupId);
                else if ("backoffice".Equals(groupId))
                    Assert.AreEqual("backoffice", groupId);
                else
                    Assert.Fail("Unexpected group: " + groupId);
            }
        }

        [Test]
        public virtual void TestDecorateCandidateGroupsFromVariable()
        {
            // given
            taskService.SetVariable(Task.Id, "management", "management");
            taskService.SetVariable(Task.Id, "accounting", "accounting");
            taskService.SetVariable(Task.Id, "backoffice", "backoffice");

            IList<string> aCandidateGroupList = new List<string>();
            aCandidateGroupList.Add("${management}");
            aCandidateGroupList.Add("${accounting}");
            aCandidateGroupList.Add("${backoffice}");

            foreach (var candidateGroup in aCandidateGroupList)
            {
                var candidateGroupExpression = ExpressionManager.CreateExpression(candidateGroup);
                TaskDefinition.AddCandidateGroupIdExpression(candidateGroupExpression);
            }

            // when
            Decorate(Task, TaskDecorator);

            // then
            var candidates = Task.Candidates;
            Assert.AreEqual(3, candidates.Count);

            foreach (var identityLink in candidates)
            {
                var taskId = identityLink.TaskId;
                Assert.AreEqual(Task.Id, taskId);

                Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

                var groupId = identityLink.GroupId;
                if ("management".Equals(groupId))
                    Assert.AreEqual("management", groupId);
                else if ("accounting".Equals(groupId))
                    Assert.AreEqual("accounting", groupId);
                else if ("backoffice".Equals(groupId))
                    Assert.AreEqual("backoffice", groupId);
                else
                    Assert.Fail("Unexpected group: " + groupId);
            }
        }

        [Test]
        public virtual void TestDecorateCandidateUsers()
        {
            // given
            IList<string> aCandidateUserList = new List<string>();
            aCandidateUserList.Add("john");
            aCandidateUserList.Add("peter");
            aCandidateUserList.Add("mary");

            foreach (var candidateUser in aCandidateUserList)
            {
                var candidateUserExpression = ExpressionManager.CreateExpression(candidateUser);
                TaskDefinition.AddCandidateUserIdExpression(candidateUserExpression);
            }

            // when
            Decorate(Task, TaskDecorator);

            // then
            var candidates = Task.Candidates;
            Assert.AreEqual(3, candidates.Count);

            foreach (var identityLink in candidates)
            {
                var taskId = identityLink.TaskId;
                Assert.AreEqual(Task.Id, taskId);

                Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

                var userId = identityLink.UserId;
                if ("john".Equals(userId))
                    Assert.AreEqual("john", userId);
                else if ("peter".Equals(userId))
                    Assert.AreEqual("peter", userId);
                else if ("mary".Equals(userId))
                    Assert.AreEqual("mary", userId);
                else
                    Assert.Fail("Unexpected user: " + userId);
            }
        }

        [Test]
        public virtual void TestDecorateCandidateUsersFromVariable()
        {
            // given
            taskService.SetVariable(Task.Id, "john", "john");
            taskService.SetVariable(Task.Id, "peter", "peter");
            taskService.SetVariable(Task.Id, "mary", "mary");

            IList<string> aCandidateUserList = new List<string>();
            aCandidateUserList.Add("${john}");
            aCandidateUserList.Add("${peter}");
            aCandidateUserList.Add("${mary}");

            foreach (var candidateUser in aCandidateUserList)
            {
                var candidateUserExpression = ExpressionManager.CreateExpression(candidateUser);
                TaskDefinition.AddCandidateUserIdExpression(candidateUserExpression);
            }

            // when
            Decorate(Task, TaskDecorator);

            // then
            var candidates = Task.Candidates;
            Assert.AreEqual(3, candidates.Count);

            foreach (var identityLink in candidates)
            {
                var taskId = identityLink.TaskId;
                Assert.AreEqual(Task.Id, taskId);

                Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);

                var userId = identityLink.UserId;
                if ("john".Equals(userId))
                    Assert.AreEqual("john", userId);
                else if ("peter".Equals(userId))
                    Assert.AreEqual("peter", userId);
                else if ("mary".Equals(userId))
                    Assert.AreEqual("mary", userId);
                else
                    Assert.Fail("Unexpected user: " + userId);
            }
        }

        [Test]
        public virtual void TestDecorateDescription()
        {
            // given
            var aDescription = "This is a Task";
            var descriptionExpression = ExpressionManager.CreateExpression(aDescription);
            TaskDefinition.DescriptionExpression = descriptionExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aDescription, Task.Description);
        }

        [Test]
        public virtual void TestDecorateDescriptionFromVariable()
        {
            // given
            var aDescription = "This is a Task";
            taskService.SetVariable(Task.Id, "description", aDescription);

            var descriptionExpression = ExpressionManager.CreateExpression("${description}");
            TaskDefinition.DescriptionExpression = descriptionExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aDescription, Task.Description);
        }

        [Test]
        public virtual void TestDecorateDueDate()
        {
            // given
            var aDueDate = "2014-06-01";
            var dueDate = DateTimeUtil.ParseDate(aDueDate);

            var dueDateExpression = ExpressionManager.CreateExpression(aDueDate);
            TaskDefinition.DueDateExpression = dueDateExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(dueDate, Task.DueDate);
        }

        [Test]
        public virtual void TestDecorateDueDateFromVariable()
        {
            // given
            var aDueDate = "2014-06-01";
            var dueDate = DateTimeUtil.ParseDate(aDueDate);
            taskService.SetVariable(Task.Id, "dueDate", dueDate);

            var dueDateExpression = ExpressionManager.CreateExpression("${dueDate}");
            TaskDefinition.DueDateExpression = dueDateExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(dueDate, Task.DueDate);
        }

        [Test]
        public virtual void TestDecorateFollowUpDate()
        {
            // given
            var aFollowUpDate = "2014-06-01";
            var followUpDate = DateTimeUtil.ParseDate(aFollowUpDate);

            var followUpDateExpression = ExpressionManager.CreateExpression(aFollowUpDate);
            TaskDefinition.FollowUpDateExpression = followUpDateExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(followUpDate, Task.FollowUpDate);
        }

        [Test]
        public virtual void TestDecorateFollowUpDateFromVariable()
        {
            // given
            var aFollowUpDateDate = "2014-06-01";
            var followUpDate = DateTimeUtil.ParseDate(aFollowUpDateDate);
            taskService.SetVariable(Task.Id, "followUpDate", followUpDate);

            var followUpDateExpression = ExpressionManager.CreateExpression("${followUpDate}");
            TaskDefinition.FollowUpDateExpression = followUpDateExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(followUpDate, Task.FollowUpDate);
        }

        [Test]
        public virtual void TestDecorateName()
        {
            // given
            var aTaskName = "A Task Name";
            var nameExpression = ExpressionManager.CreateExpression(aTaskName);
            TaskDefinition.NameExpression = nameExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aTaskName, Task.Name);
        }

        [Test]
        public virtual void TestDecorateNameFromVariable()
        {
            // given
            var aTaskName = "A Task Name";
            taskService.SetVariable(Task.Id, "taskName", aTaskName);

            var nameExpression = ExpressionManager.CreateExpression("${taskName}");
            TaskDefinition.NameExpression = nameExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(aTaskName, Task.Name);
        }

        [Test]
        public virtual void TestDecoratePriority()
        {
            // given
            var aPriority = "10";
            var priorityExpression = ExpressionManager.CreateExpression(aPriority);
            TaskDefinition.PriorityExpression = priorityExpression;

            // when
            Decorate(Task, TaskDecorator);

            // then
            Assert.AreEqual(int.Parse(aPriority), Task.Priority);
        }
    }
}