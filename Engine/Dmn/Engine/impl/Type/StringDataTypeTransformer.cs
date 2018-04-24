using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     Transform values of type <seealso cref="object" /> into <seealso cref="StringValue" />.
    ///     
    /// </summary>
    public class StringDataTypeTransformer : IDmnDataTypeTransformer
    {
        public virtual ITypedValue Transform(object value)
        {
            var stringValue = value.ToString();
            return Variables.StringValue(stringValue);
        }
    }
}