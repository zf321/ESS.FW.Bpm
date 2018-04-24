using System.Collections.Generic;
using Engine.Tests.Standalone.Pvm.Activities;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Pvm
{
    [TestFixture]
    public class PvmVariablesTest
    {
        [Test]
        public virtual void TestVariables()
        {
            var processDefinition =
                new ProcessDefinitionBuilder().CreateActivity("a")
                    .Initial()
                    .Behavior(new WaitState())
                    .EndActivity()
                    .BuildProcessDefinition();

            PvmExecutionImpl processInstance = (PvmExecutionImpl) processDefinition.CreateProcessInstance();
            processInstance.SetVariable("amount", 500L);
            processInstance.SetVariable("msg", "hello world");
            processInstance.Start();

            Assert.AreEqual(500L, processInstance.GetVariable("amount"));
            Assert.AreEqual("hello world", processInstance.GetVariable("msg"));

            var activityInstance = processInstance.FindExecution("a");
            Assert.AreEqual(500L, activityInstance.GetVariable("amount"));
            Assert.AreEqual("hello world", activityInstance.GetVariable("msg"));

            IDictionary<string, object> expectedVariables = new Dictionary<string, object>();
            expectedVariables["amount"] = 500L;
            expectedVariables["msg"] = "hello world";

            //TODO JAVA 行为不一样
            //Assert.AreEqual(expectedVariables, activityInstance.Variables);
            //Assert.AreEqual(expectedVariables, processInstance.Variables);
        }
    }
}