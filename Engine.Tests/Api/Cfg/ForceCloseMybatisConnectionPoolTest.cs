
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{

//    /// <summary>
//    /// 
//    /// 
//    /// </summary>
//    public class ForceCloseMybatisConnectionPoolTest
//    {


//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void testForceCloseMybatisConnectionPoolTrue()
//        public virtual void testForceCloseMybatisConnectionPoolTrue()
//        {

//            // given
//            // that the process engine is configured with forceCloseMybatisConnectionPool = true
//            ProcessEngineConfigurationImpl configurationImpl = (new StandaloneInMemProcessEngineConfiguration()).SetJdbcUrl("jdbc:h2:mem:camunda-forceclose").SetProcessEngineName("engine-forceclose").SetForceCloseMybatisConnectionPool(true);

//            IProcessEngine processEngine = configurationImpl.BuildProcessEngine();

//            PooledDataSource pooledDataSource = (PooledDataSource)configurationImpl.DataSource;
//            PoolState state = pooledDataSource.PoolState;


//            // then
//            // if the process engine is closed
//            processEngine.Close();

//            // the idle connections are closed
//            Assert.True(state.IdleConnectionCount == 0);

//        }

//        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//        //ORIGINAL LINE: @Test public void testForceCloseMybatisConnectionPoolFalse()
//        public virtual void testForceCloseMybatisConnectionPoolFalse()
//        {

//            // given
//            // that the process engine is configured with forceCloseMybatisConnectionPool = false
//            ProcessEngineConfigurationImpl configurationImpl = (new StandaloneInMemProcessEngineConfiguration()).SetJdbcUrl("jdbc:h2:mem:camunda-forceclose").SetProcessEngineName("engine-forceclose").SetForceCloseMybatisConnectionPool(false);

//            IProcessEngine processEngine = configurationImpl.BuildProcessEngine();

//            PooledDataSource pooledDataSource = (PooledDataSource)configurationImpl.DataSource;
//            PoolState state = pooledDataSource.PoolState;
//            int idleConnections = state.IdleConnectionCount;


//            // then
//            // if the process engine is closed
//            processEngine.Close();

//            // the idle connections are not closed
//            Assert.AreEqual(state.IdleConnectionCount, idleConnections);

//            pooledDataSource.ForceCloseAll();

//            Assert.True(state.IdleConnectionCount == 0);
//        }

//    }

//}