using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{



    /// <summary>
	///  
	/// </summary>
	[Serializable]
    public class IdentityInfoEntity : IDbEntity, IHasDbRevision, IAccount
    {
        public const string typeUseraccount = "account";
        public const string typeUserinfo = "userinfo";
        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["value"] = Value;
            persistentState["password"] = PasswordBytes;
            return persistentState;
        }

        public virtual int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        public virtual string Id { get; set; }


        public virtual int Revision { get; set; }


        public virtual string Type { get; set; }


        public virtual string UserId { get; set; }


        public virtual string Key { get; set; }


        public virtual string Value { get; set; }


        public virtual byte[] PasswordBytes { get; set; }

        [NotMapped]
        public virtual string Password { get; set; }


        public virtual string Name
        {
            get
            {
                return Key;
            }
        }

        public virtual string Username
        {
            get
            {
                return Value;
            }
        }

        public virtual string ParentId { get; set; }


        public virtual IDictionary<string, string> Details { get; set; }


        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", type=" + Type + ", userId=" + UserId + ", key=" + Key + ", value=" + Value + ", password=" + Password + ", parentId=" + ParentId + ", details=" + Details + "]";
        }
    }

}