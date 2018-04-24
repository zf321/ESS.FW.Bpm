using System.Linq;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.History;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class CustomHistoryTest : ResourceProcessEngineTestCase
    {
        public CustomHistoryTest() : base("resources/standalone/history/customhistory.Camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment(new[] {"resources/api/oneTaskProcess.bpmn20.xml"})]
        public virtual void TestReceivesVariableUpdates()
        {
            // given
            var instance = runtimeService.StartProcessInstanceByKey("oneTaskProcess");

            // when
            var value = "a Variable Value";
            runtimeService.SetVariable(instance.Id, "aStringVariable", value);
            runtimeService.SetVariable(instance.Id, "aBytesVariable", value.GetBytes());

            // then the historic variable instances and their values exist
            Assert.AreEqual(2, historyService.CreateHistoricVariableInstanceQuery().Count());

            var historicStringVariable =
                historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("aStringVariable")*/.First();
            Assert.NotNull(historicStringVariable);
            Assert.AreEqual(value, historicStringVariable.Value);

            var historicBytesVariable =
                historyService.CreateHistoricVariableInstanceQuery()/*.VariableName("aBytesVariable")*/.First();
            Assert.NotNull(historicBytesVariable);
            Assert.Equals(value.GetBytes(), (byte[]) historicBytesVariable.Value);

            // then the historic variable updates and their values exist
            Assert.AreEqual(2, historyService.CreateHistoricDetailQuery()/*.VariableUpdates()*/.Count());

            var historicStringVariableUpdate =
                (IHistoricVariableUpdate)
                historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(historicStringVariable.Id)
                    .First();

            Assert.NotNull(historicStringVariableUpdate);
            Assert.AreEqual(value, historicStringVariableUpdate.Value);

            var historicByteVariableUpdate =
                (IHistoricVariableUpdate)
                historyService.CreateHistoricDetailQuery()
                    /*.VariableUpdates()*/
                    ////.VariableInstanceId(historicBytesVariable.Id)
                    .First();
            Assert.NotNull(historicByteVariableUpdate);
            Assert.Equals(value.GetBytes(), (byte[]) historicByteVariableUpdate.Value);
        }
    }
}