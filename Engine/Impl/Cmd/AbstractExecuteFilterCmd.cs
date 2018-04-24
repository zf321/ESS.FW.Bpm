using System;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Engine.Task;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public abstract class AbstractExecuteFilterCmd
    {
        private const long SerialVersionUid = 1L;
        //protected internal IQuery<object,object> ExtendingQuery;

        protected internal string FilterId;

        public AbstractExecuteFilterCmd(string filterId)
        {
            this.FilterId = filterId;
        }

        //        public AbstractExecuteFilterCmd(string filterId, IQuery<object,object> extendingQuery)
        //        {
        //            this.filterId = filterId;
        //            this.ExtendingQuery = extendingQuery;
        //        }

        protected internal virtual IFilter GetFilter(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("No filter id given to execute", "filterId", FilterId);
            FilterEntity filter = commandContext.FilterManager.FindFilterById(FilterId);

            EnsureUtil.EnsureNotNull("No filter found for id '" + FilterId + "'", "filter", filter);

            //if (ExtendingQuery != null)
            //    ((AbstractQuery<object, object>)ExtendingQuery).Validate();

            return filter;
        }

        //protected internal virtual IQuery<object, object> GetFilterQuery(CommandContext commandContext)
        //{
        //    throw new NotImplementedException();
        //    var filter = GetFilter(commandContext);
        //    //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //    //ORIGINAL LINE: org.camunda.bpm.engine.query.Query<?, object> query = filter.getQuery();
        //    //IQuery<object> query = filter.getQuery<IQuery<object>>();
        //    //if (query is ITaskQuery)
        //    //{
        //    //    ((ITaskQuery)query).InitializeFormKeys();
        //    //}
        //    //return query;
        //    //return null;
        //}
    }
}