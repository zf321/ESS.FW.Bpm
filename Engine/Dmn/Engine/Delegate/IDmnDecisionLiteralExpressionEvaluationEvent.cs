using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.@delegate
{
    /// <summary>
    ///     Event which represents the evaluation of a decision with a literal expression.
    /// </summary>
    public interface IDmnDecisionLiteralExpressionEvaluationEvent : IDmnDecisionLogicEvaluationEvent
    {
        /// <returns> the output name of the evaluated expression </returns>
        string OutputName { get; }

        /// <returns> the value of the evaluated expression </returns>
        ITypedValue OutputValue { get; }
    }
}