using System;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetUserInfoCmd : ICommand<string>
    {
        private const long SerialVersionUid = 1L;
        protected internal string Key;
        protected internal string UserId;

        public GetUserInfoCmd(string userId, string key)
        {
            this.UserId = userId;
            this.Key = key;
        }

        public virtual string Execute(CommandContext commandContext)
        {
            IdentityInfoEntity identityInfo = commandContext.IdentityInfoManager.FindUserInfoByUserIdAndKey(UserId, Key);

            return (identityInfo != null ? identityInfo.Value : null);
        }
    }
}