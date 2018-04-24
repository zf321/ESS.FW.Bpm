//using System.Collections.Generic;
//using System.Linq;
//using JZTERP.Common.Shared.Entities.Bpm.Entities;
//using org.camunda.bpm.engine.filter;
//using org.camunda.bpm.engine.impl.Util;

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
//namespace org.camunda.bpm.engine.impl.cfg.auth
//{

////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Permissions.READ;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.DEPLOYMENT;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.FILTER;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.GROUP;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.ITask;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.TENANT;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.authorization.Resources.USER;
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.EnsureUtil.ensureValidIndividualResourceId;


//	using Permission = org.camunda.bpm.engine.authorization.Permission;
//	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
//	using Resource = org.camunda.bpm.engine.authorization.Resource;
//	using Group = org.camunda.bpm.engine.identity.Group;
//	using Tenant = org.camunda.bpm.engine.identity.Tenant;
//	using User = org.camunda.bpm.engine.identity.User;
//	using Context = org.camunda.bpm.engine.impl.context.Context;
//	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
//	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
//	
//	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
//	using DecisionRequirementsDefinition = org.camunda.bpm.engine.repository.DecisionRequirementsDefinition;
//	using Deployment = org.camunda.bpm.engine.repository.Deployment;
//	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
//	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
//	using ITask = org.camunda.bpm.engine.ITask.ITask;

//	/// <summary>
//	/// <para>Provides the default authorizations for camunda BPM.</para>
//	/// 
//	/// 
//	/// 
//	/// </summary>
//	public class DefaultAuthorizationProvider : ResourceAuthorizationProvider
//	{

//	  public virtual AuthorizationEntity[] newUser(User user)
//	  {
//		// create an authorization which gives the user all permissions on himself:
//		string userId = user.Id;

//		EnsureUtil.ensureValidIndividualResourceId("Cannot create default authorization for user " + userId, userId);
//		AuthorizationEntity resourceOwnerAuthorization = createGrantAuthorization(userId, null, USER, userId, ALL);

//		return new AuthorizationEntity[]{resourceOwnerAuthorization};
//	  }

//	  public virtual AuthorizationEntity[] newGroup(Group group)
//	  {
//		IList<AuthorizationEntity> authorizations = new List<AuthorizationEntity>();

//		// whenever a new group is created, all users part of the
//		// group are granted READ permissions on the group
//		string groupId = group.Id;

//		EnsureUtil.ensureValidIndividualResourceId("Cannot create default authorization for group " + groupId, groupId);

//		AuthorizationEntity groupMemberAuthorization = createGrantAuthorization(null, groupId, GROUP, groupId, READ);
//		authorizations.Add(groupMemberAuthorization);

//		return authorizations.ToArray();
//	  }

//	  public virtual AuthorizationEntity[] newTenant(Tenant tenant)
//	  {
//		// no default authorizations on tenants.
//		return null;
//	  }

//	  public virtual AuthorizationEntity[] groupMembershipCreated(string groupId, string userId)
//	  {

//		// no default authorizations on memberships.

//		return null;
//	  }

//	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, User user)
//	  {

//		AuthorizationEntity userAuthorization = createGrantAuthorization(user.Id, null, TENANT, tenant.Id, READ);

//		return new AuthorizationEntity[]{userAuthorization};
//	  }

//	  public virtual AuthorizationEntity[] tenantMembershipCreated(Tenant tenant, Group group)
//	  {
//		AuthorizationEntity userAuthorization = createGrantAuthorization(null, group.Id, TENANT, tenant.Id, READ);

//		return new AuthorizationEntity[]{userAuthorization};
//	  }

//	  public virtual AuthorizationEntity[] newFilter(IFilter filter)
//	  {

//		string owner = filter.Owner;
//		if (!string.ReferenceEquals(owner, null))
//		{
//		  // create an authorization which gives the owner of the filter all permissions on the filter
//		  string filterId = filter.Id;

//		  EnsureUtil.ensureValidIndividualResourceId("Cannot create default authorization for filter owner " + owner, owner);

