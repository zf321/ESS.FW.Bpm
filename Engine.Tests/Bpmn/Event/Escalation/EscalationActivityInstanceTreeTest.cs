using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Escalation
{
    [TestFixture]
    public class EscalationActivityInstanceTreeTest : PluggableProcessEngineTestCase
    {
        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.testThrowEscalationEventFromEmbeddedSubprocess.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationBoundaryEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("escalationProcess");
            // an escalation event is thrown from embedded subprocess and caught by non-interrupting boundary event on subprocess

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .Activity("taskAfterCatchedEscalation").BeginScope("subProcess").Activity("taskInSubprocess").Done());
            
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventTest.testInterruptingEscalationBoundaryEvent.bpmn20.xml" })]
        public virtual void testInterruptingEscalationBoundaryEvent()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("escalationProcess");
            // an escalation event is thrown from embedded subprocess and caught by interrupting boundary event on subprocess

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .Activity("taskAfterCatchedEscalation").Done());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testCatchEscalationEventInsideSubprocess.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationEventSubprocessInsideSubprocess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("escalationProcess");
            // an escalation event is thrown from embedded subprocess and caught by non-interrupting event subprocess inside the subprocess

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .BeginScope("subProcess").Activity("taskInSubprocess").BeginScope("escalationEventSubprocess").Activity("taskAfterCatchedEscalation").Done());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testCatchEscalationEventFromEmbeddedSubprocess.bpmn20.xml" })]
        public virtual void testNonInterruptingEscalationEventSubprocessOutsideSubprocess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("escalationProcess");
            // an escalation event is thrown from embedded subprocess and caught by non-interrupting event subprocess outside the subprocess

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .BeginScope("subProcess").Activity("taskInSubprocess").EndScope().BeginScope("escalationEventSubprocess").Activity("taskAfterCatchedEscalation").Done());
        }

        [Test]
        [Deployment(new string[] { "resources/bpmn/event/escalation/EscalationEventSubprocessTest.testInterruptionEscalationEventSubprocess.bpmn20.xml" })]
        public virtual void testInterruptingEscalationEventSubprocessInsideSubprocess()
        {
            IProcessInstance processInstance = runtimeService.StartProcessInstanceByKey("escalationProcess");
            // an escalation event is thrown from embedded subprocess and caught by interrupting event subprocess inside the subprocess

            IActivityInstance tree = runtimeService.GetActivityInstance(processInstance.Id);
            ActivityInstanceAssert.That(tree)
                .HasStructure(ActivityInstanceAssert.DescribeActivityInstanceTree(processInstance.ProcessDefinitionId)
                .BeginScope("subProcess").BeginScope("escalationEventSubprocess").Activity("taskAfterCatchedEscalation").Done());            
        }

    }

}