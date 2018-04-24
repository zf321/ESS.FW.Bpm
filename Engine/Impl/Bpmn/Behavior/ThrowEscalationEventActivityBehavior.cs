using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.tree;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     The activity behavior for an intermediate throwing escalation event and an escalation end event.
    ///     
    /// </summary>
    public class ThrowEscalationEventActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal readonly Escalation Escalation;

        public ThrowEscalationEventActivityBehavior(Escalation escalation)
        {
            this.Escalation = escalation;
        }
        
        public virtual void execute(IActivityExecution execution)
        {
            var currentActivity = execution.Activity;
            
            var escalationEventDefinitionFinder = new EscalationEventDefinitionFinder(this, Escalation.EscalationCode,
                currentActivity);
            var activityExecutionMappingCollector = new ActivityExecutionMappingCollector(execution);

            var walker = new ActivityExecutionHierarchyWalker(execution);
            walker.AddScopePreVisitor(escalationEventDefinitionFinder);
            walker.AddExecutionPreVisitor(activityExecutionMappingCollector);
            walker.AddExecutionPreVisitor(new OutputVariablesPropagator());

            walker.WalkUntil((element) => escalationEventDefinitionFinder.EscalationEventDefinition != null || element == null);

            var escalationEventDefinition = escalationEventDefinitionFinder.EscalationEventDefinition;
            if (escalationEventDefinition != null)
                executeEscalationHandler(escalationEventDefinition, activityExecutionMappingCollector);

            if ((escalationEventDefinition == null) || !escalationEventDefinition.CancelActivity)
                LeaveExecution(execution, currentActivity, escalationEventDefinition);
        }

        protected internal virtual void executeEscalationHandler(EscalationEventDefinition escalationEventDefinition,
            ActivityExecutionMappingCollector activityExecutionMappingCollector)
        {
            var escalationHandler = escalationEventDefinition.EscalationHandler;
            var escalationScope = GetScopeForEscalation(escalationEventDefinition);
            IActivityExecution escalationExecution =
                activityExecutionMappingCollector.GetExecutionForScope(escalationScope);

            if (!ReferenceEquals(escalationEventDefinition.EscalationCodeVariable, null))
                escalationExecution.SetVariable(escalationEventDefinition.EscalationCodeVariable,
                    Escalation.EscalationCode);

            escalationExecution.ExecuteActivity(escalationHandler);
        }

        protected internal virtual IPvmScope GetScopeForEscalation(EscalationEventDefinition escalationEventDefinition)
        {
            var escalationHandler = escalationEventDefinition.EscalationHandler;
            if (escalationEventDefinition.CancelActivity)
                return escalationHandler.EventScope;
            return escalationHandler.FlowScope;
        }
        
        protected internal virtual void LeaveExecution(IActivityExecution execution, IPvmActivity currentActivity,
            EscalationEventDefinition escalationEventDefinition)
        {
            //execution tree could have been expanded by triggering a non-interrupting event
           ExecutionEntity replacingExecution = (ExecutionEntity)execution.ReplacedBy;

        ExecutionEntity leavingExecution =
            (ExecutionEntity)(replacingExecution != null ? replacingExecution : execution);
            Leave(leavingExecution);
    }
        
        protected internal class EscalationEventDefinitionFinder : ITreeVisitor<IPvmScope>
        {
            protected internal readonly string EscalationCode;
            private readonly ThrowEscalationEventActivityBehavior _outerInstance;
            protected internal readonly IPvmActivity ThrowEscalationActivity;


            protected internal EscalationEventDefinition escalationEventDefinition;

            public EscalationEventDefinitionFinder(ThrowEscalationEventActivityBehavior outerInstance,
                string escalationCode, IPvmActivity throwEscalationActivity)
            {
                this._outerInstance = outerInstance;
                this.EscalationCode = escalationCode;
                this.ThrowEscalationActivity = throwEscalationActivity;
            }

            public virtual EscalationEventDefinition EscalationEventDefinition
            {
                get { return escalationEventDefinition; }
            }

            public virtual void Visit(IPvmScope scope)
            {
                var escalationEventDefinitions = scope.Properties.Get(BpmnProperties.EscalationEventDefinitions);
                escalationEventDefinition = FindMatchingEscalationEventDefinition(escalationEventDefinitions);
            }

            protected internal virtual EscalationEventDefinition FindMatchingEscalationEventDefinition(
                IList<EscalationEventDefinition> escalationEventDefinitions)
            {
                foreach (var escalationEventDefinition in escalationEventDefinitions)
                    if (IsMatchingEscalationCode(escalationEventDefinition) &&
                        !IsReThrowingEscalationEventSubprocess(escalationEventDefinition))
                        return escalationEventDefinition;
                return null;
            }

            protected internal virtual bool IsMatchingEscalationCode(EscalationEventDefinition escalationEventDefinition)
            {
                var escalationCode = escalationEventDefinition.EscalationCode;
                return ReferenceEquals(escalationCode, null) || escalationCode.Equals(this.EscalationCode);
            }

            protected internal virtual bool IsReThrowingEscalationEventSubprocess(
                EscalationEventDefinition escalationEventDefinition)
            {
                var escalationHandler = escalationEventDefinition.EscalationHandler;
                return escalationHandler.SubProcessScope && escalationHandler.Equals(ThrowEscalationActivity.FlowScope);
            }
        }
    }
}