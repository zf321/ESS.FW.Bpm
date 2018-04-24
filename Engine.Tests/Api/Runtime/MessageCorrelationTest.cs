using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Api.Runtime
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class MessageCorrelationTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testCatchingMessageEventCorrelation()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            var processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            variables = new Dictionary<string, object>();
            variables["aKey"] = "anotherValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";
            IDictionary<string, object> messagePayload = new Dictionary<string, object>();
            messagePayload["aNewKey"] = "aNewVariable";

            runtimeService.CorrelateMessage(messageName, correlationKeys, messagePayload);

            var uncorrelatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "anotherValue")
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count();
            Assert.AreEqual(1, uncorrelatedExecutions);

            // the execution that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aKey", "aValue")
                //.ProcessVariableValueEquals("aNewKey", "aNewVariable")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // this time: use the builder ////////////////

            variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            processInstance = runtimeService.StartProcessInstanceByKey("process", variables);

            // use the fluent builder
            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceVariableEquals("aKey", "aValue")
                .SetVariable("aNewKey", "aNewVariable")
                .Correlate();

            uncorrelatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "anotherValue")
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count();
            Assert.AreEqual(1, uncorrelatedExecutions);

            // the execution that has been correlated should have advanced
            correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aKey", "aValue")
                //.ProcessVariableValueEquals("aNewKey", "aNewVariable")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
        }

        [Test][Deployment( "resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml") ]
        public virtual void testOneMatchinProcessInstanceUsingFluentCorrelateAll()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            variables = new Dictionary<string, object>();
            variables["aKey"] = "anotherValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            var messageName = "newInvoiceMessage";

            // use the fluent builder: correlate to first started process instance
            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceVariableEquals("aKey", "aValue")
                .SetVariable("aNewKey", "aNewVariable")
                .CorrelateAll();

            // there exists an uncorrelated executions (the second process instance)
            var uncorrelatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "anotherValue")
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count();
            Assert.AreEqual(1, uncorrelatedExecutions);

            // the execution that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aKey", "aValue")
                //.ProcessVariableValueEquals("aNewKey", "aNewVariable")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testTwoMatchingProcessInstancesCorrelation()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";

            try
            {
                runtimeService.CorrelateMessage(messageName, correlationKeys);
                Assert.Fail("Expected an Exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("2 executions match the correlation keys", e.Message);
            }

            // fluent builder fails as well
            try
            {
                runtimeService.CreateMessageCorrelation(messageName)
                    .ProcessInstanceVariableEquals("aKey", "aValue")
                    .Correlate();
                Assert.Fail("Expected an Exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("2 executions match the correlation keys", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testTwoMatchingProcessInstancesUsingFluentCorrelateAll()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.StartProcessInstanceByKey("process", variables);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";

            // fluent builder multiple should not Assert.Fail
            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceVariableEquals("aKey", "aValue")
                .SetVariable("aNewKey", "aNewVariable")
                .CorrelateAll();

            var uncorrelatedExecutions = runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count();
            Assert.AreEqual(0, uncorrelatedExecutions);

            // the executions that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aKey", "aValue")
                //.ProcessVariableValueEquals("aNewKey", "aNewVariable")
                .Count();
            Assert.AreEqual(2, correlatedExecutions);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationByBusinessKey()
        {
            var businessKey = "aBusinessKey";
            var processInstance = runtimeService.StartProcessInstanceByKey("process", businessKey);
            runtimeService.CorrelateMessage("newInvoiceMessage", businessKey);

            // the execution that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // use fluent builder //////////////////////

            processInstance = runtimeService.StartProcessInstanceByKey("process", businessKey);
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .Correlate();

            // the execution that has been correlated should have advanced
            correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationByBusinessKeyUsingFluentCorrelateAll()
        {
            var businessKey = "aBusinessKey";
            runtimeService.StartProcessInstanceByKey("process", businessKey);
            runtimeService.StartProcessInstanceByKey("process", businessKey);

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .CorrelateAll();

            // the executions that has been correlated should be in the task
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Count();
            Assert.AreEqual(2, correlatedExecutions);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testMessageCorrelateAllResultListWithResultTypeExecution()
        {
            //given
            var procInstance1 = runtimeService.StartProcessInstanceByKey("process");
            var procInstance2 = runtimeService.StartProcessInstanceByKey("process");

            //when correlated all with result
            var resultList = runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .CorrelateAllWithResult();

            Assert.AreEqual(2, resultList.Count);
            //then result should contains executions on which messages was correlated
            foreach (var result in resultList)
            {
                Assert.NotNull(result);
                Assert.AreEqual(MessageCorrelationResultType.Execution, result.ResultType);
                Assert.True(procInstance1.Id.Equals(result.Execution.ProcessInstanceId) ||
                            procInstance2.Id.Equals(result.Execution.ProcessInstanceId));
                var entity = (ExecutionEntity) result.Execution;
                Assert.AreEqual("messageCatch", entity.ActivityId);
            }
        }


        [Test][Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageCorrelateAllResultListWithResultTypeProcessDefinition()
        {
            //when correlated all with result
            var resultList = runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .CorrelateAllWithResult();

            Assert.AreEqual(1, resultList.Count);
            //then result should contains process definitions and start event activity ids on which messages was correlated
            foreach (var result in resultList)
                checkProcessDefinitionMessageCorrelationResult(result, "theStart", "messageStartEvent");
        }


        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationByBusinessKeyWithVariables()
        {
            var businessKey = "aBusinessKey";
            var processInstance = runtimeService.StartProcessInstanceByKey("process", businessKey);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            runtimeService.CorrelateMessage("newInvoiceMessage", businessKey, variables);

            // the execution that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // use fluent builder /////////////////////////

            processInstance = runtimeService.StartProcessInstanceByKey("process", businessKey);

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .SetVariable("aKey", "aValue")
                .Correlate();

            // the execution that has been correlated should have advanced
            correlatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationByBusinessKeyWithVariablesUsingFluentCorrelateAll()
        {
            var businessKey = "aBusinessKey";

            runtimeService.StartProcessInstanceByKey("process", businessKey);
            runtimeService.StartProcessInstanceByKey("process", businessKey);

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .SetVariable("aKey", "aValue")
                .CorrelateAll();

            // the executions that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery()
                //.ProcessVariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(2, correlatedExecutions);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationSetSerializedVariableValue()
        {
            // given
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            // when
//            FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

//            var baos = new MemoryStream();
//            new ObjectOutputStream(baos).WriteObject(javaSerializable);
//            string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToByteArray()), ProcessEngine);

//            // then it is not possible to deserialize the object
//            try
//            {
//                new ObjectInputStream(new MemoryStream(baos.ToByteArray())).ReadObject();
//            }
//            catch (Exception e)
//            {
//                AssertTextPresent("Exception while deserializing object.", e.Message);
//            }

//            // but it can be set as a variable:
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
//                .SetVariable("var", Variables.SerializedObjectValue(serializedObject)
//                    .objectTypeName(typeof(FailingJavaSerializable).FullName)
//                    .serializationDataFormat(Variables.SerializationDataFormats.JAVA)
//                    .Create())
//                .Correlate();

//            // then
//            IObjectValue variableTyped = runtimeService.GetVariableTyped(processInstance.Id, "var", false);
//            Assert.NotNull(variableTyped);
//            Assert.IsFalse(variableTyped.Deserialized);
//            Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
//            Assert.AreEqual(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testExecutionCorrelationSetSerializedVariableValues()
        {
//            // given
//            var processInstance = runtimeService.StartProcessInstanceByKey("process");

//            // when
//            FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

//            var baos = new MemoryStream();
//            new ObjectOutputStream(baos).WriteObject(javaSerializable);
//            string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToByteArray()), ProcessEngine);

//            // then it is not possible to deserialize the object
//            try
//            {
//                new ObjectInputStream(new MemoryStream(baos.ToByteArray())).ReadObject();
//            }
//            catch (Exception e)
//            {
//                AssertTextPresent("Exception while deserializing object.", e.Message);
//            }

//            // but it can be set as a variable:
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
//                .SetVariables(Variable.Variables.CreateVariables()
//                    .PutValueTyped("var", Variables.SerializedObjectValue(serializedObject)
//                        .objectTypeName(typeof(FailingJavaSerializable).FullName)
//                        .serializationDataFormat(Variables.SerializationDataFormats.JAVA)
//                        .Create()))
//                .Correlate();

//            // then
//            IObjectValue variableTyped = runtimeService.GetVariableTyped(processInstance.Id, "var", false);
//            Assert.NotNull(variableTyped);
//            Assert.IsFalse(variableTyped.Deserialized);
//            Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
//            Assert.AreEqual(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
        }

        [Test]
        [Deployment]
        public virtual void testMessageStartEventCorrelation()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";

            runtimeService.CorrelateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);

            var instances = runtimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, instances);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationUsingFluentCorrelateStartMessage()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariable("aKey", "aValue")
                .CorrelateStartMessage();

            var instances = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, instances);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationUsingFluentCorrelateSingle()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariable("aKey", "aValue")
                .Correlate();

            var instances = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, instances);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationUsingFluentCorrelateAll()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariable("aKey", "aValue")
                .CorrelateAll();

            var instances = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("aKey", "aValue")
                .Count();
            Assert.AreEqual(1, instances);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithBusinessKey()
        {
            const string businessKey = "aBusinessKey";

            runtimeService.CorrelateMessage("newInvoiceMessage", businessKey);

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);
            Assert.AreEqual(businessKey, processInstance.BusinessKey);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateStartMessage()
        {
            const string businessKey = "aBusinessKey";

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .CorrelateStartMessage();

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);
            Assert.AreEqual(businessKey, processInstance.BusinessKey);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateSingle()
        {
            const string businessKey = "aBusinessKey";

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .Correlate();

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);
            Assert.AreEqual(businessKey, processInstance.BusinessKey);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithBusinessKeyUsingFluentCorrelateAll()
        {
            const string businessKey = "aBusinessKey";

            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceBusinessKey(businessKey)
                .CorrelateAll();

            var processInstance = runtimeService.CreateProcessInstanceQuery()
                .First();
            Assert.NotNull(processInstance);
            Assert.AreEqual(businessKey, processInstance.BusinessKey);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void testMessageStartEventCorrelationSetSerializedVariableValue()
        {
            // when
//            FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

//            var baos = new MemoryStream();
//            new ObjectOutputStream(baos).WriteObject(javaSerializable);
//            string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToByteArray()), ProcessEngine);

//            // then it is not possible to deserialize the object
//            try
//            {
//                new ObjectInputStream(new MemoryStream(baos.ToByteArray())).ReadObject();
//            }
//            catch (Exception e)
//            {
//                AssertTextPresent("Exception while deserializing object.", e.Message);
//            }

//            // but it can be set as a variable:
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            //runtimeService.CreateMessageCorrelation("newInvoiceMessage")
//            //    .SetVariable("var", Variables.SerializedObjectValue(serializedObject)
//            //        //.objectTypeName(typeof(FailingJavaSerializable).FullName)
//            //        .SerializationDataFormat(Variables.SerializationDataFormats.Java)
//            //        .Create())
//            //    .Correlate();

//            // then
//            var startedInstance = runtimeService.CreateProcessInstanceQuery()
//                .First();
//            Assert.NotNull(startedInstance);

            //IObjectValue variableTyped = runtimeService.GetVariableTyped(startedInstance.Id, "var", false);
            //Assert.NotNull(variableTyped);
            //Assert.IsFalse(variableTyped.Deserialized);
            //Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            //Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
            //Assert.AreEqual(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void testMessageStartEventCorrelationSetSerializedVariableValues()
        {
//            // when
//            FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

//            var baos = new MemoryStream();
//            new ObjectOutputStream(baos).WriteObject(javaSerializable);
//            string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToByteArray()), ProcessEngine);

//            // then it is not possible to deserialize the object
//            try
//            {
//                new ObjectInputStream(new MemoryStream(baos.ToByteArray())).ReadObject();
//            }
//            catch (Exception e)
//            {
//                AssertTextPresent("Exception while deserializing object.", e.Message);
//            }

//            // but it can be set as a variable:
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
//                .SetVariables(Variable.Variables.CreateVariables()
//                    .PutValueTyped("var", Variables.SerializedObjectValue(serializedObject)
//                        .objectTypeName(typeof(FailingJavaSerializable).FullName)
//                        .serializationDataFormat(Variables.SerializationDataFormats.JAVA)
//                        .Create()))
//                .Correlate();

//            // then
//            var startedInstance = runtimeService.CreateProcessInstanceQuery()
//                .First();
//            Assert.NotNull(startedInstance);

//            IObjectValue variableTyped = runtimeService.GetVariableTyped(startedInstance.Id, "var", false);
//            Assert.NotNull(variableTyped);
//            Assert.IsFalse(variableTyped.Deserialized);
//            Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
//            Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
//            Assert.AreEqual(Variables.SerializationDataFormats.JAVA.Name, variableTyped.SerializationDataFormat);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateStartMessage()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("var1", "a")
                    .PutValue("var2", "b"))
                .CorrelateStartMessage();

            var query = runtimeService.CreateProcessInstanceQuery()
                ////.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("var1", "a")
                //.VariableValueEquals("var2", "b")
                ;
            Assert.AreEqual(1, query.Count());
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateSingleMessage()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("var1", "a")
                    .PutValue("var2", "b"))
                .Correlate();

            var query = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("var1", "a")
                //.VariableValueEquals("var2", "b")
                ;
            Assert.AreEqual(1, query.Count());
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testMessageStartEventCorrelationWithVariablesUsingFluentCorrelateAll()
        {
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .SetVariables(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                    .PutValue("var1", "a")
                    .PutValue("var2", "b"))
                .CorrelateAll();

            var query = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessDefinitionKey("messageStartEvent")
                //.VariableValueEquals("var1", "a")
                //.VariableValueEquals("var2", "b")
                ;
            Assert.AreEqual(1, query.Count());
        }

        /// <summary>
        ///     this test assures the right start event is selected
        /// </summary>
        [Test]
        [Deployment]
        public virtual void testMultipleMessageStartEventsCorrelation()
        {
            runtimeService.CorrelateMessage("someMessage");
            // verify the right start event was selected:
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First());
            taskService.Complete(task.Id);

            runtimeService.CorrelateMessage("someOtherMessage");
            // verify the right start event was selected:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First());
            taskService.Complete(task.Id);
        }

        [Test][Deployment("resources/api/runtime/MessageCorrelationTest.TestMultipleMessageStartEventsCorrelation.bpmn20.xml") ]
        public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateStartMessage()
        {
            runtimeService.CreateMessageCorrelation("someMessage")
                .CorrelateStartMessage();
            // verify the right start event was selected:
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First());
            taskService.Complete(task.Id);

            runtimeService.CreateMessageCorrelation("someOtherMessage")
                .CorrelateStartMessage();
            // verify the right start event was selected:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First());
            taskService.Complete(task.Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMultipleMessageStartEventsCorrelation.bpmn20.xml")]
        public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateSingle()
        {
            runtimeService.CreateMessageCorrelation("someMessage")
                .Correlate();
            // verify the right start event was selected:
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First());
            taskService.Complete(task.Id);

            runtimeService.CreateMessageCorrelation("someOtherMessage")
                .Correlate();
            // verify the right start event was selected:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First());
            taskService.Complete(task.Id);
        }

        /// <summary>
        ///     this test assures the right start event is selected
        /// </summary>
        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMultipleMessageStartEventsCorrelation.bpmn20.xml")]
        public virtual void testMultipleMessageStartEventsCorrelationUsingFluentCorrelateAll()
        {
            runtimeService.CreateMessageCorrelation("someMessage")
                .CorrelateAll();
            // verify the right start event was selected:
            var task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First());
            taskService.Complete(task.Id);

            runtimeService.CreateMessageCorrelation("someOtherMessage")
                .CorrelateAll();
            // verify the right start event was selected:
            task = taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task2")
                .First();
            Assert.NotNull(task);
            Assert.IsNull(taskService.CreateTaskQuery(c=>c.TaskDefinitionKeyWithoutCascade == "task1")
                .First());
            taskService.Complete(task.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMatchingStartEventAndExecution()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("process");

            Assert.NotNull(runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .First());
            // correlate message -> this will trigger the execution
            runtimeService.CorrelateMessage("newInvoiceMessage");
            Assert.IsNull(runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .First());

            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // fluent builder //////////////////////

            processInstance = runtimeService.StartProcessInstanceByKey("process");

            Assert.NotNull(runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .First());
            // correlate message -> this will trigger the execution
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .Correlate();
            Assert.IsNull(runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .First());

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
        }


        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMatchingStartEventAndExecution.bpmn20.xml")]
        public virtual void testMessageCorrelationResultWithResultTypeProcessDefinition()
        {
            //given
            var msgName = "newInvoiceMessage";

            //when
            //correlate message with result
            var result = runtimeService.CreateMessageCorrelation(msgName)
                .CorrelateWithResult();

            //then
            //message correlation result contains information from receiver
            checkProcessDefinitionMessageCorrelationResult(result, "theStart", "process");
        }

        protected internal virtual void checkProcessDefinitionMessageCorrelationResult(IMessageCorrelationResult result,
            string startActivityId, string processDefinitionId)
        {
            Assert.NotNull(result);
            Assert.NotNull(result.ProcessInstance.Id);
            Assert.AreEqual(MessageCorrelationResultType.ProcessDefinition, result.ResultType);
            Assert.True(result.ProcessInstance.ProcessDefinitionId.Contains(processDefinitionId));
        }


        [Test][Deployment("resources/api/runtime/MessageCorrelationTest.TestMatchingStartEventAndExecution.bpmn20.xml")]
        public virtual void testMessageCorrelationResultWithResultTypeExecution()
        {
            //given
            var msgName = "newInvoiceMessage";
            var processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.NotNull(runtimeService.CreateExecutionQuery()
                //.MessageEventSubscriptionName(msgName)
                .First());

            //when
            //correlate message with result
            var result = runtimeService.CreateMessageCorrelation(msgName)
                .CorrelateWithResult();

            //then
            //message correlation result contains information from receiver
            checkExecutionMessageCorrelationResult(result, processInstance, "messageCatch");
        }

        protected internal virtual void checkExecutionMessageCorrelationResult(IMessageCorrelationResult result,
            IProcessInstance processInstance, string activityId)
        {
            Assert.NotNull(result);
            Assert.AreEqual(MessageCorrelationResultType.Execution, result.ResultType);
            Assert.AreEqual(processInstance.Id, result.Execution.ProcessInstanceId);
            var entity = (ExecutionEntity) result.Execution;
            Assert.AreEqual(activityId, entity.ActivityId);
        }


        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMatchingStartEventAndExecution.bpmn20.xml")]
        public virtual void testMatchingStartEventAndExecutionUsingFluentCorrelateAll()
        {
            runtimeService.StartProcessInstanceByKey("process");
            runtimeService.StartProcessInstanceByKey("process");

            Assert.AreEqual(2, runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count());
            // correlate message -> this will trigger the executions AND start a new process instance
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .CorrelateAll();
            Assert.NotNull(runtimeService.CreateExecutionQuery()
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .First());

            Assert.AreEqual(3, runtimeService.CreateProcessInstanceQuery()
                .Count());
        }


        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMatchingStartEventAndExecution.bpmn20.xml")]
        public virtual void testMatchingStartEventAndExecutionCorrelateAllWithResult()
        {
            //given
            var procInstance1 = runtimeService.StartProcessInstanceByKey("process");
            var procInstance2 = runtimeService.StartProcessInstanceByKey("process");

            //when correlated all with result
            var resultList = runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .CorrelateAllWithResult();

            //then result should contains three entries
            //two of type execution und one of type process definition
            Assert.AreEqual(3, resultList.Count);
            var executionResultCount = 0;
            var procDefResultCount = 0;
            foreach (var result in resultList)
                if (result.ResultType.Equals(MessageCorrelationResultType.Execution))
                {
                    Assert.NotNull(result);
                    Assert.AreEqual(MessageCorrelationResultType.Execution, result.ResultType);
                    Assert.True(procInstance1.Id.Equals(result.Execution.ProcessInstanceId) ||
                                procInstance2.Id.Equals(result.Execution.ProcessInstanceId));
                    var entity = (ExecutionEntity) result.Execution;
                    Assert.AreEqual("messageCatch", entity.ActivityId);
                    executionResultCount++;
                }
                else
                {
                    checkProcessDefinitionMessageCorrelationResult(result, "theStart", "process");
                    procDefResultCount++;
                }
            Assert.AreEqual(2, executionResultCount);
            Assert.AreEqual(1, procDefResultCount);
        }

        [Test]
        public virtual void testMessageStartEventCorrelationWithNonMatchingDefinition()
        {
            try
            {
                runtimeService.CorrelateMessage("aMessageName");
                Assert.Fail("Expect an Exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("Cannot correlate message", e.Message);
            }

            // fluent builder //////////////////

            try
            {
                runtimeService.CreateMessageCorrelation("aMessageName")
                    .Correlate();
                Assert.Fail("Expect an Exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("Cannot correlate message", e.Message);
            }

            // fluent builder with multiple correlation //////////////////
            // This should not Assert.Fail
            runtimeService.CreateMessageCorrelation("aMessageName")
                .CorrelateAll();
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKeyAndVariables()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["aKey"] = new StringValueImpl("aValue");
            var processInstance = runtimeService.StartProcessInstanceByKey("process", "aBusinessKey", variables);

            variables = new Dictionary<string, ITypedValue>();
            variables["aKey"] = new StringValueImpl("aValue");
            runtimeService.StartProcessInstanceByKey("process", "anotherBusinessKey", variables);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";

            IDictionary<string, object> processVariables = new Dictionary<string, object>();
            processVariables["aProcessVariable"] = "aVariableValue";
            runtimeService.CorrelateMessage(messageName, "aBusinessKey", correlationKeys, processVariables);

            var correlatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aProcessVariable", "aVariableValue")
                .First();

            Assert.NotNull(correlatedExecution);

            var correlatedProcessInstance = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceId(correlatedExecution.ProcessInstanceId)
                .First();

            Assert.AreEqual("aBusinessKey", correlatedProcessInstance.BusinessKey);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);

            // fluent builder /////////////////////////////

            variables = new Dictionary<string, ITypedValue>();
            variables["aKey"] = new StringValueImpl("aValue");
            processInstance = runtimeService.StartProcessInstanceByKey("process", "aBusinessKey", variables);

            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceBusinessKey("aBusinessKey")
                .ProcessInstanceVariableEquals("aKey", "aValue")
                .SetVariable("aProcessVariable", "aVariableValue")
                .Correlate();

            correlatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aProcessVariable", "aVariableValue")
                .First();

            Assert.NotNull(correlatedExecution);

            correlatedProcessInstance = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceId(correlatedExecution.ProcessInstanceId)
                .First();

            Assert.AreEqual("aBusinessKey", correlatedProcessInstance.BusinessKey);

            runtimeService.DeleteProcessInstance(processInstance.Id, null);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKeyAndVariablesUsingFluentCorrelateAll()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["aKey"] = new StringValueImpl("aValue");
            runtimeService.StartProcessInstanceByKey("process", "aBusinessKey", variables);
            runtimeService.StartProcessInstanceByKey("process", "aBusinessKey", variables);

            var messageName = "newInvoiceMessage";
            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceBusinessKey("aBusinessKey")
                .ProcessInstanceVariableEquals("aKey", "aValue")
                .SetVariable("aProcessVariable", "aVariableValue")
                .CorrelateAll();

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("aProcessVariable", "aVariableValue")
                
                .ToList();

            Assert.AreEqual(2, correlatedExecutions.Count);

            // Instance 1
            var correlatedExecution = correlatedExecutions[0];
            var correlatedProcessInstance = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceId(correlatedExecution.ProcessInstanceId)
                .First();

            Assert.AreEqual("aBusinessKey", correlatedProcessInstance.BusinessKey);

            // Instance 2
            correlatedExecution = correlatedExecutions[1];
            correlatedProcessInstance = runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceId(correlatedExecution.ProcessInstanceId)
                .First();

            Assert.AreEqual("aBusinessKey", correlatedProcessInstance.BusinessKey);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByProcessInstanceId()
        {
            var processInstance1 = runtimeService.StartProcessInstanceByKey("process");

            var processInstance2 = runtimeService.StartProcessInstanceByKey("process");

            // correlation with only the name is ambiguous:
            try
            {
                runtimeService.CreateMessageCorrelation("aMessageName")
                    .Correlate();
                Assert.Fail("Expect an Exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("Cannot correlate message", e.Message);
            }

            // use process instance id as well
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceId(processInstance1.Id)
                .Correlate();

            var correlatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Where(c=>c.ProcessInstanceId==processInstance1.Id)
                .First();
            Assert.NotNull(correlatedExecution);

            var uncorrelatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Where(c=>c.ProcessInstanceId==processInstance2.Id)
                .First();
            Assert.IsNull(uncorrelatedExecution);

            runtimeService.DeleteProcessInstance(processInstance1.Id, null);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByProcessInstanceIdUsingFluentCorrelateAll()
        {
            // correlate by name
            var processInstance1 = runtimeService.StartProcessInstanceByKey("process");

            var processInstance2 = runtimeService.StartProcessInstanceByKey("process");

            // correlation with only the name is ambiguous:
            runtimeService.CreateMessageCorrelation("aMessageName")
                .CorrelateAll();

            Assert.AreEqual(0, runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Count());

            // correlate process instance id
            processInstance1 = runtimeService.StartProcessInstanceByKey("process");

            processInstance2 = runtimeService.StartProcessInstanceByKey("process");

            // use process instance id as well
            runtimeService.CreateMessageCorrelation("newInvoiceMessage")
                .ProcessInstanceId(processInstance1.Id)
                .CorrelateAll();

            var correlatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Where(c=>c.ProcessInstanceId==processInstance1.Id)
                .First();
            Assert.NotNull(correlatedExecution);

            var uncorrelatedExecution = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                .Where(c=>c.ProcessInstanceId==processInstance2.Id)
                .First();
            Assert.IsNull(uncorrelatedExecution);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKeyAndNullVariableUsingFluentCorrelateAll()
        {
            runtimeService.StartProcessInstanceByKey("process", "aBusinessKey");

            var messageName = "newInvoiceMessage";

            try
            {
                runtimeService.CreateMessageCorrelation(messageName)
                    .ProcessInstanceBusinessKey("aBusinessKey")
                    .SetVariable(null, "aVariableValue")
                    .CorrelateAll();
                Assert.Fail("Variable name is null");
            }
            catch (System.Exception e)
            {
                Assert.True(e is ProcessEngineException);
                AssertTextPresent("null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKeyAndNullVariableEqualsUsingFluentCorrelateAll()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["foo"] = new StringValueImpl("bar");
            runtimeService.StartProcessInstanceByKey("process", "aBusinessKey", variables);

            var messageName = "newInvoiceMessage";

            try
            {
                runtimeService.CreateMessageCorrelation(messageName)
                    .ProcessInstanceBusinessKey("aBusinessKey")
                    .ProcessInstanceVariableEquals(null, "bar")
                    .CorrelateAll();
                Assert.Fail("Variable name is null");
            }
            catch (System.Exception e)
            {
                Assert.True(e is ProcessEngineException);
                AssertTextPresent("null", e.Message);
            }
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKeyAndNullVariablesUsingFluentCorrelateAll()
        {
            runtimeService.StartProcessInstanceByKey("process", "aBusinessKey");

            var messageName = "newInvoiceMessage";

            runtimeService.CreateMessageCorrelation(messageName)
                .ProcessInstanceBusinessKey("aBusinessKey")
                .SetVariables(null)
                .SetVariable("foo", "bar")
                .CorrelateAll();

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.ProcessVariableValueEquals("foo", "bar")
                
                .ToList();

            Assert.IsFalse(correlatedExecutions.Count == 0);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByVariablesOnly()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["variable"] = "value1";
            runtimeService.StartProcessInstanceByKey("process", variables);

            variables["variable"] = "value2";
            var instance = runtimeService.StartProcessInstanceByKey("process", variables);

            runtimeService.CorrelateMessage(null, variables);

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                
                .ToList();

            Assert.AreEqual(1, correlatedExecutions.Count);
            Assert.AreEqual(instance.Id, correlatedExecutions[0].Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByBusinessKey()
        {
            runtimeService.StartProcessInstanceByKey("process", "businessKey1");
            var instance = runtimeService.StartProcessInstanceByKey("process", "businessKey2");

            runtimeService.CorrelateMessage(null, "businessKey2");

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                
                .ToList();

            Assert.AreEqual(1, correlatedExecutions.Count);
            Assert.AreEqual(instance.Id, correlatedExecutions[0].Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationByProcessInstanceIdOnly()
        {
            runtimeService.StartProcessInstanceByKey("process");
            var instance = runtimeService.StartProcessInstanceByKey("process");

            runtimeService.CreateMessageCorrelation(null)
                .ProcessInstanceId(instance.Id)
                .Correlate();

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                
                .ToList();

            Assert.AreEqual(1, correlatedExecutions.Count);
            Assert.AreEqual(instance.Id, correlatedExecutions[0].Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationWithoutMessageNameFluent()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["variable"] = "value1";
            runtimeService.StartProcessInstanceByKey("process", variables);

            variables["variable"] = "value2";
            var instance = runtimeService.StartProcessInstanceByKey("process", variables);

            runtimeService.CreateMessageCorrelation(null)
                .ProcessInstanceVariableEquals("variable", "value2")
                .Correlate();

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                
                .ToList();

            Assert.AreEqual(1, correlatedExecutions.Count);
            Assert.AreEqual(instance.Id, correlatedExecutions[0].Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelateAllWithoutMessage()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["variable"] = "value1";
            runtimeService.StartProcessInstanceByKey("process", variables);
            runtimeService.StartProcessInstanceByKey("secondProcess", variables);

            variables["variable"] = "value2";
            var instance1 = runtimeService.StartProcessInstanceByKey("process", variables);
            var instance2 = runtimeService.StartProcessInstanceByKey("secondProcess", variables);

            runtimeService.CreateMessageCorrelation(null)
                .ProcessInstanceVariableEquals("variable", "value2")
                .CorrelateAll();

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                //.OrderByProcessDefinitionKey()
                /*.Asc()*/
                
                .ToList();

            Assert.AreEqual(2, correlatedExecutions.Count);
            Assert.AreEqual(instance1.Id, correlatedExecutions[0].Id);
            Assert.AreEqual(instance2.Id, correlatedExecutions[1].Id);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationWithoutMessageDoesNotMatchStartEvent()
        {
            try
            {
                runtimeService.CreateMessageCorrelation(null)
                    .ProcessInstanceVariableEquals("variable", "value2")
                    .Correlate();
                Assert.Fail("exception expected");
            }
            catch (MismatchingMessageCorrelationException)
            {
                // expected
            }

            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ActivityId == "task")
                
                .ToList();

            Assert.True(correlatedExecutions.Count == 0);
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testCorrelationWithoutCorrelationPropertiesFails()
        {
            runtimeService.StartProcessInstanceByKey("process");

            try
            {
                runtimeService.CreateMessageCorrelation(null)
                    .Correlate();
                Assert.Fail("expected exception");
            }
            catch (NullValueException)
            {
                // expected
            }

            try
            {
                runtimeService.CorrelateMessage(null);
                Assert.Fail("expected exception");
            }
            catch (NullValueException)
            {
                // expected
            }
        }

        [Test]
        [Deployment("resources/api/runtime/twoBoundaryEventSubscriptions.bpmn20.xml")]
        public virtual void testCorrelationToExecutionWithMultipleSubscriptionsFails()
        {
            var instance = runtimeService.StartProcessInstanceByKey("process");

            try
            {
                runtimeService.CreateMessageCorrelation(null)
                    .ProcessInstanceId(instance.Id)
                    .Correlate();
                Assert.Fail("expected exception");
            }
            catch (ProcessEngineException)
            {
                // note: this does not expect a MismatchingCorrelationException since the exception
                // is only raised in the MessageEventReceivedCmd. Otherwise, this would require explicit checking in the
                // correlation handler that a matched execution without message name has exactly one message (now it checks for
                // at least one message)

                // expected
            }
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testSuspendedProcessInstance()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            var processInstance = runtimeService.StartProcessInstanceByKey("process", variables)
                .Id;

            // suspend process instance
            runtimeService.SuspendProcessInstanceById(processInstance);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";

            try
            {
                runtimeService.CorrelateMessage(messageName, correlationKeys);
                Assert.Fail("It should not be possible to correlate a message to a suspended process instance.");
            }
            catch (MismatchingMessageCorrelationException)
            {
                // expected
            }
        }

        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestCatchingMessageEventCorrelation.bpmn20.xml")]
        public virtual void testOneMatchingAndOneSuspendedProcessInstance()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            var firstProcessInstance = runtimeService.StartProcessInstanceByKey("process", variables)
                .Id;

            variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";
            var secondProcessInstance = runtimeService.StartProcessInstanceByKey("process", variables)
                .Id;

            // suspend second process instance
            runtimeService.SuspendProcessInstanceById(secondProcessInstance);

            var messageName = "newInvoiceMessage";
            IDictionary<string, object> correlationKeys = new Dictionary<string, object>();
            correlationKeys["aKey"] = "aValue";

            IDictionary<string, object> messagePayload = new Dictionary<string, object>();
            messagePayload["aNewKey"] = "aNewVariable";

            runtimeService.CorrelateMessage(messageName, correlationKeys, messagePayload);

            // there exists an uncorrelated executions (the second process instance)
            var uncorrelatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == secondProcessInstance)
                //.ProcessVariableValueEquals("aKey", "aValue")
                /*.MessageEventSubscriptionName("newInvoiceMessage")*/
                .Count();
            Assert.AreEqual(1, uncorrelatedExecutions);

            // the execution that has been correlated should have advanced
            var correlatedExecutions = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId == firstProcessInstance&& c.ActivityId =="task")
                //.ProcessVariableValueEquals("aKey", "aValue")
                //.ProcessVariableValueEquals("aNewKey", "aNewVariable")
                .Count();
            Assert.AreEqual(1, correlatedExecutions);
        }
        [Test]
        [Deployment("resources/api/runtime/MessageCorrelationTest.TestMessageStartEventCorrelation.bpmn20.xml")]
        public virtual void testSuspendedProcessDefinition()
        {
            var processDefinitionId = repositoryService.CreateProcessDefinitionQuery()
                .First()
                .Id;

            repositoryService.SuspendProcessDefinitionById(processDefinitionId);

            IDictionary<string, object> variables = new Dictionary<string, object>();
            variables["aKey"] = "aValue";

            try
            {
                runtimeService.CorrelateMessage("newInvoiceMessage", new Dictionary<string, object>(), variables);
                Assert.Fail("It should not be possible to correlate a message to a suspended process definition.");
            }
            catch (MismatchingMessageCorrelationException)
            {
                // expected
            }
        }

        [Test]
        public virtual void testCorrelateMessageStartEventWithProcessDefinitionId()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .Message("a")
                .UserTask()
                .EndEvent()
                .Done());

            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .Message("b")
                .UserTask()
                .EndEvent()
                .Done());

            var firstProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 1)
                .First();
            var secondProcessDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Version == 2)
                .First();

            runtimeService.CreateMessageCorrelation("a")
                .ProcessDefinitionId(firstProcessDefinition.Id)
                .ProcessInstanceBusinessKey("first")
                .CorrelateStartMessage();

            runtimeService.CreateMessageCorrelation("b")
                .ProcessDefinitionId(secondProcessDefinition.Id)
                .ProcessInstanceBusinessKey("second")
                .CorrelateStartMessage();

            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceBusinessKey("first")
                //.SetProcessDefinitionId(firstProcessDefinition.Id)
                .Count());
            Assert.AreEqual(1, runtimeService.CreateProcessInstanceQuery()
                //.SetProcessInstanceBusinessKey("second")
                //.SetProcessDefinitionId(secondProcessDefinition.Id)
                .Count());
        }

        [Test]
        public virtual void testFailCorrelateMessageStartEventWithWrongProcessDefinitionId()
        {
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .Message("a")
                .UserTask()
                .EndEvent()
                .Done());

            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process")
                .StartEvent()
                .Message("b")
                .UserTask()
                .EndEvent()
                .Done());

            var latestProcessDefinition = repositoryService.CreateProcessDefinitionQuery()
                /*.LatestVersion()*/
                .First();

            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessDefinitionId(latestProcessDefinition.Id)
                    .CorrelateStartMessage();

                Assert.Fail("expected exception");
            }
            catch (MismatchingMessageCorrelationException e)
            {
                AssertTextPresent("Cannot correlate message 'a'", e.Message);
            }
        }

        [Test]
        public virtual void testFailCorrelateMessageStartEventWithNonExistingProcessDefinitionId()
        {
            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessDefinitionId("not existing")
                    .CorrelateStartMessage();

                Assert.Fail("expected exception");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("no deployed process definition found", e.Message);
            }
        }

        [Test]
        public virtual void testFailCorrelateMessageWithProcessDefinitionId()
        {
            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessDefinitionId("id")
                    .Correlate();

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                AssertTextPresent("Cannot specify a process definition id", e.Message);
            }
        }

        [Test]
        public virtual void testFailCorrelateMessagesWithProcessDefinitionId()
        {
            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessDefinitionId("id")
                    .CorrelateAll();

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                AssertTextPresent("Cannot specify a process definition id", e.Message);
            }
        }

        [Test]
        public virtual void testFailCorrelateMessageStartEventWithCorrelationVariable()
        {
            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessInstanceVariableEquals("var", "value")
                    .CorrelateStartMessage();

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                AssertTextPresent("Cannot specify correlation variables ", e.Message);
            }
        }

        [Test]
        public virtual void testFailCorrelateMessageStartEventWithCorrelationVariables()
        {
            try
            {
                runtimeService.CreateMessageCorrelation("a")
                    .ProcessInstanceVariablesEqual(ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables()
                        .PutValue("var1", "b")
                        .PutValue("var2", "c"))
                    .CorrelateStartMessage();

                Assert.Fail("expected exception");
            }
            catch (BadUserRequestException e)
            {
                AssertTextPresent("Cannot specify correlation variables ", e.Message);
            }
        }
    }
}