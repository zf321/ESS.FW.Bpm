using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    ///     This test ensures that if a user selects
    ///     <seealso cref="ProcessEngineConfiguration#HISTORY_VARIABLE" />, the level is internally
    ///     mapped to <seealso cref="ProcessEngineConfigurationImpl#HISTORYLEVEL_ACTIVITY" />.
    /// </summary>
    [TestFixture]
    public class VariableHistoryLevelCompatibilityTest : ResourceProcessEngineTestCase
    {
        public VariableHistoryLevelCompatibilityTest()
            : base("resources/standalone/history/variablehistory.Camunda.cfg.xml")
        {
        }

        [Test]
        public virtual void TestCompatibilty()
        {
            var historyLevel = processEngineConfiguration.HistoryLevel.Id;
            Assert.AreEqual(ProcessEngineConfigurationImpl.HistorylevelActivity, historyLevel);
        }
    }
}