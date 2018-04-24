using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Helper
{

    /// <summary>
    ///     Properties of bpmn elements.
    ///     
    /// </summary>
    /// <seealso cref= Properties
    /// </seealso>
    public class BpmnProperties
    {
        public static readonly PropertyKey<string> Type = new PropertyKey<string>("type");

        public static readonly PropertyListKey<EscalationEventDefinition> EscalationEventDefinitions =
            new PropertyListKey<EscalationEventDefinition>("escalationEventDefinitions");

        public static readonly PropertyListKey<ErrorEventDefinition> ErrorEventDefinitions =
            new PropertyListKey<ErrorEventDefinition>("errorEventDefinitions");

        /// <summary>
        ///     Declaration indexed by activity that is triggered by the event; assumes that there is at most one such declaration
        ///     per activity.
        ///     There is code that relies on this assumption (e.g. when determining which declaration matches a job in the
        ///     migration logic).
        /// </summary>
        public static readonly PropertyMapKey<string, TimerDeclarationImpl> TimerDeclarations =
            new PropertyMapKey<string, TimerDeclarationImpl>("timerDeclarations", false);

        /// <summary>
        ///     Declaration indexed by activity that is triggered by the event; assumes that there is at most one such declaration
        ///     per activity.
        ///     There is code that relies on this assumption (e.g. when determining which declaration matches a job in the
        ///     migration logic).
        /// </summary>
        public static readonly PropertyMapKey<string, EventSubscriptionDeclaration> EventSubscriptionDeclarations =
            new PropertyMapKey<string, EventSubscriptionDeclaration>("eventDefinitions", false);

        public static readonly PropertyKey<ActivityImpl> CompensationBoundaryEvent =
            new PropertyKey<ActivityImpl>("compensationBoundaryEvent");

        public static readonly PropertyKey<ActivityImpl> InitialActivity = new PropertyKey<ActivityImpl>("initial");

        public static readonly PropertyKey<bool> TriggeredByEvent = new PropertyKey<bool>("triggeredByEvent");

        public static readonly PropertyKey<bool> HasConditionalEvents =
            new PropertyKey<bool>(BpmnParse.PropertynameHasConditionalEvents);
    }
}