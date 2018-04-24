using System;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event
{

    [TestFixture]
    public class IntermediateNoneEventTest : PluggableProcessEngineTestCase
    {
        private static bool listenerExcecuted = false;

        public class MyExecutionListener : IDelegateListener<IBaseDelegateExecution>
        {

            public void Notify(IBaseDelegateExecution instance)
            {
                listenerExcecuted = true;
            }
        }

        [Test]
        public void TestClassNameToType()
        {
            var type= ReflectUtil.LoadClass("ESS.FW.Bpm.Engine.Tests.Bpmn.Event.IntermediateNoneEventTest+MyExecutionListener");
            Assert.AreNotEqual(null, type);
            var type_2 = ReflectUtil.LoadClass("ESS.FW.Bpm.Engine.Impl.Delegate.DefaultDelegateInterceptor,ESS.FW.Bpm.Engine");
            Assert.AreNotEqual(null, type_2);
        }
        [Test]
        public void TestGenericTypeName()
        {
            
            Type t = typeof(ICommand<string>);
            Assert.AreEqual("ICommand<String>", GetTypeName(t));
        }
        private string GetTypeName(Type type)
        {
            string name = type.Name;
            if (type.IsGenericType)
            {
                name = name.Substring(0, name.IndexOf('`'))+"<";
                foreach (var item in type.GenericTypeArguments)
                {
                    name += GetTypeName(item);
                }
                name += ">";
            }
            return name;
        }
        [Test]
        [Deployment]
        public virtual void testIntermediateNoneTimerEvent()
        {
            Assert.IsFalse(listenerExcecuted);
            IProcessInstance pi = runtimeService.StartProcessInstanceByKey("intermediateNoneEventExample");
            AssertProcessEnded(pi.ProcessInstanceId);
            Assert.True(listenerExcecuted);
        }


    }
}