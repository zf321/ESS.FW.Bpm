using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Util
{

    public class ProcessEngineBootstrapRule /*: TestWatcher*/
    {

        private IProcessEngine _processEngine;


        public ProcessEngineBootstrapRule() : this("camunda.cfg.xml")
        {
        }

        public ProcessEngineBootstrapRule(string configurationResource)
        {
            this._processEngine = BootstrapEngine(configurationResource);
        }

        public virtual IProcessEngine BootstrapEngine(string configurationResource)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(configurationResource);
            ConfigureEngine(processEngineConfiguration);
            return processEngineConfiguration.BuildProcessEngine();

        }

        public virtual ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
        {
            return configuration;
        }

        public virtual IProcessEngine ProcessEngine => _processEngine;

        [TearDown]
        protected internal void Finished()
        {
            DeleteHistoryCleanupJob();
            _processEngine.Close();
            ProcessEngines.Unregister(_processEngine);
            _processEngine = null;
        }

        private void DeleteHistoryCleanupJob()
        {
            IJob job = _processEngine.HistoryService.FindHistoryCleanupJob();
            if (job != null)
            {
                ((ProcessEngineConfigurationImpl)_processEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this, job));
            }
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ProcessEngineBootstrapRule _outerInstance;

            private IJob _job;

            public CommandAnonymousInnerClass(ProcessEngineBootstrapRule outerInstance, IJob job)
            {
                this._outerInstance = outerInstance;
                this._job = job;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                commandContext.JobManager.DeleteJob((JobEntity)_job);
                commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(_job.Id);
                return null;
            }
        }

    }

}