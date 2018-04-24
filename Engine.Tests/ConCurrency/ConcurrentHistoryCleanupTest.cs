using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    /// <summary>
    /// <para>Tests the call to history cleanup simultaneously.</para>
    /// 
    /// <para><b>Note:</b> the test is not executed on H2 because it doesn't support the
    /// exclusive lock on table.</para>
    /// 
    /// @author Svetlana Dorokhova
    /// </summary>
    public class ConcurrentHistoryCleanupTest : ConcurrencyTestCase
    {

        [TearDown]
        public void tearDown()
        {
            ((ProcessEngineConfigurationImpl)ProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
            base.TearDown();
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly ConcurrentHistoryCleanupTest outerInstance;

            public CommandAnonymousInnerClass(ConcurrentHistoryCleanupTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {

                IList<IJob> jobs = ProcessEngine.ManagementService.CreateJobQuery().ToList();
                if (jobs.Count > 0)
                {
                    Assert.AreEqual(1, jobs.Count);
                    string jobId = jobs[0].Id;
                    commandContext.JobManager.DeleteJob((JobEntity)jobs[0]);
                    commandContext.HistoricJobLogManager.DeleteHistoricJobLogByJobId(jobId);
                }

                return null;
            }
        }

        // Todo: Parent Class -> TestClass
        //protected internal override void runTest()
        //{
        //    //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //    //ORIGINAL LINE: final Nullable<int> transactionIsolationLevel = getTransactionIsolationLevel();
        //    int? transactionIsolationLevel = TransactionIsolationLevel;

        //    if (DbSqlSessionFactory.H2.Equals(DatabaseType) || DbSqlSessionFactory.MARIADB.Equals(DatabaseType) || (transactionIsolationLevel != null && !transactionIsolationLevel.Equals(Connection.TRANSACTION_READ_COMMITTED)))
        //    {
        //        // skip test method - if database is H2
        //    }
        //    else
        //    {
        //        // invoke the test method
        //        base.RunTest();
        //    }
        //}

        // Todo: ProcessEngineConfigurationImpl.dbSqlSessionFactory
        //private string DatabaseType
        //{
        //    get
        //    {
        //        return processEngineConfiguration.DbSqlSessionFactory.DatabaseType;
        //    }
        //}

        private int? TransactionIsolationLevel
        {
            get
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final Nullable<int>[] transactionIsolation = new Nullable<int>[1];
                int?[] transactionIsolation = new int?[1];
                ((ProcessEngineConfigurationImpl)ProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this, transactionIsolation));
                return transactionIsolation[0];
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly ConcurrentHistoryCleanupTest outerInstance;

            private int?[] transactionIsolation;

            public CommandAnonymousInnerClass2(ConcurrentHistoryCleanupTest outerInstance, int?[] transactionIsolation)
            {
                this.outerInstance = outerInstance;
                this.transactionIsolation = transactionIsolation;
            }

            public object Execute(CommandContext commandContext)
            {
                try
                {
                    // Todo: No Property TransactionIsolation in SqlConnection
                    //transactionIsolation[0] = commandContext.DbSqlSession.SqlSession.Connection.TransactionIsolation;
                }
                catch (System.Exception ex)
                {

                }
                return null;
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void testRunTwoHistoryCleanups() throws InterruptedException
        public virtual void testRunTwoHistoryCleanups()
        {
            ThreadControl thread1 = ExecuteControllableCommand(new ControllableHistoryCleanupCommand());
            thread1.WaitForSync();

            ThreadControl thread2 = ExecuteControllableCommand(new ControllableHistoryCleanupCommand());
            thread2.WaitForSync();

            thread1.MakeContinue();
            thread1.WaitForSync();

            thread2.MakeContinue();

            Thread.Sleep(2000);

            thread1.WaitUntilDone();

            thread2.WaitForSync();
            thread2.WaitUntilDone();

            //only one history cleanup job exists -> no exception
            IJob historyCleanupJob = ProcessEngine.HistoryService.FindHistoryCleanupJob();
            Assert.NotNull(historyCleanupJob);

            Assert.IsNull(thread1.Exception);
            Assert.IsNull(thread2.Exception);

        }

        protected internal class ControllableHistoryCleanupCommand : ControllableCommand<object>
        {

            public override object Execute(CommandContext commandContext)
            {
                Monitor.Sync(); // thread will block here until makeContinue() is called form main thread

                // Todo: ESS.FW.Bpm.Engine.Impl.Cmd.HistoryCleanupCmd
                //(new HistoryCleanupCmd(true)).Execute(commandContext);

                Monitor.Sync(); // thread will block here until waitUntilDone() is called form main thread

                return null;
            }

        }

    }

}