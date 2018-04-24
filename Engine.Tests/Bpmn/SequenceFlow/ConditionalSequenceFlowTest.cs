using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SequenceFlow
{

    [TestFixture]
    public class ConditionalSequenceFlowTest : PluggableProcessEngineTestCase
    {

        [Test]
        [Deployment]
        public virtual void TestUelExpression()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["input"] = new StringValueImpl("right");

            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("condSeqFlowUelExpr", variables);

            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pi.Id).First();
            Assert.NotNull(task);
            Assert.AreEqual("task right", task.Name);
        }

        [Test]
        [Deployment]
        public virtual void TestValueAndMethodExpression()
        {
            ConditionalSequenceFlowTestOrder order = new ConditionalSequenceFlowTestOrder(150);

            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["Order"] = new ObjectValueImpl(order);

            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("uelExpressions", variables);
            var pid = processInstance.Id;
            ITask task = taskService.CreateTaskQuery(c => c.ProcessInstanceId == pid).First();
            Assert.AreEqual("Standard service", task.Name);

            order = new ConditionalSequenceFlowTestOrder(300);
            variables["Order"] = new ObjectValueImpl(order);
            processInstance = runtimeService.StartProcessInstanceByKey("uelExpressions", variables);
            pid = processInstance.Id;
            task = taskService.CreateTaskQuery(c => c.ProcessInstanceId ==pid).First();
            Assert.AreEqual("Premium service", task.Name);
        }


        [Test]
        [Deployment]
        public virtual void testNoExpressionTrueThrowsException()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["input"] = new StringValueImpl("non-existing-value");
            //  IDictionary<string, object> variables = CollectionUtil.singletonMap("input", "non-existing-value");
            try
            {
                runtimeService.StartProcessInstanceByKey("condSeqFlowUelExpr", variables);
                Assert.Fail("Expected ProcessEngineException");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("No conditional sequence flow leaving the Flow Node 'theStart' could be selected for continuing the process", e.Message);
            }
            catch (System.Exception ex)
            {

            }
        }

    }

}