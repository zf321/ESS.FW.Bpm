using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
{
    /// <summary>
    /// </summary>
    public abstract class HistoryCleanupHelper
    {
        private static readonly string TimeFormatWithoutSeconds = "HH:mm";

        public static readonly string TimeFormatWithoutSecondsWithTimezone = "HH:mmZ";

        public static DateTime GetNextRunWithinBatchWindow(DateTime date, CommandContext commandContext)
        {
            return GetNextRunWithinBatchWindow(date, GetBatchWindowStartTime(commandContext));
        }

        public static DateTime GetNextRunWithinBatchWindow(DateTime date, DateTime batchWindowStartTime)
        {
            if (batchWindowStartTime != null)
            {
                var todayPossibleRun = UpdateTime(date, batchWindowStartTime);
                if (todayPossibleRun > date)
                    return todayPossibleRun;
                return AddDays(todayPossibleRun, 1);
            }
            throw new ProcessEngineException("Batch window must be configured");
        }

        public static DateTime GetBatchWindowStartTime(CommandContext commandContext)
        {
            return commandContext.ProcessEngineConfiguration.HistoryCleanupBatchWindowStartTimeAsDate;
        }

        public static DateTime GetBatchWindowEndTime(CommandContext commandContext)
        {
            return commandContext.ProcessEngineConfiguration.HistoryCleanupBatchWindowEndTimeAsDate;
        }

        public static bool IsBatchWindowConfigured(CommandContext commandContext)
        {
            return GetBatchWindowStartTime(commandContext) != null;
        }

        /// <summary>
        ///     Checks if given date is within a batch window. Batch window start time is checked inclusively.
        /// </summary>
        /// <param name="date">
        ///     @return
        /// </param>
        public static bool IsWithinBatchWindow(DateTime date, CommandContext commandContext)
        {
            if (IsBatchWindowConfigured(commandContext))
            {
                var todaysBatchWindowStartTime = UpdateTime(date, GetBatchWindowStartTime(commandContext));
                var todaysBatchWindowEndTime = UpdateTime(date, GetBatchWindowEndTime(commandContext));
                if (todaysBatchWindowEndTime > todaysBatchWindowStartTime)
                    return (date > todaysBatchWindowStartTime || date.Equals(todaysBatchWindowStartTime)) &&
                           date < todaysBatchWindowEndTime;
                return date > todaysBatchWindowStartTime || date.Equals(todaysBatchWindowStartTime) ||
                       date < todaysBatchWindowEndTime;
            }
            return false;
        }

        private static DateTime UpdateTime(DateTime now, DateTime newTime)
        {
            var c = new DateTime();
            c = new DateTime(now.Ticks);
            var newTimeCalendar = new DateTime();
            newTimeCalendar = new DateTime(newTime.Ticks);
            //c.set(DateTime.HOUR_OF_DAY, newTimeCalendar.Hour);
            //c.set(DateTime.MINUTE, newTimeCalendar.Minute);
            //c.set(DateTime.SECOND, newTimeCalendar.Second);
            //c.set(DateTime.MILLISECOND, newTimeCalendar.Millisecond);
            return c;
        }

        private static DateTime AddDays(DateTime date, int amount)
        {
            var c = new DateTime();
            c = new DateTime(date.Ticks);
            c.AddDays(amount);
            return c;
        }

        public static DateTime ParseTimeConfiguration(string time)
        {
            try
            {
                return DateTime.Parse(time); //TIME_FORMAT_WITHOUT_SECONDS_WITH_TIMEZONE
            }
            catch (ParseException)
            {
                return DateTime.Parse(time); //TIME_FORMAT_WITHOUT_SECONDS.parse(time);
            }
        }

        private static int? GetHistoryCleanupBatchSize(CommandContext commandContext)
        {
            return commandContext.ProcessEngineConfiguration.HistoryCleanupBatchSize;
        }

        /// <summary>
        ///     Creates next batch object for history cleanup. First searches for historic process instances ready for cleanup. If
        ///     there is still some place left in batch
        ///     (configured batch size was not reached), searches for historic decision instances and also adds them to the batch.
        /// </summary>
        /// <param name="commandContext">
        ///     @return
        /// </param>
        public static HistoryCleanupBatch GetNextBatch(CommandContext commandContext)
        {
            var batchSize = GetHistoryCleanupBatchSize(commandContext);
            var historyCleanupBatch = new HistoryCleanupBatch();

            //add process instance ids
            IList<string> historicProcessInstanceIds =
                commandContext.HistoricProcessInstanceManager.FindHistoricProcessInstanceIdsForCleanup(batchSize.Value);
            if (historicProcessInstanceIds.Count > 0)
                historyCleanupBatch.HistoricProcessInstanceIds = historicProcessInstanceIds;

            //if batch is not full, add decision instance ids
            if (historyCleanupBatch.Size() < batchSize && commandContext.ProcessEngineConfiguration.DmnEnabled)
            {
                //IList<string> historicDecisionInstanceIds = commandContext.HistoricDecisionInstanceManager.findHistoricDecisionInstanceIdsForCleanup(batchSize - historyCleanupBatch.size());
                // if (historicDecisionInstanceIds.Count > 0)
                // {
                //historyCleanupBatch.HistoricDecisionInstanceIds = historicDecisionInstanceIds;
                // }
            }

            //if batch is not full, add case instance ids
            if (historyCleanupBatch.Size() < batchSize && commandContext.ProcessEngineConfiguration.CmmnEnabled)
            {
                //IList<string> historicCaseInstanceIds = commandContext.HistoricCaseInstanceManager.findHistoricCaseInstanceIdsForCleanup(batchSize - historyCleanupBatch.size());
                // if (historicCaseInstanceIds.Count > 0)
                // {
                //historyCleanupBatch.HistoricCaseInstanceIds = historicCaseInstanceIds;
                // }
            }

            return historyCleanupBatch;
        }
    }
}