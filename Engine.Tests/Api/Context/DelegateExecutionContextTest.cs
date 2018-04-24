using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.context;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Model.Bpmn;
using NUnit.Framework;

namespace Engine.Tests.Api.Context
{

    /// <summary>
    /// Represents test class to test the delegate execution context.
    /// 
    /// 
    /// </summary>
    public class DelegateExecutionContextTest
    {
        private bool InstanceFieldsInitialized = false;

        public DelegateExecutionContextTest()
        {
            if (!InstanceFieldsInitialized)
            {
                InitializeInstanceFields();
                InstanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            testHelper = new ProcessEngineTestRule(engineRule);
            //////ruleChain = RuleChain.outerRule(engineRule).around(testHelper);
        }


        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        protected internal static readonly IBpmnModelInstance DELEGATION_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process1").StartEvent().ServiceTask("serviceTask1").CamundaClass(typeof(DelegateClass).FullName).EndEvent().Done();


        //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
        protected internal static readonly IBpmnModelInstance EXEUCTION_LISTENER_PROCESS = ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("process2").StartEvent().CamundaExecutionListenerClass(ExecutionListenerFields.EventNameStart, typeof(ExecutionListenerImpl).FullName).EndEvent().Done();

        protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        protected internal ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.RuleChain ruleChain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        ////public RuleChain ruleChain;

        [Test]
        public virtual void testDelegateExecutionContext()
        {
            // given
            IProcessDefinition definition = testHelper.DeployAndGetDefinition(DELEGATION_PROCESS);
            // a process instance with a service task and a java delegate
            IProcessInstance instance = engineRule.RuntimeService.StartProcessInstanceById(definition.Id);

            //then delegation execution context is no more available
            IDelegateExecution execution = DelegateExecutionContext.CurrentDelegationExecution;
            Assert.IsNull(execution);
        }


        [Test]
        public virtual void testDelegateExecutionContextWithExecutionListener()
        {
            //given
            IProcessDefinition definition = testHelper.DeployAndGetDefinition(EXEUCTION_LISTENER_PROCESS);
            // a process instance with a service task and an execution listener
            engineRule.RuntimeService.StartProcessInstanceById(definition.Id);

            //then delegation execution context is no more available
            IDelegateExecution execution = DelegateExecutionContext.CurrentDelegationExecution;
            Assert.IsNull(execution);
        }

        public class ExecutionListenerImpl : IDelegateListener<IBaseDelegateExecution>
        {
            public void Notify(IBaseDelegateExecution execution)
            {
                checkDelegationContext(execution as IDelegateExecution);
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void Notify(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            //public override void Notify(IDelegateExecution execution)
            //{
            //    checkDelegationContext(execution);
            //}
        }

        public class DelegateClass : IJavaDelegate
        {

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: @Override public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
            public void Execute(IBaseDelegateExecution execution)
            {
                checkDelegationContext(execution as IDelegateExecution);
            }
        }

        protected internal static void checkDelegationContext(IDelegateExecution execution)
        {
            //then delegation execution context is available
            Assert.NotNull(DelegateExecutionContext.CurrentDelegationExecution);
            Assert.AreEqual(DelegateExecutionContext.CurrentDelegationExecution, execution);
        }
    }

}