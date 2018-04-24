using System;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class GetFilterCmd : ICommand<IFilter>
    {
        private const long SerialVersionUid = 1L;

        protected internal string FilterId;

        public GetFilterCmd(string filterId)
        {
            this.FilterId = filterId;
        }

        public virtual IFilter Execute(CommandContext commandContext)
        {
            return commandContext.FilterManager.FindFilterById(FilterId);
        }
    }
}