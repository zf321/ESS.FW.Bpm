using ESS.FW.Bpm.Engine.Repository;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Represents one execution of a  <seealso cref="IProcessDefinition" />.
    ///      
    ///     
    ///     
    /// </summary>
    public interface IProcessInstance : IExecution
    {
        /// <summary>
        ///     The id of the process definition of the process instance.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     The business key of this process instance.
        /// </summary>
        string BusinessKey { get; }

        /// <summary>
        ///     The id of the case instance associated with this process instance.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     returns true if the process instance is suspended
        /// </summary>
       // bool Suspended { get; }

        int SuspensionState { get; set; }
    }
}