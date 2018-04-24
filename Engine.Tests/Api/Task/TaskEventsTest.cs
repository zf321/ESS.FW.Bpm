using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{


    /// <summary>
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressWarnings("deprecation") public class TaskEventsTest extends history.Useroperationlog.AbstractUserOperationLogTest
    public class TaskEventsTest : Tests.History.UserOperationLog.AbstractUserOperationLogTest
    {

        internal static string JONNY = "jonny";
        internal static string ACCOUNTING = "accounting";
        internal static string IMAGE_PNG = "application/png";
        internal static string IMAGE_NAME = "my-image.png";
        internal static string IMAGE_DESC = "a super duper image";
        internal static string IMAGE_URL = "file://some/location/my-image.png";

        private ITask task;
        
        [SetUp]
        public void setUp()
        {
            task = taskService.NewTask();
            taskService.SaveTask(task);
        }
        
        [TearDown]
        protected internal void tearDown()
        {
            // Delete task
            taskService.DeleteTask(task.Id, true);
        }

        public virtual void testAddUserLinkEvents()
        {

            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            taskService.AddCandidateUser(task.Id, JONNY);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(1, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(JONNY, @event.MessageParts.First());
            Assert.AreEqual(IdentityLinkType.Candidate, @event.MessageParts[1]);
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionAddUserLink, @event.Action);
            Assert.AreEqual(JONNY + CommentEntity.MessagePartsMarker + IdentityLinkType.Candidate, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.Millisecond <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }

        public virtual void testDeleteUserLinkEvents()
        {

            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            taskService.AddCandidateUser(task.Id, JONNY);

            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + 5000);

            taskService.DeleteCandidateUser(task.Id, JONNY);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(2, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(JONNY, @event.MessageParts.First());
            Assert.AreEqual(IdentityLinkType.Candidate, @event.MessageParts[1]);
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionDeleteUserLink, @event.Action);
            Assert.AreEqual(JONNY + CommentEntity.MessagePartsMarker + IdentityLinkType.Candidate, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.TimeOfDay.Milliseconds <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }

        public virtual void testAddGroupLinkEvents()
        {

            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            taskService.AddCandidateGroup(task.Id, ACCOUNTING);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(1, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(ACCOUNTING, @event.MessageParts.First());
            Assert.AreEqual(IdentityLinkType.Candidate, @event.MessageParts[1]);
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionAddGroupLink, @event.Action);
            Assert.AreEqual(ACCOUNTING + CommentEntity.MessagePartsMarker + IdentityLinkType.Candidate, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.Millisecond <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }

        public virtual void testDeleteGroupLinkEvents()
        {

            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            taskService.AddCandidateGroup(task.Id, ACCOUNTING);

            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + 5000);

            taskService.DeleteCandidateGroup(task.Id, ACCOUNTING);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(2, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(ACCOUNTING, @event.MessageParts.First());
            Assert.AreEqual(IdentityLinkType.Candidate, @event.MessageParts[1]);
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionDeleteGroupLink, @event.Action);
            Assert.AreEqual(ACCOUNTING + CommentEntity.MessagePartsMarker + IdentityLinkType.Candidate, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.Millisecond <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }

        public virtual void testAddAttachmentEvents()
        {
            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            identityService.AuthenticatedUserId = JONNY;
            taskService.CreateAttachment(IMAGE_PNG, task.Id, null, IMAGE_NAME, IMAGE_DESC, IMAGE_URL);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(1, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(1, @event.MessageParts.Count());
            Assert.AreEqual(IMAGE_NAME, @event.MessageParts.First());
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionAddAttachment, @event.Action);
            Assert.AreEqual(IMAGE_NAME, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.Millisecond <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }

        public virtual void testDeleteAttachmentEvents()
        {
            // initially there are no task events
            Assert.True(taskService.GetTaskEvents(task.Id) == null);

            identityService.AuthenticatedUserId = JONNY;
            IAttachment attachment = taskService.CreateAttachment(IMAGE_PNG, task.Id, null, IMAGE_NAME, IMAGE_DESC, IMAGE_URL);

            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Millisecond + 5000);

            taskService.DeleteAttachment(attachment.Id);

            // now there is a task event created
            IList<IEvent> events = taskService.GetTaskEvents(task.Id);
            Assert.AreEqual(2, events.Count);
            IEvent @event = events[0];
            Assert.AreEqual(1, @event.MessageParts.Count());
            Assert.AreEqual(IMAGE_NAME, @event.MessageParts.First());
            Assert.AreEqual(task.Id, @event.TaskId);
            Assert.AreEqual(EventFields.ActionDeleteAttachment, @event.Action);
            Assert.AreEqual(IMAGE_NAME, @event.Message);
            Assert.AreEqual(null, @event.ProcessInstanceId);
            Assert.NotNull(@event.Time.Millisecond <= ClockUtil.CurrentTime.Millisecond);

            AssertNoCommentsForTask();
        }


        private void AssertNoCommentsForTask()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly TaskEventsTest outerInstance;

            public CommandAnonymousInnerClass(TaskEventsTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                Assert.True(commandContext.CommentManager.FindCommentsByTaskId(outerInstance.task.Id).Count == 0);
                return null;
            }
        }

    }

}