using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Cfg.Standalone;
using ESS.FW.Bpm.Engine.Impl.Cfg.Tx;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    /// </summary>
    public class TxProcessEngineConfiguration : ProcessEngineConfigurationImpl
    {
        private new static readonly ConfigurationLogger Log = ProcessEngineLogger.ConfigLogger;
        /// <summary>
        ///     <seealso cref="CommandContextFactory" /> to be used for DbSchemaOperations
        /// </summary>
        protected internal CommandContextFactory dbSchemaOperationsCommandContextFactory;

        public TxProcessEngineConfiguration()
        {
            Log.LogDebug("启动TxProcessEngineConfiguration", "----------------------------初始化", "----------------------------");
            transactionsExternallyManaged = true;
        }

       protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired
        {
            get
            {
                IList<CommandInterceptor> defaultCommandInterceptorsTxRequired = new List<CommandInterceptor>();
                defaultCommandInterceptorsTxRequired.Add(new LogInterceptor());
                defaultCommandInterceptorsTxRequired.Add(new ProcessApplicationContextInterceptor(this));
                defaultCommandInterceptorsTxRequired.Add(new TransactionInterceptor(false));
                defaultCommandInterceptorsTxRequired.Add(new CommandContextInterceptor(commandContextFactory, this));
                return defaultCommandInterceptorsTxRequired;
            }
        }

        protected internal override ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew
        {
            get
            {
                IList<CommandInterceptor> defaultCommandInterceptorsTxRequiresNew = new List<CommandInterceptor>();
                defaultCommandInterceptorsTxRequiresNew.Add(new LogInterceptor());
                defaultCommandInterceptorsTxRequiresNew.Add(new ProcessApplicationContextInterceptor(this));
                defaultCommandInterceptorsTxRequiresNew.Add(new TransactionInterceptor(true));
                defaultCommandInterceptorsTxRequiresNew.Add(new CommandContextInterceptor(commandContextFactory, this,
                    true));
                return defaultCommandInterceptorsTxRequiresNew;
            }
        }

        public virtual CommandContextFactory DbSchemaOperationsCommandContextFactory
        {
            get { return dbSchemaOperationsCommandContextFactory; }
            set { dbSchemaOperationsCommandContextFactory = value; }
        }

        protected internal override void Init()
        {
            InitDbSchemaOperationsCommandContextFactory();
            base.Init();
        }

        /// <summary>
        ///     provide custom command executor that uses NON-JTA transactions
        /// </summary>
        protected internal override void InitCommandExecutorDbSchemaOperations()
        {
            if (commandExecutorSchemaOperations == null)
            {
                IList<CommandInterceptor> commandInterceptorsDbSchemaOperations = new List<CommandInterceptor>();
                commandInterceptorsDbSchemaOperations.Add(new LogInterceptor());
                commandInterceptorsDbSchemaOperations.Add(
                    new CommandContextInterceptor(dbSchemaOperationsCommandContextFactory, this));
                commandInterceptorsDbSchemaOperations.Add(ActualCommandExecutor);
                commandExecutorSchemaOperations = InitInterceptorChain(commandInterceptorsDbSchemaOperations);
            }
        }

        protected internal virtual void InitDbSchemaOperationsCommandContextFactory()
        {
            if (dbSchemaOperationsCommandContextFactory == null)
            {
                var cmdContextFactory = new TxContextCommandContextFactory();
                cmdContextFactory.ProcessEngineConfiguration = this;
                cmdContextFactory.TransactionContextFactory = new TxTransactionContextFactory();// StandaloneTransactionContextFactory();
                dbSchemaOperationsCommandContextFactory = cmdContextFactory;
            }
        }
        
        protected internal override void InitTransactionContextFactory()
        {
            if (transactionContextFactory == null)
            {
                transactionContextFactory = new TxTransactionContextFactory();
            }
        }
    }
}