using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Api.Delegate
{
    /// <summary>
    ///     Tests for the execution hierarchy methods exposed in delegate execution
    /// </summary>
    public class DelegateExecutionHierarchyTest : PluggableProcessEngineTestCase
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void tearDown() throws Exception
        protected internal void tearDown()
        {
            AssertingJavaDelegate.Clear();
            TearDown();
        }

        public virtual void testSingleNonScopeActivity()
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .ServiceTask()
                .CamundaClass(typeof(AssertingJavaDelegate).FullName)
                .EndEvent()
                .Done());

            AssertingJavaDelegate.AddAsserts(new DelegateExecutionAsserterAnonymousInnerClass(this));

            runtimeService.StartProcessInstanceByKey("testProcess");
        }

        private class DelegateExecutionAsserterAnonymousInnerClass : AssertingJavaDelegate.DelegateExecutionAsserter
        {
            private readonly DelegateExecutionHierarchyTest outerInstance;

            public DelegateExecutionAsserterAnonymousInnerClass(DelegateExecutionHierarchyTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void DoAssert(IDelegateExecution execution)
            {
                Assert.AreEqual(execution, execution.ProcessInstance);
                Assert.IsNull(execution.SuperExecution);
            }
        }

        public virtual void testConcurrentServiceTasks()
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .ParallelGateway("fork")
                .ServiceTask()
                .CamundaClass(typeof(AssertingJavaDelegate).FullName)
                .ParallelGateway("join")
                .EndEvent()
                //.MoveToNode("fork")
                .ServiceTask()
                .CamundaClass(typeof(AssertingJavaDelegate).FullName)
                //.connectTo("join")
                .Done());

            AssertingJavaDelegate.AddAsserts(new DelegateExecutionAsserterAnonymousInnerClass2(this));

            runtimeService.StartProcessInstanceByKey("testProcess");
        }

        private class DelegateExecutionAsserterAnonymousInnerClass2 : AssertingJavaDelegate.DelegateExecutionAsserter
        {
            private readonly DelegateExecutionHierarchyTest outerInstance;

            public DelegateExecutionAsserterAnonymousInnerClass2(DelegateExecutionHierarchyTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void DoAssert(IDelegateExecution execution)
            {
                Assert.IsFalse(execution.Equals(execution.ProcessInstance));
                Assert.IsNull(execution.SuperExecution);
            }
        }

        public virtual void testTaskInsideEmbeddedSubprocess()
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            Deployment(ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                //.SubProcess()
                //.EmbeddedSubProcess()
                //.StartEvent()
                //.ServiceTask()
                //.CamundaClass(typeof(AssertingJavaDelegate).FullName)
                //.EndEvent()
                //.SubProcessDone()
                //.EndEvent()
                .Done()
                );

            AssertingJavaDelegate.AddAsserts(new DelegateExecutionAsserterAnonymousInnerClass3(this));

            runtimeService.StartProcessInstanceByKey("testProcess");
        }

        private class DelegateExecutionAsserterAnonymousInnerClass3 : AssertingJavaDelegate.DelegateExecutionAsserter
        {
            private readonly DelegateExecutionHierarchyTest outerInstance;

            public DelegateExecutionAsserterAnonymousInnerClass3(DelegateExecutionHierarchyTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void DoAssert(IDelegateExecution execution)
            {
                Assert.IsFalse(execution.Equals(execution.ProcessInstance));
                Assert.IsNull(execution.SuperExecution);
            }
        }

        public virtual void testSubProcessInstance()
        {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            Deployment(
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess")
                .StartEvent()
                .CallActivity()
                .CalledElement("testProcess2")
                .EndEvent()
                .Done(), 
                ESS.FW.Bpm.Model.Bpmn.Bpmn.CreateExecutableProcess("testProcess2")
                .StartEvent()
                .ServiceTask()
                .CamundaClass(typeof(AssertingJavaDelegate).FullName)
                .EndEvent()
                .Done());

            AssertingJavaDelegate.AddAsserts(new DelegateExecutionAsserterAnonymousInnerClass4(this));

            runtimeService.StartProcessInstanceByKey("testProcess");
        }

        private class DelegateExecutionAsserterAnonymousInnerClass4 : AssertingJavaDelegate.DelegateExecutionAsserter
        {
            private readonly DelegateExecutionHierarchyTest outerInstance;

            public DelegateExecutionAsserterAnonymousInnerClass4(DelegateExecutionHierarchyTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public virtual void DoAssert(IDelegateExecution execution)
            {
                Assert.True(execution.Equals(execution.ProcessInstance));
                Assert.NotNull(execution.SuperExecution);
            }
        }
    }
}