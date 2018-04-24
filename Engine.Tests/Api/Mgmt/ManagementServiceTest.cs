using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ManagementServiceTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void testGetMetaDataForUnexistingTable()
        {
            var metaData = managementService.GetTableMetaData("unexistingtable");
            Assert.IsNull(metaData);
        }

        [Test]
        public virtual void testGetMetaDataNullTableName()
        {
            try
            {
                managementService.GetTableMetaData(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("tableName is null", re.Message);
            }
        }

        [Test]
        public virtual void testExecuteJobNullJobId()
        {
            try
            {
                managementService.ExecuteJob(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("jobId is null", re.Message);
            }
        }
        [Test]
        public virtual void testExecuteJobUnexistingJob()
        {
            try
            {
                managementService.ExecuteJob("unexistingjob");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("No job found with id", ae.Message);
            }
        }


        [Test][Deployment]
        public virtual void testGetJobExceptionStacktrace()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            // The execution is waiting in the first usertask. This contains a boundry
            // timer event which we will execute manual for testing purposes.
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "No job found for process instance");

            try
            {
                managementService.ExecuteJob(timerJob.Id);
                Assert.Fail("RuntimeException from within the script task expected");
            }
            catch (System.Exception re)
            {
                AssertTextPresent("This is an exception thrown from scriptTask", re.Message);
            }

            // Fetch the task to see that the exception that occurred is persisted
            timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob);
            Assert.NotNull(timerJob.ExceptionMessage);
            AssertTextPresent("This is an exception thrown from scriptTask", timerJob.ExceptionMessage);

            // Get the full stacktrace using the managementService
            var exceptionStack = managementService.GetJobExceptionStacktrace(timerJob.Id);
            Assert.NotNull(exceptionStack);
            AssertTextPresent("This is an exception thrown from scriptTask", exceptionStack);
        }

        [Test]
        public virtual void testgetJobExceptionStacktraceUnexistingJobId()
        {
            try
            {
                managementService.GetJobExceptionStacktrace("unexistingjob");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("No job found with id unexistingjob", re.Message);
            }
        }

        [Test]
        public virtual void testgetJobExceptionStacktraceNullJobId()
        {
            try
            {
                managementService.GetJobExceptionStacktrace(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("jobId is null", re.Message);
            }
        }

        [Test][Deployment( "resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml") ]
        public virtual void testSetJobRetries()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            // The execution is waiting in the first usertask. This contains a boundary
            // timer event.
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "No job found for process instance");
            Assert.AreEqual(JobEntity.DefaultRetries, timerJob.Retries);

            managementService.SetJobRetries(timerJob.Id, 5);

            timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.AreEqual(5, timerJob.Retries);
        }
        [Test]
        [Deployment( "resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml") ]
        public virtual void testSetJobsRetries()
        {
            //given
            runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");
            runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");
            var allJobIds = AllJobIds;

            //when
            managementService.SetJobRetries(allJobIds, 5);

            //then
            AssertRetries(allJobIds, 5);
        }

        [Test]
        public virtual void testSetJobsRetriesWithNull()
        {
            try
            {
                //when
                managementService.SetJobRetries((IList<string>) null, 5);
                Assert.Fail("exception expected");
                //then
            }
            catch (ProcessEngineException)
            {
                //expected
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobsRetriesWithNegativeRetries()
        {
            try
            {
                //when
                managementService.SetJobRetries(new[] {"aFake"}, -1);
                Assert.Fail("exception expected");
                //then
            }
            catch (ProcessEngineException)
            {
                //expected
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobsRetriesWithFake()
        {
            //given
            runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            var allJobIds = AllJobIds;
            allJobIds.Add("aFake");
            try
            {
                //when
                managementService.SetJobRetries(allJobIds, 5);
                Assert.Fail("exception expected");
                //then
            }
            catch (ProcessEngineException)
            {
                //expected
            }

            AssertRetries(AllJobIds, JobEntity.DefaultRetries);
        }

        protected internal virtual void AssertRetries(IList<string> allJobIds, int i)
        {
            foreach (var id in allJobIds)
                Assert.That(managementService.CreateJobQuery(c=>c.Id==id)
                    .First()
                    .Retries, Is.EqualTo(i));
        }

        protected internal virtual IList<string> AllJobIds
        {
            get
            {
                var result = new List<string>();
                foreach (var job in managementService.CreateJobQuery()
                    
                    .ToList())
                    result.Add(job.Id);
                return result;
            }
        }
        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobRetriesNullCreatesIncident()
        {
            // initially there is no incident
            Assert.AreEqual(0, runtimeService.CreateIncidentQuery()
                .Count());

            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            // The execution is waiting in the first usertask. This contains a boundary
            // timer event.
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "No job found for process instance");
            Assert.AreEqual(JobEntity.DefaultRetries, timerJob.Retries);

            managementService.SetJobRetries(timerJob.Id, 0);

            timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.AreEqual(0, timerJob.Retries);

            Assert.AreEqual(1, runtimeService.CreateIncidentQuery()
                .Count());
        }

        [Test]
        public virtual void testSetJobRetriesUnexistingJobId()
        {
            try
            {
                managementService.SetJobRetries("unexistingjob", 5);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("No job found with id 'unexistingjob'.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesEmptyJobId()
        {
            try
            {
                managementService.SetJobRetries("", 5);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesJobIdNull()
        {
            try
            {
                managementService.SetJobRetries((string) null, 5);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesNegativeNumberOfRetries()
        {
            try
            {
                managementService.SetJobRetries("unexistingjob", -1);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent(
                    "The number of job retries must be a non-negative Integer, but '-1' has been provided.", re.Message);
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobRetriesByJobDefinitionId()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");
            ExecuteAvailableJobs();

            var query = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id);

            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            var timerJob = query.First();

            Assert.NotNull(timerJob, "No job found for process instance");
            Assert.AreEqual(0, timerJob.Retries);

            managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 5);

            timerJob = query.First();
            Assert.AreEqual(5, timerJob.Retries);
        }

        [Test]
        public virtual void testSetJobRetriesByJobDefinitionIdEmptyJobDefinitionId()
        {
            try
            {
                managementService.SetJobRetriesByJobDefinitionId("", 5);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesByJobDefinitionIdNull()
        {
            try
            {
                managementService.SetJobRetriesByJobDefinitionId(null, 5);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesByJobDefinitionIdNegativeNumberOfRetries()
        {
            try
            {
                managementService.SetJobRetries("unexistingjob", -1);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent(
                    "The number of job retries must be a non-negative Integer, but '-1' has been provided.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobRetriesUnlocksInconsistentJob()
        {
            // case 1
            // given an inconsistent job that is never again picked up by a job executor
            createJob(0, "owner", ClockUtil.CurrentTime);

            // when the job retries are reset
            var job = (JobEntity) managementService.CreateJobQuery()
                .First();
            managementService.SetJobRetries(job.Id, 3);

            // then the job can be picked up again
            job = (JobEntity) managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.IsNull(job.LockOwner);
            Assert.IsNull(job.LockExpirationTime);
            Assert.AreEqual(3, job.Retries);

            deleteJobAndIncidents(job);

            // case 2
            // given an inconsistent job that is never again picked up by a job executor
            createJob(2, "owner", DateTime.MaxValue);

            // when the job retries are reset
            job = (JobEntity) managementService.CreateJobQuery()
                .First();
            managementService.SetJobRetries(job.Id, 3);

            // then the job can be picked up again
            job = (JobEntity) managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.IsNull(job.LockOwner);
            Assert.IsNull(job.LockExpirationTime);
            Assert.AreEqual(3, job.Retries);

            deleteJobAndIncidents(job);

            // case 3
            // given a consistent job
            createJob(2, "owner", ClockUtil.CurrentTime);

            // when the job retries are reset
            job = (JobEntity) managementService.CreateJobQuery()
                .First();
            managementService.SetJobRetries(job.Id, 3);

            // then the lock owner and expiration should not change
            job = (JobEntity) managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.NotNull(job.LockOwner);
            Assert.NotNull(job.LockExpirationTime);
            Assert.AreEqual(3, job.Retries);

            deleteJobAndIncidents(job);
        }
        protected internal virtual void createJob(int retries, string owner, DateTime lockExpirationTime)
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this, retries, owner, lockExpirationTime));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ManagementServiceTest outerInstance;
            private readonly DateTime lockExpirationTime;
            private readonly string owner;

            private readonly int retries;

            public CommandAnonymousInnerClass(ManagementServiceTest outerInstance, int retries, string owner,
                DateTime lockExpirationTime)
            {
                this.outerInstance = outerInstance;
                this.retries = retries;
                this.owner = owner;
                this.lockExpirationTime = lockExpirationTime;
            }

            public object Execute(CommandContext commandContext)
            {
                var jobManager = commandContext.JobManager;
                var job = new MessageEntity();
                job.JobHandlerType = "any";
                job.LockOwner = owner;
                job.LockExpirationTime = lockExpirationTime;
                job.Retries = retries;

                jobManager.Send(job);
                return null;
            }
        }
        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobRetriesByDefinitionUnlocksInconsistentJobs()
        {
            // given a job definition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.Management.IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // and an inconsistent job that is never again picked up by a job executor
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass2(this, jobDefinition));

            // when the job retries are reset
            managementService.SetJobRetriesByJobDefinitionId(jobDefinition.Id, 3);

            // then the job can be picked up again
            var job = (JobEntity) managementService.CreateJobQuery()
                .First();
            Assert.NotNull(job);
            Assert.IsNull(job.LockOwner);
            Assert.IsNull(job.LockExpirationTime);
            Assert.AreEqual(3, job.Retries);

            deleteJobAndIncidents(job);
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly ManagementServiceTest outerInstance;

            private readonly IJobDefinition jobDefinition;

            public CommandAnonymousInnerClass2(ManagementServiceTest outerInstance, IJobDefinition jobDefinition)
            {
                this.outerInstance = outerInstance;
                this.jobDefinition = jobDefinition;
            }

            public object Execute(CommandContext commandContext)
            {
                var jobManager = commandContext.JobManager;
                var job = new MessageEntity();
                job.JobDefinitionId = jobDefinition.Id;
                job.JobHandlerType = "any";
                job.LockOwner = "owner";
                job.LockExpirationTime = ClockUtil.CurrentTime;
                job.Retries = 0;

                jobManager.Send(job);
                return null;
            }
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteJobAndIncidents(final org.Camunda.bpm.Engine.Runtime.IJob job)
        protected internal virtual void deleteJobAndIncidents(IJob job)
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.Camunda.bpm.Engine.history.IHistoricIncident> incidents = historyService.CreateHistoricIncidentQuery().IncidentType(org.Camunda.bpm.Engine.Runtime.IncidentFields.FailedJobHandlerType).ToList();
            var incidents = historyService.CreateHistoricIncidentQuery(c=> c.IncidentType==IncidentFields.FailedJobHandlerType)
                
                .ToList();

            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass3(this, job, incidents));
        }

        private class CommandAnonymousInnerClass3 : ICommand<object>
        {
            private readonly ManagementServiceTest outerInstance;
            private readonly IList<IHistoricIncident> incidents;

            private readonly IJob job;

            public CommandAnonymousInnerClass3(ManagementServiceTest outerInstance, IJob job,
                IList<IHistoricIncident> incidents)
            {
                this.outerInstance = outerInstance;
                this.job = job;
                this.incidents = incidents;
            }

            public object Execute(CommandContext commandContext)
            {
                ((JobEntity) job).Delete();

                var historicIncidentManager = commandContext.HistoricIncidentManager;
                foreach (var incident in incidents)
                {
                    var incidentEntity = (HistoricIncidentEntity) incident;
                    historicIncidentManager.Delete(incidentEntity);
                }

                commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(job.Id);
                return null;
            }
        }

        [Test]
        public virtual void testDeleteJobNullJobId()
        {
            try
            {
                managementService.DeleteJob(null);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("jobId is null", re.Message);
            }
        }

        [Test]
        public virtual void testDeleteJobUnexistingJob()
        {
            try
            {
                managementService.DeleteJob("unexistingjob");
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException ae)
            {
                AssertTextPresent("No job found with id", ae.Message);
            }
        }

        [Test][Deployment("resources/api/mgmt/timerOnTask.bpmn20.xml")] 
        public virtual void testDeleteJobDeletion()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("timerOnTask");
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "ITask timer should be there");
            managementService.DeleteJob(timerJob.Id);

            timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();
            Assert.IsNull(timerJob, "There should be no job now. It was deleted");
        }

        [Test]
        [Deployment("resources/api/mgmt/timerOnTask.bpmn20.xml")]
        public virtual void testDeleteJobThatWasAlreadyAcquired()
        {
            ClockUtil.CurrentTime = DateTime.Now;

            var processInstance = runtimeService.StartProcessInstanceByKey("timerOnTask");
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            // We need to move time at least one hour to make the timer executable
            ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.TimeOfDay.Ticks + 7200000L);

            // Acquire job by running the acquire command manually
            var processEngineImpl = (ProcessEngineImpl) ProcessEngine;
            //Impl.JobExecutor.JobExecutor jobExecutor = processEngineImpl.ProcessEngineConfiguration.JobExecutor;
            //AcquireJobsCmd acquireJobsCmd = new AcquireJobsCmd(jobExecutor);
            //ICommandExecutor commandExecutor = processEngineImpl.ProcessEngineConfiguration.CommandExecutorTxRequired;
            //commandExecutor.Execute(acquireJobsCmd);

            // Try to Delete the job. This should Assert.Fail.
            try
            {
                managementService.DeleteJob(timerJob.Id);
                Assert.Fail();
            }
            catch (ProcessEngineException)
            {
                // Exception is expected
            }

            // Clean up
            managementService.ExecuteJob(timerJob.Id);
        }

        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobDuedate()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            // The execution is waiting in the first usertask. This contains a boundary
            // timer event.
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "No job found for process instance");
            Assert.NotNull(timerJob.Duedate);

            DateTime cal;
            cal = DateTime.Now;
            cal.AddDays(3); // add 3 days on the actual date
            managementService.SetJobDuedate(timerJob.Id, cal);

            var newTimerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            // normalize date for mysql dropping fractional seconds in time values
            var SECOND = 1000;
            Assert.AreEqual(cal.TimeOfDay.Ticks / SECOND * SECOND, ((DateTime)newTimerJob.Duedate).TimeOfDay.Ticks / SECOND * SECOND);
        }

        [Test]
        [Deployment("resources/api/mgmt/ManagementServiceTest.TestGetJobExceptionStacktrace.bpmn20.xml")]
        public virtual void testSetJobDuedateDateNull()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("exceptionInJobExecution");

            // The execution is waiting in the first usertask. This contains a boundary
            // timer event.
            var timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.NotNull(timerJob, "No job found for process instance");
            Assert.NotNull(timerJob.Duedate);

            managementService.SetJobDuedate(timerJob.Id, DateTime.MaxValue);

            timerJob = managementService.CreateJobQuery(c=>c.ProcessInstanceId== processInstance.Id)
                .First();

            Assert.IsNull(timerJob.Duedate);
        }


        [Test]
        public virtual void testSetJobDuedateJobIdNull()
        {
            try
            {
                managementService.SetJobDuedate(null, DateTime.Now);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("The job id is mandatory, but 'null' has been provided.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobDuedateEmptyJobId()
        {
            try
            {
                managementService.SetJobDuedate("", DateTime.Now);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("The job id is mandatory, but '' has been provided.", re.Message);
            }
        }

        [Test]
        public virtual void testSetJobDuedateUnexistingJobId()
        {
            try
            {
                managementService.SetJobDuedate("unexistingjob", DateTime.Now);
                Assert.Fail("ProcessEngineException expected");
            }
            catch (ProcessEngineException re)
            {
                AssertTextPresent("No job found with id 'unexistingjob'.", re.Message);
            }
        }

        [Test]
        public virtual void testGetProperties()
        {
            var properties = managementService.Properties;
            Assert.NotNull(properties);
            Assert.IsFalse(properties.Count == 0);
        }

        [Test]
        public virtual void testSetProperty()
        {
            const string name = "testProp";
            const string value = "testValue";
            managementService.SetProperty(name, value);

            var properties = managementService.Properties;
            Assert.True(properties.ContainsKey(name));
            var storedValue = properties[name];
            Assert.AreEqual(value, storedValue);

            managementService.DeleteProperty(name);
        }

        [Test]
        public virtual void testDeleteProperty()
        {
            const string name = "testProp";
            const string value = "testValue";
            managementService.SetProperty(name, value);

            var properties = managementService.Properties;
            Assert.True(properties.ContainsKey(name));
            var storedValue = properties[name];
            Assert.AreEqual(value, storedValue);

            managementService.DeleteProperty(name);
            properties = managementService.Properties;
            Assert.IsFalse(properties.ContainsKey(name));
        }

        [Test]
        public virtual void testDeleteNonexistingProperty()
        {
            managementService.DeleteProperty("non existing");
        }

        [Test]
        public virtual void testGetHistoryLevel()
        {
            var historyLevel = managementService.HistoryLevel;
            Assert.AreEqual(processEngineConfiguration.HistoryLevel.Id, historyLevel);
        }

        [Test][Deployment( "resources/api/mgmt/asyncTaskProcess.bpmn20.xml") ]
        public virtual void testSetJobPriority()
        {
            // given
            runtimeService.CreateProcessInstanceByKey("asyncTaskProcess")
                .StartBeforeActivity("task")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();

            // when
            managementService.SetJobPriority(job.Id, 42);

            // then
            job = managementService.CreateJobQuery()
                .First();

            Assert.AreEqual(42, job.Priority);
        }

        [Test]
        public virtual void testSetJobPriorityForNonExistingJob()
        {
            try
            {
                managementService.SetJobPriority("nonExistingJob", 42);
                Assert.Fail("should not succeed");
            }
            catch (NotFoundException e)
            {
                AssertTextPresentIgnoreCase("No job found with id 'nonExistingJob'", e.Message);
            }
        }
        [Test]
        public virtual void testSetJobPriorityForNullJob()
        {
            try
            {
                managementService.SetJobPriority(null, 42);
                Assert.Fail("should not succeed");
            }
            catch (NullValueException e)
            {
                AssertTextPresentIgnoreCase("IJob id must not be null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/asyncTaskProcess.bpmn20.xml")]
        public virtual void testSetJobPriorityToExtremeValues()
        {
            runtimeService.CreateProcessInstanceByKey("asyncTaskProcess")
                .StartBeforeActivity("task")
                .Execute();

            var job = managementService.CreateJobQuery()
                .First();

            // it is possible to set the max integer value
            managementService.SetJobPriority(job.Id, long.MaxValue);
            job = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(long.MaxValue, job.Priority);

            // it is possible to set the min integer value
            managementService.SetJobPriority(job.Id, long.MinValue + 1); // +1 for informix
            job = managementService.CreateJobQuery()
                .First();
            Assert.AreEqual(long.MinValue + 1, job.Priority);
        }

        [Test]
        public virtual void testGetTableMetaData()
        {
            var tableMetaData = managementService.GetTableMetaData("ACT_RU_TASK");
            Assert.AreEqual(tableMetaData.ColumnNames.Count, tableMetaData.ColumnTypes.Count);
            Assert.AreEqual(21, tableMetaData.ColumnNames.Count);

            var assigneeIndex = tableMetaData.ColumnNames.IndexOf("ASSIGNEE_");
            var createTimeIndex = tableMetaData.ColumnNames.IndexOf("CREATE_TIME_");

            Assert.True(assigneeIndex >= 0);
            Assert.True(createTimeIndex >= 0);

            AssertOneOf(new[] {"VARCHAR", "NVARCHAR2", "nvarchar", "NVARCHAR"}, tableMetaData.ColumnTypes[assigneeIndex]);
            AssertOneOf(new[] {"TIMESTAMP", "TIMESTAMP(6)", "datetime", "DATETIME", "DATETIME2"},
                tableMetaData.ColumnTypes[createTimeIndex]);
        }

        private void AssertOneOf(string[] possibleValues, string currentValue)
        {
            foreach (var value in possibleValues)
                if (currentValue.Equals(value))
                    return;
            Assert.Fail("Value '" + currentValue + "' should be one of: " + possibleValues);
        }

        [Test]
        public virtual void testGetTablePage()
        {
            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
            var taskIds = generateDummyTasks(20);

            //var tablePage = managementService.CreateTablePageQuery()
            //    .TableName(tablePrefix + "ACT_RU_TASK")
            //    .ListPage(0, 5);

            //Assert.AreEqual(0, tablePage.FirstResult);
            //Assert.AreEqual(5, tablePage.Size);
            //Assert.AreEqual(5, tablePage.Rows.Count);
            //Assert.AreEqual(20, tablePage.Total);

            //tablePage = managementService.CreateTablePageQuery()
            //    .TableName(tablePrefix + "ACT_RU_TASK")
            //    .ListPage(14, 10);

            //Assert.AreEqual(14, tablePage.FirstResult);
            //Assert.AreEqual(6, tablePage.Size);
            //Assert.AreEqual(6, tablePage.Rows.Count);
            //Assert.AreEqual(20, tablePage.Total);

            taskService.DeleteTasks(taskIds, true);
        }

        [Test]
        public virtual void testGetSortedTablePage()
        {
            var tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
            var taskIds = generateDummyTasks(15);

            // With an ascending sort
            //var tablePage = managementService.CreateTablePageQuery()
            //    .TableName(tablePrefix + "ACT_RU_TASK")
            //    .OrderAsc("NAME_")
            //    .ListPage(1, 7);
            //string[] expectedTaskNames = {"B", "C", "D", "E", "F", "G", "H"};
            //verifyTaskNames(expectedTaskNames, tablePage.Rows);

            //// With a descending sort
            //tablePage = managementService.CreateTablePageQuery()
            //    .TableName(tablePrefix + "ACT_RU_TASK")
            //    .OrderDesc("NAME_")
            //    .ListPage(6, 8);
            //expectedTaskNames = new[] {"I", "H", "G", "F", "E", "D", "C", "B"};
            //verifyTaskNames(expectedTaskNames, tablePage.Rows);

            //taskService.DeleteTasks(taskIds, true);
        }

        private void verifyTaskNames(string[] expectedTaskNames, IList<IDictionary<string, object>> rowData)
        {
            Assert.AreEqual(expectedTaskNames.Length, rowData.Count);
            var columnKey = "NAME_";

            for (var i = 0; i < expectedTaskNames.Length; i++)
            {
                var o = rowData[i][columnKey];
                if (o == null)
                    o = rowData[i][columnKey.ToLower()];
                Assert.AreEqual(expectedTaskNames[i], o);
            }
        }

        private IList<string> generateDummyTasks(int nrOfTasks)
        {
            var taskIds = new List<string>();
            for (var i = 0; i < nrOfTasks; i++)
            {
                var task = taskService.NewTask();
                task.Name = (char) ('A' + i) + "";
                taskService.SaveTask(task);
                taskIds.Add(task.Id);
            }
            return taskIds;
        }
    }
}