using System;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Common.Serializing;


namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
	/// Uses default java serialization to serialize java objects as byte streams.
	/// 
	/// </summary>
	public class ObjectSerializer : AbstractObjectValueSerializer
    {

        public const string NAME = "serializable";
        private IBinarySerializer _binarySerializer = new DefaultBinarySerializer();

        public ObjectSerializer() : base(Variables.SerializationDataFormats.Net.ToString())
        {
        }

        public override string Name
        {
            get
            {
                return NAME;
            }
        }

        protected internal override bool SerializationTextBased
        {
            get
            {
                return false;
            }
        }
        
        protected internal override object DeserializeFromByteArray(byte[] bytes, string objectTypeName)
        {
            return _binarySerializer.Deserialize(bytes,System.Type.GetType(objectTypeName));
            
        }
        protected internal override byte[] SerializeToByteArray(object deserializedObject)
        {
            return _binarySerializer.Serialize(deserializedObject);
        }

        protected internal override string GetTypeNameForDeserialized(object deserializedObject)
        {
            return deserializedObject.GetType().FullName;
        }

        protected internal override bool CanSerializeValue(object value)
        {
            return value.GetType()
                .IsSerializable;
        }
        
    }

}