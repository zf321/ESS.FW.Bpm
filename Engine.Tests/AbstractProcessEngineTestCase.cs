using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public abstract class AbstractProcessEngineTestCase : PvmTestCase
    {
        protected internal static IAuthorizationService authorizationService;
        protected internal ICaseService caseService;
        protected internal IDecisionService decisionService;

        protected internal string DeploymentId;
        protected internal IList<string> DeploymentIds = new List<string>();

        protected internal System.Exception Exception;
        protected internal IExternalTaskService externalTaskService;
        protected internal IFilterService filterService;
        protected internal IFormService formService;
        protected internal IHistoryService historyService;
        protected internal IIdentityService identityService;
        protected internal IManagementService managementService;

        protected internal IProcessEngine ProcessEngine;

        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;
        protected internal IRepositoryService repositoryService;
        protected internal IRuntimeService runtimeService;
        protected internal ITaskService taskService;

        protected static readonly TestLogger Log = TestLogger.Logger;

        protected internal abstract void InitializeProcessEngine();
        /// <summary>
        /// 是否每次清理所有Deployment相关数据
        /// </summary>
        protected bool ClearDeploymentAll = false;

        // Default: do nothing
        protected internal virtual void CloseDownProcessEngine()
        {
        }
        protected event Action SetUpAfterEvent;
        protected event Action TearDownAfterEvent;
        [SetUp]
        public void RunBare()
        {
            InitializeProcessEngine();
            if (repositoryService == null)
                InitializeServices();

            //try
            //{
            bool hasRequiredHistoryLevel = TestHelper.AnnotationRequiredHistoryLevelCheck(ProcessEngine, this.GetType(), TestContext.CurrentContext.Test.Name);
            //ignore test case when current history level is too low
            if (hasRequiredHistoryLevel)
            {

                DeploymentId = TestHelper.AnnotationDeploymentSetUp(ProcessEngine, this.GetType(), TestContext.CurrentContext.Test.Name.Split('.').Last());

                //base.RunBare();
                if (SetUpAfterEvent != null) SetUpAfterEvent.Invoke();
            }
            //}
            //catch (Exception e)
            //{
            //    Debug.WriteLine("ASSERTION FAILED: " + e, e);
            //    Exception = e;
            //    throw e;
            //}
            //finally
            //{
            //    //TearDown();
            //}

        }

        [TearDown]
        public virtual void TearDown()
        {
            if (TearDownAfterEvent != null)
            {
                TearDownAfterEvent.Invoke();
            }
            identityService.ClearAuthentication();
            //processEngineConfiguration.SetTenantCheckEnabled(true);

            DeleteDeployments();
            DeleteHistoryCleanupJob();

            // only fail if no test failure was recorded
            TestHelper.AssertAndEnsureCleanDbAndCache(ProcessEngine, Exception == null);
            //TestHelper.ResetIdGenerator(processEngineConfiguration);
            ClockUtil.Reset();

            // Can't do this in the teardown, as the teardown will be called as part
            // of the super.RunBare
            CloseDownProcessEngine();
            ClearServiceReferences();

        }
        private void DeleteHistoryCleanupJob()
        {
            IJob job = historyService.FindHistoryCleanupJob();
            if (job != null)
            {
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(job));
            }
        }

        protected internal virtual void DeleteDeployments()
        {
            if (DeploymentId != null)
                DeploymentIds.Add(DeploymentId);
            if (ClearDeploymentAll)
            {
                DeploymentIds = repositoryService.CreateDeploymentQuery().Select(m => m.Id).ToList();
            }

            foreach (string deploymentId in DeploymentIds)
            {
                TestHelper.AnnotationDeploymentTearDown(ProcessEngine, deploymentId, this.GetType(), TestContext.CurrentContext.Test.Name);
            }
            DeploymentId = null;
            DeploymentIds.Clear();


        }

        protected internal virtual void InitializeServices()
        {
            processEngineConfiguration = ProcessEngine.ProcessEngineConfiguration as ProcessEngineConfigurationImpl;
            repositoryService = ProcessEngine.RepositoryService;
            runtimeService = ProcessEngine.RuntimeService;
            taskService = ProcessEngine.TaskService;
            formService = ProcessEngine.FormService;
            historyService = ProcessEngine.HistoryService;
            identityService = ProcessEngine.IdentityService;
            managementService = ProcessEngine.ManagementService;
            authorizationService = ProcessEngine.AuthorizationService;
            caseService = ProcessEngine.CaseService;
            filterService = ProcessEngine.FilterService;
            externalTaskService = ProcessEngine.ExternalTaskService;
            decisionService = ProcessEngine.DecisionService;
        }

        protected internal virtual void ClearServiceReferences()
        {
            processEngineConfiguration = null;
            repositoryService = null;
            runtimeService = null;
            taskService = null;
            formService = null;
            historyService = null;
            identityService = null;
            managementService = null;
            authorizationService = null;
            caseService = null;
            filterService = null;
            externalTaskService = null;
            decisionService = null;
        }

        public virtual void AssertProcessEnded(string processInstanceId)
        {
            var processInstance =
                ProcessEngine.RuntimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstanceId)
                    .FirstOrDefault();

            if (processInstance != null)
                throw new System.Exception("Expected finished process instance '" + processInstanceId +
                                    "' but it was still in the db");
        }

        public virtual void AssertProcessNotEnded(string processInstanceId)
        {
            var processInstance =
                ProcessEngine.RuntimeService.CreateProcessInstanceQuery(c => c.ProcessInstanceId == processInstanceId)
                    .FirstOrDefault();

            if (processInstance == null)
                throw new System.Exception("Expected process instance '" + processInstanceId +
                                    "' to be still active but it was not in the db");
        }

        public virtual void AssertCaseEnded(string caseInstanceId)
        {
            //ICaseInstance caseInstance = ProcessEngine.CaseService.CreateCaseInstanceQuery(c=>c.CaseInstanceId ==caseInstanceId).First();

            //if (caseInstance != null)
            //{
            //    throw new Exception("Expected finished case instance '" + caseInstanceId + "' but it was still in the db");
            //}
        }        

        public virtual void WaitForJobExecutorToProcessAllJobs(long maxMillisToWait)
        {
            var jobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor.Start();
            int intervalMillis = 1000;

            var jobExecutorWaitTime = jobExecutor.WaitTimeInMillis * 2;
            if (maxMillisToWait < jobExecutorWaitTime)
                maxMillisToWait = jobExecutorWaitTime;

            try
            {                
                InterruptTask task = new InterruptTask(Thread.CurrentThread);
                Timer timer = new Timer(task.Run, null, maxMillisToWait, 0);

                bool areJobsAvailable = true;
                try
                {
                    while (areJobsAvailable && !task.TimeLimitExceeded)
                    {
                        Thread.Sleep(intervalMillis);
                        try
                        {
                            areJobsAvailable = AreJobsAvailable();
                        }
                        catch (System.Exception e)
                        {
                            // Ignore, possible that exception occurs due to locking/updating of table on MSSQL when
                            // isolation level doesn't allow READ of the table
                            string msg = "error";
                        }
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    var msg = e.Message;
                }
                finally
                {
                    timer.Change(-1, 0);
                }
                if (areJobsAvailable)
                {
                    throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
                }
            }
            finally
            {
                jobExecutor.Shutdown();
            }
        }
        
        public virtual void WaitForJobExecutorOnCondition(long maxMillisToWait, Func<bool> condition)
        {
            var jobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor.Start();
            int intervalMillis = 500;

            if (maxMillisToWait < (jobExecutor.WaitTimeInMillis * 2))
            {
                maxMillisToWait = (jobExecutor.WaitTimeInMillis * 2);
            }

            try
            {
                InterruptTask task = new InterruptTask(Thread.CurrentThread);
                System.Threading.Timer timer = new Timer(task.Run, null, maxMillisToWait, 0);

                bool conditionIsViolated = true;
                try
                {
                    while (conditionIsViolated && !task.TimeLimitExceeded)
                    {
                        Thread.Sleep(intervalMillis);
                        conditionIsViolated = !condition();
                    }
                }
                catch (ThreadInterruptedException e)
                {
                }
                catch (System.Exception e)
                {
                    throw new ProcessEngineException("Exception while waiting on condition: " + e.Message, e);
                }
                finally
                {
                    timer.Change(-1, 0);
                }
                if (conditionIsViolated)
                {
                    throw new ProcessEngineException("time limit of " + maxMillisToWait + " was exceeded");
                }

            }
            finally
            {
                jobExecutor.Shutdown();
            }
        }

        /// <summary>
        ///     Execute all available jobs recursively till no more jobs found.
        /// </summary>
        public virtual void ExecuteAvailableJobs()
        {
            ExecuteAvailableJobs(0, int.MaxValue, true);
        }

        /// <summary>
        ///     Execute all available jobs recursively till no more jobs found or the number of executions is higher than expected.
        /// </summary>
        /// <param name="expectedExecutions">
        ///     number of expected job executions
        /// </param>
        /// <exception cref="System.exception">
        ///     when execute less or more jobs than expected
        /// </exception>
        /// <seealso cref= # ExecuteAvailableJobs
        /// (
        /// )
        /// </seealso>
        public virtual void ExecuteAvailableJobs(int expectedExecutions)
        {
            ExecuteAvailableJobs(0, expectedExecutions, false);
        }

        private void ExecuteAvailableJobs(int jobsExecuted, int expectedExecutions, bool ignoreLessExecutions)
        {
            IList<IJob> jobs = managementService.CreateJobQuery(c => c.RetriesFromPersistence > 0).ToList();//.CreateJobQuery(c=>c.Retries > 0).ToList();

            if (jobs.Count == 0)
            {
                Assert.True(jobsExecuted == expectedExecutions || ignoreLessExecutions, "executed less jobs than expected. expected <" + expectedExecutions + "> actual <" + jobsExecuted + ">");
                return;
            }

            foreach (IJob job in jobs)
            {
                try
                {
                    managementService.ExecuteJob(job.Id);
                    jobsExecuted += 1;
                }
                catch (System.Exception ex)
                {
                }
            }

            Assert.True(jobsExecuted <= expectedExecutions,
                "executed more jobs than expected. expected <" + expectedExecutions + "> actual <" + jobsExecuted + ">");

            ExecuteAvailableJobs(jobsExecuted, expectedExecutions, ignoreLessExecutions);
        }

        public virtual bool AreJobsAvailable()
        {
            IList<IJob> list = managementService.CreateJobQuery().ToList();
            foreach (IJob job in list)
            {
                if (!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime.Ticks > job.Duedate.Value.Ticks))
                {
                    return true;
                }
            }
            return false;
        }

        private class InterruptTask 
        {
            private bool _timeLimitExceeded = false;
            private Thread _thread;
            public InterruptTask(Thread thread)
            {
                this._thread = thread;
            }
            public virtual bool TimeLimitExceeded => _timeLimitExceeded;

            public void Run(object state)
            {
                _timeLimitExceeded = true;
                _thread.Interrupt();
            }
        }

        [Obsolete]
        protected internal virtual IList<IActivityInstance> GetInstancesForActivitiyId(
            IActivityInstance activityInstance, string activityId)
        {
            return GetInstancesForActivityId(activityInstance, activityId);
        }

        protected internal virtual IList<IActivityInstance> GetInstancesForActivityId(
            IActivityInstance activityInstance, string activityId)
        {
            IList<IActivityInstance> result = new List<IActivityInstance>();
            if (activityInstance.ActivityId.Equals(activityId))
                result.Add(activityInstance);
            foreach (var childInstance in activityInstance.ChildActivityInstances)
                ((List<IActivityInstance>)result).AddRange(GetInstancesForActivityId(childInstance, activityId));
            return result;
        }

        protected internal virtual void RunAsUser(string userId, IList<string> groupIds, ThreadStart r)
        {
            try
            {
                identityService.AuthenticatedUserId = userId;
                //processEngineConfiguration.SetAuthorizationEnabled(true);

                //r.Run();
            }
            finally
            {
                identityService.AuthenticatedUserId = null;
                //processEngineConfiguration.SetAuthorizationEnabled(false);
            }
        }

        protected internal virtual string Deployment(params IBpmnModelInstance[] bpmnModelInstances)
        {
            var deploymentBuilder = repositoryService.CreateDeployment();

            return Deployment(deploymentBuilder, bpmnModelInstances);
        }

        protected internal virtual string Deployment(params string[] resources)
        {
            var deploymentBuilder = repositoryService.CreateDeployment();

            return Deployment(deploymentBuilder, resources);
        }

        protected internal virtual string DeploymentForTenant(string tenantId,
            params IBpmnModelInstance[] bpmnModelInstances)
        {
            var deploymentBuilder = repositoryService.CreateDeployment().TenantId(tenantId);

            return Deployment(deploymentBuilder, bpmnModelInstances);
        }

        protected internal virtual string DeploymentForTenant(string tenantId, params string[] resources)
        {
            var deploymentBuilder = repositoryService.CreateDeployment().TenantId(tenantId);

            return Deployment(deploymentBuilder, resources);
        }

        protected internal virtual string DeploymentForTenant(string tenantId, string classpathResource,
            IBpmnModelInstance modelInstance)
        {
            return
                Deployment(
                    repositoryService.CreateDeployment().TenantId(tenantId).AddClasspathResource(classpathResource),
                    modelInstance);
        }

        protected internal virtual string Deployment(IDeploymentBuilder deploymentBuilder,
            params IBpmnModelInstance[] bpmnModelInstances)
        {
            for (var i = 0; i < bpmnModelInstances.Length; i++)
            {
                var bpmnModelInstance = bpmnModelInstances[i];
                deploymentBuilder.AddModelInstance("testProcess-" + i + ".bpmn", bpmnModelInstance);
            }

            return DeploymentWithBuilder(deploymentBuilder);
        }

        protected internal virtual string Deployment(IDeploymentBuilder deploymentBuilder, params string[] resources)
        {
            for (var i = 0; i < resources.Length; i++)
                deploymentBuilder.AddClasspathResource(resources[i]);

            return DeploymentWithBuilder(deploymentBuilder);
        }

        protected internal virtual string DeploymentWithBuilder(IDeploymentBuilder builder)
        {
            DeploymentId = builder.Deploy().Id;
            DeploymentIds.Add(DeploymentId);

            return DeploymentId;
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {

            private readonly IJob _job;

            public CommandAnonymousInnerClass(IJob job)
            {
                _job = job;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.JobManager.DeleteJob((JobEntity)_job);
                return null;
            }
        }
    }
}