using System;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine
{


    /// <summary>
    /// Configuration information from which a process engine can be build.
    /// 
    /// <para>Most common is to create a process engine based on the default configuration file:
    /// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
    ///   .createProcessEngineConfigurationFromResourceDefault()
    ///   .buildProcessEngine();
    /// </pre>
    /// </para>
    /// 
    /// <para>To create a process engine programatic, without a configuration file,
    /// the first option is <seealso cref="#createStandaloneProcessEngineConfiguration()"/>
    /// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
    ///   .createStandaloneProcessEngineConfiguration()
    ///   .buildProcessEngine();
    /// </pre>
    /// This creates a new process engine with all the defaults to connect to
    /// a remote h2 database (jdbc:h2:tcp://localhost/activiti) in standalone
    /// mode.  Standalone mode means that Activiti will manage the transactions
    /// on the JDBC connections that it creates.  One transaction per
    /// service method.
    /// For a description of how to write the configuration files, see the
    /// userguide.
    /// </para>
    /// 
    /// <para>The second option is great for testing: <seealso cref="#createStandalonInMemeProcessEngineConfiguration()"/>
    /// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
    ///   .createStandaloneInMemProcessEngineConfiguration()
    ///   .buildProcessEngine();
    /// </pre>
    /// This creates a new process engine with all the defaults to connect to
    /// an memory h2 database (jdbc:h2:tcp://localhost/activiti) in standalone
    /// mode.  The DB schema strategy default is in this case <code>create-drop</code>.
    /// Standalone mode means that Activiti will manage the transactions
    /// on the JDBC connections that it creates.  One transaction per
    /// service method.
    /// </para>
    /// 
    /// <para>On all forms of creating a process engine, you can first customize the configuration
    /// before calling the <seealso cref="#buildProcessEngine()"/> method by calling any of the
    /// setters like this:
    /// <pre>ProcessEngine processEngine = ProcessEngineConfiguration
    ///   .createProcessEngineConfigurationFromResourceDefault()
    ///   .setMailServerHost("gmail.com")
    ///   .setJdbcUsername("mickey")
    ///   .setJdbcPassword("mouse")
    ///   .buildProcessEngine();
    /// </pre>
    /// </para>
    /// </summary>
    /// <seealso cref= ProcessEngines></seealso>
    public abstract class ProcessEngineConfiguration
    {

        /// <summary>
        /// Checks the version of the DB schema against the library when
        /// the process engine is being created and throws an exception
        /// if the versions don't match.
        /// </summary>
        public const string DbSchemaUpdateFalse = "false";

        /// <summary>
        /// Creates the schema when the process engine is being created and
        /// drops the schema when the process engine is being closed.
        /// </summary>
        public const string DbSchemaUpdateCreateDrop = "create-drop";

        /// <summary>
        /// Upon building of the process engine, a check is performed and
        /// an update of the schema is performed if it is necessary.
        /// </summary>
        public const string DbSchemaUpdateTrue = "true";

        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/> to ensure that no history is being recorded.
        /// </summary>
        public const string HistoryNone = "none";
        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances and
        /// historic activity instances are being recorded.
        /// This means no details for those entities.
        /// </summary>
        public const string HistoryActivity = "activity";
        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances,
        /// historic activity instances and last process variable values are being recorded.
        /// <para><strong>NOTE:</strong> This history level has been deprecated. Use level <seealso cref="#HISTORY_ACTIVITY"/> instead.</para>
        /// </summary>
        [Obsolete]
        public const string HistoryVariable = "variable";
        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/> to ensure that only historic process instances,
        /// historic activity instances and submitted form property values are being recorded.
        /// </summary>
        public const string HistoryAudit = "audit";
        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/> to ensure that all historic information is
        /// being recorded, including the variable updates.
        /// </summary>
        public const string HistoryFull = "full";

        /// <summary>
        /// Value for <seealso cref="#setHistory(String)"/>. Choosing auto causes the configuration to choose the level
        /// already present on the database. If none can be found, "audit" is taken.
        /// </summary>
        public const string HistoryAuto = "auto";

        /// <summary>
        /// The default history level that is used when no history level is configured
        /// </summary>
        public const string HistoryDefault = HistoryAudit;

        /// <summary>
        /// Always enables check for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations.
        /// This mode is equal to the &lt; 7.5 behavior.
        /// <p />
        /// *NOTE:* Checking revoke authorizations is very expensive for resources with a high potential
        /// cardinality like tasks or process instances and can render authorized access to the process engine
        /// effectively unusable on most databases. You are therefore strongly discouraged from using this mode.
        /// 
        /// </summary>
        public const string AuthorizationCheckRevokeAlways = "always";

        /// <summary>
        /// Never checks for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations. This mode
        /// has best performance effectively disables the use of <seealso cref="Authorization#AUTH_TYPE_REVOKE"/>.
        /// *Note*: It is strongly recommended to use this mode.
        /// </summary>
        public const string AuthorizationCheckRevokeNever = "never";

        /// <summary>
        /// This mode only checks for <seealso cref="Authorization#AUTH_TYPE_REVOKE revoke"/> authorizations if at least
        /// one revoke authorization currently exits for the current user or one of the groups the user is a member
        /// of. To achieve this it is checked once per command whether potentially applicable revoke authorizations
        /// exist. Based on the outcome, the authorization check then uses revoke or not.
        /// <p />
        /// *NOTE:* Checking revoke authorizations is very expensive for resources with a high potential
        /// cardinality like tasks or process instances and can render authorized access to the process engine
        /// effectively unusable on most databases.
        /// </summary>
        public const string AuthorizationCheckRevokeAuto = "auto";

        protected internal int idBlockSize = 100;
        protected internal bool jobExecutorActivate = false;
        protected internal bool jobExecutorDeploymentAware = false;
        protected internal bool jobExecutorPreferTimerJobs = false;
        protected internal bool jobExecutorAcquireByDueDate = false;

        /// <summary>
        /// The flag will be used inside the method "JobManager#send()". It will be used to decide whether to notify the
        /// job executor that a new job has been created. It will be used for performance improvement, so that the new job could
        /// be executed in some situations immediately.
        /// </summary>
        protected internal bool hintJobExecutor = true;

        protected internal string mailServerHost = "localhost";
        protected internal string mailServerUsername; // by default no name and password are provided, which
        protected internal string mailServerPassword; // means no authentication for mail server
        protected internal int mailServerPort = 25;
        protected internal bool UseTls = false;
        protected internal string mailServerDefaultFrom = "camunda@localhost";

        protected internal string databaseType;
        protected internal string databaseSchemaUpdate = DbSchemaUpdateFalse;
        protected internal string jdbcDriver = "org.h2.Driver";
        protected internal string jdbcUrl = "jdbc:h2:tcp://localhost/activiti";
        protected internal string jdbcUsername = "sa";
        protected internal string jdbcPassword = "";
        protected internal string dataSourceJndiName = null;
        protected internal int jdbcMaxActiveConnections;
        protected internal int jdbcMaxIdleConnections;
        protected internal int jdbcMaxCheckoutTime;
        protected internal int jdbcMaxWaitTime;
        protected internal bool jdbcPingEnabled = false;
        protected internal string jdbcPingQuery = null;
        protected internal int jdbcPingConnectionNotUsedFor;
        protected internal string dataSource;
        protected internal ISchemaOperationsCommand schemaOperationsCommand = new SchemaOperationsProcessEngineBuild();
        protected internal bool transactionsExternallyManaged = false;
        /// <summary>
        /// the number of seconds the jdbc driver will wait for a response from the database </summary>
        protected internal int? jdbcStatementTimeout;

        //protected internal ClassLoader classLoader;

        protected internal bool createIncidentOnFailedJobEnabled = true;

        /// <summary>
        /// <para>The following flag <code>authorizationEnabledForCustomCode</code> will
        /// only be taken into account iff <code>authorizationEnabled</code> is set
        /// <code>true</code>.</para>
        /// 
        /// <para>If the value of the flag <code>authorizationEnabledForCustomCode</code>
        /// is set <code>true</code> then an authorization check will be performed by
        /// executing commands inside custom code (e.g. inside <seealso cref="JavaDelegate"/>).</para>
        /// 
        /// <para>The default value is <code>false</code>.</para>
        /// 
        /// </summary>
        protected internal bool authorizationEnabledForCustomCode = false;

        /// <summary>
        /// If the value of this flag is set <code>true</code> then the process engine
        /// performs tenant checks to ensure that an authenticated user can only access
        /// data that belongs to one of his tenants.
        /// </summary>
        protected internal bool tenantCheckEnabled = true;

        protected internal IValueTypeResolver valueTypeResolver;

        /// <summary>
        /// use one of the static createXxxx methods instead </summary>
        protected internal ProcessEngineConfiguration()
        {
        }

        public abstract IProcessEngine BuildProcessEngine();

        public static ProcessEngineConfiguration CreateProcessEngineConfigurationFromResourceDefault()
        {
            ProcessEngineConfiguration processEngineConfiguration = null;
            try
            {
                processEngineConfiguration = CreateProcessEngineConfigurationFromResource("camunda.cfg.xml", "processEngineConfiguration");
            }
            catch (System.Exception)
            {
                processEngineConfiguration = CreateProcessEngineConfigurationFromResource("activiti.cfg.xml", "processEngineConfiguration");
            }
            return processEngineConfiguration;
        }

        public static ProcessEngineConfiguration CreateProcessEngineConfigurationFromResource(string resource)
        {
            return CreateProcessEngineConfigurationFromResource(resource, "processEngineConfiguration");
        }

        public static ProcessEngineConfiguration CreateProcessEngineConfigurationFromResource(string resource, string beanName)
        {
            return BeansConfigurationHelper.ParseProcessEngineConfigurationFromResource(resource, beanName);
        }

        public static ProcessEngineConfiguration CreateProcessEngineConfigurationFromInputStream(System.IO.Stream inputStream)
        {
            return CreateProcessEngineConfigurationFromInputStream(inputStream, "processEngineConfiguration");
        }

        public static ProcessEngineConfiguration CreateProcessEngineConfigurationFromInputStream(System.IO.Stream inputStream, string beanName)
        {
            return BeansConfigurationHelper.ParseProcessEngineConfigurationFromInputStream(inputStream, beanName);
        }

        public static ProcessEngineConfiguration CreateStandaloneProcessEngineConfiguration()
        {
            return new StandaloneProcessEngineConfiguration();
        }

        public static ProcessEngineConfiguration CreateStandaloneInMemProcessEngineConfiguration()
        {
            return new StandaloneInMemProcessEngineConfiguration();
        }
        public static ProcessEngineConfiguration CreateTxProcessEngineConfiguration()
        {
            return new TxProcessEngineConfiguration();
        }
        // TODO add later when we have test coverage for this
        //  public static ProcessEngineConfiguration createJtaProcessEngineConfiguration() {
        //    return new JtaProcessEngineConfiguration();
        //  }


        // getters and setters //////////////////////////////////////////////////////

        public virtual string ProcessEngineName { get; set; } = ProcessEngines.NameDefault;

        public virtual ProcessEngineConfiguration SetProcessEngineName(string processEngineName)
        {
            this.ProcessEngineName = processEngineName;
            return this;
        }

        public virtual int IdBlockSize
        {
            get
            {
                return idBlockSize;
            }
        }

        public virtual ProcessEngineConfiguration SetIdBlockSize(int idBlockSize)
        {
            this.idBlockSize = idBlockSize;
            return this;
        }

        public virtual string History { get; set; } = HistoryDefault;
        public virtual IHistoryLevel HistoryLevel { get; set; } = HistoryLevelFields.HistoryLevelAudit;

        public virtual ProcessEngineConfiguration SetHistory(string history)
        {
            this.History = history;
            return this;
        }

        public virtual string MailServerHost
        {
            get
            {
                return mailServerHost;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerHost(string mailServerHost)
        {
            this.mailServerHost = mailServerHost;
            return this;
        }

        public virtual string MailServerUsername
        {
            get
            {
                return mailServerUsername;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerUsername(string mailServerUsername)
        {
            this.mailServerUsername = mailServerUsername;
            return this;
        }

        public virtual string MailServerPassword
        {
            get
            {
                return mailServerPassword;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerPassword(string mailServerPassword)
        {
            this.mailServerPassword = mailServerPassword;
            return this;
        }

        public virtual int MailServerPort
        {
            get
            {
                return mailServerPort;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerPort(int mailServerPort)
        {
            this.mailServerPort = mailServerPort;
            return this;
        }

        public virtual bool MailServerUseTls
        {
            get
            {
                return UseTls;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerUseTls(bool useTls)
        {
            this.UseTls = useTls;
            return this;
        }

        public virtual string MailServerDefaultFrom
        {
            get
            {
                return mailServerDefaultFrom;
            }
        }

        public virtual ProcessEngineConfiguration SetMailServerDefaultFrom(string mailServerDefaultFrom)
        {
            this.mailServerDefaultFrom = mailServerDefaultFrom;
            return this;
        }

        public virtual string DatabaseType
        {
            get
            {
                return databaseType;
            }
        }

        public virtual ProcessEngineConfiguration SetDatabaseType(string databaseType)
        {
            this.databaseType = databaseType;
            return this;
        }

        public virtual string DatabaseSchemaUpdate
        {
            get
            {
                return databaseSchemaUpdate;
            }
            set { databaseSchemaUpdate = value; }
        }

        public virtual ProcessEngineConfiguration SetDatabaseSchemaUpdate(string databaseSchemaUpdate)
        {
            this.databaseSchemaUpdate = databaseSchemaUpdate;
            return this;
        }

        //public virtual DataSource DataSource
        //{
        //    get
        //    {
        //        return dataSource;
        //    }
        //}

        //public virtual ProcessEngineConfiguration SetDataSource(DataSource dataSource)
        //{
        //    this.dataSource = dataSource;
        //    return this;
        //}

        public virtual ISchemaOperationsCommand SchemaOperationsCommand
        {
            get
            {
                return schemaOperationsCommand;
            }
            set
            {
                this.schemaOperationsCommand = value;
            }
        }


        public virtual string JdbcDriver
        {
            get
            {
                return jdbcDriver;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcDriver(string jdbcDriver)
        {
            this.jdbcDriver = jdbcDriver;
            return this;
        }

        public virtual string JdbcUrl
        {
            get
            {
                return jdbcUrl;
            }
            set { jdbcUrl = value; }
        }

        public virtual ProcessEngineConfiguration SetJdbcUrl(string jdbcUrl)
        {
            this.jdbcUrl = jdbcUrl;
            return this;
        }

        public virtual string JdbcUsername
        {
            get
            {
                return jdbcUsername;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcUsername(string jdbcUsername)
        {
            this.jdbcUsername = jdbcUsername;
            return this;
        }

        public virtual string JdbcPassword
        {
            get
            {
                return jdbcPassword;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcPassword(string jdbcPassword)
        {
            this.jdbcPassword = jdbcPassword;
            return this;
        }

        public virtual bool TransactionsExternallyManaged
        {
            get
            {
                return transactionsExternallyManaged;
            }
        }

        public virtual ProcessEngineConfiguration SetTransactionsExternallyManaged(bool transactionsExternallyManaged)
        {
            this.transactionsExternallyManaged = transactionsExternallyManaged;
            return this;
        }

        public virtual int JdbcMaxActiveConnections
        {
            get
            {
                return jdbcMaxActiveConnections;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcMaxActiveConnections(int jdbcMaxActiveConnections)
        {
            this.jdbcMaxActiveConnections = jdbcMaxActiveConnections;
            return this;
        }

        public virtual int JdbcMaxIdleConnections
        {
            get
            {
                return jdbcMaxIdleConnections;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcMaxIdleConnections(int jdbcMaxIdleConnections)
        {
            this.jdbcMaxIdleConnections = jdbcMaxIdleConnections;
            return this;
        }

        public virtual int JdbcMaxCheckoutTime
        {
            get
            {
                return jdbcMaxCheckoutTime;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcMaxCheckoutTime(int jdbcMaxCheckoutTime)
        {
            this.jdbcMaxCheckoutTime = jdbcMaxCheckoutTime;
            return this;
        }

        public virtual int JdbcMaxWaitTime
        {
            get
            {
                return jdbcMaxWaitTime;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcMaxWaitTime(int jdbcMaxWaitTime)
        {
            this.jdbcMaxWaitTime = jdbcMaxWaitTime;
            return this;
        }

        public virtual bool JdbcPingEnabled
        {
            get
            {
                return jdbcPingEnabled;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcPingEnabled(bool jdbcPingEnabled)
        {
            this.jdbcPingEnabled = jdbcPingEnabled;
            return this;
        }

        public virtual string JdbcPingQuery
        {
            get
            {
                return jdbcPingQuery;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcPingQuery(string jdbcPingQuery)
        {
            this.jdbcPingQuery = jdbcPingQuery;
            return this;
        }

        public virtual int JdbcPingConnectionNotUsedFor
        {
            get
            {
                return jdbcPingConnectionNotUsedFor;
            }
        }

        public virtual ProcessEngineConfiguration SetJdbcPingConnectionNotUsedFor(int jdbcPingNotUsedFor)
        {
            this.jdbcPingConnectionNotUsedFor = jdbcPingNotUsedFor;
            return this;
        }

        /// <summary>
        /// Gets the number of seconds the jdbc driver will wait for a response from the database. </summary>
        public virtual int? JdbcStatementTimeout
        {
            get
            {
                return jdbcStatementTimeout;
            }
        }

        /// <summary>
        /// Sets the number of seconds the jdbc driver will wait for a response from the database. </summary>
        public virtual ProcessEngineConfiguration SetJdbcStatementTimeout(int? jdbcStatementTimeout)
        {
            this.jdbcStatementTimeout = jdbcStatementTimeout;
            return this;
        }

        public virtual bool JobExecutorActivate
        {
            get
            {
                return jobExecutorActivate;
            }
        }

        public virtual ProcessEngineConfiguration SetJobExecutorActivate(bool jobExecutorActivate)
        {
            this.jobExecutorActivate = jobExecutorActivate;
            return this;
        }

        public virtual bool JobExecutorDeploymentAware
        {
            get
            {
                return jobExecutorDeploymentAware;
            }
        }

        public virtual ProcessEngineConfiguration SetJobExecutorDeploymentAware(bool jobExecutorDeploymentAware)
        {
            this.jobExecutorDeploymentAware = jobExecutorDeploymentAware;
            return this;
        }

        public virtual bool JobExecutorAcquireByDueDate
        {
            get
            {
                return jobExecutorAcquireByDueDate;
            }
        }

        public virtual ProcessEngineConfiguration SetJobExecutorAcquireByDueDate(bool jobExecutorAcquireByDueDate)
        {
            this.jobExecutorAcquireByDueDate = jobExecutorAcquireByDueDate;
            return this;
        }

        public virtual bool JobExecutorPreferTimerJobs
        {
            get
            {
                return jobExecutorPreferTimerJobs;
            }
        }

        public virtual ProcessEngineConfiguration SetJobExecutorPreferTimerJobs(bool jobExecutorPreferTimerJobs)
        {
            this.jobExecutorPreferTimerJobs = jobExecutorPreferTimerJobs;
            return this;
        }

        public virtual bool HintJobExecutor
        {
            get
            {
                return hintJobExecutor;
            }
        }

        public virtual ProcessEngineConfiguration SetHintJobExecutor(bool hintJobExecutor)
        {
            this.hintJobExecutor = hintJobExecutor;
            return this;
        }

        //public virtual ClassLoader ClassLoader
        //{
        //    get
        //    {
        //        return classLoader;
        //    }
        //}

        //public virtual ProcessEngineConfiguration SetClassLoader(ClassLoader classLoader)
        //{
        //    this.classLoader = classLoader;
        //    return this;
        //}



        public virtual string DataSourceJndiName
        {
            get
            {
                return dataSourceJndiName;
            }
            set
            {
                this.dataSourceJndiName = value;
            }
        }


        public virtual bool CreateIncidentOnFailedJobEnabled
        {
            get
            {
                return createIncidentOnFailedJobEnabled;
            }
        }

        public virtual ProcessEngineConfiguration SetCreateIncidentOnFailedJobEnabled(bool createIncidentOnFailedJobEnabled)
        {
            this.createIncidentOnFailedJobEnabled = createIncidentOnFailedJobEnabled;
            return this;
        }

        public virtual bool AuthorizationEnabled { get; set; } = false;

        public virtual ProcessEngineConfiguration SetAuthorizationEnabled(bool isAuthorizationChecksEnabled)
        {
            this.AuthorizationEnabled = isAuthorizationChecksEnabled;
            return this;
        }

        public virtual string DefaultUserPermissionNameForTask { get; set; } = "UPDATE";

        public virtual ProcessEngineConfiguration SetDefaultUserPermissionNameForTask(string defaultUserPermissionNameForTask)
        {
            this.DefaultUserPermissionNameForTask = defaultUserPermissionNameForTask;
            return this;
        }

        public virtual bool AuthorizationEnabledForCustomCode
        {
            get
            {
                return authorizationEnabledForCustomCode;
            }
            set { authorizationEnabledForCustomCode = value; }
        }

        public virtual ProcessEngineConfiguration SetAuthorizationEnabledForCustomCode(bool authorizationEnabledForCustomCode)
        {
            this.authorizationEnabledForCustomCode = authorizationEnabledForCustomCode;
            return this;
        }

        public virtual bool TenantCheckEnabled
        {
            get
            {
                return tenantCheckEnabled;
            }
        }

        public virtual ProcessEngineConfiguration SetTenantCheckEnabled(bool isTenantCheckEnabled)
        {
            this.tenantCheckEnabled = isTenantCheckEnabled;
            return this;
        }

        public virtual int DefaultNumberOfRetries { get; set; } = JobEntity.DefaultRetries;


        public virtual IValueTypeResolver ValueTypeResolver
        {
            get
            {
                return valueTypeResolver;
            }
        }

        public virtual ProcessEngineConfiguration SetValueTypeResolver(IValueTypeResolver valueTypeResolver)
        {
            this.valueTypeResolver = valueTypeResolver;
            return this;
        }

        public virtual bool ProducePrioritizedJobs { get; set; } = true;


        public virtual bool JobExecutorAcquireByPriority { get; set; } = false;


        public virtual bool ProducePrioritizedExternalTasks { get; set; } = true;


        public virtual string AuthorizationCheckRevokes { set; get; } = AuthorizationCheckRevokeAuto;
    }
}
