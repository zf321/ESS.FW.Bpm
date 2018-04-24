using System.Linq;
using NUnit.Framework;

namespace Engine.Tests.Bpmn.UserTask
{

    using PluggableProcessEngineTestCase = PluggableProcessEngineTestCase;


    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class InitiatorTest : PluggableProcessEngineTestCase
    {


        [Test]
        [Deployment]
        public virtual void TestInitiator()
        {
            try
            {
                identityService.AuthenticatedUserId = "bono";
                runtimeService.StartProcessInstanceByKey("InitiatorProcess");
            }
            finally
            {
                identityService.AuthenticatedUserId = null;
            }
            var list = taskService.CreateTaskQuery().ToList();//

            Assert.IsTrue(list.Where(c => c.Assignee == "bono").Count() > 0);
        }
    }

}