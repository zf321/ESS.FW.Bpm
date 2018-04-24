using System;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;


namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    

	/// <summary>
	/// 
	/// 
	/// </summary>
	public abstract class PrimitiveValueSerializer<T> : AbstractTypedValueSerializer<T> where T : ITypedValue
    {

	  public PrimitiveValueSerializer(IPrimitiveValueType variableType) : base(variableType)
	  {
	  }

	  public override string Name
	  {
		  get
		  {
			// default implementation returns the name of the type. This is OK since we assume that
			// there is only a single serializer for a primitive variable type.
			// If multiple serializers exist for the same type, they must override
			// this method and return distinct values.
			return valueType.Name;
		  }
	  }

	  public override ITypedValue ReadValue(IValueFields valueFields, bool deserializeObjectValue)
	  {
		// primitive values are always deserialized
		return ReadValue(valueFields);
	  }

	  public abstract T ReadValue(IValueFields valueFields);

	  public override IValueType Type
	  {
		  get
		  {
			return (IPrimitiveValueType) base.Type;
		  }
	  }

	  protected internal override bool CanWriteValue(ITypedValue typedValue)
	  {
		object value = typedValue.Value;
		Type javaType = ((IPrimitiveValueType)Type).NetType;

		return value == null || javaType.IsAssignableFrom(value.GetType());
	  }

	}

}