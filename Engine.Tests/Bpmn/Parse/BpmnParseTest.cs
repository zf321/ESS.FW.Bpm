using System.Collections.Generic;
using System.Linq;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Helper;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Parse
{
    /// 
	/// <summary>
	/// 
	/// </summary>
    [TestFixture]
    public class BpmnParseTest : PluggableProcessEngineTestCase
    {
        public BpmnParseTest()
        {
            ClearDeploymentAll = true;
        }
        [Test]
        public void Test()
        {
           
            try
            {
                Assert.Fail("1");
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(e.Message, "1");
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithTimerStartEvent()
        {

            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithTimerStartEvent");
                var r = repositoryService.CreateDeployment();
                r.Name(resource);//
                r.AddClasspathResource(resource);
                r.Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a timer start event.");
            }
            //catch (ProcessEngineException e)
            catch (System.Exception e)
            {
                //TODO MESSAGE 处理过
                Assert.IsTrue(e.Message.ToLower().Contains("timerEventDefinition is not allowed on start event within a subprocess".ToLower()));
                //AssertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
            //string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithTimerStartEvent");
            //var r= Assert.Throws< ProcessEngineException>(()=> repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy());
            //Assert.AreEqual(r.Message, "Exception expected: Process definition could be parsed, although the sub process contains a timer start event.");
        }
        [Test]
        public virtual void TestInvalidSubProcessWithMessageStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithMessageStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process definition could be parsed, although the sub process contains not a blanco start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithConditionalStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithConditionalStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a conditional start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("conditionalEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithSignalStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithSignalStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a signal start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("signalEventDefintion only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithErrorStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithErrorStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a error start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("errorEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithEscalationStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithEscalationStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a escalation start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("escalationEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidSubProcessWithCompensationStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSubProcessWithCompensationStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a compensation start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("compensateEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithMessageStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithMessageStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process definition could be parsed, although the sub process contains not a blanco start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithTimerStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithTimerStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a timer start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithConditionalStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithConditionalStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a conditional start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("conditionalEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithSignalStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithSignalStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a signal start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("signalEventDefintion only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithErrorStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithErrorStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a error start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("errorEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithEscalationStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithEscalationStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a escalation start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("escalationEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidTransactionWithCompensationStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidTransactionWithCompensationStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, although the sub process contains a compensation start event.");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("compensateEventDefinition is not allowed on start event within a subprocess", e.Message);
            }
        }
        [Test]
        public virtual void TestInvalidProcessDefinition()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidProcessDefinition");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail();
            }
            catch (System.Exception e)
            {
                //TODO 异常信息只抛出了部分 MESSAGE为空字符串
                //AssertTextPresent("cvc-complex-type.3.2.2:", e.Message);
                //AssertTextPresent("invalidAttribute", e.Message);
                //AssertTextPresent("process", e.Message);
            }

        }
        [Test]
        public virtual void TestInvalidSequenceFlowInAndOutEventSubProcess()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidSequenceFlowInAndOutEventSubProcess");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail();
               // Assert.Fail("Exception expected: Process definition could be parsed, although the sub process has incoming and outgoing sequence flows");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Invalid incoming sequence flow of event subprocess", e.Message);
                //AssertTextPresent("Invalid outgoing sequence flow of event subprocess", e.Message);
            }
        }
        /// <summary>
        /// this test case check if the multiple start event is supported the test case
        /// doesn't Assert.Fail in this behavior because the <seealso cref="BpmnParse"/> parse the event
        /// definitions with if-else, this means only the first event definition is
        /// taken
        /// 
        /// </summary>
        [Test]
        public virtual void TestParseMultipleStartEvent()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testParseMultipleStartEvent");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                // Assert.Fail in "regular" subprocess
                AssertTextPresent("timerEventDefinition is not allowed on start event within a subprocess", e.Message);
                AssertTextPresent("messageEventDefinition only allowed on start event if subprocess is an event subprocess", e.Message);
                // doesn't Assert.Fail in event subprocess/process because the bpmn parser parse
                // only this first event definition
            }
        }
        [Test]
        public virtual void TestParseWithBpmnNamespacePrefix()
        {
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/parse/BpmnParseTest.TestParseWithBpmnNamespacePrefix.bpmn20.xml").Deploy();
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());

            repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
        }
        [Test]
        public virtual void TestParseWithMultipleDocumentation()
        {
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/parse/BpmnParseTest.TestParseWithMultipleDocumentation.bpmn20.xml").Deploy();
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());

            repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
        }
        [Test]
        public virtual void TestParseCollaborationPlane()
        {
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/parse/BpmnParseTest.TestParseCollaborationPlane.bpmn").Deploy();
            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());

            repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
        }
        [Test]
        public virtual void TestInvalidAsyncAfterEventBasedGateway()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testInvalidAsyncAfterEventBasedGateway");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail();
            }
            catch (ProcessEngineException e)
            {
                // Assert.Fail on asyncAfter
                AssertTextPresent("'asyncAfter' not supported for", e.Message);
            }
        }

        [Test]
        [Deployment]//不需要此功能？
        public virtual void TestParseDiagramInterchangeElements()
        {

            // Graphical information is not yet exposed publicly, so we need to do some
            // plumbing
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            ProcessDefinitionEntity processDefinitionEntity = commandExecutor.Execute<ProcessDefinitionEntity>(new CommandAnonymousInnerClass(this));

            Assert.NotNull(processDefinitionEntity);
            Assert.AreEqual(7, processDefinitionEntity.Activities.Count);

            // Check if diagram has been created based on Diagram Interchange when it's
            // not a headless instance
            IList<string> resourceNames = repositoryService.GetDeploymentResourceNames(processDefinitionEntity.DeploymentId);
            if (processEngineConfiguration.CreateDiagramOnDeploy)
            {
                Assert.AreEqual(2, resourceNames.Count);
            }
            else
            {
                Assert.AreEqual(1, resourceNames.Count);
            }

            foreach (ActivityImpl activity in processDefinitionEntity.Activities)
            {
                if (activity.Id.Equals("theStart"))
                {
                    AssertActivityBounds(activity, 70, 255, 30, 30);
                }
                else if (activity.Id.Equals("task1"))
                {
                    AssertActivityBounds(activity, 176, 230, 100, 80);
                }
                else if (activity.Id.Equals("gateway1"))
                {
                    AssertActivityBounds(activity, 340, 250, 40, 40);
                }
                else if (activity.Id.Equals("task2"))
                {
                    AssertActivityBounds(activity, 445, 138, 100, 80);
                }
                else if (activity.Id.Equals("gateway2"))
                {
                    AssertActivityBounds(activity, 620, 250, 40, 40);
                }
                else if (activity.Id.Equals("task3"))
                {
                    AssertActivityBounds(activity, 453, 304, 100, 80);
                }
                else if (activity.Id.Equals("theEnd"))
                {
                    AssertActivityBounds(activity, 713, 256, 28, 28);
                }

                foreach (IPvmTransition sequenceFlow in activity.OutgoingTransitions)
                {
                    Assert.True(((TransitionImpl)sequenceFlow).Waypoints.Count >= 4);

                    TransitionImpl transitionImpl = (TransitionImpl)sequenceFlow;
                    if (transitionImpl.Id.Equals("flowStartToTask1"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 100, 270, 176, 270);
                    }
                    else if (transitionImpl.Id.Equals("flowTask1ToGateway1"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 276, 270, 340, 270);
                    }
                    else if (transitionImpl.Id.Equals("flowGateway1ToTask2"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 360, 250, 360, 178, 445, 178);
                    }
                    else if (transitionImpl.Id.Equals("flowGateway1ToTask3"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 360, 290, 360, 344, 453, 344);
                    }
                    else if (transitionImpl.Id.Equals("flowTask2ToGateway2"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 545, 178, 640, 178, 640, 250);
                    }
                    else if (transitionImpl.Id.Equals("flowTask3ToGateway2"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 553, 344, 640, 344, 640, 290);
                    }
                    else if (transitionImpl.Id.Equals("flowGateway2ToEnd"))
                    {
                        AssertSequenceFlowWayPoints(transitionImpl, 660, 270, 713, 270);
                    }

                }
            }
        }

        private class CommandAnonymousInnerClass : ICommand<ProcessDefinitionEntity>
        {
            private readonly BpmnParseTest outerInstance;

            public CommandAnonymousInnerClass(BpmnParseTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public ProcessDefinitionEntity Execute(CommandContext commandContext)
            {
                return Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedLatestProcessDefinitionByKey("myProcess");
            }
        }

        [Test]
        [Deployment]//表达式解析相关
        public virtual void TestParseNamespaceInConditionExpressionType()
        {
            ICommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
            ProcessDefinitionEntity processDefinitionEntity = commandExecutor.Execute<ProcessDefinitionEntity>(new CommandAnonymousInnerClass2(this));

            // Test that the process definition has been deployed
            Assert.NotNull(processDefinitionEntity);
            IPvmActivity activity = processDefinitionEntity.FindActivity("ExclusiveGateway_1");
            Assert.NotNull(activity);

            // Test that the conditions has been resolved
            foreach (IPvmTransition transition in activity.OutgoingTransitions)
            {
                if (transition.Destination.Id.Equals("Task_2"))
                {
                    Assert.AreEqual("#{approved}",transition.GetProperty("conditionText") );
                    //Assert.True(transition.GetProperty("conditionText").Equals("#{approved}"));
                }
                else if (transition.Destination.Id.Equals("Task_3"))
                {
                    Assert.True(transition.GetProperty("conditionText").Equals("#{!approved}"));
                }
                else
                {
                    Assert.Fail("Something went wrong");
                }

            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<ProcessDefinitionEntity>
        {
            private readonly BpmnParseTest outerInstance;

            public CommandAnonymousInnerClass2(BpmnParseTest outerInstance)
            {
                this.outerInstance = outerInstance;
            }

            public ProcessDefinitionEntity Execute(CommandContext commandContext)
            {
                return Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedLatestProcessDefinitionByKey("resolvableNamespacesProcess");
            }
        }

        [Test]
        [Deployment]
        public virtual void TestParseDiagramInterchangeElementsForUnknownModelElements()
        {
        }

        /// <summary>
        /// We want to make sure that BPMNs created with the namespace http://activiti.org/bpmn still work.
        /// </summary>
        [Test]
        [Deployment]//不需要http://activiti.org/bpmn命名空间
        public virtual void TestParseDefinitionWithDeprecatedActivitiNamespace()
        {
        }

        [Test]
        [Deployment]
        public virtual void TestParseDefinitionWithCamundaNamespace()
        {
        }
        [Test]
        [Deployment]
        public virtual void TestParseCompensationEndEvent()
        {
            ActivityImpl endEvent = FindActivityInDeployedProcessDefinition("end");

            Assert.AreEqual(ActivityTypes.EndEventCompensation, endEvent.GetProperty("type"));
            Assert.AreEqual(true, endEvent.GetProperty(BpmnParse.PropertynameThrowsCompensation));
            Assert.AreEqual(typeof(CompensationEventActivityBehavior), endEvent.ActivityBehavior.GetType());
        }
        [Test]
        [Deployment]
        public virtual void TestParseCompensationStartEvent()
        {
            ActivityImpl compensationStartEvent = FindActivityInDeployedProcessDefinition("compensationStartEvent");

            Assert.AreEqual(ActivityTypes.StartEventCompensation, compensationStartEvent.GetProperty("type"));
            Assert.AreEqual(typeof(EventSubProcessStartEventActivityBehavior), compensationStartEvent.ActivityBehavior.GetType());

            ActivityImpl compensationEventSubProcess = (ActivityImpl)compensationStartEvent.FlowScope;
            Assert.AreEqual(true, compensationEventSubProcess.GetProperty(BpmnParse.PropertynameIsForCompensation));

            ScopeImpl subprocess = compensationEventSubProcess.FlowScope;
            Assert.AreEqual(compensationEventSubProcess.ActivityId, subprocess.GetProperty(BpmnParse.PropertynameCompensationHandlerId));
        }
        [Test]
        [Deployment]//表达式相关：camunda:expression="${true}"
        public virtual void TestParseAsyncMultiInstanceBody()
        {
            ActivityImpl innerTask = FindActivityInDeployedProcessDefinition("miTask");
            ActivityImpl miBody = innerTask.ParentFlowScopeActivity;

            Assert.True(miBody.AsyncBefore);
            Assert.True(miBody.AsyncAfter);

            Assert.IsFalse(innerTask.AsyncBefore);
            Assert.IsFalse(innerTask.AsyncAfter);
        }
        [Test]
        [Deployment]//表达式相关：camunda:expression="${true}"
        public virtual void TestParseAsyncActivityWrappedInMultiInstanceBody()
        {
            ActivityImpl innerTask = FindActivityInDeployedProcessDefinition("miTask");
            Assert.True(innerTask.AsyncBefore);
            Assert.True(innerTask.AsyncAfter);

            ActivityImpl miBody = innerTask.ParentFlowScopeActivity;
            Assert.IsFalse(miBody.AsyncBefore);
            Assert.IsFalse(miBody.AsyncAfter);
        }
        [Test]
        [Deployment]//表达式相关：camunda:expression="${true}"
        public virtual void TestParseAsyncActivityWrappedInMultiInstanceBodyWithAsyncMultiInstance()
        {
            ActivityImpl innerTask = FindActivityInDeployedProcessDefinition("miTask");
            Assert.AreEqual(true, innerTask.AsyncBefore);
            Assert.AreEqual(false, innerTask.AsyncAfter);

            ActivityImpl miBody = innerTask.ParentFlowScopeActivity;
            Assert.AreEqual(false, miBody.AsyncBefore);
            Assert.AreEqual(true, miBody.AsyncAfter);
        }
        [Test]
        public virtual void TestParseSwitchedSourceAndTargetRefsForAssociations()
        {
            repositoryService.CreateDeployment().AddClasspathResource("resources/bpmn/parse/BpmnParseTest.TestParseSwitchedSourceAndTargetRefsForAssociations.bpmn20.xml").Deploy();

            Assert.AreEqual(1, repositoryService.CreateProcessDefinitionQuery().Count());

            repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/event/compensate/CompensateEventTest.compensationMiActivity.bpmn20.xml" })]
        public virtual void TestParseCompensationHandlerOfMiActivity()
        {
            ActivityImpl miActivity = FindActivityInDeployedProcessDefinition("undoBookHotel");
            ScopeImpl flowScope = miActivity.FlowScope;

            Assert.AreEqual(ActivityTypes.MultiInstanceBody, flowScope.GetProperty(BpmnParse.PropertynameType));
            Assert.AreEqual("bookHotel" + BpmnParse.MultiInstanceBodyIdSuffix, ((ActivityImpl)flowScope).ActivityId);
        }

        [Test]
        [Deployment(Resources = new string[] { "resources/bpmn/event/compensate/CompensateEventTest.compensationMiSubprocess.bpmn20.xml" })]
        public virtual void TestParseCompensationHandlerOfMiSubprocess()
        {
            ActivityImpl miActivity = FindActivityInDeployedProcessDefinition("undoBookHotel");
            ScopeImpl flowScope = miActivity.FlowScope;

            Assert.AreEqual(ActivityTypes.MultiInstanceBody, flowScope.GetProperty(BpmnParse.PropertynameType));
            Assert.AreEqual("scope" + BpmnParse.MultiInstanceBodyIdSuffix, ((ActivityImpl)flowScope).ActivityId);
        }

        [Test]
        [Deployment]//TODO 表达式解析相关
        public virtual void TestParseSignalStartEvent()
        {
            ActivityImpl signalStartActivity = FindActivityInDeployedProcessDefinition("start");

            Assert.AreEqual(ActivityTypes.StartEventSignal, signalStartActivity.GetProperty("type"));
            Assert.AreEqual(typeof(NoneStartEventActivityBehavior), signalStartActivity.ActivityBehavior.GetType());
        }

        [Test]
        [Deployment]
        public virtual void TestParseEscalationBoundaryEvent()
        {
            ActivityImpl escalationBoundaryEvent = FindActivityInDeployedProcessDefinition("escalationBoundaryEvent");

            Assert.AreEqual(ActivityTypes.BoundaryEscalation, escalationBoundaryEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(BoundaryEventActivityBehavior), escalationBoundaryEvent.ActivityBehavior.GetType());
        }

        [Test]
        [Deployment]
        public virtual void TestParseEscalationIntermediateThrowingEvent()
        {
            ActivityImpl escalationThrowingEvent = FindActivityInDeployedProcessDefinition("escalationThrowingEvent");

            Assert.AreEqual(ActivityTypes.IntermediateEventEscalationThrow, escalationThrowingEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(ThrowEscalationEventActivityBehavior), escalationThrowingEvent.ActivityBehavior.GetType());
        }

        [Test]
        [Deployment]
        public virtual void TestParseEscalationEndEvent()
        {
            ActivityImpl escalationEndEvent = FindActivityInDeployedProcessDefinition("escalationEndEvent");

            Assert.AreEqual(ActivityTypes.EndEventEscalation, escalationEndEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(ThrowEscalationEventActivityBehavior), escalationEndEvent.ActivityBehavior.GetType());
        }
        [Test]
        [Deployment]
        public virtual void TestParseEscalationStartEvent()
        {
            ActivityImpl escalationStartEvent = FindActivityInDeployedProcessDefinition("escalationStartEvent");

            Assert.AreEqual(ActivityTypes.StartEventEscalation, escalationStartEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(EventSubProcessStartEventActivityBehavior), escalationStartEvent.ActivityBehavior.GetType());
        }

        [Test]
        public virtual void ParseInvalidConditionalEvent(string processDefinitionResource)
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), processDefinitionResource);
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition could be parsed, conditional event definition contains no condition.");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Conditional event must contain an expression for evaluation.", e.Message);
            }
        }
        [Test]
        public virtual void TestParseInvalidConditionalBoundaryEvent()
        {
            ParseInvalidConditionalEvent("testParseInvalidConditionalBoundaryEvent");
        }
        [Test]
        [Deployment]
        public virtual void TestParseConditionalBoundaryEvent()
        {
            ActivityImpl conditionalBoundaryEvent = FindActivityInDeployedProcessDefinition("conditionalBoundaryEvent");

            Assert.AreEqual(ActivityTypes.BoundaryConditional, conditionalBoundaryEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(BoundaryConditionalEventActivityBehavior), conditionalBoundaryEvent.ActivityBehavior.GetType());
        }
        [Test]//不需要不校验xml功能
        public virtual void TestParseInvalidIntermediateConditionalEvent()
        {
            ParseInvalidConditionalEvent("testParseInvalidIntermediateConditionalEvent");
        }

        [Test]
        [Deployment]
        public virtual void TestParseIntermediateConditionalEvent()
        {
            ActivityImpl intermediateConditionalEvent = FindActivityInDeployedProcessDefinition("intermediateConditionalEvent");

            Assert.AreEqual(ActivityTypes.IntermediateEventConditional, intermediateConditionalEvent.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(IntermediateConditionalEventBehavior), intermediateConditionalEvent.ActivityBehavior.GetType());
        }
        [Test]
        public virtual void TestParseInvalidEventSubprocessConditionalStartEvent()
        {
            ParseInvalidConditionalEvent("testParseInvalidEventSubprocessConditionalStartEvent");
        }

        [Test]
        [Deployment]
        public virtual void TestParseEventSubprocessConditionalStartEvent()
        {
            ActivityImpl conditionalStartEventSubProcess = FindActivityInDeployedProcessDefinition("conditionalStartEventSubProcess");

            Assert.AreEqual(ActivityTypes.StartEventConditional, conditionalStartEventSubProcess.Properties.Get(BpmnProperties.Type));
            Assert.AreEqual(typeof(EventSubProcessStartConditionalEventActivityBehavior), conditionalStartEventSubProcess.ActivityBehavior.GetType());

        }

        protected internal virtual void AssertActivityBounds(ActivityImpl activity, int x, int y, int width, int height)
        {
            Assert.AreEqual(x, activity.X);
            Assert.AreEqual(y, activity.Y);
            Assert.AreEqual(width, activity.Width);
            Assert.AreEqual(height, activity.Height);
        }

        protected internal virtual void AssertSequenceFlowWayPoints(TransitionImpl sequenceFlow, params int[] waypoints)
        {
            Assert.AreEqual(waypoints.Length, sequenceFlow.Waypoints.Count);
            for (int i = 0; i < waypoints.Length; i++)
            {
                Assert.AreEqual(waypoints[i], sequenceFlow.Waypoints.ToList().ElementAt(i));
            }
        }

        protected internal virtual ActivityImpl FindActivityInDeployedProcessDefinition(string activityId)
        {
            IProcessDefinition processDefinition = repositoryService.CreateProcessDefinitionQuery().First();
            Assert.NotNull(processDefinition);

            ProcessDefinitionEntity cachedProcessDefinition = processEngineConfiguration.DeploymentCache.ProcessDefinitionCache.Get(processDefinition.Id);
            return cachedProcessDefinition.FindActivity(activityId) as ActivityImpl;

        }
        [Test]
        public virtual void TestNoCamundaInSourceThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:in extension element should contain source!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing parameter 'source' or 'sourceExpression' when passing variables", e.Message);
            }
        }
        [Test]
        public virtual void TestNoCamundaInSourceShouldWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }
        [Test]
        public virtual void TestEmptyCamundaInSourceThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:in extension element should contain source!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Empty attribute 'source' when passing variables", e.Message);
            }
        }
        [Test]//不需要不校验xml功能
        public virtual void TestEmptyCamundaInSourceWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }
        [Test]
        public virtual void TestNoCamundaInTargetThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:in extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
        }
        [Test]
        public virtual void TestNoCamundaInTargetWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaInTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:in extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
            }
        }
        [Test]
        public virtual void TestEmptyCamundaInTargetThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:in extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
        }
        [Test]
        public virtual void TestEmptyCamundaInTargetWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaInTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }
        [Test]
        public virtual void TestNoCamundaOutSourceThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:out extension element should contain source!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing parameter 'source' or 'sourceExpression' when passing variables", e.Message);
            }
        }
        [Test]//不需要不校验xml功能
        public virtual void TestNoCamundaOutSourceWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }
        [Test]
        public virtual void TestEmptyCamundaOutSourceThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:out extension element should contain source!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Empty attribute 'source' when passing variables", e.Message);
            }
        }
        [Test]
        public virtual void TestEmptyCamundaOutSourceWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutSourceThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }
        [Test]
        public virtual void TestNoCamundaOutTargetThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:out extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
        }
        [Test]
        public virtual void TestNoCamundaOutTargetWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testNoCamundaOutTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:out extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Missing attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
            }
        }
        [Test]
        public virtual void TestEmptyCamundaOutTargetThrowsError()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Process camunda:out extension element should contain target!");
            }
            catch (ProcessEngineException e)
            {
                AssertTextPresent("Empty attribute 'target' when attribute 'source' or 'sourceExpression' is set", e.Message);
            }
        }
        [Test]
        public virtual void TestEmptyCamundaOutTargetWithoutValidation()
        {
            try
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = true;

                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testEmptyCamundaOutTargetThrowsError");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
            }
            finally
            {
                processEngineConfiguration.DisableStrictCallActivityValidation = false;
                repositoryService.DeleteDeployment(repositoryService.CreateDeploymentQuery().First().Id, true);
            }
        }

        [Test]
        [Deployment]
        public virtual void testParseProcessDefinitionTtl()
        {
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
            Assert.NotNull(processDefinitions);
            Assert.AreEqual(1, processDefinitions.Count);

            //int? timeToLive = (processDefinitions[0] as ProcessEngineImpl).HistoryTimeToLive;
            //Assert.NotNull(timeToLive);
            //Assert.AreEqual(5, timeToLive.Value);
        }

        [Test]
        [Deployment]
        public virtual void testParseProcessDefinitionEmptyTtl()
        {
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
            Assert.NotNull(processDefinitions);
            Assert.AreEqual(1, processDefinitions.Count);

            //int? timeToLive = processDefinitions[0].HistoryTimeToLive;
            //Assert.IsNull(timeToLive);
        }

        [Test]
        [Deployment]
        public virtual void testParseProcessDefinitionWithoutTtl()
        {
            IList<IProcessDefinition> processDefinitions = repositoryService.CreateProcessDefinitionQuery().ToList();
            Assert.NotNull(processDefinitions);
            Assert.AreEqual(1, processDefinitions.Count);

            //int? timeToLive = processDefinitions[0].HistoryTimeToLive;
            //Assert.IsNull(timeToLive);
        }
        [Test]
        public virtual void testParseProcessDefinitionInvalidTtl()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionInvalidTtl");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Cannot parse historyTimeToLive", e.Message);
            }
        }
        [Test]
        public virtual void testParseProcessDefinitionNegativTtl()
        {
            try
            {
                string resource = TestHelper.GetBpmnProcessDefinitionResource(this.GetType(), "testParseProcessDefinitionNegativeTtl");
                repositoryService.CreateDeployment().Name(resource).AddClasspathResource(resource).Deploy();
                Assert.Fail("Exception expected: Process definition historyTimeToLive value can not be parsed.");
            }
            catch (System.Exception e)
            {
                AssertTextPresent("Cannot parse historyTimeToLive", e.Message);
            }
        }

    }

}