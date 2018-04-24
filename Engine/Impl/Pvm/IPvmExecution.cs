using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public interface IPvmExecution: IVariableScope
    {
        /// <summary>
        ///     returns the current <seealso cref="IPvmActivity" /> of the execution.
        /// </summary>
        IPvmActivity Activity { get; set; }
        //IDictionary<string, object> Variables { get; }

        void Signal(string signalName, object signalData);
        
        //void SetVariable(string variableName, object value);
        //object GetVariable(string variableName);
    }
}