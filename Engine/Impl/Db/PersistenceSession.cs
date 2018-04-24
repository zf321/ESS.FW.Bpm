using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation;
using System.Linq.Expressions;
using System.Linq;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public interface IPersistenceSession /*: ISession*/
    {
        IList<string> TableNamesPresent { get; }

        // Entity Operations /////////////////////////////////

        void ExecuteDbOperation(DbOperation operation);

        IQueryable<T> SelectAll<T>() where T : class, IDbEntity,new();

        IQueryable<T> Select<T>(Expression<Func<T, bool>> exp) where T : class,IDbEntity;
        IList<T> SelectList<T>(Expression<Func<T, bool>> exp) where T : class, IDbEntity;

        T SelectById<T>(string id) where T : class, IDbEntity;

        T SelectOne<T>(Expression<Func<T, bool>> exp) where T : class, IDbEntity;

        T Insert<T>(T entity) where T : class, IDbEntity;


        //-------------------------JAVA-------------------------
        T SelectById<T>(Type type, string id) where T : class, IDbEntity;
        object SelectOne(string statement, object parameter);

        void Lock(string statement, object parameter);

        int ExecuteUpdate(string updateStatement, object parameter);

        int ExecuteNonEmptyUpdateStmt(string updateStmt, object parameter);

        void Commit();

        void Rollback();

        // Schema Operations /////////////////////////////////

        void DbSchemaCheckVersion();

        void DbSchemaCreate();

        void DbSchemaDrop();

        void DbSchemaPrune();

        void DbSchemaUpdate();

        // listeners //////////////////////////////////////////

        void AddEntityLoadListener(IEntityLoadListener listener);
    }
}