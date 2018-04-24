using System;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type
{
    /// <summary>
    ///     The definition of a type used in the DMN engine to
    ///     transform data
    /// </summary>
    public interface IDmnTypeDefinition
    {
        /// <returns> the type name of this definition </returns>
        string TypeName { get; }

        /// <summary>
        ///     Transform the given value into the type specified by the type name.
        /// </summary>
        /// <param name="value"> to transform into the specified type </param>
        /// <returns>
        ///     value of specified type
        /// </returns>
        /// <exception cref="IllegalArgumentException">
        ///     if the value can not be transformed
        /// </exception>
        ITypedValue Transform(object value);
    }
}