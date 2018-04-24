using System.Collections.Generic;
using ESS.FW.DataAccess;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    public interface IIdentityInfoManager : IRepository<IdentityInfoEntity,string>
    {
        string DecryptPassword(byte[] storedPassword, string userPassword);
        void DeleteIdentityInfo(IdentityInfoEntity identityInfo);
        void DeleteUserInfoByUserId(string userId);
        void DeleteUserInfoByUserIdAndKey(string userId, string key);
        byte[] EncryptPassword(string accountPassword, string userPassword);
        IdentityInfoEntity FindUserAccountByUserIdAndKey(string userId, string userPassword, string key);
        IdentityInfoEntity FindUserInfoByUserIdAndKey(string userId, string key);
        IList<string> FindUserInfoKeysByUserIdAndType(string userId, string type);
        void SetUserInfo(string userId, string userPassword, string type, string key, string value, string accountPassword, IDictionary<string, string> accountDetails);
    }
}