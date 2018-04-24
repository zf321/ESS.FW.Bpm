using System.Linq;
using ESS.FW.Bpm.Model.Dmn;
using ESS.FW.Bpm.Model.Dmn.instance;
using NUnit.Framework;


namespace Engine.Tests.Api.Repository
{
    

	public class DmnModelElementInstanceCmdTest : PluggableProcessEngineTestCase
	{

	  private const string DECISION_KEY = "one";


        [Test]
        public virtual void testRepositoryService()
	  {
		string decisionDefinitionId = repositoryService.CreateDecisionDefinitionQuery(c=>c.Key== DECISION_KEY).First().Id;

		IDmnModelInstance modelInstance = repositoryService.GetDmnModelInstance(decisionDefinitionId);
		Assert.NotNull(modelInstance);

		var decisions = modelInstance.GetModelElementsByType<IDecision>(typeof(IDecision));
		Assert.AreEqual(1, decisions.Count());

		var decisionTables = modelInstance.GetModelElementsByType<IDecisionTable>(typeof(IDecisionTable));
		Assert.AreEqual(1, decisionTables.Count());

		var inputs = modelInstance.GetModelElementsByType<IInput>(typeof(IInput));
		Assert.AreEqual(1, inputs.Count());

		var outputs = modelInstance.GetModelElementsByType<IOutput>(typeof(IOutput));
		Assert.AreEqual(1, outputs.Count());

		var rules = modelInstance.GetModelElementsByType<IRule>(typeof(IRule));
		Assert.AreEqual(2, rules.Count());
	  }

	}

}