using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;



namespace ESS.FW.Bpm.Engine.Variable.Type
{
    /// <summary>
	/// </summary>
	public interface ISerializableValueType : IValueType
	{

	  /// <summary>
	  /// Identifies the object's java type name.
	  /// </summary>

	  /// <summary>
	  /// Identifies the format in which the object is serialized.
	  /// </summary>


	  /// <summary>
	  /// Creates a new TypedValue using this type. </summary>
	  /// <param name="serializedValue"> the value in serialized form </param>
	  /// <returns> the typed value for the value </returns>
	  ISerializableValue CreateValueFromSerialized(string serializedValue, IDictionary<string, object> valueInfo);

	}

	public static class SerializableValueTypeFields
	{
	  public const string ValueInfoObjectTypeName = "objectTypeName";
	  public const string ValueInfoSerializationDataFormat = "serializationDataFormat";
	}

}