using System.Linq;
using Engine.Tests.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Testing
{
    [TestFixture]
    public class ProcessEngineRuleParentClassDeploymentTest : ProcessEngineRuleParentClassDeployment
    {
//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final test.ProcessEngineRule processEngineRule = new test.util.ProvidedProcessEngineRule();
        public readonly ProcessEngineRule processEngineRule = new ProvidedProcessEngineRule();

        [Test]
        public virtual void TestDeploymentOnParentClassLevel()
        {
            Assert.NotNull(
                processEngineRule.RepositoryService.CreateProcessDefinitionQuery(c=>c.Key =="testHelperDeploymentTest")
                    .First(), "process is not deployed");
        }
    }
}