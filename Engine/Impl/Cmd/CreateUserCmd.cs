using System;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{

    /// <summary>
    /// </summary>
    [Serializable]
    public class CreateUserCmd : AbstractWritableIdentityServiceCmd<IUser>, ICommand<IUser>
    {
        private const long SerialVersionUid = 1L;

        protected internal string UserId;

        public CreateUserCmd(string userId)
        {
            EnsureUtil.EnsureNotNull("userId", userId);
            this.UserId = userId;
        }

        protected internal override IUser ExecuteCmd(CommandContext commandContext)
        {
            return commandContext.WritableIdentityProvider.CreateNewUser(UserId);
        }
    }
}