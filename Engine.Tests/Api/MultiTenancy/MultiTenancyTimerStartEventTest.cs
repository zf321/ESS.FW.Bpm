using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy
{
    
	public class MultiTenancyTimerStartEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyTimerStartEventTest()
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


	  protected internal static readonly IBpmnModelInstance PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().StartEvent().TimerWithDuration("PT1M").UserTask().EndEvent().Done();

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testRule);
	  //public RuleChain ruleChain;

	  protected internal IManagementService managementService;
	  protected internal IRuntimeService runtimeService;
	  protected internal IRepositoryService repositoryService;

      [SetUp]
	  public virtual void initServices()
	  {
		managementService = engineRule.ManagementService;
		runtimeService = engineRule.RuntimeService;
		repositoryService = engineRule.RepositoryService;
	  }


        [Test]
        public virtual void startProcessInstanceWithTenantId()
	  {

		testRule.DeployForTenant(TENANT_ONE, PROCESS);

		IJob job = managementService.CreateJobQuery().First();
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ONE));

		managementService.ExecuteJob(job.Id);

		IProcessInstance processInstance = runtimeService.CreateProcessInstanceQuery().First();
		Assert.That(processInstance!=null);
		Assert.That(processInstance.TenantId, Is.EqualTo(TENANT_ONE));
	  }


        [Test]
        public virtual void startProcessInstanceTwoTenants()
	  {

		testRule.DeployForTenant(TENANT_ONE, PROCESS);
		testRule.DeployForTenant(TENANT_TWO, PROCESS);

		IJob jobForTenantOne = managementService.CreateJobQuery(c=>c.TenantId == TENANT_ONE).First();
		Assert.That(jobForTenantOne!=null);
		managementService.ExecuteJob(jobForTenantOne.Id);

		IJob jobForTenantTwo = managementService.CreateJobQuery(c=>c.TenantId == TENANT_TWO).First();
		Assert.That(jobForTenantTwo!=null);
		managementService.ExecuteJob(jobForTenantTwo.Id);

		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(runtimeService.CreateProcessInstanceQuery(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void deleteJobsWhileUndeployment()
	  {

		 IDeployment deploymentForTenantOne = testRule.DeployForTenant(TENANT_ONE, PROCESS);
		 IDeployment deploymentForTenantTwo = testRule.DeployForTenant(TENANT_TWO, PROCESS);

		 IQueryable<IJob> query = managementService.CreateJobQuery();
		 Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		 Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));

		 repositoryService.DeleteDeployment(deploymentForTenantOne.Id, true);

		 Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
		 Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));

		 repositoryService.DeleteDeployment(deploymentForTenantTwo.Id, true);

		 Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
		 Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }


        [Test]
        public virtual void dontCreateNewJobsWhileReDeployment()
	  {

		testRule.DeployForTenant(TENANT_ONE, PROCESS);
		testRule.DeployForTenant(TENANT_TWO, PROCESS);
		testRule.DeployForTenant(TENANT_ONE, PROCESS);

		IQueryable<IJob> query = managementService.CreateJobQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }


        [Test]
        public virtual void failedJobRetryTimeCycle()
	  {

		testRule.DeployForTenant(TENANT_ONE, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("failingProcess").StartEvent().TimerWithDuration("PT1M").CamundaFailedJobRetryTimeCycle("R5/PT1M").ServiceTask().CamundaExpression("${failing}").EndEvent().Done());

		testRule.DeployForTenant(TENANT_TWO, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("failingProcess").StartEvent().TimerWithDuration("PT1M").CamundaFailedJobRetryTimeCycle("R4/PT1M").ServiceTask().CamundaExpression("${failing}").EndEvent().Done());

	      IList<IJob> jobs = managementService.CreateJobQuery()
	         // /*.Timers()*/
	          .ToList();
		executeFailingJobs(jobs);

		IJob jobTenantOne = managementService.CreateJobQuery(c=>c.TenantId == TENANT_ONE).First();
		IJob jobTenantTwo = managementService.CreateJobQuery(c=>c.TenantId == TENANT_TWO).First();

		Assert.That(jobTenantOne.Retries, Is.EqualTo(4));
		Assert.That(jobTenantTwo.Retries, Is.EqualTo(3));
	  }


        [Test]
        public virtual void timerStartEventWithTimerCycle()
	  {

		testRule.DeployForTenant(TENANT_ONE, ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().StartEvent().TimerWithCycle("R2/PT1M").UserTask().EndEvent().Done());

		// execute first timer cycle
		IJob job = managementService.CreateJobQuery().First();
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ONE));
		managementService.ExecuteJob(job.Id);

		// execute second timer cycle
		job = managementService.CreateJobQuery().First();
		Assert.That(job.TenantId, Is.EqualTo(TENANT_ONE));
		managementService.ExecuteJob(job.Id);

		 IQueryable<IProcessInstance> query = runtimeService.CreateProcessInstanceQuery();
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
		Assert.That(query
           // .WithoutTenantId()
            .Count(), Is.EqualTo(0L));
	  }

	  protected internal virtual void executeFailingJobs(IList<IJob> jobs)
	  {
		foreach (IJob job in jobs)
		{

		  try
		  {
			managementService.ExecuteJob(job.Id);

			Assert.Fail("expected exception");
		  }
		  catch (System.Exception)
		  {
		  }
		}
	  }

	}

}