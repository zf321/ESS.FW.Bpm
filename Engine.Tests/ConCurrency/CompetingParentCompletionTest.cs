//using System.Diagnostics;
//using System.Threading;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.ConCurrency
//{

//    //Todo: org.camunda.bpm.engine.impl.cmmn.cmd.StateTransitionCaseExecutionCmd & CompleteCaseExecutionCmd & DisableCaseExecutionCmd
//    public class CompetingParentCompletionTest : PluggableProcessEngineTestCase
//    {

//        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

//        internal Thread testThread = Thread.CurrentThread;
//        internal static ControllableThread activeThread;

//        public abstract class SingleThread : ControllableThread
//        {
//            private readonly CompetingParentCompletionTest outerInstance;


//            internal string caseExecutionId;
//            internal OptimisticLockingException exception;
//            protected internal StateTransitionCaseExecutionCmd cmd;

//            public SingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId, StateTransitionCaseExecutionCmd cmd)
//            {
//                this.outerInstance = outerInstance;
//                this.caseExecutionId = caseExecutionId;
//                this.cmd = cmd;
//            }

//            public override void startAndWaitUntilControlIsReturned()
//            {
//                lock (this)
//                {
//                    activeThread = this;
//                    base.startAndWaitUntilControlIsReturned();
//                }
//            }

//            public virtual void run()
//            {
//                try
//                {
//                    outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, cmd));

//                }
//                catch (OptimisticLockingException e)
//                {
//                    this.exception = e;
//                }
//                Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
//            }
//        }

//        public class CompletionSingleThread : SingleThread
//        {
//            private readonly CompetingParentCompletionTest outerInstance;


//            public CompletionSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new CompleteCaseExecutionCmd(caseExecutionId, null, null, null, null))
//            {
//                this.outerInstance = outerInstance;
//            }

//        }

//        public class DisableSingleThread : SingleThread
//        {
//            private readonly CompetingParentCompletionTest outerInstance;


//            public DisableSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new DisableCaseExecutionCmd(caseExecutionId, null, null, null, null))
//            {
//                this.outerInstance = outerInstance;
//            }

//        }

//        public class TerminateSingleThread : SingleThread
//        {
//            private readonly CompetingParentCompletionTest outerInstance;


//            public TerminateSingleThread(CompetingParentCompletionTest outerInstance, string caseExecutionId) : base(outerInstance,caseExecutionId, new StateTransitionCaseExecutionCmd(caseExecutionId, null, null, null, null))
//            {
//                this.outerInstance = outerInstance;
//            }
//            //{

//            //private static final long serialVersionUID = 1L;

//            //protected void performStateTransition(CommandContext commandContext, CaseExecutionEntity caseExecution)
//            //{
//            //  caseExecution.Terminate();
//            //}

//            //}
//            //);
//            //}

//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingParentCompletionTest.TestComplete.cmmn" })]
//        public virtual void testComplete()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            CompetingParentCompletionTest.SingleThread threadOne = new CompetingParentCompletionTest.CompletionSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            CompetingParentCompletionTest.SingleThread threadTwo = new CompetingParentCompletionTest.CompletionSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);
//            AssertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingParentCompletionTest.TestDisable.cmmn" })]
//        public virtual void testDisable()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            CompetingParentCompletionTest.SingleThread threadOne = new CompetingParentCompletionTest.DisableSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            CompetingParentCompletionTest.SingleThread threadTwo = new CompetingParentCompletionTest.DisableSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);
//            AssertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingParentCompletionTest.TestTerminate.cmmn" })]
//        public virtual void testTerminate()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            CompetingParentCompletionTest.SingleThread threadOne = new CompetingParentCompletionTest.TerminateSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            CompetingParentCompletionTest.SingleThread threadTwo = new CompetingParentCompletionTest.TerminateSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);
//            AssertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

//        }

//    }

//}