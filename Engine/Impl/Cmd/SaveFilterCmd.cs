using System;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SaveFilterCmd : ICommand<IFilter>
    {
        private const long SerialVersionUid = 1L;

        protected internal IFilter Filter;

        public SaveFilterCmd(IFilter filter)
        {
            this.Filter = filter;
        }

        public virtual IFilter Execute(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("filter", Filter);

            return commandContext.FilterManager.InsertOrUpdateFilter(Filter);
        }
    }
}