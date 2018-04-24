using System;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;


namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{

	/// <summary>
	/// Serializes Dates as long values
	/// 
	/// 
	/// 
	/// </summary>
	public class DateValueSerializer : PrimitiveValueSerializer<IDateValue>
	{

	  public DateValueSerializer() : base(ValueTypeFields.Date)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return (ITypedValue) Variables.DateValue((DateTime) untypedValue.Value);
	  }

	  public override IDateValue ReadValue(IValueFields valueFields)
	  {
		long? longValue = valueFields.LongValue;
		DateTime? dateValue = null;
		if (longValue != null)
		{
		  dateValue = new DateTime(longValue.Value);
		}
		return Variables.DateValue(dateValue.Value);
	  }

	  public override void WriteValue(ITypedValue typedValue, IValueFields valueFields)
	  {
		DateTime dateValue = (DateTime) typedValue.Value;
		if (dateValue != null)
		{
		  valueFields.LongValue = dateValue.Ticks;
		}
		else
		{
		  valueFields.LongValue = null;
		}
	  }

	}

}