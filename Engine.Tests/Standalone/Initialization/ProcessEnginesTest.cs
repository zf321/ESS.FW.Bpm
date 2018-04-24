using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Initialization
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class ProcessEnginesTest : PvmTestCase
    {
        [SetUp]
        protected internal virtual void SetUp()
        {
            //base.SetUp();
            ProcessEngines.Destroy();
            ProcessEngines.Init();
        }

        [TearDown]
        protected internal virtual void TearDown()
        {
            ProcessEngines.Destroy();
            //base.TearDown();
        }

        [Test]
        public virtual void TestProcessEngineInfo()
        {
            var processEngineInfos = ProcessEngines.ProcessEngineInfos;
            Assert.AreEqual(1, processEngineInfos.Count);

            var processEngineInfo = processEngineInfos[0];
            Assert.IsNull(processEngineInfo.Exception);
            Assert.NotNull(processEngineInfo.Name);
            Assert.NotNull(processEngineInfo.ResourceUrl);

            var processEngine = ProcessEngines.GetProcessEngine(ProcessEngines.NameDefault);
            Assert.NotNull(processEngine);
        }
    }
}