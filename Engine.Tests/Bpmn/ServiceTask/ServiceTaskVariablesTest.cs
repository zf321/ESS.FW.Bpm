using System;
using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.ServiceTask
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ServiceTaskVariablesTest : PluggableProcessEngineTestCase
    {

        internal static bool isNullInDelegate2;
        internal static bool isNullInDelegate3;

        [Test]
        [Deployment]
        public virtual void testSerializedVariablesBothAsync()
        {
            // in this test, there is an async cont. both before the second and the
            // third service task in the sequence

            runtimeService.StartProcessInstanceByKey("process");
            WaitForJobExecutorToProcessAllJobs(10000);

            lock (typeof(ServiceTaskVariablesTest))
            {
                Assert.True(isNullInDelegate2);
                Assert.True(isNullInDelegate3);
            }
        }

        [Test]
        [Deployment]
        public virtual void testSerializedVariablesThirdAsync()
        {
            // in this test, only the third service task is async

            runtimeService.StartProcessInstanceByKey("process");
            WaitForJobExecutorToProcessAllJobs(10000);

            lock (typeof(ServiceTaskVariablesTest))
            {
                Assert.True(isNullInDelegate2);
                Assert.True(isNullInDelegate3);
            }
        }
    }
    [Serializable]
    public class Variable
    {
        internal const long serialVersionUID = 1L;
        public string value;
    }
    public class Delegate1 : IJavaDelegate
    {
        internal static bool isNullInDelegate2;
        internal static bool isNullInDelegate3;

        public void Execute(IBaseDelegateExecution execution)
        {
            var v = new Variable();
            execution.SetVariable("variable", v);
            v.value = "delegate1";
        }
    }

    public class Delegate2 : IJavaDelegate
    {
        internal static bool isNullInDelegate2;
        internal static bool isNullInDelegate3;
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            var v = (Variable)execution.GetVariable("variable");
            lock (typeof(ServiceTaskVariablesTest))
            {
                // we expect this to be 'true'
                isNullInDelegate2 = !ReferenceEquals(v.value, null) && v.value.Equals("delegate1");
            }
            v.value = "delegate2";
        }
    }

    public class Delegate3 : IJavaDelegate
    {

        internal static bool isNullInDelegate2;
        internal static bool isNullInDelegate3;
        public void Execute(IBaseDelegateExecution execution)
        {
            var v = (Variable)execution.GetVariable("variable");
            lock (typeof(ServiceTaskVariablesTest))
            {
                // we expect this to be 'true' as well
                isNullInDelegate3 = !ReferenceEquals(v.value, null) && v.value.Equals("delegate2");
            }
        }
    }
}