using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Management.Impl;
using ESS.FW.Bpm.Engine.Persistence.Deploy.Cache;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Purges the database and the deployment cache.
    ///     That means that each entity will be removed from the database and from the cache.
    ///     The command returns a purge report. That report contains information of the
    ///     deleted rows for each table and the values from the cache which are removed.
    ///      
    /// </summary>
    [Serializable]
    public class PurgeDatabaseAndCacheCmd : ICommand<PurgeReport>
    {
        protected internal const string DeleteTableData = "deleteTableData";
        protected internal const string SelectTableCount = "selectTableCount";
        protected internal const string TableName = "tableName";
        protected internal const string EmptyString = "";

        public static readonly IList<string> TablenamesExcludedFromDbCleanCheck = new List<string>
        {
            "ACT_GE_PROPERTY"
        };

        public virtual PurgeReport Execute(CommandContext commandContext)
        {
            var purgeReport = new PurgeReport();

            // purge the database
            var databasePurgeReport = PurgeDatabase(commandContext);
            purgeReport.DatabasePurgeReport = databasePurgeReport;

            // purge the deployment cache
            DeploymentCache deploymentCache = commandContext.ProcessEngineConfiguration.DeploymentCache;
            CachePurgeReport cachePurgeReport = deploymentCache.PurgeCache();
            purgeReport.CachePurgeReport = cachePurgeReport;

            return purgeReport;
        }

        private DatabasePurgeReport PurgeDatabase(CommandContext commandContext)
        {
            //var dbEntityManager = commandContext.DbEntityManager;
            // For MySQL and MariaDB we have to disable foreign key check,
            // to delete the table data as bulk operation (execution, incident etc.)
            // The flag will be reset by the DBEntityManager after flush.
            //dbEntityManager.IgnoreForeignKeysForNextFlush = true;
            //var tablesNames = dbEntityManager.TableNamesPresentInDatabase;
            var databaseTablePrefix = commandContext.ProcessEngineConfiguration.DatabaseTablePrefix.Trim();

            //for each table
            var databasePurgeReport = new DatabasePurgeReport();
            //foreach (var tableName in tablesNames)
            //{
            //    var tableNameWithoutPrefix = tableName.Replace(databaseTablePrefix, EmptyString);
            //    if (!TablenamesExcludedFromDbCleanCheck.Contains(tableNameWithoutPrefix))
            //    {
            //        // Check if table contains data
            //        IDictionary<string, string> param = new Dictionary<string, string>();
            //        param[TableName] = tableName;
            //        var count = (long?) dbEntityManager.SelectOne(SelectTableCount, param);

            //        if (count > 0)
            //            databasePurgeReport.AddPurgeInformation(tableName, count);
            //    }
            //}
            return databasePurgeReport;
        }
    }
}