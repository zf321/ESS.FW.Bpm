using System;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl
{
    //TODO 数据库初始化Cmd
    public sealed class SchemaOperationsProcessEngineBuild : ISchemaOperationsCommand
    {
        private static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        public object Execute(CommandContext commandContext)
        {
            var databaseSchemaUpdate = context.Impl.Context.ProcessEngineConfiguration.DatabaseSchemaUpdate;
            //var persistenceSession =
            //    commandContext.GetSession<IPersistenceSession>(typeof(IPersistenceSession));
            if (ProcessEngineConfigurationImpl.DbSchemaUpdateDropCreate.Equals(databaseSchemaUpdate))
                try
                {
                    //persistenceSession.DbSchemaDrop();
                }
                catch (System.Exception e)
                {
                    throw new System.Exception(e.Message);
                    // ignore
                }
            //TODO 临时注释，创建一次后 不再创建数据库
            //if (ProcessEngineConfiguration.DbSchemaUpdateCreateDrop==databaseSchemaUpdate ||
            //    ProcessEngineConfigurationImpl.DbSchemaUpdateDropCreate==databaseSchemaUpdate ||
            //    ProcessEngineConfigurationImpl.DbSchemaUpdateCreate==databaseSchemaUpdate)
            //    persistenceSession.DbSchemaCreate();
            //else if (ProcessEngineConfiguration.DbSchemaUpdateFalse==databaseSchemaUpdate)
            //    persistenceSession.DbSchemaCheckVersion();
            //else if (ProcessEngineConfiguration.DbSchemaUpdateTrue==databaseSchemaUpdate)
            //    persistenceSession.DbSchemaUpdate();


            //DbEntityManager entityManager = commandContext.GetSession<DbEntityManager>(typeof(DbEntityManager));
            //CheckHistoryLevel(entityManager);
            //CheckDeploymentLockExists(entityManager);

            return null;
        }

        public static void DbCreateHistoryLevel(CommandContext entityManager)
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            var configuredHistoryLevel = processEngineConfiguration.HistoryLevel;
            PropertyEntity property = new PropertyEntity("historyLevel", Convert.ToString(configuredHistoryLevel.Id));
            entityManager.PropertyManager.Add(property);
            Log.CreatingHistoryLevelPropertyInDatabase(configuredHistoryLevel);
        }

        /// <param name="entityManager"> entoty manager for db query </param>
        /// <returns> Integer value representing the history level or <code>null</code> if none found </returns>
        public static int? DatabaseHistoryLevel(CommandContext entityManager)
        {
            try
            {
                //PropertyEntity historyLevelProperty = entityManager.SelectById<PropertyEntity>(typeof (PropertyEntity),
                //"historyLevel");
                PropertyEntity historyLevelProperty = entityManager.PropertyManager.Get("historyLevel");
                return historyLevelProperty != null ? Convert.ToInt32(historyLevelProperty.Value) : (int?)null;
            }
            catch (System.Exception e)
            {
                Log.CouldNotSelectHistoryLevel(e.Message);
                return null;
            }
            return 0;
        }

        //public void CheckHistoryLevel(DbEntityManager entityManager)
        //{
        //    var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;


        //    var databaseHistoryLevel =
        //        new DetermineHistoryLevelCmd(processEngineConfiguration.HistoryLevels).Execute(context.Impl.Context.CommandContext);
        //    DetermineAutoHistoryLevel(processEngineConfiguration, databaseHistoryLevel);

        //    var configuredHistoryLevel = processEngineConfiguration.HistoryLevel;

        //    if (databaseHistoryLevel == null)
        //    {
        //        Log.NoHistoryLevelPropertyFound();
        //        //DbCreateHistoryLevel(entityManager);
        //    }
        //    else
        //    {
        //        if (!((int?) configuredHistoryLevel.Id).Equals(databaseHistoryLevel.Id))
        //            throw new ProcessEngineException("historyLevel mismatch: configuration says " +
        //                                             configuredHistoryLevel + " and database says " +
        //                                             databaseHistoryLevel);
        //    }
        //}

        protected internal void DetermineAutoHistoryLevel(ProcessEngineConfigurationImpl engineConfiguration,
            IHistoryLevel databaseHistoryLevel)
        {
            var configuredHistoryLevel = engineConfiguration.HistoryLevel;

            if ((configuredHistoryLevel == null) &&
                ProcessEngineConfiguration.HistoryAuto.Equals(engineConfiguration.History))
                if (databaseHistoryLevel != null)
                    engineConfiguration.HistoryLevel = databaseHistoryLevel;
                else
                    engineConfiguration.HistoryLevel = engineConfiguration.DefaultHistoryLevel;
        }

        //public void CheckDeploymentLockExists(DbEntityManager entityManager)
        //{
        //    PropertyEntity deploymentLockProperty = entityManager.SelectById<PropertyEntity>(typeof(PropertyEntity), "deployment.lock");
        //    if (deploymentLockProperty == null)
        //    {
        //        Log.NoDeploymentLockPropertyFound();
        //    }
        //}
    }
}