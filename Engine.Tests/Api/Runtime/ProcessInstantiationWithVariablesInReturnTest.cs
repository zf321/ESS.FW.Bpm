using System.IO;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Variable;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    ///     Represents the test class for the process instantiation on which
    ///     the process instance is returned with variables.
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
    [TestFixture]
    public class ProcessInstantiationWithVariablesInReturnTest
    {
        protected internal const string SUBPROCESS_PROCESS =
            "resources/api/runtime/ProcessInstanceModificationTest.Subprocess.bpmn20.xml";

        protected internal const string SET_VARIABLE_IN_DELEGATE_PROCESS =
            "resources/api/runtime/ProcessInstantiationWithVariablesInReturn.SetVariableInDelegate.bpmn20.xml";

        protected internal const string SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS =
            "resources/api/runtime/ProcessInstantiationWithVariablesInReturn.SetVariableInDelegateWithWaitState.bpmn20.xml";

        protected internal const string SIMPLE_PROCESS =
            "resources/api/runtime/ProcessInstantiationWithVariablesInReturn.simpleProcess.bpmn20.xml";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain chain;


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        private readonly bool InstanceFieldsInitialized;
        public ProcessEngineTestRule testHelper;

        public ProcessInstantiationWithVariablesInReturnTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            //chain = RuleChain.outerRule(engineRule)
            //.around(testHelper);
        }

        private void checkVariables(IVariableMap map, int expectedSize)
        {
            var variables = engineRule.HistoryService.CreateHistoricVariableInstanceQuery()
                /*.OrderByVariableName()*/
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(expectedSize, variables.Count);

            Assert.AreEqual(variables.Count, map.Count);
            foreach (var instance in variables)
            {
                Assert.True(map.ContainsKey(instance.Name));
                var instanceValue = instance.TypedValue.Value;
                var mapValue = map.GetValueTyped<ITypedValue>(instance.Name)
                    .Value;
                if (instanceValue == null)
                    Assert.IsNull(mapValue);
                else if (instanceValue is byte[])
                    Assert.Equals((byte[]) instanceValue, (byte[]) mapValue);
                else
                    Assert.AreEqual(instanceValue, mapValue);
            }
        }

        [Test]
        private void testVariablesWithoutDesrialization(string processDefinitionKey)
        {
            //given serializable variable
            //JavaSerializable javaSerializable = new JavaSerializable("foo");

            var baos = new MemoryStream();
            //new ObjectOutputStream(baos).WriteObject(javaSerializable);
            //string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos),
            //engineRule.ProcessEngine);

            //when execute process with serialized variable and wait state
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            //ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.CreateProcessInstanceByKey(
            //        processDefinitionKey)
            //    .SetVariable("serializedVar", serializedObjectValue(serializedObject)
            //        .serializationDataFormat(Variables.SerializationDataFormats.JAVA)
            //        .objectTypeName(typeof(JavaSerializable).FullName)
            //        .Create())
            //    .ExecuteWithVariablesInReturn(false, false);

            //then returned instance contains serialized variable
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            //var serializedVar = (IObjectValue) map.GetValueTyped("serializedVar");
            //Assert.IsFalse(serializedVar.Deserialized);
            ////AssertObjectValueSerializedJava(serializedVar, javaSerializable);

            ////access on value should Assert.Fail because variable is not deserialized
            //try
            //{
            //    var serializedVarValue = serializedVar.Value;
            //    Assert.Fail("Deserialization should Assert.Fail!");
            //}
            //catch (InvalidOperationException ise)
            //{
            //    Assert.True(ise.Message.Equals("Object is not deserialized."));
            //}
        }

        [Test][Deployment(SIMPLE_PROCESS)]
        public virtual void testReturnVariablesFromStartWithoutDeserialization()
        {
            testVariablesWithoutDesrialization("simpleProcess");
        }

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test Deployment(new string[] SUBPROCESS_PROCESS) public void testReturnVariablesFromStartWithoutDeserializationWithWaitstate() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void testReturnVariablesFromStartWithoutDeserializationWithWaitstate()
        {
            testVariablesWithoutDesrialization("subprocess");
        }

        [Test]
        [Deployment(SIMPLE_PROCESS)]
        public virtual void testReturnVariablesFromStart()
        {
            //given execute process with variables
            //IProcessInstanceWithVariables procInstance = engineRule.RuntimeService.CreateProcessInstanceByKey(
            //        "simpleProcess")
            //    .SetVariable("aVariable1", "aValue1")
            //    .SetVariableLocal("aVariable2", "aValue2")
            //    .SetVariables(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable3", "aValue3"))
            //    .SetVariablesLocal(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable4", new byte[] {127, 34, 64}))
            //    .ExecuteWithVariablesInReturn(false, false);

            ////when returned instance contains variables
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            //// then variables equal to variables which are accessible via query
            //checkVariables(map, 4);
        }

        [Test]
        [Deployment(SUBPROCESS_PROCESS)]
        public virtual void testReturnVariablesFromStartWithWaitstate()
        {
            //given execute process with variables and wait state
            //IProcessInstanceWithVariables procInstance = engineRule.RuntimeService.CreateProcessInstanceByKey(
            //        "subprocess")
            //    .SetVariable("aVariable1", "aValue1")
            //    .SetVariableLocal("aVariable2", "aValue2")
            //    .SetVariables(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable3", "aValue3"))
            //    .SetVariablesLocal(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable4", new byte[] {127, 34, 64}))
            //    .ExecuteWithVariablesInReturn(false, false);

            ////when returned instance contains variables
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            //// then variables equal to variables which are accessible via query
            //checkVariables(map, 4);
        }

        [Test]
        [Deployment(SUBPROCESS_PROCESS)]
        public virtual void testReturnVariablesFromStartWithWaitstateStartInSubProcess()
        {
            //given execute process with variables and wait state in sub process
            //ProcessInstanceWithVariables procInstance = engineRule.RuntimeService.CreateProcessInstanceByKey(
            //        "subprocess")
            //    .SetVariable("aVariable1", "aValue1")
            //    .SetVariableLocal("aVariable2", "aValue2")
            //    .SetVariables(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable3", "aValue3"))
            //    .SetVariablesLocal(Variable.Variables.CreateVariables()
            //        .PutValue("aVariable4", new byte[] {127, 34, 64}))
            //    .StartBeforeActivity("innerTask")
            //    .ExecuteWithVariablesInReturn(true, true);

            ////when returned instance contains variables
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            //// then variables equal to variables which are accessible via query
            //checkVariables(map, 4);
        }

        [Test]
        [Deployment(SET_VARIABLE_IN_DELEGATE_PROCESS)]
        public virtual void testReturnVariablesFromExecution()
        {
            //given executed process which sets variables in java delegate
            var procInstance =
                engineRule.RuntimeService.CreateProcessInstanceByKey("variableProcess")
                    .ExecuteWithVariablesInReturn();
            //when returned instance contains variables
            var map = procInstance.Variables;
            Assert.NotNull(map);

            // then variables equal to variables which are accessible via query
            checkVariables(map, 8);
        }
        [Test]
        [Deployment(SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS)]
        public virtual void testReturnVariablesFromExecutionWithWaitstate()
        {
            //given executed process which sets variables in java delegate
            var procInstance =
                engineRule.RuntimeService.CreateProcessInstanceByKey("variableProcess")
                    .ExecuteWithVariablesInReturn();
            //when returned instance contains variables
            var map = procInstance.Variables;
            Assert.NotNull(map);

            // then variables equal to variables which are accessible via query
            checkVariables(map, 8);
        }

        [Test]
        [Deployment(SET_VARIABLE_IN_DELEGATE_PROCESS)]
        public virtual void testReturnVariablesFromStartAndExecution()
        {
            //given executed process which sets variables in java delegate
            //ProcessInstanceWithVariables procInstance =
            //    engineRule.RuntimeService.CreateProcessInstanceByKey("variableProcess")
            //        .SetVariable("aVariable1", "aValue1")
            //        .SetVariableLocal("aVariable2", "aValue2")
            //        .SetVariables(Variable.Variables.CreateVariables()
            //            .PutValue("aVariable3", "aValue3"))
            //        .SetVariablesLocal(Variable.Variables.CreateVariables()
            //            .PutValue("aVariable4", new byte[] {127, 34, 64}))
            //        .ExecuteWithVariablesInReturn();
            ////when returned instance contains variables
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            //// then variables equal to variables which are accessible via query
            //checkVariables(map, 12);
        }

        [Test]
        [Deployment(SET_VARIABLE_IN_DELEGATE_WITH_WAIT_STATE_PROCESS)]
        public virtual void testReturnVariablesFromStartAndExecutionWithWaitstate()
        {
            //given executed process which overwrites these four variables in java delegate
            // and adds four additional variables
            //IProcessInstanceWithVariables procInstance = engineRule.RuntimeService.CreateProcessInstanceByKey(
            //        "variableProcess")
            //    .SetVariable("stringVar", "aValue1")
            //    .SetVariableLocal("integerVar", 56789)
            //    .SetVariables(Variable.Variables.CreateVariables()
            //        .PutValue("longVar", 123L))
            //    .SetVariablesLocal(Variable.Variables.CreateVariables()
            //        .PutValue("byteVar", new byte[] {127, 34, 64}))
            //    .ExecuteWithVariablesInReturn(false, false);
            //when returned instance contains variables
            //IVariableMap map = procInstance.Variables;
            //Assert.NotNull(map);

            // then variables equal to variables which are accessible via query
            //checkVariables(map, 8);
        }
    }
}