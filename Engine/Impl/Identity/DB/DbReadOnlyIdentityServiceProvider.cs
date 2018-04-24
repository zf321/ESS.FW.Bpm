//using System.Collections.Generic;
//using JZTERP.Common.Shared.Entities.Bpm.Entities;
//using org.camunda.bpm.engine.impl.db.entitymanager;
//using org.camunda.bpm.engine.impl.interceptor;

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

//	using Permission = org.camunda.bpm.engine.authorization.Permission;
//	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
//	using Resource = org.camunda.bpm.engine.authorization.Resource;
//	using Resources = org.camunda.bpm.engine.authorization.Resources;
//	using Group = org.camunda.bpm.engine.identity.Group;
//	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
//	using Tenant = org.camunda.bpm.engine.identity.Tenant;
//	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
//	using User = org.camunda.bpm.engine.identity.User;
//	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
//	using Context = org.camunda.bpm.engine.impl.context.Context;


////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.impl.Util.EncryptionUtil.saltPassword;

//	/// <summary>
//	/// <para>Read only implementation of DB-backed identity service</para>
//	/// 
//	/// 
//	/// 
//	/// </summary>
////JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") public class DbReadOnlyIdentityServiceProvider extends org.camunda.bpm.engine.impl.persistence.AbstractManager implements org.camunda.bpm.engine.impl.identity.ReadOnlyIdentityProvider
//	public class DbReadOnlyIdentityServiceProvider : AbstractManager, ReadOnlyIdentityProvider
//	{

//	  // users /////////////////////////////////////////

//	  public virtual UserEntity findUserById(string userId)
//	  {
//		checkAuthorization(Permissions.READ, Resources.USER, userId);
//		return DbEntityManager.selectById(typeof(UserEntity), userId);
//	  }

//	  public virtual UserQuery createUserQuery()
//	  {
//		return new DbUserQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
//	  }

//	  public virtual UserQueryImpl createUserQuery(CommandContext commandContext)
//	  {
//		return new DbUserQueryImpl();
//	  }

//	  public virtual long findUserCountByQueryCriteria(DbUserQueryImpl query)
//	  {
//		configureQuery(query, Resources.USER);
//		return (long?) DbEntityManager.selectOne("selectUserCountByQueryCriteria", query).Value;
//	  }

//	  public virtual IList<User> findUserByQueryCriteria(DbUserQueryImpl query)
//	  {
//		configureQuery(query, Resources.USER);
//		return DbEntityManager.selectList("selectUserByQueryCriteria", query);
//	  }

//	  public virtual bool checkPassword(string userId, string password)
//	  {
//		UserEntity user = findUserById(userId);
//		if ((user != null) && (!string.ReferenceEquals(password, null)) && matchPassword(password, user))
//		{
//		  return true;
//		}
//		else
//		{
//		  return false;
//		}
//	  }

//	  protected internal virtual bool matchPassword(string password, UserEntity user)
//	  {
//		string saltedPassword = saltPassword(password, user.Salt);
//		return Context.ProcessEngineConfiguration.PasswordManager.check(saltedPassword, user.Password);
//	  }

//	  // groups //////////////////////////////////////////

//	  public virtual GroupEntity findGroupById(string groupId)
//	  {
//		checkAuthorization(Permissions.READ, Resources.GROUP, groupId);
//		return DbEntityManager.selectById(typeof(GroupEntity), groupId);
//	  }

//	  public virtual GroupQuery createGroupQuery()
//	  {
//		return new DbGroupQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
//	  }

//	  public virtual GroupQuery createGroupQuery(CommandContext commandContext)
//	  {
//		return new DbGroupQueryImpl();
//	  }

//	  public virtual long findGroupCountByQueryCriteria(DbGroupQueryImpl query)
//	  {
//		configureQuery(query, Resources.GROUP);
//		return (long?) DbEntityManager.selectOne("selectGroupCountByQueryCriteria", query).Value;
//	  }

//	  public virtual IList<Group> findGroupByQueryCriteria(DbGroupQueryImpl query)
//	  {
//		configureQuery(query, Resources.GROUP);
//		return DbEntityManager.selectList("selectGroupByQueryCriteria", query);
//	  }

//	  //tenants //////////////////////////////////////////

//	  public virtual TenantEntity findTenantById(string tenantId)
//	  {
//		checkAuthorization(Permissions.READ, Resources.TENANT, tenantId);
//		return DbEntityManager.selectById(typeof(TenantEntity), tenantId);
//	  }

//	  public virtual TenantQuery createTenantQuery()
//	  {
//		return new DbTenantQueryImpl(Context.ProcessEngineConfiguration.CommandExecutorTxRequired);
//	  }

//	  public virtual TenantQuery createTenantQuery(CommandContext commandContext)
//	  {
//		return new DbTenantQueryImpl();
//	  }

//	  public virtual long findTenantCountByQueryCriteria(DbTenantQueryImpl query)
//	  {
//		configureQuery(query, Resources.TENANT);
//		return (long?) DbEntityManager.selectOne("selectTenantCountByQueryCriteria", query).Value;
//	  }

//	  public virtual IList<Tenant> findTenantByQueryCriteria(DbTenantQueryImpl query)
//	  {
//		configureQuery(query, Resources.TENANT);
//		return DbEntityManager.selectList("selectTenantByQueryCriteria", query);
//	  }

//	  //authorizations ////////////////////////////////////////////////////

////JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Override protected void configureQuery(@SuppressWarnings("rawtypes") org.camunda.bpm.engine.impl.AbstractQuery query, org.camunda.bpm.engine.authorization.Resource resource)
//	  protected internal override void configureQuery(("rawtypes") AbstractQuery query, Resource resource)
//	  {
//		Context.CommandContext.AuthorizationManager.configureQuery(query, resource);
//	  }

//	  protected internal override void checkAuthorization(Permission permission, Resource resource, string resourceId)
//	  {
//		Context.CommandContext.AuthorizationManager.checkAuthorization(permission, resource, resourceId);
//	  }

//	}

//}

