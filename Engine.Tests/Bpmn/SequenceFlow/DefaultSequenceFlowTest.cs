using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.SequenceFlow
{
    [TestFixture]
    public class DefaultSequenceFlowTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestDefaultSequenceFlowOnTask()
	    {

	        IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
	        variables["input"] = new IntegerValueImpl(2);
            
	        string procId = runtimeService.StartProcessInstanceByKey("defaultSeqFlow", variables).Id;
	        Assert.NotNull(
	            runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procId&& c.ActivityId =="task2").First());
	        variables["input"] = new IntegerValueImpl(3);
	        procId = runtimeService.StartProcessInstanceByKey("defaultSeqFlow", variables).Id;
	        Assert.NotNull(
	            runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procId&& c.ActivityId =="task3").First());
	        variables["input"] = new IntegerValueImpl(123);
	        procId = runtimeService.StartProcessInstanceByKey("defaultSeqFlow", variables).Id;
	        Assert.NotNull(
	            runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==procId&& c.ActivityId =="task1").First());
	    }
	}

}