//		  AuthorizationEntity filterOwnerAuthorization = createGrantAuthorization(owner, null, FILTER, filterId, ALL);

//		  return new AuthorizationEntity[]{filterOwnerAuthorization};

//		}
//		else
//		{
//		  return null;

//		}
//	  }

//	  // Deployment ///////////////////////////////////////////////

//	  public virtual AuthorizationEntity[] newDeployment(Deployment deployment)
//	  {
//		ProcessEngineConfigurationImpl processEngineConfiguration = Context.ProcessEngineConfiguration;
//		IdentityService identityService = processEngineConfiguration.IdentityService;
//		Authentication currentAuthentication = identityService.CurrentAuthentication;

//		if (currentAuthentication != null && !string.ReferenceEquals(currentAuthentication.UserId, null))
//		{
//		  string userId = currentAuthentication.UserId;
//		  string deploymentId = deployment.Id;
//		  AuthorizationEntity authorization = createGrantAuthorization(userId, null, DEPLOYMENT, deploymentId, READ, DELETE);
//		  return new AuthorizationEntity[]{authorization};
//		}

//		return null;
//	  }

//	  // Process Definition //////////////////////////////////////

//	  public virtual AuthorizationEntity[] newProcessDefinition(ProcessDefinition processDefinition)
//	  {
//		// no default authorizations on process definitions.
//		return null;
//	  }

//	  // Process Instance ///////////////////////////////////////

//	  public virtual AuthorizationEntity[] newProcessInstance(ProcessInstance processInstance)
//	  {
//		// no default authorizations on process instances.
//		return null;
//	  }

//	  // ITask /////////////////////////////////////////////////

//	  public virtual AuthorizationEntity[] newTask(ITask ITask)
//	  {
//		// no default authorizations on tasks.
//		return null;
//	  }

//	  public virtual AuthorizationEntity[] newTaskAssignee(ITask ITask, string oldAssignee, string newAssignee)
//	  {
//		if (!string.ReferenceEquals(newAssignee, null))
//		{

//		  EnsureUtil.ensureValidIndividualResourceId("Cannot create default authorization for assignee " + newAssignee, newAssignee);

//		  // create (or update) an authorization for the new assignee.

//		  string taskId = ITask.Id;

//		  // fetch existing authorization
//		  AuthorizationEntity authorization = getGrantAuthorizationByUserId(newAssignee, ITask, taskId);

//		  // update authorization:
//		  // (1) fetched authorization == null -> create a new authorization (with READ and (UPDATE/TASK_WORK) permission)
//		  // (2) fetched authorization != null -> add READ and (UPDATE/TASK_WORK) permission
//		  // Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
//		  authorization = updateAuthorization(authorization, newAssignee, null, ITask, taskId, READ, DefaultUserPermissionForTask);

//		  // return always created or updated authorization
//		  return new AuthorizationEntity[]{authorization};
//		}

//		return null;
//	  }

//	  public virtual AuthorizationEntity[] newTaskOwner(ITask ITask, string oldOwner, string newOwner)
//	  {
//		if (!string.ReferenceEquals(newOwner, null))
//		{

//		  EnsureUtil.ensureValidIndividualResourceId("Cannot create default authorization for owner " + newOwner, newOwner);

//		  // create (or update) an authorization for the new owner.
//		  string taskId = ITask.Id;

//		  // fetch existing authorization
//		  AuthorizationEntity authorization = getGrantAuthorizationByUserId(newOwner, ITask, taskId);

//		  // update authorization:
//		  // (1) fetched authorization == null -> create a new authorization (with READ and (UPDATE/TASK_WORK) permission)
//		  // (2) fetched authorization != null -> add READ and (UPDATE/TASK_WORK) permission
//		  // Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
//		  authorization = updateAuthorization(authorization, newOwner, null, ITask, taskId, READ, DefaultUserPermissionForTask);

//		  // return always created or updated authorization
//		  return new AuthorizationEntity[]{authorization};
//		}

