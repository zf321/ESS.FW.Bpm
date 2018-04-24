using System.Collections.Generic;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.Event.Signal
{
    // Todo: RunWithAttribute
    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class SignalEventParseInvalidProcessTest
    public class SignalEventParseInvalidProcessTest
    {

        private const string PROCESS_DEFINITION_DIRECTORY = "resources/bpmn/event/signal/";

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters(name = "{index}: process definition = {0}, expected error message = {1}") public static java.util.Collection<Object[]> data()
        public static ICollection<object[]> data()
        {
            return (new object[][]
            {
            new object[] {"InvalidProcessWithDuplicateSignalNames.bpmn20.xml", "duplicate signal name"},
            new object[] {"InvalidProcessWithNoSignalName.bpmn20.xml", "signal with id 'alertSignal' has no name"},
            new object[] {"InvalidProcessWithSignalNoId.bpmn20.xml", "signal must have an id"},
            new object[] {"InvalidProcessWithSignalNoRef.bpmn20.xml", "signalEventDefinition does not have required property 'signalRef'"},
            new object[] {"InvalidProcessWithMultipleSignalStartEvents.bpmn20.xml", "Cannot have more than one signal event subscription with name 'signal'"},
            new object[] {"InvalidProcessWithMultipleInterruptingSignalEventSubProcesses.bpmn20.xml", "Cannot have more than one signal event subscription with name 'alert'"}
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

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
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
                assertTextPresent(expectedErrorMessage, e.Message);
            }
        }

        public virtual void assertTextPresent(string expected, string actual)
        {
            if (string.ReferenceEquals(actual, null) || !actual.Contains(expected))
            {
                Assert.Fail("expected presence of [" + expected + "], but was [" + actual + "]");
                //throw new AssertionFailedError("expected presence of [" + expected + "], but was [" + actual + "]");
            }
        }
    }

}