using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Util
{


    public class ProcessEngineTestRule /*: TestWatcher*/
    {

        public const string DefaultBpmnResourceName = "process.bpmn20.xml";

        protected internal ProcessEngineRule engineRule;
        protected internal IProcessEngine ProcessEngine;
        public ProcessEngineTestRule( )
        {
        }
        public ProcessEngineTestRule(ProcessEngineRule processEngineRule)
        {
            this.engineRule = processEngineRule;
        }

        [SetUp]
        public   void Starting()
        {
            engineRule.Starting();
            this.ProcessEngine = engineRule.ProcessEngine;
        }
        [TearDown]
        public   void Finished()
        {
            engineRule.Finished();
            this.ProcessEngine = null;
        }


        public virtual void AssertProcessEnded(string processInstanceId)
        {
            IProcessInstance processInstance = ProcessEngine.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == processInstanceId).First();

            Assert.That(processInstance, Is.EqualTo(null), "Process instance with id " + processInstanceId + " is not finished");
        }

        public virtual void AssertCaseEnded(string caseInstanceId)
        {
            //ICaseInstance caseInstance = ProcessEngine.CaseService.CreateCaseInstanceQuery(c=>c.CaseInstanceId ==caseInstanceId).First();

            //Assert.That("Case instance with id " + caseInstanceId + " is not finished", caseInstance, Is.EqualTo(null));
        }

        public virtual IDeploymentWithDefinitions Deploy(params IBpmnModelInstance[] bpmnModelInstances)
        {
            return Deploy(CreateDeploymentBuilder(), (bpmnModelInstances), new List<string>());
        }

        public virtual IDeploymentWithDefinitions Deploy(params string[] resources)
        {
            return Deploy(CreateDeploymentBuilder(), new List<IBpmnModelInstance>(), resources);
            //return null;
            //return Deploy(CreateDeploymentBuilder(), System.Linq.Enumerable.Empty<IBpmnModelInstance>(), (resources));
        }

        public virtual IDeploymentWithDefinitions Deploy(IDeploymentBuilder deploymentBuilder)
        {
            IDeploymentWithDefinitions deployment = deploymentBuilder.DeployWithResult();

            engineRule.ManageDeployment(deployment);

            return deployment;
        }

        public virtual IDeployment Deploy(IBpmnModelInstance bpmnModelInstance, string resource)
        {
            return Deploy(CreateDeploymentBuilder(), new List<IBpmnModelInstance>() { bpmnModelInstance }, new List<string> { resource });
        }

        public virtual IDeployment DeployForTenant(string tenantId, params IBpmnModelInstance[] bpmnModelInstances)
        {
            return Deploy(CreateDeploymentBuilder().TenantId(tenantId), (bpmnModelInstances), new List<string>());
        }

        public virtual IDeployment DeployForTenant(string tenantId, params string[] resources)
        {
            return Deploy(CreateDeploymentBuilder().TenantId(tenantId), new List<IBpmnModelInstance>(), (resources));
        }

        public virtual IDeployment DeployForTenant(string tenant, IBpmnModelInstance bpmnModelInstance, string resource)
        {
            return Deploy(CreateDeploymentBuilder().TenantId(tenant), new List<IBpmnModelInstance>(){bpmnModelInstance}, new List<string>{resource});
        }

        public virtual IProcessDefinition DeployAndGetDefinition(IBpmnModelInstance bpmnModel)
        {
            return DeployForTenantAndGetDefinition(null, bpmnModel);
        }

        public virtual IProcessDefinition DeployForTenantAndGetDefinition(string tenant, IBpmnModelInstance bpmnModel)
        {
            IDeployment deployment = Deploy(CreateDeploymentBuilder().TenantId(tenant),new List<IBpmnModelInstance>(){ bpmnModel }, new List<string>());

            return engineRule.RepositoryService.CreateProcessDefinitionQuery(c => c.DeploymentId == deployment.Id).First();
        }

        protected internal virtual IDeploymentWithDefinitions Deploy(IDeploymentBuilder deploymentBuilder, IList<IBpmnModelInstance> bpmnModelInstances, IList<string> resources)
        {
            int i = 0;
            foreach (IBpmnModelInstance bpmnModelInstance in bpmnModelInstances)
            {
                deploymentBuilder.AddModelInstance(i + "_" + DefaultBpmnResourceName, bpmnModelInstance);
                i++;
            }

            foreach (string resource in resources)
            {
                deploymentBuilder.AddClasspathResource(resource);
            }

            return Deploy(deploymentBuilder);
        }

        protected internal virtual IDeploymentBuilder CreateDeploymentBuilder()
        {
            return ProcessEngine.RepositoryService.CreateDeployment();
        }

        public virtual void WaitForJobExecutorToProcessAllJobs()
        {
            WaitForJobExecutorToProcessAllJobs(0);
        }

        public virtual void WaitForJobExecutorToProcessAllJobs(long maxMillisToWait)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngine.ProcessEngineConfiguration;
            ESS.FW.Bpm.Engine.Impl.JobExecutor.JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
            jobExecutor.Start();
            int intervalMillis = 1000;

            int jobExecutorWaitTime = jobExecutor.WaitTimeInMillis * 2;
            if (maxMillisToWait < jobExecutorWaitTime)
            {
                maxMillisToWait = jobExecutorWaitTime;
            }

            try
            {
                InterruptTask task = new InterruptTask(Thread.CurrentThread);
                System.Threading.Timer timer = new System.Threading.Timer(task.Run, null, maxMillisToWait, 0);
                
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
                        catch (System.Exception ex)
                        {
                            // Ignore, possible that exception occurs due to locking/updating of table on MSSQL when
                            // isolation level doesn't allow READ of the table
                        }
                    }
                }
                catch (ThreadInterruptedException e)
                {
                }
                finally
                {
                    timer.Change(-1, 0);
                }
                if(areJobsAvailable)
                    throw new AssertionException("time limit of " + maxMillisToWait + " was exceeded");
            }
            finally
            {
                jobExecutor.Shutdown();
            }

            //try
            //{
            //    Timer timer = new Timer();
            //    InterruptTask task = new InterruptTask(Thread.CurrentThread);
            //    timer.Schedule(task, maxMillisToWait);
            //    bool areJobsAvailable = true;
            //    try
            //    {
            //        while (areJobsAvailable && !task.TimeLimitExceeded)
            //        {
            //            Thread.Sleep(intervalMillis);
            //            try
            //            {
            //                areJobsAvailable = areJobsAvailable();
            //            }
            //            catch (Exception)
            //            {
            //                // Ignore, possible that exception occurs due to locking/updating of table on MSSQL when
            //                // isolation level doesn't allow Permissions.Read of the table
            //            }
            //        }
            //    }
            //    //catch (InterruptedException)
            //    //{
            //    //}
            //    finally
            //    {
            //        timer.Cancel();
            //    }
            //    if (areJobsAvailable)
            //    {
            //        throw new AssertionError("time limit of " + maxMillisToWait + " was exceeded");
            //    }

            //}
            //finally
            //{
            //    jobExecutor.Shutdown();
            //}
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected internal virtual bool AreJobsAvailable()
        {
            IList<IJob> list = ProcessEngine.ManagementService.CreateJobQuery().ToList();
            foreach (IJob job in list)
            {
                
                if (!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime.Ticks>job.Duedate.Value.Ticks))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Execute all available jobs recursively till no more jobs found.
        /// </summary>
        public virtual void ExecuteAvailableJobs()
        {
            ExecuteAvailableJobs(0, int.MaxValue);
        }

        /// <summary>
        /// Execute all available jobs recursively till no more jobs found or the number of executions is higher than expected.
        /// </summary>
        /// <param name="expectedExecutions"> number of expected job executions
        /// </param>
        /// <exception cref="AssertionAssert.FailedError"> when execute less or more jobs than expected
        /// </exception>
        /// <seealso cref= #ExecuteAvailableJobs() </seealso>
        public virtual void ExecuteAvailableJobs(int expectedExecutions)
        {
            ExecuteAvailableJobs(0, expectedExecutions);
        }

        private void ExecuteAvailableJobs(int jobsExecuted, int expectedExecutions)
        {
            //IList<IJob> jobs = ProcessEngine.ManagementService.CreateJobQuery(c=>c.Retries > 0).ToList();

            //if (jobs.Count == 0)
            //{
            //    if (expectedExecutions != int.MaxValue)
            //    {
            //        Assert.That("executed less jobs than expected.", jobsExecuted, Is.EqualTo(expectedExecutions));
            //    }
            //    return;
            //}

            //foreach (IJob job in jobs)
            //{
            //    try
            //    {
            //        ProcessEngine.ManagementService.ExecuteJob(job.Id);
            //        jobsExecuted += 1;
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            //Assert.That("executed more jobs than expected.", jobsExecuted, LessThanOrEqualTo(expectedExecutions));

            ExecuteAvailableJobs(jobsExecuted, expectedExecutions);
        }

        public virtual void CompleteTask(string taskKey)
        {
            ITaskService taskService = ProcessEngine.TaskService;
            ITask task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==taskKey).First();
            Assert.NotNull( task,"Expected a task with key '" + taskKey + "' to exist");
            taskService.Complete(task.Id);
        }

        public virtual void CompleteAnyTask(string taskKey)
        {
            ITaskService taskService = ProcessEngine.TaskService;
            //IList<ITask> tasks = taskService.CreateTaskQuery(c=>c.TaskDefinitionKey ==taskKey).ToList();
            //Assert.True(tasks.Count > 0);
            //taskService.Complete(tasks[0].Id);
        }

        public virtual string AnyVariable
        {
            set
            {
                SetVariable(value, "any", "any");
            }
        }

        public virtual void SetVariable(string executionId, string varName, object varValue)
        {
            ProcessEngine.RuntimeService.SetVariable(executionId, varName, varValue);
        }

        public virtual void CorrelateMessage(string messageName)
        {
            ProcessEngine.RuntimeService.CreateMessageCorrelation(messageName).Correlate();
        }

        public virtual void SendSignal(string signalName)
        {
            ProcessEngine.RuntimeService.SignalEventReceived(signalName);
        }

        public virtual bool HistoryLevelNone
        {
            get
            {
                IHistoryLevel historyLevel = engineRule.ProcessEngineConfiguration.HistoryLevel;
                return HistoryLevelFields.HistoryLevelNone.Equals(historyLevel);
            }
        }

        public virtual bool HistoryLevelActivity
        {
            get
            {
                IHistoryLevel historyLevel = engineRule.ProcessEngineConfiguration.HistoryLevel;
                return HistoryLevelFields.HistoryLevelActivity.Equals(historyLevel);
            }
        }

        public virtual bool HistoryLevelAudit
        {
            get
            {
                IHistoryLevel historyLevel = engineRule.ProcessEngineConfiguration.HistoryLevel;
                return HistoryLevelFields.HistoryLevelAudit.Equals(historyLevel);
            }
        }

        public virtual bool HistoryLevelFull
        {
            get
            {
                IHistoryLevel historyLevel = engineRule.ProcessEngineConfiguration.HistoryLevel;
                return HistoryLevelFields.HistoryLevelFull.Equals(historyLevel);
            }
        }

        protected internal class InterruptTask 
        {
            protected internal bool timeLimitExceeded = false;
            protected internal Thread thread;
            public InterruptTask(Thread thread)
            {
                this.thread = thread;
            }
            public virtual bool TimeLimitExceeded => timeLimitExceeded;

            public void Run(object state)
            {
                timeLimitExceeded = true;
                thread.Interrupt();
            }
        }

    }

}