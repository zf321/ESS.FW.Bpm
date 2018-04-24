using System.Collections.Generic;
using Engine.Tests.Bpmn.ServiceTask.Util;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    /// <summary>
    ///     
    /// </summary>
    public class MethodExpressionServiceTaskTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestSetServiceResultToProcessVariables()
        {
            IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
            variables["okReturningService"] = new ObjectValueImpl(new OkReturningService());

            var pi = runtimeService.StartProcessInstanceByKey("setServiceResultToProcessVariables", variables);

            Assert.AreEqual("ok", runtimeService.GetVariable(pi.Id, "result"));
        }
    }
}