//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Authorization;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Authorization
//{
    

    
//	public class GroupAuthorizationTest : AuthorizationTest
//	{

//	  public const string testUserId = "testUser";
//	  public static readonly IList<string> testGroupIds = Arrays.asList("testGroup1", "testGroup2", "testGroup3");

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: @Override protected void setUp() throws Exception
//	  protected internal void setUp()
//	  {
//		createUser(testUserId);
//		foreach (string testGroupId in testGroupIds)
//		{
//		  createGroupAndAddUser(testGroupId, testUserId);
//		}

//		identityService.SetAuthentication(testUserId, testGroupIds);
//		processEngineConfiguration.SetAuthorizationEnabled(true);
//	  }


//	  public virtual void testTaskQueryWithoutGroupAuthorizations()
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
//	  }

//	  private class CommandAnonymousInnerClass : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

//			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(processEngine.TaskService.CreateTaskQuery());
//			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
//			when(taskQuery.AuthCheck).ThenReturn(authCheck);

//			taskQuery.ToList();

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));
//			verify(authCheck).AuthGroupIds = eq(System.Linq.Enumerable.Empty<string>());

//			return null;
//		  }
//	  }

//	  public virtual void testTaskQueryWithOneGroupAuthorization()
//	  {
//		createGroupGrantAuthorization(Resources.Task, AuthorizationFields.Any, testGroupIds[0]);

//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
//	  }

//	  private class CommandAnonymousInnerClass2 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass2(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

//			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(processEngine.TaskService.CreateTaskQuery());
//			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
//			when(taskQuery.AuthCheck).ThenReturn(authCheck);

//			taskQuery.ToList();

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));
//			verify(authCheck).AuthGroupIds = eq(testGroupIds.subList(0, 1));

//			return null;
//		  }
//	  }

//	  public virtual void testTaskQueryWithGroupAuthorization()
//	  {
//		foreach (string testGroupId in testGroupIds)
//		{
//		  createGroupGrantAuthorization(Resources.Task, AuthorizationFields.Any, testGroupId);
//		}

//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass3(this));
//	  }

//	  private class CommandAnonymousInnerClass3 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass3(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

//			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(processEngine.TaskService.CreateTaskQuery());
//			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
//			when(taskQuery.AuthCheck).ThenReturn(authCheck);

//			taskQuery.ToList();

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));
//			verify(authCheck, atLeastOnce()).AuthGroupIds = (IList<string>) argThat(containsInAnyOrder(testGroupIds.ToArray()));

//			return null;
//		  }
//	  }

//	  public virtual void testTaskQueryWithUserWithoutGroups()
//	  {
//		identityService.SetAuthentication(testUserId, null);

//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass4(this));
//	  }

//	  private class CommandAnonymousInnerClass4 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass4(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

//			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(processEngine.TaskService.CreateTaskQuery());
//			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
//			when(taskQuery.AuthCheck).ThenReturn(authCheck);

//			taskQuery.ToList();

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq((IList<string>) null));
//			verify(authCheck).AuthGroupIds = eq(System.Linq.Enumerable.Empty<string>());

//			return null;
//		  }
//	  }

//	  public virtual void testCheckAuthorizationWithoutGroupAuthorizations()
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass5(this));
//	  }

//	  private class CommandAnonymousInnerClass5 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass5(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
//			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

//			authorizationService.IsUserAuthorized(testUserId, testGroupIds, Permissions.Read, Resources.Task);

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));

//			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.ForClass(typeof(AuthorizationCheck));
//			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

//			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
//			Assert.True(authorizationCheck.AuthGroupIds.Empty);

//			return null;
//		  }
//	  }

//	  public virtual void testCheckAuthorizationWithOneGroupAuthorizations()
//	  {
//		createGroupGrantAuthorization(Resources.Task, AuthorizationFields.Any, testGroupIds[0]);

//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass6(this));
//	  }

//	  private class CommandAnonymousInnerClass6 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass6(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
//			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

//			authorizationService.IsUserAuthorized(testUserId, testGroupIds, Permissions.Read, Resources.Task);

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));

//			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.ForClass(typeof(AuthorizationCheck));
//			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

//			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
//			Assert.AreEqual(testGroupIds.subList(0, 1), authorizationCheck.AuthGroupIds);

//			return null;
//		  }
//	  }

//	  public virtual void testCheckAuthorizationWithGroupAuthorizations()
//	  {
//		foreach (string testGroupId in testGroupIds)
//		{
//		  createGroupGrantAuthorization(Resources.Task, AuthorizationFields.Any, testGroupId);
//		}

//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass7(this));
//	  }

//	  private class CommandAnonymousInnerClass7 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass7(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
//			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

//			authorizationService.IsUserAuthorized(testUserId, testGroupIds, Permissions.Read, Resources.Task);

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq(testGroupIds));

//			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.ForClass(typeof(AuthorizationCheck));
//			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

//			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
//			Assert.That(authorizationCheck.AuthGroupIds, containsInAnyOrder(testGroupIds.ToArray()));

//			return null;
//		  }
//	  }

//	  public virtual void testCheckAuthorizationWithUserWithoutGroups()
//	  {
//		processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass8(this));
//	  }

//	  private class CommandAnonymousInnerClass8 : ICommand<object>
//	  {
//		  private readonly GroupAuthorizationTest outerInstance;

//		  public CommandAnonymousInnerClass8(GroupAuthorizationTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
//			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

//			authorizationService.IsUserAuthorized(testUserId, null, Permissions.Read, Resources.Task);

//			verify(authorizationManager, atLeastOnce()).FilterAuthenticatedGroupIds(eq((IList<string>) null));

//			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.ForClass(typeof(AuthorizationCheck));
//			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

//			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
//			Assert.True(authorizationCheck.AuthGroupIds.Empty);

//			return null;
//		  }
//	  }

//	  protected internal virtual void createGroupGrantAuthorization(Resource resource, string resourceId, string groupId, params Permission[] permissions)
//	  {
//		IAuthorization authorization = createGrantAuthorization(resource, resourceId);
//		authorization.GroupId = groupId;
//		foreach (Permission permission in permissions)
//		{
//		  authorization.AddPermission(permission);
//		}
//		saveAuthorization(authorization);
//	  }

//	  protected internal virtual void createGroupAndAddUser(string groupId, string userId)
//	  {
//		createGroup(groupId);
//		identityService.CreateMembership(userId, groupId);
//	  }

//	  protected internal virtual T spyOnSession<T>(CommandContext commandContext, Type sessionClass) where T :ISession
//	  {
//		T manager = commandContext.GetSession(sessionClass);
//		T spy = spy(manager);
//		commandContext.Sessions.put(sessionClass, spy);

//		return spy;
//	  }

//	}

//}