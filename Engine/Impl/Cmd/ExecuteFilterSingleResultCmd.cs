using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ExecuteFilterSingleResultCmd : AbstractExecuteFilterCmd, ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        public ExecuteFilterSingleResultCmd(string filterId) : base(filterId)
        {
        }

        //public ExecuteFilterSingleResultCmd(string filterId, IQuery<object, object> extendingQuery)
        //    : base(filterId, extendingQuery)
        //{
        //}

        public virtual object Execute(CommandContext commandContext)
        {
            //var query = GetFilterQuery(commandContext);
            //return query.SingleResult();
            return null;
        }
    }
}