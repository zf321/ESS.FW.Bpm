using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    public class HistoricTaskInstanceAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
        protected internal const string CASE_KEY = "oneTaskCase";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/messageStartEventProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic task instance query (standalone task) ///////////////////////////////////////

        public virtual void testQueryAfterStandaloneTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            deleteTask(taskId, true);
        }

        // historic task instance query (process task) //////////////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic task instance query (multiple process instances) ////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        // historic task instance query (case task) ///////////////////////////////////////

        public virtual void testQueryAfterCaseTask()
        {
            // given
            createCaseInstanceByKey(CASE_KEY);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic task instance query (mixed tasks) ////////////////////////////////////

        public virtual void testMixedQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            createCaseInstanceByKey(CASE_KEY);
            createCaseInstanceByKey(CASE_KEY);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 7);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            createCaseInstanceByKey(CASE_KEY);
            createCaseInstanceByKey(CASE_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 10);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);

            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
            StartProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

            createTask("one");
            createTask("two");
            createTask("three");
            createTask("four");
            createTask("five");

            createCaseInstanceByKey(CASE_KEY);
            createCaseInstanceByKey(CASE_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 14);

            deleteTask("one", true);
            deleteTask("two", true);
            deleteTask("three", true);
            deleteTask("four", true);
            deleteTask("five", true);
        }

        // Delete deployment (cascade = false)

        public virtual void testQueryAfterDeletingDeployment()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery().ToList();
            foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }
            enableAuthorization();

            disableAuthorization();
            repositoryService.DeleteDeployment(deploymentId);
            enableAuthorization();

            // when
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery();

            // then
            //verifyQueryResults(query, 3);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery()
                .ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // Delete historic task (standalone task) ///////////////////////

        public virtual void testDeleteStandaloneTask()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            // when
            historyService.DeleteHistoricTaskInstance(p => p.TaskId == taskId);

            // then
            disableAuthorization();
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.Id == taskId);
            //verifyQueryResults(query, 0);
            enableAuthorization();

            deleteTask(taskId, true);
        }

        // Delete historic task (process task) ///////////////////////

        public virtual void testDeleteProcessTaskWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            try
            {
                // when
                historyService.DeleteHistoricTaskInstance(taskId);
                Assert.Fail("Exception expected: It should not be possible to Delete the historic task instance");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.DeleteHistory.ToString(), message);
                AssertTextPresent(PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testDeleteProcessTaskWithDeleteHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.DeleteHistory);

            // when
            historyService.DeleteHistoricTaskInstance(taskId);

            // then
            disableAuthorization();
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.Id == taskId);
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testDeleteProcessTaskWithDeleteHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.DeleteHistory);

            // when
            historyService.DeleteHistoricTaskInstance(taskId);

            // then
            disableAuthorization();
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.Id == taskId);
            //verifyQueryResults(query, 0);
            enableAuthorization();
        }

        public virtual void testDeleteHistoricTaskInstanceAfterDeletingDeployment()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.DeleteHistory);

            disableAuthorization();
            repositoryService.DeleteDeployment(deploymentId);
            enableAuthorization();

            // when
            historyService.DeleteHistoricTaskInstance(taskId);

            // then
            disableAuthorization();
            IQueryable<IHistoricTaskInstance> query = historyService.CreateHistoricTaskInstanceQuery(c=>c.Id == taskId);
            //verifyQueryResults(query, 0);
            enableAuthorization();

            disableAuthorization();
            historyService.DeleteHistoricProcessInstance(ProcessInstanceId);
            enableAuthorization();
        }

        public virtual void testHistoricTaskInstanceReportWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            try
            {
                // when
                historyService.CreateHistoricTaskInstanceReport().Duration(PeriodUnit.Month);
                Assert.Fail("Exception expected: It should not be possible to create a historic task instance report");
            }
            catch (AuthorizationException e)
            {
                // then
                IList<MissingAuthorization> missingAuthorizations = e.MissingAuthorizations;
                Assert.AreEqual(1, missingAuthorizations.Count);

                MissingAuthorization missingAuthorization = missingAuthorizations[0];
                Assert.AreEqual(Permissions.ReadHistory.ToString(), missingAuthorization.ViolatedPermissionName);
                Assert.AreEqual(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, missingAuthorization.ResourceType);
                Assert.AreEqual(AuthorizationFields.Any, missingAuthorization.ResourceId);
            }
        }

        public virtual void testHistoricTaskInstanceReportWithHistoryReadPermissionOnAny()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IList<IDurationReportResult> result = historyService.CreateHistoricTaskInstanceReport().Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(1, result.Count);
        }

        public virtual void testHistoricTaskInstanceReportGroupedByProcessDefinitionKeyWithHistoryReadPermissionOnAny()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IList<IHistoricTaskInstanceReportResult> result = historyService.CreateHistoricTaskInstanceReport().CountByProcessDefinitionKey();

            // then
            Assert.AreEqual(1, result.Count);
        }

        public virtual void testHistoricTaskInstanceReportGroupedByTaskNameWithHistoryReadPermissionOnAny()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.Task, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IList<IHistoricTaskInstanceReportResult> result = historyService.CreateHistoricTaskInstanceReport().CountByTaskName();

            // then
            Assert.AreEqual(1, result.Count);
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricTaskInstance> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}