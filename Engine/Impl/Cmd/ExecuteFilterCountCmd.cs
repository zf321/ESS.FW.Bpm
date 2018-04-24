using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Query;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class ExecuteFilterCountCmd : AbstractExecuteFilterCmd, ICommand<long>
    {
        private const long SerialVersionUid = 1L;

        public ExecuteFilterCountCmd(string filterId) : base(filterId)
        {
        }

        //public ExecuteFilterCountCmd(string filterId, IQuery<object, object> extendingQuery)
        //    : base(filterId, extendingQuery)
        //{
        //}

        public virtual long Execute(CommandContext commandContext)
        {
            var filter = GetFilter(commandContext);
            //return filter.Query.count();
            return 0;
        }
    }
}