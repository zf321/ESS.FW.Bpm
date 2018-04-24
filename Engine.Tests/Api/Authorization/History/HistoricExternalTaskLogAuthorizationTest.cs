using System.Linq;
using Engine.Tests.Api.Runtime.Migration.Models.Builder;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricExternalTaskLogAuthorizationTest : AuthorizationTest
    {

        protected internal readonly string WORKER_ID = "aWorkerId";
        protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;
        protected internal readonly string ERROR_DETAILS = "These are the error details!";
        protected internal readonly string ANOTHER_PROCESS_KEY = "AnotherProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {

            IDeploymentBuilder deploymentbuilder = repositoryService.CreateDeployment();
            IBpmnModelInstance defaultModel = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().Build();
            IBpmnModelInstance modifiedModel = DefaultExternalTaskModelBuilder.CreateDefaultExternalTaskModel().ProcessKey(ANOTHER_PROCESS_KEY).Build();
            deploymentId = Deployment(deploymentbuilder, defaultModel, modifiedModel);

            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }


        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithHistoryReadPermissionOnProcessDefinition()
        {

            // given
            StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);
            createGrantAuthorization(Resources.ProcessDefinition, DefaultExternalTaskModelBuilder.DefaultProcessKey, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithHistoryReadPermissionOnAnyProcessDefinition()
        {

            // given
            StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithMultipleAuthorizations()
        {
            // given
            StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, DefaultExternalTaskModelBuilder.DefaultProcessKey, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithHistoryReadPermissionOnOneProcessDefinition()
        {
            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
            createGrantAuthorization(Resources.ProcessDefinition, DefaultExternalTaskModelBuilder.DefaultProcessKey, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 6);
        }

        public virtual void testQueryWithHistoryReadPermissionOnAnyProcessDefinition()
        {
            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricExternalTaskLog> query = historyService.CreateHistoricExternalTaskLogQuery();

            // then
            //verifyQueryResults(query, 8);
        }

        public virtual void testGetErrorDetailsWithoutAuthorization()
        {
            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();

            disableAuthorization();
            string failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State== ExternalTaskStateFields.Failed.StateCode.ToString()).First().Id;
            enableAuthorization();

            try
            {
                // when
                string stacktrace = historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);
                Assert.Fail("Exception expected: It should not be possible to retrieve the error details");
            }
            catch (AuthorizationException e)
            {
                // then
                string exceptionMessage = e.Message;
                AssertTextPresent(userId, exceptionMessage);
                AssertTextPresent(Permissions.ReadHistory.ToString(), exceptionMessage);
                AssertTextPresent(DefaultExternalTaskModelBuilder.DefaultProcessKey, exceptionMessage);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, exceptionMessage);
            }
        }

        public virtual void testGetErrorDetailsWithHistoryReadPermissionOnProcessDefinition()
        {

            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
            createGrantAuthorization(Resources.ProcessDefinition, DefaultExternalTaskModelBuilder.DefaultProcessKey, userId, Permissions.ReadHistory);

            string failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State== ExternalTaskStateFields.Failed.StateCode.ToString()).First().Id;

            // when
            string stacktrace = historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

            // then
            Assert.NotNull(stacktrace);
            Assert.AreEqual(ERROR_DETAILS, stacktrace);
        }

        public virtual void testGetErrorDetailsWithHistoryReadPermissionOnProcessAnyDefinition()
        {

            // given
            startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure();
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            string failedHistoricExternalTaskLogId = historyService.CreateHistoricExternalTaskLogQuery(c=>c.State== ExternalTaskStateFields.Failed.StateCode.ToString()).First().Id;

            // when
            string stacktrace = historyService.GetHistoricExternalTaskLogErrorDetails(failedHistoricExternalTaskLogId);

            // then
            Assert.NotNull(stacktrace);
            Assert.AreEqual(ERROR_DETAILS, stacktrace);
        }

        protected internal virtual void startThreeProcessInstancesDeleteOneAndCompleteTwoWithFailure()
        {
            disableAuthorization();
            IProcessInstance pi1 = StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);
            IProcessInstance pi2 = StartProcessInstanceByKey(DefaultExternalTaskModelBuilder.DefaultProcessKey);
            IProcessInstance pi3 = StartProcessInstanceByKey(ANOTHER_PROCESS_KEY);

            completeExternalTaskWithFailure(pi1);
            completeExternalTaskWithFailure(pi2);

            runtimeService.DeleteProcessInstance(pi3.Id, "Dummy reason for deletion!");
            enableAuthorization();
        }

        protected internal virtual void completeExternalTaskWithFailure(IProcessInstance pi)
        {
            ESS.FW.Bpm.Engine.Externaltask.IExternalTask task = externalTaskService.CreateExternalTaskQuery(c=>c.ProcessInstanceId == pi.Id).First();
            completeExternalTaskWithFailure(task.Id);
        }

        protected internal virtual void completeExternalTaskWithFailure(string externalTaskId)
        {
            //IList<ILockedExternalTask> list = externalTaskService.FetchAndLock(5, WORKER_ID, false)/*.Topic(DefaultExternalTaskModelBuilder.DefaultTopic, LOCK_DURATION)*/.Execute();
            //externalTaskService.HandleFailure(externalTaskId, WORKER_ID, "This is an error!", ERROR_DETAILS, 1, 0L);
            //externalTaskService.Complete(externalTaskId, WORKER_ID);
            //// unlock the remaining tasks
            //foreach (ILockedExternalTask lockedExternalTask in list)
            //{
            //    if (!lockedExternalTask.Id.Equals(externalTaskId))
            //    {
            //        externalTaskService.Unlock(lockedExternalTask.Id);
            //    }
            //}
        }

    }

}