//using System.Collections.Generic;
//using JZTERP.Common.Shared.Entities.Bpm.Entities;
//using org.camunda.bpm.engine.impl.cfg.auth;
//using org.camunda.bpm.engine.impl.db.entitymanager;

//
//
// 
//
// 
//
// 
// 
// 
// 
// 
// 
//namespace org.camunda.bpm.engine.impl.identity.db
//{

////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;


//	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
//	using Resources = org.camunda.bpm.engine.authorization.Resources;
//	using Group = org.camunda.bpm.engine.identity.Group;
//	using Tenant = org.camunda.bpm.engine.identity.Tenant;
//	using User = org.camunda.bpm.engine.identity.User;
//	using Context = org.camunda.bpm.engine.impl.context.Context;
//	//using GroupEntity = org.camunda.bpm.engine.impl.persistence.entity.GroupEntity;
//	//using MembershipEntity = org.camunda.bpm.engine.impl.persistence.entity.MembershipEntity;
//	//using TenantEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantEntity;
//	//using TenantMembershipEntity = org.camunda.bpm.engine.impl.persistence.entity.TenantMembershipEntity;
//	//using UserEntity = org.camunda.bpm.engine.impl.persistence.entity.UserEntity;

//	/// <summary>
//	/// <para><seealso cref="WritableIdentityProvider"/> implementation backed by a
//	/// database. This implementation is used for the built-in user management.</para>
//	/// 
//	/// 
//	/// 
//	/// </summary>
//	public class DbIdentityServiceProvider :/* DbReadOnlyIdentityServiceProvider, WritableIdentityProvider
//	{

//	  // users ////////////////////////////////////////////////////////

//	  public virtual UserEntity createNewUser(string userId)
//	  {
//		//checkAuthorization(Permissions.CREATE, Resources.USER, null);
//		return new UserEntity(userId);
//	  }

//	  public virtual User saveUser(User user)
//	  {
//		UserEntity userEntity = (UserEntity) user;

//		// encrypt password
//		//userEntity.encryptPassword();

//		if (userEntity.Revision == 0)
//		{
//		  //checkAuthorization(Permissions.CREATE, Resources.USER, null);
//		  //DbEntityManager.insert(userEntity);
//		  createDefaultAuthorizations(userEntity);
//		}
//		else
//		{
//		  //checkAuthorization(Permissions.UPDATE, Resources.USER, user.Id);
//		  //DbEntityManager.merge(userEntity);
//		}

//		//return userEntity;
//	      return null;
//	  }

//	  public virtual void deleteUser(string userId)
//	  {
//		//checkAuthorization(Permissions.DELETE, Resources.USER, userId);
//		//UserEntity user = findUserById(userId);
//		//if (user != null)
//		//{
//		//  deleteMembershipsByUserId(userId);
//		//  deleteTenantMembershipsOfUser(userId);

//		//  deleteAuthorizations(Resources.USER, userId);
//		//  DbEntityManager.delete(user);
//		//}
//	  }

//	  // groups ////////////////////////////////////////////////////////

//	  public virtual GroupEntity createNewGroup(string groupId)
//	  {
//		checkAuthorization(Permissions.CREATE, Resources.GROUP, null);
//		return new GroupEntity(groupId);
//	  }

//	  public virtual GroupEntity saveGroup(Group group)
//	  {
//		GroupEntity groupEntity = (GroupEntity) group;
//		if (groupEntity.Revision == 0)
//		{
//		  checkAuthorization(Permissions.CREATE, Resources.GROUP, null);
//		  DbEntityManager.insert(groupEntity);
//		  createDefaultAuthorizations(group);
//		}
//		else
//		{
//		  checkAuthorization(Permissions.UPDATE, Resources.GROUP, group.Id);
//		  DbEntityManager.merge(groupEntity);
//		}
//		return groupEntity;
//	  }

