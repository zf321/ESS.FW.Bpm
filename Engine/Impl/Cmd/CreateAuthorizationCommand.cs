using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class CreateAuthorizationCommand : ICommand<IAuthorization>
    {
        protected internal int Type;

        public CreateAuthorizationCommand(int type)
        {
            this.Type = type;
        }

        public virtual IAuthorization Execute(CommandContext commandContext)
        {
            return commandContext.AuthorizationManager.CreateNewAuthorization(Type);
        }
    }
}