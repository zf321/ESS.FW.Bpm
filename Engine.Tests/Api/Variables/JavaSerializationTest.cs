using System.IO;
using System.Text;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Digest._apacheCommonsCodec;
using ESS.FW.Bpm.Engine.Impl.util;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{
    [TestFixture]
    public class JavaSerializationTest : PluggableProcessEngineTestCase
    {
        protected internal const string ONE_TASK_PROCESS =
            "org/camunda/bpm/engine/test/api/variables/oneTaskProcess.bpmn20.xml";

        protected internal static readonly string JAVA_DATA_FORMAT =
            ESS.FW.Bpm.Engine.Variable.Variables.SerializationDataFormats.Net.ToString();

        protected internal string originalSerializationFormat;

        protected internal IProcessEngine processEngine;

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testJavaObjectDeserializedInFirstCommand()
        {
            // this test makes sure that if a serialized value is set, it can be deserialized in the same command in which it is set.

            // given
            // a serialized Java Object
            var javaSerializable = new JavaSerializable("foo");
            var baos = new MemoryStream();
            //(new ObjectOutputStream(baos)).WriteObject(javaSerializable);
            var serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.GetBuffer()), processEngine);

            // if
            // I start a process instance in which a Java Delegate reads the value in its deserialized form
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("varName", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue(serializedObject)
                    .SerializationDataFormat(JAVA_DATA_FORMAT)
                    //.objectTypeName(typeof(JavaSerializable).FullName).Create()));
                    .GetType()
                    .Assembly));
            // then
            // it does not Assert.Fail
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testJavaObjectNotDeserializedIfNotRequested()
        {
            // this test makes sure that if a serialized value is set, it is not automatically deserialized if deserialization is not requested

            // given
            // a serialized Java Object
            var javaSerializable = new FailingJavaSerializable("foo");
            var baos = new MemoryStream();
            //  (new ObjectOutputStream(baos)).WriteObject(javaSerializable);
            // baos.Write(javaSerializable,0, baos.Length);
            var serializedObjectBytes = baos.ToArray();
            var serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(serializedObjectBytes), processEngine);

            // which cannot be deserialized
            try
            {
                var objectInputStream = new MemoryStream(serializedObjectBytes);
                objectInputStream.GetBuffer();
                Assert.Fail("Exception expected");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Exception while deserializing object", e.Message);
            }

            // if
            // I start a process instance in which a Java Delegate reads the value in its serialized form
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            runtimeService.StartProcessInstanceByKey("oneTaskProcess", ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                .PutValue("varName", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue(serializedObject)
                    .SerializationDataFormat(JAVA_DATA_FORMAT)
                    //.ObjectTypeName(typeof(JavaSerializable).FullName).Create()));
                    .GetType()
                    .Assembly));
            // then
            // it does not Assert.Fail
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSerializationAsJava()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var javaSerializable = new JavaSerializable("foo");
            runtimeService.SetVariable(instance.Id, "simpleBean", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(javaSerializable)
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .Create());

            // validate untyped value
            var value = (JavaSerializable) runtimeService.GetVariable(instance.Id, "simpleBean");
            Assert.AreEqual(javaSerializable, value);

            // validate typed value
            var typedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "simpleBean");
            TypedValueAssert.AssertObjectValueDeserialized(typedValue, javaSerializable);
            TypedValueAssert.AssertObjectValueSerializedJava(typedValue, javaSerializable);
        }


        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSetJavaOjectNullDeserialized()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // set null value as "deserialized" object
            runtimeService.SetVariable(instance.Id, "nullObject", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(null)
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .Create());

            // get null value via untyped api
            Assert.IsNull(runtimeService.GetVariable(instance.Id, "nullObject"));

            // get null via typed api
            var typedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "nullObject");
            TypedValueAssert.AssertObjectValueDeserializedNull(typedValue);
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSetJavaOjectNullSerialized()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // set null value as "serialized" object
            runtimeService.SetVariable(instance.Id, "nullObject", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue()
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .Create()); // Note: no object type name provided

            // get null value via untyped api
            Assert.IsNull(runtimeService.GetVariable(instance.Id, "nullObject"));

            // get null via typed api
            var deserializedTypedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "nullObject");
            TypedValueAssert.AssertObjectValueDeserializedNull(deserializedTypedValue);

            var serializedTypedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "nullObject", false);
            TypedValueAssert.AssertObjectValueSerializedNull(serializedTypedValue);
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSetJavaOjectNullSerializedObjectTypeName()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var typeName = "some.Type.Name";

            // set null value as "serialized" object
            runtimeService.SetVariable(instance.Id, "nullObject", ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue()
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .GetType()
                .Assembly);
            // This time an objectTypeName is provided

            // get null value via untyped api
            Assert.IsNull(runtimeService.GetVariable(instance.Id, "nullObject"));

            // get null via typed api
            var deserializedTypedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "nullObject");
            Assert.NotNull(deserializedTypedValue);
            Assert.True(deserializedTypedValue.IsDeserialized);
            Assert.AreEqual(JAVA_DATA_FORMAT, deserializedTypedValue.SerializationDataFormat);
            Assert.IsNull(deserializedTypedValue.Value);
            Assert.IsNull(deserializedTypedValue.ValueSerialized);
            Assert.IsNull(deserializedTypedValue.ObjectType);
            Assert.AreEqual(typeName, deserializedTypedValue.ObjectTypeName);

            var serializedTypedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "nullObject", false);
            Assert.NotNull(serializedTypedValue);
            Assert.IsFalse(serializedTypedValue.IsDeserialized);
            Assert.AreEqual(JAVA_DATA_FORMAT, serializedTypedValue.SerializationDataFormat);
            Assert.IsNull(serializedTypedValue.ValueSerialized);
            Assert.AreEqual(typeName, serializedTypedValue.ObjectTypeName);
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSetJavaOjectSerialized()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            var javaSerializable = new JavaSerializable("foo");


            var baos = new MemoryStream(Encoding.UTF8.GetBytes(javaSerializable.Property));
            // baos.WriteTo(javaSerializable);//ObjectOutputStream
            //  (new ObjectOutputStream(baos.GetBuffer(),FileMode.OpenOrCreate)).WriteObject(javaSerializable);
            var serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.GetBuffer()), processEngine);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            runtimeService.SetVariable(instance.Id, "simpleBean",
                ESS.FW.Bpm.Engine.Variable.Variables.SerializedObjectValue(serializedObject)
                    .SerializationDataFormat(JAVA_DATA_FORMAT)
                    // .objectTypeName(typeof(JavaSerializable).FullName).Create());
                    .GetType()
                    .Assembly);

            var value = (JavaSerializable) runtimeService.GetVariable(instance.Id, "simpleBean");
            Assert.AreEqual(javaSerializable, value);

            // validate typed value
            var typedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "simpleBean");
            TypedValueAssert.AssertObjectValueDeserialized(typedValue, javaSerializable);
            //ObjectValueSerializedJava(typedValue, javaSerializable);
        }


        [Test]
        public virtual void testSetTypedNullForExistingVariable()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // initially the variable has a value
            var javaSerializable = new JavaSerializable("foo");

            runtimeService.SetVariable(instance.Id, "varName", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(javaSerializable)
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .Create());

            // get value via untyped api
            Assert.AreEqual(javaSerializable, runtimeService.GetVariable(instance.Id, "varName"));

            // set the variable to null via typed Api
            runtimeService.SetVariable(instance.Id, "varName", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(null));

            // variable is still of type object
            var typedValue = runtimeService.GetVariableTyped<IObjectValue>(instance.Id, "varName");
            TypedValueAssert.AssertObjectValueDeserializedNull(typedValue);
        }

        [Test]
        [Deployment(ONE_TASK_PROCESS)]
        public virtual void testSetUntypedNullForExistingVariable()
        {
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // initially the variable has a value
            var javaSerializable = new JavaSerializable("foo");

            runtimeService.SetVariable(instance.Id, "varName", ESS.FW.Bpm.Engine.Variable.Variables.ObjectValue(javaSerializable)
                .SerializationDataFormat(JAVA_DATA_FORMAT)
                .Create());

            // get value via untyped api
            Assert.AreEqual(javaSerializable, runtimeService.GetVariable(instance.Id, "varName"));

            // set the variable to null via untyped Api
            runtimeService.SetVariable(instance.Id, "varName", null);

            // variable is now untyped null
            var nullValue = runtimeService.GetVariableTyped<ITypedValue>(instance.Id, "varName");
            TypedValueAssert.AssertUntypedNullValue(nullValue);
        }
    }
}