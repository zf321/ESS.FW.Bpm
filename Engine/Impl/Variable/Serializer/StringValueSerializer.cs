
using System;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{

	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class StringValueSerializer : PrimitiveValueSerializer<IStringValue>
	{

	  public StringValueSerializer() : base(ValueTypeFields.String)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.StringValue((string) untypedValue.Value);
	  }

	  public override IStringValue ReadValue(IValueFields valueFields)
	  {
		return Variables.StringValue(valueFields.TextValue);
	  }

	  public override void WriteValue(ITypedValue variableValue, IValueFields valueFields)
	  {
		valueFields.TextValue = (string) variableValue.Value;
	  }

	}

}