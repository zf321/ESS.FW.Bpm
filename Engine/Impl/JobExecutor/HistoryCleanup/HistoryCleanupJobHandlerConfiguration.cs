using System;
using Newtonsoft.Json.Linq;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor.HistoryCleanup
{

    /// <summary>
    /// 
    /// </summary>
    public class HistoryCleanupJobHandlerConfiguration : IJobHandlerConfiguration
    {

        public const int StartDelay = 10; //10 seconds
        public const int MaxDelay = 60 * 60; //hour

        public const string JobConfigCountEmptyRuns = "countEmptyRuns";
        public const string JobConfigExecuteAtOnce = "immediatelyDue";

        public HistoryCleanupJobHandlerConfiguration()
        {
        }

        public  string ToCanonicalString()
        {
            JObject json = new JObject();
            //json.(JOB_CONFIG_COUNT_EMPTY_RUNS, countEmptyRuns);
            //json.Property(JOB_CONFIG_EXECUTE_AT_ONCE, immediatelyDue);
            return json.ToString();
        }

        public static HistoryCleanupJobHandlerConfiguration FromJson(JObject jsonObject)
        {
            HistoryCleanupJobHandlerConfiguration config = new HistoryCleanupJobHandlerConfiguration();
            //if (jsonObject.has(JOB_CONFIG_COUNT_EMPTY_RUNS))
            //{
            //    config.CountEmptyRuns = jsonObject.getInt(JOB_CONFIG_COUNT_EMPTY_RUNS);
            //}
            //if (jsonObject.has(JOB_CONFIG_EXECUTE_AT_ONCE))
            //{
            //    config.ImmediatelyDue = jsonObject.getBoolean(JOB_CONFIG_EXECUTE_AT_ONCE);
            //}
            return config;
        }

        /// <summary>
        /// The delay between two "empty" runs increases twice each time until it reaches <seealso cref="HistoryCleanupJobHandlerConfiguration#MAX_DELAY"/> value. </summary>
        /// <param name="date"> date to count delay from </param>
        /// <returns> date with delay </returns>
        public virtual DateTime GetNextRunWithDelay(DateTime date)
        {
            DateTime result = AddSeconds(date, Math.Min((int)(Math.Pow(2.0, (double)CountEmptyRuns) * StartDelay), MaxDelay));
            return result;
        }

        private DateTime AddSeconds(DateTime date, int amount)
        {
            DateTime c = new DateTime();
            c = new DateTime(date.Ticks);
            c.AddSeconds(amount);
            return c;
        }

        public virtual int CountEmptyRuns { get; set; } = 0;


        public virtual bool ImmediatelyDue { get; set; }
    }


}