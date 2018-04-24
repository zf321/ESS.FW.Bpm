using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api.ExternalTask
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class ExternalTaskSupportTest
    public class ExternalTaskSupportTest
    {
        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Rule public ProcessEngineRule rule = new util.ProvidedProcessEngineRule();
        public ProcessEngineRule rule = new ProvidedProcessEngineRule();

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameters public static java.util.Collection<Object[]> processResources()
        public static ICollection<object[]> processResources()
        {
            return (new object[][]
            {
            new object[] {"resources/api/externaltask/ExternalTaskSupportTest.BusinessRuleTask.bpmn20.xml"},
            new object[] {"resources/api/externaltask/ExternalTaskSupportTest.MessageEndEvent.bpmn20.xml"},
            new object[] {"resources/api/externaltask/ExternalTaskSupportTest.MessageIntermediateEvent.bpmn20.xml"},
            new object[] {"resources/api/externaltask/ExternalTaskSupportTest.SendTask.bpmn20.xml"}
            });
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Parameter public String processDefinitionResource;
        public string processDefinitionResource;

        protected internal string deploymentId;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp()
        public virtual void setUp()
        {
            deploymentId = rule.RepositoryService.CreateDeployment().AddClasspathResource(processDefinitionResource).Deploy().Id;
        }

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            if (!string.ReferenceEquals(deploymentId, null))
            {
                rule.RepositoryService.DeleteDeployment(deploymentId, true);
            }
        }

        [Test]
        public virtual void testExternalTaskSupport()
        {
            // given
            IProcessDefinition processDefinition = rule.RepositoryService.CreateProcessDefinitionQuery().First();

            // when
            IProcessInstance processInstance = rule.RuntimeService.StartProcessInstanceById(processDefinition.Id);

            // then
            IList<ILockedExternalTask> externalTasks = rule.ExternalTaskService.FetchAndLock(1, "aWorker")
                 //.Topic("externalTaskTopic", 5000L).Execute();
                 as IList<ILockedExternalTask>;
            Assert.AreEqual(1, externalTasks.Count);
            Assert.AreEqual(processInstance.Id, externalTasks[0].ProcessInstanceId);

            // and it is possible to complete the external task successfully and end the process instance
            rule.ExternalTaskService.Complete(externalTasks[0].Id, "aWorker");

            Assert.AreEqual(0L, rule.RuntimeService.CreateProcessInstanceQuery().Count());
        }
    }

}