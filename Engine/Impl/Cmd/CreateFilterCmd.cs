using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class CreateFilterCmd : ICommand<IFilter>
    {
        protected internal string ResourceType;

        public CreateFilterCmd(string resourceType)
        {
            this.ResourceType = resourceType;
        }

        public virtual IFilter Execute(CommandContext commandContext)
        {
            return commandContext.FilterManager.CreateNewFilter(ResourceType);
        }
    }
}