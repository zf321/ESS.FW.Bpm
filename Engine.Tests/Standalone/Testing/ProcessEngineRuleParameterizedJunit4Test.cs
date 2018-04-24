using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    /// <summary>
    /// </summary>
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Parameterized.class) public class ProcessEngineRuleParameterizedJunit4Test
    [TestFixture]
    public class ProcessEngineRuleParameterizedJunit4Test
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public test.ProcessEngineRule engineRule = new test.util.ProvidedProcessEngineRule();
        public ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();

        public ProcessEngineRuleParameterizedJunit4Test(int parameter)
        {
        }

        public static ICollection<object[]> Data()
        {
            return new[]
            {
                new object[] {1},
                new object[] {2}
            };
        }

        /// <summary>
        ///     Unnamed Deployment annotations don't work with parameterized Unit tests
        /// </summary>
        [Test]
        [Deployment]
        public virtual void RuleUsageExample()
        {
            var runtimeService = EngineRule.RuntimeService;
            runtimeService.StartProcessInstanceByKey("ruleUsage");

            var taskService = EngineRule.TaskService;
            var task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("My Task", task.Name);

            taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        [Test]
        [Deployment(
            "resources/standalone/testing/ProcessEngineRuleParameterizedJunit4Test.RuleUsageExample.bpmn20.xml"
        )]
        public virtual void RuleUsageExampleWithNamedAnnotation()
        {
            var runtimeService = EngineRule.RuntimeService;
            runtimeService.StartProcessInstanceByKey("ruleUsage");

            var taskService = EngineRule.TaskService;
            var task = taskService.CreateTaskQuery().First();
            Assert.AreEqual("My Task", task.Name);

            taskService.Complete(task.Id);
            Assert.AreEqual(0, runtimeService.CreateProcessInstanceQuery().Count());
        }

        /// <summary>
        ///     The rule should work with tests that have no deployment annotation
        /// </summary>
        [Test]
        public virtual void TestWithoutDeploymentAnnotation()
        {
            Assert.AreEqual("aString", "aString");
        }
    }
}