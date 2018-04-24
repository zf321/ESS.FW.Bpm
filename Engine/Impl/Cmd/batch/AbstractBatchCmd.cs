using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd.Batch
{
    /// <summary>
    ///     Representation of common logic to all Batch commands
    ///     
    /// </summary>
    public abstract class AbstractBatchCmd<T> : ICommand<T>
    {
        public abstract T Execute(CommandContext commandContext);

        protected internal virtual void CheckAuthorizations(CommandContext commandContext)
        {
            commandContext.AuthorizationManager.CheckAuthorization(Permissions.Create, Resources.Batch);
        }
    }
}