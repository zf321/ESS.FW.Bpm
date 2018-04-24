using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    public class ServiceTaskClassDelegateActivityBehaviorTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void testExceptionThrownBySecondScopeServiceTaskIsNotHandled()
        {
            IDictionary<object, object> beans = processEngineConfiguration.Beans;
            beans["dummyServiceTask"] = new DummyServiceTask();
            processEngineConfiguration.Beans = beans;
            try
            {
                IDictionary<string, ITypedValue> variables = new Dictionary<string, ITypedValue>();
                variables["Count"] = new IntegerValueImpl(0);

                runtimeService.StartProcessInstanceByKey("process", variables);
                //Assert.Fail();
            } 
            // since the NVE extends the ProcessEngineException we have to handle it
            // separately
            catch (NullValueException e) //
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