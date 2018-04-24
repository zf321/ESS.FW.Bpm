

//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.Cfg;
//using ESS.FW.Bpm.Engine.Impl.DB.Sql;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Api.Cfg
//{

    

//    /// <summary>
//	/// @author Ronny Bräunlich
//	/// </summary>
//	public class DatabaseTableSchemaTest : TestCase
//	{

//	  private const string SCHEMA_NAME = "SCHEMA1";
//	  private const string PREFIX_NAME = "PREFIX1_";

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void testPerformDatabaseSchemaOperationCreateTwice() throws Exception
//	  public virtual void testPerformDatabaseSchemaOperationCreateTwice()
//	  {

//		// both process engines will be using this datasource.
//		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

//		Connection connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("drop schema if exists " + SCHEMA_NAME);
//		connection.CreateStatement().Execute("create schema " + SCHEMA_NAME);
//		connection.Close();

//		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().SetProcessEngineName("DatabaseTablePrefixTest-engine1").SetDataSource(pooledDataSource).SetDatabaseSchemaUpdate("NO_CHECK");
//		// disable auto create/drop schema
//		config1.DatabaseTablePrefix = SCHEMA_NAME + ".";
//		config1.DatabaseSchema = SCHEMA_NAME;
//		config1.DbMetricsReporterActivate = false;
//		IProcessEngine engine1 = config1.BuildProcessEngine();

//		// create the tables for the first time
//		connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("set schema " + SCHEMA_NAME);
//		engine1.ManagementService.DatabaseSchemaUpgrade(connection, "", SCHEMA_NAME);
//		connection.Close();
//		// create the tables for the second time; here we shouldn't crash since the
//		// session should tell us that the tables are already present and
//		// databaseSchemaUpdate is set to noop
//		connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("set schema " + SCHEMA_NAME);
//		engine1.ManagementService.DatabaseSchemaUpgrade(connection, "", SCHEMA_NAME);
//		engine1.Close();
//	  }

////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
////ORIGINAL LINE: public void testTablePresentWithSchemaAndPrefix() throws java.sql.SQLException
//	  public virtual void testTablePresentWithSchemaAndPrefix()
//	  {
//		PooledDataSource pooledDataSource = new PooledDataSource(ReflectUtil.ClassLoader, "org.h2.Driver", "jdbc:h2:mem:DatabaseTablePrefixTest;DB_CLOSE_DELAY=1000", "sa", "");

//		Connection connection = pooledDataSource.Connection;
//		connection.CreateStatement().Execute("drop schema if exists " + SCHEMA_NAME);
//		connection.CreateStatement().Execute("create schema " + SCHEMA_NAME);
//		connection.CreateStatement().Execute("create table " + SCHEMA_NAME + "." + PREFIX_NAME + "SOME_TABLE(id varchar(64));");
//		connection.Close();

//		ProcessEngineConfigurationImpl config1 = createCustomProcessEngineConfiguration().SetProcessEngineName("DatabaseTablePrefixTest-engine1").SetDataSource(pooledDataSource).SetDatabaseSchemaUpdate("NO_CHECK");
//		// disable auto create/drop schema
//		config1.DatabaseTablePrefix = SCHEMA_NAME + "." + PREFIX_NAME;
//		config1.DatabaseSchema = SCHEMA_NAME;
//		config1.DbMetricsReporterActivate = false;
//		IProcessEngine engine = config1.BuildProcessEngine();
//		CommandExecutor commandExecutor = config1.CommandExecutorTxRequired;

//		commandExecutor.Execute(new CommandAnonymousInnerClass(this));

//		engine.Close();

//	  }

//	  private class CommandAnonymousInnerClass : ICommand<object>
//	  {
//		  private readonly DatabaseTableSchemaTest outerInstance;

//		  public CommandAnonymousInnerClass(DatabaseTableSchemaTest outerInstance)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  public virtual object Execute(CommandContext commandContext)
//		  {
//			DbSqlSession sqlSession = commandContext.GetSession(typeof(DbSqlSession));
//			Assert.True(sqlSession.IsTablePresent("SOME_TABLE"));
//			return null;
//		  }
//	  }

//	  public virtual void testCreateConfigurationWithMismatchtingSchemaAndPrefix()
//	  {
//		try
//		{
//		  StandaloneInMemProcessEngineConfiguration configuration = new StandaloneInMemProcessEngineConfiguration();
//		  configuration.DatabaseSchema = "foo";
//		  configuration.DatabaseTablePrefix = "bar";
//		  configuration.BuildProcessEngine();
//		  Assert.Fail("Should throw exception");
//		}
//		catch (ProcessEngineException e)
//		{
//		  // as expected
//		  Assert.True(e.Message.Contains("When setting a schema the prefix has to be schema + '.'"));
//		}
//	  }

//	  public virtual void testCreateConfigurationWithMissingDotInSchemaAndPrefix()
//	  {
//		try
//		{
//		  StandaloneInMemProcessEngineConfiguration configuration = new StandaloneInMemProcessEngineConfiguration();
//		  configuration.DatabaseSchema = "foo";
//		  configuration.DatabaseTablePrefix = "foo";
//		  configuration.BuildProcessEngine();
//		  Assert.Fail("Should throw exception");
//		}
//		catch (ProcessEngineException e)
//		{
//		  // as expected
//		  Assert.True(e.Message.Contains("When setting a schema the prefix has to be schema + '.'"));
//		}
//	  }

//	  // ----------------------- TEST HELPERS -----------------------

//	  // allows to return a process engine configuration which doesn't create a
//	  // schema when it's build.
//	  private class CustomStandaloneInMemProcessEngineConfiguration : StandaloneInMemProcessEngineConfiguration
//	  {

//		public virtual IProcessEngine buildProcessEngine()
//		{
//		  init();
//		  return new NoSchemaProcessEngineImpl(this, this);
//		}

//		internal class NoSchemaProcessEngineImpl : ProcessEngineImpl
//		{
//			private readonly DatabaseTableSchemaTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance;

//		  public NoSchemaProcessEngineImpl(DatabaseTableSchemaTest.CustomStandaloneInMemProcessEngineConfiguration outerInstance, ProcessEngineConfigurationImpl processEngineConfiguration) : base(processEngineConfiguration)
//		  {
//			  this.outerInstance = outerInstance;
//		  }

//		  protected internal virtual void executeSchemaOperations()
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