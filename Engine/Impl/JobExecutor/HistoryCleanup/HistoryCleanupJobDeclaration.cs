//using System;
//using ESS.FW.Bpm.Engine.Impl.Cmd;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ESS.FW.Bpm.Engine.Persistence.Entity;

//namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
//{

//    /// <summary>
//	/// Job declaration for history cleanup.
//	/// 
//	/// </summary>
//	public class HistoryCleanupJobDeclaration : IJobDeclaration<HistoryCleanupContext, EverLivingJobEntity>
//    {

//        private static readonly CommandLogger LOG = ProcessEngineLogger.CmdLogger;

//        public HistoryCleanupJobDeclaration() : base(HistoryCleanupJobHandler.TYPE)
//        {
//        }

//        protected internal override ExecutionEntity resolveExecution(HistoryCleanupContext context)
//        {
//            return null;
//        }

//        protected internal override EverLivingJobEntity newJobInstance(HistoryCleanupContext context)
//        {
//            return new EverLivingJobEntity();
//        }

//        protected internal override void postInitialize(HistoryCleanupContext context, EverLivingJobEntity job)
//        {
//        }


//        public override EverLivingJobEntity reconfigure(HistoryCleanupContext context, EverLivingJobEntity job)
//        {
//            HistoryCleanupJobHandlerConfiguration configuration = resolveJobHandlerConfiguration(context);
//            job.JobHandlerConfiguration = configuration;
//            return job;
//        }

//        protected internal override HistoryCleanupJobHandlerConfiguration resolveJobHandlerConfiguration(HistoryCleanupContext context)
//        {
//            HistoryCleanupJobHandlerConfiguration config = new HistoryCleanupJobHandlerConfiguration();
//            config.ImmediatelyDue = context.ImmediatelyDue;
//            return config;
//        }

//        public override DateTime resolveDueDate(HistoryCleanupContext context)
//        {
//            return resolveDueDate(context.ImmediatelyDue);
//        }

//        private DateTime resolveDueDate(bool isImmediatelyDue)
//        {
//            CommandContext commandContext = Context.CommandContext;
//            if (isImmediatelyDue)
//            {
//                return ClockUtil.CurrentTime;
//            }
//            else
//            {
//                return HistoryCleanupHelper.getNextRunWithinBatchWindow(ClockUtil.CurrentTime, commandContext);
//            }
//        }
//    }

//}