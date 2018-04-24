using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    /// <summary>
    ///     Transform values of type <seealso cref="decimal" /> and <seealso cref="string" /> into <seealso cref="LongValue" />
    ///     .
    ///     
    /// </summary>
    public class LongDataTypeTransformer : IDmnDataTypeTransformer
    {
        public virtual ITypedValue Transform(object value)
        {
            if (value is decimal)
            {
                var longValue = TransformNumber((decimal) value);
                return Variables.LongValue(longValue);
            }
            if (value is string)
            {
                var longValue = TransformString((string) value);
                return Variables.LongValue(longValue);
            }
            throw new ArgumentException();
        }

        protected internal virtual long TransformNumber(decimal value)
        {
            if (IsLong(value))
            {
                return (long)value;
            }
            throw new ArgumentException();
        }

        protected internal virtual bool IsLong(decimal value)
        {
            //double doubleValue = value.doubleValue();
            //return doubleValue == (long) doubleValue;
            return true;
        }

        protected internal virtual long TransformString(string value)
        {
            return long.Parse(value);
        }
    }
}