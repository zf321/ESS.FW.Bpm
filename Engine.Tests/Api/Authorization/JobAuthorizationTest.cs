using System;
using System.Linq;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    public class JobAuthorizationTest : AuthorizationTest
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
            private readonly JobAuthorizationTest outerInstance;

            public CommandAnonymousInnerClass(JobAuthorizationTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendJobDefinitionHandler.TYPE);
                return null;
            }
        }

        // job query (jobs associated to a process) //////////////////////////////////////////////////

        public virtual void testQueryWithoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 0);
        }

        public virtual void testQueryWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 1);
        }

        public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithMultiple()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        public virtual void testQueryWithReadInstancePermissionOnTimerStartProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 1);

            ESS.FW.Bpm.Engine.Runtime.IJob job = query.First();
            Assert.IsNull(job.ProcessInstanceId);
            Assert.AreEqual(TIMER_START_PROCESS_KEY, job.ProcessDefinitionKey);
        }

        public virtual void testQueryWithReadInstancePermissionOnTimerBoundaryProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 1);

            ESS.FW.Bpm.Engine.Runtime.IJob job = query.First();
            Assert.AreEqual(ProcessInstanceId, job.ProcessInstanceId);
            Assert.AreEqual(TIMER_BOUNDARY_PROCESS_KEY, job.ProcessDefinitionKey);
        }

        public virtual void testQueryWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 2);
        }

        // job query (standalone job) /////////////////////////////////

        public virtual void testStandaloneJobQueryWithoutAuthorization()
        {
            // given
            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, true, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            // when
            IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query = managementService.CreateJobQuery();

            // then
            //verifyQueryResults(query, 1);

            ESS.FW.Bpm.Engine.Runtime.IJob job = query.First();
            Assert.NotNull(job);
            Assert.IsNull(job.ProcessInstanceId);
            Assert.IsNull(job.ProcessDefinitionKey);

            deleteJob(job.Id);
        }

        // execute job ////////////////////////////////////////////////

        public virtual void testExecuteJobWithoutAuthorization()
        {
            // given
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectAnyJob();
            string jobId = job.Id;

            try
            {
                // when
                managementService.ExecuteJob(jobId);
                Assert.Fail("Exception expected: It should not be possible to execute the job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(job.ProcessDefinitionKey, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testExecuteJobWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testExecuteJobWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testExecuteJobWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        public virtual void testExecuteJobWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            ITask task = selectSingleTask();
            Assert.NotNull(task);
            Assert.AreEqual("taskAfterBoundaryEvent", task.TaskDefinitionKey);
        }

        // execute job (standalone job) ////////////////////////////////

        public virtual void testExecuteStandaloneJob()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            managementService.ExecuteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY);
            Assert.True(jobDefinition.Suspended);
        }

        // Delete job ////////////////////////////////////////////////

        public virtual void testDeleteJobWithoutAuthorization()
        {
            // given
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectAnyJob();
            string jobId = job.Id;

            try
            {
                // when
                managementService.DeleteJob(jobId);
                Assert.Fail("Exception expected: It should not be possible to Delete the job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(job.ProcessDefinitionKey, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testDeleteJobWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.DeleteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.IsNull(job);
        }

        public virtual void testDeleteJobWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.DeleteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.IsNull(job);
        }

        public virtual void testDeleteJobWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.DeleteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.IsNull(job);
        }

        public virtual void testDeleteJobWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            // when
            managementService.DeleteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.IsNull(job);
        }

        // Delete standalone job ////////////////////////////////

        public virtual void testDeleteStandaloneJob()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            managementService.DeleteJob(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job);
        }

        // set job retries ////////////////////////////////////////////////

        public virtual void testSetJobRetriesWithoutAuthorization()
        {
            // given
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectAnyJob();
            string jobId = job.Id;

            try
            {
                // when
                managementService.SetJobRetries(jobId, 1);
                Assert.Fail("Exception expected: It should not be possible to set job retries");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(job.ProcessDefinitionKey, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetJobRetriesWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        public virtual void testSetJobRetriesWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        public virtual void testSetJobRetriesWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        public virtual void testSetJobRetriesWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        // set job retries (standalone) ////////////////////////////////

        public virtual void testSetStandaloneJobRetries()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.AreEqual(1, job.Retries);

            deleteJob(jobId);
        }

        // set job retries by job definition id ///////////////////////

        public virtual void testSetJobRetriesByJobDefinitionIdWithoutAuthorization()
        {
            // given
            disableAuthorization();
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();
            enableAuthorization();

            string jobDefinitionId = jobDefinition.Id;

            try
            {
                // when
                managementService.SetJobRetriesByJobDefinitionId(jobDefinitionId, 1);
                Assert.Fail("Exception expected: It should not be possible to set job retries");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(jobDefinition.ProcessDefinitionKey, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetJobRetriesByJobDefinitionIdWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SetJobRetriesByJobDefinitionId(jobDefinitionId, 1);
                Assert.Fail("Exception expected: It should not be possible to set job retries");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetJobRetriesByJobDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            disableAuthorization();
            managementService.SetJobRetries(jobId, 0);
            enableAuthorization();

            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            // when
            managementService.SetJobRetriesByJobDefinitionId(jobDefinitionId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        public virtual void testSetJobRetriesByJobDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        public virtual void testSetJobRetriesByJobDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobRetries(jobId, 1);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.AreEqual(1, job.Retries);
        }

        // set job due date ///////////////////////////////////////////

        public virtual void testSetJobDueDateWithoutAuthorization()
        {
            // given
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectAnyJob();
            string jobId = job.Id;

            try
            {
                // when
                managementService.SetJobDuedate(jobId, DateTime.Now);
                Assert.Fail("Exception expected: It should not be possible to set the job due date");
            }
            catch (AuthorizationException e)
            {
                // then
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(job.ProcessDefinitionKey, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSetJobDueDateWithUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobDuedate(jobId, DateTime.Parse(null));

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job.Duedate);
        }

        public virtual void testSetJobDueDateWithUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobDuedate(jobId, DateTime.Parse(null));

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job.Duedate);
        }

        public virtual void testSetJobDueDateWithUpdateInstancePermissionOnTimerBoundaryProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobDuedate(jobId, DateTime.Parse(null));

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job.Duedate);
        }

        public virtual void testSetJobDueDateWithUpdateInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            // when
            managementService.SetJobDuedate(jobId, DateTime.Parse(null));

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job.Duedate);
        }

        // set job retries (standalone) ////////////////////////////////

        public virtual void testSetStandaloneJobDueDate()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            managementService.SetJobDuedate(jobId, DateTime.Parse(null));

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.IsNull(job.Duedate);

            deleteJob(jobId);
        }

        // get exception stacktrace ///////////////////////////////////////////

        public virtual void testGetExceptionStacktraceWithoutAuthorization()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            disableAuthorization();
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            try
            {
                // when
                managementService.GetJobExceptionStacktrace(jobId);
                Assert.Fail("Exception expected: It should not be possible to get the exception stacktrace");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Read.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.ReadInstance.ToString(), message);
                AssertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testGetExceptionStacktraceWithReadPermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Read);

            // when
            string jobExceptionStacktrace = managementService.GetJobExceptionStacktrace(jobId);

            // then
            Assert.NotNull(jobExceptionStacktrace);
        }

        public virtual void testGetExceptionStacktraceReadPermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Read);

            // when
            string jobExceptionStacktrace = managementService.GetJobExceptionStacktrace(jobId);

            // then
            Assert.NotNull(jobExceptionStacktrace);
        }

        public virtual void testGetExceptionStacktraceWithReadInstancePermissionOnTimerBoundaryProcessDefinition()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessDefinition, ONE_INCIDENT_PROCESS_KEY, userId, Permissions.ReadInstance);

            // when
            string jobExceptionStacktrace = managementService.GetJobExceptionStacktrace(jobId);

            // then
            Assert.NotNull(jobExceptionStacktrace);
        }

        public virtual void testGetExceptionStacktraceWithReadInstancePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.ReadInstance);

            // when
            string jobExceptionStacktrace = managementService.GetJobExceptionStacktrace(jobId);

            // then
            Assert.NotNull(jobExceptionStacktrace);
        }

        // get exception stacktrace (standalone) ////////////////////////////////

        public virtual void testStandaloneJobGetExceptionStacktrace()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            string jobExceptionStacktrace = managementService.GetJobExceptionStacktrace(jobId);

            // then
            Assert.IsNull(jobExceptionStacktrace);

            deleteJob(jobId);
        }

        // suspend job by id //////////////////////////////////////////

        public virtual void testSuspendJobByIdWihtoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            try
            {
                // when
                managementService.SuspendJobById(jobId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            managementService.SuspendJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendStandaloneJobById()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;

            // when
            managementService.SuspendJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);

            deleteJob(jobId);
        }

        // activate job by id //////////////////////////////////////////

        public virtual void testActivateJobByIdWihtoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            suspendJobById(jobId);

            try
            {
                // when
                managementService.ActivateJobById(jobId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            suspendJobById(jobId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            managementService.ActivateJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            suspendJobById(jobId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            suspendJobById(jobId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobId = selectJobByProcessInstanceId(ProcessInstanceId).Id;
            suspendJobById(jobId);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateStandaloneJobById()
        {
            // given
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_START_PROCESS_KEY, userId, Permissions.Update);

            DateTime startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

            disableAuthorization();
            // creates a new "standalone" job
            managementService.SuspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
            enableAuthorization();

            string jobId = managementService.CreateJobQuery().First().Id;
            suspendJobById(jobId);

            // when
            managementService.ActivateJobById(jobId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobById(jobId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);

            deleteJob(jobId);
        }

        // suspend job by process instance id //////////////////////////////////////////

        public virtual void testSuspendJobByProcessInstanceIdWihtoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SuspendJobByProcessInstanceId(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            managementService.SuspendJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job by process instance id //////////////////////////////////////////

        public virtual void testActivateJobByProcessInstanceIdWihtoutAuthorization()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessInstanceId(ProcessInstanceId);

            try
            {
                // when
                managementService.ActivateJobByProcessInstanceId(ProcessInstanceId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(ProcessInstanceId, message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessInstanceId(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            // when
            managementService.ActivateJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessInstanceId(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessInstanceId(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessInstanceId(ProcessInstanceId);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessInstanceId(ProcessInstanceId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // suspend job by job definition id //////////////////////////////////////////

        public virtual void testSuspendJobByJobDefinitionIdWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SuspendJobByJobDefinitionId(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobByJobDefinitionId(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job by job definition id //////////////////////////////////////////

        public virtual void testActivateJobByJobDefinitionIdWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByJobDefinitionId(jobDefinitionId);

            try
            {
                // when
                managementService.ActivateJobByJobDefinitionId(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByJobDefinitionId(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobByJobDefinitionId(jobDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByJobDefinitionId(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByJobDefinitionId(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByJobDefinitionId(jobDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByJobDefinitionId(jobDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // suspend job by process definition id //////////////////////////////////////////

        public virtual void testSuspendJobByProcessDefinitionIdWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            try
            {
                // when
                managementService.SuspendJobByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job by process definition id //////////////////////////////////////////

        public virtual void testActivateJobByProcessDefinitionIdWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionId(processDefinitionId);

            try
            {
                // when
                managementService.ActivateJobByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobByProcessDefinitionId(processDefinitionId);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionId(processDefinitionId);

            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessDefinitionId(processDefinitionId);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // suspend job by process definition key //////////////////////////////////////////

        public virtual void testSuspendJobByProcessDefinitionKeyWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

            try
            {
                // when
                managementService.SuspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.SuspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to suspend a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.SuspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.SuspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.True(job.Suspended);
        }

        // activate job by process definition key //////////////////////////////////////////

        public virtual void testActivateJobByProcessDefinitionKeyWihtoutAuthorization()
        {
            // given
            StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
            suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            try
            {
                // when
                managementService.ActivateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, ProcessInstanceId, userId, Permissions.Update);

            try
            {
                // when
                managementService.ActivateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
                Assert.Fail("Exception expected: It should not be possible to activate a job");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent(userId, message);
                AssertTextPresent(Permissions.Update.ToString(), message);
                AssertTextPresent(Resources.ProcessInstance.ToString()/*.ResourceName()*/, message);
                AssertTextPresent(Permissions.UpdateInstance.ToString(), message);
                AssertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
                AssertTextPresent(Resources.ProcessDefinition.ToString()/*.ResourceName()*/, message);
            }
        }

        public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessInstance()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessInstance, AuthorizationFields.Any, userId, Permissions.Update);

            // when
            managementService.ActivateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, TIMER_BOUNDARY_PROCESS_KEY, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessDefinition()
        {
            // given
            string ProcessInstanceId = StartProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
            suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
            createGrantAuthorization(Resources.ProcessDefinition, AuthorizationFields.Any, userId, Permissions.UpdateInstance);

            // when
            managementService.ActivateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

            // then
            ESS.FW.Bpm.Engine.Runtime.IJob job = selectJobByProcessInstanceId(ProcessInstanceId);
            Assert.NotNull(job);
            Assert.IsFalse(job.Suspended);
        }

        // helper /////////////////////////////////////////////////////

        protected internal virtual void verifyQueryResults(IQueryable<ESS.FW.Bpm.Engine.Runtime.IJob> query, int countExpected)
        {
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            //ORIGINAL LINE: //verifyQueryResults((org.camunda.bpm.Engine.impl.AbstractQuery<?, ?>) query, countExpected);
            ////verifyQueryResults((AbstractQuery<object, object>)query, countExpected);
        }

        protected internal virtual ESS.FW.Bpm.Engine.Runtime.IJob selectAnyJob()
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery()/*.GetListPage(0, 1)*/.ElementAt(0);
            enableAuthorization();
            return job;
        }

        protected internal virtual void deleteJob(string jobId)
        {
            disableAuthorization();
            managementService.DeleteJob(jobId);
            enableAuthorization();
        }

        protected internal virtual ESS.FW.Bpm.Engine.Runtime.IJob selectJobByProcessInstanceId(string ProcessInstanceId)
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
            enableAuthorization();
            return job;
        }

        protected internal virtual ESS.FW.Bpm.Engine.Runtime.IJob selectJobById(string jobId)
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Runtime.IJob job = managementService.CreateJobQuery(c=>c.Id == jobId).First();
            enableAuthorization();
            return job;
        }

        protected internal virtual ESS.FW.Bpm.Engine.Management.IJobDefinition selectJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
        {
            disableAuthorization();
            ESS.FW.Bpm.Engine.Management.IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery(c=>c.ProcessDefinitionKey==processDefinitionKey).First();
            enableAuthorization();
            return jobDefinition;
        }

    }

}