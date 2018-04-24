using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable.Type;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime.Util
{
    /// <summary>
    /// </summary>
    public class AssertVariableInstancesDelegate : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            // validate integer variable
            int? expectedIntValue = 1234;
            Assert.AreEqual(expectedIntValue, execution.GetVariable("anIntegerVariable"));
            Assert.AreEqual(expectedIntValue, execution.GetVariableTyped<int>("anIntegerVariable")
                /*.Value*/);
            Assert.AreEqual(ValueTypeFields.Integer, execution.GetVariableTyped<int>("anIntegerVariable")
                /*.Type*/);
            Assert.IsNull(execution.GetVariableLocal("anIntegerVariable"));
            Assert.IsNull(execution.GetVariableLocalTyped<int>("anIntegerVariable"));

            // set an additional local variable
            execution.SetVariableLocal("aStringVariable", "aStringValue");

            var expectedStringValue = "aStringValue";
            Assert.AreEqual(expectedStringValue, execution.GetVariable("aStringVariable"));
            //Assert.AreEqual(expectedStringValue, execution.GetVariableTyped("aStringVariable")
            //    .Value);
            //Assert.AreEqual(ValueTypeFields.String, execution.GetVariableTyped("aStringVariable")
            //    .Type);
            //Assert.AreEqual(expectedStringValue, execution.GetVariableLocal("aStringVariable"));
            //Assert.AreEqual(expectedStringValue, execution.GetVariableLocalTyped("aStringVariable")
            //    .Value);
            //Assert.AreEqual(ValueTypeFields.String, execution.GetVariableLocalTyped("aStringVariable")
            //    .Type);

            //var objectValue = (SimpleSerializableBean) execution.GetVariable("anObjectValue");
            //Assert.NotNull(objectValue);
            //Assert.AreEqual(10, objectValue.IntProperty);
            //IObjectValue variableTyped = execution.GetVariableTyped("anObjectValue");
            //Assert.AreEqual(10, variableTyped.GetValue(typeof(SimpleSerializableBean))
            //    .IntProperty);
            //Assert.AreEqual(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);

            //objectValue = (SimpleSerializableBean) execution.GetVariable("anUntypedObjectValue");
            //Assert.NotNull(objectValue);
            //Assert.AreEqual(30, objectValue.IntProperty);
            //variableTyped = execution.GetVariableTyped("anUntypedObjectValue");
            //Assert.AreEqual(30, variableTyped.GetValue(typeof(SimpleSerializableBean))
            //    .IntProperty);
            //Assert.AreEqual(Context.ProcessEngineConfiguration.DefaultSerializationFormat,
            //    variableTyped.SerializationDataFormat);
        }
    }
}