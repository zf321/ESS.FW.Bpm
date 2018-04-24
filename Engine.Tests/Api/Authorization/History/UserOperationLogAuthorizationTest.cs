using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.History
{

    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
    public class UserOperationLogAuthorizationTest : AuthorizationTest
    {

        protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
        protected internal const string ONE_TASK_CASE_KEY = "oneTaskCase";
        protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

        protected internal string deploymentId;

        [SetUp]
        public void setUp()
        {
            deploymentId = createDeployment(null, "resources/api/oneTaskProcess.bpmn20.xml", "resources/api/authorization/oneTaskCase.cmmn", "resources/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
            base.setUp();
        }

        public void tearDown()
        {
            base.TearDown();
            DeleteDeployment(deploymentId);
        }

        // standalone task ///////////////////////////////

        public virtual void testQueryCreateStandaloneTaskUserOperationLog()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            deleteTask(taskId, true);
        }

        public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLog()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);
            setAssignee(taskId, "demo");

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 2);

            deleteTask(taskId, true);
        }

        // (process) user task /////////////////////////////

        public virtual void testQuerySetAssigneeTaskUserOperationLogWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQuerySetAssigneeTaskUserOperationLogWithMultiple()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // (case) human task /////////////////////////////

        public virtual void testQuerySetAssigneeHumanTaskUserOperationLog()
        {
            // given
            createCaseInstanceByKey(ONE_TASK_CASE_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // standalone job ///////////////////////////////

        public virtual void testQuerySetStandaloneJobRetriesUserOperationLog()
        {
            // given
            disableAuthorization();
            repositoryService.SuspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
            enableAuthorization();

            disableAuthorization();
            string jobId = managementService.CreateJobQuery().First().Id;
            managementService.SetJobRetries(jobId, 5);
            enableAuthorization();

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            disableAuthorization();
            managementService.DeleteJob(jobId);
            enableAuthorization();

            clearDatabase();
        }

        // job ///////////////////////////////

        public virtual void testQuerySetJobRetriesUserOperationLogWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string jobId = selectSingleJob().Id;

            disableAuthorization();
            managementService.SetJobRetries(jobId, 5);
            enableAuthorization();

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string jobId = selectSingleJob().Id;

            disableAuthorization();
            managementService.SetJobRetries(jobId, 5);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string jobId = selectSingleJob().Id;

            disableAuthorization();
            managementService.SetJobRetries(jobId, 5);
            enableAuthorization();

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        // process definition ////////////////////////////////////////////

        public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithoutAuthorization()
        {
            // given
            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 0);

            clearDatabase();
        }

        public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            clearDatabase();
        }

        public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            clearDatabase();
        }

        // process instance //////////////////////////////////////////////

        public virtual void testQuerySuspendProcessInstanceUserOperationLogWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 0);

            clearDatabase();
        }

        public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            clearDatabase();
        }

        public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            suspendProcessInstanceById(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadHistory);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 1);

            clearDatabase();
        }

        // Delete deployment (cascade = false)

        public virtual void testQueryAfterDeletingDeployment()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory);

            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            DeleteDeployment(deploymentId, false);

            // when
            IQueryable<IUserOperationLogEntry> query = historyService.CreateUserOperationLogQuery();

            // then
            //verifyQueryResults(query, 2);

            disableAuthorization();
            IList<IHistoricProcessInstance> instances = historyService.CreateHistoricProcessInstanceQuery()
                .ToList();
            foreach (IHistoricProcessInstance instance in instances)
            {
                historyService.DeleteHistoricProcessInstance(instance.Id);
            }
            enableAuthorization();
        }

        // Delete user operation log (standalone) ////////////////////////

        public virtual void testDeleteStandaloneEntry()
        {
            // given
            string taskId = "myTask";
            createTask(taskId);

            string entryId = historyService.CreateUserOperationLogQuery().First().Id;

            // when
            historyService.DeleteUserOperationLogEntry(entryId);

            // then
            Assert.IsNull(historyService.CreateUserOperationLogQuery().First());

            deleteTask(taskId, true);
        }

        // Delete user operation log /////////////////////////////////////

        public virtual void testDeleteEntryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            disableAuthorization();
            string entryId = historyService.CreateUserOperationLogQuery().First().Id;
            enableAuthorization();

            try
            {
                // when
                historyService.DeleteUserOperationLogEntry(entryId);
                Assert.Fail("Exception expected: It should not be possible to Delete the user operation log");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.DeleteHistory.ToString(), message);
                AssertTextPresent(ONE_TASK_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testDeleteEntryWithDeleteHistoryPermissionOnProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.DeleteHistory);

            disableAuthorization();
            string entryId = historyService.CreateUserOperationLogQuery().First().Id;
            enableAuthorization();

            // when
            historyService.DeleteUserOperationLogEntry(entryId);

            // then
            disableAuthorization();
            Assert.IsNull(historyService.CreateUserOperationLogQuery().First());
            enableAuthorization();
        }

        public virtual void testDeleteEntryWithDeleteHistoryPermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.DeleteHistory);

            disableAuthorization();
            string entryId = historyService.CreateUserOperationLogQuery().First().Id;
            enableAuthorization();

            // when
            historyService.DeleteUserOperationLogEntry(entryId);

            // then
            disableAuthorization();
            Assert.IsNull(historyService.CreateUserOperationLogQuery().First());
            enableAuthorization();
        }

        public virtual void testDeleteEntryAfterDeletingDeployment()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
            string taskId = selectSingleTask().Id;
            createGrantAuthorization(Resources.ProcessDefinition, ONE_TASK_PROCESS_KEY, userId, Permissions.ReadHistory, Permissions.DeleteHistory);

            disableAuthorization();
            taskService.Complete(taskId);
            enableAuthorization();

            DeleteDeployment(deploymentId, false);

            string entryId = historyService.CreateUserOperationLogQuery().First().Id;

            // when
            historyService.DeleteUserOperationLogEntry(entryId);

            // then
            disableAuthorization();
            Assert.IsNull(historyService.CreateUserOperationLogQuery().First());
            enableAuthorization();

            disableAuthorization();
            historyService.DeleteHistoricProcessInstance(ProcessInstanceId);
            enableAuthorization();
        }

        // Delete user operation log (case) //////////////////////////////

        public virtual void testCaseDeleteEntry()
        {
            // given
            createCaseInstanceByKey(ONE_TASK_CASE_KEY);
            string taskId = selectSingleTask().Id;
            setAssignee(taskId, "demo");

            string entryId = historyService.CreateUserOperationLogQuery().First().Id;

            // when
            historyService.DeleteUserOperationLogEntry(entryId);

            // then
            Assert.IsNull(historyService.CreateUserOperationLogQuery().First());
        }

        // helper ////////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<IUserOperationLogEntry> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual ESS.FW.Bpm.Engine.Runtime.IJob selectSingleJob()
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery().First();
            enableAuthorization();
            return job;
        }

        protected internal virtual void clearDatabase()
        {
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly UserOperationLogAuthorizationTest outerInstance;

            public CommandAnonymousInnerClass(UserOperationLogAuthorizationTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
                IList<IHistoricIncident> incidents =
                    ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration.HistoryService.CreateHistoricIncidentQuery()
                        .ToList();
                foreach (IHistoricIncident incident in incidents)
                {
                    commandContext.HistoricIncidentManager.Delete((HistoricIncidentEntity)incident);
                }
                return null;
            }
        }
    }

}