using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Task
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class TaskServiceTest : PluggableProcessEngineTestCase
    {
        protected internal const string TWO_TASKS_PROCESS = "resources/api/twoTasksProcess.bpmn20.xml";
        [Test]
        public virtual void testRemoveVariablesNullTaskId()
        {
            try
            {
                taskService.RemoveVariables(null,null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test][Deployment( "resources/api/oneTaskProcess.bpmn20.xml" )]
        public virtual void testDeleteTaskPartOfProcess()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);
            var listTaskId = new List<string> {task.Id};
            try
            {
                taskService.DeleteTask(task.Id);
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }

            try
            {
                taskService.DeleteTask(task.Id, true);
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }

            try
            {
                taskService.DeleteTask(task.Id, "test");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId);
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId, true);
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId, "test");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running process", ae.Message);
            }
        }
        [Test][Deployment("resources/api/cmmn/oneTaskCase.cmmn") ]
        public virtual void testDeleteTaskPartOfCaseInstance()
        {
            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
                .First()
                .Id;

            // an active case instance
            caseService.WithCaseDefinition(caseDefinitionId)
                .Create();

            var caseExecutionId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();
            var listTaskId = new List<string> {task.Id};
            Assert.NotNull(task);

            try
            {
                taskService.DeleteTask(task.Id);
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }

            try
            {
                taskService.DeleteTask(task.Id, true);
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }

            try
            {
                taskService.DeleteTask(task.Id, "test");
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId);
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId, true);
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }

            try
            {
                taskService.DeleteTasks(listTaskId, "test");
                Assert.Fail("Should not be possible to Delete task");
            }
            catch (ProcessEngineException ae)
            {
                Assert.AreEqual("The task cannot be deleted because is part of a running case instance", ae.Message);
            }
        }

        [Test]
        private void checkHistoricVariableUpdateEntity(string VariableName, string ProcessInstanceId)
        {
            if (processEngineConfiguration.HistoryLevel.Id == ProcessEngineConfigurationImpl.HistorylevelFull)
            {
                var deletedVariableUpdateFound = false;

                IList<IHistoricDetail> resultSet = historyService.CreateHistoricDetailQuery()
                    .Where(c => c.ProcessInstanceId == ProcessInstanceId)
                    .ToList();
                foreach (var currentHistoricDetail in resultSet)
                {
                    Assert.True(currentHistoricDetail is HistoricVariableUpdateEventEntity);
                    var historicVariableUpdate = (HistoricVariableUpdateEventEntity) currentHistoricDetail;

                    if (historicVariableUpdate.Name.Equals(VariableName))
                        if (historicVariableUpdate.Value == null)
                            if (deletedVariableUpdateFound)
                                Assert.Fail("Mismatch: A HistoricVariableUpdateEntity with a null value already found");
                            else
                                deletedVariableUpdateFound = true;
                }

                Assert.True(deletedVariableUpdateFound);
            }
        }

        [Test]
        public virtual void testAddCandidateGroupNullGroupId()
        {
            try
            {
                taskService.AddCandidateGroup("taskId", null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("userId and groupId cannot both be null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddCandidateGroupNullTaskId()
        {
            try
            {
                taskService.AddCandidateGroup(null, "groupId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddCandidateGroupUnexistingTask()
        {
            var group = identityService.NewGroup("group");
            identityService.SaveGroup(group);
            try
            {
                taskService.AddCandidateGroup("unexistingTaskId", group.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
            }
            identityService.DeleteGroup(group.Id);
        }

        [Test]
        public virtual void testAddCandidateUserDuplicate()
        {
            // Check behavior when adding the same user twice as candidate
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            var task = taskService.NewTask();
            taskService.SaveTask(task);

            taskService.AddCandidateUser(task.Id, user.Id);

            // Add as candidate the second time
            taskService.AddCandidateUser(task.Id, user.Id);

            identityService.DeleteUser(user.Id);
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testAddCandidateUserNullTaskId()
        {
            try
            {
                taskService.AddCandidateUser(null, "userId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddCandidateUserNullUserId()
        {
            try
            {
                taskService.AddCandidateUser("taskId", null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("userId and groupId cannot both be null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddCandidateUserUnexistingTask()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            try
            {
                taskService.AddCandidateUser("unexistingTaskId", user.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
            }

            identityService.DeleteUser(user.Id);
        }

        [Test]
        public virtual void testAddGroupIdentityLinkNullTaskId()
        {
            try
            {
                taskService.AddGroupIdentityLink(null, "groupId", IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddGroupIdentityLinkNullUserId()
        {
            try
            {
                taskService.AddGroupIdentityLink("taskId", null, IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("userId and groupId cannot both be null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddGroupIdentityLinkUnexistingTask()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            try
            {
                taskService.AddGroupIdentityLink("unexistingTaskId", user.Id, IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
            }

            identityService.DeleteUser(user.Id);
        }

        [Test]
        public virtual void testAddTaskCommentNull()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var task = taskService.NewTask("testId");
                taskService.SaveTask(task);
                try
                {
                    taskService.CreateComment(task.Id, null, null);
                    Assert.Fail("Expected process engine exception");
                }
                catch (ProcessEngineException)
                {
                }
                finally
                {
                    taskService.DeleteTask(task.Id, true);
                }
            }
        }

        [Test]
        public virtual void testAddTaskNullComment()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
                try
                {
                    taskService.CreateComment(null, null, "test");
                    Assert.Fail("Expected process engine exception");
                }
                catch (ProcessEngineException)
                {
                }
        }

        [Test]
        public virtual void testAddUserIdentityLinkNullTaskId()
        {
            try
            {
                taskService.AddUserIdentityLink(null, "userId", IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddUserIdentityLinkNullUserId()
        {
            try
            {
                taskService.AddUserIdentityLink("taskId", null, IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("userId and groupId cannot both be null", ae.Message);
            }
        }

        [Test]
        public virtual void testAddUserIdentityLinkUnexistingTask()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            try
            {
                taskService.AddUserIdentityLink("unexistingTaskId", user.Id, IdentityLinkType.Candidate);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
            }

            identityService.DeleteUser(user.Id);
        }

        [Test]
        public virtual void testClaimAlreadyClaimedTaskByOtherUser()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);
            var secondUser = identityService.NewUser("seconduser");
            identityService.SaveUser(secondUser);

            // Claim task the first time
            taskService.Claim(task.Id, user.Id);

            try
            {
                taskService.Claim(task.Id, secondUser.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (TaskAlreadyClaimedException ae)
            {
                AssertTextPresent("ITask '" + task.Id + "' is already claimed by someone else.", ae.Message);
            }

            taskService.DeleteTask(task.Id, true);
            identityService.DeleteUser(user.Id);
            identityService.DeleteUser(secondUser.Id);
        }

        [Test]
        public virtual void testClaimAlreadyClaimedTaskBySameUser()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            // Claim task the first time
            taskService.Claim(task.Id, user.Id);
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();

            // Claim the task again with the same user. No exception should be thrown
            taskService.Claim(task.Id, user.Id);

            taskService.DeleteTask(task.Id, true);
            identityService.DeleteUser(user.Id);
        }

        [Test]
        public virtual void testClaimNullArguments()
        {
            try
            {
                taskService.Claim(null, "userid");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testClaimUnexistingTaskId()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            try
            {
                taskService.Claim("unexistingtaskid", user.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingtaskid", ae.Message);
            }

            identityService.DeleteUser(user.Id);
        }

        [Test]
        public virtual void testCompleteTaskNullTaskId()
        {
            try
            {
                taskService.Complete(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }


        [Test]
        public virtual void testCompleteTaskShouldCompleteCaseExecution()
        {
            // given
            var caseDefinitionId = repositoryService.CreateCaseDefinitionQuery()
                .First()
                .Id;

            // an active case instance
            caseService.WithCaseDefinition(caseDefinitionId)
                .Create();

            var caseExecutionId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            var task = taskService.CreateTaskQuery()
                .First();
            Assert.NotNull(task);

            // when
            taskService.Complete(task.Id);

            // then

            task = taskService.CreateTaskQuery()
                .First();

            Assert.IsNull(task);

            var caseExecution = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First();

            Assert.IsNull(caseExecution);

            var caseInstance = caseService.CreateCaseInstanceQuery()
                .First();

            Assert.NotNull(caseInstance);
            Assert.True(caseInstance.Completed);
        }

        [Test]
        public virtual void testCompleteTaskUnexistingTaskId()
        {
            try
            {
                taskService.Complete("unexistingtask");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingtask", ae.Message);
            }
        }


        [Test]
        public virtual void testCompleteTaskWithParametersEmptyParameters()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            var taskId = task.Id;
            taskService.Complete(taskId); //, Collections.EMPTY_MAP

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                historyService.DeleteHistoricTaskInstance(taskId);

            // Fetch the task again
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.IsNull(task);
        }

        [Test]
        public virtual void testCompleteTaskWithParametersNullParameters()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            var taskId = task.Id;
            taskService.Complete(taskId, null);

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
                historyService.DeleteHistoricTaskInstance(taskId);

            // Fetch the task again
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.IsNull(task);
        }

        [Test]
        public virtual void testCompleteTaskWithParametersNullTaskId()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["myKey"] = "myValue";

            try
            {
                taskService.Complete(null, variables);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testCompleteTaskWithParametersUnexistingTaskId()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["myKey"] = "myValue";

            try
            {
                taskService.Complete("unexistingtask", variables);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingtask", ae.Message);
            }
        }


        [Test]
        public virtual void testCompleteWithParametersTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("twoTasksProcess");

            // Fetch first task
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("First task", task.Name);

            // Complete first task
            IDictionary<string, object> taskParams = new Dictionary<string, object>();
            taskParams["myParam"] = "myValue";
            taskService.Complete(task.Id, taskParams);

            // Fetch second task
            task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("Second task", task.Name);

            // Verify task parameters set on execution
            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("myValue", variables["myParam"]);
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @RequiredHistoryLevel(org.Camunda.bpm.Engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testCreateTaskAttachmentWithNullTaskAndProcessInstance()

        [Test]
        public virtual void testCreateTaskAttachmentWithNullTaskAndProcessInstance()
        {
            try
            {
                taskService.CreateAttachment("web page", null, null, "weatherforcast", "temperatures and more",
                    new MemoryStream("someContent".GetBytes()));
                Assert.Fail("expected process engine exception");
            }
            catch (ProcessEngineException)
            {
            }
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testCreateTaskAttachmentWithNullTaskId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var attachment = taskService.CreateAttachment("web page", null, processInstance.Id, "weatherforcast",
                "temperatures and more", new MemoryStream("someContent".GetBytes()));
            var fetched = taskService.GetAttachment(attachment.Id);
            Assert.IsTrue(fetched != null);
            Assert.That(fetched.TaskId, Is.EqualTo(null));
            Assert.IsTrue(fetched.ProcessInstanceId != null);
            taskService.DeleteAttachment(attachment.Id);
        }

        [Test]
        public virtual void testDeleteTaskAttachmentWithNullParameters()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
                try
                {
                    taskService.DeleteTaskAttachment(null, null);
                    Assert.Fail("expected process engine exception");
                }
                catch (ProcessEngineException)
                {
                }
        }

        [Test]
        public virtual void testDeleteTaskAttachmentWithTaskIdNull()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
                try
                {
                    taskService.DeleteTaskAttachment(null, "myAttachmentId");
                    Assert.Fail("expected process engine exception");
                }
                catch (ProcessEngineException)
                {
                }
        }

        [Test]
        public virtual void testDeleteTaskNullTaskId()
        {
            try
            {
                taskService.DeleteTask(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Expected exception
            }
        }

        [Test]
        public virtual void testDeleteTasksNullTaskIds()
        {
            try
            {
                taskService.DeleteTasks(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException)
            {
                // Expected exception
            }
        }

        [Test]
        public virtual void testDeleteTasksTaskIdsUnexistingTaskId()
        {
            var existingTask = taskService.NewTask();
            taskService.SaveTask(existingTask);

            // The unexisting taskId's should be silently ignored. Existing task should
            // have been deleted.
            taskService.DeleteTasks(new List<string> {"unexistingtaskid1", existingTask.Id}, true);

            existingTask = taskService.CreateTaskQuery(c => c.Id == existingTask.Id)
                .First();
            Assert.IsNull(existingTask);
        }

        [Test]
        public virtual void testDeleteTaskUnexistingTaskId()
        {
            // Deleting unexisting task should be silently ignored
            taskService.DeleteTask("unexistingtaskid");
        }

        [Test]
        public virtual void testDeleteTaskWithDeleteReason()
        {
            // ACT-900: deleteReason can be manually specified - can only be validated when historyLevel > ACTIVITY
            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelActivity)
            {
                var task = taskService.NewTask();
                task.Name = "test task";
                taskService.SaveTask(task);

                Assert.NotNull(task.Id);

                taskService.DeleteTask(task.Id, "deleted for testing purposes");

                var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery(c => c.Id == task.Id)
                    .First();

                Assert.NotNull(historicTaskInstance);
                Assert.AreEqual("deleted for testing purposes", historicTaskInstance.DeleteReason);

                // Delete historic task that is left behind, will not be cleaned up because this is not part of a process
                taskService.DeleteTask(task.Id, true);
            }
        }

        [Test]
        public virtual void testGetIdentityLinksWithAssignee()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            identityService.SaveUser(identityService.NewUser("kermit"));

            taskService.Claim(taskId, "kermit");
            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(1, identityLinks.Count);
            Assert.AreEqual("kermit", identityLinks[0]
                .UserId);
            Assert.IsNull(identityLinks[0]
                .GroupId);
            Assert.AreEqual(IdentityLinkType.Assignee, identityLinks[0]
                .Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
            identityService.DeleteUser("kermit");
        }

        [Test]
        public virtual void testGetIdentityLinksWithCandidateGroup()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            identityService.SaveGroup(identityService.NewGroup("muppets"));

            taskService.AddCandidateGroup(taskId, "muppets");
            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(1, identityLinks.Count);
            Assert.AreEqual("muppets", identityLinks[0]
                .GroupId);
            Assert.IsNull(identityLinks[0]
                .UserId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLinks[0]
                .Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
            identityService.DeleteGroup("muppets");
        }

        [Test]
        public virtual void testGetIdentityLinksWithCandidateUser()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            identityService.SaveUser(identityService.NewUser("kermit"));

            taskService.AddCandidateUser(taskId, "kermit");
            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(1, identityLinks.Count);
            Assert.AreEqual("kermit", identityLinks[0]
                .UserId);
            Assert.IsNull(identityLinks[0]
                .GroupId);
            Assert.AreEqual(IdentityLinkType.Candidate, identityLinks[0]
                .Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
            identityService.DeleteUser("kermit");
        }

        [Test]
        public virtual void testGetIdentityLinksWithNonExistingAssignee()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            taskService.Claim(taskId, "nonExistingAssignee");
            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(1, identityLinks.Count);
            Assert.AreEqual("nonExistingAssignee", identityLinks[0]
                .UserId);
            Assert.IsNull(identityLinks[0]
                .GroupId);
            Assert.AreEqual(IdentityLinkType.Assignee, identityLinks[0]
                .Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testGetIdentityLinksWithNonExistingOwner()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            taskService.Claim(taskId, "nonExistingOwner");
            taskService.DelegateTask(taskId, "nonExistingAssignee");
            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(2, identityLinks.Count);

            var assignee = identityLinks[0];
            Assert.AreEqual("nonExistingAssignee", assignee.UserId);
            Assert.IsNull(assignee.GroupId);
            Assert.AreEqual(IdentityLinkType.Assignee, assignee.Type);

            var owner = identityLinks[1];
            Assert.AreEqual("nonExistingOwner", owner.UserId);
            Assert.IsNull(owner.GroupId);
            Assert.AreEqual(IdentityLinkType.Owner, owner.Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testGetIdentityLinksWithOwner()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var taskId = task.Id;

            identityService.SaveUser(identityService.NewUser("kermit"));
            identityService.SaveUser(identityService.NewUser("fozzie"));

            taskService.Claim(taskId, "kermit");
            taskService.DelegateTask(taskId, "fozzie");

            var identityLinks = taskService.GetIdentityLinksForTask(taskId);
            Assert.AreEqual(2, identityLinks.Count);

            var assignee = identityLinks[0];
            Assert.AreEqual("fozzie", assignee.UserId);
            Assert.IsNull(assignee.GroupId);
            Assert.AreEqual(IdentityLinkType.Assignee, assignee.Type);

            var owner = identityLinks[1];
            Assert.AreEqual("kermit", owner.UserId);
            Assert.IsNull(owner.GroupId);
            Assert.AreEqual(IdentityLinkType.Owner, owner.Type);

            //cleanup
            taskService.DeleteTask(taskId, true);
            identityService.DeleteUser("kermit");
            identityService.DeleteUser("fozzie");
        }

        [Test]
        public virtual void testGetTaskAttachmentContentByTaskIdAndAttachmentId()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // create and save task
                var task = taskService.NewTask();
                taskService.SaveTask(task);
                var taskId = task.Id;

                // Fetch the task again and update
                // add attachment
                var attachment = taskService.CreateAttachment("web page", taskId, "someprocessinstanceid",
                    "weatherforcast", "temperatures and more", new MemoryStream("someContent".GetBytes()));
                var attachmentId = attachment.Id;

                // get attachment for taskId and attachmentId
                var taskAttachmentContent = taskService.GetTaskAttachmentContent(taskId, attachmentId);
                Assert.NotNull(taskAttachmentContent);

                var byteContent = IoUtil.ReadInputStream(taskAttachmentContent, "weatherforcast");
                Assert.AreEqual("someContent", StringHelperClass.NewString(byteContent));

                taskService.DeleteTask(taskId, true);
            }
        }

        [Test]
        public virtual void testGetTaskAttachmentContentWithNullParameters()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var content = taskService.GetTaskAttachmentContent(null, null);
                Assert.IsNull(content);
            }
        }

        [Test]
        public virtual void testGetTaskAttachmentsWithTaskIdNull()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
                Assert.AreEqual(Enumerable.Empty<IAttachment>(), taskService.GetTaskAttachments(null));
        }

        [Test]
        public virtual void testGetTaskAttachmentWithNullParameters()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var attachment = taskService.GetTaskAttachment(null, null);
                Assert.IsNull(attachment);
            }
        }

        [Test]
        public virtual void testGetTaskCommentByTaskIdAndCommentId()
        {
            if (processEngineConfiguration.HistoryLevel.Id > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // create and save new task
                var task = taskService.NewTask();
                taskService.SaveTask(task);

                var taskId = task.Id;

                // add comment to task
                var comment = taskService.CreateComment(taskId, null,
                    "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd");

                // select task comment for task id and comment id
                comment = taskService.GetTaskComment(taskId, comment.Id);
                // check returned comment
                Assert.NotNull(comment.Id);
                Assert.AreEqual(taskId, comment.TaskId);
                Assert.IsNull(comment.ProcessInstanceId);
                Assert.AreEqual(
                    "look at this isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg ..",
                    ((IEvent) comment).Message);
                Assert.AreEqual(
                    "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd",
                    comment.FullMessage);
                Assert.NotNull(comment.Time);

                // Delete task
                taskService.DeleteTask(task.Id, true);
            }
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesLocalTyped()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            taskService.SetVariablesLocal(taskId, vars);

            var variablesTyped = taskService.GetVariablesLocalTyped(taskId);
            Assert.AreEqual(vars, variablesTyped);
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesLocalTypedDeserialize()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            //taskService.SetVariablesLocal(taskId, Variable.Variables.CreateVariables().PutValue("broken", Variable.Variables.SerializedObjectValue("broken")
            //          .SerializationDataFormat(Variable.Variables.SerializationDataFormats.Java.ToString())
            //          .objectTypeName("unexisting").Create()));

            // this works
            var variablesTyped = taskService.GetVariablesLocalTyped(taskId, false);
            Assert.NotNull(variablesTyped.GetValueTyped<ITypedValue>("broken"));
            variablesTyped = taskService.GetVariablesLocalTyped(taskId, new List<string> {"broken"}, false);
            Assert.NotNull(variablesTyped.GetValueTyped<ITypedValue>("broken"));

            // this does not
            try
            {
                taskService.GetVariablesLocalTyped(taskId);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }

            // this does not
            try
            {
                taskService.GetVariablesLocalTyped(taskId, new List<string> {"broken"}, true);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesTyped()
        {
            IDictionary<string, object> vars = new Dictionary<string, object>();
            vars["variable1"] = "value1";
            vars["variable2"] = "value2";

            runtimeService.StartProcessInstanceByKey("oneTaskProcess", vars);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;
            var variablesTyped = taskService.GetVariablesTyped(taskId);
            Assert.AreEqual(vars, variablesTyped);
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testGetVariablesTypedDeserialize()
        {
            //runtimeService.StartProcessInstanceByKey("oneTaskProcess",Variable.Variables.CreateVariables().PutValue("broken", Variable.Variables.SerializedObjectValue("broken").SerializationDataFormat(Variable.Variables.SerializationDataFormats.Java.ToString())
            //          .objectTypeName("unexisting").Create()));
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // this works
            var variablesTyped = taskService.GetVariablesTyped(taskId, false);
            Assert.NotNull(variablesTyped.GetValueTyped<ITypedValue>("broken"));
            variablesTyped = taskService.GetVariablesTyped(taskId, new List<string> {"broken"}, false);
            Assert.NotNull(variablesTyped.GetValueTyped<ITypedValue>("broken"));

            // this does not
            try
            {
                taskService.GetVariablesTyped(taskId);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }

            // this does not
            try
            {
                taskService.GetVariablesTyped(taskId, new List<string> {"broken"}, true);
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Cannot deserialize object", e.Message);
            }
        }
        
        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testHumanTaskCompleteWithVariables()
        {
            // given
            caseService.CreateCaseInstanceByKey("oneTaskCase");

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            var VariableName = "aVariable";
            var variableValue = "aValue";

            // when
            taskService.Complete(taskId, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue(VariableName, variableValue));

            // then
            var variable = runtimeService.CreateVariableInstanceQuery()
                .First();

            Assert.AreEqual(variable.Name, VariableName);
            Assert.AreEqual(variable.Value, variableValue);
        }
        
        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testHumanTaskLocalVariables()
        {
            // given
            var caseInstanceId = caseService.CreateCaseInstanceByKey("oneTaskCase")
                .Id;

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            var VariableName = "aVariable";
            var variableValue = "aValue";

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // when
            taskService.SetVariableLocal(taskId, VariableName, variableValue);

            // then
            var variableInstance = runtimeService.CreateVariableInstanceQuery()
                // .TaskIdIn(taskId)
                .First();
            Assert.NotNull(variableInstance);

            Assert.AreEqual(caseInstanceId, variableInstance.CaseInstanceId);
            Assert.AreEqual(humanTaskId, variableInstance.CaseExecutionId);
        }
        
        [Test]
        [Deployment("resources/api/cmmn/oneTaskCase.cmmn")]
        public virtual void testHumanTaskWithLocalVariablesCompleteWithVariable()
        {
            // given
            caseService.CreateCaseInstanceByKey("oneTaskCase");

            var humanTaskId = caseService.CreateCaseExecutionQuery(c => c.ActivityId == "PI_HumanTask_1")
                .First()
                .Id;

            var VariableName = "aVariable";
            var variableValue = "aValue";
            var variableAnotherValue = "anotherValue";

            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            taskService.SetVariableLocal(taskId, VariableName, variableValue);

            // when
            taskService.Complete(taskId, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue(VariableName, variableAnotherValue));

            // then
            var variable = runtimeService.CreateVariableInstanceQuery()
                .First();

            Assert.AreEqual(variable.Name, VariableName);
            Assert.AreEqual(variable.Value, variableAnotherValue);
        }

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariable()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var currentTask = taskService.CreateTaskQuery()
                .First();

            taskService.SetVariable(currentTask.Id, "variable1", "value1");
            Assert.AreEqual("value1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable1"));

            taskService.RemoveVariable(currentTask.Id, "variable1");

            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable1"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariableLocal()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var currentTask = taskService.CreateTaskQuery()
                .First();

            taskService.SetVariableLocal(currentTask.Id, "variable1", "value1");
            Assert.AreEqual("value1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.AreEqual("value1", taskService.GetVariableLocal(currentTask.Id, "variable1"));

            taskService.RemoveVariableLocal(currentTask.Id, "variable1");

            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable1"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
        }

        [Test]
        public virtual void testRemoveVariableLocalNullTaskId()
        {
            try
            {
                taskService.RemoveVariableLocal(null, "variable");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testRemoveVariableNullTaskId()
        {
            try
            {
                taskService.RemoveVariable(null, "variable");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariables()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var currentTask = taskService.CreateTaskQuery()
                .First();

            IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
            varsToDelete["variable1"] = "value1";
            varsToDelete["variable2"] = "value2";
            taskService.SetVariables(currentTask.Id, varsToDelete);
            taskService.SetVariable(currentTask.Id, "variable3", "value3");

            Assert.AreEqual("value1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.AreEqual("value2", taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariable(currentTask.Id, "variable3"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable2"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable3"));

            taskService.RemoveVariables(currentTask.Id, varsToDelete.Keys);

            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariable(currentTask.Id, "variable3"));

            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable2"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable3"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
        }
        
        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testRemoveVariablesLocal()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var currentTask = taskService.CreateTaskQuery()
                .First();

            IDictionary<string, object> varsToDelete = new Dictionary<string, object>();
            varsToDelete["variable1"] = "value1";
            varsToDelete["variable2"] = "value2";
            taskService.SetVariablesLocal(currentTask.Id, varsToDelete);
            taskService.SetVariableLocal(currentTask.Id, "variable3", "value3");

            Assert.AreEqual("value1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.AreEqual("value2", taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariable(currentTask.Id, "variable3"));
            Assert.AreEqual("value1", taskService.GetVariableLocal(currentTask.Id, "variable1"));
            Assert.AreEqual("value2", taskService.GetVariableLocal(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariableLocal(currentTask.Id, "variable3"));

            taskService.RemoveVariables(currentTask.Id, varsToDelete.Keys);

            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariable(currentTask.Id, "variable3"));

            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariableLocal(currentTask.Id, "variable2"));
            Assert.AreEqual("value3", taskService.GetVariableLocal(currentTask.Id, "variable3"));

            checkHistoricVariableUpdateEntity("variable1", processInstance.Id);
            checkHistoricVariableUpdateEntity("variable2", processInstance.Id);
        }
        
        [Test]
        public virtual void testRemoveVariablesLocalNullTaskId()
        {
            try
            {
                //taskService.RemoveVariablesLocal(null, Collections.EMPTY_LIST);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testResolveTaskNullTaskId()
        {
            try
            {
                taskService.ResolveTask(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testResolveTaskUnexistingTaskId()
        {
            try
            {
                taskService.ResolveTask("unexistingtask");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingtask", ae.Message);
            }
        }


        [Test]
        public virtual void testResolveTaskWithParametersEmptyParameters()
        {
            var task = taskService.NewTask();
            task.DelegationState = DelegationState.Pending;
            taskService.SaveTask(task);

            var taskId = task.Id;
            taskService.ResolveTask(taskId); // Collections.EMPTY_MAP

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
                historyService.DeleteHistoricTaskInstance(taskId);

            // Fetch the task again
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testResolveTaskWithParametersNullParameters()
        {
            var task = taskService.NewTask();
            task.DelegationState = DelegationState.Pending;
            taskService.SaveTask(task);

            var taskId = task.Id;
            taskService.ResolveTask(taskId, null);

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
                historyService.DeleteHistoricTaskInstance(taskId);

            // Fetch the task again
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            taskService.DeleteTask(taskId, true);
        }


        [Test]
        public virtual void testResolveWithParametersTask()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("twoTasksProcess");

            // Fetch first task
            var task = taskService.CreateTaskQuery()
                .First();
            Assert.AreEqual("First task", task.Name);

            task.Delegate("johndoe");

            // Resolve first task
            IDictionary<string, object> taskParams = new Dictionary<string, object>();
            taskParams["myParam"] = "myValue";
            taskService.ResolveTask(task.Id, taskParams);

            // Verify that task is resolved
            task = taskService.CreateTaskQuery()
                //.TaskDelegationState(DelegationState.Resolved)
                .First();
            Assert.AreEqual("First task", task.Name);

            // Verify task parameters set on execution
            var variables = runtimeService.GetVariables(processInstance.Id);
            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("myValue", variables["myParam"]);
        }

        [Test]
        public virtual void testSaveAttachment()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // given
                var task = taskService.NewTask();
                taskService.SaveTask(task);

                var attachmentType = "someAttachment";
                var ProcessInstanceId = "someProcessInstanceId";
                var attachmentName = "attachmentName";
                var attachmentDescription = "attachmentDescription";
                var url = "http://camunda.org";

                var attachment = taskService.CreateAttachment(attachmentType, task.Id, ProcessInstanceId,
                    attachmentName, attachmentDescription, url);

                // when
                attachment.Description = "updatedDescription";
                attachment.Name = "updatedName";
                taskService.SaveAttachment(attachment);

                // then
                var fetchedAttachment = taskService.GetAttachment(attachment.Id);
                Assert.AreEqual(attachment.Id, fetchedAttachment.Id);
                Assert.AreEqual(attachmentType, fetchedAttachment.Type);
                Assert.AreEqual(task.Id, fetchedAttachment.TaskId);
                Assert.AreEqual(ProcessInstanceId, fetchedAttachment.ProcessInstanceId);
                Assert.AreEqual("updatedName", fetchedAttachment.Name);
                Assert.AreEqual("updatedDescription", fetchedAttachment.Description);
                Assert.AreEqual(url, fetchedAttachment.Url);

                taskService.DeleteTask(task.Id, true);
            }
        }


        [Test]
        public virtual void testSaveTaskNullTask()
        {
            try
            {
                taskService.SaveTask(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("task is null", ae.Message);
            }
        }

        [Test]
        public virtual void testSaveTaskSetParentTaskId()
        {
            // given
            var parent = taskService.NewTask("parent");
            taskService.SaveTask(parent);

            var task = taskService.NewTask("subTask");

            // when
            task.ParentTaskId = "parent";

            // then
            taskService.SaveTask(task);

            // update task
            task = taskService.CreateTaskQuery(c => c.Id == "subTask")
                .First();

            Assert.AreEqual(parent.Id, task.ParentTaskId);

            taskService.DeleteTask("parent", true);
            taskService.DeleteTask("subTask", true);
        }


        [Test]
        public virtual void testSaveTaskUpdate()
        {
            var task = taskService.NewTask();
            task.Description = "description";
            task.Name = "taskname";
            task.Priority = 0;
            task.Assignee = "taskassignee";
            task.Owner = "taskowner";
            var dueDate = DateTime.Parse("2003/02/01 04:05:06");
            task.DueDate = dueDate;
            task.CaseInstanceId = "taskcaseinstanceid";
            taskService.SaveTask(task);

            // Fetch the task again and update
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("description", task.Description);
            Assert.AreEqual("taskname", task.Name);
            Assert.AreEqual("taskassignee", task.Assignee);
            Assert.AreEqual("taskowner", task.Owner);
            Assert.AreEqual(dueDate, task.DueDate);
            Assert.AreEqual(0, task.Priority);
            Assert.AreEqual("taskcaseinstanceid", task.CaseInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery(c => c.Id == task.Id)
                    .First();
                Assert.AreEqual("taskname", historicTaskInstance.Name);
                Assert.AreEqual("description", historicTaskInstance.Description);
                Assert.AreEqual("taskassignee", historicTaskInstance.Assignee);
                Assert.AreEqual("taskowner", historicTaskInstance.Owner);
                Assert.AreEqual(dueDate, historicTaskInstance.DueDate);
                Assert.AreEqual(0, historicTaskInstance.Priority);
                Assert.AreEqual("taskcaseinstanceid", historicTaskInstance.CaseInstanceId);
            }

            task.Name = "updatedtaskname";
            task.Description = "updateddescription";
            task.Priority = 1;
            task.Assignee = "updatedassignee";
            task.Owner = "updatedowner";
            dueDate = DateTime.Parse("2003/02/01 04:05:06");
            task.DueDate = dueDate;
            task.CaseInstanceId = "updatetaskcaseinstanceid";
            taskService.SaveTask(task);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("updatedtaskname", task.Name);
            Assert.AreEqual("updateddescription", task.Description);
            Assert.AreEqual("updatedassignee", task.Assignee);
            Assert.AreEqual("updatedowner", task.Owner);
            Assert.AreEqual(dueDate, task.DueDate);
            Assert.AreEqual(1, task.Priority);
            Assert.AreEqual("updatetaskcaseinstanceid", task.CaseInstanceId);

            if (processEngineConfiguration.HistoryLevel.Id >= ProcessEngineConfigurationImpl.HistorylevelAudit)
            {
                var historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery(c => c.Id == task.Id)
                    .First();
                Assert.AreEqual("updatedtaskname", historicTaskInstance.Name);
                Assert.AreEqual("updateddescription", historicTaskInstance.Description);
                Assert.AreEqual("updatedassignee", historicTaskInstance.Assignee);
                Assert.AreEqual("updatedowner", historicTaskInstance.Owner);
                Assert.AreEqual(dueDate, historicTaskInstance.DueDate);
                Assert.AreEqual(1, historicTaskInstance.Priority);
                Assert.AreEqual("updatetaskcaseinstanceid", historicTaskInstance.CaseInstanceId);
            }

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testSaveTaskWithNonExistingParentTask()
        {
            // given
            var task = taskService.NewTask();

            // when
            task.ParentTaskId = "non-existing";

            // then
            try
            {
                taskService.SaveTask(task);
                Assert.Fail("It should not be possible to save a task with a non existing parent task.");
            }
            catch (NotValidException)
            {
            }
        }

        [Test]
        public virtual void testSetAssignee()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            var task = taskService.NewTask();
            Assert.IsNull(task.Assignee);
            taskService.SaveTask(task);

            // Set assignee
            taskService.SetAssignee(task.Id, user.Id);

            // Fetch task again
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual(user.Id, task.Assignee);

            identityService.DeleteUser(user.Id);
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testSetAssigneeNullTaskId()
        {
            try
            {
                taskService.SetAssignee(null, "userId");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testSetAssigneeUnexistingTask()
        {
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            try
            {
                taskService.SetAssignee("unexistingTaskId", user.Id);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingTaskId", ae.Message);
            }

            identityService.DeleteUser(user.Id);
        }

        /// <seealso cref= http:// jira.codehaus.org/ browse/ ACT-1059
        /// </seealso>
        [Test]
        public virtual void testSetDelegationState()
        {
            var task = taskService.NewTask();
            task.Owner = "wuzh";
            task.Delegate("other");
            taskService.SaveTask(task);
            var taskId = task.Id;

            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("wuzh", task.Owner);
            Assert.AreEqual("other", task.Assignee);
            Assert.AreEqual(DelegationState.Pending, task.DelegationState);

            task.DelegationState = DelegationState.Resolved;
            taskService.SaveTask(task);

            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("wuzh", task.Owner);
            Assert.AreEqual("other", task.Assignee);
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testSetPriority()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);

            taskService.SetPriority(task.Id, 12345);

            // Fetch task again to check if the priority is set
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual(12345, task.Priority);

            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testSetPriorityNullTaskId()
        {
            try
            {
                taskService.SetPriority(null, 12345);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("taskId is null", ae.Message);
            }
        }

        [Test]
        public virtual void testSetPriorityUnexistingTaskId()
        {
            try
            {
                taskService.SetPriority("unexistingtask", 12345);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("Cannot find task with id unexistingtask", ae.Message);
            }
        }

        [Test]
        public virtual void testTaskAssignee()
        {
            var task = taskService.NewTask();
            task.Assignee = "johndoe";
            taskService.SaveTask(task);

            // Fetch the task again and update
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("johndoe", task.Assignee);

            task.Assignee = "joesmoe";
            taskService.SaveTask(task);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("joesmoe", task.Assignee);

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testTaskAttachmentByTaskIdAndAttachmentId()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                // create and save task
                var task = taskService.NewTask();
                taskService.SaveTask(task);
                var taskId = task.Id;

                // Fetch the task again and update
                // add attachment
                var attachment = taskService.CreateAttachment("web page", taskId, "someprocessinstanceid",
                    "weatherforcast", "temperatures and more", "http://weather.com");
                var attachmentId = attachment.Id;

                // get attachment for taskId and attachmentId
                attachment = taskService.GetTaskAttachment(taskId, attachmentId);
                Assert.AreEqual("weatherforcast", attachment.Name);
                Assert.AreEqual("temperatures and more", attachment.Description);
                Assert.AreEqual("web page", attachment.Type);
                Assert.AreEqual(taskId, attachment.TaskId);
                Assert.AreEqual("someprocessinstanceid", attachment.ProcessInstanceId);
                Assert.AreEqual("http://weather.com", attachment.Url);
                Assert.IsNull(taskService.GetAttachmentContent(attachment.Id));

                // Delete attachment for taskId and attachmentId
                taskService.DeleteTaskAttachment(taskId, attachmentId);

                // check if attachment deleted
                Assert.IsNull(taskService.GetTaskAttachment(taskId, attachmentId));

                taskService.DeleteTask(taskId, true);
            }
        }

        [Test]
        public virtual void testTaskAttachments()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var task = taskService.NewTask();
                task.Owner = "johndoe";
                taskService.SaveTask(task);
                var taskId = task.Id;
                identityService.AuthenticatedUserId = "johndoe";
                // Fetch the task again and update
                taskService.CreateAttachment("web page", taskId, "someprocessinstanceid", "weatherforcast",
                    "temperatures and more", "http://weather.com");
                var attachment = taskService.GetTaskAttachments(taskId)
                    .First();
                Assert.AreEqual("weatherforcast", attachment.Name);
                Assert.AreEqual("temperatures and more", attachment.Description);
                Assert.AreEqual("web page", attachment.Type);
                Assert.AreEqual(taskId, attachment.TaskId);
                Assert.AreEqual("someprocessinstanceid", attachment.ProcessInstanceId);
                Assert.AreEqual("http://weather.com", attachment.Url);
                Assert.IsNull(taskService.GetAttachmentContent(attachment.Id));

                // Finally, clean up
                taskService.DeleteTask(taskId);

                Assert.AreEqual(0, taskService.GetTaskComments(taskId)
                    .Count());
                Assert.AreEqual(1, historyService.CreateHistoricTaskInstanceQuery(c => c.Id == taskId)
                    .Count());

                taskService.DeleteTask(taskId, true);
            }
        }

        [Test]
        public virtual void testTaskCaseInstanceId()
        {
            var task = taskService.NewTask();
            task.CaseInstanceId = "aCaseInstanceId";
            taskService.SaveTask(task);

            // Fetch the task again and update
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("aCaseInstanceId", task.CaseInstanceId);

            task.CaseInstanceId = "anotherCaseInstanceId";
            taskService.SaveTask(task);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("anotherCaseInstanceId", task.CaseInstanceId);

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testTaskComments()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            if (historyLevel > ProcessEngineConfigurationImpl.HistorylevelNone)
            {
                var task = taskService.NewTask();
                task.Owner = "johndoe";
                taskService.SaveTask(task);
                var taskId = task.Id;

                identityService.AuthenticatedUserId = "johndoe";
                // Fetch the task again and update
                var comment = taskService.CreateComment(taskId, null,
                    "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd");
                Assert.NotNull(comment.Id);
                Assert.AreEqual("johndoe", comment.UserId);
                Assert.AreEqual(taskId, comment.TaskId);
                Assert.IsNull(comment.ProcessInstanceId);
                Assert.AreEqual(
                    "look at this isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg ..",
                    ((IEvent) comment).Message);
                Assert.AreEqual(
                    "look at this \n       isn't this great? slkdjf sldkfjs ldkfjs ldkfjs ldkfj sldkfj sldkfj sldkjg laksfg sdfgsd;flgkj ksajdhf skjdfh ksjdhf skjdhf kalskjgh lskh dfialurhg kajsh dfuieqpgkja rzvkfnjviuqerhogiuvysbegkjz lkhf ais liasduh flaisduh ajiasudh vaisudhv nsfd",
                    comment.FullMessage);
                Assert.NotNull(comment.Time);

                taskService.CreateComment(taskId, "pid", "one");
                taskService.CreateComment(taskId, "pid", "two");

                ISet<string> expectedComments = new HashSet<string>();
                expectedComments.Add("one");
                expectedComments.Add("two");

                ISet<string> comments = new HashSet<string>();
                foreach (var cmt in taskService.GetProcessInstanceComments("pid"))
                    comments.Add(cmt.FullMessage);

                Assert.AreEqual(expectedComments, comments);

                // Finally, Delete task
                taskService.DeleteTask(taskId, true);
            }
        }

        [Test]
        public virtual void testTaskDelegation()
        {
            var task = taskService.NewTask();
            task.Owner = "johndoe";
            task.Delegate("joesmoe");
            taskService.SaveTask(task);
            var taskId = task.Id;

            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.AreEqual("joesmoe", task.Assignee);
            Assert.AreEqual(DelegationState.Pending, task.DelegationState);

            taskService.ResolveTask(taskId);
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.AreEqual("johndoe", task.Assignee);
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            task.Assignee = null;
            task.DelegationState = DelegationState.None; // task.DelegationState = NULL
            taskService.SaveTask(task);
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.IsNull(task.Assignee);
            Assert.IsNull(task.DelegationState);

            task.Assignee = "jackblack";
            task.DelegationState = DelegationState.Resolved;
            taskService.SaveTask(task);
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.AreEqual("jackblack", task.Assignee);
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            // Finally, Delete task
            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testTaskDelegationThroughServiceCall()
        {
            var task = taskService.NewTask();
            task.Owner = "johndoe";
            taskService.SaveTask(task);
            var taskId = task.Id;

            // Fetch the task again and update
            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();

            taskService.DelegateTask(taskId, "joesmoe");

            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.AreEqual("joesmoe", task.Assignee);
            Assert.AreEqual(DelegationState.Pending, task.DelegationState);

            taskService.ResolveTask(taskId);

            task = taskService.CreateTaskQuery(c => c.Id == taskId)
                .First();
            Assert.AreEqual("johndoe", task.Owner);
            Assert.AreEqual("johndoe", task.Assignee);
            Assert.AreEqual(DelegationState.Resolved, task.DelegationState);

            // Finally, Delete task
            taskService.DeleteTask(taskId, true);
        }

        [Test]
        public virtual void testTaskOwner()
        {
            var task = taskService.NewTask();
            task.Owner = "johndoe";
            taskService.SaveTask(task);

            // Fetch the task again and update
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("johndoe", task.Owner);

            task.Owner = "joesmoe";
            taskService.SaveTask(task);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual("joesmoe", task.Owner);

            // Finally, Delete task
            taskService.DeleteTask(task.Id, true);
        }

        [Test]
        public virtual void testUnClaimTask()
        {
            var task = taskService.NewTask();
            taskService.SaveTask(task);
            var user = identityService.NewUser("user");
            identityService.SaveUser(user);

            // Claim task the first time
            taskService.Claim(task.Id, user.Id);
            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.AreEqual(user.Id, task.Assignee);

            // Unclaim the task
            taskService.Claim(task.Id, null);

            task = taskService.CreateTaskQuery(c => c.Id == task.Id)
                .First();
            Assert.IsNull(task.Assignee);

            taskService.DeleteTask(task.Id, true);
            identityService.DeleteUser(user.Id);
        }
        

        [Test]
        [Deployment("resources/api/oneSubProcess.bpmn20.xml")]
        public virtual void testUpdateVariables()
        {
            IDictionary<string, object> globalVars = new Dictionary<string, object>();
            globalVars["variable4"] = "value4";
            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", globalVars);

            var currentTask = taskService.CreateTaskQuery()
                .First();
            IDictionary<string, object> localVars = new Dictionary<string, object>();
            localVars["variable1"] = "value1";
            localVars["variable2"] = "value2";
            localVars["variable3"] = "value3";
            taskService.SetVariablesLocal(currentTask.Id, localVars);

            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            ((TaskServiceImpl) taskService).UpdateVariables<TaskServiceImpl>(currentTask.Id, modifications, deletions);

            Assert.AreEqual("anotherValue1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable3"));
            Assert.IsNull(runtimeService.GetVariable(processInstance.Id, "variable4"));
        }

        [Test]
        public virtual void testUpdateVariablesForNonExistingTaskId()
        {
            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            try
            {
                ((TaskServiceImpl) taskService).UpdateVariables<TaskServiceImpl>("nonExistingId", modifications,
                    deletions);
                Assert.Fail("expected process engine exception");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testUpdateVariablesForNullTaskId()
        {
            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            try
            {
                ((TaskServiceImpl) taskService).UpdateVariables<TaskServiceImpl>(null, modifications, deletions);
                Assert.Fail("expected process engine exception");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testUpdateVariablesLocaForNullTaskId()
        {
            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            try
            {
                ((TaskServiceImpl) taskService).UpdateVariablesLocal(null, modifications, deletions);
                Assert.Fail("expected process engine exception");
            }
            catch (ProcessEngineException)
            {
            }
        }
        

        [Test]
        [Deployment("resources/api/oneSubProcess.bpmn20.xml")]
        public virtual void testUpdateVariablesLocal()
        {
            IDictionary<string, object> globalVars = new Dictionary<string, object>();
            globalVars["variable4"] = "value4";
            var processInstance = runtimeService.StartProcessInstanceByKey("startSimpleSubProcess", globalVars);

            var currentTask = taskService.CreateTaskQuery()
                .First();
            IDictionary<string, object> localVars = new Dictionary<string, object>();
            localVars["variable1"] = "value1";
            localVars["variable2"] = "value2";
            localVars["variable3"] = "value3";
            taskService.SetVariablesLocal(currentTask.Id, localVars);

            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            ((TaskServiceImpl) taskService).UpdateVariablesLocal(currentTask.Id, modifications, deletions);

            Assert.AreEqual("anotherValue1", taskService.GetVariable(currentTask.Id, "variable1"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable2"));
            Assert.IsNull(taskService.GetVariable(currentTask.Id, "variable3"));
            Assert.AreEqual("value4", runtimeService.GetVariable(processInstance.Id, "variable4"));
        }

        [Test]
        public virtual void testUpdateVariablesLocalForNonExistingTaskId()
        {
            IDictionary<string, object> modifications = new Dictionary<string, object>();
            modifications["variable1"] = "anotherValue1";
            modifications["variable2"] = "anotherValue2";

            IList<string> deletions = new List<string>();
            deletions.Add("variable2");
            deletions.Add("variable3");
            deletions.Add("variable4");

            try
            {
                ((TaskServiceImpl) taskService).UpdateVariablesLocal("nonExistingId", modifications, deletions);
                Assert.Fail("expected process engine exception");
            }
            catch (ProcessEngineException)
            {
            }
        }
        

        [Test]
        [Deployment("resources/api/oneTaskProcess.bpmn20.xml")]
        public virtual void testUserTaskOptimisticLocking()
        {
            runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var task1 = taskService.CreateTaskQuery()
                .First();
            var task2 = taskService.CreateTaskQuery()
                .First();

            task1.Description = "test description one";
            taskService.SaveTask(task1);

            try
            {
                task2.Description = "test description two";
                taskService.SaveTask(task2);

                Assert.Fail("Expecting exception");
            }
            catch (OptimisticLockingException)
            {
                // Expected exception
            }
        }
        
        [Test]
        [Deployment("resources/api/twoTasksProcess.bpmn20.xml")]
        public virtual void testUserTaskWithLocalVariablesCompleteWithVariable()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("twoTasksProcess");

            var VariableName = "aVariable";
            var variableValue = "aValue";
            var variableAnotherValue = "anotherValue";

            var taskId = taskService.CreateTaskQuery(c=>c.ProcessInstanceId == processInstance.Id)
                .First()
                .Id;

            taskService.SetVariableLocal(taskId, VariableName, variableValue);

            // when
            taskService.Complete(taskId, ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue(VariableName, variableAnotherValue));

            // then
            var variable = runtimeService.CreateVariableInstanceQuery(c=>c.ProcessInstanceId == processInstance.Id)
                .First();

            Assert.AreEqual(variable.Name, VariableName);
            Assert.AreEqual(variable.Value, variableAnotherValue);
        }
    }
}