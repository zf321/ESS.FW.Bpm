using System.Linq;
using ESS.FW.Bpm.Engine.Externaltask;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization.ExternalTask
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class HandleExternalTaskFailureAuthorizationTest extends HandleExternalTaskAuthorizationTest
    [TestFixture]
    public class HandleExternalTaskFailureAuthorizationTest : HandleExternalTaskAuthorizationTest
    {

        public override void AssertExternalTaskResults()
        {
            ESS.FW.Bpm.Engine.Externaltask.IExternalTask externalTask = engineRule.ExternalTaskService.CreateExternalTaskQuery().First();

            Assert.AreEqual(5, (int)externalTask.Retries);
            Assert.AreEqual("error", externalTask.ErrorMessage);
        }

        public override void testExternalTaskApi(ILockedExternalTask task)
        {
            engineRule.ExternalTaskService.HandleFailure(task.Id, "workerId", "error", 5, 5000L);
        }
    }

}