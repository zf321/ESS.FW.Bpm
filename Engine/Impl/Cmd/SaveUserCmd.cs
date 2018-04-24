using System;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    


    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class SaveUserCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal UserEntity User;

        public SaveUserCmd(IUser user)
        {
            this.User = (UserEntity) user;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("user", User);

            commandContext.WritableIdentityProvider.SaveUser((IUser) User);

            return null;
        }
    }
}