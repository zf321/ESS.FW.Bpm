using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.External
{
    [TestFixture]
	public class ExternalTaskParseTest : PluggableProcessEngineTestCase
	{

	  public virtual void testParseExternalTaskWithoutTopic()
	  {
		IDeploymentBuilder deploymentBuilder = repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/external/ExternalTaskParseTest.TestParseExternalTaskWithoutTopic.bpmn20.xml");

		try
		{
		  deploymentBuilder.Deploy();
		  Assert.Fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  AssertTextPresent("External tasks must specify a 'topic' attribute in the camunda namespace", e.Message);
		}
	  }
	}

}