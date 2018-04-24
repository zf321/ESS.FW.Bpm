using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Repository.Impl;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{

    [TestFixture]
    public class CompetingSuspensionTest : PluggableProcessEngineTestCase
    {

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        internal static ControllableThread activeThread;

        internal class SuspendProcessDefinitionThread : ControllableThread
        {
            private readonly CompetingSuspensionTest outerInstance;


            internal string processDefinitionId;
            internal OptimisticLockingException exception;

            public SuspendProcessDefinitionThread(CompetingSuspensionTest outerInstance, string processDefinitionId)
            {
                this.outerInstance = outerInstance;
                this.processDefinitionId = processDefinitionId;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }
            // Todo: ControllableThread.run()
            //public override void run()
            //{
            //  try
            //  {
            //                  outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, createSuspendCommand()));

            //  }
            //  catch (OptimisticLockingException e)
            //  {
            //	this.exception = e;
            //  }
            //  Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}

            protected internal virtual SuspendProcessDefinitionCmd CreateSuspendCommand()
            {
                /*UpdateProcessDefinitionSuspensionStateBuilderImpl builder*/
                IUpdateProcessDefinitionSuspensionStateBuilder builder = new UpdateProcessDefinitionSuspensionStateBuilderImpl().ByProcessDefinitionId(processDefinitionId);//.IncludeProcessInstances(true);

                return new SuspendProcessDefinitionCmd(builder as UpdateProcessDefinitionSuspensionStateBuilderImpl);
            }
        }

        internal class SignalThread : ControllableThread
        {
            private readonly CompetingSuspensionTest outerInstance;


            internal string executionId;
            internal OptimisticLockingException exception;

            public SignalThread(CompetingSuspensionTest outerInstance, string executionId)
            {
                this.outerInstance = outerInstance;
                this.executionId = executionId;
            }

            public override void startAndWaitUntilControlIsReturned()
            {
                lock (this)
                {
                    activeThread = this;
                    base.startAndWaitUntilControlIsReturned();
                }
            }

            // Todo: ControllableThread.run()
            //public override void run()
            //{
            //    try
            //    {
            //        outerInstance.processEngineConfiguration.CommandExecutorTxRequired.Execute(new ControlledCommand<object>(activeThread, new SignalCmd(executionId, null, null, null)));

            //    }
            //    catch (OptimisticLockingException e)
            //    {
            //        this.exception = e;
            //    }
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " ends");
            //}
        }

        [Deployment]
        public virtual void testCompetingSuspension()
        {
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery(c=>c.Key == "CompetingSuspensionProcess").First();

            IProcessInstance processInstance = runtimeService.StartProcessInstanceById(processDefinition.Id);
            IExecution execution = runtimeService.CreateExecutionQuery(c=>c.ProcessInstanceId ==processInstance.Id&& c.ActivityId =="wait1").First();

            SuspendProcessDefinitionThread suspensionThread = new SuspendProcessDefinitionThread(this, processDefinition.Id);
            suspensionThread.startAndWaitUntilControlIsReturned();

            SignalThread signalExecutionThread = new SignalThread(this, execution.Id);
            signalExecutionThread.startAndWaitUntilControlIsReturned();

            suspensionThread.proceedAndWaitTillDone();
            Assert.IsNull(suspensionThread.exception);

            signalExecutionThread.proceedAndWaitTillDone();
            Assert.NotNull(signalExecutionThread.exception);
        }
    }

}