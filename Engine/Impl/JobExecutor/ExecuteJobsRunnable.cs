using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class ExecuteJobsRunnable 
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;
        

        private readonly IList<string> _jobIds;
        private JobExecutor _jobExecutor;
        private ProcessEngineImpl _processEngine;

        public ExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            _jobIds = jobIds;
            _processEngine = processEngine;
            _jobExecutor = ((ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration).JobExecutor;
        }

        public virtual void Run(object state)
        {
            var jobExecutorContext = new JobExecutorContext();

            var currentProcessorJobQueue = jobExecutorContext.CurrentProcessorJobQueue;
            ICommandExecutor commandExecutor = ((ProcessEngineConfigurationImpl)_processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired;
            _jobIds.ForEach(d =>
            {
                currentProcessorJobQueue.Enqueue(d);
            });

            Context.JobExecutorContext = jobExecutorContext;
            try
            {
                while (!currentProcessorJobQueue.IsEmpty())
                {
                    string nextJobId = currentProcessorJobQueue.Dequeue();
                    if (_jobExecutor.IsActive)
                    {
                        try
                        {
                            ExecuteJob(nextJobId, commandExecutor);
                        }
                        catch (System.Exception t)
                        {
                            Log.ExceptionWhileExecutingJob(nextJobId, t);
                        }
                    }
                    else
                    {
                        try
                        {
                            UnlockJob(nextJobId, commandExecutor);
                        }
                        catch (System.Exception t)
                        {
                            Log.ExceptionWhileUnlockingJob(nextJobId, t);
                        }
                    }
                    currentProcessorJobQueue.TrimExcess();
                }

                // if there were only exclusive jobs then the job executor
                // does a backoff. In order to avoid too much waiting time
                // we need to tell him to check once more if there were any jobs added.
                _jobExecutor.JobWasAdded();
            }
            finally
            {
                Context.RemoveJobExecutorContext();
            }
        }
         
        /// <summary>
        ///     Note: this is a hook to be overridden by
        ///     org.camunda.bpm.container.impl.threading.ra.inflow.JcaInflowExecuteJobsRunnable.executeJob(String,
        ///     ICommandExecutor)
        /// </summary>
        protected internal virtual void ExecuteJob(string nextJobId, ICommandExecutor commandExecutor)
        {
            ExecuteJobHelper.ExecuteJob(nextJobId, commandExecutor);
        }

        protected internal virtual void UnlockJob(string nextJobId, ICommandExecutor commandExecutor)
        {
            commandExecutor.Execute(new UnlockJobCmd(nextJobId));
        }
    }
}