using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    public class IncidentAuthorizationTest : AuthorizationTest
    {

        protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";
        protected internal const string ANOTHER_ONE_INCIDENT_PROCESS_KEY = "anotherOneIncidentProcess";

        protected internal string deploymentId;

        [SetUp]
        public  void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/timerStartEventProcess.bpmn20.xml", "resources/api/authorization/oneIncidentProcess.bpmn20.xml", "resources/api/authorization/anotherOneIncidentProcess.bpmn20.xml").Id;
            base.setUp();
        }

        [TearDown]
        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        public virtual void testQueryForStandaloneIncidents()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string jobId = null;
            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery()
                .ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    jobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            disableAuthorization();
            managementService.DeleteJob(jobId);
            enableAuthorization();

            clearDatabase();
        }

        public virtual void testStartTimerJobIncidentQueryWithoutAuthorization()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testStartTimerJobIncidentQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testStartTimerJobIncidentQueryWithReadInstancePermissionOnProcessDefinition()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testStartTimerJobIncidentQueryWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            IIncident incident = query.First();
            Assert.NotNull(incident);
            Assert.AreEqual(ProcessInstanceId, incident.ProcessInstanceId);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 3);
        }

        public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<IIncident> query = runtimeService.CreateIncidentQuery();

            // then
            //verifyQueryResults(query, 7);
        }

        protected internal virtual void verifyQueryResults(IQueryable<IIncident> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual void clearDatabase()
        {
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly IncidentAuthorizationTest outerInstance;

            public CommandAnonymousInnerClass(IncidentAuthorizationTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                IHistoryLevel historyLevel = ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration.HistoryLevel;
                if (historyLevel.Equals(HistoryLevelFields.HistoryLevelFull))
                {
                    commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
                    IList<IHistoricIncident> incidents =
                            ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration.HistoryService.CreateHistoricIncidentQuery()
                                .ToList();
                        ;foreach (IHistoricIncident incident in incidents)
                    {
                        commandContext.HistoricIncidentManager
                            .Delete((HistoricIncidentEntity)incident);
                    }
                }

                return null;
            }
        }

    }

}