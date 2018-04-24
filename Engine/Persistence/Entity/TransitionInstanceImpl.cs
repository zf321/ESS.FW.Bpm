

using ESS.FW.Bpm.Engine.Runtime;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class TransitionInstanceImpl : ProcessElementInstanceImpl, ITransitionInstance
    {
        public virtual string ActivityId { get; set; }


        public virtual string TargetActivityId
        {
            get
            {
                return ActivityId;
            }
        }

        public virtual string ExecutionId { get; set; }


        public virtual string ActivityType { get; set; }


        public virtual string ActivityName { get; set; }


        public override string ToString()
        {
            return this.GetType().Name + "[executionId=" + ExecutionId + ", targetActivityId=" + ActivityId + ", activityName=" + ActivityName + ", activityType=" + ActivityType + ", id=" + Id + ", parentActivityInstanceId=" + ParentActivityInstanceId + ", processInstanceId=" + ProcessInstanceId + ", processDefinitionId=" + ProcessDefinitionId + "]";
        }

    }

}