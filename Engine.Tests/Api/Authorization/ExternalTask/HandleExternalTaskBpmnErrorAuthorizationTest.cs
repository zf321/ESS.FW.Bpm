using System.Linq;
using ESS.FW.Bpm.Engine.Externaltask;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{


    /// <summary>
    /// Tests the authorization of the bpmn error handling of an external task.
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class HandleExternalTaskBpmnErrorAuthorizationTest extends HandleExternalTaskAuthorizationTest
    public class HandleExternalTaskBpmnErrorAuthorizationTest : HandleExternalTaskAuthorizationTest
    {

        public override void AssertExternalTaskResults()
        {
            Assert.AreEqual(0, engineRule.ExternalTaskService.CreateExternalTaskQuery().Count());
        }

        public override void testExternalTaskApi(ILockedExternalTask task)
        {
            engineRule.ExternalTaskService.HandleBpmnError(task.Id, "workerId", "errorCode");
        }
    }


}