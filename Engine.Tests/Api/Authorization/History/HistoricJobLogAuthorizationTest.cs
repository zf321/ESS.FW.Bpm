using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricJobLogAuthorizationTest : AuthorizationTest
    {

        protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
        protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";
        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/timerStartEventProcess.bpmn20.xml", "resources/api/authorization/timerBoundaryEventProcess.bpmn20.xml", "resources/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));
            DeleteDeployment(deploymentId);
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly HistoricJobLogAuthorizationTest outerInstance;

            public CommandAnonymousInnerClass(HistoricJobLogAuthorizationTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
                return null;
            }
        }

        // historic job log query (start timer job) ////////////////////////////////

        public virtual void testStartTimerJobLogQueryWithoutAuthorization()
        {
            // given

            // when

            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testStartTimerJobLogQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testStartTimerJobLogQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic job log query ////////////////////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithHistoryReadPermissionOnProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 4);
        }

        public virtual void testSimpleQueryWithHistoryReadPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 5);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 5);
        }

        // historic job log query (multiple process instance) ////////////////////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobId = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            jobId = managementService.CreateJobQuery(c => c.ProcessDefinitionKey == TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            enableAuthorization();

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithHistoryReadPermissionOnProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobId = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey==TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            jobId = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 12);
        }

        public virtual void testQueryWithHistoryReadPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobId = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            jobId = managementService.CreateJobQuery(c=>c.ProcessDefinitionKey == TIMER_START_PROCESS_KEY).First().Id;
            managementService.ExecuteJob(jobId);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 17);
        }

        // historic job log query (standalone job) ///////////////////////

        public virtual void testQueryAfterStandaloneJob()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY, true, DateTime.Now);
            enableAuthorization();

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 1);

            IHistoricJobLog jobLog = query.First();
            Assert.IsNull(jobLog.ProcessDefinitionKey);

            DeleteDeployment(deploymentId);

            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.DeleteJob(jobId);
            enableAuthorization();
        }

        // Delete deployment (cascade = false)

        public virtual void testQueryAfterDeletingDeployment()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.ReadHistory);

            disableAuthorization();
            IList<ITask> tasks = taskService.CreateTaskQuery()
                .ToList();
                foreach (ITask task in tasks)
            {
                taskService.Complete(task.Id);
            }
            enableAuthorization();

            disableAuthorization();
            repositoryService.DeleteDeployment(deploymentId);
            enableAuthorization();

            // when
            IQueryable<IHistoricJobLog> query = historyService.CreateHistoricJobLogQuery();

            // then
            //verifyQueryResults(query, 6);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery()
                .ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // get historic job log exception stacktrace (standalone) /////////////////////

        public virtual void testGetHistoricStandaloneJobLogExceptionStacktrace()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY, true, DateTime.Now);
            enableAuthorization();
            string jobLogId = historyService.CreateHistoricJobLogQuery().First().Id;

            // when
            string stacktrace = historyService.GetHistoricJobLogExceptionStacktrace(jobLogId);

            // then
            Assert.IsNull(stacktrace);

            DeleteDeployment(deploymentId);

            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.DeleteJob(jobId);
            enableAuthorization();
        }

        // get historic job log exception stacktrace /////////////////////

        public virtual void testGetHistoricJobLogExceptionStacktraceWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobLogId = historyService.CreateHistoricJobLogQuery()/*.FailureLog()/*.ListPage(0, 1)*/.First().Id;
            enableAuthorization();

            try
            {
                // when
                historyService.GetHistoricJobLogExceptionStacktrace(jobLogId);
                Assert.Fail("Exception expected: It should not be possible to get the historic job log exception stacktrace");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.ReadHistory.ToString(), message);
                AssertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetHistoricJobLogExceptionStacktraceWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobLogId = historyService.CreateHistoricJobLogQuery()/*.FailureLog()/*.ListPage(0, 1)*/.First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            string stacktrace = historyService.GetHistoricJobLogExceptionStacktrace(jobLogId);

            // then
            Assert.NotNull(stacktrace);
        }

        public virtual void testGetHistoricJobLogExceptionStacktraceWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            disableAuthorization();
            string jobLogId = historyService.CreateHistoricJobLogQuery()/*.FailureLog()/*.ListPage(0, 1)*/.First().Id;
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            string stacktrace = historyService.GetHistoricJobLogExceptionStacktrace(jobLogId);

            // then
            Assert.NotNull(stacktrace);
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricJobLog> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

    }

}