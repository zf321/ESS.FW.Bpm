using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.MultiTenancy.TenantCheck
{

    
	/// <summary>
	/// 
	/// </summary>
	public class MultiTenancyDeploymentCmdsTenantCheckTest
	{
		private bool InstanceFieldsInitialized = false;

		public MultiTenancyDeploymentCmdsTenantCheckTest()
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


	  protected internal const string TENANT_TWO = "tenant2";
	  protected internal const string TENANT_ONE = "tenant1";

	  protected internal static readonly IBpmnModelInstance emptyProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().Done();
	  protected internal static readonly IBpmnModelInstance startEndProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess().StartEvent().EndEvent().Done();

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal ProcessEngineTestRule testRule;



	  protected internal IRepositoryService repositoryService;
	  protected internal IIdentityService identityService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

        [SetUp]
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
	  }

        [Test]
        public virtual void createDeploymentForAnotherTenant()
	  {
		identityService.SetAuthentication("user", null, null);

		repositoryService.CreateDeployment().AddModelInstance("emptyProcess.bpmn", emptyProcess).TenantId(TENANT_ONE).Deploy();

		identityService.ClearAuthentication();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void createDeploymentWithAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null, new List<string>(){ TENANT_ONE });

		repositoryService.CreateDeployment().AddModelInstance("emptyProcess.bpmn", emptyProcess).TenantId(TENANT_ONE).Deploy();

		identityService.ClearAuthentication();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void createDeploymentDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		repositoryService.CreateDeployment().AddModelInstance("emptyProcessOne", emptyProcess).TenantId(TENANT_ONE).Deploy();
		repositoryService.CreateDeployment().AddModelInstance("emptyProcessTwo", startEndProcess).TenantId(TENANT_TWO).Deploy();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

        [Test]
        public virtual void failToDeleteDeploymentNoAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot Delete the deployment");

		repositoryService.DeleteDeployment(deployment.Id);
	  }

        [Test]
        public virtual void deleteDeploymentWithAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null, new List<string>() { TENANT_ONE });

		repositoryService.DeleteDeployment(deployment.Id);

		identityService.ClearAuthentication();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void deleteDeploymentDisabledTenantCheck()
	  {
		IDeployment deploymentOne = testRule.DeployForTenant(TENANT_ONE, emptyProcess);
		IDeployment deploymentTwo = testRule.DeployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		repositoryService.DeleteDeployment(deploymentOne.Id);
		repositoryService.DeleteDeployment(deploymentTwo.Id);

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
	  }

        [Test]
        public virtual void failToGetDeploymentResourceNamesNoAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the deployment");

		repositoryService.GetDeploymentResourceNames(deployment.Id);
	  }

        [Test]
        public virtual void getDeploymentResourceNamesWithAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null, new List<string>() { TENANT_ONE });

		IList<string> deploymentResourceNames = repositoryService.GetDeploymentResourceNames(deployment.Id);
		Assert.That(deploymentResourceNames.Count>=1);
	  }

        [Test]
        public virtual void getDeploymentResourceNamesDisabledTenantCheck()
	  {
		IDeployment deploymentOne = testRule.DeployForTenant(TENANT_ONE, emptyProcess);
		IDeployment deploymentTwo = testRule.DeployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IList<string> deploymentResourceNames = repositoryService.GetDeploymentResourceNames(deploymentOne.Id);
		Assert.That(deploymentResourceNames.Count>=1);

		deploymentResourceNames = repositoryService.GetDeploymentResourceNames(deploymentTwo.Id);
		Assert.That(deploymentResourceNames.Count >= 1);
	  }

        [Test]
        public virtual void failToGetDeploymentResourcesNoAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the deployment");

		repositoryService.GetDeploymentResources(deployment.Id);
	  }

        [Test]
        public virtual void getDeploymentResourcesWithAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		identityService.SetAuthentication("user", null,new List<string>() { TENANT_ONE });

		IList<IResource> deploymentResources = repositoryService.GetDeploymentResources(deployment.Id);
		Assert.That(deploymentResources.Count>=1);
	  }

        [Test]
        public virtual void getDeploymentResourcesDisabledTenantCheck()
	  {
		IDeployment deploymentOne = testRule.DeployForTenant(TENANT_ONE, emptyProcess);
		IDeployment deploymentTwo = testRule.DeployForTenant(TENANT_TWO, startEndProcess);

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IList<IResource> deploymentResources = repositoryService.GetDeploymentResources(deploymentOne.Id);
		Assert.That(deploymentResources.Count>=1);

		deploymentResources = repositoryService.GetDeploymentResources(deploymentTwo.Id);
		Assert.That(deploymentResources.Count>=1);
	  }

        [Test]
        public virtual void failToGetResourceAsStreamNoAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		IResource resource = repositoryService.GetDeploymentResources(deployment.Id).First();

		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the deployment");

		repositoryService.GetResourceAsStream(deployment.Id, resource.Name);
	  }

        [Test]
        public virtual void getResourceAsStreamWithAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		IResource resource = repositoryService.GetDeploymentResources(deployment.Id)[0];

		identityService.SetAuthentication("user", null,new List<string>() { TENANT_ONE });

		System.IO.Stream inputStream = repositoryService.GetResourceAsStream(deployment.Id, resource.Name);
		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getResourceAsStreamDisabledTenantCheck()
	  {
		IDeployment deploymentOne = testRule.DeployForTenant(TENANT_ONE, emptyProcess);
		IDeployment deploymentTwo = testRule.DeployForTenant(TENANT_TWO, startEndProcess);

		IResource resourceOne = repositoryService.GetDeploymentResources(deploymentOne.Id)[0];
		IResource resourceTwo = repositoryService.GetDeploymentResources(deploymentTwo.Id)[0];

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetResourceAsStream(deploymentOne.Id, resourceOne.Name);
		Assert.That(inputStream!=null);

		inputStream = repositoryService.GetResourceAsStream(deploymentTwo.Id, resourceTwo.Name);
		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void failToGetResourceAsStreamByIdNoAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		IResource resource = repositoryService.GetDeploymentResources(deployment.Id)[0];

		identityService.SetAuthentication("user", null, null);

		// declare expected exception
		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the deployment");

		repositoryService.GetResourceAsStreamById(deployment.Id, resource.Id);
	  }

        [Test]
        public virtual void getResourceAsStreamByIdWithAuthenticatedTenant()
	  {
		IDeployment deployment = testRule.DeployForTenant(TENANT_ONE, emptyProcess);

		IResource resource = repositoryService.GetDeploymentResources(deployment.Id)[0];

		identityService.SetAuthentication("user", null,new List<string>() { TENANT_ONE });

		System.IO.Stream inputStream = repositoryService.GetResourceAsStreamById(deployment.Id, resource.Id);
		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void getResourceAsStreamByIdDisabledTenantCheck()
	  {
		IDeployment deploymentOne = testRule.DeployForTenant(TENANT_ONE, emptyProcess);
		IDeployment deploymentTwo = testRule.DeployForTenant(TENANT_TWO, startEndProcess);

		IResource resourceOne = repositoryService.GetDeploymentResources(deploymentOne.Id)[0];
		IResource resourceTwo = repositoryService.GetDeploymentResources(deploymentTwo.Id)[0];

		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		System.IO.Stream inputStream = repositoryService.GetResourceAsStreamById(deploymentOne.Id, resourceOne.Id);
		Assert.That(inputStream!=null);

		inputStream = repositoryService.GetResourceAsStreamById(deploymentTwo.Id, resourceTwo.Id);
		Assert.That(inputStream!=null);
	  }

        [Test]
        public virtual void redeployForDifferentAuthenticatedTenants()
	  {
		IDeployment deploymentOne = repositoryService.CreateDeployment().AddModelInstance("emptyProcess.bpmn", emptyProcess).AddModelInstance("startEndProcess.bpmn", startEndProcess).TenantId(TENANT_ONE).Deploy();

		identityService.SetAuthentication("user", null, new List<string>() { TENANT_TWO });

		//thrown.Expect(typeof(ProcessEngineException));
		//thrown.ExpectMessage("Cannot get the deployment");

		repositoryService.CreateDeployment().AddDeploymentResources(deploymentOne.Id).TenantId(TENANT_TWO).Deploy();
	  }

        [Test]
        public virtual void redeployForTheSameAuthenticatedTenant()
	  {
		IDeployment deploymentOne = repositoryService.CreateDeployment().AddModelInstance("emptyProcess.bpmn", emptyProcess).AddModelInstance("startEndProcess.bpmn", startEndProcess).TenantId(TENANT_ONE).Deploy();

		identityService.SetAuthentication("user", null,new List<string>() { TENANT_ONE });

		repositoryService.CreateDeployment().AddDeploymentResources(deploymentOne.Id).TenantId(TENANT_ONE).Deploy();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(2L));
	  }

        [Test]
        public virtual void redeployForDifferentAuthenticatedTenantsDisabledTenantCheck()
	  {
		IDeployment deploymentOne = repositoryService.CreateDeployment().AddModelInstance("emptyProcess.bpmn", emptyProcess).AddModelInstance("startEndProcess.bpmn", startEndProcess).TenantId(TENANT_ONE).Deploy();

		identityService.SetAuthentication("user", null, null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		repositoryService.CreateDeployment().AddDeploymentResources(deploymentOne.Id).TenantId(TENANT_TWO).Deploy();

		 IQueryable<IDeployment> query = repositoryService.CreateDeploymentQuery();
		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

    [TearDown]
	  public virtual void tearDown()
	  {
		identityService.ClearAuthentication();
		foreach (IDeployment deployment in repositoryService.CreateDeploymentQuery().ToList())
		{
		  repositoryService.DeleteDeployment(deployment.Id, true);
		}
	  }
	}

}