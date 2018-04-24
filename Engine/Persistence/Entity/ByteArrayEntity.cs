//using System;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using System.Collections.Generic;

//namespace ESS.FW.Bpm.Engine.Persistence.Entity
//{


//    /// <summary>
//    ///  
//    /// </summary>
//    [Serializable]
//    public partial class ResourceEntity : IDbEntity, IHasDbRevision
//    {

//        private const long SerialVersionUid = 1L;

//        private static readonly object PersistentstateNull = new object();

//        public ResourceEntity()
//        {
//        }

//        public ResourceEntity(string name, byte[] bytes)
//        {
//            this.Name = name;
//            this.Bytes = bytes;
//        }

//        public ResourceEntity(byte[] bytes)
//        {
//            this.Bytes = bytes;
//        }

//        public virtual byte[] Bytes { get; set; }

//        public virtual object GetPersistentState()
//        {
//            return (Bytes != null ? Bytes : PersistentstateNull);
//        }

//        public virtual int RevisionNext
//        {
//            get
//            {
//                return Revision + 1;
//            }
//        }

//        // getters and setters //////////////////////////////////////////////////////

//        public virtual string Id { get; set; } = Guid.NewGuid().ToString();


//        public virtual string Name { get; set; }


//        public virtual string DeploymentId { get; set; }



//        public virtual int Revision { get; set; }


//        public virtual string TenantId { get; set; }
        
//        public virtual ICollection<ExternalTaskEntity> ExternalTasks { get; set; }
//        public virtual ICollection<JobEntity> Jobs { get; set; }
//        public virtual ICollection<VariableInstanceEntity> Variables { get; set; }
//        public override string ToString()
//        {
//            return this.GetType().Name + "[id=" + Id + ", revision=" + Revision + ", name=" + Name + ", deploymentId=" + DeploymentId + ", tenantId=" + TenantId + "]";
//        }

//    }

//}