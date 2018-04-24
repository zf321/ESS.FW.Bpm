using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetUserInfoKeysCmd : ICommand<IList<string>>
    {
        private const long SerialVersionUid = 1L;
        protected internal string UserId;
        protected internal string UserInfoType;

        public GetUserInfoKeysCmd(string userId, string userInfoType)
        {
            this.UserId = userId;
            this.UserInfoType = userInfoType;
        }

        public virtual IList<string> Execute(CommandContext commandContext)
        {
            return commandContext.IdentityInfoManager.FindUserInfoKeysByUserIdAndType(UserId, UserInfoType);
        }
    }
}