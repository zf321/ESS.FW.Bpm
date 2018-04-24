using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.Impl.Configuration;
using ESS.FW.Bpm.Engine.Form.Impl.Engine;
using ESS.FW.Bpm.Engine.Form.Impl.Type;
using ESS.FW.Bpm.Engine.Form.Impl.Validator;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Calendar;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Impl.Cfg.Multitenancy;
using ESS.FW.Bpm.Engine.Impl.Cfg.Standalone;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Digest;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Incident;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.migration;
using ESS.FW.Bpm.Engine.Impl.migration.validation.activity;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instance;
using ESS.FW.Bpm.Engine.Impl.migration.validation.instruction;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Impl.Metrics.Reporter;
using ESS.FW.Bpm.Engine.Impl.runtime;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;
using ESS.FW.Bpm.Engine.Persistence.Deploy;
using ESS.FW.Bpm.Engine.Impl.Metrics.Parser;
using ESS.FW.Bpm.Engine.Dmn.Impl.Deployer;
using ESS.FW.Bpm.Engine.Batch.Impl;
using ESS.FW.Bpm.Engine.Batch.Impl.Deletion;
using ESS.FW.Bpm.Engine.Batch.Impl.Job;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Handler;
using ESS.FW.Bpm.Engine.History.Impl.Parser;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using Microsoft.Extensions.Logging;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Deployer;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Impl.migration.batch;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using ESS.FW.Bpm.Engine.Impl.Scripting.Env;
using ESS.FW.Bpm.Engine.Impl.Scripting.Engine;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{

    /// <summary>
    ///      
    /// </summary>
    public abstract class ProcessEngineConfigurationImpl : ProcessEngineConfiguration
    {
        /// <summary>
        ///     创建ProcessEngine
        /// </summary>
        /// <returns></returns>
        public override IProcessEngine BuildProcessEngine()
        {
            Init();
            processEngine = new ProcessEngineImpl(this);
            InvokePostProcessEngineBuild(processEngine);
            return processEngine;
        }

        #region 静态变量

        public const string DbSchemaUpdateCreate = "create";
        public const string DbSchemaUpdateDropCreate = "drop-create";

        public const string DefaultWsSyncFactory =
            "org.camunda.bpm.engine.impl.webservice.CxfWebServiceClientFactory";

        public const string DefaultMybatisMappingFile = "org/camunda/bpm/engine/impl/mapping/mappings.xml";

        //protected internal static Properties databaseTypeMappings = DefaultDatabaseTypeMappings;
        protected internal const string MySqlProductName = "MySQL";
        protected internal const string MariaDbProductName = "MariaDB";

        protected internal static readonly ConfigurationLogger Log = ProcessEngineLogger.ConfigLogger;

        public static readonly int HistorylevelNone = HistoryLevelFields.HistoryLevelNone.Id;
        public static readonly int HistorylevelActivity = HistoryLevelFields.HistoryLevelActivity.Id;
        public static readonly int HistorylevelAudit = HistoryLevelFields.HistoryLevelAudit.Id;
        public static readonly int HistorylevelFull = HistoryLevelFields.HistoryLevelFull.Id;

        public static readonly int DefaultFailedJobListenerMaxRetries = 3;

        #endregion

        #region 内部变量

        protected internal CommandInterceptor ActualCommandExecutor;

        /// <summary>
        ///     used to create instances for listeners, JavaDelegates, etc
        /// </summary>
        protected internal IArtifactFactory artifactFactory;

        protected internal ScriptingEngines scriptingEngines;
        protected internal IList<IResolverFactory> resolverFactories;
        protected internal ScriptingEnvironment scriptingEnvironment;
        protected internal IList<IScriptEnvResolver> scriptEnvResolvers;
        protected internal ScriptFactory scriptFactory;
        protected internal bool autoStoreScriptVariables;

        // BATCH ////////////////////////////////////////////////////////////////////

        protected internal IDictionary<string, IBatchJobHandler> batchHandlers;
        protected internal IList<IBatchJobHandler> customBatchJobHandlers;

        /// <summary>
        ///     decimal of jobs created by a batch seed job invocation
        /// </summary>
        protected internal int batchJobsPerSeed = 100;

        //protected internal IList<CmmnTransformListener> customPreCmmnTransformListeners;
        //protected internal IList<CmmnTransformListener> customPostCmmnTransformListeners;

        protected internal IDictionary<object, object> beans;
        protected internal IBpmnParseFactory BpmnParseFactory;

        protected internal IBusinessCalendarManager businessCalendarManager;

        // DEPLOYERS ////////////////////////////////////////////////////////////////

        protected internal IList<IDeployer> customPreDeployers;
        protected internal IList<IDeployer> customPostDeployers;
        protected internal IList<IDeployer> deployers;
        protected internal DeploymentCache deploymentCache;

        // CACHE ////////////////////////////////////////////////////////////////////

        protected internal ICacheFactory cacheFactory;
        protected internal int cacheCapacity = 1000;

        protected internal bool cmmnEnabled = true;

        protected internal ICmmnHistoryEventProducer cmmnHistoryEventProducer;

        protected internal IList<ICommandChecker> commandCheckers;

        protected internal CommandContextFactory commandContextFactory;

        /// <summary>
        ///     Separate command executor to be used for db schema operations. Must always use NON-JTA transactions
        /// </summary>
        protected internal ICommandExecutor commandExecutorSchemaOperations;

        protected internal ICorrelationHandler correlationHandler;
        protected internal IList<IEventHandler> customEventHandlers;

        // OTHER ////////////////////////////////////////////////////////////////////
        protected internal IList<IFormEngine> customFormEngines;
        protected internal IDictionary<string, Type> customFormFieldValidators;

        protected internal IList<AbstractFormFieldType> customFormTypes;

        /// <summary>
        ///     a list of supported custom history levels
        /// </summary>
        protected internal IList<IHistoryLevel> customHistoryLevels;

        protected internal IList<IIncidentHandler> customIncidentHandlers;

        protected internal IList<IPasswordEncryptor> customPasswordChecker;
        protected internal IList<IMigratingActivityInstanceValidator> customPostMigratingActivityInstanceValidators;
        protected internal IList<IMigrationActivityValidator> customPostMigrationActivityValidators;
        protected internal IList<IMigrationInstructionValidator> customPostMigrationInstructionValidators;

        // COMMAND EXECUTORS ////////////////////////////////////////////////////////

        // Command executor and interceptor stack

        protected internal IList<IMigratingActivityInstanceValidator> customPreMigratingActivityInstanceValidators;

        protected internal IList<IMigrationActivityValidator> customPreMigrationActivityValidators;

        protected internal IList<IMigrationInstructionValidator> customPreMigrationInstructionValidators;

        protected internal IRejectedJobsHandler customRejectedJobsHandler;

        // SESSION FACTORIES ////////////////////////////////////////////////////////

        //protected internal IList<ISessionFactory> customSessionFactories;

        /// <summary>
        ///     In some situations you want to set the schema to use for table checks / generation if the database metadata
        ///     doesn't return that correctly, see https://jira.codehaus.org/browse/ACT-1220,
        ///     https://jira.codehaus.org/browse/ACT-1062
        /// </summary>
        protected internal string databaseSchema;

        protected internal string databaseTablePrefix = "";

        protected internal DbEntityCacheKeyMapping dbEntityCacheKeyMapping =
            DbEntityCacheKeyMapping.DefaultEntityCacheKeyMapping();

        protected internal DbMetricsReporter dbMetricsReporter;
        //protected internal DbSqlSessionFactory dbSqlSessionFactory;

        protected internal IVariableSerializerFactory fallbackSerializerFactory;

        protected internal string defaultSerializationFormat = Variables.SerializationDataFormats.Net.ToString();
        public Encoding DefaultCharset;
        protected internal string DefaultCharsetName;

        // 任务的默认用户权限Default user permission for task
        protected internal Permissions defaultUserPermissionForTask;

        protected internal IDelegateInterceptor delegateInterceptor;

        protected internal bool disableStrictCallActivityValidation;
        protected internal bool dmnEnabled = true;
        protected internal IDmnEngine dmnEngine;

        public IDmnEngine DmnEngine
        {
            get { return dmnEngine; }
            //set { dmnEngine = value; }
        }

        // cmmn
        //protected internal CmmnTransformFactory cmmnTransformFactory;
        //protected internal DefaultCmmnElementHandlerRegistry cmmnElementHandlerRegistry;

        // dmn
        protected internal DefaultDmnEngineConfiguration dmnEngineConfiguration;

        public DefaultDmnEngineConfiguration DmnEngineConfiguration
        {
            get { return dmnEngineConfiguration; }
            set { dmnEngineConfiguration = value; }
        }

        protected internal IDmnHistoryEventProducer dmnHistoryEventProducer;

        /// <summary>
        ///     通过API提交的表达式的处理；可以用作远程代码执行的防护。
        ///     handling of expressions submitted via API; can be used as guards against remote code execution
        /// </summary>
        protected internal bool enableExpressionsInAdhocQueries;

        protected internal bool enableExpressionsInStoredQueries = true;
        protected internal bool enableFetchProcessDefinitionDescription = true;
        protected internal bool enableFetchScriptEngineFromProcessApplication = true;

        protected internal bool enableGracefulDegradationOnContextSwitchFailure = true;
        protected internal bool enableScriptCompilation = true;
        protected internal bool enableScriptEngineCaching = true;

        protected internal IDictionary<string, IEventHandler> eventHandlers;
        //protected internal Encoding defaultCharset = null;
        protected string processEngineName;
        protected internal ExpressionManager expressionManager;

        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //ORIGINAL LINE: protected org.camunda.bpm.engine.impl.PriorityProvider<org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration<?, object>> jobPriorityProvider;
        //protected internal PriorityProvider<JobDeclaration<object, object>> jobPriorityProvider;

        // 外部任务EXTERNAL TASK /////////////////////////////////////////////////////////////

        protected internal IFailedJobCommandFactory failedJobCommandFactory;
        //protected internal CaseService caseService = new CaseServiceImpl();

        protected internal bool forceCloseMybatisConnectionPool = true;
        protected internal IDictionary<string, IFormEngine> formEngines;
        protected internal FormTypes formTypes;
        protected internal FormValidators formValidators;

        protected internal IHistoryEventHandler historyEventHandler;

        /// <summary>
        ///     支持的历史级别列表
        ///     a list of supported history levels
        /// </summary>
        protected internal IList<IHistoryLevel> historyLevels;

        /// <summary>
        ///     用于获取身份提供程序会话的会话工厂
        ///     session factory to be used for obtaining identity provider sessions
        /// </summary>
        //protected internal ISessionFactory identityProviderSessionFactory;

        // MyBatis SQL会话工厂MYBATIS SQL SESSION FACTORY //////////////////////////////////////////////

        //protected internal IDbConnectionFactory sqlSessionFactory;
        //protected internal TransactionFactory transactionFactory;


        // 身份证生成器ID GENERATOR /////////////////////////////////////////////////////////////
        //protected internal IIdGenerator idGenerator;
        //protected internal DataSource idGeneratorDataSource;
        protected internal string idGeneratorDataSourceJndiName;

        // 事件处理程序INCIDENT HANDLER /////////////////////////////////////////////////////////

        protected internal IDictionary<string, IIncidentHandler> incidentHandlers;

        protected internal bool IsBpmnStacktraceVerbose;

        protected internal bool IsCreateDiagramOnDeploy;

        /// <summary>
        ///     允许设置进程引擎是否应该尝试重用第一级实体缓存。
        ///     默认设置是Fasle，使其提高异步执行性能。
        ///     Allows setting whether the process engine should try reusing the first level entity cache.
        ///     Default setting is false, enabling it improves performance of asynchronous continuations.
        /// </summary>
        protected internal bool IsDbEntityCacheReuseEnabled;

        protected internal bool IsDbHistoryUsed = true;

        protected internal bool IsDbIdentityUsed = true;
        protected internal bool IsDbMetricsReporterActivate = true;

        /// <summary>
        ///     如果TRUE进程引擎将试图获得独家锁前
        ///     创建部署。
        ///     If true the process engine will attempt to acquire an exclusive lock before
        ///     creating a deployment.
        /// </summary>
        protected internal bool IsDeploymentLockUsed = false;

        protected internal bool IsExecutionTreePrefetchEnabled = true;

        protected internal bool IsInvokeCustomVariableListeners = true;

        protected internal bool IsMetricsEnabled = true;

        protected internal bool IsUseSharedSqlSessionFactory;

        // job执行者JOB EXECUTOR /////////////////////////////////////////////////////////////
        protected internal IList<IJobHandler<IJobHandlerConfiguration>> customJobHandlers;
        protected internal IDictionary<string, IJobHandler> jobHandlers;
        protected internal JobExecutor.JobExecutor jobExecutor;

        /// <summary>
        ///     注册表的度量
        ///     the metrics registry
        /// </summary>
        protected internal MetricsRegistry metricsRegistry;

        protected internal IMetricsReporterIdProvider metricsReporterIdProvider;
        protected internal IList<IMigratingActivityInstanceValidator> migratingActivityInstanceValidators;
        protected internal IList<IMigratingCompensationInstanceValidator> migratingCompensationInstanceValidators;
        protected internal IList<IMigratingTransitionInstanceValidator> migratingTransitionInstanceValidators;

        // 迁移Migration
        protected internal IMigrationActivityMatcher migrationActivityMatcher;
        protected internal IMigrationInstructionGenerator migrationInstructionGenerator;
        protected internal IList<IMigrationInstructionValidator> migrationInstructionValidators;

        protected internal IPasswordEncryptor passwordEncryptor;

        protected internal IList<IBpmnParseListener> postParseListeners;

        protected internal IList<IBpmnParseListener> preParseListeners;

        protected internal ProcessApplicationManager processApplicationManager;

        /// <summary>
        ///     由该配置创建的进程流程引擎
        ///     The process engine created by this configuration.
        /// </summary>
        protected internal ProcessEngineImpl processEngine;

        protected internal ISet<string> registeredDeployments;

        //public static SqlSessionFactory cachedSqlSessionFactory;

        // SERVICES /////////////////////////////////////////////////////////////////

        #region SERVICES

        /// <summary>
        ///     If true, user operation log entries are only written if there is an
        ///     authenticated user present in the context. If false, user operation log
        ///     entries are written regardless of authentication state.
        /// </summary>
        protected internal bool restrictUserOperationLogToAuthenticatedUsers = true;

        protected internal ISaltGenerator saltGenerator;
        //protected internal IDictionary<Type, ISessionFactory> sessionFactories;

        protected internal ITenantIdProvider tenantIdProvider;
        protected internal ITransactionContextFactory transactionContextFactory;

        protected internal string wsSyncFactoryClassName = DefaultWsSyncFactory;

        #endregion

        #endregion

        #region command executors
        
        protected internal abstract ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequired { get; }
        
        protected internal abstract ICollection<CommandInterceptor> DefaultCommandInterceptorsTxRequiresNew { get; }

        // public static void initSqlSessionFactoryProperties(Properties properties, string databaseTablePrefix, string databaseType)
        // {

        //if (!string.ReferenceEquals(databaseType, null))
        //{
        //  properties.put("limitBefore", DbSqlSessionFactory.databaseSpecificLimitBeforeStatements[databaseType]);
        //  properties.put("limitAfter", DbSqlSessionFactory.databaseSpecificLimitAfterStatements[databaseType]);
        //  properties.put("innerLimitAfter", DbSqlSessionFactory.databaseSpecificInnerLimitAfterStatements[databaseType]);
        //  properties.put("limitBetween", DbSqlSessionFactory.databaseSpecificLimitBetweenStatements[databaseType]);
        //  properties.put("limitBetweenFilter", DbSqlSessionFactory.databaseSpecificLimitBetweenFilterStatements[databaseType]);
        //  properties.put("orderBy", DbSqlSessionFactory.databaseSpecificOrderByStatements[databaseType]);
        //  properties.put("limitBeforeNativeQuery", DbSqlSessionFactory.databaseSpecificLimitBeforeNativeQueryStatements[databaseType]);
        //  properties.put("distinct", DbSqlSessionFactory.databaseSpecificDistinct[databaseType]);

        //  properties.put("bitand1", DbSqlSessionFactory.databaseSpecificBitAnd1[databaseType]);
        //  properties.put("bitand2", DbSqlSessionFactory.databaseSpecificBitAnd2[databaseType]);
        //  properties.put("bitand3", DbSqlSessionFactory.databaseSpecificBitAnd3[databaseType]);

        //  properties.put("datepart1", DbSqlSessionFactory.databaseSpecificDatepart1[databaseType]);
        //  properties.put("datepart2", DbSqlSessionFactory.databaseSpecificDatepart2[databaseType]);
        //  properties.put("datepart3", DbSqlSessionFactory.databaseSpecificDatepart3[databaseType]);

        //  properties.put("trueConstant", DbSqlSessionFactory.databaseSpecificTrueConstant[databaseType]);
        //  properties.put("falseConstant", DbSqlSessionFactory.databaseSpecificFalseConstant[databaseType]);

        //  properties.put("dbSpecificDummyTable", DbSqlSessionFactory.databaseSpecificDummyTable[databaseType]);
        //  properties.put("dbSpecificIfNullFunction", DbSqlSessionFactory.databaseSpecificIfNull[databaseType]);

        //  IDictionary<string, string> constants = DbSqlSessionFactory.dbSpecificConstants[databaseType];
        //  foreach (KeyValuePair<string, string> entry in constants)
        //  {
        //	properties.put(entry.Key, entry.Value);
        //  }
        //}
        // }
        //TODO 加载数据库Maping配置文件
        protected internal virtual Stream MyBatisXmlConfigurationSteam { get; }
        //= ReflectUtil.GetResourceAsStream(DefaultMybatisMappingFile);

        #endregion

        #region getters and setters


        public virtual IHistoryLevel DefaultHistoryLevel
        {
            get
            {
                if (historyLevels != null)
                    foreach (var historyLevel in historyLevels)
                        if (!ReferenceEquals(HistoryDefault, null) &&
HistoryDefault.Equals(historyLevel.Name, StringComparison.CurrentCultureIgnoreCase))
                            return historyLevel;

                return null;
            }
        }

        public override ProcessEngineConfiguration SetProcessEngineName(string processEngineName)
        {
            this.processEngineName = processEngineName;
            return this;
        }

        public virtual IList<CommandInterceptor> CustomPreCommandInterceptorsTxRequired { get; protected internal set; }

        public virtual IList<CommandInterceptor> CustomPostCommandInterceptorsTxRequired
        {
            get; protected internal set;
        }

        public virtual IList<CommandInterceptor> CommandInterceptorsTxRequired { get; protected internal set; }

        public virtual ICommandExecutor CommandExecutorTxRequired { get; protected internal set; }

        public virtual IList<CommandInterceptor> CustomPreCommandInterceptorsTxRequiresNew
        {
            get;
            protected internal set;
        }

        public virtual IList<CommandInterceptor> CustomPostCommandInterceptorsTxRequiresNew
        {
            get;
            protected internal set;
        }

        public virtual IList<CommandInterceptor> CommandInterceptorsTxRequiresNew { get; protected internal set; }

        public virtual ICommandExecutor CommandExecutorTxRequiresNew { get; protected internal set; }

        public virtual IRepositoryService RepositoryService { get; protected internal set; } =
            new RepositoryServiceImpl();

        public virtual IRuntimeService RuntimeService { get; protected internal set; } = new RuntimeServiceImpl();

        public virtual IHistoryService HistoryService { get; protected internal set; } = new HistoryServiceImpl();

        public virtual IIdentityService IdentityService { get; protected internal set; } = new IdentityServiceImpl();

        public virtual ITaskService TaskService { get; protected internal set; } = new TaskServiceImpl();

        public virtual IFormService FormService { get; protected internal set; } = new FormServiceImpl();

        public virtual IManagementService ManagementService { get; protected internal set; } =
            new ManagementServiceImpl();

        public virtual IAuthorizationService AuthorizationService { get; } = new AuthorizationServiceImpl();

        //public virtual CaseService CaseService
        //{
        //    get
        //    {
        //        return caseService;
        //    }
        //    set
        //    {
        //        this.caseService = value;
        //    }
        //}


        public virtual IFilterService FilterService { get; set; } = new FilterServiceImpl();


        public virtual IExternalTaskService ExternalTaskService { get; set; } = new ExternalTaskServiceImpl();


        public virtual IDecisionService DecisionService { get; set; } = new DecisionServiceImpl();


        //public virtual IDictionary<Type, ISessionFactory> SessionFactories
        //{
        //    get { return sessionFactories; }
        //}

        public virtual IList<IDeployer> Deployers
        {
            get
            {
                return deployers;
            }
        }

        public virtual ProcessEngineConfigurationImpl SetDeployers(IList<IDeployer> deployers)
        {
            this.deployers = deployers;
            return this;
        }

        public virtual JobExecutor.JobExecutor JobExecutor
        {
            get { return jobExecutor; }
        }

        public virtual IPriorityProvider<IJobDeclaration> JobPriorityProvider { get; set; }


        public virtual IPriorityProvider<ExternalTaskActivityBehavior> ExternalTaskPriorityProvider { get; set; }

        //TODO Autofac
        public virtual IDGenerator IdGenerator
        {
            get
            {
                IScope scope = ObjectContainer.BeginLifetimeScope();
                return scope.Resolve<IDGenerator>();
                //return idGenerator;
            }
            set
            {
                IdGenerator = value;
            }
        }

        public virtual string WsSyncFactoryClassName
        {
            get { return wsSyncFactoryClassName; }
        }

        public virtual IDictionary<string, IFormEngine> FormEngines
        {
            get { return formEngines; }
        }

        public virtual FormTypes FormTypes
        {
            get { return formTypes; }
        }

        //public virtual ScriptingEngines ScriptingEngines
        //{
        //    get
        //    {
        //        return scriptingEngines;
        //    }
        //}

        //public virtual ProcessEngineConfigurationImpl setScriptingEngines(ScriptingEngines scriptingEngines)
        //{
        //    this.scriptingEngines = scriptingEngines;
        //    return this;
        //}


        public virtual IVariableSerializerFactory FallbackSerializerFactory
        {
            get
            {
                return fallbackSerializerFactory;
            }
            set
            {
                this.fallbackSerializerFactory = value;
            }
        }


        //public virtual ProcessEngineConfigurationImpl setVariableTypes(IVariableSerializers variableSerializers)
        //{
        //    this.variableSerializers = variableSerializers;
        //    return this;
        //}

        public virtual ExpressionManager ExpressionManager
        {
            get { return expressionManager; }
            set { expressionManager = value; }
        }

        public virtual IBusinessCalendarManager BusinessCalendarManager
        {
            get { return businessCalendarManager; }
        }

        public virtual CommandContextFactory CommandContextFactory
        {
            get { return commandContextFactory; }
        }

        public virtual ITransactionContextFactory TransactionContextFactory
        {
            get { return transactionContextFactory; }
        }


        // public virtual IList<Deployer> CustomPreDeployers
        // {
        //  get
        //  {
        //	return customPreDeployers;
        //  }
        // }


        // public virtual ProcessEngineConfigurationImpl setCustomPreDeployers(IList<Deployer> customPreDeployers)
        // {
        //this.customPreDeployers = customPreDeployers;
        //return this;
        // }


        // public virtual IList<Deployer> CustomPostDeployers
        // {
        //  get
        //  {
        //	return customPostDeployers;
        //  }
        // }


        // public virtual ProcessEngineConfigurationImpl setCustomPostDeployers(IList<Deployer> customPostDeployers)
        // {
        //this.customPostDeployers = customPostDeployers;
        //return this;
        // }

        public virtual ICacheFactory CacheFactory
        {
            set
            {
                this.cacheFactory = value;
            }
        }

        public virtual int CacheCapacity
        {
            set { cacheCapacity = value; }
        }

        public virtual bool EnableFetchProcessDefinitionDescription
        {
            set { enableFetchProcessDefinitionDescription = value; }
            get { return enableFetchProcessDefinitionDescription; }
        }


        public virtual Permissions DefaultUserPermissionForTask
        {
            get { return defaultUserPermissionForTask; }
            set { defaultUserPermissionForTask = value; }
        }

        public virtual IDictionary<string, IJobHandler> JobHandlers
        {
            get
            {
                return jobHandlers;
            }
        }


        public virtual ProcessEngineConfigurationImpl setJobHandlers(IDictionary<string, IJobHandler> jobHandlers)
        {
            this.jobHandlers = jobHandlers;
            return this;
        }


        //public virtual IDbConnectionFactory SqlSessionFactory
        //{
        //    get
        //    {
        //        return sqlSessionFactory;
        //    }
        //}


        //public virtual ProcessEngineConfigurationImpl setSqlSessionFactory(IDbConnectionFactory sqlSessionFactory)
        //{
        //    this.sqlSessionFactory = sqlSessionFactory;
        //    return this;
        //}


        //public virtual DbSqlSessionFactory DbSqlSessionFactory
        //{
        //    get { return dbSqlSessionFactory; }
        //}

        //public virtual TransactionFactory TransactionFactory
        //{
        // get
        // {
        //return transactionFactory;
        // }
        //}

        // public virtual ProcessEngineConfigurationImpl setTransactionFactory(TransactionFactory transactionFactory)
        // {
        //this.transactionFactory = transactionFactory;
        //return this;
        // }

        //public virtual IList<ISessionFactory> CustomSessionFactories
        //{
        //    get { return customSessionFactories; }
        //}

        // public virtual IList<JobHandler> CustomJobHandlers
        // {
        //  get
        //  {
        //	return customJobHandlers;
        //  }
        // }

        // public virtual ProcessEngineConfigurationImpl setCustomJobHandlers(IList<JobHandler> customJobHandlers)
        // {
        //this.customJobHandlers = customJobHandlers;
        //return this;
        // }

        public virtual IList<IFormEngine> CustomFormEngines
        {
            get { return customFormEngines; }
        }

        public virtual IList<AbstractFormFieldType> CustomFormTypes
        {
            get { return customFormTypes; }
        }

        // public virtual IList<TypedValueSerializer> CustomPreVariableSerializers
        // {
        //  get
        //  {
        //	return customPreVariableSerializers;
        //  }
        // }


        // public virtual ProcessEngineConfigurationImpl setCustomPreVariableSerializers(IList<TypedValueSerializer> customPreVariableTypes)
        // {
        //this.customPreVariableSerializers = customPreVariableTypes;
        //return this;
        // }


        // public virtual IList<TypedValueSerializer> CustomPostVariableSerializers
        // {
        //  get
        //  {
        //	return customPostVariableSerializers;
        //  }
        // }


        // public virtual ProcessEngineConfigurationImpl setCustomPostVariableSerializers(IList<TypedValueSerializer> customPostVariableTypes)
        // {
        //this.customPostVariableSerializers = customPostVariableTypes;
        //return this;
        // }

        public virtual IList<IBpmnParseListener> CustomPreBpmnParseListeners
        {
            get { return preParseListeners; }
            set { preParseListeners = value; }
        }


        public virtual IList<IBpmnParseListener> CustomPostBpmnParseListeners
        {
            get { return postParseListeners; }
            set { postParseListeners = value; }
        }


        /// @deprecated use
        /// <seealso cref="#getCustomPreBPMNParseListeners" />
        /// instead.
        [Obsolete("use <seealso cref='#getCustomPreBPMNParseListeners'/> instead.")]
        public virtual IList<IBpmnParseListener> PreParseListeners
        {
            get { return preParseListeners; }
            set { preParseListeners = value; }
        }


        /// @deprecated use
        /// <seealso cref="#getCustomPostBPMNParseListeners" />
        /// instead.
        [Obsolete("use <seealso cref=\"#getCustomPostBPMNParseListeners\"/> instead.")]
        public virtual IList<IBpmnParseListener> PostParseListeners
        {
            get { return postParseListeners; }
            set { postParseListeners = value; }
        }


        //public virtual IList<CmmnTransformListener> CustomPreCmmnTransformListeners
        //{
        // get
        // {
        //return customPreCmmnTransformListeners;
        // }
        // set
        // {
        //this.customPreCmmnTransformListeners = value;
        // }
        //}


        //public virtual IList<CmmnTransformListener> CustomPostCmmnTransformListeners
        //{
        // get
        // {
        //return customPostCmmnTransformListeners;
        // }
        // set
        // {
        //this.customPostCmmnTransformListeners = value;
        // }
        //}


        public virtual IDictionary<object, object> Beans
        {
            get { return beans; }
            set { beans = value; }
        }


        // public override ProcessEngineConfigurationImpl setClassLoader(ClassLoader classLoader)
        // {
        //base.ClassLoader = classLoader;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setDatabaseType(string databaseType)
        // {
        //base.DatabaseType = databaseType;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setDataSource(DataSource dataSource)
        // {
        //base.DataSource = dataSource;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setDatabaseSchemaUpdate(string databaseSchemaUpdate)
        // {
        //base.DatabaseSchemaUpdate = databaseSchemaUpdate;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setHistory(string history)
        // {
        //base.History = history;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setIdBlockSize(int idBlockSize)
        // {
        //base.IdBlockSize = idBlockSize;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcDriver(string jdbcDriver)
        // {
        //base.JdbcDriver = jdbcDriver;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcPassword(string jdbcPassword)
        // {
        //base.JdbcPassword = jdbcPassword;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcUrl(string jdbcUrl)
        // {
        //base.JdbcUrl = jdbcUrl;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcUsername(string jdbcUsername)
        // {
        //base.JdbcUsername = jdbcUsername;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJobExecutorActivate(bool jobExecutorActivate)
        // {
        //base.JobExecutorActivate = jobExecutorActivate;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerDefaultFrom(string mailServerDefaultFrom)
        // {
        //base.MailServerDefaultFrom = mailServerDefaultFrom;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerHost(string mailServerHost)
        // {
        //base.MailServerHost = mailServerHost;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerPassword(string mailServerPassword)
        // {
        //base.MailServerPassword = mailServerPassword;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerPort(int mailServerPort)
        // {
        //base.MailServerPort = mailServerPort;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerUseTLS(bool useTLS)
        // {
        //base.MailServerUseTLS = useTLS;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setMailServerUsername(string mailServerUsername)
        // {
        //base.MailServerUsername = mailServerUsername;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcMaxActiveConnections(int jdbcMaxActiveConnections)
        // {
        //base.JdbcMaxActiveConnections = jdbcMaxActiveConnections;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcMaxCheckoutTime(int jdbcMaxCheckoutTime)
        // {
        //base.JdbcMaxCheckoutTime = jdbcMaxCheckoutTime;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcMaxIdleConnections(int jdbcMaxIdleConnections)
        // {
        //base.JdbcMaxIdleConnections = jdbcMaxIdleConnections;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcMaxWaitTime(int jdbcMaxWaitTime)
        // {
        //base.JdbcMaxWaitTime = jdbcMaxWaitTime;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setTransactionsExternallyManaged(bool transactionsExternallyManaged)
        // {
        //base.TransactionsExternallyManaged = transactionsExternallyManaged;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJpaEntityManagerFactory(object jpaEntityManagerFactory)
        // {
        //this.jpaEntityManagerFactory = jpaEntityManagerFactory;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJpaHandleTransaction(bool jpaHandleTransaction)
        // {
        //this.jpaHandleTransaction = jpaHandleTransaction;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJpaCloseEntityManager(bool jpaCloseEntityManager)
        // {
        //this.jpaCloseEntityManager = jpaCloseEntityManager;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcPingEnabled(bool jdbcPingEnabled)
        // {
        //this.jdbcPingEnabled = jdbcPingEnabled;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcPingQuery(string jdbcPingQuery)
        // {
        //this.jdbcPingQuery = jdbcPingQuery;
        //return this;
        // }

        // public override ProcessEngineConfigurationImpl setJdbcPingConnectionNotUsedFor(int jdbcPingNotUsedFor)
        // {
        //this.jdbcPingConnectionNotUsedFor = jdbcPingNotUsedFor;
        //return this;
        // }


        private int failedJobListenerMaxRetries = DefaultFailedJobListenerMaxRetries;
        public virtual int FailedJobListenerMaxRetries
        {
            get { return this.failedJobListenerMaxRetries; }
            set { failedJobListenerMaxRetries = value; }
        }


        public virtual bool DbIdentityUsed
        {
            get { return IsDbIdentityUsed; }
            set { IsDbIdentityUsed = value; }
        }


        public virtual bool DbHistoryUsed
        {
            get { return IsDbHistoryUsed; }
            set { IsDbHistoryUsed = value; }
        }

        public virtual IDelegateInterceptor DelegateInterceptor
        {
            get { return delegateInterceptor; }
        }

        public virtual IRejectedJobsHandler CustomRejectedJobsHandler
        {
            get { return customRejectedJobsHandler; }
        }

        public virtual IDictionary<string, IEventHandler> EventHandlers
        {
            set { eventHandlers = value; }
            get { return eventHandlers; }
        }


        public virtual IList<IEventHandler> CustomEventHandlers
        {
            get { return customEventHandlers; }
            set { customEventHandlers = value; }
        }


        public virtual IFailedJobCommandFactory FailedJobCommandFactory
        {
            get { return failedJobCommandFactory; }
            set { failedJobCommandFactory = value; }
        }

        public virtual string DatabaseTablePrefix
        {
            get { return databaseTablePrefix; }
        }

        public virtual bool CreateDiagramOnDeploy
        {
            get { return IsCreateDiagramOnDeploy; }
        }

        public virtual string DatabaseSchema
        {
            get { return databaseSchema; }
            set { databaseSchema = value; }
        }


        //public virtual DataSource IdGeneratorDataSource
        //{
        // get
        // {
        //return idGeneratorDataSource;
        // }
        // set
        // {
        //this.idGeneratorDataSource = value;
        // }
        //}


        public virtual string IdGeneratorDataSourceJndiName
        {
            get { return idGeneratorDataSourceJndiName; }
            set { idGeneratorDataSourceJndiName = value; }
        }


        public virtual ProcessApplicationManager ProcessApplicationManager
        {
            get { return processApplicationManager; }
            set { processApplicationManager = value; }
        }


        public virtual ICommandExecutor CommandExecutorSchemaOperations
        {
            get { return commandExecutorSchemaOperations; }
            set { commandExecutorSchemaOperations = value; }
        }


        public virtual ICorrelationHandler CorrelationHandler
        {
            get { return correlationHandler; }
            set { correlationHandler = value; }
        }

        public virtual IHistoryEventHandler HistoryEventHandler
        {
            get { return historyEventHandler; }
            set { historyEventHandler = value; }
        }

        public virtual IDictionary<string, IIncidentHandler> IncidentHandlers
        {
            get { return incidentHandlers; }
            set { incidentHandlers = value; }
        }


        public virtual IList<IIncidentHandler> CustomIncidentHandlers
        {
            get { return customIncidentHandlers; }
            set { customIncidentHandlers = value; }
        }

        public virtual IDictionary<string, IBatchJobHandler> BatchHandlers
        {
            get { return batchHandlers; }
            set { batchHandlers = value; }
        }



        public virtual IList<IBatchJobHandler> CustomBatchJobHandlers
        {
            get
            {
                return customBatchJobHandlers;
            }
            set
            {
                this.customBatchJobHandlers = value;
            }
        }


        public virtual int BatchJobsPerSeed
        {
            get { return batchJobsPerSeed; }
            set { batchJobsPerSeed = value; }
        }


        public virtual int InvocationsPerBatchJob { get; set; } = 1;


        public virtual int BatchPollTime { get; set; } = 30;


        public virtual long BatchJobPriority { get; set; } = DefaultJobPriorityProvider.DEFAULT_PRIORITY;


        //public virtual ISessionFactory IdentityProviderSessionFactory
        //{
        //    get { return identityProviderSessionFactory; }
        //    set { identityProviderSessionFactory = value; }
        //}


        public virtual ISaltGenerator SaltGenerator
        {
            get { return saltGenerator; }
            set { saltGenerator = value; }
        }


        public virtual IPasswordEncryptor PasswordEncryptor
        {
            set { passwordEncryptor = value; }
            get { return passwordEncryptor; }
        }


        public virtual IList<IPasswordEncryptor> CustomPasswordChecker
        {
            get { return customPasswordChecker; }
            set { customPasswordChecker = value; }
        }


        public virtual PasswordManager PasswordManager { get; set; }


        public virtual ISet<string> RegisteredDeployments
        {
            get { return registeredDeployments; }
            set { registeredDeployments = value; }
        }


        public virtual IResourceAuthorizationProvider ResourceAuthorizationProvider { get; set; }


        public virtual IList<IProcessEnginePlugin> ProcessEnginePlugins { get; set; } = new List<IProcessEnginePlugin>();

        public virtual IHistoryEventProducer HistoryEventProducer { get; protected internal set; }

        public virtual ICmmnHistoryEventProducer CmmnHistoryEventProducer
        {
            get { return cmmnHistoryEventProducer; }
        }

        public virtual IDmnHistoryEventProducer DmnHistoryEventProducer
        {
            get { return dmnHistoryEventProducer; }
        }

        public virtual IDictionary<string, Type> CustomFormFieldValidators
        {
            get { return customFormFieldValidators; }
            set { customFormFieldValidators = value; }
        }


        public virtual FormValidators FormValidators
        {
            set { formValidators = value; }
            get { return formValidators; }
        }


        public virtual bool ExecutionTreePrefetchEnabled
        {
            get { return IsExecutionTreePrefetchEnabled; }
            set { IsExecutionTreePrefetchEnabled = value; }
        }


        public virtual ProcessEngineImpl ProcessEngine
        {
            get { return processEngine; }
        }

        /// <summary>
        ///     If set to true, the process engine will save all script variables (created from Java Script, Groovy ...)
        ///     as process variables.
        /// </summary>
        public virtual bool IsAutoStoreScriptVariables
        {
            set { autoStoreScriptVariables = value; }
            get { return autoStoreScriptVariables; }
        }


        /// <summary>
        ///     If set to true, the process engine will attempt to pre-compile script sources at runtime
        ///     to optimize script task execution performance.
        /// </summary>
        public virtual bool EnableScriptCompilation
        {
            set { enableScriptCompilation = value; }
            get { return enableScriptCompilation; }
        }


        public virtual bool EnableGracefulDegradationOnContextSwitchFailure
        {
            get { return enableGracefulDegradationOnContextSwitchFailure; }
            set { enableGracefulDegradationOnContextSwitchFailure = value; }
        }


        /// <returns> true if the process engine acquires an exclusive lock when creating a deployment. </returns>
        public virtual bool DeploymentLockUsed
        {
            get { return IsDeploymentLockUsed; }
            set { IsDeploymentLockUsed = value; }
        }


        public virtual bool CmmnEnabled
        {
            get { return false; }// cmmnEnabled; }
            set { cmmnEnabled = value; }
        }


        public virtual bool DmnEnabled
        {
            get { return dmnEnabled; }
            set { dmnEnabled = value; }
        }

        public virtual IArtifactFactory ArtifactFactory
        {
            get { return artifactFactory; }
        }

        // public virtual ProcessEngineConfigurationImpl setDefaultCharset(Encoding defautlCharset)
        // {
        //this.defaultCharset = defautlCharset;
        //return this;
        // }

        // public virtual Encoding DefaultCharset
        // {
        //  get
        //  {
        //	return defaultCharset;
        //  }
        // }

        public virtual bool DbEntityCacheReuseEnabled
        {
            get { return IsDbEntityCacheReuseEnabled; }
            set { IsDbEntityCacheReuseEnabled = value; }
        }

        public virtual DbEntityCacheKeyMapping DbEntityCacheKeyMapping
        {
            get { return dbEntityCacheKeyMapping; }
        }

        public virtual IList<IHistoryLevel> HistoryLevels
        {
            get { return historyLevels; }
        }

        public virtual IList<IHistoryLevel> CustomHistoryLevels
        {
            get { return customHistoryLevels; }
        }

        public virtual bool InvokeCustomVariableListeners
        {
            get { return IsInvokeCustomVariableListeners; }
        }

        // public virtual void close()
        // {
        //if (forceCloseMybatisConnectionPool && dataSource is PooledDataSource)
        //{

        //  // ACT-233: connection pool of Ibatis is not properely initialized if this is not called!
        //  ((PooledDataSource) dataSource).forceCloseAll();
        //}
        // }

        public virtual MetricsRegistry MetricsRegistry
        {
            get { return metricsRegistry; }
        }

        public virtual bool MetricsEnabled
        {
            get { return IsMetricsEnabled; }
        }

        public virtual DbMetricsReporter DbMetricsReporter
        {
            get { return dbMetricsReporter; }
        }

        public virtual bool DbMetricsReporterActivate
        {
            get { return IsDbMetricsReporterActivate; }
            set { IsDbMetricsReporterActivate = value; }
        }

        public virtual IMetricsReporterIdProvider MetricsReporterIdProvider
        {
            get { return metricsReporterIdProvider; }
            set { metricsReporterIdProvider = value; }
        }


        public virtual bool EnableScriptEngineCaching
        {
            get { return enableScriptEngineCaching; }
        }

        public virtual bool IsEnableFetchScriptEngineFromProcessApplication
        {
            get { return enableFetchScriptEngineFromProcessApplication; }
        }

        public virtual bool EnableExpressionsInAdhocQueries
        {
            get { return enableExpressionsInAdhocQueries; }
            set { enableExpressionsInAdhocQueries = value; }
        }


        public virtual bool EnableExpressionsInStoredQueries
        {
            get { return enableExpressionsInStoredQueries; }
            set { enableExpressionsInStoredQueries = value; }
        }

        public virtual bool BpmnStacktraceVerbose
        {
            get { return IsBpmnStacktraceVerbose; }
        }

        public virtual bool ForceCloseMybatisConnectionPool
        {
            get { return forceCloseMybatisConnectionPool; }
        }

        public virtual bool RestrictUserOperationLogToAuthenticatedUsers
        {
            get { return restrictUserOperationLogToAuthenticatedUsers; }
        }

        public virtual ITenantIdProvider TenantIdProvider
        {
            get { return tenantIdProvider; }
            set { tenantIdProvider = value; }
        }

        public virtual IMigrationActivityMatcher MigrationActivityMatcher
        {
            set { migrationActivityMatcher = value; }
            get { return migrationActivityMatcher; }
        }


        public virtual IList<IMigrationActivityValidator> CustomPreMigrationActivityValidators
        {
            set { customPreMigrationActivityValidators = value; }
            get { return customPreMigrationActivityValidators; }
        }


        public virtual IList<IMigrationActivityValidator> CustomPostMigrationActivityValidators
        {
            set { customPostMigrationActivityValidators = value; }
            get { return customPostMigrationActivityValidators; }
        }


        public virtual IList<IMigrationActivityValidator> DefaultMigrationActivityValidators
        {
            get
            {
                IList<IMigrationActivityValidator> migrationActivityValidators = new List<IMigrationActivityValidator>();
                migrationActivityValidators.Add(SupportedActivityValidator.Instance);
                migrationActivityValidators.Add(SupportedPassiveEventTriggerActivityValidator.Instance);
                migrationActivityValidators.Add(NoCompensationHandlerActivityValidator.Instance);
                return migrationActivityValidators;
            }
        }

        public virtual IMigrationInstructionGenerator MigrationInstructionGenerator
        {
            set { migrationInstructionGenerator = value; }
            get { return migrationInstructionGenerator; }
        }


        public virtual IList<IMigrationInstructionValidator> MigrationInstructionValidators
        {
            set { migrationInstructionValidators = value; }
            get { return migrationInstructionValidators; }
        }


        public virtual IList<IMigrationInstructionValidator> CustomPostMigrationInstructionValidators
        {
            set { customPostMigrationInstructionValidators = value; }
            get { return customPostMigrationInstructionValidators; }
        }


        public virtual IList<IMigrationInstructionValidator> CustomPreMigrationInstructionValidators
        {
            set { customPreMigrationInstructionValidators = value; }
            get { return customPreMigrationInstructionValidators; }
        }


        public virtual IList<IMigrationInstructionValidator> DefaultMigrationInstructionValidators
        {
            get
            {
                IList<IMigrationInstructionValidator> migrationInstructionValidators =
                    new List<IMigrationInstructionValidator>();
                migrationInstructionValidators.Add(new SameBehaviorInstructionValidator());
                migrationInstructionValidators.Add(new SameEventTypeValidator());
                migrationInstructionValidators.Add(new OnlyOnceMappedActivityInstructionValidator());
                migrationInstructionValidators.Add(new CannotAddMultiInstanceBodyValidator());
                migrationInstructionValidators.Add(new CannotAddMultiInstanceInnerActivityValidator());
                migrationInstructionValidators.Add(new CannotRemoveMultiInstanceInnerActivityValidator());
                migrationInstructionValidators.Add(new GatewayMappingValidator());
                migrationInstructionValidators.Add(new SameEventScopeInstructionValidator());
                migrationInstructionValidators.Add(new UpdateEventTriggersValidator());
                migrationInstructionValidators.Add(new AdditionalFlowScopeInstructionValidator());
                migrationInstructionValidators.Add(new ConditionalEventUpdateEventTriggerValidator());
                return migrationInstructionValidators;
            }
        }

        public virtual IList<IMigratingActivityInstanceValidator> MigratingActivityInstanceValidators
        {
            set { migratingActivityInstanceValidators = value; }
            get { return migratingActivityInstanceValidators; }
        }


        public virtual IList<IMigratingActivityInstanceValidator> CustomPostMigratingActivityInstanceValidators
        {
            set { customPostMigratingActivityInstanceValidators = value; }
            get { return customPostMigratingActivityInstanceValidators; }
        }


        public virtual IList<IMigratingActivityInstanceValidator> CustomPreMigratingActivityInstanceValidators
        {
            set { customPreMigratingActivityInstanceValidators = value; }
            get { return customPreMigratingActivityInstanceValidators; }
        }


        public virtual IList<IMigratingTransitionInstanceValidator> MigratingTransitionInstanceValidators
        {
            get { return migratingTransitionInstanceValidators; }
        }

        public virtual IList<IMigratingCompensationInstanceValidator> MigratingCompensationInstanceValidators
        {
            get { return migratingCompensationInstanceValidators; }
        }

        public virtual IList<IMigratingActivityInstanceValidator> DefaultMigratingActivityInstanceValidators
        {
            get
            {
                IList<IMigratingActivityInstanceValidator> migratingActivityInstanceValidators =
                    new List<IMigratingActivityInstanceValidator>();

                migratingActivityInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
                migratingActivityInstanceValidators.Add(new VariableConflictActivityInstanceValidator());
                migratingActivityInstanceValidators.Add(new SupportedActivityInstanceValidator());

                return migratingActivityInstanceValidators;
            }
        }

        public virtual IList<IMigratingTransitionInstanceValidator> DefaultMigratingTransitionInstanceValidators
        {
            get
            {
                IList<IMigratingTransitionInstanceValidator> migratingTransitionInstanceValidators =
                    new List<IMigratingTransitionInstanceValidator>();

                migratingTransitionInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
                migratingTransitionInstanceValidators.Add(new AsyncAfterMigrationValidator());
                migratingTransitionInstanceValidators.Add(new AsyncProcessStartMigrationValidator());
                migratingTransitionInstanceValidators.Add(new AsyncMigrationValidator());

                return migratingTransitionInstanceValidators;
            }
        }

        public virtual IList<ICommandChecker> CommandCheckers
        {
            get { return commandCheckers; }
            set { commandCheckers = value; }
        }

        public virtual bool UseSharedSqlSessionFactory
        {
            get { return IsUseSharedSqlSessionFactory; }
            set { IsUseSharedSqlSessionFactory = value; }
        }

        public virtual bool DisableStrictCallActivityValidation
        {
            get { return disableStrictCallActivityValidation; }
            set { disableStrictCallActivityValidation = value; }
        }

        #endregion

        //TODO ProcessEngine重构入口 初始化了大量基础设施来支撑
        #region Init

        protected internal virtual void Init()
        {
            InvokePreInit();
            InitDefaultCharset();
            InitHistoryLevel();
            //TODO InitHistoryEventProducer
            InitHistoryEventProducer();
            //InitCmmnHistoryEventProducer();
            InitDmnHistoryEventProducer();
            InitHistoryEventHandler();
            InitExpressionManager();
            InitBeans();
            InitArtifactFactory();
            InitFormEngines();
            InitFormTypes();
            InitFormFieldValidators();
            InitScripting();
            InitDmnEngine();
            InitBusinessCalendarManager();
            InitCommandContextFactory();
            InitTransactionContextFactory();
            InitCommandExecutors();
            InitServices();
            InitIdGenerator();
            InitDeployers();
            InitJobProvider();
            InitExternalTaskPriorityProvider();
            initBatchHandlers();
            InitJobExecutor();
            InitDataSource();
            InitTransactionFactory();
            InitSqlSessionFactory();
            //InitIdentityProviderSessionFactory();
            //InitSessionFactories();
            InitITypeValueResolver();
            //InitJpa();
            InitDelegateInterceptor();
            initEventHandlers();
            InitFailedJobCommandFactory();
            InitProcessApplicationManager();
            initCorrelationHandler();
            initIncidentHandlers();
            InitPasswordDigest();
            InitDeploymentRegistration();
            InitResourceAuthorizationProvider();
            InitMetrics();
            InitMigration();
            InitCommandCheckers();
            InitDefaultUserPermissionForTask();
            InvokePostInit();
        }
        public virtual void InitHistoryCleanup()
        {
            //if (historyCleanupBatchWindowStartTime != null)
            //{
            //    initHistoryCleanupBatchWindowStartTime();
            //}

            //if (historyCleanupBatchWindowEndTime != null)
            //{
            //    initHistoryCleanupBatchWindowEndTime();
            //}

            //if (historyCleanupBatchSize > HistoryCleanupBatch.MAX_BATCH_SIZE || historyCleanupBatchSize <= 0)
            //{
            //    throw LOG.invalidPropertyValue("historyCleanupBatchSize", historyCleanupBatchSize.ToString(), string.Format("value for batch size should be between 1 and {0}", HistoryCleanupBatch.MAX_BATCH_SIZE));
            //}

            //if (historyCleanupBatchThreshold < 0)
            //{
            //    throw LOG.invalidPropertyValue("historyCleanupBatchThreshold", historyCleanupBatchThreshold.ToString(), "History cleanup batch threshold cannot be negative.");
            //}
        }

        private void InitHistoryCleanupBatchWindowEndTime()
        {
            //try
            //{
            //    historyCleanupBatchWindowEndTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(historyCleanupBatchWindowEndTime);
            //}
            //catch (ParseException)
            //{
            //    throw LOG.invalidPropertyValue("historyCleanupBatchWindowEndTime", historyCleanupBatchWindowEndTime);
            //}
        }

        private void InitHistoryCleanupBatchWindowStartTime()
        {
            //try
            //{
            //    historyCleanupBatchWindowStartTimeAsDate = HistoryCleanupHelper.parseTimeConfiguration(historyCleanupBatchWindowStartTime);
            //}
            //catch (ParseException)
            //{
            //    throw LOG.invalidPropertyValue("historyCleanupBatchWindowStartTime", historyCleanupBatchWindowStartTime);
            //}
        }

        protected internal virtual void InvokePreInit()
        {
            foreach (var plugin in ProcessEnginePlugins)
            {
                Log.PluginActivated(plugin.ToString(), ProcessEngineName);
                plugin.PreInit(this);
            }
        }

        protected internal virtual void InvokePostInit()
        {
            foreach (var plugin in ProcessEnginePlugins)
                plugin.PostInit(this);
        }

        protected internal virtual void InvokePostProcessEngineBuild(IProcessEngine engine)
        {
            foreach (var plugin in ProcessEnginePlugins)
                plugin.PostProcessEngineBuild(engine);
        }


        // failedJobCommandFactory ////////////////////////////////////////////////////////
        /// <summary>
        ///     Init failedJobCommandFactory DefaultFailedJobCommandFactory
        /// </summary>
        protected internal virtual void InitFailedJobCommandFactory()
        {
            if (failedJobCommandFactory == null)
                failedJobCommandFactory = new DefaultFailedJobCommandFactory();
        }

        // incident handlers /////////////////////////////////////////////////////////////
        /// <summary>
        ///     init事件处理程序 incidentHandlers
        /// </summary>
        protected internal virtual void initIncidentHandlers()
        {
            if (incidentHandlers == null)
            {
                incidentHandlers = new Dictionary<string, IIncidentHandler>();

                var failedJobIncidentHandler = new DefaultIncidentHandler(IncidentFields.FailedJobHandlerType);
                incidentHandlers[failedJobIncidentHandler.IncidentHandlerType] = failedJobIncidentHandler;

                var failedExternalTaskIncidentHandler =
                    new DefaultIncidentHandler(IncidentFields.ExternalTaskHandlerType);
                incidentHandlers[failedExternalTaskIncidentHandler.IncidentHandlerType] =
                    failedExternalTaskIncidentHandler;

                var failedTaskIncidentHandler =
                    new DefaultIncidentHandler(IncidentFields.FailedTask);
                incidentHandlers[failedTaskIncidentHandler.IncidentHandlerType] =
                    failedTaskIncidentHandler;
            }
            if (customIncidentHandlers != null)
                foreach (var incidentHandler in customIncidentHandlers)
                    incidentHandlers[incidentHandler.IncidentHandlerType] = incidentHandler;
        }

        // batch ///////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init批处理Handler batchHandlers
        /// </summary>
        protected internal virtual void initBatchHandlers()
        {
            if (batchHandlers == null)
            {
                batchHandlers = new Dictionary<string, IBatchJobHandler>();

                MigrationBatchJobHandler migrationHandler = new MigrationBatchJobHandler();
                batchHandlers[migrationHandler.Type] = migrationHandler;

                DeleteProcessInstancesJobHandler deleteProcessJobHandler = new DeleteProcessInstancesJobHandler();
                batchHandlers[deleteProcessJobHandler.Type] = deleteProcessJobHandler;

                DeleteHistoricProcessInstancesJobHandler deleteHistoricProcessInstancesJobHandler = new DeleteHistoricProcessInstancesJobHandler();
                batchHandlers[deleteHistoricProcessInstancesJobHandler.Type] = deleteHistoricProcessInstancesJobHandler;

                SetJobRetriesJobHandler setJobRetriesJobHandler = new SetJobRetriesJobHandler();
                batchHandlers[setJobRetriesJobHandler.Type] = setJobRetriesJobHandler;
            }

            if (customBatchJobHandlers != null)
            {
                //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
                //ORIGINAL LINE: for (org.camunda.bpm.engine.impl.batch.BatchJobHandler<object> customBatchJobHandler : customBatchJobHandlers)
                foreach (IBatchJobHandler customBatchJobHandler in customBatchJobHandlers)
                {
                    batchHandlers[customBatchJobHandler.Type] = customBatchJobHandler;
                }
            }
        }

        /// <summary>
        ///     Init命令执行者
        /// </summary>
        protected internal virtual void InitCommandExecutors()
        {
            InitActualCommandExecutor();
            InitCommandInterceptorsTxRequired();
            InitCommandExecutorTxRequired();
            InitCommandInterceptorsTxRequiresNew();
            InitCommandExecutorTxRequiresNew();
            InitCommandExecutorDbSchemaOperations();
        }

        /// <summary>
        ///     Init实际的命令的执行者actualCommandExecutor
        /// </summary>
        protected internal virtual void InitActualCommandExecutor()
        {
            ActualCommandExecutor = new CommandExecutorImpl();
        }

        /// <summary>
        ///     Init命令拦截器commandInterceptorsTxRequires
        /// </summary>
        protected internal virtual void InitCommandInterceptorsTxRequired()
        {
            if (CommandInterceptorsTxRequired == null)
            {
                if (CustomPreCommandInterceptorsTxRequired != null)
                    CommandInterceptorsTxRequired = new List<CommandInterceptor>(CustomPreCommandInterceptorsTxRequired);
                else
                    CommandInterceptorsTxRequired = new List<CommandInterceptor>();
                ((List<CommandInterceptor>)CommandInterceptorsTxRequired).AddRange(DefaultCommandInterceptorsTxRequired);
                if (CustomPostCommandInterceptorsTxRequired != null)
                    ((List<CommandInterceptor>)CommandInterceptorsTxRequired).AddRange(
                        CustomPostCommandInterceptorsTxRequired);
                CommandInterceptorsTxRequired.Add(ActualCommandExecutor);
            }
        }

        /// <summary>
        ///     Init新的命令拦截器commandInterceptorsTxRequiresNew
        /// </summary>
        protected internal virtual void InitCommandInterceptorsTxRequiresNew()
        {
            if (CommandInterceptorsTxRequiresNew == null)
            {
                if (CustomPreCommandInterceptorsTxRequiresNew != null)
                    CommandInterceptorsTxRequiresNew =
                        new List<CommandInterceptor>(CustomPreCommandInterceptorsTxRequiresNew);
                else
                    CommandInterceptorsTxRequiresNew = new List<CommandInterceptor>();
                ((List<CommandInterceptor>)CommandInterceptorsTxRequiresNew).AddRange(
                    DefaultCommandInterceptorsTxRequiresNew);
                if (CustomPostCommandInterceptorsTxRequiresNew != null)
                    ((List<CommandInterceptor>)CommandInterceptorsTxRequiresNew).AddRange(
                        CustomPostCommandInterceptorsTxRequiresNew);
                CommandInterceptorsTxRequiresNew.Add(ActualCommandExecutor);
            }
        }

        /// <summary>
        ///     Init命令执行器 commandExecutorTxRequired
        /// </summary>
        protected internal virtual void InitCommandExecutorTxRequired()
        {
            if (CommandExecutorTxRequired == null)
                CommandExecutorTxRequired = InitInterceptorChain(CommandInterceptorsTxRequired);
        }

        /// <summary>
        ///     Init新的命令执行器commandExecutorTxRequiredNew
        /// </summary>
        protected internal virtual void InitCommandExecutorTxRequiresNew()
        {
            if (CommandExecutorTxRequiresNew == null)
                CommandExecutorTxRequiresNew = InitInterceptorChain(CommandInterceptorsTxRequiresNew);
        }

        /// <summary>
        ///     Init命令执行器DB模式操作 commandExecutorSchemaOperations
        /// </summary>
        protected internal virtual void InitCommandExecutorDbSchemaOperations()
        {
            if (commandExecutorSchemaOperations == null)
                commandExecutorSchemaOperations = CommandExecutorTxRequired;
        }

        /// <summary>
        ///     Init拦截器链 chain
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        protected internal virtual CommandInterceptor InitInterceptorChain(IList<CommandInterceptor> chain)
        {
            if ((chain == null) || (chain.Count == 0))
                throw new ProcessEngineException("invalid command interceptor chain configuration: " + chain);
            for (var i = 0; i < chain.Count - 1; i++)
                chain[i].Next = chain[i + 1];
            return chain[0];
        }

        // services /////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init服务
        /// </summary>
        protected internal virtual void InitServices()
        {
            InitService(RepositoryService);
            InitService(RuntimeService);
            InitService(HistoryService);
            InitService(IdentityService);
            InitService(TaskService);
            InitService(FormService);
            InitService(ManagementService);
            InitService(AuthorizationService);
            //initService(caseService);
            InitService(FilterService);
            InitService(ExternalTaskService);
            InitService(DecisionService);
        }

        /// <summary>
        ///     添加服务
        /// </summary>
        /// <param name="service"></param>
        protected internal virtual void InitService(object service)
        {
            if (service is ServiceImpl)
                ((ServiceImpl)service).CommandExecutor = CommandExecutorTxRequired;
            if (service is RepositoryServiceImpl)
            {
                //((RepositoryServiceImpl) service).DeploymentCharset = DefaultCharset;
            }
        }

        // DataSource ///////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init数据库
        /// </summary>
        protected internal virtual void InitDataSource()
        {
            if (string.IsNullOrEmpty(dataSource)) // == null)
            {
                if (!string.IsNullOrWhiteSpace(dataSourceJndiName))// ReferenceEquals(, null))
                {
                    try
                    {
                        //dataSource = (DataSource)(new InitialContext()).lookup(dataSourceJndiName);
                        dataSource = dataSourceJndiName;
                    }
                    catch (System.Exception e)
                    {
                        throw new ProcessEngineException("couldn't lookup datasource from " + dataSourceJndiName + ": " + e.Message, e);
                    }

                }
                else if (!string.IsNullOrWhiteSpace(JdbcUrl))//.ReferenceEquals(jdbcUrl, null))
                {
                    if ((string.ReferenceEquals(jdbcDriver, null)) || (string.ReferenceEquals(jdbcUrl, null)) || (string.ReferenceEquals(jdbcUsername, null)))
                    {
                        throw new ProcessEngineException("DataSource or JDBC properties have to be specified in a process engine configuration");
                    }
                    //TODO 拼接SQL连接字符串
                    string dataSchema = DatabaseSchema;//SYERP
                    string dataPrefix = DatabaseTablePrefix;//TB_BPM_
                    dataSource = string.Format("Data Source={0};Initial Catalog={1};User ID={2} Pwd={3}", jdbcUrl, "testBpm", JdbcUsername, JdbcPassword);
                    //dataSource=string.Format("Data source = 10.3.5.229 / orcl;Initial Catalog=testBpm; User Id = syerp; Password = syerp; Min Pool Size = 5; Max Pool Size = 500");
                    //PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, jdbcDriver, jdbcUrl, jdbcUsername, jdbcPassword);

                    //if (jdbcMaxActiveConnections > 0)
                    //{
                    //    pooledDataSource.PoolMaximumActiveConnections = jdbcMaxActiveConnections;
                    //}
                    //if (jdbcMaxIdleConnections > 0)
                    //{
                    //    pooledDataSource.PoolMaximumIdleConnections = jdbcMaxIdleConnections;
                    //}
                    //if (jdbcMaxCheckoutTime > 0)
                    //{
                    //    pooledDataSource.PoolMaximumCheckoutTime = jdbcMaxCheckoutTime;
                    //}
                    //if (jdbcMaxWaitTime > 0)
                    //{
                    //    pooledDataSource.PoolTimeToWait = jdbcMaxWaitTime;
                    //}
                    //if (jdbcPingEnabled == true)
                    //{
                    //    pooledDataSource.PoolPingEnabled = true;
                    //    if (!string.ReferenceEquals(jdbcPingQuery, null))
                    //    {
                    //        pooledDataSource.PoolPingQuery = jdbcPingQuery;
                    //    }
                    //    pooledDataSource.PoolPingConnectionsNotUsedFor = jdbcPingConnectionNotUsedFor;
                    //}
                    //dataSource = pooledDataSource;
                }

                //if (dataSource is PooledDataSource)
                //{
                //    // ACT-233: connection pool of Ibatis is not properely initialized if this is not called!
                //    ((PooledDataSource)dataSource).forceCloseAll();
                //}
            }

            if (string.ReferenceEquals(databaseType, null))
            {
                InitDatabaseType();
            }
        }

        //protected internal static Properties DefaultDatabaseTypeMappings
        //{
        // get
        // {
        //Properties databaseTypeMappings = new Properties();
        //databaseTypeMappings.setProperty("H2", "h2");
        //databaseTypeMappings.setProperty(MY_SQL_PRODUCT_NAME, "mysql");
        //databaseTypeMappings.setProperty(MARIA_DB_PRODUCT_NAME, "mariadb");
        //databaseTypeMappings.setProperty("Oracle", "oracle");
        //databaseTypeMappings.setProperty("PostgreSQL", "postgres");
        //databaseTypeMappings.setProperty("Microsoft SQL Server", "mssql");
        //databaseTypeMappings.setProperty("DB2", "db2");
        //databaseTypeMappings.setProperty("DB2", "db2");
        //databaseTypeMappings.setProperty("DB2/NT", "db2");
        //databaseTypeMappings.setProperty("DB2/NT64", "db2");
        //databaseTypeMappings.setProperty("DB2 UDP", "db2");
        //databaseTypeMappings.setProperty("DB2/LINUX", "db2");
        //databaseTypeMappings.setProperty("DB2/LINUX390", "db2");
        //databaseTypeMappings.setProperty("DB2/LINUXX8664", "db2");
        //databaseTypeMappings.setProperty("DB2/LINUXZ64", "db2");
        //databaseTypeMappings.setProperty("DB2/400 SQL", "db2");
        //databaseTypeMappings.setProperty("DB2/6000", "db2");
        //databaseTypeMappings.setProperty("DB2 UDB iSeries", "db2");
        //databaseTypeMappings.setProperty("DB2/AIX64", "db2");
        //databaseTypeMappings.setProperty("DB2/HPUX", "db2");
        //databaseTypeMappings.setProperty("DB2/HP64", "db2");
        //databaseTypeMappings.setProperty("DB2/SUN", "db2");
        //databaseTypeMappings.setProperty("DB2/SUN64", "db2");
        //databaseTypeMappings.setProperty("DB2/PTX", "db2");
        //databaseTypeMappings.setProperty("DB2/2", "db2");
        //return databaseTypeMappings;
        // }
        //}
        /// <summary>
        ///     Init数据库类型
        /// </summary>
        public virtual void InitDatabaseType()
        {

            SetDatabaseType("mssql");
            //SetDatabaseType("oracle");
            //TODO
            //Connection connection = null;
            //try
            //{
            //  connection = dataSource.Connection;
            //  DatabaseMetaData databaseMetaData = connection.MetaData;
            //  string databaseProductName = databaseMetaData.DatabaseProductName;
            //  if (MY_SQL_PRODUCT_NAME.Equals(databaseProductName))
            //  {
            //	databaseProductName = checkForMariaDb(databaseMetaData, databaseProductName);
            //  }
            //  LOG.debugDatabaseproductName(databaseProductName);
            //  databaseType = databaseTypeMappings.getProperty(databaseProductName);
            //  EnsureUtil.EnsureNotNull("couldn't deduct database type from database product name '" + databaseProductName + "'", "databaseType", databaseType);
            //  LOG.debugDatabaseType(databaseType);

            //}
            //catch (SQLException e)
            //{
            //  Console.WriteLine(e.ToString());
            //  Console.Write(e.StackTrace);
            //}
            //finally
            //{
            //  try
            //  {
            //	if (connection != null)
            //	{
            //	  connection.close();
            //	}
            //  }
            //  catch (SQLException e)
            //  {
            //	Console.WriteLine(e.ToString());
            //	Console.Write(e.StackTrace);
            //  }
            //}
        }

        //TODO
        ///// <summary>
        ///// The product name of mariadb is still 'MySQL'. This method
        ///// tries if it can find some evidence for mariadb. If it is successful
        ///// it will return "MariaDB", otherwise the provided database name.
        ///// </summary>
        //	  protected internal virtual string checkForMariaDb(DatabaseMetaData databaseMetaData, string databaseName)
        //	  {
        //		try
        //		{
        //		  string databaseProductVersion = databaseMetaData.DatabaseProductVersion;
        //		  if (!string.ReferenceEquals(databaseProductVersion, null) && databaseProductVersion.ToLower().Contains("mariadb"))
        //		  {
        //			return MARIA_DB_PRODUCT_NAME;
        //		  }
        //		}
        //		catch (SQLException)
        //		{
        //		}

        //		try
        //		{
        //		  string driverName = databaseMetaData.DriverName;
        //		  if (!string.ReferenceEquals(driverName, null) && driverName.ToLower().Contains("mariadb"))
        //		  {
        //			return MARIA_DB_PRODUCT_NAME;
        //		  }
        //		}
        //		catch (SQLException)
        //		{
        //		}

        ////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //		string metaDataClassName = databaseMetaData.GetType().FullName;
        //		if (!string.ReferenceEquals(metaDataClassName, null) && metaDataClassName.ToLower().Contains("mariadb"))
        //		{
        //		  return MARIA_DB_PRODUCT_NAME;
        //		}

        //		return databaseName;
        //	  }

        // myBatis SqlSessionFactory ////////////////////////////////////////////////
        /// <summary>
        ///     Init事务工厂 transactionFactory
        /// </summary>
        protected internal virtual void InitTransactionFactory()
        {
            //if (transactionFactory == null)
            //{
            //  if (transactionsExternallyManaged)
            //  {
            //	transactionFactory = new ManagedTransactionFactory();
            //  }
            //  else
            //  {
            //	transactionFactory = new JdbcTransactionFactory();
            //  }
            //}
        }

        public ILoggerFactory LoggerFactory { get; set; }

        /// <summary>
        /// 生命周期容器
        /// </summary>
        public IScope Scope
        {
            get { return ObjectContainer.BeginLifetimeScope(); }
        }

        /// <summary>
        ///     Init SQL会话工厂 sqlSessionFactory
        /// </summary>
        protected internal virtual void InitSqlSessionFactory()
        {

            //sqlSessionFactory = new DbSqlSessionFactory();
            //TODO Ioc管理生命周期
            //DbContext = new BpmDbContext(DataSource);
            //LoggerFactory = new EmptyLoggerFactory();
            // to protect access to cachedSqlSessionFactory see CAM-6682
            //lock (typeof(ProcessEngineConfigurationImpl))
            //{

            //  if (isUseSharedSqlSessionFactory)
            //  {
            //	sqlSessionFactory = cachedSqlSessionFactory;
            //  }

            //  if (sqlSessionFactory == null)
            //  {
            //	System.IO.Stream inputStream = null;
            //	try
            //	{
            //	  inputStream = MyBatisXmlConfigurationSteam;

            //	  // update the jdbc parameters to the configured ones...
            //	  Environment environment = new Environment("default", transactionFactory, dataSource);
            //	  Reader reader = new System.IO.StreamReader(inputStream);

            //	  Properties properties = new Properties();

            //	  if (isUseSharedSqlSessionFactory)
            //	  {
            //		properties.put("prefix", "${@org.camunda.bpm.engine.impl.context.Context@getProcessEngineConfiguration().databaseTablePrefix}");
            //	  }
            //	  else
            //	  {
            //		properties.put("prefix", databaseTablePrefix);
            //	  }

            //	  initSqlSessionFactoryProperties(properties, databaseTablePrefix, databaseType);

            //	  XMLConfigBuilder parser = new XMLConfigBuilder(reader, "", properties);
            //	  Configuration configuration = parser.Configuration;
            //	  configuration.Environment = environment;
            //	  configuration = parser.parse();

            //	  configuration.DefaultStatementTimeout = jdbcStatementTimeout;

            //	  sqlSessionFactory = new DefaultSqlSessionFactory(configuration);

            //	  if (isUseSharedSqlSessionFactory)
            //	  {
            //		cachedSqlSessionFactory = sqlSessionFactory;
            //	  }


            //	}
            //	catch (Exception e)
            //	{
            //	  throw new ProcessEngineException("Error while building ibatis SqlSessionFactory: " + e.Message, e);
            //	}
            //	finally
            //	{
            //	  IoUtil.closeSilently(inputStream);
            //	}
            //  }
            //}
        }

        // session factories ////////////////////////////////////////////////////////
        /// <summary>
        ///     Init身份提供器会话工厂 identityProviderSessionFactory
        /// </summary>
        //protected internal virtual void InitIdentityProviderSessionFactory()
        //{
        //    if (identityProviderSessionFactory == null)
        //    {
        //        //TOTO DbIdentityServiceProvider
        //        identityProviderSessionFactory = new GenericManagerFactory(typeof(DbIdentityServiceProvider));
        //    }
        //}

        /// <summary>
        ///     Init会话工厂 sessionFactories initPersistenceProviders()
        /// </summary>
        //protected internal virtual void InitSessionFactories()
        //{
        //    if (sessionFactories == null)
        //    {
        //        sessionFactories = new Dictionary<Type, ISessionFactory>();

        //        InitPersistenceProviders();

        //        //AddSessionFactory(new DbEntityManagerFactory(idGenerator));

        //        AddSessionFactory(new GenericManagerFactory(typeof(AttachmentManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(CommentManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(DeploymentManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ExecutionManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricActivityInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricCaseActivityInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricStatisticsManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricDetailManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricProcessInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricCaseInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(UserOperationLogManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricTaskInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricVariableInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricIncidentManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricIdentityLinkLogManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricJobLogManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricExternalTaskLogManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(IdentityInfoManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(IdentityLinkManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(JobManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(JobDefinitionManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ProcessDefinitionManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(PropertyManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ResourceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ByteArrayManager)));
        //        //AddSessionFactory(new GenericManagerFactory(typeof(TableDataManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(TaskManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(TaskReportManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(VariableInstanceManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(EventSubscriptionManager)));
        //        //AddSessionFactory(new GenericManagerFactory(typeof(StatisticsManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(IncidentManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(AuthorizationManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(FilterManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(MeterLogManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ExternalTaskManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(ReportManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(BatchManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(HistoricBatchManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(TenantManager)));

        //        //AddSessionFactory(new GenericManagerFactory(typeof(CaseDefinitionManager)));
        //        //AddSessionFactory(new GenericManagerFactory(typeof(CaseExecutionManager)));
        //        //AddSessionFactory(new GenericManagerFactory(typeof(CaseSentryPartManager)));

        //        AddSessionFactory(new GenericManagerFactory(typeof(DecisionDefinitionManager)));
        //        AddSessionFactory(new GenericManagerFactory(typeof(DecisionRequirementsDefinitionManager)));
        //        //AddSessionFactory(new GenericManagerFactory(typeof(HistoricDecisionInstanceManager)));

        //        sessionFactories[typeof(IReadOnlyIdentityProvider)] = identityProviderSessionFactory;
        //        //check whether identityProviderSessionFactory implements WritableIdentityProvider
        //        //sessionFactories[typeof(IUnitOfWork)] = new UnitOfWork(new BpmDbContext());

        //        var identityProviderType = identityProviderSessionFactory.SessionType;
        //        if (identityProviderType.IsSubclassOf(typeof(IWritableIdentityProvider)))
        //            sessionFactories[typeof(IWritableIdentityProvider)] = identityProviderSessionFactory;
        //    }
        //    if (customSessionFactories != null)
        //        foreach (var sessionFactory in customSessionFactories)
        //            AddSessionFactory(sessionFactory);
        //}

        /// <summary>
        ///     Init持久化提供器
        /// </summary>
        protected internal virtual void InitPersistenceProviders()
        {
            //由IScope提供

            //EnsurePrefixAndSchemaFitToegether(databaseTablePrefix, databaseSchema);
            //dbSqlSessionFactory = new DbSqlSessionFactory();
            //dbSqlSessionFactory.DatabaseType = DatabaseType;// databaseType;see 2085
            //dbSqlSessionFactory.IdGenerator = idGenerator; //暂时用guid代替
            //dbSqlSessionFactory.SqlSessionFactory = sqlSessionFactory;
            //dbSqlSessionFactory.DbIdentityUsed = IsDbIdentityUsed;
            //dbSqlSessionFactory.DbHistoryUsed = IsDbHistoryUsed;
            //dbSqlSessionFactory.CmmnEnabled = false;// cmmnEnabled;
            //dbSqlSessionFactory.DmnEnabled = dmnEnabled;
            //dbSqlSessionFactory.DatabaseTablePrefix = databaseTablePrefix;
            //dbSqlSessionFactory.DatabaseSchema = databaseSchema;
            //AddSessionFactory(dbSqlSessionFactory);
            //AddSessionFactory(new DbSqlPersistenceProviderFactory());
        }

        /// <summary>
        ///     Init迁移
        /// </summary>
        protected internal virtual void InitMigration()
        {
            InitMigrationInstructionValidators();
            InitMigrationActivityMatcher();
            InitMigrationInstructionGenerator();
            InitMigratingActivityInstanceValidators();
            InitMigratingTransitionInstanceValidators();
            InitMigratingCompensationInstanceValidators();
        }

        /// <summary>
        ///     Init 迁移活动匹配 migrationActivityMatcher
        /// </summary>
        protected internal virtual void InitMigrationActivityMatcher()
        {
            if (migrationActivityMatcher == null)
                migrationActivityMatcher = new DefaultMigrationActivityMatcher();
        }

        /// <summary>
        ///     Init 迁移指令发生器 migrationInstructionGenerator
        /// </summary>
        protected internal virtual void InitMigrationInstructionGenerator()
        {
            if (migrationInstructionGenerator == null)
                migrationInstructionGenerator = new DefaultMigrationInstructionGenerator(migrationActivityMatcher);

            IList<IMigrationActivityValidator> migrationActivityValidators = new List<IMigrationActivityValidator>();
            if (customPreMigrationActivityValidators != null)
                ((List<IMigrationActivityValidator>)migrationActivityValidators).AddRange(
                    customPreMigrationActivityValidators);
            ((List<IMigrationActivityValidator>)migrationActivityValidators).AddRange(DefaultMigrationActivityValidators);
            if (customPostMigrationActivityValidators != null)
                ((List<IMigrationActivityValidator>)migrationActivityValidators).AddRange(
                    customPostMigrationActivityValidators);
            migrationInstructionGenerator =
                migrationInstructionGenerator.MigrationActivityValidators(migrationActivityValidators)
                    .MigrationInstructionValidators(migrationInstructionValidators);
        }

        /// <summary>
        ///     Init迁移指令校验 migrationInstructionValidators
        /// </summary>
        protected internal virtual void InitMigrationInstructionValidators()
        {
            if (migrationInstructionValidators == null)
            {
                migrationInstructionValidators = new List<IMigrationInstructionValidator>();
                if (customPreMigrationInstructionValidators != null)
                    ((List<IMigrationInstructionValidator>)migrationInstructionValidators).AddRange(
                        customPreMigrationInstructionValidators);
                ((List<IMigrationInstructionValidator>)migrationInstructionValidators).AddRange(
                    DefaultMigrationInstructionValidators);
                if (customPostMigrationInstructionValidators != null)
                    ((List<IMigrationInstructionValidator>)migrationInstructionValidators).AddRange(
                        customPostMigrationInstructionValidators);
            }
        }

        /// <summary>
        ///     Init 迁移活动实例校验 migratingActivityInstanceValidators
        /// </summary>
        protected internal virtual void InitMigratingActivityInstanceValidators()
        {
            if (migratingActivityInstanceValidators == null)
            {
                migratingActivityInstanceValidators = new List<IMigratingActivityInstanceValidator>();
                if (customPreMigratingActivityInstanceValidators != null)
                    ((List<IMigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(
                        customPreMigratingActivityInstanceValidators);
                ((List<IMigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(
                    DefaultMigratingActivityInstanceValidators);
                if (customPostMigratingActivityInstanceValidators != null)
                    ((List<IMigratingActivityInstanceValidator>)migratingActivityInstanceValidators).AddRange(
                        customPostMigratingActivityInstanceValidators);
            }
        }

        /// <summary>
        ///     Init 迁移Transition实例校验 migratingTransitionInstanceValidators
        /// </summary>
        protected internal virtual void InitMigratingTransitionInstanceValidators()
        {
            if (migratingTransitionInstanceValidators == null)
            {
                migratingTransitionInstanceValidators = new List<IMigratingTransitionInstanceValidator>();
                ((List<IMigratingTransitionInstanceValidator>)migratingTransitionInstanceValidators).AddRange(
                    DefaultMigratingTransitionInstanceValidators);
            }
        }

        /// <summary>
        ///     Init迁移Compensation(补偿)实例校验 migratingCompensationInstanceValidators
        /// </summary>
        protected internal virtual void InitMigratingCompensationInstanceValidators()
        {
            if (migratingCompensationInstanceValidators == null)
            {
                migratingCompensationInstanceValidators = new List<IMigratingCompensationInstanceValidator>();

                migratingCompensationInstanceValidators.Add(new NoUnmappedLeafInstanceValidator());
                migratingCompensationInstanceValidators.Add(new NoUnmappedCompensationStartEventValidator());
            }
        }


        /// <summary>
        ///     当提供了a schema and a prefix，前缀必须是带有一个点的模式结尾
        ///     When providing a schema and a prefix  the prefix has to be the schema ending with a dot.
        /// </summary>
        protected internal virtual void EnsurePrefixAndSchemaFitToegether(string prefix, string schema)
        {
            if (ReferenceEquals(schema, null))
            {
            }
            else if (ReferenceEquals(prefix, null) ||
                (!ReferenceEquals(prefix, null) && !prefix.StartsWith(schema + ".", StringComparison.Ordinal)))
                throw new ProcessEngineException(
                    "When setting a schema the prefix has to be schema + '.'. Received schema: " + schema + " prefix: " +
                    prefix);
        }

        /// <summary>
        ///     添加会话工厂 sessionFactories
        /// </summary>
        /// <param name="sessionFactory"></param>
        //protected internal virtual void AddSessionFactory(ISessionFactory sessionFactory)
        //{
        //    sessionFactories[sessionFactory.SessionType] = sessionFactory;
        //}

        //TODO 部署deployers ////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init部署 deployers deploymentCache initCacheFactory() deploymentCache.Deployers
        /// </summary>
        protected internal virtual void InitDeployers()
        {
            if (this.deployers == null)
            {
                this.deployers = new List<IDeployer>();
                if (customPreDeployers != null)
                {
                    ((List<IDeployer>)this.deployers).AddRange(customPreDeployers);
                }
              ((List<IDeployer>)this.deployers).AddRange(DefaultDeployers);
                if (customPostDeployers != null)
                {
                    ((List<IDeployer>)this.deployers).AddRange(customPostDeployers);
                }
            }
            if (deploymentCache == null)
            {
                IList<IDeployer> deployers = new List<IDeployer>();
                if (customPreDeployers != null)
                {
                    ((List<IDeployer>)deployers).AddRange(customPreDeployers);
                }
              ((List<IDeployer>)deployers).AddRange(DefaultDeployers);
                if (customPostDeployers != null)
                {
                    ((List<IDeployer>)deployers).AddRange(customPostDeployers);
                }

                InitCacheFactory();
                deploymentCache = new DeploymentCache(cacheFactory, cacheCapacity);
                deploymentCache.SetDeployers(deployers);
            }

        }

        protected internal virtual ICollection<IDeployer> DefaultDeployers
        {
            get
            {
                IList<IDeployer> defaultDeployers = new List<IDeployer>();

                BpmnDeployer bpmnDeployer = BpmnDeployer;
                defaultDeployers.Add(bpmnDeployer);

                //if (CmmnEnabled)
                //{
                //    CmmnDeployer cmmnDeployer = CmmnDeployer;
                //    defaultDeployers.Add(cmmnDeployer);
                //}

                if (DmnEnabled)
                {

                    DecisionRequirementsDefinitionDeployer decisionRequirementsDefinitionDeployer = DecisionRequirementsDefinitionDeployer;
                    DecisionDefinitionDeployer decisionDefinitionDeployer = DecisionDefinitionDeployer;
                    //the DecisionRequirementsDefinition cacheDeployer must be before the DecisionDefinitionDeployer
                    defaultDeployers.Add(decisionRequirementsDefinitionDeployer);
                    defaultDeployers.Add(decisionDefinitionDeployer);
                }

                return defaultDeployers;
            }
        }

        protected internal virtual BpmnDeployer BpmnDeployer
        {
            get
            {
                BpmnDeployer bpmnDeployer = new BpmnDeployer();
                bpmnDeployer.ExpressionManager = expressionManager;
                bpmnDeployer.IdGenerator = IdGenerator;

                if (BpmnParseFactory == null)
                {
                    BpmnParseFactory = new DefaultBpmnParseFactory();
                }

                BpmnParser bpmnParser = new BpmnParser(expressionManager, BpmnParseFactory);

                if (preParseListeners != null)
                {
                    bpmnParser.ParseListeners.AddRange(preParseListeners);
                }
                bpmnParser.ParseListeners.AddRange(DefaultBPMNParseListeners);
                if (postParseListeners != null)
                {
                    bpmnParser.ParseListeners.AddRange(postParseListeners);
                }

                bpmnDeployer.BpmnParser = bpmnParser;

                return bpmnDeployer;
            }
        }

        protected internal virtual IList<IBpmnParseListener> DefaultBPMNParseListeners
        {
            get
            {
                IList<IBpmnParseListener> defaultListeners = new List<IBpmnParseListener>();
                if (HistoryLevelFields.HistoryLevelNone != HistoryLevel)
                {
                    defaultListeners.Add(new HistoryParseListener(HistoryLevel, HistoryEventProducer));
                }
                if (IsMetricsEnabled)
                {
                    defaultListeners.Add(new MetricsBpmnParseListener());
                }
                return defaultListeners;
            }
        }

        //protected internal virtual CmmnDeployer CmmnDeployer
        //{
        // get
        // {
        //CmmnDeployer cmmnDeployer = new CmmnDeployer();

        //cmmnDeployer.IdGenerator = idGenerator;

        //if (cmmnTransformFactory == null)
        //{
        //  cmmnTransformFactory = new DefaultCmmnTransformFactory();
        //}

        //if (cmmnElementHandlerRegistry == null)
        //{
        //  cmmnElementHandlerRegistry = new DefaultCmmnElementHandlerRegistry();
        //}

        //CmmnTransformer cmmnTransformer = new CmmnTransformer(expressionManager, cmmnElementHandlerRegistry, cmmnTransformFactory);

        //if (customPreCmmnTransformListeners != null)
        //{
        //  ((List<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(customPreCmmnTransformListeners);
        //}
        //((List<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(DefaultCmmnTransformListeners);
        //if (customPostCmmnTransformListeners != null)
        //{
        //  ((List<CmmnTransformListener>)cmmnTransformer.TransformListeners).AddRange(customPostCmmnTransformListeners);
        //}

        //cmmnDeployer.Transformer = cmmnTransformer;

        //return cmmnDeployer;
        // }
        //}

        //protected internal virtual IList<CmmnTransformListener> DefaultCmmnTransformListeners
        //{
        // get
        // {
        //IList<CmmnTransformListener> defaultListener = new List<CmmnTransformListener>();
        //if (!org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE.Equals(historyLevel))
        //{
        //  defaultListener.Add(new CmmnHistoryTransformListener(historyLevel, cmmnHistoryEventProducer));
        //}
        //if (isMetricsEnabled)
        //{
        //  defaultListener.Add(new MetricsCmmnTransformListener());
        //}
        //return defaultListener;
        // }
        //}

        protected internal virtual DecisionDefinitionDeployer DecisionDefinitionDeployer
        {
            get
            {
                DecisionDefinitionDeployer decisionDefinitionDeployer = new DecisionDefinitionDeployer();
                decisionDefinitionDeployer.IdGenerator = IdGenerator;
                decisionDefinitionDeployer.Transformer = dmnEngineConfiguration.Transformer;
                return decisionDefinitionDeployer;
            }
        }

        protected internal virtual DecisionRequirementsDefinitionDeployer DecisionRequirementsDefinitionDeployer
        {
            get
            {
                DecisionRequirementsDefinitionDeployer drdDeployer = new DecisionRequirementsDefinitionDeployer();
                drdDeployer.IdGenerator = IdGenerator;
                drdDeployer.Transformer = dmnEngineConfiguration.Transformer;
                return drdDeployer;
            }
        }





        // job executor /////////////////////////////////////////////////////////////

        /// <summary>
        ///     Init 任务执行器 jobExecutor jobHandlers TimerCatchIntermediateEventJobHandler。。。
        /// </summary>
        protected internal virtual void InitJobExecutor()
        {
            if (jobExecutor == null)
                jobExecutor = new DefaultJobExecutor();

            jobHandlers = new Dictionary<string, IJobHandler>();
            TimerExecuteNestedActivityJobHandler timerExecuteNestedActivityJobHandler = new TimerExecuteNestedActivityJobHandler();
            jobHandlers[timerExecuteNestedActivityJobHandler.Type] = timerExecuteNestedActivityJobHandler;

            TimerCatchIntermediateEventJobHandler timerCatchIntermediateEvent = new TimerCatchIntermediateEventJobHandler();
            jobHandlers[timerCatchIntermediateEvent.Type] = timerCatchIntermediateEvent;

            TimerStartEventJobHandler timerStartEvent = new TimerStartEventJobHandler();
            jobHandlers[timerStartEvent.Type] = timerStartEvent;

            TimerStartEventSubprocessJobHandler timerStartEventSubprocess = new TimerStartEventSubprocessJobHandler();
            jobHandlers[timerStartEventSubprocess.Type] = timerStartEventSubprocess;

            AsyncContinuationJobHandler asyncContinuationJobHandler = new AsyncContinuationJobHandler();
            jobHandlers[asyncContinuationJobHandler.Type] = asyncContinuationJobHandler;

            ProcessEventJobHandler processEventJobHandler = new ProcessEventJobHandler();
            jobHandlers[processEventJobHandler.Type] = processEventJobHandler;

            TimerSuspendProcessDefinitionHandler suspendProcessDefinitionHandler = new TimerSuspendProcessDefinitionHandler();
            jobHandlers[suspendProcessDefinitionHandler.Type] = suspendProcessDefinitionHandler;

            TimerActivateProcessDefinitionHandler activateProcessDefinitionHandler = new TimerActivateProcessDefinitionHandler();
            jobHandlers[activateProcessDefinitionHandler.Type] = activateProcessDefinitionHandler;

            TimerSuspendJobDefinitionHandler suspendJobDefinitionHandler = new TimerSuspendJobDefinitionHandler();
            jobHandlers[suspendJobDefinitionHandler.Type] = suspendJobDefinitionHandler;

            TimerActivateJobDefinitionHandler activateJobDefinitionHandler = new TimerActivateJobDefinitionHandler();
            jobHandlers[activateJobDefinitionHandler.Type] = activateJobDefinitionHandler;

            BatchSeedJobHandler batchSeedJobHandler = new BatchSeedJobHandler();
            jobHandlers[batchSeedJobHandler.Type] = batchSeedJobHandler;

            BatchMonitorJobHandler batchMonitorJobHandler = new BatchMonitorJobHandler();
            jobHandlers[batchMonitorJobHandler.Type] = batchMonitorJobHandler;

            foreach (IJobHandler batchHandler in batchHandlers.Values)
            {
                jobHandlers[batchHandler.Type] = batchHandler;
            }

            // if we have custom job handlers, register them
            if (customJobHandlers != null)
            {
                foreach (IJobHandler customJobHandler in customJobHandlers)
                {
                    jobHandlers[customJobHandler.Type] = customJobHandler;
                }
            }

            jobExecutor.AutoActivate = jobExecutorActivate;

            if (jobExecutor.RejectedJobsHandler == null)
            {
                if (customRejectedJobsHandler != null)
                {
                    jobExecutor.RejectedJobsHandler = customRejectedJobsHandler;
                }
                else
                {
                    jobExecutor.RejectedJobsHandler = new NotifyAcquisitionRejectedJobsHandler();
                }
            }
        }

        /// <summary>
        ///     Init Job提供器 jobPriorityProvider
        /// </summary>
        protected internal virtual void InitJobProvider()
        {
            if (ProducePrioritizedJobs && JobPriorityProvider == null)
            {
                JobPriorityProvider = new DefaultJobPriorityProvider();
            }
        }

        //外部任务external task /////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init 外部任务优先级提供者 externalTaskPriorityProvider
        /// </summary>
        protected internal virtual void InitExternalTaskPriorityProvider()
        {
            if (ProducePrioritizedExternalTasks && (ExternalTaskPriorityProvider == null))
                ExternalTaskPriorityProvider = new DefaultExternalTaskPriorityProvider();
        }

        // history //////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init 历史分级 historyLevels
        /// </summary>
        public virtual void InitHistoryLevel()
        {
            if (HistoryLevel != null)
            {
                SetHistory(HistoryLevel.Name);
            }

            if (historyLevels == null)
            {
                historyLevels = new List<IHistoryLevel>();
                historyLevels.Add(HistoryLevelFields.HistoryLevelNone);
                historyLevels.Add(HistoryLevelFields.HistoryLevelActivity);
                historyLevels.Add(HistoryLevelFields.HistoryLevelAudit);
                historyLevels.Add(HistoryLevelFields.HistoryLevelFull);
            }

            if (customHistoryLevels != null)
                ((List<IHistoryLevel>)historyLevels).AddRange(customHistoryLevels);

            //if (HistoryVariable.Equals(History, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    HistoryLevel = HistoryLevelFields.HistoryLevelActivity;
            //    Log.UsingDeprecatedHistoryLevelVariable();
            //}
            if (HistoryVariable.ToLower() == History.ToLower())
            {
                HistoryLevel = HistoryLevelFields.HistoryLevelActivity;
                Log.UsingDeprecatedHistoryLevelVariable();
            }
            else
            {
                foreach (IHistoryLevel historyLevel in historyLevels)
                {
                    if (historyLevel.Name.ToLower() == History.ToLower())
                    {
                        this.HistoryLevel = historyLevel;
                    }
                }
            }

            // do allow null for history level in case of "auto"
            if ((HistoryLevel == null) && !HistoryAuto.Equals(History, StringComparison.CurrentCultureIgnoreCase))
                throw new ProcessEngineException("invalid history level: " + History);
        }

        //TODO id生成器id generator /////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init Id生成器 idGenerator
        /// </summary>
        protected internal virtual void InitIdGenerator()
        {

            if (IdGenerator == null)
            {

                // ICommandExecutor IdGeneratorCommandExecutor //=context.Impl.Context.ProcessEngineConfiguration.CommandExecutorTxRequiresNew;// null;
                //if (IdGeneratorDataSource != null)
                //{
                //ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
                //    processEngineConfiguration.DataSource = idGeneratorDataSource;
                //    processEngineConfiguration.SetDatabaseSchemaUpdate( DB_SCHEMA_UPDATE_FALSE);
                //    processEngineConfiguration.Init();
                //IdGeneratorCommandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
                //}
                //else if (!string.ReferenceEquals(idGeneratorDataSourceJndiName, null))
                //{
                //    ProcessEngineConfigurationImpl processEngineConfiguration = new StandaloneProcessEngineConfiguration();
                //    processEngineConfiguration.DataSourceJndiName = idGeneratorDataSourceJndiName;
                //    processEngineConfiguration.DatabaseSchemaUpdate = DB_SCHEMA_UPDATE_FALSE;
                //    processEngineConfiguration.init();
                //    idGeneratorCommandExecutor = processEngineConfiguration.CommandExecutorTxRequiresNew;
                //}
                //else
                //{
                //IdGeneratorCommandExecutor = CommandExecutorTxRequiresNew;
                //}

                //DbIdGenerator dbIdGenerator = new DbIdGenerator();
                //dbIdGenerator.IdBlockSize = idBlockSize;
                ////dbIdGenerator.CommandExecutor = IdGeneratorCommandExecutor;
                //IdGenerator = dbIdGenerator;
            }
        }

        // OTHER ////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Init 命令上下文工厂 commandContextFactory commandContextFactory.ProcessEngineConfiguration = this
        /// </summary>
        protected internal virtual void InitCommandContextFactory()
        {
            if (commandContextFactory == null)
            {
                commandContextFactory = new CommandContextFactory();
                commandContextFactory.ProcessEngineConfiguration = this;
            }
        }

        /// <summary>
        ///     Init 事务上下文工厂 transactionContextFactory
        /// </summary>
        protected internal virtual void InitTransactionContextFactory()
        {
            if (transactionContextFactory == null)
                //TODO 事务工厂
                transactionContextFactory = new /*TxTransactionContextFactory();*/ StandaloneTransactionContextFactory();
        }

        /// <summary>
        ///     Init 值类型解析 ITypeValueResolver
        /// </summary>
        protected internal virtual void InitITypeValueResolver()
        {
            //if (ITypeValueResolver == null)
            //{
            //    ITypeValueResolver = new TypeValueResolverImpl();
            //}
        }

        /// <summary>
        ///     Init 默认的字符集 defaultCharset UTF-8
        /// </summary>
        protected internal virtual void InitDefaultCharset()
        {
            if (DefaultCharset == null)
            {
                if (ReferenceEquals(DefaultCharsetName, null))
                    DefaultCharsetName = "UTF-8";
                DefaultCharset = Encoding.GetEncoding(DefaultCharsetName); //.forName(defaultCharsetName);
            }
        }

        /// <summary>
        ///     Init Metrics metricsReporterIdProvider metricsRegistry dbMetricsReporter
        /// </summary>
        protected internal virtual void InitMetrics()
        {
            if (IsMetricsEnabled)
            {
                if (metricsReporterIdProvider == null)
                    metricsReporterIdProvider = new SimpleIpBasedProvider();

                if (metricsRegistry == null)
                    metricsRegistry = new MetricsRegistry();

                InitDefaultMetrics(metricsRegistry);

                if (dbMetricsReporter == null)
                    dbMetricsReporter = new DbMetricsReporter(metricsRegistry, CommandExecutorTxRequired);
            }
        }

        /// <summary>
        ///     Init 默认Metrics metricsRegistry.createMeter(...)
        /// </summary>
        /// <param name="metricsRegistry"></param>
        protected internal virtual void InitDefaultMetrics(MetricsRegistry metricsRegistry)
        {
            metricsRegistry.CreateMeter(Engine.Management.Metrics.ActivtyInstanceStart);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.ActivtyInstanceEnd);

            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobAcquisitionAttempt);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobAcquiredSuccess);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobAcquiredFailure);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobSuccessful);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobFailed);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobLockedExclusive);
            metricsRegistry.CreateMeter(Engine.Management.Metrics.JobExecutionRejected);

            metricsRegistry.CreateMeter(Engine.Management.Metrics.ExecutedDecisionElements);
        }

        

        /// <summary>
        ///     Init 表单引擎 formEngines
        /// </summary>
        protected internal virtual void InitFormEngines()
        {
            if (formEngines == null)
            {
                formEngines = new Dictionary<string, IFormEngine>();
                // html form engine = default form engine
                IFormEngine defaultFormEngine = new HtmlFormEngine();
                formEngines["null"] = defaultFormEngine; // default form engine is looked up with null
                formEngines[defaultFormEngine.Name] = defaultFormEngine;
                IFormEngine juelFormEngine = new JuelFormEngine();
                formEngines[juelFormEngine.Name] = juelFormEngine;
            }
            if (customFormEngines != null)
                foreach (var formEngine in customFormEngines)
                    formEngines[formEngine.Name] = formEngine;
        }

        /// <summary>
        ///     Init 表单类型 formTypes
        /// </summary>
        protected internal virtual void InitFormTypes()
        {
            if (formTypes == null)
            {
                formTypes = new FormTypes();
                formTypes.AddFormType(new StringFormType());
                formTypes.AddFormType(new LongFormType());
                formTypes.AddFormType(new DateFormType("dd/MM/yyyy"));
                //formTypes.addFormType(new BooleanFormType());
            }
            if (customFormTypes != null)
                foreach (var customFormType in customFormTypes)
                    formTypes.AddFormType(customFormType);
        }

        /// <summary>
        ///     Init 表单错误验证 formValidators
        /// </summary>
        protected internal virtual void InitFormFieldValidators()
        {
            if (formValidators == null)
            {
                formValidators = new FormValidators();
                formValidators.AddValidator("min", typeof(MinValidator));
                formValidators.AddValidator("max", typeof(MaxValidator));
                formValidators.AddValidator("minlength", typeof(MinLengthValidator));
                formValidators.AddValidator("maxlength", typeof(MaxLengthValidator));
                formValidators.AddValidator("required", typeof(RequiredValidator));
                formValidators.AddValidator("readonly", typeof(ReadOnlyValidator));
            }
            if (customFormFieldValidators != null)
            {
                // foreach (KeyValuePair<string, Type> validator in customFormFieldValidators)
                // {
                //formValidators.addValidator(validator.Key, validator.Value);
                // }
            }
        }

        /// <summary>
        ///     Init 脚本 resolverFactories scriptingEngines scriptFactory scriptEnvResolvers scriptingEnvironment
        /// </summary>
        protected internal virtual void InitScripting()
        {
            if (resolverFactories == null)
            {
                resolverFactories = new List<IResolverFactory>();
                resolverFactories.Add(new VariableScopeResolverFactory());
                resolverFactories.Add(new BeansResolverFactory());
            }
            if (scriptingEngines == null)
            {
                scriptingEngines = new ScriptingEngines(new ScriptBindingsFactory(resolverFactories));
                scriptingEngines.SetEnableScriptEngineCaching(enableScriptEngineCaching);
            }
            if (scriptFactory == null)
            {
                scriptFactory = new ScriptFactory();
            }
            if (scriptEnvResolvers == null)
            {
                scriptEnvResolvers = new List<IScriptEnvResolver>();
            }
            if (scriptingEnvironment == null)
            {
                scriptingEnvironment = new ScriptingEnvironment(scriptFactory, scriptEnvResolvers, scriptingEngines);
            }
        }

        /// <summary>
        ///     Init Dmn引擎 dmnEngine dmnEngineConfiguration
        /// </summary>
        protected internal virtual void InitDmnEngine()
        {
            if (dmnEngine == null)
            {
                if (dmnEngineConfiguration == null)
                    dmnEngineConfiguration = (DefaultDmnEngineConfiguration)
                        ESS.FW.Bpm.Engine.Dmn.engine.DmnEngineConfiguration.createDefaultDmnEngineConfiguration();

                dmnEngineConfiguration = new DmnEngineConfigurationBuilder(dmnEngineConfiguration)
                    .HistoryLevel(HistoryLevel)
                    .DmnHistoryEventProducer(dmnHistoryEventProducer)
                    .ScriptEngineResolver(scriptingEngines)
                    .ExpressionManager(expressionManager).Build();

                dmnEngine = dmnEngineConfiguration.buildEngine();
            }
            else if (dmnEngineConfiguration == null)
            {
                dmnEngineConfiguration = (DefaultDmnEngineConfiguration)dmnEngine.Configuration;
            }
        }

        /// <summary>
        ///     Init 表达式管理器 expressionManager
        /// </summary>
        protected internal virtual void InitExpressionManager()
        {
            if (expressionManager == null)
                expressionManager = new ExpressionManager(beans);

            // add function mapper for command context (eg currentUser(), currentUserGroups())
            expressionManager.AddFunctionMapper(new CommandContextFunctionMapper());
            // add function mapper for date time (eg now(), dateTime())
            expressionManager.AddFunctionMapper(new DateTimeFunctionMapper());
            expressionManager.AddFunctionMapper(new StringFunctionMapper());
        }

        /// <summary>
        ///     Init 企业日历管理器 businessCalendarManager mapBusinessCalendarManager
        /// </summary>
        protected internal virtual void InitBusinessCalendarManager()
        {
            if (businessCalendarManager == null)
            {
                var mapBusinessCalendarManager = new MapBusinessCalendarManager();
                mapBusinessCalendarManager.AddBusinessCalendar(DurationBusinessCalendar.Name,
                    new DurationBusinessCalendar());
                mapBusinessCalendarManager.AddBusinessCalendar(DueDateBusinessCalendar.Name,
                    new DueDateBusinessCalendar());
                mapBusinessCalendarManager.AddBusinessCalendar(CycleBusinessCalendar.Name, new CycleBusinessCalendar());

                businessCalendarManager = mapBusinessCalendarManager;
            }
        }

        /// <summary>
        ///     Init 委托拦截器 delegateInterceptor
        /// </summary>
        protected internal virtual void InitDelegateInterceptor()
        {
            if (delegateInterceptor == null)
                delegateInterceptor = new DefaultDelegateInterceptor();
        }

        /// <summary>
        ///     Init 事件处理器 eventHandlers signalEventHander compensationEventHandler conditionalEventHandler
        /// </summary>
        protected internal virtual void initEventHandlers()
        {
            if (eventHandlers == null)
            {
                eventHandlers = new Dictionary<string, IEventHandler>();

                var signalEventHander = new SignalEventHandler();
                eventHandlers[signalEventHander.EventHandlerType] = signalEventHander;

                var compensationEventHandler = new CompensationEventHandler();
                eventHandlers[compensationEventHandler.EventHandlerType] = compensationEventHandler;

                IEventHandler messageEventHandler = new EventHandlerImpl(EventType.Message);
                eventHandlers[messageEventHandler.EventHandlerType] = messageEventHandler;

                IEventHandler conditionalEventHandler = new ConditionalEventHandler();
                eventHandlers[conditionalEventHandler.EventHandlerType] = conditionalEventHandler;
            }
            if (customEventHandlers != null)
                foreach (var eventHandler in customEventHandlers)
                    eventHandlers[eventHandler.EventHandlerType] = eventHandler;
        }

        /// <summary>
        ///     Init 命令检查 commandCheckers
        /// </summary>
        protected internal virtual void InitCommandCheckers()
        {
            if (commandCheckers == null)
            {
                commandCheckers = new List<ICommandChecker>();

                // add the default command checkers
                commandCheckers.Add(new TenantCommandChecker());
                commandCheckers.Add(new AuthorizationCommandChecker());
            }
        }

        //// JPA //////////////////////////////////////////////////////////////////////
        ////JPA(Java Persistence API)是Sun官方提出的Java持久化规范。它为Java开发人员提供了一种对象/关系映射工具来管理Java应用中的关系数据。，而Hibernate是它的一种实现。除了Hibernate，还有EclipseLink(曾经的toplink)，OpenJPA等可供选择，所以使用Jpa的一个好处是，可以更换实现而不必改动太多代码。
        //protected internal virtual void InitJpa()
        //{
        //    if (!ReferenceEquals(jpaPersistenceUnitName, null))
        //    {
        //        //jpaEntityManagerFactory = JpaHelper.createEntityManagerFactory(jpaPersistenceUnitName);
        //    }
        //    if (jpaEntityManagerFactory != null)
        //    {
        //        //sessionFactories[typeof(EntityManagerSession)] = new EntityManagerSessionFactory(jpaEntityManagerFactory, jpaHandleTransaction, jpaCloseEntityManager);
        //        //JPAVariableSerializer jpaType = (JPAVariableSerializer)variableSerializers.getSerializerByName(JPAVariableSerializer.NAME);
        //        // Add JPA-type
        //        //if (jpaType == null)
        //        //{
        //        //    We try adding the variable right after byte serializer, if available
        //        //   int serializableIndex = variableSerializers.getSerializerIndexByName(ITypedValue.BYTES.Name);
        //        //    if (serializableIndex > -1)
        //        //    {
        //        //        variableSerializers.addSerializer(new JPAVariableSerializer(), serializableIndex);
        //        //    }
        //        //    else
        //        //    {
        //        //        variableSerializers.addSerializer(new JPAVariableSerializer());
        //        //    }
        //        //    }
        //    }
        //}

        /// <summary>
        ///     Init 组件？beans Dictionary<object, object>
        /// </summary>
        protected internal virtual void InitBeans()
        {
            if (beans == null)
                beans = new Dictionary<object, object>();
        }

        /// <summary>
        ///     Init Artifact工厂 artifactFactory
        /// </summary>
        protected internal virtual void InitArtifactFactory()
        {
            if (artifactFactory == null)
                artifactFactory = new DefaultArtifactFactory();
        }

        /// <summary>
        ///     Init Process应用管理器 processApplicationManager
        /// </summary>
        protected internal virtual void InitProcessApplicationManager()
        {
            if (processApplicationManager == null)
                processApplicationManager = new ProcessApplicationManager();
        }

        // correlation handler //////////////////////////////////////////////////////
        /// <summary>
        ///     Init 相关的处理程序 correlationHandler
        /// </summary>
        protected internal virtual void initCorrelationHandler()
        {
            if (correlationHandler == null)
                correlationHandler = new DefaultCorrelationHandler();
        }

        // history handlers /////////////////////////////////////////////////////
        /// <summary>
        ///     Init 历史事件产生器 historyEventProducer
        /// </summary>
        protected internal virtual void InitHistoryEventProducer()
        {
            if (HistoryEventProducer == null)
                HistoryEventProducer = new CacheAwareHistoryEventProducer();
        }

        /// <summary>
        ///     Init Cmmn(代理)历史事件产生器(Cmmn commission) cmmnHistoryEventProducer
        /// </summary>
        protected internal virtual void InitCmmnHistoryEventProducer()
        {
            if (cmmnHistoryEventProducer == null)
            {
                //cmmnHistoryEventProducer = new CacheAwareCmmnHistoryEventProducer();
            }
        }

        /// <summary>
        ///     Init Dmn历史事件产生器 dmnHistoryEventProducer
        /// </summary>
        protected internal virtual void InitDmnHistoryEventProducer()
        {
            if (dmnHistoryEventProducer == null)
                dmnHistoryEventProducer = new DefaultDmnHistoryEventProducer();
        }

        /// <summary>
        ///     Init 历史事件处理程序 historyEventHandler
        /// </summary>
        protected internal virtual void InitHistoryEventHandler()
        {
            if (historyEventHandler == null)
                historyEventHandler = new DbHistoryEventHandler();
        }

        // password digest //////////////////////////////////////////////////////////
        /// <summary>
        ///     Init 密码摘要? saltGenerator passwordEncryptor customPasswordChecker passwordManager
        /// </summary>
        protected internal virtual void InitPasswordDigest()
        {
            if (saltGenerator == null)
                saltGenerator = new Default16ByteSaltGenerator();
            if (passwordEncryptor == null)
                passwordEncryptor = new Sha512HashDigest();
            if (customPasswordChecker == null)
                customPasswordChecker = new List<IPasswordEncryptor>();
            if (PasswordManager == null)
                PasswordManager = new PasswordManager(passwordEncryptor, customPasswordChecker);
        }

        /// <summary>
        ///     Init 部署登记 registeredDeployments
        /// </summary>
        protected internal virtual void InitDeploymentRegistration()
        {
            if (registeredDeployments == null)
            {
                registeredDeployments = new HashSet<string>();
            }
        }

        // cache factory //////////////////////////////////////////////////////////
        /// <summary>
        ///     Init 缓存工厂 cacheFactory DefaultCacheFactory
        /// </summary>
        protected internal virtual void InitCacheFactory()
        {
            if (cacheFactory == null)
            {
                cacheFactory = new DefaultCacheFactory();
            }
        }

        //resource authorization provider //////////////////////////////////////////
        /// <summary>
        ///     Init 资源授权提供器 resourceAuthorizationProvider
        /// </summary>
        protected internal virtual void InitResourceAuthorizationProvider()
        {
            if (ResourceAuthorizationProvider == null)
            {
                //resourceAuthorizationProvider = new DefaultAuthorizationProvider();
            }
        }

        /// <summary>
        ///     Init 任务的默认用户权限 defaultUserPermissionForTask
        /// </summary>
        protected internal virtual void InitDefaultUserPermissionForTask()
        {
            if (defaultUserPermissionForTask == null)
            {
                if (Permissions.Update.ToString().Equals(DefaultUserPermissionNameForTask))
                {
                    defaultUserPermissionForTask = Permissions.Update;
                }
                else if (Permissions.TaskWork.ToString().Equals(DefaultUserPermissionNameForTask))
                {
                    defaultUserPermissionForTask = Permissions.TaskWork;
                }
                else
                {
                    throw Log.InvalidConfigDefaultUserPermissionNameForTask(DefaultUserPermissionNameForTask, new string[] { Permissions.Update.ToString(), Permissions.TaskWork.ToString() });
                }
            }
        }

        #endregion

        #region Public virtual Set...

        public virtual ProcessEngineConfigurationImpl SetCustomPreCommandInterceptorsTxRequired(
            IList<CommandInterceptor> customPreCommandInterceptorsTxRequired)
        {
            CustomPreCommandInterceptorsTxRequired = customPreCommandInterceptorsTxRequired;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCustomPostCommandInterceptorsTxRequired(
            IList<CommandInterceptor> customPostCommandInterceptorsTxRequired)
        {
            CustomPostCommandInterceptorsTxRequired = customPostCommandInterceptorsTxRequired;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCommandInterceptorsTxRequired(
            IList<CommandInterceptor> commandInterceptorsTxRequired)
        {
            CommandInterceptorsTxRequired = commandInterceptorsTxRequired;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCommandExecutorTxRequired(
            ICommandExecutor commandExecutorTxRequired)
        {
            CommandExecutorTxRequired = commandExecutorTxRequired;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCustomPreCommandInterceptorsTxRequiresNew(
            IList<CommandInterceptor> customPreCommandInterceptorsTxRequiresNew)
        {
            CustomPreCommandInterceptorsTxRequiresNew = customPreCommandInterceptorsTxRequiresNew;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCustomPostCommandInterceptorsTxRequiresNew(
            IList<CommandInterceptor> customPostCommandInterceptorsTxRequiresNew)
        {
            CustomPostCommandInterceptorsTxRequiresNew = customPostCommandInterceptorsTxRequiresNew;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCommandInterceptorsTxRequiresNew(
            IList<CommandInterceptor> commandInterceptorsTxRequiresNew)
        {
            CommandInterceptorsTxRequiresNew = commandInterceptorsTxRequiresNew;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCommandExecutorTxRequiresNew(
            ICommandExecutor commandExecutorTxRequiresNew)
        {
            CommandExecutorTxRequiresNew = commandExecutorTxRequiresNew;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetRepositoryService(IRepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetRuntimeService(IRuntimeService runtimeService)
        {
            RuntimeService = runtimeService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetHistoryService(IHistoryService historyService)
        {
            HistoryService = historyService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetIdentityService(IIdentityService identityService)
        {
            IdentityService = identityService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetTaskService(ITaskService taskService)
        {
            TaskService = taskService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetFormService(IFormService formService)
        {
            FormService = formService;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetManagementService(IManagementService managementService)
        {
            ManagementService = managementService;
            return this;
        }

        //public virtual ProcessEngineConfigurationImpl SetSessionFactories(
        //    IDictionary<Type, ISessionFactory> sessionFactories)
        //{
        //    this.sessionFactories = sessionFactories;
        //    return this;
        //}

        public virtual ProcessEngineConfigurationImpl SetJobExecutor(JobExecutor.JobExecutor jobExecutor)
        {
            this.jobExecutor = jobExecutor;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetIdGenerator(IDGenerator idGenerator)
        {
            this.IdGenerator = idGenerator;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetWsSyncFactoryClassName(string wsSyncFactoryClassName)
        {
            this.wsSyncFactoryClassName = wsSyncFactoryClassName;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetFormEngines(IDictionary<string, IFormEngine> formEngines)
        {
            this.formEngines = formEngines;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetFormTypes(FormTypes formTypes)
        {
            this.formTypes = formTypes;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetExpressionManager(ExpressionManager expressionManager)
        {
            this.expressionManager = expressionManager;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetBusinessCalendarManager(
            IBusinessCalendarManager businessCalendarManager)
        {
            this.businessCalendarManager = businessCalendarManager;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCommandContextFactory(
            CommandContextFactory commandContextFactory)
        {
            this.commandContextFactory = commandContextFactory;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetTransactionContextFactory(
            ITransactionContextFactory transactionContextFactory)
        {
            this.transactionContextFactory = transactionContextFactory;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDefaultUserPermissionForTask(
            Permissions defaultUserPermissionForTask)
        {
            this.defaultUserPermissionForTask = defaultUserPermissionForTask;
            return this;
        }

        //public virtual ProcessEngineConfigurationImpl SetDbSqlSessionFactory(DbSqlSessionFactory dbSqlSessionFactory)
        //{
        //    this.dbSqlSessionFactory = dbSqlSessionFactory;
        //    return this;
        //}

        //public virtual ProcessEngineConfigurationImpl SetCustomSessionFactories(
        //    IList<ISessionFactory> customSessionFactories)
        //{
        //    this.customSessionFactories = customSessionFactories;
        //    return this;
        //}

        public virtual ProcessEngineConfigurationImpl SetCustomFormEngines(IList<IFormEngine> customFormEngines)
        {
            this.customFormEngines = customFormEngines;
            return this;
        }


        public virtual ProcessEngineConfigurationImpl SetCustomFormTypes(IList<AbstractFormFieldType> customFormTypes)
        {
            this.customFormTypes = customFormTypes;
            return this;
        }


        //public virtual IList<ResolverFactory> ResolverFactories
        //{
        // get
        // {
        //return resolverFactories;
        // }
        // set
        // {
        //this.resolverFactories = value;
        // }
        //}


        public virtual DeploymentCache DeploymentCache
        {
            get
            {
                return deploymentCache;
            }
            set
            {
                this.deploymentCache = value;
            }
        }


        public virtual ProcessEngineConfigurationImpl SetDelegateInterceptor(IDelegateInterceptor delegateInterceptor)
        {
            this.delegateInterceptor = delegateInterceptor;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl setCustomRejectedJobsHandler(
            IRejectedJobsHandler customRejectedJobsHandler)
        {
            this.customRejectedJobsHandler = customRejectedJobsHandler;
            return this;
        }

        public virtual IEventHandler getEventHandler(string eventType)
        {
            return eventHandlers[eventType];
        }

        public virtual ProcessEngineConfigurationImpl SetFailedJobCommandFactory(
            IFailedJobCommandFactory failedJobCommandFactory)
        {
            this.failedJobCommandFactory = failedJobCommandFactory;
            return this;
        }

        /// <summary>
        ///     Allows configuring a database table prefix which is used for all runtime operations of the process engine.
        ///     For example, if you specify a prefix named 'PRE1.', activiti will query for executions in a table named
        ///     'PRE1.ACT_RU_EXECUTION_'.
        ///     <para>
        ///         <p />
        ///         <strong>
        ///             NOTE: the prefix is not respected by automatic database schema management. If you use
        ///             <seealso cref="ProcessEngineConfiguration#DB_SCHEMA_UPDATE_CREATE_DROP" />
        ///             or <seealso cref="ProcessEngineConfiguration#DB_SCHEMA_UPDATE_TRUE" />, activiti will create the database
        ///             tables
        ///             using the default names, regardless of the prefix configured here.
        ///         </strong>
        ///         @since 5.9
        ///     </para>
        /// </summary>
        public virtual ProcessEngineConfiguration SetDatabaseTablePrefix(string databaseTablePrefix)
        {
            this.databaseTablePrefix = databaseTablePrefix;
            return this;
        }

        public virtual ProcessEngineConfiguration SetCreateDiagramOnDeploy(bool createDiagramOnDeploy)
        {
            IsCreateDiagramOnDeploy = createDiagramOnDeploy;
            return this;
        }


        public virtual ProcessEngineConfigurationImpl setHistoryEventHandler(IHistoryEventHandler historyEventHandler)
        {
            this.historyEventHandler = historyEventHandler;
            return this;
        }

        public virtual IIncidentHandler getIncidentHandler(string incidentType)
        {
            return incidentHandlers[incidentType];
        }


        public virtual ProcessEngineConfigurationImpl SetHistoryEventProducer(IHistoryEventProducer historyEventProducer)
        {
            HistoryEventProducer = historyEventProducer;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCmmnHistoryEventProducer(
            ICmmnHistoryEventProducer cmmnHistoryEventProducer)
        {
            this.cmmnHistoryEventProducer = cmmnHistoryEventProducer;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDmnHistoryEventProducer(
            IDmnHistoryEventProducer dmnHistoryEventProducer)
        {
            this.dmnHistoryEventProducer = dmnHistoryEventProducer;
            return this;
        }


        public virtual ScriptFactory ScriptFactory
        {
            get
            {
                return scriptFactory;
            }
            set
            {
                this.scriptFactory = value;
            }
        }

        public virtual ScriptingEnvironment ScriptingEnvironment
        {
            get
            {
                return scriptingEnvironment;
            }
            set
            {
                this.scriptingEnvironment = value;
            }
        }


        public virtual IList<IScriptEnvResolver> EnvScriptResolvers
        {
            get
            {
                return scriptEnvResolvers;
            }
            set
            {
                this.scriptEnvResolvers = value;
            }
        }


        public virtual ProcessEngineConfiguration SetArtifactFactory(IArtifactFactory artifactFactory)
        {
            this.artifactFactory = artifactFactory;
            return this;
        }

        public virtual string DefaultSerializationFormat
        {
            get
            {
                return defaultSerializationFormat;
            }
        }

        public DateTime HistoryCleanupBatchWindowEndTimeAsDate { get; set; }
        public string HistoryCleanupBatchWindowEndTime { get; set; }
        public int HistoryCleanupBatchThreshold { get; set; }
        public int HistoryCleanupBatchSize { get; set; }
        public DateTime HistoryCleanupBatchWindowStartTimeAsDate { get; set; }
        public string HistoryCleanupBatchWindowStartTime { get; set; }
        public bool HistoryCleanupMetricsEnabled { get; set; }

        public virtual string GetDefaultSerializationFormat()
        {
            return defaultSerializationFormat;
        }
        public virtual ProcessEngineConfigurationImpl SetDefaultSerializationFormat(string defaultSerializationFormat)
        {
            this.defaultSerializationFormat = defaultSerializationFormat;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDefaultCharsetName(string defaultCharsetName)
        {
            this.DefaultCharsetName = defaultCharsetName;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDbEntityCacheReuseEnabled(bool isDbEntityCacheReuseEnabled)
        {
            this.IsDbEntityCacheReuseEnabled = isDbEntityCacheReuseEnabled;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDbEntityCacheKeyMapping(
            DbEntityCacheKeyMapping dbEntityCacheKeyMapping)
        {
            this.dbEntityCacheKeyMapping = dbEntityCacheKeyMapping;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetCustomHistoryLevels(IList<IHistoryLevel> customHistoryLevels)
        {
            this.customHistoryLevels = customHistoryLevels;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetInvokeCustomVariableListeners(
            bool isInvokeCustomVariableListeners)
        {
            this.IsInvokeCustomVariableListeners = isInvokeCustomVariableListeners;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetMetricsRegistry(MetricsRegistry metricsRegistry)
        {
            this.metricsRegistry = metricsRegistry;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetMetricsEnabled(bool isMetricsEnabled)
        {
            this.IsMetricsEnabled = isMetricsEnabled;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDbMetricsReporter(DbMetricsReporter dbMetricsReporter)
        {
            this.dbMetricsReporter = dbMetricsReporter;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetDbMetricsReporterActivate(bool isDbMetricsReporterEnabled)
        {
            IsDbMetricsReporterActivate = isDbMetricsReporterEnabled;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetEnableScriptEngineCaching(bool enableScriptEngineCaching)
        {
            this.enableScriptEngineCaching = enableScriptEngineCaching;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetEnableFetchScriptEngineFromProcessApplication(bool enable)
        {
            enableFetchScriptEngineFromProcessApplication = enable;
            return this;
        }


        public virtual ProcessEngineConfigurationImpl SetBpmnStacktraceVerbose(bool isBpmnStacktraceVerbose)
        {
            this.IsBpmnStacktraceVerbose = isBpmnStacktraceVerbose;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetForceCloseMybatisConnectionPool(
            bool forceCloseMybatisConnectionPool)
        {
            this.forceCloseMybatisConnectionPool = forceCloseMybatisConnectionPool;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetRestrictUserOperationLogToAuthenticatedUsers(
            bool restrictUserOperationLogToAuthenticatedUsers)
        {
            this.restrictUserOperationLogToAuthenticatedUsers = restrictUserOperationLogToAuthenticatedUsers;
            return this;
        }

        public virtual ProcessEngineConfigurationImpl SetTenantIdProvider(ITenantIdProvider tenantIdProvider)
        {
            this.tenantIdProvider = tenantIdProvider;
            return this;
        }


        public virtual ProcessEngineConfigurationImpl SetUseSharedSqlSessionFactory(bool isUseSharedSqlSessionFactory)
        {
            this.IsUseSharedSqlSessionFactory = isUseSharedSqlSessionFactory;
            return this;
        }

        internal ICommandExecutor GetCommandExecutorTxRequired()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}