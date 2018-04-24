using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl
{
    using CommandExecutor = ICommandExecutor;

    /// <summary>
    ///      
    /// </summary>
    public class ProcessEngineImpl : IProcessEngine
    {
        private static readonly ProcessEngineLogger Log = ProcessEngineLogger.Instance;
        protected internal CommandExecutor commandExecutor;
        protected internal CommandExecutor CommandExecutorSchemaOperations;

        protected internal string DatabaseSchemaUpdate;
        protected internal ExpressionManager ExpressionManager;
        protected internal IHistoryLevel HistoryLevel;
        protected internal JobExecutor.JobExecutor JobExecutor;

        protected internal ProcessEngineConfigurationImpl processEngineConfiguration;

        //protected internal IDictionary<Type, ISessionFactory> SessionFactories;
        protected internal ITransactionContextFactory TransactionContextFactory;

        public ProcessEngineImpl(ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            this.processEngineConfiguration = processEngineConfiguration;
            Name = processEngineConfiguration.ProcessEngineName;

            RepositoryService = processEngineConfiguration.RepositoryService;
            RuntimeService = processEngineConfiguration.RuntimeService;
            HistoryService = processEngineConfiguration.HistoryService;
            IdentityService = processEngineConfiguration.IdentityService;
            TaskService = processEngineConfiguration.TaskService;
            FormService = processEngineConfiguration.FormService;
            ManagementService = processEngineConfiguration.ManagementService;
            AuthorizationService = processEngineConfiguration.AuthorizationService;
            //this.caseService = processEngineConfiguration.CaseService;
            FilterService = processEngineConfiguration.FilterService;
            ExternalTaskService = processEngineConfiguration.ExternalTaskService;
            DecisionService = processEngineConfiguration.DecisionService;

            DatabaseSchemaUpdate = processEngineConfiguration.DatabaseSchemaUpdate;
            JobExecutor = processEngineConfiguration.JobExecutor;
            commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            CommandExecutorSchemaOperations = processEngineConfiguration.CommandExecutorSchemaOperations;
            //SessionFactories = processEngineConfiguration.SessionFactories;
            HistoryLevel = processEngineConfiguration.HistoryLevel;
            TransactionContextFactory = processEngineConfiguration.TransactionContextFactory;
            Log.LogDebug("使用的TransactionContextFactory:", TransactionContextFactory.GetType().Name.ToString());
            //TODO 数据库初始化入口
            //ExecuteSchemaOperations();

            if (ReferenceEquals(Name, null))
            { Name = ProcessEngines.NameDefault;
                Log.ProcessEngineCreated(ProcessEngines.NameDefault);
            }
            else
                Log.ProcessEngineCreated(Name);

            ProcessEngines.RegisterProcessEngine(this);

            if (JobExecutor != null)
                JobExecutor.RegisterProcessEngine(this);

            if (processEngineConfiguration.MetricsEnabled)
            {
                var reporterId = processEngineConfiguration.MetricsReporterIdProvider.ProvideId(this);
                var dbMetricsReporter = processEngineConfiguration.DbMetricsReporter;
                dbMetricsReporter.ReporterId = reporterId;

                if (processEngineConfiguration.DbMetricsReporterActivate)
                    dbMetricsReporter.Start();
            }
        }

        public virtual void Close()
        {
            ProcessEngines.Unregister(this);

            if (processEngineConfiguration.MetricsEnabled)
                processEngineConfiguration.DbMetricsReporter.Stop();

            if (JobExecutor != null)
                JobExecutor.UnregisterProcessEngine(this);

            CommandExecutorSchemaOperations.Execute(new SchemaOperationProcessEngineClose());

            //processEngineConfiguration.Close();

            Log.ProcessEngineClosed(Name);
        }

        public virtual string Name { get; }

        public virtual ProcessEngineConfiguration ProcessEngineConfiguration
        {
            get { return processEngineConfiguration; }
        }

        public virtual IIdentityService IdentityService { get; }

        public virtual IManagementService ManagementService { get; }

        public virtual ITaskService TaskService { get; }

        public virtual IHistoryService HistoryService { get; }

        public virtual IRuntimeService RuntimeService { get; }

        public virtual IRepositoryService RepositoryService { get; }

        public virtual IFormService FormService { get; }

        public virtual IAuthorizationService AuthorizationService { get; }

        public virtual ICaseService CaseService { get; }

        public virtual IFilterService FilterService { get; }

        public virtual IExternalTaskService ExternalTaskService { get; }

        public virtual IDecisionService DecisionService { get; }

        protected internal virtual void ExecuteSchemaOperations()
        {
            CommandExecutorSchemaOperations.Execute(processEngineConfiguration.SchemaOperationsCommand);
        }
    }
}