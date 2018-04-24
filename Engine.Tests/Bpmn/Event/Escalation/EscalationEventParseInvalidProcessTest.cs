using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Escalation
{
    
    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class EscalationEventParseInvalidProcessTest
    public class EscalationEventParseInvalidProcessTest
    {

        private const string PROCESS_DEFINITION_DIRECTORY = "resources/bpmn/event/escalation/";

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}, expected error message = {1}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {
            return (new object[][]
            {
            new object[] {"EscalationEventParseInvalidProcessTest.missingIdOnEscalation.bpmn20.xml", "escalation must have an id"},
            new object[] {"EscalationEventParseInvalidProcessTest.invalidAttachement.bpmn20.xml", "An escalation boundary event should only be attached to a subprocess or a call activity"},
            new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnBoundaryEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithSameEscalationCode.bpmn20.xml", "multiple escalation boundary events with the same escalationCode 'escalationCode' are not supported on same scope"},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithAndWithoutEscalationCode.bpmn20.xml", "The same scope can not contains an escalation boundary event without escalation code and another one with escalation code."},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationBoundaryEventsWithoutEscalationCode.bpmn20.xml", "The same scope can not contains more than one escalation boundary event without escalation code."},
            new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationCodeOnIntermediateThrowingEscalationEvent.bpmn20.xml", "throwing escalation event must have an 'escalationCode'"},
            new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationRefOnIntermediateThrowingEvent.bpmn20.xml", "escalationEventDefinition does not have required attribute 'escalationRef'"},
            new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnIntermediateThrowingEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
            new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationCodeOnEscalationEndEvent.bpmn20.xml", "escalation end event must have an 'escalationCode'"},
            new object[] {"EscalationEventParseInvalidProcessTest.missingEscalationRefOnEndEvent.bpmn20.xml", "escalationEventDefinition does not have required attribute 'escalationRef'"},
            new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnEndEvent.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
            new object[] {"EscalationEventParseInvalidProcessTest.invalidEscalationRefOnEscalationEventSubprocess.bpmn20.xml", "could not find escalation with id 'invalid-escalation'"},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleInterruptingEscalationEventSubprocesses.bpmn20.xml", "multiple escalation event subprocesses with the same escalationCode 'escalationCode' are not supported on same scope"},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithSameEscalationCode.bpmn20.xml", "multiple escalation event subprocesses with the same escalationCode 'escalationCode' are not supported on same scope"},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithAndWithoutEscalationCode.bpmn20.xml", "The same scope can not contains an escalation event subprocess without escalation code and another one with escalation code."},
            new object[] {"EscalationEventParseInvalidProcessTest.multipleEscalationEventSubprocessWithoutEscalationCode.bpmn20.xml", "The same scope can not contains more than one escalation event subprocess without escalation code."}
            });
        }

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(0) public String processDefinitionResource;
        public string processDefinitionResource;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(1) public String expectedErrorMessage;
        public string expectedErrorMessage;

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public org.Camunda.bpm.engine.test.ProcessEngineRule rule = new org.Camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
        public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        protected internal IRepositoryService repositoryService;

        [SetUp]
        public virtual void initServices()
        {
            repositoryService = rule.RepositoryService;
        }

        [Test]
        public virtual void testParseInvalidProcessDefinition()
        {
            try
            {
                string deploymentId = repositoryService.CreateDeployment().AddClasspathResource(PROCESS_DEFINITION_DIRECTORY + processDefinitionResource).Deploy().Id;

                // in case that the deployment do not Assert.Fail
                repositoryService.DeleteDeployment(deploymentId, true);

                Assert.Fail("exception expected: " + expectedErrorMessage);
            }
            catch (System.Exception e)
            {
                assertExceptionMessageContainsText(e, expectedErrorMessage);
            }
        }

        public virtual void assertExceptionMessageContainsText(System.Exception e, string expectedMessage)
        {
            string actualMessage = e.Message;
            if (string.ReferenceEquals(actualMessage, null) || !actualMessage.Contains(expectedMessage))
            {
                Assert.Fail("expected presence of [" + expectedMessage + "], but was [" + actualMessage + "]");
                //throw new AssertionFailedError("expected presence of [" + expectedMessage + "], but was [" + actualMessage + "]");
            }
        }
    }

}