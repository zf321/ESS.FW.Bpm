using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESS.FW.Bpm.Engine.Common;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
	///  
	/// </summary>
    [Component]
    public class IdentityInfoManager : AbstractManagerNet<IdentityInfoEntity>, IIdentityInfoManager
    {
        public IdentityInfoManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public virtual void DeleteUserInfoByUserIdAndKey(string userId, string key)
        {
            IdentityInfoEntity identityInfoEntity = FindUserInfoByUserIdAndKey(userId, key);
            if (identityInfoEntity != null)
            {
                DeleteIdentityInfo(identityInfoEntity);
            }
        }

        public virtual void DeleteIdentityInfo(IdentityInfoEntity identityInfo)
        {
            Delete(identityInfo);
            if (IdentityInfoEntity.typeUseraccount.Equals(identityInfo.Type))
            {
                foreach (IdentityInfoEntity identityInfoDetail in FindIdentityInfoDetails(identityInfo.Id))
                {
                    Delete(identityInfoDetail);
                }
            }
        }

        public virtual IdentityInfoEntity FindUserAccountByUserIdAndKey(string userId, string userPassword, string key)
        {
            IdentityInfoEntity identityInfoEntity = FindUserInfoByUserIdAndKey(userId, key);
            if (identityInfoEntity == null)
            {
                return null;
            }

            IDictionary<string, string> details = new Dictionary<string, string>();
            string identityInfoId = identityInfoEntity.Id;
            IList<IdentityInfoEntity> identityInfoDetails = FindIdentityInfoDetails(identityInfoId);
            foreach (IdentityInfoEntity identityInfoDetail in identityInfoDetails)
            {
                details[identityInfoDetail.Key] = identityInfoDetail.Value;
            }
            identityInfoEntity.Details = details;

            if (identityInfoEntity.PasswordBytes != null)
            {
                string password = DecryptPassword(identityInfoEntity.PasswordBytes, userPassword);
                identityInfoEntity.Password = password;
            }

            return identityInfoEntity;
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.Util.List<IdentityInfoEntity> findIdentityInfoDetails(String identityInfoId)
        protected internal virtual IList<IdentityInfoEntity> FindIdentityInfoDetails(string identityInfoId)
        {
            //return context.Impl.Context.CommandContext.DbSqlSession.SqlSession.SelectList<IdentityInfoEntity>("selectIdentityInfoDetails", identityInfoId);
            return Find(m=>m.ParentId==identityInfoId).ToList();
        }

        public virtual void SetUserInfo(string userId, string userPassword, string type, string key, string value, string accountPassword, IDictionary<string, string> accountDetails)
        {
            byte[] storedPassword = null;
            if (accountPassword != null)
            {
                storedPassword = EncryptPassword(accountPassword, userPassword);
            }

            IdentityInfoEntity identityInfoEntity = FindUserInfoByUserIdAndKey(userId, key);
            if (identityInfoEntity != null)
            {
                // update
                identityInfoEntity.Value = value;
                identityInfoEntity.PasswordBytes = storedPassword;

                if (accountDetails == null)
                {
                    accountDetails = new Dictionary<string, string>();
                }

                //IDictionary<string, string>.KeyCollection newKeys = new HashSet<string>(accountDetails.Keys);
                HashSet<string> newKeys = new HashSet<string>(accountDetails.Keys);
                IList<IdentityInfoEntity> identityInfoDetails = FindIdentityInfoDetails(identityInfoEntity.Id);
                foreach (IdentityInfoEntity identityInfoDetail in identityInfoDetails)
                {
                    string detailKey = identityInfoDetail.Key;
                    newKeys.Remove(detailKey);
                    string newDetailValue = accountDetails[detailKey];
                    if (newDetailValue == null)
                    {
                        DeleteIdentityInfo(identityInfoDetail);
                    }
                    else
                    {
                        // update detail
                        identityInfoDetail.Value = newDetailValue;
                    }
                }
                InsertAccountDetails(identityInfoEntity, accountDetails, newKeys);


            }
            else
            {
                // insert
                identityInfoEntity = new IdentityInfoEntity();
                identityInfoEntity.UserId = userId;
                identityInfoEntity.Type = type;
                identityInfoEntity.Key = key;
                identityInfoEntity.Value = value;
                identityInfoEntity.PasswordBytes = storedPassword;
                Add(identityInfoEntity);
                if (accountDetails != null)
                {
                    InsertAccountDetails(identityInfoEntity, accountDetails, new HashSet<string>(accountDetails.Keys));
                }
            }
        }

        private void InsertAccountDetails(IdentityInfoEntity identityInfoEntity, IDictionary<string, string> accountDetails, ISet<string> keys)
        {
            foreach (string newKey in keys)
            {
                // insert detail
                IdentityInfoEntity identityInfoDetail = new IdentityInfoEntity();
                identityInfoDetail.ParentId = identityInfoEntity.Id;
                identityInfoDetail.Key = newKey;
                identityInfoDetail.Value = accountDetails[newKey];
                Add(identityInfoDetail);
            }
        }

        public virtual byte[] EncryptPassword(string accountPassword, string userPassword)
        {
            // TODO
            return accountPassword.GetBytes();
        }

        public virtual string DecryptPassword(byte[] storedPassword, string userPassword)
        {
            // TODO
            return StringHelperClass.NewString(storedPassword);
        }

        public virtual IdentityInfoEntity FindUserInfoByUserIdAndKey(string userId, string key)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["userId"] = userId;
            //parameters["key"] = key;
            //return (IdentityInfoEntity)DbEntityManager.SelectOne("selectIdentityInfoByUserIdAndKey", parameters);
            return Single(m => m.UserId == userId && m.Key == key);
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<String> findUserInfoKeysByUserIdAndType(String userId, String type)
        public virtual IList<string> FindUserInfoKeysByUserIdAndType(string userId, string type)
        {
            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["userId"] = userId;
            //parameters["type"] = type;
            //return DbSqlSession.SqlSession.SelectList<string>("selectIdentityInfoKeysByUserIdAndType", parameters);
            return Find(m => m.UserId == userId && m.Type == type).Select(m => m.Key).ToList();
        }

        public virtual void DeleteUserInfoByUserId(string userId)
        {
            //IList<IdentityInfoEntity> identityInfos =ListExt.ConvertToListT<IdentityInfoEntity>( DbEntityManager.SelectList("selectIdentityInfoByUserId", userId));
            IList<IdentityInfoEntity> identityInfos = Find(m => m.UserId == userId).ToList();
            foreach (IdentityInfoEntity identityInfo in identityInfos)
            {
                DeleteIdentityInfo(identityInfo);
            }
        }
    }

}