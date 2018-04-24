using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    [TestFixture]
    public class ConcurrentJobExecutorTest
    {
        private bool InstanceFieldsInitialized = false;

        public ConcurrentJobExecutorTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testRule = new ProcessEngineTestRule(engineRule);
            //ruleChain = RuleChain.outerRule(engineRule).around(testRule);
        }


        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testRule;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
        //public RuleChain ruleChain;


        protected internal IRuntimeService runtimeService;
        protected internal IRepositoryService repositoryService;
        protected internal IManagementService managementService;
        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        protected internal Thread testThread = Thread.CurrentThread;
        protected internal static ControllableThread activeThread;

        protected internal static readonly IBpmnModelInstance SIMPLE_ASYNC_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("simpleAsyncProcess").StartEvent().ServiceTask().CamundaExpression("${true}").CamundaAsyncBefore().EndEvent().Done();

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void initServices()
        public virtual void initServices()
        {
            runtimeService = engineRule.RuntimeService;
            repositoryService = engineRule.RepositoryService;
            managementService = engineRule.ManagementService;
            processEngineConfiguration = engineRule.ProcessEngineConfiguration;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void deleteJobs()
        public virtual void deleteJobs()
        {
            foreach (IJob job in managementService.CreateJobQuery().ToList())
            {

                processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, job));
            }
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ConcurrentJobExecutorTest outerInstance;
            private IJob job;

            public CommandAnonymousInnerClass(ConcurrentJobExecutorTest outerInstance, IJob job)
            {
                this.outerInstance = outerInstance;
                this.job = job;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                ((JobEntity)job).Delete();
                return null;
            }
        }

        [Test]
        public virtual void testCompetingJobExecutionDeleteJobDuringExecution()
        {
            //given a simple process with a async service task
            //testRule.Deploy(Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask("task").CamundaAsyncBefore().CamundaDelegateExpression().CamundaExpression("${true}").EndEvent().Done());
           var task= ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask("task");

            task.CamundaAsyncBefore();
            var r= task.CamundaDelegateExpression(string.Empty).CamundaExpression("${true}").EndEvent().Done();
            testRule.Deploy(r);
            runtimeService.StartProcessInstanceByKey("process");
            IJob currentJob = managementService.CreateJobQuery().First();

            // when a job is executed
            JobExecutionThread threadOne = new JobExecutionThread(this, currentJob.Id);
            threadOne.startAndWaitUntilControlIsReturned();
            //and deleted in parallel
            managementService.DeleteJob(currentJob.Id);

            // then the job fails with a OLE and the failed job listener throws no NPE
            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.True(threadOne.exception is OptimisticLockingException);
        }

        [Test]
        public virtual void testCompetingJobExecutionDefaultRetryStrategy()
        {
            // given an MI subprocess with two instances
            runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            IList<IJob> currentJobs = managementService.CreateJobQuery()
                .ToList();
            Assert.AreEqual(2, currentJobs.Count);

            // when the jobs are executed in parallel
            JobExecutionThread threadOne = new JobExecutionThread(this, currentJobs[0].Id);
            threadOne.startAndWaitUntilControlIsReturned();

            JobExecutionThread threadTwo = new JobExecutionThread(this, currentJobs[1].Id);
            threadTwo.startAndWaitUntilControlIsReturned();

            // then the first committing thread succeeds
            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.IsNull(threadOne.exception);

            // then the second committing thread fails with an OptimisticLockingException
            // and the job retries have not been decremented
            Debug.WriteLine("test thread notifies thread 2");
            threadTwo.proceedAndWaitTillDone();
            Assert.NotNull(threadTwo.exception);

            IJob remainingJob = managementService.CreateJobQuery().First();
            Assert.AreEqual(currentJobs[1].Retries, remainingJob.Retries);

            Assert.NotNull(remainingJob.ExceptionMessage);

            JobEntity jobEntity = (JobEntity)remainingJob;
            Assert.IsNull(jobEntity.LockOwner);

            // and there is no lock expiration time due to the default retry strategy
            Assert.IsNull(jobEntity.LockExpirationTime);
        }

        [Test]
        [Deployment]
        public virtual void testCompetingJobExecutionFoxRetryStrategy()
        {
            // given an MI subprocess with two instances
            runtimeService.StartProcessInstanceByKey("miParallelSubprocess");

            IList<IJob> currentJobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, currentJobs.Count);

            // when the jobs are executed in parallel
            JobExecutionThread threadOne = new JobExecutionThread(this, currentJobs[0].Id);
            threadOne.startAndWaitUntilControlIsReturned();

            JobExecutionThread threadTwo = new JobExecutionThread(this, currentJobs[1].Id);
            threadTwo.startAndWaitUntilControlIsReturned();

            // then the first committing thread succeeds
            Debug.WriteLine("test thread notifies thread 1");
            threadOne.proceedAndWaitTillDone();
            Assert.IsNull(threadOne.exception);

            // then the second committing thread fails with an OptimisticLockingException
            // and the job retries have not been decremented
            Debug.WriteLine("test thread notifies thread 2");
            threadTwo.proceedAndWaitTillDone();
            Assert.NotNull(threadTwo.exception);

            IJob remainingJob = managementService.CreateJobQuery().First();
            // retries are configured as R5/PT5M, so no decrement means 5 retries left
            Assert.AreEqual(5, remainingJob.Retries);

            Assert.NotNull(remainingJob.ExceptionMessage);

            JobEntity jobEntity = (JobEntity)remainingJob;
            Assert.IsNull(jobEntity.LockOwner);

            // and there is a custom lock expiration time
            Assert.NotNull(jobEntity.LockExpirationTime);
        }

        [Test]
        public virtual void testCompletingJobExecutionSuspendDuringExecution()
        {
            testRule.Deploy(SIMPLE_ASYNC_PROCESS);

            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            IJob job = managementService.CreateJobQuery().First();

            // given a waiting execution and a waiting suspension
            JobExecutionThread executionthread = new JobExecutionThread(this, job.Id);
            executionthread.startAndWaitUntilControlIsReturned();

            JobSuspensionThread jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
            jobSuspensionThread.startAndWaitUntilControlIsReturned();

            // first complete suspension:
            jobSuspensionThread.proceedAndWaitTillDone();
            executionthread.proceedAndWaitTillDone();

            // then the execution will Assert.Fail with optimistic locking
            Assert.IsNull(jobSuspensionThread.exception);
            Assert.NotNull(executionthread.exception);

            //--------------------------------------------

            // given a waiting execution and a waiting suspension
            executionthread = new JobExecutionThread(this, job.Id);
            executionthread.startAndWaitUntilControlIsReturned();

            jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
            jobSuspensionThread.startAndWaitUntilControlIsReturned();

            // first complete execution:
            executionthread.proceedAndWaitTillDone();
            jobSuspensionThread.proceedAndWaitTillDone();

            // then there are no optimistic locking exceptions
            Assert.IsNull(jobSuspensionThread.exception);
            Assert.IsNull(executionthread.exception);
        }

        [Test]
        public virtual void testCompletingSuspendJobDuringAcquisition()
        {
            testRule.Deploy(SIMPLE_ASYNC_PROCESS);

            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // given a waiting acquisition and a waiting suspension
            JobAcquisitionThread acquisitionThread = new JobAcquisitionThread(this);
            acquisitionThread.startAndWaitUntilControlIsReturned();

            JobSuspensionThread jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
            jobSuspensionThread.startAndWaitUntilControlIsReturned();

            // first complete suspension:
            jobSuspensionThread.proceedAndWaitTillDone();
            acquisitionThread.proceedAndWaitTillDone();

            // then the acquisition will not Assert.Fail with optimistic locking
            Assert.IsNull(jobSuspensionThread.exception);
            Assert.IsNull(acquisitionThread.exception);
            // but the job will also not be acquired
            Assert.AreEqual(0, acquisitionThread.acquiredJobs.Size());

            //--------------------------------------------

            // given a waiting acquisition and a waiting suspension
            acquisitionThread = new JobAcquisitionThread(this);
            acquisitionThread.startAndWaitUntilControlIsReturned();

            jobSuspensionThread = new JobSuspensionThread(this, "simpleAsyncProcess");
            jobSuspensionThread.startAndWaitUntilControlIsReturned();

            // first complete acquisition:
            acquisitionThread.proceedAndWaitTillDone();
            jobSuspensionThread.proceedAndWaitTillDone();

            // then there are no optimistic locking exceptions
            Assert.IsNull(jobSuspensionThread.exception);
            Assert.IsNull(acquisitionThread.exception);
        }

        [Test]
        public virtual void testCompletingSuspendedJobDuringRunningInstance()
        {
            testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ReceiveTask().IntermediateCatchEvent().TimerWithDuration("PT0M").EndEvent().Done());

            // given
            // a process definition
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();

            // a running instance
            IProcessInstance processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);

            // suspend the process definition (and the job definitions)
            repositoryService.SuspendProcessDefinitionById(processDefinition.Id);

            // Assert that there still exists a running and active process instance
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

            // when
            runtimeService.Signal(processInstance.Id);

            // then
            // there should be one suspended job
            Assert.AreEqual(1, managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Suspended.StateCode).Count());
            Assert.AreEqual(0, managementService.CreateJobQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery(c=>c.SuspensionState == SuspensionStateFields.Active.StateCode).Count());

        }

        [Test]
        public virtual void testCompletingUpdateJobDefinitionPriorityDuringExecution()
        {
            testRule.Deploy(SIMPLE_ASYNC_PROCESS);

            // given
            // two running instances
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // and a job definition
            IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

            // and two jobs
            IList<IJob> jobs = managementService.CreateJobQuery().ToList();

            // when the first job is executed but has not yet committed
            JobExecutionThread executionThread = new JobExecutionThread(this, jobs[0].Id);
            executionThread.startAndWaitUntilControlIsReturned();

            // and the job priority is updated
            JobDefinitionPriorityThread priorityThread = new JobDefinitionPriorityThread(this, jobDefinition.Id, 42L, true);
            priorityThread.startAndWaitUntilControlIsReturned();

            // and the priority threads commits first
            priorityThread.proceedAndWaitTillDone();

            // then both jobs priority has changed
            IList<IJob> currentJobs = managementService.CreateJobQuery().ToList();
            foreach (IJob job in currentJobs)
            {
                Assert.AreEqual(42, job.Priority);
            }

            // and the execution thread can nevertheless successfully finish job execution
            executionThread.proceedAndWaitTillDone();

            Assert.IsNull(executionThread.exception);

            // and ultimately only one job with an updated priority is left
            IJob remainingJob = managementService.CreateJobQuery().First();
            Assert.NotNull(remainingJob);
        }

        [Test]
        public virtual void testCompletingSuspensionJobDuringPriorityUpdate()
        {
            testRule.Deploy(SIMPLE_ASYNC_PROCESS);

            // given
            // two running instances (ie two jobs)
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");
            runtimeService.StartProcessInstanceByKey("simpleAsyncProcess");

            // a job definition
            IJobDefinition jobDefinition = managementService.CreateJobDefinitionQuery().First();

            // when suspending the jobs is attempted
            JobSuspensionByJobDefinitionThread suspensionThread = new JobSuspensionByJobDefinitionThread(this, jobDefinition.Id);
            suspensionThread.startAndWaitUntilControlIsReturned();

            // and updating the priority is attempted
            JobDefinitionPriorityThread priorityUpdateThread = new JobDefinitionPriorityThread(this, jobDefinition.Id, 42L, true);
            priorityUpdateThread.startAndWaitUntilControlIsReturned();

            // and both commands overlap each other
            suspensionThread.proceedAndWaitTillDone();
            priorityUpdateThread.proceedAndWaitTillDone();

            // then both updates have been performed
            IList<IJob> updatedJobs = managementService.CreateJobQuery().ToList();
            Assert.AreEqual(2, updatedJobs.Count);
            foreach (IJob job in updatedJobs)
            {
                Assert.AreEqual(42, job.Priority);
                Assert.True(job.Suspended);
            }
        }


        public class JobExecutionThread : ControllableThread
        {
            private readonly ConcurrentJobExecutorTest outerInstance;


            internal OptimisticLockingException exception;
            internal string jobId;

            internal JobExecutionThread(ConcurrentJobExecutorTest outerInstance, string jobId)
            {
                this.outerInstance = outerInstance;
                this.jobId = jobId;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread
            //public override void run()
            //{
            //    try
            //    {
            //        JobFailureCollector jobFailureCollector = new JobFailureCollector(jobId);
            //        ExecuteJobHelper.ExecuteJob(jobId, outerInstance.processEngineConfiguration.CommandExecutorTxRequired, jobFailureCollector, new ControlledICommand<object>(activeThread, new ExecuteJobsCmd(jobId, jobFailureCollector)));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}
        }

        public class JobAcquisitionThread : ControllableThread
        {
            private readonly ConcurrentJobExecutorTest outerInstance;

            public JobAcquisitionThread(ConcurrentJobExecutorTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            internal OptimisticLockingException exception;
            internal AcquiredJobs acquiredJobs;
            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread
            //public override void run()
            //{
            //    try
            //    {
            //        JobExecutor jobExecutor = outerInstance.processEngineConfiguration.JobExecutor;
            //        acquiredJobs = outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<AcquiredJobs>(activeThread, new AcquireJobsCmd(jobExecutor)));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}
        }

        public class JobSuspensionThread : ControllableThread
        {
            private readonly ConcurrentJobExecutorTest outerInstance;

            internal OptimisticLockingException exception;
            internal string processDefinitionKey;

            public JobSuspensionThread(ConcurrentJobExecutorTest outerInstance, string processDefinitionKey)
            {
                this.outerInstance = outerInstance;
                this.processDefinitionKey = processDefinitionKey;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread
            //public override void run()
            //{
            //    try
            //    {
            //        outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledICommand<object>(activeThread, createSuspendJobCommand()));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}

            protected internal virtual ICommand<object> createSuspendJobCommand()
            {
                IUpdateJobDefinitionSuspensionStateBuilder builder = (new UpdateJobDefinitionSuspensionStateBuilderImpl()).ByProcessDefinitionKey(processDefinitionKey);//.IncludeJobs(true);

                return new SuspendJobDefinitionCmd(builder);
            }
        }

        public class JobSuspensionByJobDefinitionThread : ControllableThread
        {
            private readonly ConcurrentJobExecutorTest outerInstance;

            internal OptimisticLockingException exception;
            internal string jobDefinitionId;

            public JobSuspensionByJobDefinitionThread(ConcurrentJobExecutorTest outerInstance, string jobDefinitionId)
            {
                this.outerInstance = outerInstance;
                this.jobDefinitionId = jobDefinitionId;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread
            //public override void run()
            //{
            //    try
            //    {
            //        outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledICommand<object>(activeThread, createSuspendJobCommand()));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}

            protected internal virtual SuspendJobCmd createSuspendJobCommand()
            {
                IUpdateJobSuspensionStateBuilder builder = (new UpdateJobSuspensionStateBuilderImpl()).ByJobDefinitionId(jobDefinitionId);
                return new SuspendJobCmd(builder);
            }
        }

        public class JobDefinitionPriorityThread : ControllableThread
        {
            private readonly ConcurrentJobExecutorTest outerInstance;

            internal OptimisticLockingException exception;
            internal string jobDefinitionId;
            internal long? priority;
            internal bool cascade;

            public JobDefinitionPriorityThread(ConcurrentJobExecutorTest outerInstance, string jobDefinitionId, long? priority, bool cascade)
            {
                this.outerInstance = outerInstance;
                this.jobDefinitionId = jobDefinitionId;
                this.priority = priority;
                this.cascade = cascade;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread
            //public override void run()
            //{
            //    try
            //    {
            //        outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledICommand<object>(activeThread, new SetJobDefinitionPriorityCmd(jobDefinitionId, priority, cascade)));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //}
        }

    }

}