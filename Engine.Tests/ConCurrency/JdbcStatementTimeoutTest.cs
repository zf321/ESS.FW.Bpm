using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    [TestFixture]
    public class JdbcStatementTimeoutTest : ConcurrencyTestCase
    {

        private const int STATEMENT_TIMEOUT_IN_SECONDS = 1;
        // some databases (like mysql and oracle) need more time to cancel the statement
        private const int TEST_TIMEOUT_IN_MILLIS = 10000;
        private const string JOB_ENTITY_ID = "42";

        private ThreadControl thread1;
        private ThreadControl thread2;



        protected internal override void InitializeProcessEngine()
        {
            ((AbstractProcessEngineTestCase)this).ProcessEngine = ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource("camunda.cfg.xml").SetJdbcStatementTimeout(STATEMENT_TIMEOUT_IN_SECONDS).BuildProcessEngine();
        }

        protected internal override void CloseDownProcessEngine()
        {
            ProcessEngine.Close();
            ((AbstractProcessEngineTestCase)this).ProcessEngine = null;
        }

        public virtual void testTimeoutOnUpdate()
        {
            createJobEntity();

            thread1 = ExecuteControllableCommand(new UpdateJobCommand("p1"));
            // wait for thread 1 to perform UPDATE
            thread1.WaitForSync();

            thread2 = ExecuteControllableCommand(new UpdateJobCommand("p2"));
            // wait for thread 2 to perform UPDATE
            thread2.WaitForSync();

            // perform FLUSH for thread 1 (but no commit of transaction)
            thread1.MakeContinue();
            // wait for thread 1 to perform FLUSH
            thread1.WaitForSync();

            // perform FLUSH for thread 2
            thread2.MakeContinue();
            // wait for thread 2 to cancel FLUSH because of timeout
            thread2.ReportInterrupts();
            thread2.WaitForSync(TEST_TIMEOUT_IN_MILLIS);

            Assert.NotNull(thread2.Exception, "expected timeout exception");
        }

        [TearDown]
        protected internal void tearDown()
        {

            thread1.WaitUntilDone();

            deleteJobEntities();
        }

        private void createJobEntity()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<JobEntity>
        {
            private readonly JdbcStatementTimeoutTest outerInstance;

            public CommandAnonymousInnerClass(JdbcStatementTimeoutTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }


            public JobEntity Execute(CommandContext commandContext)
            {
                MessageEntity jobEntity = new MessageEntity();
                jobEntity.Id = JOB_ENTITY_ID;
                jobEntity.Insert();

                return jobEntity;
            }
        }

        private void deleteJobEntities()
        {
            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this));
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly JdbcStatementTimeoutTest outerInstance;

            public CommandAnonymousInnerClass2(JdbcStatementTimeoutTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                //IList<IJob> jobs = commandContext.JobManager.CreateJobQuery()
                //    .ToList();
                //foreach (IJob job in jobs)
                //{
                //    commandContext.JobManager.DeleteJob((JobEntity)job, false);
                //}

                //foreach (IHistoricJobLog jobLog in commandContext.DbEntityManager.CreateHistoricJobLogQuery().ToList())
                //{
                //    commandContext.HistoricJobLogManager.DeleteHistoricJobLogById(jobLog.Id);
                //}

                return null;
            }

        }

        internal class UpdateJobCommand : ControllableCommand<object>
        {

            protected internal string lockOwner;

            public UpdateJobCommand(string lockOwner)
            {
                this.lockOwner = lockOwner;
            }


            public override object Execute(CommandContext commandContext)
            {
                throw new System.NotImplementedException();
                //DbEntityManagerFactory dbEntityManagerFactory = new DbEntityManagerFactory(Context.ProcessEngineConfiguration.IdGenerator);

                ////Todo: DbEntityManagerFactory.OpenSession()
                ////DbEntityManager entityManager = dbEntityManagerFactory.openSession();
                //DbEntityManager entityManager = null;

                //JobEntity job = entityManager.SelectById<JobEntity>(typeof(JobEntity), JOB_ENTITY_ID);
                //job.LockOwner = lockOwner;
                //entityManager.ForceUpdate(job);

                //Monitor.Sync();

                //// flush the changed entity and create a lock for the table
                //entityManager.Flush();

                //Monitor.Sync();


                //// commit transaction and remove the lock
                //commandContext.TransactionContext.Commit();

                //return null;
            }

        }
    }

}