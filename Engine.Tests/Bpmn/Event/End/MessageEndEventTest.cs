using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.End
{
    [TestFixture]
    public class MessageEndEventTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void testMessageEndEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process");
            Assert.NotNull(processInstance);
            AssertProcessEnded(processInstance.Id);
        }

        [Test]
        [Deployment]
        public virtual void testMessageEndEventServiceTaskBehavior()
        {
            IDictionary<string, object> variables = new Dictionary<string, object>();

            // class
            variables["wasExecuted"] = true;
            variables["expressionWasExecuted"] = false;
            variables["delegateExpressionWasExecuted"] = false;
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("process", variables);
            Assert.NotNull(processInstance);

            AssertProcessEnded(processInstance.Id);
            Assert.True(DummyServiceTask.wasExecuted);

            // expression
            variables = new Dictionary<string, object>();
            variables["wasExecuted"] = false;
            variables["expressionWasExecuted"] = true;
            variables["delegateExpressionWasExecuted"] = false;
            variables["endEventBean"] = new EndEventBean();
            processInstance = runtimeService.StartProcessInstanceByKey("process", variables);
            Assert.NotNull(processInstance);

            AssertProcessEnded(processInstance.Id);
            Assert.True(DummyServiceTask.expressionWasExecuted);

            // delegate expression
            variables = new Dictionary<string, object>();
            variables["wasExecuted"] = false;
            variables["expressionWasExecuted"] = false;
            variables["delegateExpressionWasExecuted"] = true;
            variables["endEventBean"] = new EndEventBean();
            processInstance = runtimeService.StartProcessInstanceByKey("process", variables);
            Assert.NotNull(processInstance);

            AssertProcessEnded(processInstance.Id);
            Assert.True(DummyServiceTask.delegateExpressionWasExecuted);
        }

    }

}