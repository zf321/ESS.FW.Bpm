using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
{
    /// <summary>
    ///     Batch of work for history cleanup.
    /// </summary>
    public class HistoryCleanupBatch
    {
        /// <summary>
        ///     Maximum allowed batch size.
        /// </summary>
        public const int MaxBatchSize = 500;

        private IList<string> _historicCaseInstanceIds = new List<string>();
        private IList<string> _historicDecisionInstanceIds = new List<string>();

        private IList<string> _historicProcessInstanceIds = new List<string>();

        public virtual IList<string> HistoricProcessInstanceIds
        {
            get { return _historicProcessInstanceIds; }
            set { _historicProcessInstanceIds = value; }
        }


        public virtual IList<string> HistoricDecisionInstanceIds
        {
            get { return _historicDecisionInstanceIds; }
            set { _historicDecisionInstanceIds = value; }
        }


        public virtual IList<string> HistoricCaseInstanceIds
        {
            get { return _historicCaseInstanceIds; }
            set { _historicCaseInstanceIds = value; }
        }


        /// <summary>
        ///     Size of the batch.
        /// </summary>
        public virtual int Size()
        {
            return _historicProcessInstanceIds.Count + _historicDecisionInstanceIds.Count + _historicCaseInstanceIds.Count;
        }

        public virtual void PerformCleanup()
        {
            var commandContext = Context.CommandContext;
            //if (historicProcessInstanceIds.Count > 0)
            //{
            //  commandContext.HistoricProcessInstanceManager.DeleteHistoricProcessInstanceByIds(historicProcessInstanceIds);
            //  recordValue(Metrics.HISTORY_CLEANUP_REMOVED_PROCESS_INSTANCES, historicProcessInstanceIds.Count);
            //}
            //if (historicDecisionInstanceIds.Count > 0)
            //{
            //  commandContext.HistoricDecisionInstanceManager.deleteHistoricDecisionInstanceByIds(historicDecisionInstanceIds);
            //  recordValue(Metrics.HISTORY_CLEANUP_REMOVED_DECISION_INSTANCES, historicDecisionInstanceIds.Count);
            //}
            //if (historicCaseInstanceIds.Count > 0)
            //{
            //  commandContext.HistoricCaseInstanceManager.deleteHistoricCaseInstancesByIds(historicCaseInstanceIds);
            //  recordValue(Metrics.HISTORY_CLEANUP_REMOVED_CASE_INSTANCES, historicCaseInstanceIds.Count);
            //}
        }

        protected internal virtual void RecordValue(string name, long value)
        {
            var processEngineConfiguration = Context.CommandContext.ProcessEngineConfiguration;
            if (processEngineConfiguration.HistoryCleanupMetricsEnabled)
            {
                //processEngineConfiguration.DbMetricsReporter.ReportValueAtOnce(name, value);
            }
        }
    }
}