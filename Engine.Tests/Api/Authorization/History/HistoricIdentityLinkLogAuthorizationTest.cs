using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricIdentityLinkLogAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_PROCESS_KEY = "demoAssigneeProcess";
        protected internal const string CASE_KEY = "oneTaskCase";
        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/oneTaskProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic identity link query (standalone task) - Authorization

        public virtual void testQueryForStandaloneTaskHistoricIdentityLinkWithoutAuthrorization()
        {
            // given
            disableAuthorization();

            ITask taskAssignee = taskService.NewTask("NewTask");
            taskAssignee.Assignee = "aUserId";
            taskService.SaveTask(taskAssignee);

            enableAuthorization();

            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 1);

            disableAuthorization();
            taskService.DeleteTask("NewTask", true);
            enableAuthorization();
        }

        public virtual void testQueryForTaskHistoricIdentityLinkWithoutUserPermission()
        {
            // given
            disableAuthorization();
            StartProcessInstanceByKey(ONE_PROCESS_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // if
            identityService.AuthenticatedUserId = "aAssignerId";
            taskService.AddCandidateUser(taskId, "aUserId");

            enableAuthorization();

            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 0);
        }

        public virtual void testQueryForTaskHistoricIdentityLinkWithUserPermission()
        {
            // given
            disableAuthorization();
            StartProcessInstanceByKey(ONE_PROCESS_KEY);

            // if
            createGrantAuthorization(Resources.ProcessDefinition, ONE_PROCESS_KEY, userId, Permissions.ReadHistory);

            enableAuthorization();
            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            disableAuthorization();
            StartProcessInstanceByKey(ONE_PROCESS_KEY);

            // if
            createGrantAuthorization(Resources.ProcessDefinition, ONE_PROCESS_KEY, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            enableAuthorization();
            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testQueryCaseTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);
            string taskId = taskService.CreateTaskQuery().First().Id;

            // if
            identityService.AuthenticatedUserId = "aAssignerId";
            taskService.AddCandidateUser(taskId, "aUserId");
            enableAuthorization();

            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 1);
        }

        public virtual void testMixedQuery()
        {

            disableAuthorization();
            // given
            StartProcessInstanceByKey(ONE_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_PROCESS_KEY);
            StartProcessInstanceByKey(ONE_PROCESS_KEY);

            createCaseInstanceByKey(CASE_KEY);
            taskService.AddCandidateUser(taskService.CreateTaskQuery().ToList().ElementAt(3).Id, "dUserId");
            createCaseInstanceByKey(CASE_KEY);
            taskService.AddCandidateUser(taskService.CreateTaskQuery().ToList().ElementAt(4).Id, "eUserId");

            createTaskAndAssignUser("one");
            createTaskAndAssignUser("two");
            createTaskAndAssignUser("three");
            createTaskAndAssignUser("four");
            createTaskAndAssignUser("five");

            enableAuthorization();

            // when
            IQueryable<IHistoricIdentityLinkLog> query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 7);

            disableAuthorization();

            query = historyService.CreateHistoricIdentityLinkLogQuery();
            // then
            ////verifyQueryResults(query, 10);

            // if
            createGrantAuthorization(Resources.ProcessDefinition, ONE_PROCESS_KEY, userId, Permissions.ReadHistory);
            enableAuthorization();
            query = historyService.CreateHistoricIdentityLinkLogQuery();

            // then
            ////verifyQueryResults(query, 10);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        public virtual void createTaskAndAssignUser(string taskId)
        {
            ITask task = taskService.NewTask(taskId);
            task.Assignee = "demo";
            taskService.SaveTask(task);
        }

    }

}