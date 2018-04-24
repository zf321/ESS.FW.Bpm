
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
	/// </summary>
	public class DoubleValueSerializer : PrimitiveValueSerializer<IDoubleValue>
	{

	  public DoubleValueSerializer() : base(ValueTypeFields.Double)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.DoubleValue((double?) untypedValue.Value);
	  }

	  public override void WriteValue(ITypedValue value, IValueFields valueFields)
	  {
		valueFields.DoubleValue = (double?) value.Value;
	  }

	  public override IDoubleValue ReadValue(IValueFields valueFields)
	  {
		return Variables.DoubleValue(valueFields.DoubleValue);
	  }

	}

}