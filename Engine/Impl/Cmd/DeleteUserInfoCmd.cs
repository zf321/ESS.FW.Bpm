using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class DeleteUserInfoCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal string Key;
        protected internal string UserId;

        public DeleteUserInfoCmd(string userId, string key)
        {
            this.UserId = userId;
            this.Key = key;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            commandContext.IdentityInfoManager.DeleteUserInfoByUserIdAndKey(UserId, Key);
            return null;
        }
    }
}