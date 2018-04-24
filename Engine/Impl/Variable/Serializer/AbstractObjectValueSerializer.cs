
using System;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{

	/// <summary>
	/// Abstract implementation of a <seealso cref="ITypedValueSerializer{T}"/> for <seealso cref="ObjectValue ObjectValues"/>.
	/// 
	/// 
	/// 
	/// </summary>
	public abstract class AbstractObjectValueSerializer : AbstractSerializableValueSerializer<IObjectValue>
	{

	  public AbstractObjectValueSerializer(string serializationDataFormat) : base(ValueTypeFields.Object, serializationDataFormat)
	  {
	  }

	  public override ITypedValue ConvertToTypedValue(UntypedValueImpl untypedValue)
	  {
		// untyped values are always deserialized
		return (ITypedValue)Variables.ObjectValue(untypedValue.Value).Create();
	  }

	  protected internal override void WriteToValueFields(IObjectValue value, IValueFields valueFields, byte[] serializedValue)
	  {
		string objectTypeName = GetObjectTypeName(value, valueFields);
		valueFields.ByteArrayValue=(serializedValue);
            valueFields.TextValue2 = (objectTypeName);
		
	  }

	  protected internal virtual string GetObjectTypeName(IObjectValue value, IValueFields valueFields)
	  {
            string objectTypeName = value.Type.Name;//.ObjectTypeName;

		if (string.ReferenceEquals(objectTypeName, null) && !value.IsDeserialized && value.ValueSerialized != null)
		{
		  throw new ProcessEngineException("Cannot write serialized value for variable '" + valueFields.Name + "': no 'objectTypeName' provided for non-null value.");
		}

		// update type name if the object is deserialized
		if (value.IsDeserialized && value.Value != null)
		{
		  objectTypeName = GetTypeNameForDeserialized(value.Value);
		}

		return objectTypeName;
	  }

	  protected internal override void UpdateTypedValue(IObjectValue value, string serializedStringValue)
	  {
		string objectTypeName = GetObjectTypeName(value, null);
		ObjectValueImpl objectValue = (ObjectValueImpl) value;
		objectValue.ObjectTypeName = objectTypeName;
		objectValue.SerializedValue = serializedStringValue;
		objectValue.SerializationDataFormat =  SerializationDataFormat;
	  }

	  protected internal override IObjectValue CreateDeserializedValue(object deserializedObject, string serializedStringValue, IValueFields valueFields)
	  {
		string objectTypeName = ReadObjectNameFromFields(valueFields);
		return new ObjectValueImpl(deserializedObject, serializedStringValue, SerializationDataFormat, objectTypeName, true);
	  }


	  protected internal override IObjectValue CreateSerializedValue(string serializedStringValue, IValueFields valueFields)
	  {
		string objectTypeName = ReadObjectNameFromFields(valueFields);
		return new ObjectValueImpl(null, serializedStringValue, SerializationDataFormat, objectTypeName, false);
	  }

	  protected internal virtual string ReadObjectNameFromFields(IValueFields valueFields)
	  {
		return valueFields.TextValue2;
	  }

	  public virtual bool IsMutableValue(IObjectValue typedValue)
	  {
		return typedValue.IsDeserialized;
	  }

	  // methods to be implemented by subclasses ////////////

	  /// <summary>
	  /// Returns the type name for the deserialized object.
	  /// </summary>
	  /// <param name="deserializedObject."> Guaranteed not to be null </param>
	  /// <returns> the type name fot the object. </returns>
	  protected internal abstract string GetTypeNameForDeserialized(object deserializedObject);

	  /// <summary>
	  /// Implementations must return a byte[] representation of the provided object.
	  /// The object is guaranteed not to be null.
	  /// </summary>
	  /// <param name="deserializedObject"> the object to serialize </param>
	  /// <returns> the byte array value of the object </returns>
	  /// <exception cref="exception"> in case the object cannot be serialized </exception>
	  protected internal abstract override byte[] SerializeToByteArray(object deserializedObject);
	  protected internal override object DeserializeFromByteArray(byte[] @object, IValueFields valueFields)
	  {
		string objectTypeName = ReadObjectNameFromFields(valueFields);
		return DeserializeFromByteArray(@object, objectTypeName);
	  }

	  /// <summary>
	  /// Deserialize the object from a byte array.
	  /// </summary>
	  /// <param name="object"> the object to deserialize </param>
	  /// <param name="objectTypeName"> the type name of the object to deserialize </param>
	  /// <returns> the deserialized object </returns>
	  /// <exception cref="exception"> in case the object cannot be deserialized </exception>
	  protected internal abstract object DeserializeFromByteArray(byte[] @object, string objectTypeName);

	  /// <summary>
	  /// Return true if the serialization is text based. Return false otherwise
	  /// 
	  /// </summary>
	  protected internal override abstract bool SerializationTextBased {get;}

	}

}