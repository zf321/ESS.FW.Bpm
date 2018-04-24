using System;
using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Mgmt
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class SuspendJobDefinitionTest : PluggableProcessEngineTestCase
    {
        [TearDown]
        public void tearDown()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly SuspendJobDefinitionTest outerInstance;

            public CommandAnonymousInnerClass(SuspendJobDefinitionTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(
                    TimerSuspendJobDefinitionHandler.TYPE);
                return null;
            }
        }

        // Test ManagementService#SuspendJobDefinitionByProcessDefinitionId() /////////////////////////
        [Test]
        public virtual void testSuspensionByProcessDefinitionId_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void
            testSuspensionByProcessDefinitionIdAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, false, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, true, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionId(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        // Test ManagementService#SuspendJobDefinitionByProcessDefinitionKey() /////////////////////////

        [Test]
        public virtual void testSuspensionByProcessDefinitionKey_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }
        [Test]

        public virtual void
            testSuspensionByProcessDefinitionKeyAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, false, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, true, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionByProcessDefinitionKey(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        // Test ManagementService#SuspendJobDefinitionByProcessDefinitionKey() with multiple process definition
        // with same process definition key

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldRetainJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key);

            // then
            // all job definitions are suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still active
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldRetainJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, false);

            // then
            // all job definitions are suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still active
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldSuspendJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, true);

            // then
            // all job definitions are suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // and the jobs too
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, false, null);

            // then
            // all job definitions are suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still active
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, true, null);

            // then
            // all job definitions are suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // and the jobs too
            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, false, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // but the jobs are still active
            jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        public virtual void testMultipleSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
        {
            // given
            var key = "suspensionProcess";

            // Deploy three processes and start for each deployment a process instance
            // with a failed job
            var nrOfProcessDefinitions = 3;
            for (var i = 0; i < nrOfProcessDefinitions; i++)
            {
                repositoryService.CreateDeployment()
                    .AddClasspathResource("resources/api/mgmt/SuspensionTest.TestBase.bpmn")
                    .Deploy();
                IDictionary<string, object> @params = new Dictionary<string, object>();
                @params["Assert.Fail"] = true;
                runtimeService.StartProcessInstanceByKey(key, @params);
            }

            // a job definition (which was created for the asynchronous continuation)

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(key, true, oneWeekLater());

            // then
            // the job definitions are still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // the corresponding jobs are suspended
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        protected internal virtual DateTime oneWeekLater()
        {
            var startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            var oneWeekFromStartTime = startTime.Ticks + 7 * 24 * 60 * 60 * 1000;
            return new DateTime(oneWeekFromStartTime);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testDelayedSuspensionUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();

            // when
            // suspend the job definition in one week
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .ExecutionDate(oneWeekLater())
                .Suspend();

            // then
            // the job definition is still active
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            //var delayedSuspensionJob = managementService.CreateJobQuery()
            //    .Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            // execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldExecuteDelayedAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, false, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldExecuteDelayedAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, true, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still suspended
            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);

            Assert.True(suspendedJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldExecuteImmediatelyAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, false, null);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldExecuteImmediatelyAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, true, null);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionById_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);

            // there does not exist any active job definition
            jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);
            Assert.True(!jobDefinitionQuery
                .Any());

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        // Test ManagementService#SuspendJobDefinitionById() /////////////////////////

        [Test]
        public virtual void testSuspensionById_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionById(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, false);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionById(jobDefinition.Id, true);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        public virtual void testSuspensionByIdAndSuspendJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionById(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionById(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testSuspensionByIdAndSuspendJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.SuspendJobDefinitionById(null, false, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionById(null, true, null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionById(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.SuspendJobDefinitionById(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByIdUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();
            Assert.IsFalse(jobDefinition.Suspended);

            // when
            // suspend the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .Suspend();

            // then
            // there exists a suspended job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldExecuteDelayedAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldExecuteDelayedAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is suspended
            jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldExecuteImmediatelyAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false, null);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldExecuteImmediatelyAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true, null);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionId_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);

            // there does not exist any active job definition
            jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);
            Assert.True(!jobDefinitionQuery
                
                .Any());

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, false);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);

            jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);
            Assert.AreEqual(0, jobQuery.Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionIdAndSuspendJobsFlag_shouldSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionId(processDefinition.Id, true);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionIdUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();
            Assert.IsFalse(jobDefinition.Suspended);

            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Suspend();

            // then
            // there exists a suspended job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, oneWeekLater());

            // then
            // the job definition is still active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed suspension execution
            var jobQuery = managementService.CreateJobQuery();

                        //var delayedSuspensionJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedSuspensionJob);

            //// execute job
            //managementService.ExecuteJob(delayedSuspensionJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is suspended
            jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, null);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, null);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKey_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);

            // there does not exist any active job definition
            jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode);
            Assert.True(!jobDefinitionQuery
                
                .Any());

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldRetainJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, false);

            // then
            // there exists a suspended job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // the corresponding job is still active
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);

            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKeyAndSuspendJobsFlag_shouldSuspendJobs()
        {
            // given
            // a deployed process definition with asynchronous continuation
            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // a running process instance with a failed job
            IDictionary<string, object> @params = new Dictionary<string, object>();
            @params["Assert.Fail"] = true;
            runtimeService.StartProcessInstanceByKey("suspensionProcess", @params);

            // a job definition (which was created for the asynchronous continuation)
            var jobDefinition = managementService.CreateJobDefinitionQuery()
                .First();

            // when
            // suspend the job definition
            managementService.SuspendJobDefinitionByProcessDefinitionKey(processDefinition.Key, true);

            // then
            // there exists a suspended job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobDefinitionQuery.Count());

            var suspendedJobDefinition = jobDefinitionQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJobDefinition.Id);
            Assert.True(suspendedJobDefinition.Suspended);

            // ..and a suspended job of the provided job definition
            var jobQuery = managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode);

            Assert.AreEqual(1, jobQuery.Count());

            var suspendedJob = jobQuery.First();
            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionByProcessDefinitionKeyUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();
            Assert.IsFalse(jobDefinition.Suspended);

            // when
            // suspend the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey("suspensionProcess")
                .Suspend();

            // then
            // there exists a suspended job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testSuspensionJobDefinitionIncludeJobsdUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();
            Assert.IsFalse(jobDefinition.Suspended);

            var jobQuery = managementService.CreateJobQuery();
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());


            // when
            // suspend the job definition and the job
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .SetIncludeJobs(true)
                .Suspend();

            // then
            // there exists a suspended job definition and job
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
        }
    }
}