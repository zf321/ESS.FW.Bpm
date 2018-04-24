using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class CheckPassword : ICommand<bool>
    {
        private const long SerialVersionUid = 1L;
        internal string Password;

        internal string UserId;

        public CheckPassword(string userId, string password)
        {
            this.UserId = userId;
            this.Password = password;
        }

        public virtual bool Execute(CommandContext commandContext)
        {
            return commandContext.ReadOnlyIdentityProvider.CheckPassword(UserId, Password);
        }
    }
}