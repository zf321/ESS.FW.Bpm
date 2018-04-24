using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    /// <summary>
    ///     Test runners follow the this rule:
    ///     - if the class extends Testcase, run as Junit 3
    ///     - otherwise use Junit 4
    ///     So this test can be included in the regular test suite without problems.
    /// </summary>
    [TestFixture]
    public class ProcessEngineRuleJunit4Test
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public test.ProcessEngineRule engineRule = new test.util.ProvidedProcessEngineRule();
        public ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();

        protected internal virtual string CurrentHistoryLevel()
        {
            return EngineRule.ProcessEngine.ProcessEngineConfiguration.History;
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        [Test]
        public virtual void RequiredHistoryLevelActivity()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryActivity)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryAudit)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit) ]
        [Test]
        public virtual void RequiredHistoryLevelAudit()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryAudit).Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull) ]
        [Test]
        public virtual void RequiredHistoryLevelFull()
        {
            Assert.That(CurrentHistoryLevel(), Is.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }


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