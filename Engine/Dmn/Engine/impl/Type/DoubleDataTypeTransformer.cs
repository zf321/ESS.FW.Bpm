using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     Transform values of type <seealso cref="decimal" /> and <seealso cref="string" /> into
    ///     <seealso cref="DoubleValue" />.
    ///     
    /// </summary>
    public class DoubleDataTypeTransformer : IDmnDataTypeTransformer
    {

        public virtual ITypedValue Transform(object value)
        {
            if (value is decimal)
            {
                var doubleValue = TransformNumber((decimal)value);
                return Variables.DoubleValue(doubleValue);
            }
            if (value is string)
            {
                var doubleValue = TransformString((string)value);
                return Variables.DoubleValue(doubleValue);
            }
            return null;
        }

        protected internal virtual double TransformNumber(decimal value)
        {
            return (double) value;
        }

        protected internal virtual double TransformString(string value)
        {
            return Convert.ToDouble(value);
        }
    }
}