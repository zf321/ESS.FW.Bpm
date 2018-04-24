using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Runtime
{
    /// <summary>
    ///     
    /// </summary>
    public interface IActivityInstantiationBuilder<T> where T : IActivityInstantiationBuilder<T>
    {
        /// <summary>
        ///     If an instruction is submitted before then the variable is set when the
        ///     instruction is executed. Otherwise, the variable is set on the process
        ///     instance itself.
        /// </summary>
        T SetVariable(string name, object value);

        /// <summary>
        ///     If an instruction is submitted before then the local variable is set when
        ///     the instruction is executed. Otherwise, the variable is set on the process
        ///     instance itself.
        /// </summary>
        T SetVariableLocal(string name, object value);

        /// <summary>
        ///     If an instruction is submitted before then all variables are set when the
        ///     instruction is executed. Otherwise, the variables are set on the process
        ///     instance itself.
        /// </summary>
        T SetVariables(IDictionary<string, ITypedValue> variables);

        /// <summary>
        ///     If an instruction is submitted before then all local variables are set when
        ///     the instruction is executed. Otherwise, the variables are set on the
        ///     process instance itself.
        /// </summary>
        T SetVariablesLocal(IDictionary<string, ITypedValue> variables);
    }
}