//		return null;
//	  }

//	  public virtual AuthorizationEntity[] newTaskUserIdentityLink(ITask ITask, string userId, string type)
//	  {
//            // create (or update) an authorization for the given user
//            // whenever a new user identity link will be added

//            EnsureUtil.ensureValidIndividualResourceId("Cannot grant default authorization for identity link to user " + userId, userId);

//		string taskId = ITask.Id;

//		// fetch existing authorization
//		AuthorizationEntity authorization = getGrantAuthorizationByUserId(userId, ITask, taskId);

//		// update authorization:
//		// (1) fetched authorization == null -> create a new authorization (with READ and (UPDATE/TASK_WORK) permission)
//		// (2) fetched authorization != null -> add READ and (UPDATE or TASK_WORK) permission
//		// Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
//		authorization = updateAuthorization(authorization, userId, null, ITask, taskId, READ, DefaultUserPermissionForTask);

//		// return always created or updated authorization
//		return new AuthorizationEntity[]{authorization};
//	  }

//	  public virtual AuthorizationEntity[] newTaskGroupIdentityLink(ITask ITask, string groupId, string type)
//	  {

//		EnsureUtil.ensureValidIndividualResourceId("Cannot grant default authorization for identity link to group " + groupId, groupId);

//		// create (or update) an authorization for the given group
//		// whenever a new user identity link will be added
//		string taskId = ITask.Id;

//		// fetch existing authorization
//		AuthorizationEntity authorization = getGrantAuthorizationByGroupId(groupId, ITask, taskId);

//		// update authorization:
//		// (1) fetched authorization == null -> create a new authorization (with READ and (UPDATE/TASK_WORK) permission)
//		// (2) fetched authorization != null -> add READ and UPDATE permission
//		// Update or TASK_WORK permission is configurable in camunda.cfg.xml and by default, UPDATE permission is provided
//		authorization = updateAuthorization(authorization, null, groupId, ITask, taskId, READ, DefaultUserPermissionForTask);

//		// return always created or updated authorization
//		return new AuthorizationEntity[]{authorization};
//	  }

//	  public virtual AuthorizationEntity[] deleteTaskUserIdentityLink(ITask ITask, string userId, string type)
//	  {
//		// an existing authorization will not be deleted in such a case
//		return null;
//	  }

//	  public virtual AuthorizationEntity[] deleteTaskGroupIdentityLink(ITask ITask, string groupId, string type)
//	  {
//		// an existing authorization will not be deleted in such a case
//		return null;
//	  }

//	  public virtual AuthorizationEntity[] newDecisionDefinition(DecisionDefinition decisionDefinition)
//	  {
//		// no default authorizations on decision definitions.
//		return null;
//	  }

//	  public virtual AuthorizationEntity[] newDecisionRequirementsDefinition(DecisionRequirementsDefinition decisionRequirementsDefinition)
//	  {
//		// no default authorizations on decision requirements definitions.
//		return null;
//	  }

//	  // helper //////////////////////////////////////////////////////////////

//	  protected internal virtual AuthorizationManager AuthorizationManager
//	  {
//		  get
//		  {
//			CommandContext commandContext = Context.CommandContext;
//			return commandContext.AuthorizationManager;
//		  }
//	  }

//	  protected internal virtual AuthorizationEntity getGrantAuthorizationByUserId(string userId, Resource resource, string resourceId)
//	  {
//		AuthorizationManager authorizationManager = AuthorizationManager;
//		return authorizationManager.findAuthorizationByUserIdAndResourceId(AUTH_TYPE_GRANT, userId, resource, resourceId);
//	  }

//	  protected internal virtual AuthorizationEntity getGrantAuthorizationByGroupId(string groupId, Resource resource, string resourceId)
//	  {
//		AuthorizationManager authorizationManager = AuthorizationManager;
//		return authorizationManager.findAuthorizationByGroupIdAndResourceId(AUTH_TYPE_GRANT, groupId, resource, resourceId);
//	  }

