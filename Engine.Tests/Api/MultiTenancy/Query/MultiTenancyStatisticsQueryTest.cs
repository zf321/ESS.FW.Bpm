using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyStatisticsQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {

		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("EmptyProcess").StartEvent().Done();

		IBpmnModelInstance singleTaskProcess = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("SingleTaskProcess").StartEvent().UserTask().Done();

		Deployment(process);
		DeploymentForTenant(TENANT_ONE, singleTaskProcess);
		DeploymentForTenant(TENANT_TWO, process);
	  }

	   [Test]   public virtual void testDeploymentStatistics()
	   {
	       IList<IDeploymentStatistics> deploymentStatistics = managementService.CreateDeploymentStatisticsQuery()
	           .ToList();

		Assert.That(deploymentStatistics.Count, Is.EqualTo(3));

		ISet<string> tenantIds = collectDeploymentTenantIds(deploymentStatistics);
            Assert.Contains(tenantIds,new List<string>() { null,TENANT_TWO, TENANT_ONE });
		//Assert.That(tenantIds,NU.Has.() hasItems(null, TENANT_ONE, TENANT_TWO));
	  }

	   [Test]   public virtual void testProcessDefinitionStatistics()
	  {
		IList<IProcessDefinitionStatistics> processDefinitionStatistics = managementService.CreateProcessDefinitionStatisticsQuery().ToList();

		Assert.That(processDefinitionStatistics.Count, Is.EqualTo(3));

		ISet<string> tenantIds = collectDefinitionTenantIds(processDefinitionStatistics);
            Assert.Contains(tenantIds, new List<string>() { null, TENANT_TWO, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryNoAuthenticatedTenantsForDeploymentStatistics()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(1));
		Assert.That(tenantIds.GetEnumerator().MoveNext(), Is.EqualTo(null));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenantForDeploymentStatistics()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(2));
            Assert.Contains(tenantIds, new List<string>() { null, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryAuthenticatedTenantsForDeploymentStatistics()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(3));
        Assert.Contains(tenantIds, new List<string>() { null, TENANT_TWO, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryDisabledTenantCheckForDeploymentStatistics()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IDeploymentStatistics> query = managementService.CreateDeploymentStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));

		ISet<string> tenantIds = collectDeploymentTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(3));
        Assert.Contains(tenantIds, new List<string>() { null, TENANT_TWO, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryNoAuthenticatedTenantsForProcessDefinitionStatistics()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(1));
		Assert.That(tenantIds.GetEnumerator().MoveNext(), Is.EqualTo(null));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenantForProcessDefinitionStatistics()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(2));
            Assert.Contains(tenantIds, new List<string>() { null, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryAuthenticatedTenantsForProcessDefinitionStatistics()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(3));
            Assert.Contains(tenantIds, new List<string>() { null, TENANT_TWO, TENANT_ONE });
        }

	   [Test]   public virtual void testQueryDisabledTenantCheckForProcessDefinitionStatistics()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IProcessDefinitionStatistics> query = managementService.CreateProcessDefinitionStatisticsQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));

		ISet<string> tenantIds = collectDefinitionTenantIds(query.ToList());
		Assert.That(tenantIds.Count, Is.EqualTo(3));
            Assert.Contains(tenantIds, new List<string>() { null, TENANT_TWO, TENANT_ONE });
        }

	   [Test]   public virtual void testActivityStatistics()
	  {
		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("SingleTaskProcess");

		IQueryable<IActivityStatistics> query = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		Assert.That(query.Count(), Is.EqualTo(1L));

	  }

	   [Test]   public virtual void testQueryAuthenticatedTenantForActivityStatistics()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("SingleTaskProcess");

		IQueryable<IActivityStatistics> query = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		Assert.That(query.Count(), Is.EqualTo(1L));

	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenantForActivityStatistics()
	  {

		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("SingleTaskProcess");

		identityService.SetAuthentication("user", null);

		IQueryable<IActivityStatistics> query = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		Assert.That(query.Count(), Is.EqualTo(0L));

	  }

	   [Test]   public virtual void testQueryDisabledTenantCheckForActivityStatistics()
	  {

		IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("SingleTaskProcess");

		identityService.SetAuthentication("user", null);
		processEngineConfiguration.SetTenantCheckEnabled(false);

		IQueryable<IActivityStatistics> query = managementService.CreateActivityStatisticsQuery(processInstance.ProcessDefinitionId);

		Assert.That(query.Count(), Is.EqualTo(1L));

	  }

	  protected internal virtual ISet<string> collectDeploymentTenantIds(IList<IDeploymentStatistics> deploymentStatistics)
	  {
		ISet<string> tenantIds = new HashSet<string>();

		foreach (IDeploymentStatistics statistics in deploymentStatistics)
		{
		  tenantIds.Add(statistics.TenantId);
		}
		return tenantIds;
	  }

	  protected internal virtual ISet<string> collectDefinitionTenantIds(IList<IProcessDefinitionStatistics> processDefinitionStatistics)
	  {
		ISet<string> tenantIds = new HashSet<string>();

		foreach (IProcessDefinitionStatistics statistics in processDefinitionStatistics)
		{
		  tenantIds.Add(statistics.TenantId);
		}
		return tenantIds;
	  }

	}

}