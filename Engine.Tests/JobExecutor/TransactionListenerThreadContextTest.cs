using NUnit.Framework;

namespace Engine.Tests.JobExecutor
{
    /// <summary>
    ///     This test makes sure that if the transaction synchronization / transaction listener ExclusiveJobAddedNotification
    ///     is
    ///     executed in a different thread than the Thread which executed the job, the notification still works.
    /// </summary>
    [TestFixture]
    public class TransactionListenerThreadContextTest : ResourceProcessEngineTestCase
    {
        public TransactionListenerThreadContextTest()
            : base("resources/jobexecutor/TransactionListenerThreadContextTest.cfg.xml")
        {
        }

        [Test]
        public virtual void TestTxListenersInvokeAsync()
        {
            var process =
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                    .StartEvent()
                    .CamundaAsyncBefore()
                    .CamundaAsyncAfter()
                    .EndEvent()
                    .Done();

            var deployment = repositoryService.CreateDeployment().AddModelInstance("testProcess.bpmn", process).Deploy();

            var pi = runtimeService.StartProcessInstanceByKey("testProcess");

            WaitForJobExecutorToProcessAllJobs(6000);


            AssertProcessEnded(pi.Id);

            repositoryService.DeleteDeployment(deployment.Id, true);
        }
    }
}