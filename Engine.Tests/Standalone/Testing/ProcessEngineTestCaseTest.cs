using System.Linq;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessEngineTestCaseTest : ProcessEngineTestCase
    {
        protected internal virtual string CurrentHistoryLevel()
        {
            return ProcessEngine.ProcessEngineConfiguration.History;
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        [Test]
        public virtual void TestRequiredHistoryLevelActivity()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryActivity)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryAudit)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)]
        [Test]
        public virtual void TestRequiredHistoryLevelAudit()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryAudit).Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]
        [Test]
        public virtual void TestRequiredHistoryLevelFull()
        {
            Assert.That(CurrentHistoryLevel(), Is.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [Test]
        [Deployment]
        public virtual void TestSimpleProcess()
        {
            RuntimeService.StartProcessInstanceByKey("simpleProcess");

            var task = TaskService.CreateTaskQuery().First();
            Assert.AreEqual("My Task", task.Name);

            TaskService.Complete(task.Id);
            Assert.AreEqual(0, RuntimeService.CreateProcessInstanceQuery().Count());
        }
    }
}