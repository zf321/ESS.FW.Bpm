//using System.Diagnostics;
//using System.Threading;
//using ESS.FW.Bpm.Engine.Runtime;
//using NUnit.Framework;


//namespace ESS.FW.Bpm.Engine.Tests.ConCurrency
//{

//    //Todo: org.camunda.bpm.engine.impl.cmmn.cmd.StateTransitionCaseExecutionCmd & CompleteCaseExecutionCmd & ManualStartCaseExecutionCmd
//    public class CompetingSentrySatisfactionTest : PluggableProcessEngineTestCase
//    {

//        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

//        internal Thread testThread = Thread.CurrentThread;
//        internal static ControllableThread activeThread;

//        public abstract class SingleThread : ControllableThread
//        {
//            private readonly CompetingSentrySatisfactionTest outerInstance;


//            internal string caseExecutionId;
//            internal OptimisticLockingException exception;
//            protected internal StateTransitionCaseExecutionCmd cmd;

//            public SingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId, StateTransitionCaseExecutionCmd cmd)
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
//            private readonly CompetingSentrySatisfactionTest outerInstance;


//            public CompletionSingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new CompleteCaseExecutionCmd(caseExecutionId, null, null, null, null))
//            {
//                this.outerInstance = outerInstance;
//            }

//        }

//        public class ManualStartSingleThread : SingleThread
//        {
//            private readonly CompetingSentrySatisfactionTest outerInstance;


//            public ManualStartSingleThread(CompetingSentrySatisfactionTest outerInstance, string caseExecutionId) : base(outerInstance, caseExecutionId, new ManualStartCaseExecutionCmd(caseExecutionId, null, null, null, null))
//            {
//                this.outerInstance = outerInstance;
//            }

//        }

//        [Deployment(new string[] {"resources/concurrency/CompetingSentrySatisfactionTest.TestEntryCriteriaWithAndSentry.cmmn"})]
//        public virtual void testEntryCriteriaWithAndSentry()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);

//            string message = threadTwo.exception.Message;
//            AssertTextPresent("CaseSentryPartEntity", message);
//            AssertTextPresent("was updated by another transaction concurrently", message);
//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingSentrySatisfactionTest.TestExitCriteriaWithAndSentry.cmmn" })]
//        public virtual void testExitCriteriaWithAndSentry()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);

//            string message = threadTwo.exception.Message;
//            AssertTextPresent("CaseSentryPartEntity", message);
//            AssertTextPresent("was updated by another transaction concurrently", message);
//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingSentrySatisfactionTest.TestEntryCriteriaWithOrSentry.cmmn" })]
//        public virtual void testEntryCriteriaWithOrSentry()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            Debug.WriteLine("test thread starts thread one");
//            SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);

//            string message = threadTwo.exception.Message;
//            AssertTextPresent("CaseExecutionEntity", message);
//            AssertTextPresent("was updated by another transaction concurrently", message);
//        }

//        [Deployment(new string[] { "resources/concurrency/CompetingSentrySatisfactionTest.TestExitCriteriaWithOrSentry.cmmn", "resources/concurrency/CompetingSentrySatisfactionTest.oneTaskProcess.bpmn20.xml" })]
//        public virtual void testExitCriteriaWithOrSentry()
//        {
//            string caseInstanceId = caseService.WithCaseDefinitionByKey("case").Create().Id;

//            string firstHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_1").First().Id;

//            string secondHumanTaskId = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="PI_HumanTask_2").First().Id;

//            ICaseExecution thirdTask = caseService.CreateCaseExecutionQuery(c=>c.CaseInstanceId ==caseInstanceId&& c.ActivityId =="ProcessTask_3").First();
//            caseService.ManuallyStartCaseExecution(thirdTask.Id);

//            Debug.WriteLine("test thread starts thread one");
//            SingleThread threadOne = new ManualStartSingleThread(this, firstHumanTaskId);
//            threadOne.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread continues to start thread two");
//            SingleThread threadTwo = new CompletionSingleThread(this, secondHumanTaskId);
//            threadTwo.startAndWaitUntilControlIsReturned();

//            Debug.WriteLine("test thread notifies thread 1");
//            threadOne.proceedAndWaitTillDone();
//            Assert.IsNull(threadOne.exception);

//            Debug.WriteLine("test thread notifies thread 2");
//            threadTwo.proceedAndWaitTillDone();
//            Assert.NotNull(threadTwo.exception);

//            string message = threadTwo.exception.Message;
//            AssertTextPresent("CaseExecutionEntity", message);
//            AssertTextPresent("was updated by another transaction concurrently", message);
//        }

//    }

//}