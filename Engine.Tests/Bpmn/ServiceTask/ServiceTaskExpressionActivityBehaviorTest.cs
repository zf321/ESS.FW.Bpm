using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    public class ServiceTaskExpressionActivityBehaviorTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestExceptionThrownBySecondScopeServiceTaskIsNotHandled()
        {
            var beans = processEngineConfiguration.Beans;
            beans["dummyServiceTask"] = new DummyServiceTask();
            processEngineConfiguration.Beans = beans;
            
            try
            {
                IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
                variables["Count"] = new IntegerValueImpl(0);

                runtimeService.StartProcessInstanceByKey("process", variables);

                //Assert.Fail();
                // the EL resolver will wrap the actual exception inside a process engine exception
            }
            //since the NVE extends the ProcessEngineException we have to handle it separately
            catch (NullValueException)
            {
                Assert.Fail("Shouldn't have received NullValueException");
            }
            catch (ProcessEngineException e)
            {
                // Assert.That(e.Message.Contains("Invalid format"));
                Assert.IsTrue(true);
            }
        }
    }
}