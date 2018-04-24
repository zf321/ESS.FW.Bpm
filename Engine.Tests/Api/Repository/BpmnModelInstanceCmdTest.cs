using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using NUnit.Framework;


namespace Engine.Tests.Api.Repository
{
    

	/// <summary>
	/// 
	/// </summary>
	[TestFixture]public class BpmnModelInstanceCmdTest : PluggableProcessEngineTestCase
	{

	  private const string PROCESS_KEY = "one";


        [Test]
        public virtual void testRepositoryService()
	  {
		string processDefinitionId = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == PROCESS_KEY).First().Id;

		IBpmnModelInstance modelInstance = repositoryService.GetBpmnModelInstance(processDefinitionId);
		Assert.NotNull(modelInstance);

	      IEnumerable<IEvent> events = modelInstance.GetModelElementsByType<IEvent>(typeof(IEvent));
		Assert.AreEqual(2, events.Count());

	      IEnumerable<ISequenceFlow> sequenceFlows = modelInstance.GetModelElementsByType< ISequenceFlow>(typeof(ISequenceFlow));
		Assert.AreEqual(1, sequenceFlows.Count());

		IStartEvent startEvent = (IStartEvent)modelInstance.GetModelElementById/*<IStartEvent>*/("start");
		Assert.NotNull(startEvent);
	  }

	}

}