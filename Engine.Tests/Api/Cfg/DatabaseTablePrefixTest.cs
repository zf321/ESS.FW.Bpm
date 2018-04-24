

//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{
    
//    /// <summary>
//	/// 
//	/// </summary>
//	public class DatabaseTablePrefixTest
//	{

////JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @Test public void shouldPerformDatabaseSchemaOperationCreate() throws Exception
////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//	  public virtual void shouldPerformDatabaseSchemaOperationCreate()
//	  {

//		// both process engines will be using this datasource.
//		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

//		// create two schemas is the database
//		Connection connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("drop schema if exists SCHEMA1");
//		connection.CreateStatement().Execute("drop schema if exists SCHEMA2");
//		connection.CreateStatement().Execute("create schema SCHEMA1");
//		connection.CreateStatement().Execute("create schema SCHEMA2");
//		connection.Close();

//		// configure & build two different process engines, each having a separate table prefix
//		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().SetProcessEngineName("DatabaseTablePrefixTest-engine1").SetDataSource(pooledDataSource).SetDbMetricsReporterActivate(false).SetDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
//		config1.DatabaseTablePrefix = "SCHEMA1.";
//		config1.UseSharedSqlSessionFactory = true;
//		IProcessEngine engine1 = config1.BuildProcessEngine();

//		ProcessEngineConfigurationImpl config2 = createCustomProcessEngineConfiguration().SetProcessEngineName("DatabaseTablePrefixTest-engine2").SetDataSource(pooledDataSource).SetDbMetricsReporterActivate(false).SetDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
//		config2.DatabaseTablePrefix = "SCHEMA2.";
//		config2.UseSharedSqlSessionFactory = true;
//		IProcessEngine engine2 = config2.BuildProcessEngine();

//		// create the tables in SCHEMA1
//		connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("set schema SCHEMA1");
//		engine1.ManagementService.DatabaseSchemaUpgrade(connection, "", "SCHEMA1");
//		connection.Close();

//		// create the tables in SCHEMA2
//		connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("set schema SCHEMA2");
//		engine2.ManagementService.DatabaseSchemaUpgrade(connection, "", "SCHEMA2");
//		connection.Close();

//		// if I deploy a process to one engine, it is not visible to the other
//		// engine:
//		try
//		{
//		  engine1.RepositoryService.CreateDeployment().AddClasspathResource("resources/api/cfg/oneJobProcess.bpmn20.xml").Deploy();

//		  Assert.AreEqual(1, engine1.RepositoryService.CreateDeploymentQuery().Count());
//		  Assert.AreEqual(0, engine2.RepositoryService.CreateDeploymentQuery().Count());

//		}
//		finally
//		{
//		  engine1.Close();
//		  engine2.Close();
//		  ProcessEngineConfigurationImpl.cachedSqlSessionFactory = null;
//		}
//	  }


//	  //----------------------- TEST HELPERS -----------------------

//	  // allows to return a process engine configuration which doesn't create a schema when it's build.
//	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
//	  {

//		public override IProcessEngine buildProcessEngine()
//		{
//		  init();
//		  return new NoSchemaProcessEngineImpl(this, this);
//		}

//		internal class NoSchemaProcessEngineImpl : ProcessEngineImpl
//		{
//			private readonly DatabaseTablePrefixTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance;

//		  public NoSchemaProcessEngineImpl(DatabaseTablePrefixTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  protected internal override void executeSchemaOperations()
//		  {
//			// nop - do not execute create schema operations
//		  }
//		}

//	  }

//	  private static ProcessEngineConfigurationImpl createCustomProcessEngineConfiguration()
//	  {
//		return (new CustomStandaloneInMemProcessEngineConfiguration()).SetHistory(ProcessEngineConfiguration.HistoryFull);
//	  }

//	}

//}