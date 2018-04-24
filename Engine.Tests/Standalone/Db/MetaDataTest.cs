using System;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Db
{




    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class MetaDataTest : PluggableProcessEngineTestCase
	{

        //private static Logger LOG = ProcessEngineLogger.l.Logger;

        [Test]
        public virtual void TestMetaData()
	  {
		((ProcessEngineConfigurationImpl)((ProcessEngineImpl)ProcessEngine).ProcessEngineConfiguration).CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : ICommand<object>
	  {
		  private readonly MetaDataTest _outerInstance;

		  public CommandAnonymousInnerClass(MetaDataTest outerInstance)
		  {
			  this._outerInstance = outerInstance;
		  }

		  public virtual object Execute(CommandContext commandContext)
		  {
			// PRINT THE TABLE NAMES TO CHECK IF WE CAN USE METADATA INSTEAD
			// THIS IS INTENDED FOR TEST THAT SHOULD RUN ON OUR QA INFRASTRUCTURE TO SEE IF METADATA
			// CAN BE USED INSTEAD OF PERFORMING A QUERY THAT MIGHT FAIL
			try
			{
			  //SqlSession sqlSession = commandContext.GetSession(typeof(DbSqlSession)).SqlSession;
			  //ResultSet tables = sqlSession.Connection.MetaData.GetTables(null, null, null, null);
			 // while (tables.Next())
			 // {
				//ResultSetMetaData resultSetMetaData = tables.MetaData;
				//int columnCount = resultSetMetaData.ColumnCount;
				//for (int i = 1; i <= columnCount; i++)
				//{
				//  Console.WriteLine("result set column " + i + " | " + resultSetMetaData.GetColumnName(i) + " | " + resultSetMetaData.GetColumnLabel(i) + " | " + tables.GetString(i));
				//}
    //                    Console.WriteLine("-------------------------------------------------------");
			 // }
			}
			catch (System.Exception e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}
			return null;
		  }
	  }

        [Test]
        public virtual void TestMariaDbDatabaseType()
	  {
		if (MariaDbConfigured)
		{
		  Assert.AreEqual("mariadb", processEngineConfiguration.DatabaseType);
		}
	  }

	  public virtual bool MariaDbConfigured
	  {
		  get
		  {
			return processEngineConfiguration.JdbcUrl.ToLower().Contains("mariadb") || processEngineConfiguration.JdbcDriver.ToLower().Contains("mariadb");
		  }
	  }
	}

}