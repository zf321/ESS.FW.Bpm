

//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Common.Configurations;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{
    


//	/// <summary>
//	/// 
//	/// </summary>
//	public class ConnectionPoolTest : PvmTestCase
//	{

//	  public virtual void testMyBatisConnectionPoolProperlyConfigured()
//	  {
//		ProcessEngineConfigurationImpl config = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource("resources/api/cfg/connection-pool.Camunda.cfg.xml");

//		IProcessEngine engine = config.BuildProcessEngine();

//		// Expected values
//		int maxActive = 25;
//		int maxIdle = 10;
//		int maxCheckoutTime = 30000;
//		int maxWaitTime = 25000;
//		int? jdbcStatementTimeout = 300;

//		Assert.AreEqual(maxActive, config.JdbcMaxActiveConnections);
//		Assert.AreEqual(maxIdle, config.JdbcMaxIdleConnections);
//		Assert.AreEqual(maxCheckoutTime, config.JdbcMaxCheckoutTime);
//		Assert.AreEqual(maxWaitTime, config.JdbcMaxWaitTime);
//		Assert.AreEqual(jdbcStatementTimeout, config.JdbcStatementTimeout);

//		// Verify that these properties are correctly set in the MyBatis datasource
//		Configuration sessionFactoryConfiguration = config.DbSqlSessionFactory.SqlSessionFactory.Configuration;
//		DataSource datasource = sessionFactoryConfiguration.Environment.DataSource;
//		Assert.True(datasource is PooledDataSource);

//		PooledDataSource pooledDataSource = (PooledDataSource) datasource;
//		Assert.AreEqual(maxActive, pooledDataSource.PoolMaximumActiveConnections);
//		Assert.AreEqual(maxIdle, pooledDataSource.PoolMaximumIdleConnections);
//		Assert.AreEqual(maxCheckoutTime, pooledDataSource.PoolMaximumCheckoutTime);
//		Assert.AreEqual(maxWaitTime, pooledDataSource.PoolTimeToWait);

//		Assert.AreEqual(jdbcStatementTimeout, sessionFactoryConfiguration.DefaultStatementTimeout);

//		engine.Close();
//	  }

//	}

//}