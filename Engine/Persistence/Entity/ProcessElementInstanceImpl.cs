using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ProcessElementInstanceImpl : IProcessElementInstance
    {
        public virtual string Id { get; set; }

        public virtual string ParentActivityInstanceId { get; set; }

        public virtual string ProcessInstanceId { get; set; }

        public virtual string ProcessDefinitionId { get; set; }

        public override string ToString()
        {
            return this.GetType().Name + "[id=" + Id + ", parentActivityInstanceId=" + ParentActivityInstanceId + ", processInstanceId=" + ProcessInstanceId + ", processDefinitionId=" + ProcessDefinitionId + "]";
        }

    }

}