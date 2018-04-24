
using System;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    

	/// <summary>
	/// Used to serialize untyped null values.
	/// </summary>
	public class NullValueSerializer : AbstractTypedValueSerializer<NullValueImpl>
	{

	  public NullValueSerializer() : base(ValueTypeFields.Null)
	  {
	  }

	  public override string Name
	  {
		  get
		  {
			return ValueTypeFields.Null.Name.ToLower();
		  }
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		return NullValueImpl.Instance;
	  }

	  public override void WriteValue(ITypedValue value, IValueFields valueFields)
	  {
		// nothing to do
	  }

	  public override ITypedValue ReadValue(IValueFields valueFields, bool deserialize)
	  {
		return NullValueImpl.Instance;
	  }

	  protected internal override bool CanWriteValue(ITypedValue value)
	  {
		return value.Value == null;
	  }

	}

}