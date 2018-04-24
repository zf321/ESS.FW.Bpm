using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
	/// 
	/// 
	/// </summary>
    [Component]
    public class MeterLogManager : AbstractManagerNet<MeterLogEntity>, IMeterLogManager
    {

        public const string SelectMeterInterval = "selectMeterLogAggregatedByTimeInterval";
        public const string SelectMeterSum = "selectMeterLogSum";
        public const string DeleteAllMeter = "deleteAllMeterLogEntries";
        public const string DeleteAllMeterByTimestampAndReporter = "deleteMeterLogEntriesByTimestampAndReporter";

        public MeterLogManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void Insert(MeterLogEntity meterLogEntity)
        {
            Insert(meterLogEntity);
        }

        public virtual long? ExecuteSelectSum(MetricsQueryImpl query)
        {
            throw new NotImplementedException();
            //long? result = (long?)DbEntityManager.SelectOne(SelectMeterSum, query);
            //result = result != null ? result : 0;

            //if (ShouldAddCurrentUnloggedCount(query))
            //{
            //    // add current unlogged count
            //    Meter meter = context.Impl.Context.ProcessEngineConfiguration.MetricsRegistry.GetMeterByName(query.Name);
            //    if (meter != null)
            //    {
            //        result += meter.get();
            //    }
            //}

            //return result;
        }

        //public virtual IList<IMetricIntervalValue> ExecuteSelectInterval(MetricsQueryImpl query)
        //{
        //    throw new NotImplementedException();
        //    //IList<IMetricIntervalValue> intervalResult = ListExt.ConvertToListT<IMetricIntervalValue>(DbEntityManager.SelectList(SelectMeterInterval, query));
        //    //intervalResult = intervalResult != null ? intervalResult : new List<IMetricIntervalValue>();

        //    //string reporterId = context.Impl.Context.ProcessEngineConfiguration.DbMetricsReporter.MetricsCollectionTask.Reporter;
        //    //if (intervalResult.Count > 0 && IsEndTimeAfterLastReportInterval(query) && reporterId != null)
        //    //{

        //    //    IDictionary<string, Meter> metrics = context.Impl.Context.ProcessEngineConfiguration.MetricsRegistry.Meters;
        //    //    string queryName = query.Name;
        //    //    //we have to add all unlogged metrics to last interval
        //    //    if (queryName != null)
        //    //    {
        //    //        MetricIntervalEntity intervalEntity = (MetricIntervalEntity)intervalResult[0];
        //    //        intervalEntity.Value = intervalEntity.Value + metrics[queryName].get();
        //    //    }
        //    //    else
        //    //    {
        //    //        //IDictionary<string, Meter>.KeyCollection metricNames = metrics.Keys;
        //    //        var metricNames = metrics.Keys;
        //    //        DateTime lastIntervalTimestamp = intervalResult[0].Timestamp;
        //    //        foreach (string metricName in metricNames)
        //    //        {
        //    //            MetricIntervalEntity entity = new MetricIntervalEntity(lastIntervalTimestamp, metricName, reporterId);
        //    //            int idx = intervalResult.IndexOf(entity);
        //    //            if (idx >= 0)
        //    //            {
        //    //                MetricIntervalEntity intervalValue = (MetricIntervalEntity)intervalResult[idx];
        //    //                intervalValue.Value = intervalValue.Value + metrics[metricName].get();
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //return intervalResult;
        //}

        //protected internal virtual bool IsEndTimeAfterLastReportInterval(MetricsQueryImpl query)
        //{
        //    long reportingIntervalInSeconds = context.Impl.Context.ProcessEngineConfiguration.DbMetricsReporter.ReportingIntervalInSeconds;
        //    throw new NotImplementedException();
        //    //return (query.EndDate == null || query.EndDateMilliseconds >= ClockUtil.CurrentTime.Time - (1000 * reportingIntervalInSeconds));
        //}

        //protected internal virtual bool ShouldAddCurrentUnloggedCount(MetricsQueryImpl query)
        //{
        //    throw new NotImplementedException();
        //    //return query.Name != null && IsEndTimeAfterLastReportInterval(query);

        //}

        public virtual void DeleteAll()
        {
            //DbEntityManager.Delete(typeof(MeterLogEntity), DeleteAllMeter, null);
            DeleteAll();
        }

        public virtual void DeleteByTimestampAndReporter(DateTime? timestamp, string reporter)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            if (timestamp != null)
            {
                parameters["milliseconds"] = ((DateTime)timestamp).Ticks;
            }
            parameters["reporter"] = reporter;
            //DbEntityManager.Delete(typeof(MeterLogEntity), DeleteAllMeterByTimestampAndReporter, parameters);
            Delete(m => m.Reporter == reporter && m.Timestamp < timestamp);
        }

    }

}