//	  protected internal virtual AuthorizationEntity updateAuthorization(AuthorizationEntity authorization, string userId, string groupId, Resource resource, string resourceId, params Permission[] permissions)
//	  {
//		if (authorization == null)
//		{
//		  authorization = createGrantAuthorization(userId, groupId, resource, resourceId);
//		  updateAuthorizationBasedOnCacheEntries(authorization, userId, groupId, resource, resourceId);
//		}

//		if (permissions != null)
//		{
//		  foreach (Permission permission in permissions)
//		  {
//			authorization.addPermission(permission);
//		  }
//		}

//		return authorization;
//	  }

//	  protected internal virtual AuthorizationEntity createGrantAuthorization(string userId, string groupId, Resource resource, string resourceId, params Permission[] permissions)
//	  {
//		// assuming that there are no default authorizations for
//		if (!string.ReferenceEquals(userId, null))
//		{
//		  EnsureUtil.ensureValidIndividualResourceId("Cannot create authorization for user " + userId, userId);
//		}
//		if (!string.ReferenceEquals(groupId, null))
//		{
//		  EnsureUtil.ensureValidIndividualResourceId("Cannot create authorization for group " + groupId, groupId);
//		}

//		AuthorizationEntity authorization = new AuthorizationEntity(AUTH_TYPE_GRANT);
//		authorization.UserId = userId;
//		authorization.GroupId = groupId;
//		authorization.setResource(resource);
//		authorization.ResourceId = resourceId;

//		if (permissions != null)
//		{
//		  foreach (Permission permission in permissions)
//		  {
//			authorization.addPermission(permission);
//		  }
//		}

//		return authorization;
//	  }

//	  protected internal virtual Permission DefaultUserPermissionForTask
//	  {
//		  get
//		  {
//			return Context.ProcessEngineConfiguration.DefaultUserPermissionForTask;
//		  }
//	  }

//	  /// <summary>
//	  /// Searches through the cache, if there is already an authorization with same rights. If that's the case
//	  /// update the given authorization with the permissions and remove the old one from the cache.
//	  /// </summary>
//	  protected internal virtual void updateAuthorizationBasedOnCacheEntries(AuthorizationEntity authorization, string userId, string groupId, Resource resource, string resourceId)
//	  {
//		DbEntityManager dbManager = Context.CommandContext.DbEntityManager;
//		IList<AuthorizationEntity> list = dbManager.getCachedEntitiesByType(typeof(AuthorizationEntity));
//		foreach (AuthorizationEntity authEntity in list)
//		{
//		  bool hasSameAuthRights = hasEntitySameAuthorizationRights(authEntity, userId, groupId, resource, resourceId);
//		  if (hasSameAuthRights)
//		  {
//			int previousPermissions = authEntity.getPermissions();
//			authorization.setPermissions(previousPermissions);
//			dbManager.DbEntityCache.remove(authEntity);
//			return;
//		  }
//		}
//	  }

//	  protected internal virtual bool hasEntitySameAuthorizationRights(AuthorizationEntity authEntity, string userId, string groupId, Resource resource, string resourceId)
//	  {
//		bool sameUserId = areIdsEqual(authEntity.UserId, userId);
//		bool sameGroupId = areIdsEqual(authEntity.GroupId, groupId);
//		bool sameResourceId = areIdsEqual(authEntity.ResourceId, (resourceId));
//		bool sameResourceType = authEntity.ResourceType == resource.resourceType();
//		bool sameAuthorizationType = authEntity.AuthorizationType == AUTH_TYPE_GRANT;
//		return sameUserId && sameGroupId && sameResourceType && sameResourceId && sameAuthorizationType;
//	  }

//	  protected internal virtual bool areIdsEqual(string firstId, string secondId)
//	  {
//		if (string.ReferenceEquals(firstId, null) || string.ReferenceEquals(secondId, null))
//		{
//		  return string.ReferenceEquals(firstId, secondId);
//		}
//		else
//		{
//		  return firstId.Equals(secondId);
//		}
//	  }
//	}

//}

