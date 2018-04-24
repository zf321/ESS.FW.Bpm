using System;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class GetUserAccountCmd : ICommand<IAccount>
    {
        private const long SerialVersionUid = 1L;
        protected internal string AccountName;
        protected internal string UserId;
        protected internal string UserPassword;

        public GetUserAccountCmd(string userId, string userPassword, string accountName)
        {
            this.UserId = userId;
            this.UserPassword = userPassword;
            this.AccountName = accountName;
        }

        public virtual IAccount Execute(CommandContext commandContext)
        {
           return commandContext.IdentityInfoManager.FindUserAccountByUserIdAndKey(UserId, UserPassword, AccountName);
        }
    }
}