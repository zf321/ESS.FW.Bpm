using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Read the already configured historyLevel from DB and map to given list of total levels.
    /// </summary>
    public class DetermineHistoryLevelCmd : ICommand<IHistoryLevel>
    {
        private readonly IList<IHistoryLevel> _historyLevels;
        
        public DetermineHistoryLevelCmd(IList<IHistoryLevel> historyLevels)
        {
            this._historyLevels = historyLevels;
        }
        
        public virtual IHistoryLevel Execute(CommandContext commandContext)
        {
            //var databaseHistoryLevel =
            //    SchemaOperationsProcessEngineBuild.DatabaseHistoryLevel(
            //        commandContext.GetSession<DbEntityManager>(typeof(DbEntityManager)));


            //IHistoryLevel result = null;

            //if (databaseHistoryLevel != null)
            //{
            //    foreach (var historyLevel in _historyLevels)
            //    {
            //        if (historyLevel.Id == databaseHistoryLevel)
            //        {
            //            result = historyLevel;
            //            break;
            //        }
            //    }

            //    if (result != null)
            //    {
            //        return result;
            //    }
            //    // if a custom non-null value is not registered, throw an exception.
            //    throw new ProcessEngineException(
            //        string.Format("The configured history level with id='{0}' is not registered in this config.",
            //            databaseHistoryLevel));
            //}
            return null;
        }
    }
}