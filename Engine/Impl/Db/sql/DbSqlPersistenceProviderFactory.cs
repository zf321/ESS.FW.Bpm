//using System;
//using ESS.FW.Bpm.Engine.context.Impl;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;

//namespace ESS.FW.Bpm.Engine.Impl.DB.Sql
//{
//    /// <summary>
//    ///     Provides the <seealso cref="DbSqlSession" /> as <seealso cref="PersistenceSession" />.
//    ///     
//    /// </summary>
//    [Obsolete("弃用",true)]
//    public class DbSqlPersistenceProviderFactory : ISessionFactory
//    {
//        public virtual Type SessionType
//        {
//            get { return typeof(IPersistenceSession); }
//        }

//        public virtual ISession OpenSession()
//        {
//            return Context.CommandContext.DbSqlSession;
//        }
//    }
//}