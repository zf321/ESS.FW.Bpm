namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     Exception that is thrown when the Activiti engine discovers a mismatch between the
    ///     database schema version and the engine version.
    ///     The check is done when the engine is created in <seealso cref="ProcessEngineBuilder#buildProcessEngine()" />.
    ///      
    /// </summary>
    public class WrongDbException : ProcessEngineException
    {
        private const long SerialVersionUid = 1L;
        internal string dbVersion;

        internal string libraryVersion;

        public WrongDbException(string libraryVersion, string dbVersion)
            : this(
                "version mismatch: activiti library version is '" + libraryVersion + "', db version is " + dbVersion +
                " Hint: Set <property name=\"databaseSchemaUpdate\" to value=\"true\" or value=\"create-drop\" (use create-drop for testing only!) in bean processEngineConfiguration in camunda.cfg.xml for automatic schema creation",
                libraryVersion, dbVersion)
        {
        }

        public WrongDbException(string exceptionMessage, string libraryVersion, string dbVersion)
            : base(exceptionMessage)
        {
            this.libraryVersion = libraryVersion;
            this.dbVersion = dbVersion;
        }

        /// <summary>
        ///     The version of the Activiti library used.
        /// </summary>
        public virtual string LibraryVersion
        {
            get { return libraryVersion; }
        }

        /// <summary>
        ///     The version of the Activiti library that was used to create the database schema.
        /// </summary>
        public virtual string DbVersion
        {
            get { return dbVersion; }
        }
    }
}