using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    /// <summary>
    ///     Checks if the test is ignored than the current history level is lower than
    ///     the required history level which is specified on the super class.
    /// </summary>
    public class ProcessEngineRuleRequiredHistoryLevelSuperClassTest : ProcessEngineRuleRequiredHistoryLevelClassTest
    {
        [Test]
        public virtual void RequiredHistoryLevelOnSuperClass()
        {
            Assert.That(CurrentHistoryLevel(),
                Is.EqualTo(ProcessEngineConfiguration.HistoryAudit).Or.EqualTo(ProcessEngineConfiguration.HistoryFull));
        }
    }
}