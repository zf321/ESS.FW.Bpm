//using System;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Task;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.History
//{
//    /// <summary>
//    /// </summary>
//    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
//    [TestFixture]
//    public class HistoricIdentityLinkLogQueryTest : PluggableProcessEngineTestCase
//    {
//        private const string A_USER_ID = "aUserId";
//        private const string A_GROUP_ID = "aGroupId";
//        private const int numberOfUsers = 3;
//        private const string A_ASSIGNER_ID = "aAssignerId";

//        private const string INVALID_USER_ID = "InvalidUserId";
//        private const string INVALID_TASK_ID = "InvalidTask";
//        private const string INVALID_GROUP_ID = "InvalidGroupId";
//        private const string INVALID_ASSIGNER_ID = "InvalidAssignerId";
//        private const string INVALID_HISTORY_EVENT_TYPE = "InvalidEventType";
//        private const string INVALID_IDENTITY_LINK_TYPE = "InvalidIdentityLinkType";
//        private const string INVALID_PROCESS_DEFINITION_ID = "InvalidProcessDefinitionId";
//        private const string INVALID_PROCESS_DEFINITION_KEY = "InvalidProcessDefinitionKey";
//        private const string GROUP_1 = "Group1";
//        private const string USER_1 = "User1";
//        private static readonly string PROCESS_DEFINITION_KEY = "oneTaskProcess";

//        private static readonly string PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER =
//            "oneTaskProcessForHistoricIdentityLinkWithMultipleCanidateUser";

//        private const string IDENTITY_LINK_ADD = "add";
//        private const string IDENTITY_LINK_DELETE = "Delete";

//        [Test][Deployment(  "resources/api/runtime/oneTaskProcess.bpmn20.xml" ) ]
//        public virtual void testQueryAddTaskCandidateforAddIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            var processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateUser(taskId, A_USER_ID);

//            // Query test
//            var historicIdentityLink = historyService.CreateHistoricIdentityLinkLogQuery()
//                .First();
//            Assert.AreEqual(historicIdentityLink.UserId, A_USER_ID);
//            Assert.AreEqual(historicIdentityLink.TaskId, taskId);
//            Assert.AreEqual(historicIdentityLink.Type, IdentityLinkType.Candidate);
//            Assert.AreEqual(historicIdentityLink.AssignerId, A_ASSIGNER_ID);
//            Assert.AreEqual(historicIdentityLink.GroupId, null);
//            Assert.AreEqual(historicIdentityLink.OperationType, IDENTITY_LINK_ADD);
//            Assert.AreEqual(historicIdentityLink.ProcessDefinitionId, processInstance.ProcessDefinitionId);
//            Assert.AreEqual(historicIdentityLink.ProcessDefinitionKey, PROCESS_DEFINITION_KEY);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            var processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateGroup(taskId, A_GROUP_ID);

//            // Query test
//            var historicIdentityLink = historyService.CreateHistoricIdentityLinkLogQuery()
//                .First();
//            Assert.AreEqual(historicIdentityLink.UserId, null);
//            Assert.AreEqual(historicIdentityLink.TaskId, taskId);
//            Assert.AreEqual(historicIdentityLink.Type, IdentityLinkType.Candidate);
//            Assert.AreEqual(historicIdentityLink.AssignerId, A_ASSIGNER_ID);
//            Assert.AreEqual(historicIdentityLink.GroupId, A_GROUP_ID);
//            Assert.AreEqual(historicIdentityLink.OperationType, IDENTITY_LINK_ADD);
//            Assert.AreEqual(historicIdentityLink.ProcessDefinitionId, processInstance.ProcessDefinitionId);
//            Assert.AreEqual(historicIdentityLink.ProcessDefinitionKey, PROCESS_DEFINITION_KEY);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testValidIndividualQueryTaskCandidateForAddAndDeleteIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            var processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateUser(taskId, A_USER_ID);
//            taskService.DeleteCandidateUser(taskId, A_USER_ID);

