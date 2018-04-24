using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Metrics.Reporter
{
    /// <summary>
    ///     
    /// </summary>
    public class DbMetricsReporter
    {
        protected internal string reporterId;

        // log every 15 minutes...
        protected internal long reportingIntervalInSeconds = 60*15;
        private Timer _timer;

        public DbMetricsReporter(MetricsRegistry metricsRegistry, ICommandExecutor commandExecutor)
        {
            MetricsRegistry = metricsRegistry;
            CommandExecutor = commandExecutor;
            InitMetricsCollectionTask();
        }

        public virtual long ReportingIntervalInSeconds
        {
            get { return reportingIntervalInSeconds; }
            set { reportingIntervalInSeconds = value; }
        }


        public virtual MetricsRegistry MetricsRegistry { get; }

        public virtual ICommandExecutor CommandExecutor { get; }

        public virtual MetricsCollectionTask MetricsCollectionTask { get; set; }


        public virtual string ReporterId
        {
            set
            {
                reporterId = value;
                if (MetricsCollectionTask != null)
                    MetricsCollectionTask.Reporter = value;
            }
        }

        protected internal virtual void InitMetricsCollectionTask()
        {
            MetricsCollectionTask = new MetricsCollectionTask(MetricsRegistry, CommandExecutor);
        }

        public virtual void Start()
        {
            //timer = new Timer("Camunda Metrics Reporter", true);
            var reportingIntervalInMillis = reportingIntervalInSeconds*1000;

            //timer.scheduleAtFixedRate(metricsCollectionTask, reportingIntervalInMillis, reportingIntervalInMillis);
        }

        public virtual void Stop()
        {
            if (_timer != null)
            {
                // cancel the timer
                //timer.cancel();
                _timer = null;
                // collect and log manually for the last time
                ReportNow();
            }
        }

        public virtual void ReportNow()
        {
            if (MetricsCollectionTask != null)
                MetricsCollectionTask.Run();
        }
    }
}