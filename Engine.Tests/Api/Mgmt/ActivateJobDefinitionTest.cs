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
    [TestFixture]
    public class ActivateJobDefinitionTest : PluggableProcessEngineTestCase
    {
        [TearDown]
        public void tearDown()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            commandExecutor.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ActivateJobDefinitionTest outerInstance;

            public CommandAnonymousInnerClass(ActivateJobDefinitionTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogsByHandlerType(
                    TimerActivateJobDefinitionHandler.TYPE);
                return null;
            }
        }

        // Test ManagementService#ActivateJobDefinitionById() /////////////////////////

        [Test]
        public virtual void testActivationById_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionById(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testActivationByIdAndActivateJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionById(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionById(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testActivationByIdAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionById(null, false, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionById(null, true, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionById(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionById(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test][Deployment( "resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id);

            // then
            // there exists a active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            var suspendedJob = jobQuery.First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByIdAndActivateJobsFlag_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, false);

            // then
            // there exists a active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            var suspendedJob = jobQuery.First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByIdAndActivateJobsFlag_shouldSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, true);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and a active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldExecuteImmediatelyAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, false, DateTime.MaxValue);

            // then
            // there exists an active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            var suspendedJob = jobQuery.First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldExecuteImmediatelyAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, true, DateTime.MaxValue);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and an active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldExecuteDelayedAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, false, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c => c.SuspensionState == SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobQuery.Where(c => c.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                .Count());

            var suspendedJob = jobQuery.First();

            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationById_shouldExecuteDelayedAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionById(jobDefinition.Id, true, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is active
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        // Test ManagementService#ActivateJobDefinitionByProcessDefinitionId() /////////////////////////

        [Test]
        public virtual void testActivationByProcessDefinitionId_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void
            testActivationByProcessDefinitionIdAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, false, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, true, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionId(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionId_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id);

            // then
            // there exists a active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

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
        public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, false);

            // then
            // there exists an active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

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
        public virtual void testActivationByProcessDefinitionIdAndActivateJobsFlag_shouldSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, true);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and an active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.First();
            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionId_shouldExecuteImmediatelyAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, false, DateTime.MaxValue);

            // then
            // there exists an active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionId_shouldExecuteImmediatelyAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, true, DateTime.MaxValue);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and an active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var activeJob = jobQuery.First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionId_shouldExecuteDelayedAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, false, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
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
        public virtual void testActivationByProcessDefinitionId_shouldExecuteDelayedAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionId(processDefinition.Id, true, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is active
            jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        // Test ManagementService#activateJobDefinitionByProcessDefinitionKey() /////////////////////////

        [Test]
        public virtual void testActivationByProcessDefinitionKey_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, false);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, true);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        public virtual void
            testActivationByProcessDefinitionKeyAndActivateJobsFlagAndExecutionDate_shouldThrowProcessEngineException()
        {
            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, false, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, true, DateTime.MaxValue);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, false, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }

            try
            {
                managementService.ActivateJobDefinitionByProcessDefinitionKey(null, true, DateTime.Now);
                Assert.Fail("A ProcessEngineException was expected.");
            }
            catch (ProcessEngineException)
            {
            }
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key);

            // then
            // there exists a active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

//            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .Count());
//            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

//            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
////                .First();

//            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false);

            // then
            // there exists an active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKeyAndActivateJobsFlag_shouldSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and an active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            Assert.IsFalse(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldExecuteImmediatelyAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false,
                DateTime.MaxValue);

            // then
            // there exists an active job definition
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            var jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            //Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldExecuteImmediatelyAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, DateTime.MaxValue);

            // then
            // there exists an active job definition..
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();

            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // ..and an active job of the provided job definition
            var jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());

            var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
            Assert.IsFalse(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldExecuteDelayedAndRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, false, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is still suspended
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            //var suspendedJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
//                .First();

            //Assert.AreEqual(jobDefinition.Id, suspendedJob.JobDefinitionId);
//            Assert.True(suspendedJob.Suspended);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKey_shouldExecuteDelayedAndSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(processDefinition.Key, true, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(1, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJobDefinition = jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJobDefinition.Id);
            Assert.IsFalse(activeJobDefinition.Suspended);

            // the corresponding job is active
            jobQuery = managementService.CreateJobQuery();

            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var activeJob = jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .First();

            Assert.AreEqual(jobDefinition.Id, activeJob.JobDefinitionId);
            Assert.IsFalse(activeJob.Suspended);
        }

        // Test ManagementService#activateJobDefinitionByProcessDefinitionKey() with multiple process definition
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key);

            // then
            // all job definitions are active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still suspended
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
        public virtual void testMultipleSuspensionByProcessDefinitionKeyAndActivateJobsFlag_shouldRetainJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, false);

            // then
            // all job definitions are active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still suspended
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
        public virtual void testMultipleSuspensionByProcessDefinitionKeyAndActivateJobsFlag_shouldSuspendJobs()
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, true);

            // then
            // all job definitions are active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // and the jobs too
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, false, DateTime.MaxValue);

            // then
            // all job definitions are active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // but the jobs are still suspended
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, true, DateTime.MaxValue);

            // then
            // all job definitions are active
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());

            // and the jobs too
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, false, oneWeekLater());

            // then
            // the job definition is still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // but the jobs are still suspended
            jobQuery = managementService.CreateJobQuery();
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
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            // when
            // activate the job definition
            managementService.ActivateJobDefinitionByProcessDefinitionKey(key, true, oneWeekLater());

            // then
            // the job definitions are still suspended
            var jobDefinitionQuery = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            var jobQuery = managementService.CreateJobQuery();

            //var delayedActivationJob = jobQuery.Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be active
            Assert.AreEqual(3, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobDefinitionQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // the corresponding jobs are active
            jobQuery = managementService.CreateJobQuery();

            Assert.AreEqual(3, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // Clean DB
            foreach (var deployment in repositoryService.CreateDeploymentQuery()
                
                .ToList())
                repositoryService.DeleteDeployment(deployment.Id, true);
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByIdUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();

            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // when
            // activate the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .Activate();

            // then
            // there exists a active job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionIdUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            var query = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            var processDefinition = repositoryService.CreateProcessDefinitionQuery()
                .First();

            // when
            // activate the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionId(processDefinition.Id)
                .Activate();

            // then
            // there exists a active job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationByProcessDefinitionKeyUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            var query = managementService.CreateJobDefinitionQuery();
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // when
            // activate the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByProcessDefinitionKey("suspensionProcess")
                .Activate();

            // then
            // there exists a active job definition
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }
        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testActivationJobDefinitionIncludingJobsUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();

            var jobQuery = managementService.CreateJobQuery();
            //Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
            //    .Count());
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
            //    .Count());

            // when
            // activate the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .SetIncludeJobs(true)
                .Activate();

            // then
            // there exists a active job definition
            //Assert.AreEqual(1, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
//                .Count());
            Assert.AreEqual(0, jobQuery.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        [Test]
        [Deployment("resources/api/mgmt/SuspensionTest.TestBase.bpmn")]
        public virtual void testDelayedActivationUsingBuilder()
        {
            // given
            // a deployed process definition with asynchronous continuation

            // a running process instance with a failed job
            runtimeService.StartProcessInstanceByKey("suspensionProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("Assert.Fail", true));

            // a job definition (which was created for the asynchronous continuation)
            // ..Which will be suspended with the corresponding jobs
            managementService.SuspendJobDefinitionByProcessDefinitionKey("suspensionProcess", true);

            var query = managementService.CreateJobDefinitionQuery();
            var jobDefinition = query.First();

            // when
            // activate the job definition
            managementService.UpdateJobDefinitionSuspensionState()
                .ByJobDefinitionId(jobDefinition.Id)
                .ExecutionDate(oneWeekLater())
                .Activate();

            // then
            // the job definition is still suspended
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());

            // there exists a job for the delayed activation execution
            //var delayedActivationJob = managementService.CreateJobQuery()
            //    .Timers(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode)
            //    .First();
            //Assert.NotNull(delayedActivationJob);

            //// execute job
            //managementService.ExecuteJob(delayedActivationJob.Id);

            // the job definition should be suspended
            Assert.AreEqual(1, query.Where(c=>c.SuspensionState==SuspensionStateFields.Active.StateCode)
                .Count());
            Assert.AreEqual(0, query.Where(c=>c.SuspensionState==SuspensionStateFields.Suspended.StateCode)
                .Count());
        }

        protected internal virtual DateTime oneWeekLater()
        {
            // one week from now
            var startTime = DateTime.Now;
            ClockUtil.CurrentTime = startTime;
            var oneWeekFromStartTime = startTime.Ticks + 7 * 24 * 60 * 60 * 1000;
            return new DateTime(oneWeekFromStartTime);
        }
    }
}