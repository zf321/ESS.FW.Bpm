using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{
    [TestFixture]
    public class DefaultUserPermissionNameForTaskCfgTest
    {

        [Test]
        public virtual void updateIsDefaultTaskPermission()
        {
            Assert.AreEqual("Permissions.Update", (new StandaloneInMemProcessEngineConfiguration()).DefaultUserPermissionNameForTask);
        }

        [Test]
        public virtual void shouldInitUpdatePermission()
        {
            TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

            // given
            testProcessEngineCfg.DefaultUserPermissionNameForTask = "Permissions.Update";

            // if
            testProcessEngineCfg.InitDefaultUserPermissionForTask();

            // then
            Assert.AreEqual(Permissions.Update, testProcessEngineCfg.DefaultUserPermissionForTask);
        }

        [Test]
        public virtual void shouldInitTaskWorkPermission()
        {
            TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

            // given
            testProcessEngineCfg.DefaultUserPermissionNameForTask = "TASK_WORK";

            // if
            testProcessEngineCfg.InitDefaultUserPermissionForTask();

            // then
            Assert.AreEqual(Permissions.TaskWork, testProcessEngineCfg.DefaultUserPermissionForTask);
        }

        [Test]
        public virtual void shouldThrowExceptionOnUnsupportedPermission()
        {
            TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

            // given
            testProcessEngineCfg.DefaultUserPermissionNameForTask = "UNSUPPORTED";

            // if
            try
            {
                testProcessEngineCfg.InitDefaultUserPermissionForTask();
                Assert.Fail("Exception expected");

            }
            catch (ProcessEngineException e)
            {
                string expectedExceptionMessage = string.Format("Invalid value '{0}' for configuration property 'defaultUserPermissionNameForTask'.", "UNSUPPORTED");
                //Assert.That(e.Message, containsString(expectedExceptionMessage));
                Assert.That(e.Message, Does.Contain(expectedExceptionMessage));
            }
        }

        [Test]
        public virtual void shouldThrowExceptionOnNullPermissionName()
        {
            TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

            // given
            testProcessEngineCfg.DefaultUserPermissionNameForTask = null;

            // if
            try
            {
                testProcessEngineCfg.InitDefaultUserPermissionForTask();
                Assert.Fail("Exception expected");

            }
            catch (ProcessEngineException e)
            {
                string expectedExceptionMessage = "Invalid value 'null' for configuration property 'defaultUserPermissionNameForTask'.";
                //Assert.That(e.Message, containsString(expectedExceptionMessage));
                Assert.That(e.Message, Does.Contain(expectedExceptionMessage));
            }
        }

        [Test]
        public virtual void shouldNotInitIfAlreadySet()
        {
            TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

            // given
            testProcessEngineCfg.DefaultUserPermissionForTask = Permissions.All;

            // if
            testProcessEngineCfg.InitDefaultUserPermissionForTask();

            // then
            Assert.AreEqual(Permissions.All, testProcessEngineCfg.DefaultUserPermissionForTask);
        }

        [Test]
        public virtual void shouldInitTaskPermission()
        {
            IProcessEngine engine = null;
            try
            {
                // if
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();
                TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

                engine = testProcessEngineCfg.SetProcessEngineName("DefaultTaskPermissionsCfgTest-engine")
                    .SetJdbcUrl(string.Format("jdbc:h2:mem:{0}", "DefaultTaskPermissionsCfgTest-engine-db"))
                    //.SetMetricsEnabled(false)
                    .SetJobExecutorActivate(false).BuildProcessEngine();

                // then
                Assert.True(testProcessEngineCfg.initMethodCalled);
            }
            finally
            {
                if (engine != null)
                {
                    engine.Close();
                }
            }
        }

        internal class TestProcessEngineCfg : StandaloneInMemProcessEngineConfiguration
        {

            internal bool initMethodCalled = false;

            public new void InitDefaultUserPermissionForTask()
            {
                base.InitDefaultUserPermissionForTask();
                initMethodCalled = true;
            }

            //public override void initDefaultUserPermissionForTask()
            //{
            //    base.initDefaultUserPermissionForTask();
            //    initMethodCalled = true;
            //}
        }


    }

}