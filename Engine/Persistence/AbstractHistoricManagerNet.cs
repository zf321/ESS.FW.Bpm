using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence
{
    public class AbstractHistoricManagerNet<TEntity> :AbstractManagerNet<TEntity> where TEntity:class,IDbEntity,new()
    {
        private bool _instanceFieldsInitialized = false;
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal IHistoryLevel historyLevel ;

        protected internal bool IsHistoryEnabled;
        protected internal bool IsHistoryLevelFullEnabled;
        public AbstractHistoricManagerNet(DbContext dbContex,ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }
        private void InitializeInstanceFields()
        {
            IsHistoryEnabled = historyLevel!=(HistoryLevelFields.HistoryLevelNone);
            IsHistoryLevelFullEnabled = historyLevel==(HistoryLevelFields.HistoryLevelFull);
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
