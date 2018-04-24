using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///      
    /// </summary>
    [Serializable]
    public class SetUserInfoCmd : ICommand<object>
    {
        private const long SerialVersionUid = 1L;
        protected internal IDictionary<string, string> AccountDetails;
        protected internal string AccountPassword;
        protected internal string Key;
        protected internal string Type;
        protected internal string UserId;
        protected internal string UserPassword;
        protected internal string Value;

        public SetUserInfoCmd(string userId, string key, string value)
        {
            this.UserId = userId;
            Type = IdentityInfoEntity.typeUserinfo;
            this.Key = key;
            this.Value = value;
        }

        public SetUserInfoCmd(string userId, string userPassword, string accountName, string accountUsername,
            string accountPassword, IDictionary<string, string> accountDetails)
        {
            this.UserId = userId;
            this.UserPassword = userPassword;
            Type = IdentityInfoEntity.typeUseraccount;
            Key = accountName;
            Value = accountUsername;
            this.AccountPassword = accountPassword;
            this.AccountDetails = accountDetails;
        }

        public virtual object Execute(CommandContext commandContext)
        {
            commandContext.IdentityInfoManager.SetUserInfo(UserId, UserPassword, Type, Key, Value, AccountPassword,
                AccountDetails);
            return null;
        }
    }
}