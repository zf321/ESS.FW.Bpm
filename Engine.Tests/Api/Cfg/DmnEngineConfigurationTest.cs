using System.Collections.Generic;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Dmn.engine;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Dmn.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using NUnit.Framework;

namespace Engine.Tests.Api.Cfg
{


    /// <summary>
    /// 
    /// </summary>
    public class DmnEngineConfigurationTest
    {

        protected internal const string CONFIGURATION_XML = "resources/api/cfg/custom-dmn-camunda.cfg.xml";

        protected internal IProcessEngine engine;

        //JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @After public void tearDown()
        public virtual void tearDown()
        {
            if (engine != null)
            {
                engine.Close();
                engine = null;
            }
        }

        [Test]
        public virtual void setDefaultInputExpressionLanguage()
        {
            // given a DMN engine configuration with default expression language
            DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration)DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
            dmnEngineConfiguration.DefaultInputExpressionExpressionLanguage = "groovy";

            ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
            processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the default expression language should be set on the DMN engine
            Assert.That(ConfigurationOfDmnEngine.DefaultInputExpressionExpressionLanguage, Is.EqualTo("groovy"));
        }

        [Test]
        public virtual void setCustomPostTableExecutionListener()
        {
            // given a DMN engine configuration with custom listener
            DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration)DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
            // Todo: mockito-core-1.9.5.jar
            //DmnDecisionTableEvaluationListener customEvaluationListener = mock(typeof(DmnDecisionTableEvaluationListener));
            IDmnDecisionTableEvaluationListener customEvaluationListener = null;
            IList<IDmnDecisionTableEvaluationListener> customListeners = new List<IDmnDecisionTableEvaluationListener>();
            customListeners.Add(customEvaluationListener);
            dmnEngineConfiguration.CustomPostDecisionTableEvaluationListeners = customListeners;

            ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
            processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the custom listener should be set on the DMN engine
            //Assert.That(ConfigurationOfDmnEngine.CustomPostDecisionTableEvaluationListeners, hasItem(customEvaluationListener));
            CollectionAssert.Contains(ConfigurationOfDmnEngine.CustomPostDecisionTableEvaluationListeners,
                customEvaluationListener);
        }

        // Todo: org.camunda.bpm.dmn.feel.impl.FeelEngineFactory
        //[Test]
        //public virtual void setFeelEngineFactory()
        //{
        //    // given a DMN engine configuration with feel engine factory
        //    DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration)DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
        //    FeelEngineFactory feelEngineFactory = mock(typeof(FeelEngineFactory));
        //    dmnEngineConfiguration.FeelEngineFactory = feelEngineFactory;

        //    ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
        //    processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

        //    // when the engine is initialized
        //    engine = processEngineConfiguration.BuildProcessEngine();

        //    // then the feel engine factory should be set on the DMN engine
        //    Assert.That(ConfigurationOfDmnEngine.FeelEngineFactory, Is.EqualTo(feelEngineFactory));
        //}

        [Test]
        public virtual void setScriptEngineResolver()
        {
            // given a DMN engine configuration with script engine resolver
            DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration)DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
            // Todo: mockito-core-1.9.5.jar
            //DmnScriptEngineResolver scriptEngineResolver = mock(typeof(DmnScriptEngineResolver));
            IDmnScriptEngineResolver scriptEngineResolver = null;
            dmnEngineConfiguration.ScriptEngineResolver = scriptEngineResolver;

            ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
            processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the script engine resolver should be set on the DMN engine
            Assert.That(ConfigurationOfDmnEngine.ScriptEngineResolver, Is.EqualTo(scriptEngineResolver));
        }

        [Test]
        public virtual void setElProvider()
        {
            // given a DMN engine configuration with el provider
            DefaultDmnEngineConfiguration dmnEngineConfiguration = (DefaultDmnEngineConfiguration)DmnEngineConfiguration.createDefaultDmnEngineConfiguration();
            // Todo: mockito-core-1.9.5.jar
            //ElProvider elProvider = mock(typeof(ElProvider));
            IELProvider elProvider = null;
            dmnEngineConfiguration.ElProvider = elProvider;

            ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();
            processEngineConfiguration.DmnEngineConfiguration = dmnEngineConfiguration;

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the el provider should be set on the DMN engine
            Assert.That(ConfigurationOfDmnEngine.ElProvider, Is.EqualTo(elProvider));
        }

        [Test]
        public virtual void setProcessEngineElProviderByDefault()
        {
            // given a default DMN engine configuration without el provider
            ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the DMN engine should use the process engine el provider
            Assert.AreEqual(typeof(ProcessEngineElProvider), ConfigurationOfDmnEngine.ElProvider.GetType());
        }

        // Todo: org.camunda.bpm.engine.impl.scripting.engine.ScriptingEngines
        //[Test]
        //public virtual void setProcessEngineScriptEnginesByDefault()
        //{
        //    // given a default DMN engine configuration without script engine resolver
        //    ProcessEngineConfigurationImpl processEngineConfiguration = createProcessEngineConfiguration();

        //    // when the engine is initialized
        //    engine = processEngineConfiguration.BuildProcessEngine();

        //    // then the DMN engine should use the script engines from the process engine
        //    Assert.AreEqual(processEngineConfiguration.ScriptingEngines, ConfigurationOfDmnEngine.ScriptEngineResolver);
        //}

        [Test]
        public virtual void setDmnEngineConfigurationOverXmlConfiguration()
        {
            // given an embedded DMN engine configuration in XML process engine configuration
            // with default expression language
            ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.CreateProcessEngineConfigurationFromResource(CONFIGURATION_XML);

            // checks that the configuration is set as on XML
            DefaultDmnEngineConfiguration dmnEngineConfiguration = processEngineConfiguration.DmnEngineConfiguration;
            //Assert.That(dmnEngineConfiguration, Is.EqualTo(notNullValue()));
            Assert.That(dmnEngineConfiguration, Is.Not.Null);
            Assert.That(dmnEngineConfiguration.DefaultInputExpressionExpressionLanguage, Is.EqualTo("groovy"));

            // when the engine is initialized
            engine = processEngineConfiguration.BuildProcessEngine();

            // then the default expression language should be set in the DMN engine
            Assert.That(ConfigurationOfDmnEngine.DefaultInputExpressionExpressionLanguage, Is.EqualTo("groovy"));
        }

        protected internal virtual ProcessEngineConfigurationImpl createProcessEngineConfiguration()
        {
            return (ProcessEngineConfigurationImpl)ProcessEngineConfiguration.CreateStandaloneInMemProcessEngineConfiguration().SetJdbcUrl("jdbc:h2:mem:camunda" + this.GetType().Name);
        }

        protected internal virtual DefaultDmnEngineConfiguration ConfigurationOfDmnEngine
        {
            get
            {
                ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl)engine.ProcessEngineConfiguration;

                IDmnEngine dmnEngine = processEngineConfiguration.DmnEngine;
                return (DefaultDmnEngineConfiguration)dmnEngine.Configuration;
            }
        }

    }

}