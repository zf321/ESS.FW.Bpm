using NUnit.Framework;

namespace Engine.Tests.Bpmn.Job
{


    /// <summary>
	/// 
	/// </summary>
    [TestFixture]
    public class DefaultJobPrioritizationBpmnTest : PluggableProcessEngineTestCase
    {
        [Test]
        public virtual void TestDefaultProducePrioritizedJobsSetting()
        {
            Assert.True(processEngineConfiguration.ProducePrioritizedJobs);
        }

    }

}