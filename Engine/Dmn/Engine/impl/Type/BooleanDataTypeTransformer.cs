using System;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.type;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl.type
{
    //using BooleanValue = BooleanValue;

    /// <summary>
    ///     Transform values of type <seealso cref="string" /> into <seealso cref="BooleanValue" />.
    ///     
    /// </summary>
    public class BooleanDataTypeTransformer : IDmnDataTypeTransformer
    {
        public virtual ITypedValue Transform(object value)
        {
            if (value is bool?)
            {
                return Variables.BooleanValue((bool?) value);
            }
            if (value is string)
            {
                var booleanValue = TransformString((string) value);
                return Variables.BooleanValue(booleanValue);
            }
            throw new ArgumentException();
        }

        protected internal virtual bool TransformString(string value)
        {
            if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                return true;
            if (value.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                return false;
            throw new ArgumentException();
        }
    }
}