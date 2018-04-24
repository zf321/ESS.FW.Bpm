
//using NUnit.Framework;
//using ResourceProcessEngineTestCase = ESS.FW.Bpm.Engine.Tests.ResourceProcessEngineTestCase;

//namespace ESS.FW.Bpm.Engine.Tests.Standalone.Deploy
//{
//    /// <summary>
//	/// 
//	/// </summary>
//	public class CmmnTransformListenerTest : ResourceProcessEngineTestCase
//	{

//	  public CmmnTransformListenerTest() : base("resources/standalone/deploy/cmmn.Transform.listener.Camunda.cfg.xml")
//	  {
//	  }

//[TearDown]
//	  public virtual void tearDown()
//	  {
//		TestCmmnTransformListener.Reset();
//	  }

//[Test]
//[Deployment]
//	  public virtual void TestListenerInvocation()
//	  {
//		// Check if case definition has different key
//		Assert.AreEqual(0, repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey("testCase").Count());
//		Assert.AreEqual(0, repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey("testCase-modified").Count());
//		Assert.AreEqual(1, repositoryService.CreateCaseDefinitionQuery().CaseDefinitionKey("testCase-modified-modified").Count());

//		Assert.AreEqual(1, numberOfRegistered(typeof(Definitions)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(Case)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(CasePlanModel)));
//		Assert.AreEqual(3, numberOfRegistered(typeof(HumanTask)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(ProcessTask)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(CaseTask)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(DecisionTask)));
//		// 3x HumanTask, 1x ProcessTask, 1x CaseTask, 1x DecisionTask, 1x Task
//		Assert.AreEqual(7, numberOfRegistered(typeof(Task)));
//		// 1x CasePlanModel, 1x Stage
//		Assert.AreEqual(2, numberOfRegistered(typeof(Stage)));
//		Assert.AreEqual(1, numberOfRegistered(typeof(Milestone)));
//		// Note: EventListener is currently not supported!
//		Assert.AreEqual(0, numberOfRegistered(typeof(EventListener)));
//		Assert.AreEqual(3, numberOfRegistered(typeof(Sentry)));

//		Assert.AreEqual(11, TestCmmnTransformListener.cmmnActivities.Count);
//		Assert.AreEqual(24, TestCmmnTransformListener.ModelElementInstances.Count);
//		Assert.AreEqual(3, TestCmmnTransformListener.sentryDeclarations.Count);
//	  }

//	}

//}