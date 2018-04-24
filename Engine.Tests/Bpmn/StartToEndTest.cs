using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Engine.Tests.Bpmn
{
    /// <summary>
    /// </summary>
    public class StartToEndTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment]
        public virtual void TestStartToEnd()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("startToEnd");
            AssertProcessEnded(processInstance.Id);
            Assert.True(processInstance.IsEnded);
        }

        [Test]
        public void TestJsonConvert()
        {
            var result = runtimeService.CreateExecutionQuery().FirstOrDefault();
            var s = JsonConvert.SerializeObject(result);
        }

        [Test]
        public virtual void TestBpmn()
        {
            var processInstance = runtimeService.StartProcessInstanceByKey("Process_rrrr");
            AssertProcessEnded(processInstance.Id);
            Assert.True(processInstance.IsEnded);
        }
    }
}