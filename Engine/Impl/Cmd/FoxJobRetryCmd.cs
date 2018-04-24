using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class FoxJobRetryCmd : JobRetryCmd
    {
        public static readonly IList<string> SupportedTypes = new List<string>
        {
            TimerExecuteNestedActivityJobHandler.TYPE,
            TimerCatchIntermediateEventJobHandler.TYPE,
            TimerStartEventJobHandler.TYPE,
            TimerStartEventSubprocessJobHandler.TYPE,
            AsyncContinuationJobHandler.TYPE
        };

        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        public FoxJobRetryCmd(string jobId, System.Exception exception) : base(jobId, exception)
        {
        }


        public override object Execute(CommandContext commandContext)
        {
            var job = Job;

            var activity = GetCurrentActivity(commandContext, job);

            if (activity == null)
            {
                Log.DebugFallbackToDefaultRetryStrategy();
                ExecuteStandardStrategy(commandContext);
            }
            else
            {
                try
                {
                    ExecuteCustomStrategy(commandContext, job, activity);
                }
                catch (System.Exception)
                {
                    Log.DebugFallbackToDefaultRetryStrategy();
                    ExecuteStandardStrategy(commandContext);
                }
            }

            return null;
        }

        protected internal virtual void ExecuteStandardStrategy(CommandContext commandContext)
        {
            var decrementCmd = new DecrementJobRetriesCmd(JobId, Exception);
            decrementCmd.Execute(commandContext);
        }
        protected internal virtual void ExecuteCustomStrategy(CommandContext commandContext, JobEntity job,
            ActivityImpl activity)
        {
            var failedJobRetryTimeCycle = GetFailedJobRetryTimeCycle(activity);

            if (ReferenceEquals(failedJobRetryTimeCycle, null))
            {
                ExecuteStandardStrategy(commandContext);
            }
            else
            {
                var durationHelper = GetDurationHelper(failedJobRetryTimeCycle);

                SetLockExpirationTime(job, failedJobRetryTimeCycle, durationHelper);

                if (IsFirstJobExecution(job))
                {
                    // then change default retries to the ones configured
                    InitializeRetries(job, failedJobRetryTimeCycle, durationHelper);
                }
                else
                {
                    Log.DebugDecrementingRetriesForJob(job.Id);
                }

                LogException(job);
                DecrementRetries(job);
                NotifyAcquisition(commandContext);
            }
        }

        protected internal virtual ActivityImpl GetCurrentActivity(CommandContext commandContext, JobEntity job)
        {
            var type = job.JobHandlerType;
            ActivityImpl activity = null;

            if (SupportedTypes.Contains(type))// SUPPORTED_TYPES.Contains(type))
            {
                DeploymentCache deploymentCache = Context.ProcessEngineConfiguration.DeploymentCache;
                ProcessDefinitionEntity processDefinitionEntity =
                    deploymentCache.FindDeployedProcessDefinitionById(job.ProcessDefinitionId);
                activity = (ActivityImpl) processDefinitionEntity.FindActivity(job.ActivityId);
            }

            return activity;
        }

        protected internal virtual ExecutionEntity FetchExecutionEntity(string executionId)
        {
            return context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(executionId);
        }

        protected internal virtual string GetFailedJobRetryTimeCycle(ActivityImpl activity)
        {
            return activity.Properties.Get(FoxFailedJobParseListener.FoxFailedJobConfiguration);
        }
        
        protected internal virtual DurationHelper GetDurationHelper(string failedJobRetryTimeCycle)
        {
            return new DurationHelper(failedJobRetryTimeCycle);
        }

        protected internal virtual void SetLockExpirationTime(JobEntity job, string failedJobRetryTimeCycle,
            DurationHelper durationHelper)
        {
            job.LockExpirationTime = durationHelper.DateAfter;
        }

        protected internal virtual bool IsFirstJobExecution(JobEntity job)
        {
            // check if this is jobs' first execution (recognize
            // this because no exception is set. Only the first
            // execution can be without exception - because if
            // no exception occurred the job would have been completed)
            // see https://app.camunda.com/jira/browse/CAM-1039
            return ReferenceEquals(job.ExceptionByteArrayId, null) && ReferenceEquals(job.ExceptionMessage, null);
        }

        protected internal virtual void InitializeRetries(JobEntity job, string failedJobRetryTimeCycle,
            DurationHelper durationHelper)
        {
            Log.DebugInitiallyAppyingRetryCycleForJob(job.Id, durationHelper.Times);
            job.Retries = durationHelper.Times;

        }
    }
}