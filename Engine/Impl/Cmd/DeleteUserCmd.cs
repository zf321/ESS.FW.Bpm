using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{


    /// <summary>
    /// </summary>
    [Serializable]
    public class DeleteUserCmd : AbstractWritableIdentityServiceCmd<object>, ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        internal string UserId;

        public DeleteUserCmd(string userId)
        {
            this.UserId = userId;
        }

        protected internal override object ExecuteCmd(CommandContext commandContext)
        {
            EnsureUtil.EnsureNotNull("userId", UserId);

            // delete user picture
            new DeleteUserPictureCmd(UserId).Execute(commandContext);

            commandContext.IdentityInfoManager.DeleteUserInfoByUserId(UserId);

            commandContext.WritableIdentityProvider.DeleteUser(UserId);

            return null;
        }
    }
}