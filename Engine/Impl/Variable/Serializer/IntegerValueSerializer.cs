
using System;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class IntegerValueSerializer : PrimitiveValueSerializer<IIntegerValue>
	{

	  public IntegerValueSerializer() : base(ValueTypeFields.Integer)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.IntegerValue((int?) untypedValue.Value);
	  }

	  public override void WriteValue(ITypedValue variableValue, IValueFields valueFields)
	  {
		int? value = (int?) variableValue.Value;

		if (value != null)
		{
		  valueFields.LongValue = ((int?) value).Value;
		  valueFields.TextValue = value.ToString();
		}
		else
		{
		  valueFields.LongValue = null;
		  valueFields.TextValue = null;
		}

	  }

	  public override IIntegerValue ReadValue(IValueFields valueFields)
	  {
		int? intValue = null;

		if (valueFields.LongValue != null)
		{
		  intValue = Convert.ToInt32(valueFields.LongValue.Value);
		}

		return Variables.IntegerValue(intValue);
	  }

	}

}