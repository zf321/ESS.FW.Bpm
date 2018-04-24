namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    /// <summary>
    ///     
    /// </summary>
    public class ConfigurationLogger : ProcessEngineLogger
    {
        public virtual ProcessEngineException InvalidConfigTransactionManagerIsNull()
        {
            return
                new ProcessEngineException(ExceptionMessage("001",
                    "Property 'transactionManager' is null and 'transactionManagerJndiName' is not set. " +
                    "Please set either the 'transactionManager' property or the 'transactionManagerJndiName' property."));
        }

        //public virtual ProcessEngineException invalidConfigCannotFindTransactionManger(string jndiName,
        //    NamingException e)
        //{
        //    return
        //        new ProcessEngineException(
        //            ExceptionMessage("002", "Cannot lookup instance of Jta Transaction manager in JNDI using name '{}'",
        //                jndiName), e);
        //}

        public virtual void PluginActivated(string pluginName, string processEngineName)
        {
            LogInfo("003", "Plugin '{0}' activated on process engine '{1}'", pluginName, processEngineName);
        }

        public virtual void DebugDatabaseproductName(string databaseProductName)
        {
            LogDebug("004", "Database product name {0}", databaseProductName);
        }

        public virtual void DebugDatabaseType(string databaseType)
        {
            LogDebug("005", "Database type {0}", databaseType);
        }

        public virtual void UsingDeprecatedHistoryLevelVariable()
        {
            LogWarn("006",
                "Using deprecated history level 'variable'. " +
                "This history level is deprecated and replaced by 'activity'. " + "Consider using 'ACTIVITY' instead.");
        }

        public virtual ProcessEngineException InvalidConfigDefaultUserPermissionNameForTask(
            string defaultUserPermissionNameForTask, string[] validPermissionNames)
        {
            return
                new ProcessEngineException(ExceptionMessage("007",
                    "Invalid value '{0}' for configuration property 'defaultUserPermissionNameForTask'. Valid values are: '{1}'",
                    defaultUserPermissionNameForTask, validPermissionNames));
        }
    }
}