//	  public virtual void deleteGroup(string groupId)
//	  {
//		checkAuthorization(Permissions.DELETE, Resources.GROUP, groupId);
//		GroupEntity group = findGroupById(groupId);
//		if (group != null)
//		{
//		  deleteMembershipsByGroupId(groupId);
//		  deleteTenantMembershipsOfGroup(groupId);

//		  deleteAuthorizations(Resources.GROUP, groupId);
//		  DbEntityManager.delete(group);
//		}
//	  }

//	  // tenants //////////////////////////////////////////////////////

//	  public virtual Tenant createNewTenant(string tenantId)
//	  {
//		checkAuthorization(Permissions.CREATE, Resources.TENANT, null);
//		return new TenantEntity(tenantId);
//	  }

//	  public virtual Tenant saveTenant(Tenant tenant)
//	  {
//		TenantEntity tenantEntity = (TenantEntity) tenant;
//		if (tenantEntity.Revision == 0)
//		{
//		  checkAuthorization(Permissions.CREATE, Resources.TENANT, null);
//		  DbEntityManager.insert(tenantEntity);
//		  createDefaultAuthorizations(tenant);
//		}
//		else
//		{
//		  checkAuthorization(Permissions.UPDATE, Resources.TENANT, tenant.Id);
//		  DbEntityManager.merge(tenantEntity);
//		}
//		return tenantEntity;
//	  }

//	  public virtual void deleteTenant(string tenantId)
//	  {
//		checkAuthorization(Permissions.DELETE, Resources.TENANT, tenantId);
//		TenantEntity tenant = findTenantById(tenantId);
//		if (tenant != null)
//		{
//		  deleteTenantMembershipsOfTenant(tenantId);

//		  deleteAuthorizations(Resources.TENANT, tenantId);
//		  DbEntityManager.delete(tenant);
//		}
//	  }

//	  // membership //////////////////////////////////////////////////////

//	  public virtual void createMembership(string userId, string groupId)
//	  {
//		checkAuthorization(Permissions.CREATE, Resources.GROUP_MEMBERSHIP, groupId);
//		UserEntity user = findUserById(userId);
//		GroupEntity group = findGroupById(groupId);
//		MembershipEntity membership = new MembershipEntity();
//		membership.User = user;
//		membership.Group = group;
//		DbEntityManager.insert(membership);
//		createDefaultMembershipAuthorizations(userId, groupId);
//	  }

//	  public virtual void deleteMembership(string userId, string groupId)
//	  {
//		checkAuthorization(Permissions.DELETE, Resources.GROUP_MEMBERSHIP, groupId);
//		deleteAuthorizations(Resources.GROUP_MEMBERSHIP, groupId);

//		IDictionary<string, object> parameters = new Dictionary<string, object>();
//		parameters["userId"] = userId;
//		parameters["groupId"] = groupId;
//		DbEntityManager.delete(typeof(MembershipEntity), "deleteMembership", parameters);
//	  }

//	  protected internal virtual void deleteMembershipsByUserId(string userId)
//	  {
//		DbEntityManager.delete(typeof(MembershipEntity), "deleteMembershipsByUserId", userId);
//	  }

//	  protected internal virtual void deleteMembershipsByGroupId(string groupId)
//	  {
//		DbEntityManager.delete(typeof(MembershipEntity), "deleteMembershipsByGroupId", groupId);
//	  }

//	  public virtual void createTenantUserMembership(string tenantId, string userId)
//	  {
//		checkAuthorization(Permissions.CREATE, Resources.TENANT_MEMBERSHIP, tenantId);

//		TenantEntity tenant = findTenantById(tenantId);
//		UserEntity user = findUserById(userId);

//		EnsureUtil.EnsureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
//		EnsureUtil.EnsureNotNull("No user found with id '" + userId + "'.", "user", user);

//		TenantMembershipEntity membership = new TenantMembershipEntity();
//		membership.Tenant = tenant;
//		membership.User = user;

