using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    ///     Serializes booleans as long values.
    /// </summary>
    public class BooleanValueSerializer : PrimitiveValueSerializer<IBooleanValue>
    {
        // boolean is modeled as long values
        private const long TRUE = 1L;
        private const long FALSE = 0L;

        public BooleanValueSerializer() : base(ValueTypeFields.Boolean)
        {
        }

        public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
        {
            return Variables.BooleanValue((bool?) untypedValue.Value);
        }

        public override IBooleanValue ReadValue(IValueFields valueFields)
        {
            bool? boolValue = null;
            var longValue = valueFields.LongValue;

            if (longValue != null)
                boolValue = longValue.Equals(TRUE);

            return Variables.BooleanValue(boolValue);
        }

        public override void WriteValue(ITypedValue variableValue, IValueFields valueFields)
        {
            long? longValue = null;
            var boolValue = variableValue.Value;

            if (boolValue != null)
                longValue = (bool) boolValue ? TRUE : FALSE;

            valueFields.LongValue = longValue;
        }
    }
}