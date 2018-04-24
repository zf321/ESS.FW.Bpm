using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     The input for a evaluated decision.
    /// </summary>
    public interface IDmnEvaluatedInput
    {
        /// <returns> the id of the evaluated input or null if not set </returns>
        string Id { get; }

        /// <returns> the name of the evaluated input or null if not set </returns>
        string Name { get; }

        /// <returns> the input variable name for the input </returns>
        string InputVariable { get; }

        /// <returns> the value of the evaluated input or null if non set </returns>
        ITypedValue Value { get; }
    }
}