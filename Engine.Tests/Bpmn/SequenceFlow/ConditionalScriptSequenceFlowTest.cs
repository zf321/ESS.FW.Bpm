using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SequenceFlow
{

    public class ConditionalScriptSequenceFlowTest : PluggableProcessEngineTestCase
    {
        [Test]//groovy
        [Deployment]
        public virtual void TestScriptExpression()
        {
            string[] directions = new string[] { "left", "right" };
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            foreach (string direction in directions)
            {
                variables["foo"] = new StringValueImpl(direction);

                runtimeService.StartProcessInstanceByKey("process", variables);

                ITask task = taskService.CreateTaskQuery(m => m.TaskDefinitionKey == direction).First();
                Assert.AreEqual(direction, task.TaskDefinitionKey);
                taskService.Complete(task.Id);
            }
        }

        [Test]//groovy
        [Deployment]
        public virtual void TestScriptExpressionWithNonBooleanResult()
        {
            try
            {
                runtimeService.StartProcessInstanceByKey("process");
                Assert.Fail("expected exception: invalid return value in script");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("condition script returns non-Boolean", e.Message);
            }
        }


        [Test]//groovy
        [Deployment]
        public virtual void TestScriptResourceExpression()
        {
            string[] directions = new string[] { "left", "right" };
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            foreach (string direction in directions)
            {
                variables["foo"] = new StringValueImpl(direction);
                runtimeService.StartProcessInstanceByKey("process", variables);

                ITask task = taskService.CreateTaskQuery().First();
                Assert.AreEqual(direction, task.TaskDefinitionKey);
                taskService.Complete(task.Id);
            }
        }
    }
}