//            // Valid Individual Query test
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.TaskId==taskId)
//                .Count(), 2);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Candidate)
//                .Count(), 2);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.UserId==A_USER_ID)
//                .Count(), 2);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.AssignerId==A_ASSIGNER_ID)
//                .Count(), 2);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 1);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 1);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==processInstance.ProcessDefinitionId)
//                .Count(), 2);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== PROCESS_DEFINITION_KEY)
//                .Count(), 2);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testValidGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            var processInstance = startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateUser(taskId, A_USER_ID);
//            taskService.DeleteCandidateUser(taskId, A_USER_ID);

//            // Valid group query test
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.TaskId==taskId)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.Type==IdentityLinkType.Candidate)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.UserId==A_USER_ID)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.AssignerId==A_ASSIGNER_ID)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==processInstance.ProcessDefinitionId)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== PROCESS_DEFINITION_KEY)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 1);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 1);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testInvalidIndividualQueryTaskCandidateForAddAndDeleteIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateUser(taskId, A_USER_ID);
//            taskService.DeleteCandidateUser(taskId, A_USER_ID);

//            // Invalid Individual Query test
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.TaskId==INVALID_TASK_ID)
//                .Count(), 0);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.Type==INVALID_IDENTITY_LINK_TYPE)
//                .Count(), 0);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.UserId==INVALID_USER_ID)
//                .Count(), 0);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.GroupId==INVALID_GROUP_ID)
//                .Count(), 0);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.AssignerId==INVALID_ASSIGNER_ID)
//                .Count(), 0);

//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.OperationType==INVALID_HISTORY_EVENT_TYPE)
//                .Count(), 0);
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testInvalidGroupQueryTaskCandidateForAddAndDeleteIdentityLink()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            taskService.AddCandidateUser(taskId, A_USER_ID);
//            taskService.DeleteCandidateUser(taskId, A_USER_ID);

//            // Invalid Individual Query test
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.TaskId==INVALID_TASK_ID)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.Type==INVALID_IDENTITY_LINK_TYPE)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.UserId==INVALID_USER_ID)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.GroupId==INVALID_GROUP_ID)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.AssignerId==INVALID_ASSIGNER_ID)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.OperationType==INVALID_HISTORY_EVENT_TYPE)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==INVALID_PROCESS_DEFINITION_ID)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== INVALID_PROCESS_DEFINITION_KEY)
//                .Count(), 0);
//        }

//        /// <summary>
//        ///     Should add 3 history records of identity link addition at 01-01-2016
//        ///     00:00.00 Should add 3 history records of identity link deletion at
//        ///     01-01-2016 12:00.00
//        ///     Should add 3 history records of identity link addition at 01-01-2016
//        ///     12:30.00 Should add 3 history records of identity link deletion at
//        ///     01-01-2016 21:00.00
//        ///     Test case: Query the number of added records at different time interval.
//        /// </summary>
//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testShouldAddTaskOwnerForAddandDeleteIdentityLinkByTimeStamp()
//        {
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);

//            // given
//            startProcessInstance(PROCESS_DEFINITION_KEY);
//            var taskId = taskService.CreateTaskQuery()
//                .First()
//                .Id;

//            // if
//            ClockUtil.CurrentTime = newYearMorning(0);
//            identityService.AuthenticatedUserId = A_ASSIGNER_ID;
//            // Adds aUserId1, deletes aUserID1, adds aUserId2, deletes aUserId2, Adds aUserId3 - 5
//            addUserIdentityLinks(taskId);

//            ClockUtil.CurrentTime = newYearNoon(0);
//            //Deletes aUserId3
//            deleteUserIdentityLinks(taskId);

//            ClockUtil.CurrentTime = newYearNoon(30);
//            addUserIdentityLinks(taskId);

//            ClockUtil.CurrentTime = newYearEvening();
//            deleteUserIdentityLinks(taskId);

