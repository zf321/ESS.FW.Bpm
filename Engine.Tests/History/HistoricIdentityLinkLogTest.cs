using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.History
{
    /// <summary>
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    [TestFixture]
    public class HistoricIdentityLinkLogTest : PluggableProcessEngineTestCase
    {
        private const string A_USER_ID = "aUserId";
        private const string B_USER_ID = "bUserId";
        private const string C_USER_ID = "cUserId";
        private const int numberOfUsers = 3;
        private const string A_GROUP_ID = "aGroupId";
        private const string INVALID_USER_ID = "InvalidUserId";
        private const string A_ASSIGNER_ID = "aAssignerId";
        private static readonly string PROCESS_DEFINITION_KEY = "oneTaskProcess";
        private const string GROUP_1 = "Group1";
        private const string USER_1 = "User1";
        private const string OWNER_1 = "Owner1";
        private const string IDENTITY_LINK_ADD = "add";
        private const string IDENTITY_LINK_DELETE = "Delete";

        [Test]
        public virtual void testShouldAddIdentityLinkForTaskCreationWithAssigneeAndOwner()
        {
            var taskAssigneeId = "Assigneee";
            var taskOwnerId = "Owner";
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            var taskAssignee = taskService.NewTask(taskAssigneeId);
            taskAssignee.Assignee = USER_1;
            taskService.SaveTask(taskAssignee);

            var taskOwner = taskService.NewTask(taskOwnerId);
            taskOwner.Owner = OWNER_1;
            taskService.SaveTask(taskOwner);

            var taskEmpty = taskService.NewTask();
            taskService.SaveTask(taskEmpty);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);

            // Basic Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 1);
            Assert.AreEqual(query.Where(c=>c.UserId==USER_1)
                .Count(), 1);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Owner)
                .Count(), 1);
            Assert.AreEqual(query.Where(c=>c.UserId==OWNER_1)
                .Count(), 1);

            taskService.DeleteTask(taskAssigneeId, true);
            taskService.DeleteTask(taskOwnerId, true);
            taskService.DeleteTask(taskEmpty.Id, true);
        }

        public virtual void addAndDeleteUserWithAssigner(string taskId, string identityLinkType)
        {
            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddUserIdentityLink(taskId, A_USER_ID, identityLinkType);
            taskService.DeleteUserIdentityLink(taskId, A_USER_ID, identityLinkType);
        }

        public virtual void addUserIdentityLinks(string taskId)
        {
            for (var userIndex = 1; userIndex <= numberOfUsers; userIndex++)
                taskService.AddUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.Owner);
        }

        public virtual void deleteUserIdentityLinks(string taskId)
        {
            for (var userIndex = 1; userIndex <= numberOfUsers; userIndex++)
                taskService.DeleteUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.Owner);
        }

        protected internal virtual IProcessInstance startProcessInstance(string key)
        {
            return runtimeService.StartProcessInstanceByKey(key);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddAndRemoveIdentityLinksForProcessDefinition()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // Given
            var latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==PROCESS_DEFINITION_KEY)
                .First();
            Assert.NotNull(latestProcessDef);
            var links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
            Assert.AreEqual(0, links.Count);

            // Add candiate group with process definition
            repositoryService.AddCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            // Add candidate IUser for process definition
            repositoryService.AddCandidateStarterUser(latestProcessDef.Id, USER_1);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);

            // Delete candiate group with process definition
            repositoryService.DeleteCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 3);

            // Delete candidate IUser for process definition
            repositoryService.DeleteCandidateStarterUser(latestProcessDef.Id, USER_1);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 4);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddClaimTaskCandidateforAddIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            taskService.Claim(taskId, A_USER_ID);

            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);

            //Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==A_USER_ID)
                .Count(), 1);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
                .Count(), 1);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
                .Count(), 0);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 1);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddDelegateTaskCandidateforAddIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddUserIdentityLink(taskId, A_USER_ID, IdentityLinkType.Assignee);
            taskService.DelegateTask(taskId, B_USER_ID);
            taskService.DeleteUserIdentityLink(taskId, B_USER_ID, IdentityLinkType.Assignee);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            // Addition of A_USER, Deletion of A_USER, Addition of A_USER as owner, Addition of B_USER and deletion of B_USER
            Assert.AreEqual(historicIdentityLinks.Count, 5);

            //Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==A_USER_ID)
                .Count(), 3);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==B_USER_ID)
                .Count(), 2);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
                .Count(), 3);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
                .Count(), 2);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 4);
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Owner)
                .Count(), 1);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddGroupCandidateForAddAndDeleteIdentityLink()
        {
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddCandidateGroup(taskId, A_GROUP_ID);
            taskService.DeleteCandidateGroup(taskId, A_GROUP_ID);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);

            // Basic Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.GroupId==A_GROUP_ID)
                .Count(), 2);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddIdentityLinkByProcessDefinitionAndStandalone()
        {
            var taskAssigneeId = "Assigneee";
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            var processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // given
            var taskAssignee = taskService.NewTask(taskAssigneeId);
            taskAssignee.Assignee = USER_1;
            taskService.SaveTask(taskAssignee);

            // if
            addAndDeleteUserWithAssigner(taskId, IdentityLinkType.Assignee);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 3);

            // Basic Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 3);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==processInstance.ProcessDefinitionId)
                .Count(), 2);
            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== PROCESS_DEFINITION_KEY)
                .Count(), 2);

            taskService.DeleteTask(taskAssigneeId, true);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddMultipleDelegateTaskCandidateforAddIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddUserIdentityLink(taskId, A_USER_ID, IdentityLinkType.Assignee);
            taskService.DelegateTask(taskId, B_USER_ID);
            taskService.DelegateTask(taskId, C_USER_ID);
            taskService.DeleteUserIdentityLink(taskId, C_USER_ID, IdentityLinkType.Assignee);
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            // Addition of A_USER, Deletion of A_USER, Addition of A_USER as owner,
            // Addition of B_USER, Deletion of B_USER, Addition of C_USER, Deletion of C_USER
            Assert.AreEqual(historicIdentityLinks.Count, 7);

            //Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==A_USER_ID)
                .Count(), 3);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==B_USER_ID)
                .Count(), 2);


            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.UserId==C_USER_ID)
                .Count(), 2);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
                .Count(), 4);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
                .Count(), 3);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 6);

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Owner)
                .Count(), 1);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddTaskAssigneeForAddandDeleteIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            addAndDeleteUserWithAssigner(taskId, IdentityLinkType.Assignee);
            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);

            // Basic Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Assignee)
                .Count(), 2);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddTaskCandidateForAddAndDeleteIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddCandidateUser(taskId, A_USER_ID);
            taskService.DeleteCandidateUser(taskId, A_USER_ID);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddTaskCandidateforAddIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.AddCandidateUser(taskId, A_USER_ID);

            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 1);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldAddTaskOwnerForAddandDeleteIdentityLink()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            addAndDeleteUserWithAssigner(taskId, IdentityLinkType.Owner);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 2);

            // Basic Query test
            var query = historyService.CreateHistoricIdentityLinkLogQuery();
            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Owner)
                .Count(), 2);
        }

        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldNotAddTaskCandidateForInvalidIdentityLinkDelete()
        {
            // Pre test
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);

            // given
            startProcessInstance(PROCESS_DEFINITION_KEY);
            var taskId = taskService.CreateTaskQuery()
                .First()
                .Id;

            // if
            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
            taskService.DeleteCandidateUser(taskId, INVALID_USER_ID);

            // then
            historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);
        }

        //CAM-7456
        [Test]
        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
        public virtual void testShouldNotDeleteIdentityLinkForTaskCompletion()
        {
            //given
            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(historicIdentityLinks.Count, 0);
            startProcessInstance(PROCESS_DEFINITION_KEY);

            var task = taskService.CreateTaskQuery()
                .First();
            taskService.AddCandidateUser(task.Id, "demo");

            //when
            taskService.Complete(task.Id);

            //then
            var historicIdentityLinkLogs = historyService.CreateHistoricIdentityLinkLogQuery()
                
                .ToList();
            Assert.AreEqual(1, historicIdentityLinkLogs.Count);
            Assert.AreNotEqual(IDENTITY_LINK_DELETE, historicIdentityLinkLogs[0].OperationType);
        }
    }
}