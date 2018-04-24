//using System;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.JobExecutor;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.context.Impl;
//using ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup;

//namespace ESS.FW.Bpm.Engine.Impl.Cmd
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [Serializable]
//    public class HistoryCleanupCmd : ICommand<IJob>
//    {

//        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

//        public static readonly IJobDeclaration HISTORY_CLEANUP_JOB_DECLARATION = new HistoryCleanupJobDeclaration();

//        private bool immediatelyDue;

//        public HistoryCleanupCmd(bool immediatelyDue)
//        {
//            this.immediatelyDue = immediatelyDue;
//        }

//        public  IJob Execute(CommandContext commandContext)
//        {
//            commandContext.AuthorizationManager.CheckAuthorization(Permissions.DeleteHistory, Resources.ProcessDefinition);

//            //validate
//            if (!willBeScheduled(commandContext))
//            {
//                Log.DebugHistoryCleanupWrongConfiguration();
//            }

//            //find job instance
//            JobEntity historyCleanupJob = commandContext.JobManager.FindJobByHandlerType(HistoryCleanupJobHandler.TYPE);

//            bool createJob = historyCleanupJob == null && willBeScheduled(commandContext);

//            bool reconfigureJob = historyCleanupJob != null && willBeScheduled(commandContext);

//            bool suspendJob = historyCleanupJob != null && !willBeScheduled(commandContext);

//            if (createJob)
//            {
//                //exclusive lock
//                commandContext.PropertyManager.acquireExclusiveLockForHistoryCleanupJob();

//                //check again after lock
//                historyCleanupJob = commandContext.JobManager.findJobByHandlerType(HistoryCleanupJobHandler.TYPE);

//                if (historyCleanupJob == null)
//                {
//                    historyCleanupJob = HISTORY_CLEANUP_JOB_DECLARATION.CreateJobInstance(new HistoryCleanupContext(immediatelyDue));
//                    Context.CommandContext.JobManager.insertAndHintJobExecutor(historyCleanupJob);
//                }
//            }
//            else if (reconfigureJob)
//            {
//                //apply new configuration
//                HistoryCleanupContext historyCleanupContext = new HistoryCleanupContext(immediatelyDue);
//                HISTORY_CLEANUP_JOB_DECLARATION.Reconfigure(historyCleanupContext, historyCleanupJob);
//                DateTime newDueDate = HISTORY_CLEANUP_JOB_DECLARATION.resolveDueDate(historyCleanupContext);
//                commandContext.JobManager.reschedule(historyCleanupJob, newDueDate);
//            }
//            else if (suspendJob)
//            {
//                historyCleanupJob.Duedate = null;
//                historyCleanupJob.SuspensionState = SuspensionState.SUSPENDED.StateCode;
//            }

//            return historyCleanupJob;
//        }

//        private bool willBeScheduled(CommandContext commandContext)
//        {
//            return immediatelyDue || HistoryCleanupHelper.isBatchWindowConfigured(commandContext);
//        }

//    }
//}
