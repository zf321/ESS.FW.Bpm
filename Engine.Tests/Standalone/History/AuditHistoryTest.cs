using System.Linq;
using ESS.FW.Bpm.Engine.Common;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class AuditHistoryTest : ResourceProcessEngineTestCase
    {
        public AuditHistoryTest() : base("resources/standalone/history/audithistory.Camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment(new[] {"resources/api/oneTaskProcess.bpmn20.xml"})]
        public virtual void TestReceivesNoHistoricVariableUpdatesAsDetails()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // when
            var value = "a Variable Value";
            runtimeService.SetVariable(instance.Id, "aStringVariable", value);
            runtimeService.SetVariable(instance.Id, "aBytesVariable", value.GetBytes());

            var newValue = "a new Variable Value";
            runtimeService.SetVariable(instance.Id, "aStringVariable", newValue);
            runtimeService.SetVariable(instance.Id, "aBytesVariable", newValue.GetBytes());

            // then the historic variable instances exist and they have the latest values
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery().Count());

            var historicStringVariable =
                historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("aStringVariable")*/.First();
            Assert.NotNull(historicStringVariable);
            Assert.AreEqual(newValue, historicStringVariable.Value);

            var historicBytesVariable =
                historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("aBytesVariable")*/.First();
            Assert.NotNull(historicBytesVariable);
            Assert.Equals(newValue.GetBytes(), (byte[]) historicBytesVariable.Value);

            // and no historic details exist
            Assert.AreEqual(0, historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Count());
        }
    }
}