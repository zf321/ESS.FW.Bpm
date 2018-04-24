using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management;

namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class MetricsQueryImpl : ICommand<object>, IMetricsQuery
    {
        public const int DefaultLimitSelectInterval = 200;
        public const long DefaultSelectInterval = 15 * 60;



        /// <summary>
        ///     Contains the command implementation which should be executed either
        ///     metric sum or select metric grouped by time interval.
        ///     Note: this enables to quit with the enum distinction
        /// </summary>
        protected internal ICommand<object> Callback;

        //[NonSerialized]
        //protected internal CommandExecutor commandExecutor;
        
        protected internal long? IntervalRenamed;
        

        public MetricsQueryImpl(ICommandExecutor commandExecutor)
        {
            //this.commandExecutor = commandExecutor;
            //maxResults = DefaultLimitSelectInterval;
            IntervalRenamed = DefaultSelectInterval;
        }

        //public  int MaxResults
        //{
        //    set
        //    {
        //        if (value > DefaultLimitSelectInterval)
        //            throw new ProcessEngineException("Metrics interval query row limit can't be set larger than " +
        //                                             DefaultLimitSelectInterval + '.');
        //        //maxResults = value;
        //    }
        //    get
        //    {
        //        //if (maxResults > DefaultLimitSelectInterval)
        //        //    return DefaultLimitSelectInterval;
        //        //return base.MaxResults;
        //    }
        //}

        public virtual DateTime _StartDate { get; protected internal set; }

        public virtual DateTime _EndDate { get; protected internal set; }

        public virtual long? StartDateMilliseconds { get; protected internal set; }

        public virtual long? EndDateMilliseconds { get; protected internal set; }

        public virtual string _Name { get; protected internal set; }

        public virtual string Reporter { get; protected internal set; }

        public virtual long? _Interval
        {
            get
            {
                if (IntervalRenamed == null)
                    return DefaultSelectInterval;
                return IntervalRenamed;
            }
        }

        public virtual object Execute(CommandContext commandContext)
        {
            if (Callback != null)
                return Callback.Execute(commandContext);
            throw new ProcessEngineException("Query can't be executed. Use either sum or interval to query the metrics.");
        }

        public virtual IMetricsQuery reporter(string reporter)
        {
            Reporter = reporter;
            return this;
        }

        public virtual IList<IMetricIntervalValue> Interval()
        {
            throw new NotImplementedException();
            //callback = new CommandAnonymousInnerClass(this);

            // return (IList<MetricIntervalValue>) commandExecutor.execute(this);
            return null;
        }


        public virtual IList<IMetricIntervalValue> Interval(long interval)
        {
            IntervalRenamed = interval;
            //return interval();
            return null;
        }


        public virtual long Sum()
        {
            //callback = new CommandAnonymousInnerClass2(this);

            //return (long?) commandExecutor.execute(this).Value;
            return 0;
        }

        IMetricsQuery IMetricsQuery.Name(string name)
        {
            throw new NotImplementedException();
        }

        IMetricsQuery IMetricsQuery.Reporter(string reporter)
        {
            throw new NotImplementedException();
        }

        IMetricsQuery IMetricsQuery.StartDate(DateTime startTime)
        {
            throw new NotImplementedException();
        }

        IMetricsQuery IMetricsQuery.EndDate(DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public virtual IMetricsQuery Offset(int offset)
        {
            //FirstResult = offset;
            return this;
        }

        public virtual IMetricsQuery Limit(int maxResults)
        {
            //MaxResults = maxResults;
            return this;
        }



        public virtual MetricsQueryImpl Name(string name)
        {
            this._Name = name;
            return this;
        }

        public virtual MetricsQueryImpl StartDate(DateTime startDate)
        {
            this._StartDate = startDate;
            StartDateMilliseconds = startDate.Ticks;
            return this;
        }

        public virtual MetricsQueryImpl EndDate(DateTime endDate)
        {
            _EndDate = endDate;
            EndDateMilliseconds = endDate.Ticks;
            return this;
        }

        //private class CommandAnonymousInnerClass : ICommand
        //{
        //    private readonly MetricsQueryImpl outerInstance;

        //    public CommandAnonymousInnerClass(MetricsQueryImpl outerInstance)
        //    {
        //        this.outerInstance = outerInstance;
        //    }

        //    public virtual object execute(CommandContext commandContext)
        //    {
        //        return commandContext.MeterLogManager.executeSelectInterval(outerInstance);
        //    }
        //}

        //private class CommandAnonymousInnerClass2 : ICommand
        //{
        //    private readonly MetricsQueryImpl outerInstance;

        //    public CommandAnonymousInnerClass2(MetricsQueryImpl outerInstance)
        //    {
        //        this.outerInstance = outerInstance;
        //    }

        //    public virtual object execute(CommandContext commandContext)
        //    {
        //        return commandContext.MeterLogManager.executeSelectSum(outerInstance);
        //    }

        //}
    }
}