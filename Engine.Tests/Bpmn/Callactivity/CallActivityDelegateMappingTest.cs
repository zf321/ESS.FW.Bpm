using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Task;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Callactivity
{
    [TestFixture]
    public class CallActivityDelegateMappingTest
    {
        private bool InstanceFieldsInitialized = false;

        public CallActivityDelegateMappingTest()
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
            //chain = RuleChain.outerRule(engineRule).around(testHelper);
        }


        public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
        public ProcessEngineTestRule testHelper;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.junit.Rules.Rule    bn Chain chain = org.junit.Rules.RuleChain.outerRule(engineRule).around(testHelper);
        //public RuleChain chain;


        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMapping.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMapping()
        {
            //given
            engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery();

            //when
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("ITask in subprocess", taskInSubProcess.Name);

            //then check value from input variable
            object inputVar = engineRule.RuntimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "TestInputVar");
            Assert.AreEqual("inValue", inputVar);

            //when completing the task in the subprocess, finishes the subprocess
            engineRule.TaskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("ITask after subprocess", taskAfterSubProcess.Name);

            //then check value from output variable
            IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery().First();
            object outputVar = engineRule.RuntimeService.GetVariable(processInstance.Id, "TestOutputVar");
            Assert.AreEqual("outValue", outputVar);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpression.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpression()
        {
            //given

            IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
            vars["expr"] = new DelegatedVarMapping();
            engineRule.ProcessEngineConfiguration.Beans = vars;
            engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery();

            //when
            ITask taskInSubProcess = taskQuery.First();
            Assert.AreEqual("ITask in subprocess", taskInSubProcess.Name);

            //then check if variable mapping was executed - check if input variable exist
            object inputVar = engineRule.RuntimeService.GetVariable(taskInSubProcess.ProcessInstanceId, "TestInputVar");
            Assert.AreEqual("inValue", inputVar);

            //when completing the task in the subprocess, finishes the subprocess
            engineRule.TaskService.Complete(taskInSubProcess.Id);
            ITask taskAfterSubProcess = taskQuery.First();
            Assert.AreEqual("ITask after subprocess", taskAfterSubProcess.Name);

            //then check if variable output mapping was executed - check if output variable exist
            IProcessInstance processInstance = engineRule.RuntimeService.CreateProcessInstanceQuery().First();
            object outputVar = engineRule.RuntimeService.GetVariable(processInstance.Id, "TestOutputVar");
            Assert.AreEqual("outValue", outputVar);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingNotFound.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingNotFound()
        {
            try
            {
                engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
                Assert.Fail("Execption expected!");
            }
            catch (ProcessEngineException e)
            {
                //Exception while instantiating class 'test.bpmn.Callactivity.NotFoundMapping'
                Assert.AreEqual("ENGINE-09008 Exception while instantiating class 'test.bpmn.Callactivity.NotFoundMapping': ENGINE-09017 Cannot load class 'test.bpmn.Callactivity.NotFoundMapping': test.bpmn.Callactivity.NotFoundMapping", e.Message);
            }
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpressionNotFound.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionNotFound()
        {
            try
            {
                engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
                Assert.Fail("Exception expected!");
            }
            catch (ProcessEngineException pee)
            {
                Assert.AreEqual("Unknown property used in expression: ${notFound}. Cause: Cannot resolve identifier 'notFound'", pee.Message);
            }
        }

        private void delegateVariableMappingThrowException()
        {
            //given
            engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("ITask before subprocess", taskBeforeSubProcess.Name);

            //when completing the task continues the process which leads to calling the subprocess
            //which throws an exception
            try
            {
                engineRule.TaskService.Complete(taskBeforeSubProcess.Id);
                Assert.Fail("Exeption expected!");
            }
            catch (ProcessEngineException pee)
            {
                Assert.True(pee.Message.Equals("ProcessEngineException: New process engine exception.", StringComparison.OrdinalIgnoreCase) || pee.Message.Contains("1234"));
            }

            //then process rollback to IUser task which is before sub process
            //not catched by boundary event
            taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("ITask before subprocess", taskBeforeSubProcess.Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingThrowException.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingThrowException()
        {
            delegateVariableMappingThrowException();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowException()
        {
            //given
            IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
            vars["expr"] = new DelegateVarMappingThrowException();
            engineRule.ProcessEngineConfiguration.Beans = vars;
            delegateVariableMappingThrowException();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingThrowBpmnError.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingThrowBpmnError()
        {
            delegateVariableMappingThrowException();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnError()
        {
            //given
            IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
            vars["expr"] = new DelegateVarMappingThrowBpmnError();
            engineRule.ProcessEngineConfiguration.Beans = vars;
            delegateVariableMappingThrowException();
        }

        private void delegateVariableMappingThrowExceptionOutput()
        {
            //given
            engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");
            IQueryable<ITask> taskQuery = engineRule.TaskService.CreateTaskQuery();
            ITask taskBeforeSubProcess = taskQuery.First();
            Assert.AreEqual("ITask before subprocess", taskBeforeSubProcess.Name);
            engineRule.TaskService.Complete(taskBeforeSubProcess.Id);
            ITask taskInSubProcess = taskQuery.First();

            //when completing the task continues the process which leads to calling the output mapping
            //which throws an exception
            try
            {
                engineRule.TaskService.Complete(taskInSubProcess.Id);
                Assert.Fail("Exeption expected!");
            }
            catch (ProcessEngineException pee)
            { 
                Assert.True(pee.Message.Equals("ProcessEngineException: New process engine exception.", StringComparison.OrdinalIgnoreCase) || pee.Message.Contains("1234"));                
            }

            //then process rollback to IUser task which is in sub process
            //not catched by boundary event
            taskInSubProcess = taskQuery.First();
            Assert.AreEqual("ITask in subprocess", taskInSubProcess.Name);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingThrowExceptionOutput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingThrowExceptionOutput()
        {
            delegateVariableMappingThrowExceptionOutput();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowExceptionOutput()
        {
            //given
            IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
            vars["expr"] = new DelegateVarMappingThrowExceptionOutput();
            engineRule.ProcessEngineConfiguration.Beans = vars;
            delegateVariableMappingThrowExceptionOutput();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingThrowBpmnErrorOutput.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingThrowBpmnErrorOutput()
        {
            delegateVariableMappingThrowExceptionOutput();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSimpleSubProcessDelegateVarMappingExpressionThrowException.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcess.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingeExpressionThrowBpmnErrorOutput()
        {
            //given
            IDictionary<object, object> vars = engineRule.ProcessEngineConfiguration.Beans;
            vars["expr"] = new DelegateVarMappingThrowBpmnErrorOutput();
            engineRule.ProcessEngineConfiguration.Beans = vars;
            delegateVariableMappingThrowExceptionOutput();
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallFailingSubProcessWithDelegatedVariableMapping.bpmn20.xml", "resources/bpmn/callactivity/failingSubProcess.bpmn20.xml" })]
        public virtual void testCallFailingSubProcessWithDelegatedVariableMapping()
        {
            //given starting process instance with call activity
            //when call activity execution fails
            IProcessInstance procInst = engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            //then output mapping should be executed
            object outputVar = engineRule.RuntimeService.GetVariable(procInst.Id, "TestOutputVar");
            Assert.AreEqual("outValue", outputVar);
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/callactivity/CallActivityDelegateMappingTest.TestCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask.bpmn20.xml", "resources/bpmn/callactivity/simpleSubProcessWithAsyncService.bpmn20.xml" })]
        public virtual void testCallSubProcessWithDelegatedVariableMappingAndAsyncServiceTask()
        {
            //given starting process instance with call activity which has asyn service task
            IProcessInstance superProcInst = engineRule.RuntimeService.StartProcessInstanceByKey("callSimpleSubProcess");

            IProcessInstance subProcInst = engineRule.RuntimeService.CreateProcessInstanceQuery(c=>c.ProcessInstanceId == "simpleSubProcessWithAsyncService").First();

            //then delegation variable mapping class should also been resolved
            //input mapping should be executed
            object inVar = engineRule.RuntimeService.GetVariable(subProcInst.Id, "TestInputVar");
            Assert.AreEqual("inValue", inVar);

            //and after finish call activity the ouput mapping is executed
            testHelper.ExecuteAvailableJobs();

            object outputVar = engineRule.RuntimeService.GetVariable(superProcInst.Id, "TestOutputVar");
            Assert.AreEqual("outValue", outputVar);
        }

    }

}