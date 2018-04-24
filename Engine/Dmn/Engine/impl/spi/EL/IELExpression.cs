

using ESS.FW.Bpm.Engine.Variable.Context;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el
{
    /// <summary>
    ///     
    /// </summary>
    public interface IELExpression
    {
        /// <summary>
        ///     Execute the expression and return the value
        /// </summary>
        /// <param name="variableContext"> the context in which the expression should be executed </param>
        /// <returns> the value </returns>
        object GetValue(IVariableContext variableContext);
    }
}