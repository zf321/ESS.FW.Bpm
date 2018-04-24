using System;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    [TestFixture]
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)]
    public class DbDeadlockTest : ConcurrencyTestCase
    {

        private ThreadControl thread1;
        private ThreadControl thread2;

        /// <summary>
        /// In this test, we run two transactions concurrently.
        /// The transactions have the following behavior:
        /// 
        /// (1) INSERT row into a table
        /// (2) SELECT ALL rows from that table
        /// 
        /// We execute it with two threads in the following interleaving:
        /// 
        ///      Thread 1             Thread 2
        ///      ========             ========
        /// ------INSERT---------------------------   |
        /// ---------------------------INSERT------   |
        /// ---------------------------SELECT------   v time
        /// ------SELECT---------------------------
        /// 
        /// Deadlocks may occur if readers are not properly isolated from writers.
        /// 
        /// </summary>
        public virtual void testTransactionIsolation()
        {

            thread1 = ExecuteControllableCommand(new TestCommand("p1"));

            // wait for Thread 1 to perform INSERT
            thread1.WaitForSync();

            thread2 = ExecuteControllableCommand(new TestCommand("p2"));

            // wait for Thread 2 to perform INSERT
            thread2.WaitForSync();

            // wait for Thread 2 to perform SELECT
            thread2.MakeContinue();

            // wait for Thread 1  to perform same SELECT => deadlock
            thread1.MakeContinue();

            thread2.WaitForSync();
            thread1.WaitForSync();

        }

        internal class TestCommand : ControllableCommand<object>
        {

            protected internal string id;

            public TestCommand(string id)
            {
                this.id = id;
            }

            // Todo: DbEntityManagerFactory.OpenSession()
            public override object Execute(CommandContext commandContext)
            {
                throw new NotImplementedException();
                //DbEntityManagerFactory dbEntityManagerFactory = new DbEntityManagerFactory(Context.ProcessEngineConfiguration.IdGenerator);
                ////DbEntityManager newEntityManager = dbEntityManagerFactory.OpenSession();
                //DbEntityManager newEntityManager = null;

                //HistoricProcessInstanceEventEntity hpi = new HistoricProcessInstanceEventEntity();
                //hpi.Id = id;
                //hpi.ProcessInstanceId = id;
                //hpi.ProcessDefinitionId = "someProcDefId";
                //hpi.StartTime = DateTime.Now;
                //hpi.State = HistoricProcessInstanceFields.StateActive;

                //newEntityManager.Insert(hpi);
                //newEntityManager.Flush();

                //Monitor.Sync();

                //DbEntityManager cmdEntityManager = commandContext.DbEntityManager;
                ////cmdEntityManager.CreateHistoricProcessInstanceQuery().ToList();

                //Monitor.Sync();

                //return null;
            }

        }

        [TearDown]
        protected internal void tearDown()
        {

            // end interaction with Thread 2
            thread2.WaitUntilDone();

            // end interaction with Thread 1
            thread1.WaitUntilDone();

            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));
        }

        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly DbDeadlockTest outerInstance;

            public CommandAnonymousInnerClass(DbDeadlockTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }


            public virtual object Execute(CommandContext commandContext)
            {
                //IList<IHistoricProcessInstance> list = commandContext.DbEntityManager.CreateHistoricProcessInstanceQuery().ToList();
                //foreach (IHistoricProcessInstance historicProcessInstance in list)
                //{
                //    commandContext.DbEntityManager.Delete(typeof(HistoricProcessInstanceEventEntity), "DeleteHistoricProcessInstance", historicProcessInstance.Id);
                //}
                return null;
            }

        }

    }

}