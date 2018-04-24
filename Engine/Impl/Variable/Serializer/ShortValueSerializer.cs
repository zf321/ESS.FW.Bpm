
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
	public class ShortValueSerializer : PrimitiveValueSerializer<IShortValue>
	{

	  public ShortValueSerializer() : base(ValueTypeFields.Short)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return Variables.ShortValue((short?) untypedValue.Value);
	  }

	  public override IShortValue ReadValue(IValueFields valueFields)
	  {
		long? longValue = valueFields.LongValue;
		short? shortValue = null;

		if (longValue != null)
		{
		  shortValue = short.Parse(longValue.Value.ToString());
		}

		return Variables.ShortValue(shortValue);
	  }

	  public override void WriteValue(ITypedValue value, IValueFields valueFields)
	  {

		short? shortValue = (short?) value.Value;

		if (shortValue != null)
		{
		  valueFields.LongValue = shortValue.Value;
		  valueFields.TextValue = value.ToString();
		}
		else
		{
		  valueFields.LongValue = null;
		  valueFields.TextValue = null;
		}
	  }

	}

}