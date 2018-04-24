

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     <para>
    ///         The result of a message correlation. A message may be correlated to either
    ///         a waiting execution (BPMN receive message event) or a process definition
    ///         (BPMN message start event). The type of the correlation (execution vs.
    ///         processDefinition) can be obtained using <seealso cref="#getResultType()" />
    ///     </para>
    ///     
    ///     
    ///         
    /// </summary>
    public interface IMessageCorrelationResult
    {
        /// <summary>
        ///     Returns the execution entity on which the message was correlated to.
        /// </summary>
        /// <returns> the execution </returns>
        IExecution Execution { get; }

        /// <summary>
        ///     Returns the process instance id on which the message was correlated to.
        /// </summary>
        /// <returns> the process instance id </returns>
        IProcessInstance ProcessInstance { get; }

        /// <summary>
        ///     Returns the result type of the message correlation result.
        ///     Indicates if either the message was correlated to a waiting execution
        ///     or to a process definition like a start event.
        /// </summary>
        /// <returns> the result type of the message correlation result </returns>
        /// <seealso cref=
        /// <seealso cref="MessageCorrelationResultType" />
        /// </seealso>
        MessageCorrelationResultType ResultType { get; }
    }
}

