using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;


namespace Engine.Tests.Api.MultiTenancy.Query
{
    

	public class MultiTenancyEventSubscriptionQueryTest : PluggableProcessEngineTestCase
	{

	  protected internal const string TENANT_ONE = "tenant1";
	  protected internal const string TENANT_TWO = "tenant2";
        [SetUp]
	  protected internal void setUp()
	  {
		IBpmnModelInstance process = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess").StartEvent().Message("start").UserTask().EndEvent().Done();

		Deployment(process);
		DeploymentForTenant(TENANT_ONE, process);
		DeploymentForTenant(TENANT_TWO, process);

		// the deployed process definition contains a message start event
		// - so a message event subscription is created on deployment.
	  }

	   [Test]   public virtual void testQueryNoTenantIdSet()
	  {
		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByTenantId()
	  {
		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == TENANT_ONE);

		Assert.That(query.Count(), Is.EqualTo(1L));

		query = runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == TENANT_TWO);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIds()
	  {
		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));

		Assert.That(query.Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryBySubscriptionsWithoutTenantId()
	  {
		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == null);

		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryByTenantIdsIncludeSubscriptionsWithoutTenantId()
	   {
	       IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery(c => c.TenantId == TENANT_ONE);//.IncludeEventSubscriptionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == TENANT_TWO);//.IncludeEventSubscriptionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(2L));

		query = runtimeService.CreateEventSubscriptionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId));//.IncludeEventSubscriptionsWithoutTenantId();

		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	   [Test]   public virtual void testQueryByNonExistingTenantId()
	  {
		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == "nonExisting");

		Assert.That(query.Count(), Is.EqualTo(0L));
	  }

	   [Test]   public virtual void testFailQueryByTenantIdNull()
	  {
		try
		{
		  runtimeService.CreateEventSubscriptionQuery(c=>c.TenantId == (string) null);

		  Assert.Fail("expected exception");
		}
		catch (NullValueException)
		{
		}
	  }

	   [Test]   public virtual void testQuerySortingAsc()
	  {
		// exclude subscriptions without tenant id because of database-specific ordering
	      IList<IEventSubscription> eventSubscriptions = runtimeService.CreateEventSubscriptionQuery(c=>new string[]{ TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))
	          /*.OrderByTenantId()*/
	          /*.Asc()*/
	          .ToList();

		Assert.That(eventSubscriptions.Count, Is.EqualTo(2));
		Assert.That(eventSubscriptions[0].TenantId, Is.EqualTo(TENANT_ONE));
		Assert.That(eventSubscriptions[1].TenantId, Is.EqualTo(TENANT_TWO));
	  }

	   [Test]   public virtual void testQuerySortingDesc()
	  {
		// exclude subscriptions without tenant id because of database-specific ordering
		IList<IEventSubscription> eventSubscriptions = runtimeService.CreateEventSubscriptionQuery(c=>new []{TENANT_ONE, TENANT_TWO }.Contains(c.TenantId))/*.OrderByTenantId()*//*.Desc()*/.ToList();

		Assert.That(eventSubscriptions.Count, Is.EqualTo(2));
		Assert.That(eventSubscriptions[0].TenantId, Is.EqualTo(TENANT_TWO));
		Assert.That(eventSubscriptions[1].TenantId, Is.EqualTo(TENANT_ONE));
	  }

	   [Test]   public virtual void testQueryNoAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null, null);

		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();
		Assert.That(query.Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenant()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE});

		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

		Assert.That(query.Count(), Is.EqualTo(2L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(0L));
		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId))//.IncludeEventSubscriptionsWithoutTenantId()
            .Count(), Is.EqualTo(2L));
	  }

	   [Test]   public virtual void testQueryAuthenticatedTenants()
	  {
		identityService.SetAuthentication("user", null,new List<string>(){TENANT_ONE, TENANT_TWO});

		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();

		Assert.That(query.Count(), Is.EqualTo(3L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_ONE).Count(), Is.EqualTo(1L));
		Assert.That(query.Where(c=>c.TenantId == TENANT_TWO).Count(), Is.EqualTo(1L));
	  }

	   [Test]   public virtual void testQueryDisabledTenantCheck()
	  {
		processEngineConfiguration.SetTenantCheckEnabled(false);
		identityService.SetAuthentication("user", null, null);

		IQueryable<IEventSubscription> query = runtimeService.CreateEventSubscriptionQuery();
		Assert.That(query.Count(), Is.EqualTo(3L));
	  }

	}

}