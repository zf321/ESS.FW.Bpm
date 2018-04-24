using System.Linq;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    [TestFixture]
    public class ProcessEngineRuleClassDeploymentTest
    {
        public readonly ProcessEngineRule ProcessEngineRule = new ProvidedProcessEngineRule();

        [Test]
        public virtual void TestDeploymentOnClassLevel()
        {
            Assert.NotNull(
                ProcessEngineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="testHelperDeploymentTest")
                    .First(), "process is not deployed");
        }

        [Test]
        [Deployment]
        public virtual void TestDeploymentOnMethodOverridesClass()
        {
            Assert.NotNull(
                ProcessEngineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="testHelperDeploymentTestOverride")
                    .First(), "process is not deployed");
        }
    }
}