//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Identity;
//using ESS.FW.Bpm.Engine.Tests.Util;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Identity
//{
    
//    public class TenantQueryTest
//	{

//	  protected internal const string TENANT_ONE = "tenant1";
//	  protected internal const string TENANT_TWO = "tenant2";

//	  protected internal const string IUser = "user";
//	  protected internal const string IGroup = "group";


//	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//	  protected internal IIdentityService identityService;

//        [SetUp]
//	  public virtual void setUp()
//	  {
//		identityService = engineRule.IdentityService;

//		createTenant(TENANT_ONE, "Tenant_1");
//		createTenant(TENANT_TWO, "Tenant_2");

//		IUser user = identityService.NewUser(IUser);
//		identityService.SaveUser(user);

//		IGroup group = identityService.NewGroup(IGroup);
//		identityService.SaveGroup(group);

//		identityService.CreateMembership(IUser, IGroup);

//		identityService.CreateTenantUserMembership(TENANT_ONE, IUser);
//		identityService.CreateTenantGroupMembership(TENANT_TWO, IGroup);
//	  }

//    [Test]
//	  public virtual void queryById()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery().TenantId(TENANT_ONE);

//		Assert.That(query.Count(), Is.EqualTo(1L));
//		Assert.That(query.Count(), Is.EqualTo(1));

//		ITenant tenant = query.First();
//		Assert.That(tenant!=null);
//		Assert.That(tenant.Name, Is.EqualTo("Tenant_1"));
//	  }


//        [Test]
//        public virtual void queryByNonExistingId()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery().TenantId("nonExisting");

//		Assert.That(query.Count(), Is.EqualTo(0L));
//	  }


//        [Test]
//        public virtual void queryByIdIn()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery();

//		Assert.That(query.TenantIdIn("non", "existing").Count(), Is.EqualTo(0L));
//		Assert.That(query.Where(c=>new string[]{TENANT_ONE, TENANT_TWO}.Contains(c.TenantId)).Count(), Is.EqualTo(2L));
//	  }


//        [Test]
//        public virtual void queryByName()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery();

//		Assert.That(query.TenantName("nonExisting").Count(), Is.EqualTo(0L));
//		Assert.That(query.TenantName("Tenant_1").Count(), Is.EqualTo(1L));
//		Assert.That(query.TenantName("Tenant_2").Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByNameLike()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery();

//		Assert.That(query.TenantNameLike("%nonExisting%").Count(), Is.EqualTo(0L));
//		Assert.That(query.TenantNameLike("%Tenant\\_1%").Count(), Is.EqualTo(1L));
//		Assert.That(query.TenantNameLike("%Tenant%").Count(), Is.EqualTo(2L));
//	  }


//        [Test]
//        public virtual void queryByUser()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery();

//		Assert.That(query.UserMember("nonExisting").Count(), Is.EqualTo(0L));
//		Assert.That(query.UserMember(IUser).Count(), Is.EqualTo(1L));
//		Assert.That(query.UserMember(IUser).TenantId(TENANT_ONE).Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByGroup()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery();

//		Assert.That(query.GroupMember("nonExisting").Count(), Is.EqualTo(0L));
//		Assert.That(query.GroupMember(IGroup).Count(), Is.EqualTo(1L));
//		Assert.That(query.GroupMember(IGroup).TenantId(TENANT_TWO).Count(), Is.EqualTo(1L));
//	  }


//        [Test]
//        public virtual void queryByUserIncludingGroups()
//	  {
//		IQueryable<ITenant> query = identityService.CreateTenantQuery().UserMember(IUser);

//		Assert.That(query.IncludingGroupsOfUser(false).Count(), Is.EqualTo(1L));
//		Assert.That(query.IncludingGroupsOfUser(true).Count(), Is.EqualTo(2L));
//	  }


//        [Test]
//        public virtual void queryOrderById()
//	  {
//		// ascending
//		IList<ITenant> tenants = identityService.CreateTenantQuery()/*.OrderByTenantId()*//*.Asc()*/.ToList();
//		Assert.That(tenants.Count, Is.EqualTo(2));

//		Assert.That(tenants[0].Id, Is.EqualTo(TENANT_ONE));
//		Assert.That(tenants[1].Id, Is.EqualTo(TENANT_TWO));

//		// descending
//		tenants = identityService.CreateTenantQuery()/*.OrderByTenantId()*//*.Desc()*/.ToList();

//		Assert.That(tenants[0].Id, Is.EqualTo(TENANT_TWO));
//		Assert.That(tenants[1].Id, Is.EqualTo(TENANT_ONE));
//	  }


//        [Test]
//        public virtual void queryOrderByName()
//	  {
//		// ascending
//		IList<ITenant> tenants = identityService.CreateTenantQuery().OrderByTenantName()/*.Asc()*/.ToList();
//		Assert.That(tenants.Count, Is.EqualTo(2));

//		Assert.That(tenants[0].Name, Is.EqualTo("Tenant_1"));
//		Assert.That(tenants[1].Name, Is.EqualTo("Tenant_2"));

//		// descending
//		tenants = identityService.CreateTenantQuery().OrderByTenantName()/*.Desc()*/.ToList();

//		Assert.That(tenants[0].Name, Is.EqualTo("Tenant_2"));
//		Assert.That(tenants[1].Name, Is.EqualTo("Tenant_1"));
//	  }

//	  protected internal virtual ITenant createTenant(string id, string name)
//	  {
//		ITenant tenant = engineRule.IdentityService.NewTenant(id);
//		tenant.Name = name;
//		identityService.SaveTenant(tenant);

//		return tenant;
//	  }


//        [TearDown]
//        public virtual void tearDown()
//	  {
//		identityService.DeleteTenant(TENANT_ONE);
//		identityService.DeleteTenant(TENANT_TWO);

//		identityService.DeleteUser(IUser);
//		identityService.DeleteGroup(IGroup);
//	  }

//	}

//}