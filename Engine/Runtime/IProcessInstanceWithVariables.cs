

using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     Represents a process instance with the corresponding latest variables.
    ///      
    /// </summary>
    public interface IProcessInstanceWithVariables : IProcessInstance
    {
        /// <summary>
        ///     Returns the latest variables of the process instance.
        /// </summary>
        /// <returns> the latest variables </returns>
        IVariableMap Variables { get; }
    }
}

