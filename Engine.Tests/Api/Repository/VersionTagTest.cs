using System.Linq;
using ESS.FW.Bpm.Engine.Repository;
using NUnit.Framework;

namespace Engine.Tests.Api.Repository
{

    /// <summary>
    /// </summary>
    public class VersionTagTest : PluggableProcessEngineTestCase
    {

        [Test]
        public virtual void testParsingVersionTag()
        {
            IProcessDefinition process = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/.First();

            Assert.AreEqual("ver_tag_1", process.VersionTag);
        }


        [Test]
        public virtual void testParsingNullVersionTag()
        {
            IProcessDefinition process = repositoryService.CreateProcessDefinitionQuery()/*.OrderByProcessDefinitionId()*//*.Asc()*/.First();

            Assert.AreEqual(null, process.VersionTag);
        }
    }

}