//		DbEntityManager.insert(membership);

//		createDefaultTenantMembershipAuthorizations(tenant, user);
//	  }

//	  public virtual void createTenantGroupMembership(string tenantId, string groupId)
//	  {
//		checkAuthorization(Permissions.CREATE, Resources.TENANT_MEMBERSHIP, tenantId);

//		TenantEntity tenant = findTenantById(tenantId);
//		GroupEntity group = findGroupById(groupId);

//		EnsureUtil.EnsureNotNull("No tenant found with id '" + tenantId + "'.", "tenant", tenant);
//		EnsureUtil.EnsureNotNull("No group found with id '" + groupId + "'.", "group", group);

//		TenantMembershipEntity membership = new TenantMembershipEntity();
//		membership.Tenant = tenant;
//		membership.Group = group;

//		DbEntityManager.insert(membership);

//		createDefaultTenantMembershipAuthorizations(tenant, group);
//	  }

//	  public virtual void deleteTenantUserMembership(string tenantId, string userId)
//	  {
//		checkAuthorization(Permissions.DELETE, Resources.TENANT_MEMBERSHIP, tenantId);
//		deleteAuthorizations(Resources.TENANT_MEMBERSHIP, userId);

//		deleteAuthorizationsForUser(Resources.TENANT, tenantId, userId);

//		IDictionary<string, object> parameters = new Dictionary<string, object>();
//		parameters["tenantId"] = tenantId;
//		parameters["userId"] = userId;
//		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembership", parameters);
//	  }

//	  public virtual void deleteTenantGroupMembership(string tenantId, string groupId)
//	  {
//		checkAuthorization(Permissions.DELETE, Resources.TENANT_MEMBERSHIP, tenantId);
//		deleteAuthorizations(Resources.TENANT_MEMBERSHIP, groupId);

//		deleteAuthorizationsForGroup(Resources.TENANT, tenantId, groupId);

//		IDictionary<string, object> parameters = new Dictionary<string, object>();
//		parameters["tenantId"] = tenantId;
//		parameters["groupId"] = groupId;
//		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembership", parameters);
//	  }

//	  protected internal virtual void deleteTenantMembershipsOfUser(string userId)
//	  {
//		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfUser", userId);
//	  }

//	  protected internal virtual void deleteTenantMembershipsOfGroup(string groupId)
//	  {
//		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfGroup", groupId);
//	  }

//	  protected internal virtual void deleteTenantMembershipsOfTenant(string tenant)
//	  {
//		DbEntityManager.delete(typeof(TenantMembershipEntity), "deleteTenantMembershipsOfTenant", tenant);
//	  }

//	  // authorizations ////////////////////////////////////////////////////////////

//	  protected internal virtual void createDefaultAuthorizations(UserEntity userEntity)
//	  {
//		if (Context.ProcessEngineConfiguration.AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newUser(userEntity));
//		}
//	  }

//	  protected internal virtual void createDefaultAuthorizations(Group group)
//	  {
//		if (AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newGroup(group));
//		}
//	  }

//	  protected internal virtual void createDefaultAuthorizations(Tenant tenant)
//	  {
//		if (AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.newTenant(tenant));
//		}
//	  }

//	  protected internal virtual void createDefaultMembershipAuthorizations(string userId, string groupId)
//	  {
//		if (AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.groupMembershipCreated(groupId, userId));
//		}
//	  }

//	  protected internal virtual void createDefaultTenantMembershipAuthorizations(Tenant tenant, User user)
//	  {
//		if (AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.tenantMembershipCreated(tenant, user));
//		}
//	  }

//	  protected internal virtual void createDefaultTenantMembershipAuthorizations(Tenant tenant, Group group)
//	  {
//		if (AuthorizationEnabled)
//		{
//		  saveDefaultAuthorizations(ResourceAuthorizationProvider.tenantMembershipCreated(tenant, group));
//		}
//	  }

//	}

//}

