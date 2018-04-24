using System.Text;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class ActivityBeforeInstantiationCmd : AbstractInstantiationCmd
    {
        protected internal string ActivityId;

        public ActivityBeforeInstantiationCmd(string activityId) : this(null, activityId)
        {
        }

        public ActivityBeforeInstantiationCmd(string processInstanceId, string activityId)
            : this(processInstanceId, activityId, null)
        {
        }

        public ActivityBeforeInstantiationCmd(string processInstanceId, string activityId,
            string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
        {
            ActivityId = activityId;
        }

        protected internal override string TargetElementId => ActivityId;

        public override object Execute(CommandContext commandContext)
        {
            var processInstance = commandContext.ExecutionManager.FindExecutionById(processInstanceId);
            ProcessDefinitionImpl processDefinition = processInstance.GetProcessDefinition();

            var activity = processDefinition.FindActivity(ActivityId);

            //forbid instantiation of compensation boundary events
            if (activity != null && "compensationBoundaryCatch".Equals(activity.GetProperty("type")))
                throw new ProcessEngineException("Cannot start before activity " + ActivityId + "; activity " +
                                                 "is a compensation boundary event.");

            return base.Execute(commandContext);
        }

        protected internal override ScopeImpl GetTargetFlowScope(ProcessDefinitionImpl processDefinition)
        {
            var activity = processDefinition.FindActivity(ActivityId);
            return activity.FlowScope;
        }

        protected internal override CoreModelElement GetTargetElement(ProcessDefinitionImpl processDefinition)
        {
            var activity = (ActivityImpl) processDefinition.FindActivity(ActivityId);
            return activity;
        }

        protected internal override string Describe()
        {
            var sb = new StringBuilder();
            sb.Append("Start before activity '");
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