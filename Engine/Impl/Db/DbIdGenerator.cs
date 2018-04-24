//using System;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.Cmd;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Persistence.Entity;

//namespace ESS.FW.Bpm.Engine.Impl.DB
//{
//    /// <summary>
//    ///      
//    /// </summary>
//    public class DbIdGenerator : IDGenerator
//    {
//        protected internal ICommandExecutor commandExecutor;

//        protected internal int idBlockSize;
//        protected internal long lastId;
//        protected internal long nextId;

//        public DbIdGenerator()
//        {
//            Reset();
//        }

//        public virtual int IdBlockSize
//        {
//            get { return idBlockSize; }
//            set { idBlockSize = value; }
//        }


//        public virtual ICommandExecutor CommandExecutor
//        {
//            get { return commandExecutor; }
//            set { commandExecutor = value; }
//        }

//        public virtual string NextId
//        {
//            get
//            {
//                return Guid.NewGuid().ToString();
//                //lock (this)
//                //{
//                //    if (lastId < nextId)
//                //    {
//                //        GetNewBlock();
//                //    }
//                //    long _nextId = nextId++;
//                //    return _nextId.ToString();
//                //}
//            }
//        }

//        protected internal virtual void GetNewBlock()
//        {
//            lock (this)
//            {
//                // TODO http://jira.codehaus.org/browse/ACT-45 use a separate 'requiresNew' command executor
//                //IdBlock idBlock = commandExecutor.Execute(new GetNextIdBlockCmd(idBlockSize));

//                //TODO Ioc重构时修改
//                PropertyEntity property = null;
//                using (var scope = Frameworks.Common.Components.ObjectContainer.BeginLifetimeScope())
//                {
//                    var repository = scope.Resolve<DataAccess.IRepository<PropertyEntity, string>>();
//                    property = repository.First(m => m.Name == "next.dbid");
//                }

//                long oldValue = long.Parse(property.Value);
//                long newValue = oldValue + IdBlockSize;
//                property.Value = Convert.ToString(newValue);
//                IdBlock idBlock= new IdBlock(oldValue,   - 1);

//                nextId = idBlock.NextId;
//                lastId = idBlock.LastId;
//            }
//        }


//        /// <summary>
//        ///     Reset inner state so that the generator fetches a new block of IDs from the database
//        ///     when the next ID generation request is received.
//        /// </summary>
//        public virtual void Reset()
//        {
//            nextId = 0;
//            lastId = -1;
//        }
//    }
//}