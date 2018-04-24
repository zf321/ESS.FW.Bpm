using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public abstract class AbstractPersistenceSession : IPersistenceSession
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        protected internal IList<IEntityLoadListener> Listeners = new List<IEntityLoadListener>(1);

        protected internal abstract string DbVersion { get; }

        public abstract bool IsEngineTablePresent { get; }

        public abstract bool HistoryTablePresent { get; }

        public abstract bool IdentityTablePresent { get; }

        public abstract bool CmmnTablePresent { get; }

        public abstract bool CmmnHistoryTablePresent { get; }

        public abstract bool DmnTablePresent { get; }

        public abstract bool DmnHistoryTablePresent { get; }
        public abstract void Close();
        public abstract void Flush();
        public abstract void DbSchemaCheckVersion();
        public abstract void Rollback();
        public abstract void Commit();
        public abstract int ExecuteNonEmptyUpdateStmt(string updateStmt, object parameter);
        public abstract int ExecuteUpdate(string updateStatement, object parameter);
        public abstract void Lock(string statement, object parameter);
        public abstract object SelectOne(string statement, object parameter);
        public abstract T SelectById<T>(Type type, string id) where T:class, IDbEntity;
        public abstract IList<object> SelectList(string statement, object parameter);

        public virtual void ExecuteDbOperation(DbOperation operation)
        {
            switch (operation.OperationType)
            {
                case DbOperationType.Insert:
                    InsertEntity((DbEntityOperation) operation);
                    break;

                case DbOperationType.Delete:
                    DeleteEntity((DbEntityOperation) operation);
                    break;
                case DbOperationType.DeleteBulk:
                    DeleteBulk((DbBulkOperation) operation);
                    break;

                case DbOperationType.Update:
                    UpdateEntity((DbEntityOperation) operation);
                    break;
                case DbOperationType.UpdateBulk:
                    UpdateBulk((DbBulkOperation) operation);
                    break;
            }
        }
        //TODO DbSchemaCreate
        public virtual void DbSchemaCreate()
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            var configuredHistoryLevel = processEngineConfiguration.HistoryLevel;
            if ((!processEngineConfiguration.DbHistoryUsed) &&
                (!configuredHistoryLevel.Equals(HistoryLevelFields.HistoryLevelNone)))
            {
                throw Log.DatabaseHistoryLevelException(configuredHistoryLevel.Name);
            }

            if (IsEngineTablePresent)
            {
                var dbVersion = DbVersion;
                if (!ProcessEngineFields.Version.Equals(dbVersion))
                {
                    throw Log.WrongDbVersionException(ProcessEngineFields.Version, dbVersion);
                }
            }
            else
            {
                DbSchemaCreateEngine();
            }

            if (processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaCreateHistory();
            }

            if (processEngineConfiguration.DbIdentityUsed)
            {
                DbSchemaCreateIdentity();
            }

            if (processEngineConfiguration.CmmnEnabled)
            {
                DbSchemaCreateCmmn();
            }

            if (processEngineConfiguration.CmmnEnabled && processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaCreateCmmnHistory();
            }

            if (processEngineConfiguration.DmnEnabled)
            {
                DbSchemaCreateDmn();

                if (processEngineConfiguration.DbHistoryUsed)
                {
                    DbSchemaCreateDmnHistory();
                }
            }
        }

        //TODO DbSchemaDrop
        public virtual void DbSchemaDrop()
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;

            if (processEngineConfiguration.DmnEnabled)
            {
                DbSchemaDropDmn();

                if (processEngineConfiguration.DbHistoryUsed)
                {
                    DbSchemaDropDmnHistory();
                }
            }

            if (processEngineConfiguration.CmmnEnabled)
            {
                DbSchemaDropCmmn();
            }

            DbSchemaDropEngine();

            if (processEngineConfiguration.CmmnEnabled && processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaDropCmmnHistory();
            }

            if (processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaDropHistory();
            }

            if (processEngineConfiguration.DbIdentityUsed)
            {
                DbSchemaDropIdentity();
            }
        }

        public virtual void DbSchemaPrune()
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            if (HistoryTablePresent && !processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaDropHistory();
            }
            if (IdentityTablePresent && !processEngineConfiguration.DbIdentityUsed)
            {
                DbSchemaDropIdentity();
            }
            if (CmmnTablePresent && !processEngineConfiguration.CmmnEnabled)
            {
                DbSchemaDropCmmn();
            }
            if (CmmnHistoryTablePresent &&
                (!processEngineConfiguration.CmmnEnabled || !processEngineConfiguration.DbHistoryUsed))
            {
                DbSchemaDropCmmnHistory();
            }
            if (DmnTablePresent && !processEngineConfiguration.DmnEnabled)
            {
                DbSchemaDropDmn();
            }
            if (DmnHistoryTablePresent &&
                (!processEngineConfiguration.DmnEnabled || !processEngineConfiguration.DbHistoryUsed))
            {
                DbSchemaDropDmnHistory();
            }
        }

        public virtual void DbSchemaUpdate()
        {
            var processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;

            if (!IsEngineTablePresent)
            {
                DbSchemaCreateEngine();
            }

            if (!HistoryTablePresent && processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaCreateHistory();
            }

            if (!IdentityTablePresent && processEngineConfiguration.DbIdentityUsed)
            {
                DbSchemaCreateIdentity();
            }

            if (!CmmnTablePresent && processEngineConfiguration.CmmnEnabled)
            {
                DbSchemaCreateCmmn();
            }

            if (!CmmnHistoryTablePresent && processEngineConfiguration.CmmnEnabled &&
                processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaCreateCmmnHistory();
            }

            if (!DmnTablePresent && processEngineConfiguration.DmnEnabled)
            {
                DbSchemaCreateDmn();
            }

            if (!DmnHistoryTablePresent && processEngineConfiguration.DmnEnabled &&
                processEngineConfiguration.DbHistoryUsed)
            {
                DbSchemaCreateDmnHistory();
            }
        }

        public virtual IList<string> TableNamesPresent
        {
            get { return new List<string>(); }
        }

        public virtual void AddEntityLoadListener(IEntityLoadListener listener)
        {
            Listeners.Add(listener);
        }

        protected internal abstract void InsertEntity(DbEntityOperation operation);

        protected internal abstract void DeleteEntity(DbEntityOperation operation);

        protected internal abstract void DeleteBulk(DbBulkOperation operation);

        protected internal abstract void UpdateEntity(DbEntityOperation operation);

        protected internal abstract void UpdateBulk(DbBulkOperation operation);

        protected internal abstract void DbSchemaCreateIdentity();

        protected internal abstract void DbSchemaCreateHistory();

        protected internal abstract void DbSchemaCreateEngine();

        protected internal abstract void DbSchemaCreateCmmn();

        protected internal abstract void DbSchemaCreateCmmnHistory();

        protected internal abstract void DbSchemaCreateDmn();

        protected internal abstract void DbSchemaCreateDmnHistory();

        protected internal abstract void DbSchemaDropIdentity();

        protected internal abstract void DbSchemaDropHistory();

        protected internal abstract void DbSchemaDropEngine();

        protected internal abstract void DbSchemaDropCmmn();

        protected internal abstract void DbSchemaDropCmmnHistory();

        protected internal abstract void DbSchemaDropDmn();

        protected internal abstract void DbSchemaDropDmnHistory();


        protected internal virtual void FireEntityLoaded(object result)
        {
            if (result != null && result is IDbEntity)
            {
                var entity = (IDbEntity) result;
                foreach (var entityLoadListener in Listeners)
                {
                    entityLoadListener.OnEntityLoaded(entity);
                }
            }
        }
        public abstract IQueryable<T> SelectAll<T>() where T : class, IDbEntity ,new();
        public abstract IQueryable<T> Select<T>(Expression<Func<T, bool>> exp) where T : class, IDbEntity;

        public abstract T SelectById<T>(string id) where T : class, IDbEntity;


        public abstract T SelectOne<T>(Expression<Func<T, bool>> exp) where T : class, IDbEntity;

        public abstract T Insert<T>(T entity) where T : class, IDbEntity;
        public abstract IList<T> SelectList<T>(Expression<Func<T, bool>> exp) where T : class, IDbEntity;
    }
}