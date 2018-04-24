using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    //using IntegerValue = IntegerValue;

    /// <summary>
    ///     Transform values of type <seealso cref="decimal" /> and <seealso cref="string" /> into
    ///     <seealso cref="IntegerValue" />.
    ///     
    /// </summary>
    public class IntegerDataTypeTransformer : IDmnDataTypeTransformer
    {
        public virtual ITypedValue Transform(object value)
        {
            //if (value is decimal)
            if(value is int || value is decimal)
            {
                var intValue = TransformNumber(value);
                return Variables.IntegerValue(intValue);
            }
            else if (value is string)
            {
                var intValue = TransformString((string) value);
                return Variables.IntegerValue(intValue);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        protected internal virtual int TransformNumber(object value)
        {
            if (IsInteger(value))
            {
                return Convert.ToInt32(value);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        protected internal virtual bool IsInteger(object value)
        {
            //double doubleValue = (double) value;
            double doubleValue = Convert.ToDouble(value);
            return doubleValue == Convert.ToInt32(doubleValue);
        }

        protected internal virtual int TransformString(string value)
        {
            return int.Parse(value);
        }
    }
}