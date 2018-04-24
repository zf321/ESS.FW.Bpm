using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     The output for a evaluated decision.
    ///     <para>
    ///         In a decision table implementation an output can have a human readable
    ///         name and a name which can be used to reference the output value in
    ///         the decision result.
    ///     </para>
    ///     <para>
    ///         The human readable name is the {@code label} attribute of the DMN XML
    ///         {@code output} element. You can access this name by the <seealso cref="#getName()" />
    ///         getter.
    ///     </para>
    ///     <para>
    ///         The output name to reference the output value in the decision result
    ///         is the {@code name} attribute of the DMN XML {@code output} element.
    ///         You can access this output name by the <seealso cref="#getOutputName()" />
    ///         getter.
    ///     </para>
    ///     <para>
    ///         The {@code id} and {@code value} of the evaluated decision table
    ///         output entry can be access by the <seealso cref="#getId()" /> and <seealso cref="#getValue()" />
    ///         getter.
    ///     </para>
    /// </summary>
    public interface IDmnEvaluatedOutput
    {
        /// <returns> the id of the evaluated output or null if not set </returns>
        string Id { get; }

        /// <returns> the name of the evaluated output or null if not set </returns>
        string Name { get; }

        /// <returns> the output name of the evaluated output or null if not set </returns>
        string OutputName { get; }

        /// <returns> the value of the evaluated output or null if non set </returns>
        ITypedValue Value { get; }
    }
}