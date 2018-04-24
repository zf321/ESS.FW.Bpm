using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.History.Impl.Event;

namespace ESS.FW.Bpm.Engine.History.Impl.Producer
{
    /// <summary>
    ///     <para>
    ///         The producer for CMMN history events. The history event producer is
    ///         responsible for extracting data from the runtime structures
    ///         (Executions, Tasks, ...) and adding the data to a <seealso cref="HistoryEvent" />.
    ///         
    ///     </para>
    /// </summary>
    public interface ICmmnHistoryEventProducer
    {
        /// <summary>
        ///     Creates the history event fired when a case instance is <strong>created</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseInstanceCreateEvt(IDelegateCaseExecution caseExecution);

        /// <summary>
        ///     Creates the history event fired when a case instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseInstanceUpdateEvt(IDelegateCaseExecution caseExecution);

        /// <summary>
        ///     Creates the history event fired when a case instance is <strong>closed</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseInstanceCloseEvt(IDelegateCaseExecution caseExecution);

        /// <summary>
        ///     Creates the history event fired when a case activity instance is <strong>created</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseActivityInstanceCreateEvt(IDelegateCaseExecution caseExecution);

        /// <summary>
        ///     Creates the history event fired when a case activity instance is <strong>updated</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseActivityInstanceUpdateEvt(IDelegateCaseExecution caseExecution);

        /// <summary>
        ///     Creates the history event fired when a case activity instance is <strong>ended</strong>.
        /// </summary>
        /// <param name="caseExecution"> the current case execution </param>
        /// <returns> the created history event </returns>
        HistoryEvent CreateCaseActivityInstanceEndEvt(IDelegateCaseExecution caseExecution);
    }
}