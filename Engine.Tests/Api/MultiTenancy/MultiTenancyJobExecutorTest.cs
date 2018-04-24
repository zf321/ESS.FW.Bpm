using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Identity;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;
using AssertingJavaDelegate = Engine.Tests.Api.Delegate.AssertingJavaDelegate;



namespace Engine.Tests.Api.MultiTenancy
{
    
	public class MultiTenancyJobExecutorTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyJobExecutorTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			//ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ID = "tenant1";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;


        [Test]
        public virtual void setAuthenticatedTenantForTimerStartEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().TimerWithDuration("PT1M").ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).UserTask().EndEvent().Done());

		AssertingJavaDelegate.AddAsserts(hasAuthenticatedTenantId(TENANT_ID));

		ClockUtil.CurrentTime = tomorrow();
		testRule.WaitForJobExecutorToProcessAllJobs();

		Assert.That(engineRule.TaskService.CreateTaskQuery().Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void setAuthenticatedTenantForIntermediateTimerEvent()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().IntermediateCatchEvent().TimerWithDuration("PT1M").ServiceTask().CamundaClass(typeof(AssertingJavaDelegate).FullName).EndEvent().Done());

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("process");

		AssertingJavaDelegate.AddAsserts(hasAuthenticatedTenantId(TENANT_ID));

		ClockUtil.CurrentTime = tomorrow();
		testRule.WaitForJobExecutorToProcessAllJobs();
		testRule.AssertProcessEnded(processInstance.Id);
	  }


        [Test]
        public virtual void setAuthenticatedTenantForAsyncJob()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask().CamundaAsyncBefore().Func(m=>m.CamundaClass=(typeof(AssertingJavaDelegate).FullName)).EndEvent().Done());

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("process");

		AssertingJavaDelegate.AddAsserts(hasAuthenticatedTenantId(TENANT_ID));

		testRule.WaitForJobExecutorToProcessAllJobs();
		testRule.AssertProcessEnded(processInstance.Id);
	  }


        [Test]
        public virtual void dontSetAuthenticatedTenantForJobWithoutTenant()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		testRule.Deploy(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask().CamundaAsyncBefore().Func(m=>m.CamundaClass=(typeof(AssertingJavaDelegate).FullName)).EndEvent().Done());

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("process");

		AssertingJavaDelegate.AddAsserts(hasNoAuthenticatedTenantId());

		testRule.WaitForJobExecutorToProcessAllJobs();
		testRule.AssertProcessEnded(processInstance.Id);
	  }


        [Test]
        public virtual void dontSetAuthenticatedTenantWhileManualJobExecution()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
		testRule.DeployForTenant(TENANT_ID, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process").StartEvent().ServiceTask().CamundaAsyncBefore().Func(m=>m.CamundaClass=(typeof(AssertingJavaDelegate).FullName)).EndEvent().Done());

		IProcessInstance processInstance = engineRule.RuntimeService.StartProcessInstanceByKey("process");

		AssertingJavaDelegate.AddAsserts(hasNoAuthenticatedTenantId());

		testRule.ExecuteAvailableJobs();
		testRule.AssertProcessEnded(processInstance.Id);
	  }


        [Test]
        protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasAuthenticatedTenantId(string expectedTenantId)
	  {
		return new DelegateExecutionAsserterAnonymousInnerClass(expectedTenantId);
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass : Delegate.AssertingJavaDelegate.DelegateExecutionAsserter
	  {
		  private string expectedTenantId;

		  public DelegateExecutionAsserterAnonymousInnerClass(string expectedTenantId)
		  {
			  this.expectedTenantId = expectedTenantId;
		  }


		  public virtual void DoAssert(IDelegateExecution execution)
		  {
			IIdentityService identityService = execution.ProcessEngineServices.IdentityService;

			Authentication currentAuthentication = identityService.CurrentAuthentication;
			Assert.That(currentAuthentication!=null);
			Assert.Contains(currentAuthentication.TenantIds, new  List<string>(){ expectedTenantId });
		  }
	  }

	  protected internal static AssertingJavaDelegate.DelegateExecutionAsserter hasNoAuthenticatedTenantId()
	  {
		return new DelegateExecutionAsserterAnonymousInnerClass2();
	  }

	  private class DelegateExecutionAsserterAnonymousInnerClass2 : Delegate.AssertingJavaDelegate.DelegateExecutionAsserter
	  {
		  public DelegateExecutionAsserterAnonymousInnerClass2()
		  {
		  }


		  public virtual void DoAssert(IDelegateExecution execution)
		  {
			IIdentityService identityService = execution.ProcessEngineServices.IdentityService;

			Authentication currentAuthentication = identityService.CurrentAuthentication;
			Assert.That(currentAuthentication, Is.EqualTo(null));
		  }
	  }

	  protected internal virtual DateTime tomorrow()
	  {
		DateTime calendar = new DateTime();
		calendar.AddDays(1);

		return calendar;
	  }

     [TearDown]
	  public virtual void tearDown()
	  {
		AssertingJavaDelegate.Clear();
	  }

	}

}