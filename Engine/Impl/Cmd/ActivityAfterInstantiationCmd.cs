using System.Text;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class ActivityAfterInstantiationCmd : AbstractInstantiationCmd
    {
        protected internal string ActivityId;

        public ActivityAfterInstantiationCmd(string activityId):this(null, activityId)
        {
            
        }
        public ActivityAfterInstantiationCmd(string processInstanceId, string activityId)
            : this(processInstanceId, activityId, null)
        {
        }

        public ActivityAfterInstantiationCmd(string processInstanceId, string activityId,
            string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
        {
            this.ActivityId = activityId;
        }

        protected internal override string TargetElementId
        {
            get { return ActivityId; }
        }

        protected internal override ScopeImpl GetTargetFlowScope(ProcessDefinitionImpl processDefinition)
        {
            var transition = FindTransition(processDefinition);

            return transition.Destination.FlowScope;
        }

        protected internal override CoreModelElement GetTargetElement(ProcessDefinitionImpl processDefinition)
        {
            return FindTransition(processDefinition);
        }

        protected internal virtual TransitionImpl FindTransition(ProcessDefinitionImpl processDefinition)
        {
            var activity = (IPvmActivity) processDefinition.FindActivity(ActivityId);

            EnsureUtil.EnsureNotNull(typeof(NotValidException),
                DescribeFailure("Activity '" + ActivityId + "' does not exist"), "activity", activity);

            if (activity.OutgoingTransitions.Count == 0)
                throw new ProcessEngineException("Cannot start after activity " + ActivityId + "; activity " +
                                                 "has no outgoing sequence flow to take");
            if (activity.OutgoingTransitions.Count > 1)
                throw new ProcessEngineException("Cannot start after activity " + ActivityId + "; " +
                                                 "activity has more than one outgoing sequence flow");

            return (TransitionImpl) activity.OutgoingTransitions[0];
        }

        protected internal override string Describe()
        {
            var sb = new StringBuilder();
            sb.Append("Start after activity '");
            sb.Append(ActivityId);
            sb.Append("'");
            if (!ReferenceEquals(AncestorActivityInstanceId, null))
            {
                sb.Append(" with ancestor activity instance '");
                sb.Append(AncestorActivityInstanceId);
                sb.Append("'");
            }

            return sb.ToString();
        }
    }
}