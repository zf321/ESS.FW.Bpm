using System.Linq;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{
    [TestFixture]
    public class DecisionInstanceHistoryTest : ResourceProcessEngineTestCase
    {
        public const string DecisionSingleOutputDmn =
            "resources/history/HistoricDecisionInstanceTest.DecisionSingleOutput.Dmn11.xml";

        public DecisionInstanceHistoryTest()
            : base("resources/standalone/history/decisionInstanceHistory.Camunda.cfg.xml")
        {
        }

        [Test]
        [Deployment(new[] {DecisionSingleOutputDmn})]
        public virtual void TestDecisionDefinitionPassedToHistoryLevel()
        {
            var historyLevel = (RecordHistoryLevel) processEngineConfiguration.HistoryLevel;
            var decisionDefinition =
                repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== "testDecision").First();

            var variables = ESS.FW.Bpm.Engine.Variable.Variables.CreateVariables().PutValue("input1", true);
            decisionService.EvaluateDecisionTableByKey("testDecision", variables);

            var producedHistoryEvents = historyLevel.ProducedHistoryEvents;
            Assert.AreEqual(1, producedHistoryEvents.Count);

            var producedHistoryEvent = producedHistoryEvents[0];
            Assert.AreEqual(HistoryEventTypes.DmnDecisionEvaluate, producedHistoryEvent.EventType);

            var entity = (IDecisionDefinition) producedHistoryEvent.Entity;
            Assert.NotNull(entity);
            Assert.AreEqual(decisionDefinition.Id, entity.Id);
        }
    }
}