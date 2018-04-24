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
    //ORIGINAL LINE: @RunWith(Parameterized.class) public class CompleteExternalTaskAuthorizationTest extends HandleExternalTaskAuthorizationTest
    public class CompleteExternalTaskAuthorizationTest : HandleExternalTaskAuthorizationTest
    {
        public override void testExternalTaskApi(ILockedExternalTask task)
        {
            engineRule.ExternalTaskService.Complete(task.Id, "workerId");
        }

        public override void AssertExternalTaskResults()
        {
            Assert.AreEqual(0, engineRule.ExternalTaskService.CreateExternalTaskQuery().Count());
        }
        
    }

}