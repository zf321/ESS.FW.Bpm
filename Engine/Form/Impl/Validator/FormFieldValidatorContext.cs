using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Form.Impl.Handler;

namespace ESS.FW.Bpm.Engine.Form.Impl.Validator
{
    /// <summary>
    ///     <para>Object passed in to a <seealso cref="IFormFieldValidator" /> providing access to validation properties</para>
    ///     
    /// </summary>
    public interface IFormFieldValidatorContext
    {
        FormFieldHandler FormFieldHandler { get; }

        /// <returns>
        ///     the execution
        ///     Deprecated, use <seealso cref="#getVariableScope()" />
        /// </returns>
        [Obsolete]
        IDelegateExecution Execution { get; }

        /// <returns> the variable scope in which the value is submitted </returns>
        IVariableScope VariableScope { get; }

        /// <returns> the configuration of this validator </returns>
        string Configuration { get; }

        /// <returns> all values submitted in the form </returns>
        IDictionary<string, object> SubmittedValues { get; }
    }
}