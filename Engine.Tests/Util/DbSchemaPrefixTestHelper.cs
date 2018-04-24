
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.Util;

//namespace ESS.FW.Bpm.Engine.Tests.Util
//{
    

//	/// <summary>
//	/// <para>Test utility allowing to run the testsuite with a database
//	/// table prefix</para>
//	/// 
//	/// 
//	/// 
//	/// </summary>
//	public class DbSchemaPrefixTestHelper : InitializingBean, DisposableBean
//	{

//	  private PooledDataSource _dataSource;

////JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void afterPropertiesSet() throws Exception
//	  public virtual void AfterPropertiesSet()
//	  {

//		_dataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000;MVCC=TRUE;", "sa", "");

//		// create schema in the
//		Connection connection = _dataSource.Connection;
//		connection.CreateStatement().Execute("drop schema if exists SCHEMA1");
//		connection.CreateStatement().Execute("create schema SCHEMA1");
//		connection.Close();

//		ProcessEngineConfigurationImpl config1 = CreateCustomProcessEngineConfiguration().SetProcessEngineName("DatabaseTablePrefixTest-engine1").SetDataSource(_dataSource).SetDbMetricsReporterActivate(false).SetDatabaseSchemaUpdate("NO_CHECK"); // disable auto create/drop schema
//		config1.DatabaseTablePrefix = "SCHEMA1.";
//		IProcessEngine engine1 = config1.BuildProcessEngine();

//		// create the tables in SCHEMA1
//		connection = _dataSource.Connection;
//		connection.CreateStatement().Execute("set schema SCHEMA1");
//		engine1.ManagementService.DatabaseSchemaUpgrade(connection, "", "SCHEMA1");
//		connection.Close();

//		engine1.Close();

//	  }

////JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void destroy() throws Exception
//	  public virtual void Destroy()
//	  {
//		Connection connection = _dataSource.Connection;
//		connection.CreateStatement().Execute("drop schema if exists SCHEMA1");
//		connection.Close();
//	  }

//	  //----------------------- TEST HELPERS -----------------------

//	  // allows to return a process engine configuration which doesn't create a schema when it's build.
//	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
//	  {

//		public virtual IProcessEngine BuildProcessEngine()
//		{
//		  init();
//		  return new NoSchemaProcessEngineImpl(this, this);
//		}

//		internal class NoSchemaProcessEngineImpl : ProcessEngineImpl
//		{
//			private readonly DbSchemaPrefixTestHelper.CustomStandaloneInMemProcessEngineConfiguration _outerInstance;

//		  public NoSchemaProcessEngineImpl(DbSchemaPrefixTestHelper.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
//		  {
//			  this._outerInstance = outerInstance;
//		  }

//		  protected internal virtual void ExecuteSchemaOperations()
//		  {
//			// nop - do not execute create schema operations
//		  }
//		}

//	  }

//	  private static ProcessEngineConfigurationImpl CreateCustomProcessEngineConfiguration()
//	  {
//		return (new CustomStandaloneInMemProcessEngineConfiguration()).SetHistory(ProcessEngineConfiguration.HistoryFull);
//	  }

//	}

//}