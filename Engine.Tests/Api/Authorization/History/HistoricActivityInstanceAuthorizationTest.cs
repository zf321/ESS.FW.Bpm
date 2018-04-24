using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    public class HistoricActivityInstanceAuthorizationTest : AuthorizationTest
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

        // historic activity instance query /////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);

            // when
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testSimpleQueryMultiple()
        {
            // given
            StartProcessInstanceByKey(PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        // historic activity instance query (multiple process instances) ////////////////////////

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
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

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
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 6);
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
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 14);
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
            IQueryable<IHistoricActivityInstance> query = historyService.CreateHistoricActivityInstanceQuery();

            // then
            //verifyQueryResults(query, 9);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery().ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricActivityInstance> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}