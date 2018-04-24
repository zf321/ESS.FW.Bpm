using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public class LongValueSerlializer : PrimitiveValueSerializer<ILongValue>
    {
        public LongValueSerlializer() : base(ValueTypeFields.Long)
        {
        }

        public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
        {
            return Variables.LongValue((long?) untypedValue.Value);
        }

        public override ILongValue ReadValue(IValueFields valueFields)
        {
            return Variables.LongValue(valueFields.LongValue);
        }

        public override void WriteValue(ITypedValue value, IValueFields valueFields)
        {
            var longValue = value.Value;

            valueFields.LongValue = (long?) longValue;

            if (longValue != null)
                valueFields.TextValue = longValue.ToString();
            else
                valueFields.TextValue = null;
        }
    }
}