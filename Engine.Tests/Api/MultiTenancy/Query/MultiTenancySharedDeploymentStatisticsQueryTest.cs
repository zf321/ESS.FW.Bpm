using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;
using ProcessEngineBootstrapRule = Engine.Tests.Util.ProcessEngineBootstrapRule;



namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	/// 
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class MultiTenancySharedDeploymentStatisticsQueryTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancySharedDeploymentStatisticsQueryTest()
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
			//tenantRuleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";

	  protected internal const string ONE_TASK_PROCESS_DEFINITION_KEY = "oneTaskProcess";
	  protected internal const string FAILED_JOBS_PROCESS_DEFINITION_KEY = "ExampleProcess";
	  protected internal const string ANOTHER_FAILED_JOBS_PROCESS_DEFINITION_KEY = "AnotherExampleProcess";

	  protected internal static StaticTenantIdTestProvider tenantIdProvider;


	  public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : Util.ProcessEngineBootstrapRule
	  {
		  public ProcessEngineBootstrapRuleAnonymousInnerClass()
		  {
		  }

		  public override ProcessEngineConfiguration ConfigureEngine(ProcessEngineConfigurationImpl configuration)
		  {

		  tenantIdProvider = new StaticTenantIdTestProvider(TENANT_ONE);
		  configuration.TenantIdProvider = tenantIdProvider;

		  return configuration;
		  }
	  }

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

	  protected internal ProcessEngineTestRule testRule;

	  protected internal IRuntimeService runtimeService;

	  protected internal IManagementService managementService;

	  protected internal IIdentityService identityService;

	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal static readonly IBpmnModelInstance oneTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ONE_TASK_PROCESS_DEFINITION_KEY).StartEvent().UserTask().Done();

	  protected internal static readonly IBpmnModelInstance failingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(FAILED_JOBS_PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaClass("api.Multitenancy.FailingDelegate").CamundaAsyncBefore().Done();

	  protected internal static readonly IBpmnModelInstance anotherFailingProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess(ANOTHER_FAILED_JOBS_PROCESS_DEFINITION_KEY).StartEvent().ServiceTask().CamundaClass("api.Multitenancy.FailingDelegate").CamundaAsyncBefore().Done();

    [SetUp]
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		identityService = engineRule.IdentityService;
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
	  }


	  public virtual void activeProcessInstancesCountWithNoAuthenticatedTenant()
	  {

		testRule.Deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("user", null, null);

	      IList<IDeploymentStatistics> deploymentStatistics = managementService.CreateDeploymentStatisticsQuery()
	          .ToList();

		// then
		Assert.AreEqual(1,deploymentStatistics.Count);
		// user must see only the process instances that belongs to no tenant
		Assert.AreEqual(1, deploymentStatistics[0].Instances);

	  }


	  public virtual void activeProcessInstancesCountWithAuthenticatedTenant()
	  {

		testRule.Deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery().ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		// user can see the process instances that belongs to tenant1 and instances that have no tenant  
		Assert.AreEqual(2, deploymentStatistics[0].Instances);

	  }


	  public virtual void activeProcessInstancesCountWithDisabledTenantCheck()
	  {

		testRule.Deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery().ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		Assert.AreEqual(3, deploymentStatistics[0].Instances);
	  }


	  public virtual void activeProcessInstancesCountWithMultipleAuthenticatedTenants()
	  {

		testRule.Deploy(oneTaskProcess);

		startProcessInstances(ONE_TASK_PROCESS_DEFINITION_KEY);

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_ONE, TENANT_TWO});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery().ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		// user can see all the active process instances 
		Assert.AreEqual(3, deploymentStatistics[0].Instances);

	  }


	  public virtual void failedJobsCountWithWithNoAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null, null);

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		Assert.AreEqual(1, deploymentStatistics[0].FailedJobs);

	  }


	  public virtual void failedJobsCountWithWithDisabledTenantCheck()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		Assert.AreEqual(3, deploymentStatistics[0].FailedJobs);

	  }


	  public virtual void failedJobsCountWithAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		Assert.AreEqual(2, deploymentStatistics[0].FailedJobs);
	  }


	  public virtual void failedJobsCountWithMultipleAuthenticatedTenants()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_ONE, TENANT_TWO});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		Assert.AreEqual(3, deploymentStatistics[0].FailedJobs);
	  }


	  public virtual void incidentsCountWithNoAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null, null);

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);

		IList<IIncidentStatistics> incidentStatistics = deploymentStatistics[0].IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(1, incidentStatistics[0].IncidentCount);
	  }


	  public virtual void incidentsCountWithDisabledTenantCheck()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);

		IList<IIncidentStatistics> incidentStatistics = deploymentStatistics[0].IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(3, incidentStatistics[0].IncidentCount);
	  }


	  public virtual void incidentsCountWithAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);

		IList<IIncidentStatistics> incidentStatistics = deploymentStatistics[0].IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(2, incidentStatistics[0].IncidentCount);
	  }


	  public virtual void incidentsCountWithMultipleAuthenticatedTenants()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null, new List<string>(){TENANT_ONE, TENANT_TWO});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidents()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		IList<IIncidentStatistics> incidentStatistics = deploymentStatistics[0].IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(3, incidentStatistics[0].IncidentCount);
	  }


	  public virtual void incidentsCountWithIncidentTypeAndAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeIncidentsForType("failedJob")*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);

		IList<IIncidentStatistics> incidentStatistics = deploymentStatistics[0].IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(2, incidentStatistics[0].IncidentCount);
	  }


	  public virtual void instancesFailedJobsAndIncidentsCountWithAuthenticatedTenant()
	  {

		testRule.Deploy(failingProcess,anotherFailingProcess);

		startProcessInstances(FAILED_JOBS_PROCESS_DEFINITION_KEY);
		startProcessInstances(ANOTHER_FAILED_JOBS_PROCESS_DEFINITION_KEY);

		testRule.ExecuteAvailableJobs();

		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IList<IDeploymentStatistics> deploymentStatistics =  managementService.CreateDeploymentStatisticsQuery()/*.IncludeFailedJobs()*//*.IncludeIncidents()*/.ToList();

		// then
		Assert.AreEqual(1, deploymentStatistics.Count);
		IDeploymentStatistics singleDeploymentStatistics = deploymentStatistics[0];
		Assert.AreEqual(4, singleDeploymentStatistics.Instances);
		Assert.AreEqual(4, singleDeploymentStatistics.FailedJobs);

		IList<IIncidentStatistics> incidentStatistics = singleDeploymentStatistics.IncidentStatistics;
		Assert.AreEqual(1, incidentStatistics.Count);
		Assert.AreEqual(4, incidentStatistics[0].IncidentCount);
	  }

	  protected internal virtual void startProcessInstances(string key)
	  {
		TenantIdProvider = null;
		runtimeService.StartProcessInstanceByKey(key);

		TenantIdProvider = TENANT_ONE;
		runtimeService.StartProcessInstanceByKey(key);

		TenantIdProvider = TENANT_TWO;
		runtimeService.StartProcessInstanceByKey(key);
	  }

	  protected internal virtual string TenantIdProvider
	  {
		  set
		  {
			tenantIdProvider.TenantIdProvider = value;
		  }
	  }
	}

}