//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.context.Impl;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
//using ESS.FW.Bpm.Engine.Impl.Identity;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Persistence.Entity;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{
//    // Todo: mockito-core-1.9.5.jar
//    public class AuthorizationCheckRevokesCfgTest
//    {

//        private static readonly IList<string> AUTHENTICATED_GROUPS = new List<string> { "aGroup" };
//        private const string AUTHENTICATED_USER_ID = "userId";

//        internal CommandContext mockedCmdContext;
//        internal ProcessEngineConfigurationImpl mockedConfiguration;
//        internal AuthorizationManager authorizationManager;
//        internal DbEntityManager mockedEntityManager;

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Before public void setup()
//        public virtual void setup()
//        {

//            mockedCmdContext = mock(typeof(CommandContext));
//            mockedConfiguration = mock(typeof(ProcessEngineConfigurationImpl));
//            authorizationManager = spy(new AuthorizationManager());
//            mockedEntityManager = mock(typeof(DbEntityManager));

//            when(mockedCmdContext.GetSession(eq(typeof(DbEntityManager)))).ThenReturn(mockedEntityManager);

//            when(authorizationManager.FilterAuthenticatedGroupIds(eq(AUTHENTICATED_GROUPS))).ThenReturn(AUTHENTICATED_GROUPS);
//            when(mockedCmdContext.Authentication).ThenReturn(new Authentication(AUTHENTICATED_USER_ID, AUTHENTICATED_GROUPS));
//            when(mockedCmdContext.AuthorizationCheckEnabled).ThenReturn(true);
//            when(mockedConfiguration.AuthorizationEnabled).ThenReturn(true);

//            Context.CommandContext = mockedCmdContext;
//            Context.ProcessEngineConfiguration = mockedConfiguration;
//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @After public void cleanup()
//        public virtual void cleanup()
//        {
//            Context.RemoveCommandContext();
//            Context.RemoveProcessEngineConfiguration();
//        }

//       [Test]
//        public virtual void shouldUseCfgValue_always()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_ALWAYS);

//            // if
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(true, authCheck.RevokeAuthorizationCheckEnabled);
//            verifyNoMoreInteractions(mockedEntityManager);
//        }

//        [Test]
//        public virtual void shouldUseCfgValue_never()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn(ProcessEngineConfiguration.AuthorizationCheckRevokeNever);

//            // if
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(false, authCheck.RevokeAuthorizationCheckEnabled);
//            verify(mockedEntityManager, never()).selectBoolean(eq("selectRevokeAuthorization"), any());
//            verifyNoMoreInteractions(mockedEntityManager);
//        }

//        [Test]
//        public virtual void shouldCheckDbForCfgValue_auto()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
//            Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
//            expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
//            expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
//            when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).ThenReturn(true);

//            // if
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(true, authCheck.RevokeAuthorizationCheckEnabled);
//            verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
//        }

//        [Test]
//        public virtual void shouldCheckDbForCfgValueWithNoRevokes_auto()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
//            Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
//            expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
//            expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
//            when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).ThenReturn(false);

//            // if
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(false, authCheck.RevokeAuthorizationCheckEnabled);
//            verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
//        }

//        [Test]
//        public virtual void shouldCheckDbForCfgCaseInsensitive()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
//            Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
//            expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
//            expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn("AuTo");
//            when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).ThenReturn(true);

//            // if
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(true, authCheck.RevokeAuthorizationCheckEnabled);
//            verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
//        }

//        [Test]
//        public virtual void shouldCacheCheck()
//        {
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject query = new org.Camunda.bpm.Engine.impl.Db.ListQueryParameterObject();
//            ListQueryParameterObject query = new ListQueryParameterObject();
//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.Db.AuthorizationCheck authCheck = query.GetAuthCheck();
//            AuthorizationCheck authCheck = query.AuthCheck;

//            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//            //ORIGINAL LINE: final java.util.HashMap<String, Object> expectedQueryParams = new java.util.HashMap<String, Object>();
//            Dictionary<string, object> expectedQueryParams = new Dictionary<string, object>();
//            expectedQueryParams["userId"] = AUTHENTICATED_USER_ID;
//            expectedQueryParams["authGroupIds"] = AUTHENTICATED_GROUPS;

//            // given
//            when(mockedConfiguration.AuthorizationCheckRevokes).ThenReturn(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO);
//            when(mockedEntityManager.selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams))).ThenReturn(true);

//            // if
//            authorizationManager.configureQuery(query);
//            authorizationManager.configureQuery(query);

//            // then
//            Assert.AreEqual(true, authCheck.RevokeAuthorizationCheckEnabled);
//            verify(mockedEntityManager, times(1)).selectBoolean(eq("selectRevokeAuthorization"), eq(expectedQueryParams));
//        }

//        [Test]
//        public virtual void testAutoIsDefault()
//        {
//            Assert.AreEqual(ProcessEngineConfiguration.AUTHORIZATION_CHECK_REVOKE_AUTO, (new StandaloneProcessEngineConfiguration()).AuthorizationCheckRevokes);
//        }

//    }

//}