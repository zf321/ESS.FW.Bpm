//using System;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.Util;
//using ISessionFactory = ESS.FW.Bpm.Engine.Impl.Interceptor.ISessionFactory;


//namespace ESS.FW.Bpm.Engine.Persistence
//{
//    using EnginePersistenceLogger = Impl.DB.EnginePersistenceLogger;


//    /// <summary>
//    ///  
//    /// </summary>
//    public class GenericManagerFactory : Impl.Interceptor.ISessionFactory
//    {

//        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

//        protected internal Type ManagerImplementation;

//        public GenericManagerFactory(Type managerImplementation)
//        {
//            this.ManagerImplementation = managerImplementation;
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @SuppressWarnings("unchecked") public GenericManagerFactory(String classname)
//        public GenericManagerFactory(string classname)
//        {
//            ManagerImplementation = (Type)ReflectUtil.LoadClass(classname);
//        }

//        public virtual Type SessionType
//        {
//            get
//            {
//                return ManagerImplementation;
//            }
//        }


//        public ISession OpenSession()
//        {
//            return (ISession)ManagerImplementation.Assembly.CreateInstance(ManagerImplementation.FullName);
//        }
//    }

//}