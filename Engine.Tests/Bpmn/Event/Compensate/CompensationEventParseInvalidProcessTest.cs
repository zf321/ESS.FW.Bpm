using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Compensate
{
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class CompensationEventParseInvalidProcessTest
    [TestFixture]
    public class CompensationEventParseInvalidProcessTest
    {

        private const string PROCESS_DEFINITION_DIRECTORY = "resources/bpmn/event/compensate/";

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}, expected error message = {1}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {
            return (new object[][]
            {
            new object[] {"CompensationEventParseInvalidProcessTest.illegalCompensateActivityRefParentScope.bpmn20.xml", "Invalid attribute value for 'activityRef': no activity with id 'someServiceInMainProcess' in scope 'subProcess'"},
            new object[] {"CompensationEventParseInvalidProcessTest.illegalCompensateActivityRefNestedScope.bpmn20.xml", "Invalid attribute value for 'activityRef': no activity with id 'someServiceInNestedScope' in scope 'subProcess'"},
            new object[] {"CompensationEventParseInvalidProcessTest.invalidActivityRefFails.bpmn20.xml", "Invalid attribute value for 'activityRef':"},
            new object[] {"CompensationEventParseInvalidProcessTest.MultipleCompensationCatchEventsCompensationAttributeMissingFails.bpmn20.xml", "compensation boundary catch must be connected to element with isForCompensation=true"},
            new object[] {"CompensationEventParseInvalidProcessTest.MultipleCompensationCatchEventsFails.bpmn20.xml", "multiple boundary events with compensateEventDefinition not supported on same activity"},
            new object[] {"CompensationEventParseInvalidProcessTest.MultipleCompensationEventSubProcesses.bpmn20.xml", "multiple event subprocesses with compensation start event are not supported on the same scope"},
            new object[] {"CompensationEventParseInvalidProcessTest.compensationEventSubProcessesAtProcessLevel.bpmn20.xml", "event subprocess with compensation start event is only supported for embedded subprocess"},
            new object[] {"CompensationEventParseInvalidProcessTest.compensationEventSubprocessAndBoundaryEvent.bpmn20.xml", "compensation boundary event and event subprocess with compensation start event are not supported on the same scope"},
            new object[] {"CompensationEventParseInvalidProcessTest.invalidOutgoingSequenceflow.bpmn20.xml", "Invalid outgoing sequence flow of compensation activity 'undoTask'. A compensation activity should not have an incoming or outgoing sequence flow."},
            new object[] {"CompensationEventParseInvalidProcessTest.invalidIncomingSequenceflow.bpmn20.xml", "Invalid incoming sequence flow of compensation activity 'task'. A compensation activity should not have an incoming or outgoing sequence flow."}
            });
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(0) public String processDefinitionResource;
        public string processDefinitionResource;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter(1) public String expectedErrorMessage;
        public string expectedErrorMessage;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        protected internal IRepositoryService repositoryService;

        [SetUp]
        public virtual void initServices()
        {
            repositoryService = rule.RepositoryService;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void testParseInvalidProcessDefinition()
        public virtual void testParseInvalidProcessDefinition()
        {
            try
            {
                repositoryService.CreateDeployment().AddClasspathResource(PROCESS_DEFINITION_DIRECTORY + processDefinitionResource).Deploy();

                Assert.Fail("exception expected: " + expectedErrorMessage);
            }
            catch (System.Exception e)
            {
                AssertExceptionMessageContainsText(e, expectedErrorMessage);
            }
        }

        public virtual void AssertExceptionMessageContainsText(System.Exception e, string expectedMessage)
        {
            string actualMessage = e.Message;
            if (string.ReferenceEquals(actualMessage, null) || !actualMessage.Contains(expectedMessage))
            {
                throw new AssertionException("expected presence of [" + expectedMessage + "], but was [" + actualMessage + "]");
            }
        }
    }

}