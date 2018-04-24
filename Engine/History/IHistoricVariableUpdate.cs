using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Update of a process variable.  This is only available if history
    ///     level is configured to FULL.
    ///      
    /// </summary>
    public interface IHistoricVariableUpdate : IHistoricDetail
    {
        string VariableName { get; }

        /// <summary>
        ///     Returns the id of the corresponding variable instance.
        /// </summary>
        string VariableInstanceId { get; }

        /// <summary>
        ///     Returns the type name of the variable
        /// </summary>
        /// <returns> the type name of the variable </returns>
        string TypeName { get; }

        /// <returns> the name of the variable type. </returns>
        /// @deprecated since 7.2. Use
        /// <seealso cref="#getTypeName()" />
        [Obsolete("since 7.2. Use <seealso cref=\"#getTypeName()\"/>")]
        string VariableTypeName { get; }

        object Value { get; }

        /// <returns> the <seealso cref="TypedValue" /> for this variable update </returns>
        ITypedValue TypedValue { get; }

        int Revision { get; }

        /// <summary>
        ///     If the variable value could not be loaded, this returns the error message.
        /// </summary>
        /// <returns> an error message indicating why the variable value could not be loaded. </returns>
        string ErrorMessage { get; }
    }
}