using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;


namespace Engine.Tests.Api.Task
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [TestFixture]
    public class TaskIdentityLinksTest : PluggableProcessEngineTestCase
    {


        [Test]
        [Deployment("resources/api/task/IdentityLinksProcess.bpmn20.xml")]
        public virtual void testCandidateUserLink()
        {
            runtimeService.StartProcessInstanceByKey("IdentityLinksProcess");

            string taskId = taskService.CreateTaskQuery().First().Id;

            taskService.AddCandidateUser(taskId, "kermit");

            IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(taskId);
            IIdentityLink identityLink = identityLinks[0];

            Assert.IsNull(identityLink.GroupId);
            Assert.AreEqual("kermit", identityLink.UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
            Assert.AreEqual(taskId, identityLink.TaskId);

            Assert.AreEqual(1, identityLinks.Count);

            taskService.DeleteCandidateUser(taskId, "kermit");

            Assert.AreEqual(0, taskService.GetIdentityLinksForTask(taskId).Count);
        }
        

        [Test]
        [Deployment("resources/api/task/IdentityLinksProcess.bpmn20.xml")]
        public virtual void testCandidateGroupLink()
        {
            try
            {
                identityService.AuthenticatedUserId = "demo";

                runtimeService.StartProcessInstanceByKey("IdentityLinksProcess");

                string taskId = taskService.CreateTaskQuery().First().Id;

                taskService.AddCandidateGroup(taskId, "muppets");

                IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(taskId);
                IIdentityLink identityLink = identityLinks[0];

                Assert.AreEqual("muppets", identityLink.GroupId);
                Assert.IsNull("kermit", identityLink.UserId);
                Assert.AreEqual(IdentityLinkType.Candidate, identityLink.Type);
                Assert.AreEqual(taskId, identityLink.TaskId);

                Assert.AreEqual(1, identityLinks.Count);

                if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelFull)
                {
                    IList<IEvent> taskEvents = taskService.GetTaskEvents(taskId);
                    Assert.AreEqual(1, taskEvents.Count);
                    IEvent taskEvent = taskEvents[0];
                    Assert.AreEqual(EventFields.ActionAddGroupLink, taskEvent.Action);
                    IList<string> taskEventMessageParts = taskEvent.MessageParts;
                    Assert.AreEqual("muppets", taskEventMessageParts[0]);
                    Assert.AreEqual(IdentityLinkType.Candidate, taskEventMessageParts[1]);
                    Assert.AreEqual(2, taskEventMessageParts.Count);
                }

                taskService.DeleteCandidateGroup(taskId, "muppets");

                if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelFull)
                {
                    IList<IEvent> taskEvents = taskService.GetTaskEvents(taskId);
                    IEvent taskEvent = findTaskEvent(taskEvents, EventFields.ActionDeleteGroupLink);
                    Assert.AreEqual(EventFields.ActionDeleteGroupLink, taskEvent.Action);
                    IList<string> taskEventMessageParts = taskEvent.MessageParts;
                    Assert.AreEqual("muppets", taskEventMessageParts[0]);
                    Assert.AreEqual(IdentityLinkType.Candidate, taskEventMessageParts[1]);
                    Assert.AreEqual(2, taskEventMessageParts.Count);
                    Assert.AreEqual(2, taskEvents.Count);
                }

                Assert.AreEqual(0, taskService.GetIdentityLinksForTask(taskId).Count);
            }
            finally
            {
                identityService.ClearAuthentication();
            }
        }


        [Test]
        public virtual void testAssigneeLink()
        {
            ITask task = taskService.NewTask("task");
            task.Assignee = "assignee";
            taskService.SaveTask(task);

            IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(task.Id);
            Assert.NotNull(identityLinks);
            Assert.AreEqual(1, identityLinks.Count);

            IIdentityLink identityLink = identityLinks[0];
            Assert.AreEqual(IdentityLinkType.Assignee, identityLink.Type);
            Assert.AreEqual("assignee", identityLink.UserId);
            Assert.AreEqual("task", identityLink.TaskId);

            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testOwnerLink()
        {
            ITask task = taskService.NewTask("task");
            task.Owner = "owner";
            taskService.SaveTask(task);

            IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(task.Id);
            Assert.NotNull(identityLinks);
            Assert.AreEqual(1, identityLinks.Count);

            IIdentityLink identityLink = identityLinks[0];
            Assert.AreEqual(IdentityLinkType.Owner, identityLink.Type);
            Assert.AreEqual("owner", identityLink.UserId);
            Assert.AreEqual("task", identityLink.TaskId);

            taskService.DeleteTask(task.Id, true);
        }

        private IEvent findTaskEvent(IList<IEvent> taskEvents, string action)
        {
            foreach (IEvent @event in taskEvents)
            {
                if (action.Equals(@event.Action))
                {
                    return @event;
                }
            }
            //throw new AssertionFailedError("no task event found with action " + action);
            throw new System.Exception("no task event found with action " + action);
        }
        
        [Test]
        [Deployment("resources/api/task/IdentityLinksProcess.bpmn20.xml")]
        public virtual void testCustomTypeUserLink()
        {
            runtimeService.StartProcessInstanceByKey("IdentityLinksProcess");

            string taskId = taskService.CreateTaskQuery().First().Id;

            taskService.AddUserIdentityLink(taskId, "kermit", "interestee");

            IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(taskId);
            IIdentityLink identityLink = identityLinks[0];

            Assert.IsNull(identityLink.GroupId);
            Assert.AreEqual("kermit", identityLink.UserId);
            Assert.AreEqual("interestee", identityLink.Type);
            Assert.AreEqual(taskId, identityLink.TaskId);

            Assert.AreEqual(1, identityLinks.Count);

            taskService.DeleteUserIdentityLink(taskId, "kermit", "interestee");

            Assert.AreEqual(0, taskService.GetIdentityLinksForTask(taskId).Count);
        }
        
        [Test]
        [Deployment("resources/api/task/IdentityLinksProcess.bpmn20.xml")]
        public virtual void testCustomLinkGroupLink()
        {
            runtimeService.StartProcessInstanceByKey("IdentityLinksProcess");

            string taskId = taskService.CreateTaskQuery().First().Id;

            taskService.AddGroupIdentityLink(taskId, "muppets", "playing");

            IList<IIdentityLink> identityLinks = taskService.GetIdentityLinksForTask(taskId);
            IIdentityLink identityLink = identityLinks[0];

            Assert.AreEqual("muppets", identityLink.GroupId);
            Assert.IsNull("kermit", identityLink.UserId);
            Assert.AreEqual("playing", identityLink.Type);
            Assert.AreEqual(taskId, identityLink.TaskId);

            Assert.AreEqual(1, identityLinks.Count);

            taskService.DeleteGroupIdentityLink(taskId, "muppets", "playing");

            Assert.AreEqual(0, taskService.GetIdentityLinksForTask(taskId).Count);
        }

        [Test]
        public virtual void testDeleteAssignee()
        {
            ITask task = taskService.NewTask();
            task.Assignee = "nonExistingUser";
            taskService.SaveTask(task);

            taskService.DeleteUserIdentityLink(task.Id, "nonExistingUser", IdentityLinkType.Assignee);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id).First();
            Assert.IsNull(task.Assignee);
            Assert.AreEqual(0, taskService.GetIdentityLinksForTask(task.Id).Count);

            // cleanup
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testDeleteOwner()
        {
            ITask task = taskService.NewTask();
            task.Owner = "nonExistingUser";
            taskService.SaveTask(task);

            taskService.DeleteUserIdentityLink(task.Id, "nonExistingUser", IdentityLinkType.Owner);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id).First();
            Assert.IsNull(task.Owner);
            Assert.AreEqual(0, taskService.GetIdentityLinksForTask(task.Id).Count);

            // cleanup
            taskService.DeleteTask(task.Id, true);
        }

    }

}