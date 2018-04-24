using System;
using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;
using ESS.FW.Bpm.Engine.Impl.util;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Variable.Serializer
{
    /// <summary>
    /// </summary>
    public abstract class AbstractSerializableValueSerializer<T> : AbstractTypedValueSerializer<T>
        where T : ISerializableValue
    {
        protected internal string serializationDataFormat;

        public AbstractSerializableValueSerializer(ISerializableValueType type, string serializationDataFormat)
            : base(type)
        {
            this.serializationDataFormat = serializationDataFormat;
        }

        public override string SerializationDataFormat
        {
            get { return serializationDataFormat; }
        }

        /// <summary>
        ///     Return true if the serialization is text based. Return false otherwise
        /// </summary>
        protected internal abstract bool SerializationTextBased { get; }

        public override void WriteValue(ITypedValue value, IValueFields valueFields)
        {
            var v = (T) value;
            var serializedStringValue = v.ValueSerialized;
            byte[] serializedByteValue = null;

            if (v.IsDeserialized)
            {
                var objectToSerialize = value.Value;
                if (objectToSerialize != null)
                    try
                    {
                        serializedByteValue = SerializeToByteArray(objectToSerialize);
                        serializedStringValue = GetSerializedStringValue(serializedByteValue);
                    }
                    catch (System.Exception e)
                    {
                        throw new ProcessEngineException(
                            "Cannot serialize object in variable '" + valueFields.Name + "': " + e.Message, e);
                    }
            }
            else
            {
                if (!ReferenceEquals(serializedStringValue, null))
                    serializedByteValue = GetSerializedBytesValue(serializedStringValue);
            }

            // write value and type to fields.
            WriteToValueFields(v, valueFields, serializedByteValue);

            // update the ObjectValue to keep it consistent with value fields.
            UpdateTypedValue(v, serializedStringValue);
        }

        public override ITypedValue ReadValue(IValueFields valueFields, bool deserializeObjectValue)
        {
            var serializedByteValue = ReadSerializedValueFromFields(valueFields);
            var serializedStringValue = GetSerializedStringValue(serializedByteValue);

            if (deserializeObjectValue)
            {
                object deserializedObject = null;
                if (serializedByteValue != null)
                    try
                    {
                        deserializedObject = DeserializeFromByteArray(serializedByteValue, valueFields);
                    }
                    catch (System.Exception e)
                    {
                        throw new ProcessEngineException(
                            "Cannot deserialize object in variable '" + valueFields.Name + "': " + e.Message, e);
                    }
                var value = CreateDeserializedValue(deserializedObject, serializedStringValue, valueFields);
                return value;
            }
            return CreateSerializedValue(serializedStringValue, valueFields);
        }

        protected internal abstract T CreateDeserializedValue(object deserializedObject, string serializedStringValue,
            IValueFields valueFields);

        protected internal abstract T CreateSerializedValue(string serializedStringValue, IValueFields valueFields);

        protected internal abstract void WriteToValueFields(T value, IValueFields valueFields, byte[] serializedValue);

        protected internal abstract void UpdateTypedValue(T value, string serializedStringValue);

        protected internal virtual byte[] ReadSerializedValueFromFields(IValueFields valueFields)
        {
            if (valueFields.ByteArrayValue == null&&!string.IsNullOrEmpty(valueFields.ByteArrayId))
            {
                if (context.Impl.Context.CommandContext != null)
                {
                    var bytearr = context.Impl.Context.CommandContext.ByteArrayManager.Get(valueFields.ByteArrayId);
                    valueFields.ByteArrayValue = bytearr.Bytes;
                }
                else
                {
                    using(IScope scope = ObjectContainer.BeginLifetimeScope())
                    {
                        IByteArrayManager manager= scope.Resolve<IByteArrayManager>();
                        var bytearr = manager.Get(valueFields.ByteArrayId);
                        valueFields.ByteArrayValue = bytearr.Bytes;
                    }
                }
            }
                return valueFields.ByteArrayValue;
            
        }

        protected internal virtual string GetSerializedStringValue(byte[] serializedByteValue)
        {
            if (serializedByteValue != null)
            {
                if (!SerializationTextBased)
                    return Convert.ToBase64String(serializedByteValue);
            }
            return null;
        }

        protected internal virtual byte[] GetSerializedBytesValue(string serializedStringValue)
        {
            if (!ReferenceEquals(serializedStringValue, null))
            {
                var serializedByteValue = StringUtil.ToByteArray(serializedStringValue);
                if (!SerializationTextBased)
                    //serializedByteValue = Convert.ToBase64CharArray(serializedByteValue);
                return serializedByteValue;
            }
            return null;
        }

        protected internal override bool CanWriteValue(ITypedValue typedValue)
        {
            object objectToSerialize = null;
            string requestedDataformat = null;

            if (typedValue is UntypedValueImpl)
            {
                objectToSerialize = typedValue.Value;
                requestedDataformat = null;
            }
            else if (typedValue is ISerializableValue)
            {
                var serializableValue = (ISerializableValue) typedValue;
                var requestedDataFormat = serializableValue.SerializationDataFormat;

                if (!serializableValue.IsDeserialized)
                    return serializationDataFormat.Equals(requestedDataFormat);
                objectToSerialize = typedValue.Value;
                requestedDataformat = serializableValue.SerializationDataFormat;
            }
            else
            {
                // not an object value
                return false;
            }

            var canSerialize = objectToSerialize == null || CanSerializeValue(objectToSerialize);

            if (!ReferenceEquals(requestedDataformat, null))
                if (requestedDataformat.Equals(serializationDataFormat))
                    return canSerialize;
                else
                    return false;
            return canSerialize;
        }


        /// <summary>
        ///     return true if this serializer is able to serialize the provided object.
        /// </summary>
        /// <param name="value"> the object to test (guaranteed to be a non-null value) </param>
        /// <returns> true if the serializer can handle the object. </returns>
        protected internal abstract bool CanSerializeValue(object value);

        // methods to be implemented by subclasses ////////////

        /// <summary>
        ///     Implementations must return a byte[] representation of the provided object.
        ///     The object is guaranteed not to be null.
        /// </summary>
        /// <param name="deserializedObject"> the object to serialize </param>
        /// <returns> the byte array value of the object </returns>
        /// <exception cref="exception"> in case the object cannot be serialized </exception>
        protected internal abstract byte[] SerializeToByteArray(object deserializedObject);

        /// <summary>
        ///     Deserialize the object from a byte array.
        /// </summary>
        /// <param name="object"> the object to deserialize </param>
        /// <param name="valueFields"> the value fields </param>
        /// <returns> the deserialized object </returns>
        /// <exception cref="exception"> in case the object cannot be deserialized </exception
        protected internal abstract object DeserializeFromByteArray(byte[] @object, IValueFields valueFields);
    }
}