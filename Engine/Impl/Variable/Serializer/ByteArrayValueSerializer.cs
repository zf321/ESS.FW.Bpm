
using System;
using ESS.FW.Bpm.Engine.Impl.Util;
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
	public class ByteArrayValueSerializer : PrimitiveValueSerializer<IBytesValue>
	{

	  public ByteArrayValueSerializer() : base(ValueTypeFields.Bytes)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		object value = untypedValue.Value;
		if (value is byte[])
		{
		  return (ITypedValue) Variables.ByteArrayValue((byte[]) value);
		}
		else
		{
		  byte[] data = IoUtil.ReadInputStream((System.IO.Stream) value, null);
		  return (ITypedValue) Variables.ByteArrayValue(data);
		}
	  }

	  public override IBytesValue ReadValue(IValueFields valueFields)
	  {
		return Variables.ByteArrayValue(valueFields.ByteArrayValue);
	  }

	  public override void WriteValue(ITypedValue variableValue, IValueFields valueFields)
	  {
		valueFields.ByteArrayValue = (byte[]) variableValue.Value;
	  }

	  protected internal override bool CanWriteValue(ITypedValue typedValue)
	  {
		return base.CanWriteValue(typedValue) || typedValue.Value is System.IO.Stream;
	  }

	}

}