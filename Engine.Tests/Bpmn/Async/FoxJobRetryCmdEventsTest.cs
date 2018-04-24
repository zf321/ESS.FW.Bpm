//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Repository;
//using ESS.FW.Bpm.Engine.Runtime;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Bpmn.Async
//{
    
//    /// <summary>
//	/// 
//	/// </summary>
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @RunWith(Parameterized.class) public class FoxJobRetryCmdEventsTest
//	public class FoxJobRetryCmdEventsTest
//	{
//		private bool InstanceFieldsInitialized = false;

//		public FoxJobRetryCmdEventsTest()
//		{
//			if (!InstanceFieldsInitialized)
//			{
//				InitializeInstanceFields();
//				InstanceFieldsInitialized = true;
//			}
//		}

//		private void InitializeInstanceFields()
//		{
//			testRule = new ProcessEngineTestRule(engineRule);
//			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
//		}


//	  public ProcessEngineRule engineRule = new ProcessEngineRule();
//	  public ProcessEngineTestRule testRule;
////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.ExpectedException thrown = org.junit.Rules.ExpectedException.None();
//	  //public ExpectedException thrown = ExpectedException.None();

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
//	  //public RuleChain ruleChain;


////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Parameterized.Parameter public RetryCmdDeployment deployment;
//	  public RetryCmdDeployment deployment;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Parameterized.Parameters(name = "deployment {index}") public static java.util.Collection<RetryCmdDeployment[]> scenarios()
//	  public static ICollection<RetryCmdDeployment[]> scenarios()
//	  {return null;
//            //return RetryCmdDeployment.asParameters(Deployment().WithEventProcess(prepareSignalEventProcess()), Deployment().WithEventProcess(prepareMessageEventProcess()), Deployment().WithEventProcess(prepareEscalationEventProcess()), Deployment().WithEventProcess(prepareCompensationEventProcess()));
//        }

//	  private IDeployment currentDeployment;

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Before public void setUp()
//	  public virtual void setUp()
//	  {
//		currentDeployment = testRule.Deploy(deployment.BpmnModelInstances);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void testFailedIntermediateThrowingSignalEventAsync()
//	  public virtual void testFailedIntermediateThrowingSignalEventAsync()
//	  {
//		IProcessInstance pi = engineRule.RuntimeService.StartProcessInstanceByKey(RetryCmdDeployment.PROCESS_ID);
//		AssertJobRetries(pi);
//	  }

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @After public void tearDown()
//	  public virtual void tearDown()
//	  {
//		engineRule.RepositoryService.DeleteDeployment(currentDeployment.Id,true,true);
//	  }

//	  protected internal virtual void AssertJobRetries(IProcessInstance pi)
//	  {
//		Assert.That(pi,Is.Not.EqualTo(null));

//		IJob job = fetchJob(pi.ProcessInstanceId);

//		try
//		{
//		  engineRule.ManagementService.ExecuteJob(job.Id);
//		}
//		catch (Exception)
//		{
//		}

//		// update job
//		job = fetchJob(pi.ProcessInstanceId);
//		Assert.That(job.Retries,Is.EqualTo(4));
//	  }

//	  protected internal virtual IJob fetchJob(string ProcessInstanceId)
//	  {
//		return engineRule.ManagementService.CreateJobQuery(c=>c.ProcessInstanceId == ProcessInstanceId).First();
//	  }


//	}

//}