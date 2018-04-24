



using System;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;

namespace ESS.FW.Bpm.Engine.Persistence
{

    /// <summary>
    ///  
    /// </summary>
    public class AbstractHistoricManager /*: AbstractManager*/
    {
        private bool _instanceFieldsInitialized = false;
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal IHistoryLevel historyLevel = context.Impl.Context.ProcessEngineConfiguration.HistoryLevel;

        protected internal bool IsHistoryEnabled;
        protected internal bool IsHistoryLevelFullEnabled;
        public AbstractHistoricManager()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            IsHistoryEnabled = !historyLevel.Equals(HistoryLevelFields.HistoryLevelNone);
            IsHistoryLevelFullEnabled = historyLevel.Equals(HistoryLevelFields.HistoryLevelFull);
        }
        protected internal virtual void CheckHistoryEnabled()
        {
            if (!IsHistoryEnabled)
            {
                throw Log.DisabledHistoryException();
            }
        }
        public virtual bool HistoryEnabled
        {
            get
            {
                return IsHistoryEnabled;
            }
        }

        public virtual bool HistoryLevelFullEnabled
        {
            get
            {
                return IsHistoryLevelFullEnabled;
            }
        }
    }

}