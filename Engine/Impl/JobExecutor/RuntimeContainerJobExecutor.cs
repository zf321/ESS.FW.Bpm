using System;
using System.Collections.Generic;
using System.Threading;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    /// <summary>
    ///     <para>
    ///         JobExecutor implementation that delegates the execution of jobs
    ///         to the <seealso cref="RuntimeContainerDelegate RuntimeContainer" />
    ///     </para>
    ///     
    /// </summary>
    public class RuntimeContainerJobExecutor : JobExecutor
    {
        //protected internal virtual RuntimeContainerDelegate RuntimeContainerDelegate
        //{
        //    get { return RuntimeContainerDelegate_Fields.INSTANCE.get(); }
        //}

//        protected internal override void StartExecutingJobs()
//        {
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
//            var runtimeContainerDelegate = RuntimeContainerDelegate;

//            // schedule job acquisition
//            //if (!runtimeContainerDelegate.ExecutorService.Schedule(acquireJobsRunnable, true))
//            //{
//            //    throw new ProcessEngineException("Could not schedule AcquireJobsRunnable for execution.");
//            //}
//        }

        protected internal override void StopExecutingJobs()
        {
            // nothing to do
        }

        public override void ExecuteJobs(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            throw new NotImplementedException();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
//            var runtimeContainerDelegate = RuntimeContainerDelegate;
////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
////ORIGINAL LINE: final org.camunda.bpm.container.ExecutorService executorService = runtimeContainerDelegate.getExecutorService();
//            var executorService = runtimeContainerDelegate.ExecutorService;

//            var executeJobsRunnable = GetExecuteJobsRunnable(jobIds, processEngine);

//            // delegate job execution to runtime container
//            if (!executorService.Schedule(executeJobsRunnable, false))
//            {
//                LogRejectedExecution(processEngine, jobIds.Count);
//                rejectedJobsHandler.JobsRejected(jobIds, processEngine, this);
//            }
        }

        public override ExecuteJobsRunnable GetExecuteJobsRunnable(IList<string> jobIds, ProcessEngineImpl processEngine)
        {
            throw new NotImplementedException();
            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            ////ORIGINAL LINE: final org.camunda.bpm.container.RuntimeContainerDelegate runtimeContainerDelegate = getRuntimeContainerDelegate();
            //var runtimeContainerDelegate = runtimeContainerDelegate;
            ////JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            ////ORIGINAL LINE: final org.camunda.bpm.container.ExecutorService executorService = runtimeContainerDelegate.getExecutorService();
            //var executorService = runtimeContainerDelegate.ExecutorService;
            //return executorService.getExecuteJobsRunnable(jobIds, processEngine);
        }

        protected internal override void StartExecutingJobs()
        {
            throw new NotImplementedException();
        }
    }
}