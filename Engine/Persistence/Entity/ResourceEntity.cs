using System;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Repository;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ResourceEntity : IDbEntity, IResource
    {
        public virtual byte[] Bytes { get; set; }

        public virtual bool Generated { get; set; }


        public virtual string TenantId { get; set; }


        public virtual string Id { get; set; }


        public virtual object GetPersistentState()
        {
            return typeof(ResourceEntity);
        }


        public virtual string Name { get; set; }


        public virtual string DeploymentId { get; set; }

        public ResourceEntity()
        {
        }

        public ResourceEntity(string name, byte[] bytes)
        {
            this.Name = name;
            this.Bytes = bytes;
        }

        public ResourceEntity(byte[] bytes)
        {
            this.Bytes = bytes;
        }

        [NotMapped]
        public string ByteString
        {
            get
            {
                if (Bytes == null) return string.Empty;
                return System.Text.Encoding.UTF8.GetString(Bytes);
            }
            set { this.Bytes = System.Text.Encoding.UTF8.GetBytes(value); }
        }
        //public virtual DeploymentEntity Deployment { get; set; }
        //public virtual ICollection<ExternalTaskEntity> ExternalTasks { get; set; }
        //public virtual ICollection<JobEntity> Jobs { get; set; }
        //public virtual ICollection<VariableInstanceEntity> Variables { get; set; }
        public override string ToString()
        {
            return GetType()
                       .Name + "[id=" + Id + ", name=" + Name + ", deploymentId=" + DeploymentId + ", generated=" +
                   Generated + ", tenantId=" + TenantId + "]";
        }
    }
}