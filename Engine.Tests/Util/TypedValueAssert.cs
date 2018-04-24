using System.IO;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Util
{
    /// <summary>
    /// </summary>
    public class TypedValueAssert
    {
        public static void AssertObjectValueDeserializedNull(IObjectValue typedValue)
        {
            Assert.NotNull(typedValue);
            Assert.True(typedValue.IsDeserialized);
            Assert.NotNull(typedValue.SerializationDataFormat);
            Assert.IsNull(typedValue.Value);
            Assert.IsNull(typedValue.ValueSerialized);
            Assert.IsNull(typedValue.ObjectType);
            Assert.IsNull(typedValue.ObjectTypeName);
        }

        public static void AssertObjectValueSerializedNull(IObjectValue typedValue)
        {
            Assert.NotNull(typedValue);
            Assert.IsFalse(typedValue.IsDeserialized);
            Assert.NotNull(typedValue.SerializationDataFormat);
            Assert.IsNull(typedValue.ValueSerialized);
            Assert.IsNull(typedValue.ObjectTypeName);
        }

        public static void AssertObjectValueDeserialized(IObjectValue typedValue, object value)
        {
            var expectedObjectType = value.GetType();
            Assert.True(typedValue.IsDeserialized);

            Assert.AreEqual(ValueTypeFields.Object, typedValue.Type);

            Assert.AreEqual(value, typedValue.Value);
            Assert.AreEqual(value, typedValue.GetValue(expectedObjectType));

            Assert.AreEqual(expectedObjectType, typedValue.ObjectType);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            Assert.AreEqual(expectedObjectType.FullName, typedValue.ObjectTypeName);
        }

        public static void AssertObjectValueSerializedJava(IObjectValue typedValue, object value)
        {
            Assert.AreEqual(Variables.SerializationDataFormats.Net, typedValue.SerializationDataFormat);

            try
            {
                //TODO 
                // validate this is the base 64 encoded string representation of the serialized value of the java object
                var valueSerialized = typedValue.ValueSerialized;
                //byte[] decodedObject = Base64.DecodeBase64(valueSerialized.GetBytes(Encoding.UTF8));
                //ObjectInputStream objectInputStream = new ObjectInputStream(new MemoryStream(decodedObject));
                //Assert.AreEqual(value, objectInputStream.ReadObject());
            }
            catch (IOException e)
            {
                throw;
            }
            //catch (ClassNotFoundException e)
            //{
            //  throw new Exception(e);
            //}
        }

        public static void AssertUntypedNullValue(ITypedValue nullValue)
        {
            Assert.NotNull(nullValue);
            Assert.IsNull(nullValue.Value);
            Assert.AreEqual(ValueTypeFields.Null, nullValue.Type);
        }
    }
}