using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Beans
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class DefaultArtifactFactoryTest
    {
        public class DummyArtifact
        {
            // no content
        }

        [Test]
        public virtual void TestDefaultArtifactService()
        {
            IArtifactFactory artifactFactory = new DefaultArtifactFactory();

            var artifact = artifactFactory.GetArtifact<DummyArtifact>(typeof(DummyArtifact));
            Assert.NotNull(artifact);
        }
    }
}