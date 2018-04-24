using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Represents an interface for the condition event behaviors.
    ///     Makes it possible to leave the current activity if the condition of the
    ///     conditional event is satisfied.
    ///      
    /// </summary>
    public interface IConditionalEventBehavior
    {
        /// <summary>
        ///     Returns the current conditional event definition.
        /// </summary>
        /// <returns> the conditional event definition </returns>
        ConditionalEventDefinition ConditionalEventDefinition { get; }

        /// <summary>
        ///     Checks the condition, on satisfaction the activity is leaved.
        /// </summary>
        /// <param name="eventSubscription"> the event subscription which contains all necessary informations </param>
        /// <param name="variableEvent"> the variableEvent to evaluate the condition </param>
        void LeaveOnSatisfiedCondition(EventSubscriptionEntity eventSubscription, VariableEvent variableEvent);
    }
}

