using System.Text;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    /// </summary>
    public class TransitionInstantiationCmd : AbstractInstantiationCmd
    {
        protected internal string TransitionId;

        public TransitionInstantiationCmd(string transitionId): this(null, transitionId)
        {
            
        }
        public TransitionInstantiationCmd(string processInstanceId, string transitionId)
            : this(processInstanceId, transitionId, null)
        {
        }

        public TransitionInstantiationCmd(string processInstanceId, string transitionId,
            string ancestorActivityInstanceId) : base(processInstanceId, ancestorActivityInstanceId)
        {
            this.TransitionId = transitionId;
        }

        protected internal override string TargetElementId
        {
            get { return TransitionId; }
        }

        protected internal override ScopeImpl GetTargetFlowScope(ProcessDefinitionImpl processDefinition)
        {
            var transition = processDefinition.FindTransition(TransitionId);
            return transition.Source.FlowScope;
        }

        protected internal override CoreModelElement GetTargetElement(ProcessDefinitionImpl processDefinition)
        {
            var transition = processDefinition.FindTransition(TransitionId);
            return transition;
        }

        protected internal override string Describe()
        {
            var sb = new StringBuilder();
            sb.Append("Start transition '");
            sb.Append(TransitionId);
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