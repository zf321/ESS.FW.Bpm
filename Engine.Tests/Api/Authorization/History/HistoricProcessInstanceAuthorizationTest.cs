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
    public class HistoricProcessInstanceAuthorizationTest : AuthorizationTest
    {

        protected internal const string PROCESS_KEY = "oneTaskProcess";
        protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/messageStartEventProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic process instance query //////////////////////////////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IHistoricProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);

            IHistoricProcessInstance instance = query.First();
            Assert.NotNull(instance);
            Assert.AreEqual(ProcessInstanceId, instance.Id);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            var id = StartProcessInstanceByKey(PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic process instance query (multiple process instances) ////////////////////////

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
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

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
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

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
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 7);
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
            IQueryable<IHistoricProcessInstance> query = historyService.CreateHistoricProcessInstanceQuery();

            // then
            //verifyQueryResults(query, 3);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery().ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // Delete historic process instance //////////////////////////////

        public virtual void testDeleteHistoricProcessInstanceWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            try
            {
                // when
                historyService.DeleteHistoricProcessInstance(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to Delete the historic process instance");
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

        public virtual void testDeleteHistoricProcessInstanceWithDeleteHistoryPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.DeleteHistory);

            // when
            historyService.DeleteHistoricProcessInstance(ProcessInstanceId);

            // then
            disableAuthorization();
            long Count = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId).Count();
            Assert.AreEqual(0, Count);
            enableAuthorization();
        }

        public virtual void testDeleteHistoricProcessInstanceWithDeleteHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.DeleteHistory);

            // when
            historyService.DeleteHistoricProcessInstance(ProcessInstanceId);

            // then
            disableAuthorization();
            long Count = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId).Count();
            Assert.AreEqual(0, Count);
            enableAuthorization();
        }

        public virtual void testDeleteHistoricProcessInstanceAfterDeletingDeployment()
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
            historyService.DeleteHistoricProcessInstance(ProcessInstanceId);

            // then
            disableAuthorization();
            long Count = historyService.CreateHistoricProcessInstanceQuery(c=>c.ProcessInstanceId == ProcessInstanceId).Count();
            Assert.AreEqual(0, Count);
            enableAuthorization();
        }

        // create historic process instance report

        public virtual void testHistoricProcessInstanceReportWithoutAuthorization()
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
                historyService.CreateHistoricProcessInstanceReport().Duration(PeriodUnit.Month);
                Assert.Fail("Exception expected: It should not be possible to create a historic process instance report");
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

        public virtual void testHistoricProcessInstanceReportWithHistoryReadPermissionOnAny()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IList<IDurationReportResult> result = historyService.CreateHistoricProcessInstanceReport().Duration(PeriodUnit.Month);

            // then
            Assert.AreEqual(1, result.Count);
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricProcessInstance> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}