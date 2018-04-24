using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class HistoricIncidentAuthorizationTest : AuthorizationTest
    {

        protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
        protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";
        protected internal const string ANOTHER_ONE_INCIDENT_PROCESS_KEY = "anotherOneIncidentProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/authorization/timerStartEventProcess.bpmn20.xml", "resources/api/authorization/oneIncidentProcess.bpmn20.xml", "resources/api/authorization/anotherOneIncidentProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // historic incident query (standalone) //////////////////////////////

        public virtual void testQueryForStandaloneHistoricIncidents()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string jobId = null;
            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery().ToList();
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
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);

            disableAuthorization();
            managementService.DeleteJob(jobId);
            enableAuthorization();

            clearDatabase();
        }

        // historic incident query (start timer job incident) //////////////////////////////

        public virtual void testStartTimerJobIncidentQueryWithoutAuthorization()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testStartTimerJobIncidentQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testStartTimerJobIncidentQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

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

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic incident query ///////////////////////////////////////////

        public virtual void testSimpleQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testSimpleQueryWithMultiple()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // historic incident query (multiple incidents ) ///////////////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 5);
        }

        // historic job log (mixed) //////////////////////////////////////////

        public virtual void testMixedQueryWithoutAuthorization()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string firstJobId = null;
            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    firstJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(firstJobId, 0);

            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string secondJobId = null;
            jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    secondJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(secondJobId, 0);
            enableAuthorization();

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 2);

            disableAuthorization();
            managementService.DeleteJob(firstJobId);
            managementService.DeleteJob(secondJobId);
            enableAuthorization();

            clearDatabase();
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string firstJobId = null;
            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    firstJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(firstJobId, 0);

            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string secondJobId = null;
            jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    secondJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(secondJobId, 0);
            enableAuthorization();

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 4);

            disableAuthorization();
            managementService.DeleteJob(firstJobId);
            managementService.DeleteJob(secondJobId);
            enableAuthorization();

            clearDatabase();
        }

        public virtual void testMixedQueryWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string firstJobId = null;
            IList<ESS.FW.Bpm.Engine.Runtime.IJob> jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    firstJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(firstJobId, 0);

            repositoryService.SuspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
            string secondJobId = null;
            jobs = managementService.CreateJobQuery(c=>c.Retries > 0).ToList();
            foreach (ESS.FW.Bpm.Engine.Runtime.IJob job in jobs)
            {
                if (job.ProcessDefinitionKey == null)
                {
                    secondJobId = job.Id;
                    break;
                }
            }
            managementService.SetJobRetries(secondJobId, 0);
            enableAuthorization();

            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
            startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IHistoricIncident> query = historyService.CreateHistoricIncidentQuery();

            // then
            //verifyQueryResults(query, 7);

            disableAuthorization();
            managementService.DeleteJob(firstJobId);
            managementService.DeleteJob(secondJobId);
            enableAuthorization();

            clearDatabase();
        }

        // helper ////////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IHistoricIncident> query, int countExpected)
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
            private readonly HistoricIncidentAuthorizationTest outerInstance;

            public CommandAnonymousInnerClass(HistoricIncidentAuthorizationTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
                IList<IHistoricIncident> incidents = ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration.HistoryService.CreateHistoricIncidentQuery().ToList();
                foreach (IHistoricIncident incident in incidents)
                {
                    (commandContext.HistoricIncidentManager as HistoricIncidentManager).Delete((HistoricIncidentEntity)incident);
                }
                return null;
            }
        }

    }

}