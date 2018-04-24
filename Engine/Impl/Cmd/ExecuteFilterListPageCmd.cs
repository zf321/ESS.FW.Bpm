using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ExecuteFilterListPageCmd : AbstractExecuteFilterCmd, ICommand<IList<object>>
    {
        private const long SerialVersionUid = 1L;

        protected internal int FirstResult;
        protected internal int MaxResults;

        public ExecuteFilterListPageCmd(string filterId, int firstResult, int maxResults) : base(filterId)
        {
            this.FirstResult = firstResult;
            this.MaxResults = maxResults;
        }

        //public ExecuteFilterListPageCmd(string filterId, IQuery<object, object> extendingQuery, int firstResult,
        //    int maxResults) : base(filterId, extendingQuery)
        //{
        //    this.FirstResult = firstResult;
        //    this.MaxResults = maxResults;
        //}
        
        public virtual IList<object> Execute(CommandContext commandContext)
        {
            //var query = GetFilterQuery(commandContext);
            //return (IList<object>)query.ListPage(FirstResult, MaxResults);
            throw new NotImplementedException();
        }
    }
}