using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{
    /// <summary>
    ///     The producer for DMN history events. The history event producer is
    ///     responsible for extracting data from the dmn engine and adding the data to a
    ///     <seealso cref="HistoryEvent" />.
    ///     
    /// </summary>
    public interface IDmnHistoryEventProducer
    {
        /// <summary>
        ///     Creates the history event fired when a decision is evaluated while execute
        ///     a process instance.
        /// </summary>
        /// <param name="execution">
        ///     the current execution
        /// </param>
        /// <param name="decisionEvaluationEvent">
        ///     the evaluation event
        /// </param>
        /// <returns>
        ///     the history event
        /// </returns>
        /// <seealso cref= # createDecisionEvaluatedEvt( DmnDecisionEvaluationEvent
        /// )
        /// </seealso>
        HistoryEvent CreateDecisionEvaluatedEvt(IDelegateExecution execution,
            IDmnDecisionEvaluationEvent decisionEvaluationEvent);

        /// <summary>
        ///     Creates the history event fired when a decision is evaluated while execute
        ///     a case instance.
        /// </summary>
        /// <param name="execution">
        ///     the current case execution
        /// </param>
        /// <param name="decisionEvaluationEvent">
        ///     the evaluation event
        /// </param>
        /// <returns>
        ///     the history event
        /// </returns>
        /// <seealso cref= # createDecisionEvaluatedEvt( DmnDecisionEvaluationEvent
        /// )
        /// </seealso>
        HistoryEvent CreateDecisionEvaluatedEvt(IDelegateCaseExecution execution,
            IDmnDecisionEvaluationEvent decisionEvaluationEvent);

        /// <summary>
        ///     Creates the history event fired when a decision is evaluated. If the
        ///     decision is evaluated while execute a process instance then you should use
        ///     <seealso cref="#createDecisionEvaluatedEvt(DelegateExecution, DmnDecisionEvaluationEvent)" /> instead.
        /// </summary>
        /// <param name="decisionEvaluationEvent">
        ///     the evaluation event
        /// </param>
        /// <returns> the history event </returns>
        HistoryEvent CreateDecisionEvaluatedEvt(IDmnDecisionEvaluationEvent decisionEvaluationEvent);
    }
}