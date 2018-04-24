using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NSubstitute;
using NUnit.Framework;

namespace Engine.Tests.Cfg
{
    [TestFixture]
    public class CompositeProcessEnginePluginTest
    {
        private IProcessEnginePlugin PluginA;
        private IProcessEnginePlugin PluginB;
        //private static readonly InOrder ORDER = inOrder(PLUGIN_A, PLUGIN_B);

        private static readonly ProcessEngineConfigurationImpl Configuration =
            Substitute.For<ProcessEngineConfigurationImpl>();

        private static readonly IProcessEngine Engine = Substitute.For<IProcessEngine>();
        //private InOrder inOrder;

        public CompositeProcessEnginePluginTest()
        {
            PluginA = ProcessEnginePlugin("PluginA");
            PluginB = ProcessEnginePlugin("PluginB");
    }

        [Test]
        public virtual void AddPlugin()
        {
            var composite = new CompositeProcessEnginePlugin(PluginA);

            Assert.That(composite.Plugins.Count, Is.EqualTo(1));
            Assert.That(composite.Plugins[0], Is.EqualTo(PluginA));

            composite.AddProcessEnginePlugin(PluginB);
            Assert.That(composite.Plugins.Count, Is.EqualTo(2));
            Assert.That(composite.Plugins[1], Is.EqualTo(PluginB));
        }

        [Test]
        public virtual void AddPlugins()
        {
            var composite = new CompositeProcessEnginePlugin(PluginA);
            composite.AddProcessEnginePlugins(new List<IProcessEnginePlugin>(){PluginB});

            Assert.That(composite.Plugins.Count, Is.EqualTo(2));
            Assert.That(composite.Plugins[0], Is.EqualTo(PluginA));
            Assert.That(composite.Plugins[1], Is.EqualTo(PluginB));
        }

        [Test]
        public virtual void AllPluginsOnPreInit()
        {
            new CompositeProcessEnginePlugin(PluginA, PluginB).PreInit(Configuration);

           PluginA.PreInit(Configuration);
            PluginB.PreInit(Configuration);
        }

        public virtual void AllPluginsOnPostInit()
        {
            new CompositeProcessEnginePlugin(PluginA, PluginB).PostInit(Configuration);

            PluginA.PostInit(Configuration);
            PluginB.PostInit(Configuration);
        }

        [Test]
        public virtual void AllPluginsOnPostProcessEngineBuild()
        {
            new CompositeProcessEnginePlugin(PluginA, PluginB).PostProcessEngineBuild(Engine);

            PluginA.PostProcessEngineBuild(Engine);
            PluginB.PostProcessEngineBuild(Engine);
        }

        //[Test]
        //public virtual void VerifyToString()
        //{
        //    Assert.That(new CompositeProcessEnginePlugin(PluginA, PluginB).ToString(),
        //        Is.EqualTo("CompositeProcessEnginePlugin[PluginA, PluginB]"));
        //}

        private IProcessEnginePlugin ProcessEnginePlugin(string name)
        {
            var plugin = Substitute.For<IProcessEnginePlugin>();
            //plugin.ToString().Returns(name);

            return plugin;
        }
    }
}