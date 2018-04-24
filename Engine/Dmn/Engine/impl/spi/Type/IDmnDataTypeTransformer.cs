using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type
{
    /// <summary>
    ///     Transform a value into a specific type.
    ///     
    /// </summary>
    public interface IDmnDataTypeTransformer
    {
        /// <summary>
        ///     Transform the given value.
        /// </summary>
        /// <param name="value"> of any type </param>
        /// <returns>
        ///     value of the specific type
        /// </returns>
        /// <exception cref="IllegalArgumentException">
        ///     if the value can not be transformed
        /// </exception>
        ITypedValue Transform(object value);
    }
}