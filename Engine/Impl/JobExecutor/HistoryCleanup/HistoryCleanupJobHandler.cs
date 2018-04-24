using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using Newtonsoft.Json.Linq;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
{

    /// <summary>
    /// Job handler for history cleanup job.
    /// 
    /// </summary>
    public class HistoryCleanupJobHandler : IJobHandler<HistoryCleanupJobHandlerConfiguration>
    {

        public const string TYPE = "history-cleanup";

        public  string Type
        {
            get
            {
                return TYPE;
            }
        }

      public  void Execute( IJobHandlerConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
      {
          var conf = (HistoryCleanupJobHandlerConfiguration) configuration;
            //find JobEntity
            JobEntity jobEntity = commandContext.JobManager.FindJobByHandlerType(Type);

            bool rescheduled = false;

            if (conf.ImmediatelyDue || (HistoryCleanupHelper.IsBatchWindowConfigured(commandContext) && HistoryCleanupHelper.IsWithinBatchWindow(ClockUtil.CurrentTime, commandContext)))
            {
                //find data to delete
                HistoryCleanupBatch nextBatch = HistoryCleanupHelper.GetNextBatch(commandContext);
                if (nextBatch.Size() >= GetBatchSizeThreshold(commandContext))
                {

                    //delete bunch of data
                    nextBatch.PerformCleanup();

                    //ReSchedule now
                    commandContext.JobManager.ReSchedule(jobEntity, ClockUtil.CurrentTime);
                    rescheduled = true;
                    CancelCountEmptyRuns(conf, jobEntity);
                }
                else
                {
                    //still have something to delete
                    if (nextBatch.Size() > 0)
                    {
                        nextBatch.PerformCleanup();
                    }
                    //not enough data for cleanup was found
                    if (HistoryCleanupHelper.IsWithinBatchWindow(ClockUtil.CurrentTime, commandContext))
                    {
                        //ReSchedule after some delay
                        DateTime nextRunDate = conf.GetNextRunWithDelay(ClockUtil.CurrentTime);
                        if (HistoryCleanupHelper.IsWithinBatchWindow(nextRunDate, commandContext))
                        {
                            commandContext.JobManager.ReSchedule(jobEntity, nextRunDate);
                            rescheduled = true;
                            IncrementCountEmptyRuns(conf, jobEntity);
                        }
                    }
                }
            }
            if (!rescheduled)
            {
                if (HistoryCleanupHelper.IsBatchWindowConfigured(commandContext))
                {
                    ReScheduleRegularCall(commandContext, jobEntity);
                }
                else
                {
                    //nothing more to do, suspend the job
                    SuspendJob(jobEntity);
                }
                CancelCountEmptyRuns(conf, jobEntity);
            }
        }

        private void ReScheduleRegularCall(CommandContext commandContext, JobEntity jobEntity)
        {
            commandContext.JobManager.ReSchedule(jobEntity, HistoryCleanupHelper.GetNextRunWithinBatchWindow(ClockUtil.CurrentTime, commandContext));
        }

        private void SuspendJob(JobEntity jobEntity)
        {
            jobEntity.SuspensionState = SuspensionStateFields.Suspended.StateCode;
        }

        private void IncrementCountEmptyRuns(HistoryCleanupJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            configuration.CountEmptyRuns = configuration.CountEmptyRuns + 1;
            jobEntity.JobHandlerConfiguration = configuration;
        }

        private void CancelCountEmptyRuns(HistoryCleanupJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
            configuration.CountEmptyRuns = 0;
            jobEntity.JobHandlerConfiguration = configuration;
        }

        public  IJobHandlerConfiguration NewConfiguration(string canonicalString)
        {
            JObject jsonObject = new JObject(canonicalString);
            return HistoryCleanupJobHandlerConfiguration.FromJson(jsonObject);
        }

        public void OnDelete(IJobHandlerConfiguration configuration, JobEntity jobEntity)
        {
        }

        public virtual int? GetBatchSizeThreshold(CommandContext commandContext)
        {
            return commandContext.ProcessEngineConfiguration.HistoryCleanupBatchThreshold;
        }

    }

}