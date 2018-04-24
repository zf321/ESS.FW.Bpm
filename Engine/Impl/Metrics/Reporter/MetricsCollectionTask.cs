using System.Collections.Generic;
using System.Timers;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Reporter
{
    /// <summary>
    ///     
    /// </summary>
    public class MetricsCollectionTask : Timer
    {
        private static readonly MetricsLogger Log = ProcessEngineLogger.MetricsLogger;
        protected internal ICommandExecutor CommandExecutor;

        protected internal MetricsRegistry MetricsRegistry;

        public MetricsCollectionTask(MetricsRegistry metricsRegistry, ICommandExecutor commandExecutor)
        {
            this.MetricsRegistry = metricsRegistry;
            this.CommandExecutor = commandExecutor;
        }

        public virtual string Reporter { get; set; }

        public virtual void Run()
        {
            try
            {
                CollectMetrics();
            }
            catch (System.Exception e)
            {
                try
                {
                    Log.CouldNotCollectAndLogMetrics(e);
                }
                catch (System.Exception)
                {
                    // ignore if log can't be written
                }
            }
        }

        protected internal virtual void CollectMetrics()
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.Util.List<org.camunda.bpm.engine.impl.persistence.entity.MeterLogEntity> logs = new java.Util.ArrayList<org.camunda.bpm.engine.impl.persistence.entity.MeterLogEntity>();
            IList<MeterLogEntity> logs = new List<MeterLogEntity>();
            foreach (var meter in MetricsRegistry.Meters.Values)
            {
                //logs.Add(new MeterLogEntity(meter.Name, reporterId, meter.AndClear, ClockUtil.CurrentTime));
            }

            CommandExecutor.Execute(new CommandAnonymousInnerClass(this, logs));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly IList<MeterLogEntity> _logs;
            private readonly MetricsCollectionTask _outerInstance;

            public CommandAnonymousInnerClass(MetricsCollectionTask outerInstance, IList<MeterLogEntity> logs)
            {
                this._outerInstance = outerInstance;
                this._logs = logs;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                foreach (var meterLogEntity in _logs)
                {
                    //commandContext.MeterLogManager.insert(meterLogEntity);
                }
                return null;
            }
        }
    }
}