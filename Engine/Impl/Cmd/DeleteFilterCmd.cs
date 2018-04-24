using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class DeleteFilterCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;

        protected internal string FilterId;

        public DeleteFilterCmd(string filterId)
        {
            this.FilterId = filterId;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            commandContext.FilterManager.DeleteFilter(FilterId);
            return null;
        }
    }
}