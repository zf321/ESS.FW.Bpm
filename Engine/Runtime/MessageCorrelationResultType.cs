
 

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     The message correlation result type indicates which type
    ///     of message correlation result is returned after a message correlation.
    ///     A message may be correlated to either
    ///     a waiting execution (BPMN receive message event) or a process definition
    ///     (BPMN message start event). The result type indicates which correlation was performed.
    ///      
    /// </summary>
    public enum MessageCorrelationResultType
    {
        /// <summary>
        ///     signifies a message correlated to an execution
        /// </summary>
        Execution,

        /// <summary>
        ///     signifies a message correlated to a process definition
        /// </summary>
        ProcessDefinition
    }
}

