

using ESS.FW.Bpm.Engine;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    [RequiredHistoryLevel(ProcessEngineConfiguration.HistoryFull)]

    [TestFixture]
    public abstract class AbstractUserOperationLogTest : PluggableProcessEngineTestCase
    {

        public const string USER_ID = "demo";

        [SetUp]
        public void setUp()
        {
            identityService.AuthenticatedUserId = USER_ID;
        }
        [TearDown]
        public void tearDown()
        {
            identityService.ClearAuthentication();
        }

    }

}