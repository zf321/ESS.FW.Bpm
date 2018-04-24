using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.Metrics
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsRegistry
    {
        protected internal IDictionary<string, Meter> meters = new Dictionary<string, Meter>();

        public virtual IDictionary<string, Meter> Meters
        {
            get { return meters; }
        }

        public virtual Meter GetMeterByName(string name)
        {
            return meters[name];
        }

        public virtual void MarkOccurrence(string name)
        {
            MarkOccurrence(name, 1);
        }

        public virtual void MarkOccurrence(string name, long times)
        {
            var meter = meters[name];

            if (meter != null)
            {
                meter.markTimes(times);
            }
        }

        public virtual Meter CreateMeter(string name)
        {
            var meter = new Meter(name);
            meters[name] = meter;
            return meter;
        }
    }
}