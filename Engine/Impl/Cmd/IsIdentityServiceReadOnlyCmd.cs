using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    public class IsIdentityServiceReadOnlyCmd : ICommand<bool>
    {
        public virtual bool Execute(CommandContext commandContext)
        {
            return false;
            //return !commandContext.SessionFactories.ContainsKey(typeof(IWritableIdentityProvider));
        }
    }
}