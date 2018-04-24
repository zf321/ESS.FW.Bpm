using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressionServiceTaskTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestSetServiceResultToProcessVariables()
        {
           IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["bean"] = new StringValueImpl("ok");


            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("setServiceResultToProcessVariables",
                variables);
            Assert.AreEqual("ok", runtimeService.GetVariable(pi.Id, "result"));
        }

        [Test]
        [Deployment]
        public virtual void TestBackwardsCompatibleExpression()
        {

            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["var"] = new StringValueImpl("---");
            
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("BackwardsCompatibleExpressionProcess",
                variables);
            Assert.AreEqual("..---..", runtimeService.GetVariable(pi.Id, "result"));
        }
    }

}