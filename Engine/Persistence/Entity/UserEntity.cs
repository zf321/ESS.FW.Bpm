using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Identity;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{

    /// <summary>
    ///  
    /// </summary>
    [Serializable]
    public class UserEntity : IUser,IDbEntity, IHasDbRevision
    {

        private const long SerialVersionUid = 1L;

        protected internal string password;
        protected internal string newPassword;

        public UserEntity()
        {
        }

        public UserEntity(string id)
        {
            this.Id = id;
        }

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["firstName"] = FirstName;
            persistentState["lastName"] = LastName;
            persistentState["email"] = Email;
            persistentState["password"] = password;
            persistentState["salt"] = Salt;
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
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password
        {
            get
            {
                return password;
            }
            set
            {
                this.newPassword = value;
            }
        }

        public virtual string Salt { get; set; }

        /// <summary>
        /// Special setter for MyBatis.
        /// </summary>
        public virtual string DbPassword
        {
            get
            {
                return password;
            }
            set
            {
                this.password = value;
            }
        }

        public virtual int Revision { get; set; }

        public virtual void EncryptPassword()
        {
            if (newPassword != null)
            {
                Salt = GenerateSalt();
                DbPassword = EncryptPassword(newPassword, Salt);
            }
        }

        protected internal virtual string EncryptPassword(string password, string salt)
        {
            if (password == null)
            {
                return null;
            }
            else
            {
                throw new NotImplementedException();
                //string saltedPassword =SaltPassword(password, salt);
                //return context.Impl.Context.ProcessEngineConfiguration.PasswordManager.Encrypt(saltedPassword);
            }
        }
        public virtual ICollection<GroupEntity> Groups { get; set; }
        public virtual ICollection<TenantMembershipEntity> TenantMembers { get; set; }
        protected internal virtual string GenerateSalt()
        {
            return context.Impl.Context.ProcessEngineConfiguration.SaltGenerator.GenerateSalt();
        }

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", firstName=" + FirstName + ", lastName=" + LastName + ", email=" + Email + ", password=" + password + ", salt=" + Salt + "]";
        }

    }

}