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
    public class ExecuteFilterListCmd : AbstractExecuteFilterCmd, ICommand<IList<object>>
    {
        private const long SerialVersionUid = 1L;

        public ExecuteFilterListCmd(string filterId) : base(filterId)
        {
        }

        //public ExecuteFilterListCmd(string filterId, IQuery<object, object> extendingQuery)
        //    : base(filterId, extendingQuery)
        //{
        //}
        
        public virtual IList<object> Execute(CommandContext commandContext)
        {
            //var query = GetFilterQuery(commandContext);
            //return query.List();
            throw new NotImplementedException();
        }
    }
}