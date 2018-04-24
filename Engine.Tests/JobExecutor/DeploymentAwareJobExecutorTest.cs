using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    [TestFixture]
    public class DeploymentAwareJobExecutorTest : PluggableProcessEngineTestCase
    {
        protected internal IProcessEngine OtherProcessEngine;
        [SetUp]
        public virtual void SetUp()
        {
            processEngineConfiguration.SetJobExecutorDeploymentAware(true);
        }
        [TearDown]
        public virtual void tearDown()
        {
            processEngineConfiguration.SetJobExecutorDeploymentAware(false);
        }

        protected internal virtual void closeDownProcessEngine()
        {
            CloseDownProcessEngine();
            if (OtherProcessEngine != null)
            {
                OtherProcessEngine.Close();
                ProcessEngines.Unregister(OtherProcessEngine);
                OtherProcessEngine = null;
            }
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestProcessingOfJobsWithMatchingDeployment()
        {
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            var registeredDeployments = managementService.RegisteredDeployments;
            Assert.AreEqual(1, registeredDeployments.Count);
            Assert.True(registeredDeployments.Contains(DeploymentId));

            var executableJob = managementService.CreateJobQuery().First();

            var otherDeploymentId =
                DeployAndInstantiateWithNewEngineConfiguration(
                    "resources/jobexecutor/simpleAsyncProcessVersion2.bpmn20.xml");

            // Assert that two jobs have been created, one for each deployment
            var jobs = managementService.CreateJobQuery()
                .ToList();
            Assert.AreEqual(2, jobs.Count);
            ISet<string> jobDeploymentIds = new HashSet<string>();
            jobDeploymentIds.Add(jobs[0].DeploymentId);
            jobDeploymentIds.Add(jobs[1].DeploymentId);

            Assert.True(jobDeploymentIds.Contains(DeploymentId));
            Assert.True(jobDeploymentIds.Contains(otherDeploymentId));

            // select executable jobs for executor of first engine
            var acquiredJobs = GetExecutableJobs(processEngineConfiguration.JobExecutor);
            Assert.AreEqual(1, acquiredJobs.Size());
            Assert.True(acquiredJobs.Contains(executableJob.Id));

            repositoryService.DeleteDeployment(otherDeploymentId, true);
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestExplicitDeploymentRegistration()
        {
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            var otherDeploymentId =
                DeployAndInstantiateWithNewEngineConfiguration(
                    "resources/jobexecutor/simpleAsyncProcessVersion2.bpmn20.xml");

            ProcessEngine.ManagementService.RegisterDeploymentForJobExecutor(otherDeploymentId);

            var jobs = managementService.CreateJobQuery().ToList();

            var acquiredJobs = GetExecutableJobs(processEngineConfiguration.JobExecutor);
            Assert.AreEqual(2, acquiredJobs.Size());
            foreach (var job in jobs)
                Assert.True(acquiredJobs.Contains(job.Id));

            repositoryService.DeleteDeployment(otherDeploymentId, true);
        }

        [Test]
        public virtual void TestRegistrationOfNonExistingDeployment()
        {
            var nonExistingDeploymentId = "some non-existing id";

            try
            {
                ProcessEngine.ManagementService.RegisterDeploymentForJobExecutor(nonExistingDeploymentId);
                Assert.Fail("Registering a non-existing deployment should not succeed");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Deployment " + nonExistingDeploymentId + " does not exist", e.Message);
                // happy path
            }
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestDeploymentUnregistrationOnUndeployment()
        {
            Assert.AreEqual(1, managementService.RegisteredDeployments.Count);

            repositoryService.DeleteDeployment(DeploymentId, true);

            Assert.AreEqual(0, managementService.RegisteredDeployments.Count);
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestNoUnregistrationOnFailingUndeployment()
        {
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            try
            {
                repositoryService.DeleteDeployment(DeploymentId, false);
                Assert.Fail();
            }
            catch (System.Exception ex)
            {
                // should still be registered, if not successfully undeployed
                Assert.AreEqual(1, managementService.RegisteredDeployments.Count);
            }
        }

        [Test]
        [Deployment("resources/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
        public virtual void TestExplicitDeploymentUnregistration()
        {
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            ProcessEngine.ManagementService.UnregisterDeploymentForJobExecutor(DeploymentId);

            var acquiredJobs = GetExecutableJobs(processEngineConfiguration.JobExecutor);
            Assert.AreEqual(0, acquiredJobs.Size());
        }

        public virtual void TestJobsWithoutDeploymentIdAreAlwaysProcessed()
        {
            var commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;

            var messageId = commandExecutor.Execute(new CommandAnonymousInnerClass(this));

            var acquiredJobs = GetExecutableJobs(processEngineConfiguration.JobExecutor);
            Assert.AreEqual(1, acquiredJobs.Size());
            Assert.True(acquiredJobs.Contains(messageId));

            commandExecutor.Execute(new DeleteJobsCmd(messageId, true));
        }

        private class CommandAnonymousInnerClass : ICommand<string>
        {
            private readonly DeploymentAwareJobExecutorTest _outerInstance;

            public CommandAnonymousInnerClass(DeploymentAwareJobExecutorTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual string Execute(CommandContext commandContext)
            {
                var message = new MessageEntity();
                commandContext.JobManager.Send(message);
                return message.Id;
            }
        }

        private AcquiredJobs GetExecutableJobs(ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor)
        {
            return processEngineConfiguration.CommandExecutorTxRequired.Execute(new AcquireJobsCmd(jobExecutor));
        }

        private string DeployAndInstantiateWithNewEngineConfiguration(string resource)
        {
            // 1. create another process engine
            try
            {
                OtherProcessEngine =
                    ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource("camunda.cfg.xml")
                        .BuildProcessEngine();
            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is FileNotFoundException)
                    OtherProcessEngine =
                        ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource("activiti.cfg.xml")
                            .BuildProcessEngine();
                else
                    throw ex;
            }

            // 2. deploy again
            var otherRepositoryService = OtherProcessEngine.RepositoryService;

            var deploymentId = otherRepositoryService.CreateDeployment().AddClasspathResource(resource).Deploy().Id;

            // 3. start instance (i.E. create job)
            try
            {
                var newDefinition =
                    otherRepositoryService.CreateProcessDefinitionQuery(c => c.DeploymentId == deploymentId).First();
                OtherProcessEngine.RuntimeService.StartProcessInstanceById(newDefinition.Id);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return deploymentId;
        }

        [Test]
        [Deployment("resources/jobexecutor/processWithTimerCatch.bpmn20.xml")]
        public virtual void TestIntermediateTimerEvent()
        {
            runtimeService.StartProcessInstanceByKey("testProcess");

            var registeredDeployments = processEngineConfiguration.RegisteredDeployments;


            var existingJob = managementService.CreateJobQuery().SingleOrDefault();

            //ClockUtil.CurrentTime = new DateTime(DateTime.Now.Ticks + 61 * 1000);
            ClockUtil.CurrentTime = DateTime.Now.AddMilliseconds(61 * 1000);

            var acquirableJobs = FindAcquirableJobs();

            Assert.AreEqual(1, acquirableJobs.Count);
            Assert.AreEqual(existingJob.Id, acquirableJobs[0].Id);

            registeredDeployments.Clear();

            acquirableJobs = FindAcquirableJobs();

            Assert.AreEqual(0, acquirableJobs.Count);
        }

        [Test]
        [Deployment("resources/jobexecutor/processWithTimerStart.bpmn20.xml")]
        public virtual void TestTimerStartEvent()
        {
            var registeredDeployments = processEngineConfiguration.RegisteredDeployments;

            var existingJob = managementService.CreateJobQuery().First();

            ClockUtil.CurrentTime = new DateTime(DateTime.Now.Ticks + 1000);

            var acquirableJobs = FindAcquirableJobs();

            Assert.AreEqual(1, acquirableJobs.Count);
            Assert.AreEqual(existingJob.Id, acquirableJobs[0].Id);

            registeredDeployments.Clear();

            acquirableJobs = FindAcquirableJobs();

            Assert.AreEqual(0, acquirableJobs.Count);
        }

        protected internal virtual IList<JobEntity> FindAcquirableJobs()
        {
            return processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2());
        }

        private class CommandAnonymousInnerClass2 : ICommand<IList<JobEntity>>
        {
            public IList<JobEntity> Execute(CommandContext commandContext)
            {
                return commandContext.JobManager.FindNextJobsToExecute(new Page(0, 100));
            }
        }
    }
}