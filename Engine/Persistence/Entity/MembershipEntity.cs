using System;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///  
    /// </summary>
    [Serializable]
    public class MembershipEntity : IDbEntity
    {

        private const long SerialVersionUid = 1L;
        

        public virtual object GetPersistentState()
        {
            // membership is not updatable
            return typeof(MembershipEntity);
        }
        public virtual string Id { get; set; }
        

        // required for mybatis
        public virtual string UserId { get; set; }

        // required for mybatis
        public virtual string GroupId { get; set; }

        public override string ToString()
        {
            return this.GetType().Name + "[user=" + UserId + ", group=" + GroupId + "]";
        }
    }

}