//            // Query records with time before 12:20
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.DateBefore(newYearNoon(20))
//                .Count(), 6);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 3);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 3);

//            // Query records with time between 00:01 and 12:00
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.DateBefore(newYearNoon(0))
//                .Count(), 6);
//            Assert.AreEqual(query.DateAfter(newYearMorning(1))
//                .Count(), 1);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 1);

//            // Query records with time after 12:45
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.DateAfter(newYearNoon(45))
//                .Count(), 1);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 0);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 1);

//            ClockUtil.CurrentTime = DateTime.Now;
//        }

//        [Test]
//        [Deployment("resources/api/runtime/oneTaskProcess.bpmn20.xml")]
//        public virtual void testQueryAddAndRemoveIdentityLinksForProcessDefinition()
//        {
//            var latestProcessDef = repositoryService.CreateProcessDefinitionQuery(c=>c.Key ==PROCESS_DEFINITION_KEY)
//                .First();
//            Assert.NotNull(latestProcessDef);
//            var links = repositoryService.GetIdentityLinksForProcessDefinition(latestProcessDef.Id);
//            Assert.AreEqual(0, links.Count);

//            // Add candiate group with process definition
//            repositoryService.AddCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 1);
//            // Query test
//            var query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==latestProcessDef.Id)
//                .Count(), 1);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 1);
//            Assert.AreEqual(query.Where(c=>c.GroupId==GROUP_1)
//                .Count(), 1);

//            // Add candidate IUser for process definition
//            repositoryService.AddCandidateStarterUser(latestProcessDef.Id, USER_1);
//            // Query test
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==latestProcessDef.Id)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== latestProcessDef.Key)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_ADD)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.UserId==USER_1)
//                .Count(), 1);

//            // Delete candiate group with process definition
//            repositoryService.DeleteCandidateStarterGroup(latestProcessDef.Id, GROUP_1);
//            // Query test
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==latestProcessDef.Id)
//                .Count(), 3);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== latestProcessDef.Key)
//                .Count(), 3);
//            Assert.AreEqual(query.Where(c=>c.GroupId==GROUP_1)
//                .Count(), 2);
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 1);

//            // Delete candidate IUser for process definition
//            repositoryService.DeleteCandidateStarterUser(latestProcessDef.Id, USER_1);
//            // Query test
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionId==latestProcessDef.Id)
//                .Count(), 4);
//            Assert.AreEqual(query.Where(c=>c.ProcessDefinitionKey== latestProcessDef.Key)
//                .Count(), 4);
//            Assert.AreEqual(query.Where(c=>c.UserId==USER_1)
//                .Count(), 2);
//            query = historyService.CreateHistoricIdentityLinkLogQuery();
//            Assert.AreEqual(query.Where(c=>c.OperationType==IDENTITY_LINK_DELETE)
//                .Count(), 2);
//        }

//        [Test][Deployment( "resources/api/runtime/OneTaskProcessWithMultipleCandidateUser.bpmn20.xml" )]
//        public virtual void testHistoricIdentityLinkQueryPaging()
//        {
//            startProcessInstance(PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER);

//            var query = historyService.CreateHistoricIdentityLinkLogQuery();

//            Assert.AreEqual(4, query.ListPage(0, 4)
//                .Count());
//            Assert.AreEqual(1, query.ListPage(2, 1)
//                .Count());
//            Assert.AreEqual(2, query.ListPage(1, 2)
//                .Count());
//            Assert.AreEqual(3, query.ListPage(1, 4)
//                .Count());
//        }

//        [Test]
//        [Deployment("resources/api/runtime/OneTaskProcessWithMultipleCandidateUser.bpmn20.xml")]
//        public virtual void testHistoricIdentityLinkQuerySorting()
//        {
//            // Pre test - Historical identity link is added as part of deployment
//            var historicIdentityLinks = historyService.CreateHistoricIdentityLinkLogQuery()
                
