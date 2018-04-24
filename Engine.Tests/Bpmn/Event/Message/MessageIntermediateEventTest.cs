using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Message
{
    [TestFixture]
    public class MessageIntermediateEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testSingleIntermediateMessageEvent()
        {

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(pi.Id);
            Assert.NotNull(activeActivityIds);
            Assert.AreEqual(1, activeActivityIds.Count);
            Assert.True(activeActivityIds.Contains("messageCatch"));

            string messageName = "newInvoiceMessage";
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName(messageName)*/.First();

            Assert.NotNull(execution);

            runtimeService.MessageEventReceived(messageName, execution.Id);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);

        }

        [Test]
        [Deployment]
        public virtual void testConcurrentIntermediateMessageEvent()
        {

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(pi.Id);
            Assert.NotNull(activeActivityIds);
            Assert.AreEqual(2, activeActivityIds.Count);
            Assert.True(activeActivityIds.Contains("messageCatch1"));
            Assert.True(activeActivityIds.Contains("messageCatch2"));

            string messageName = "newInvoiceMessage";
            IList<IExecution> executions = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName(messageName)*/.ToList();

            Assert.NotNull(executions);
            Assert.AreEqual(2, executions.Count);

            runtimeService.MessageEventReceived(messageName, executions[0].Id);

            ITask task = taskService.CreateTaskQuery().First();
            Assert.IsNull(task);

            runtimeService.MessageEventReceived(messageName, executions[1].Id);

            task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);

            taskService.Complete(task.Id);
        }

        public virtual void testIntermediateMessageEventRedeployment()
        {

            // deploy version 1
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml").Deploy();
            // now there is one process deployed
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process");

            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(pi.Id);
            Assert.NotNull(activeActivityIds);
            Assert.AreEqual(1, activeActivityIds.Count);
            Assert.True(activeActivityIds.Contains("messageCatch"));

            // deploy version 2
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml").Deploy();

            // now there are two versions deployed:
            Assert.AreEqual(2, repositoryService.CreateProcessDefinitionQuery().Count());

            // assert process is still waiting in message event:
            activeActivityIds = runtimeService.GetActiveActivityIds(pi.Id);
            Assert.NotNull(activeActivityIds);
            Assert.AreEqual(1, activeActivityIds.Count);
            Assert.True(activeActivityIds.Contains("messageCatch"));

            // Delete both versions:
            foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
            {
                repositoryService.DeleteDeployment(deployment.Id, true);
            }

        }

        public virtual void testEmptyMessageNameFails()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/event/message/MessageIntermediateEventTest.testEmptyMessageNameFails.bpmn20.xml").Deploy();
                Assert.Fail("exception expected");
            }
            catch (ProcessEngineException e)
            {
                Assert.True(e.Message.Contains("Cannot have a message event subscription with an empty or missing name"));
            }
        }

        // Todo: ObjectOutputStream
        //[Test]
        //[Deployment(new string[] { "resources/bpmn/event/message/MessageIntermediateEventTest.testSingleIntermediateMessageEvent.bpmn20.xml" })]
        //public virtual void testSetSerializedVariableValues()
        //{

        //    // given
        //    IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");

        //    IEventSubscription messageEventSubscription = runtimeService.CreateEventSubscriptionQuery().First();

        //    // when
        //    FailingJavaSerializable javaSerializable = new FailingJavaSerializable("foo");

        //    System.IO.MemoryStream baos = new System.IO.MemoryStream();

        //    (new ObjectOutputStream(baos)).writeObject(javaSerializable);
        //    string serializedObject = StringUtil.FromBytes(Base64.EncodeBase64(baos.ToArray()), ProcessEngine);

        //    // then it is not possible to deserialize the object
        //    try
        //    {
        //        (new ObjectInputStream(new System.IO.MemoryStream(baos.ToArray()))).readObject();
        //    }
        //    catch (Exception e)
        //    {
        //        AssertTextPresent("Exception while deserializing object.", e.Message);
        //    }

        //    // but it can be set as a variable when delivering a message:
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //    runtimeService.MessageEventReceived("newInvoiceMessage", messageEventSubscription.ExecutionId, Variable.Variables.CreateVariables().PutValueTyped("var", Variables.SerializedObjectValue(serializedObject).ObjectTypeName(typeof(FailingJavaSerializable).FullName).SerializationDataFormat(Variables.SerializationDataFormats.Java.ToString()).Create()));

        //    // then
        //    IObjectValue variableTyped = runtimeService.GetVariableTyped<IObjectValue>(processInstance.Id, "var", false);
        //    Assert.NotNull(variableTyped);
        //    Assert.IsFalse(variableTyped.Deserialized);
        //    Assert.AreEqual(serializedObject, variableTyped.ValueSerialized);
        //    //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //    Assert.AreEqual(typeof(FailingJavaSerializable).FullName, variableTyped.ObjectTypeName);
        //    Assert.AreEqual(Variables.SerializationDataFormats.Java.ToString(), variableTyped.SerializationDataFormat);
        //}


        [Test]
        [Deployment]
        public virtual void testExpressionInSingleIntermediateMessageEvent()
        {

            // given
            Dictionary<string, object> variables = new Dictionary<string, object>();
            variables["foo"] = "bar";

            // when
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("process", variables);
            IList<string> activeActivityIds = runtimeService.GetActiveActivityIds(pi.Id);
            Assert.NotNull(activeActivityIds);
            Assert.AreEqual(1, activeActivityIds.Count);
            Assert.True(activeActivityIds.Contains("messageCatch"));

            // then
            string messageName = "newInvoiceMessage-bar";
            IExecution execution = runtimeService.CreateExecutionQuery()/*.MessageEventSubscriptionName(messageName)*/.First();
            Assert.NotNull(execution);

            runtimeService.MessageEventReceived(messageName, execution.Id);
            ITask task = taskService.CreateTaskQuery().First();
            Assert.NotNull(task);
            taskService.Complete(task.Id);
        }

    }

}