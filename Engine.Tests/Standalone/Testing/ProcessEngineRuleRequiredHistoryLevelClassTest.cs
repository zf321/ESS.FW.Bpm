using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryAudit)] 
    [TestFixture]
    public class ProcessEngineRuleRequiredHistoryLevelClassTest
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final test.ProcessEngineRule engineRule = new test.util.ProvidedProcessEngineRule();
        public readonly ProcessEngineRule EngineRule = new ProvidedProcessEngineRule();

        protected internal virtual string CurrentHistoryLevel()
        {
            return EngineRule.ProcessEngine.ProcessEngineConfiguration.History;
        }

        [Test]
        [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
        public virtual void OverrideRequiredHistoryLevelOnClass()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryActivity)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryAudit)
                    .Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }

        [Test]
        public virtual void RequiredHistoryLevelOnClass()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryAudit).Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }
    }
}