//                .ToList();
//            Assert.AreEqual(historicIdentityLinks.Count, 0);
//            startProcessInstance(PROCESS_DEFINITION_KEY_MULTIPLE_CANDIDATE_USER);

//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByAssignerId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByTime()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByGroupId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByType()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByOperationType()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                //.OrderByTaskId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                /*.OrderByTenantId()*/
//                /*.Asc()*/
                
//                .Count());
//            Assert.AreEqual("aUser", historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Asc()*/
                
//                .First().UserId);
//            Assert.AreEqual("dUser", historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Asc()*/
                
//                .ToList()[3].UserId);

//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByAssignerId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByTime()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByGroupId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByType()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByOperationType()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                /*.OrderByProcessDefinitionId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                //.OrderByProcessDefinitionKey()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                //.OrderByTaskId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual(4, historyService.CreateHistoricIdentityLinkLogQuery()
//                /*.OrderByTenantId()*/
//                /*.Desc()*/
                
//                .Count());
//            Assert.AreEqual("dUser", historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Desc()*/
                
//                .First().UserId);
//            Assert.AreEqual("aUser", historyService.CreateHistoricIdentityLinkLogQuery()
//                .OrderByUserId()
//                /*.Desc()*/
                
//                .ToList()[3].UserId);
//        }

//        public virtual void addUserIdentityLinks(string taskId)
//        {
//            for (var userIndex = 1; userIndex <= numberOfUsers; userIndex++)
//                taskService.AddUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.Assignee);
//        }

//        public virtual void deleteUserIdentityLinks(string taskId)
//        {
//            for (var userIndex = 1; userIndex <= numberOfUsers; userIndex++)
//                taskService.DeleteUserIdentityLink(taskId, A_USER_ID + userIndex, IdentityLinkType.Assignee);
//        }

//        public virtual DateTime newYearMorning(int minutes)
//        {
//            var calendar = new DateTime();
//            //calendar.Set(DateTime.YEAR, 2016);
//            //calendar.Set(DateTime.MONTH, 0);
//            //calendar.Set(DateTime.DAY_OF_MONTH, 1);
//            //calendar.Set(DateTime.HOUR_OF_DAY, 0);
//            //calendar.Set(DateTime.MINUTE, minutes);
//            //calendar.Set(DateTime.SECOND, 0);
//            //calendar.Set(DateTime.MILLISECOND, 0);
//            var morning = calendar;
//            return morning;
//        }

//        public virtual DateTime newYearNoon(int minutes)
//        {
//            var calendar = new DateTime(); //new GregorianCalendar());
//            //calendar.Set(DateTime.YEAR, 2016);
//            //calendar.Set(DateTime.MONTH, 0);
//            //calendar.Set(DateTime.DAY_OF_MONTH, 1);
//            //calendar.Set(DateTime.HOUR_OF_DAY, 12);
//            //calendar.Set(DateTime.MINUTE, minutes);
//            //calendar.Set(DateTime.SECOND, 0);
//            //calendar.Set(DateTime.MILLISECOND, 0);
//            var morning = calendar;
//            return morning;
//        }

//        public virtual DateTime newYearEvening()
//        {
//            var calendar = new DateTime(); //new GregorianCalendar());
//            //          calendar.Set(DateTime.YEAR, 2016);
//            //calendar.Set(DateTime.MONTH, 0);
//            //calendar.Set(DateTime.DAY_OF_MONTH, 1);
//            //calendar.Set(DateTime.HOUR_OF_DAY, 21);
//            //calendar.Set(DateTime.MINUTE, 0);
//            //calendar.Set(DateTime.SECOND, 0);
//            //calendar.Set(DateTime.MILLISECOND, 0);
//            var morning = calendar;
//            return morning;
//        }

//        protected internal virtual IProcessInstance startProcessInstance(string key)
//        {
//            return runtimeService.StartProcessInstanceByKey(key);
//        }
//    }
//}