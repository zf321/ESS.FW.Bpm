using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Dmn.Impl.EL;
using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Parser;
using ESS.FW.Bpm.Engine.History.Impl.Producer;
using ESS.FW.Bpm.Engine.Impl.EL;
using ESS.FW.Bpm.Engine.Impl.Metrics.Dmn;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Dmn.Impl.Transformer;
using ESS.FW.Bpm.Model.Dmn.instance;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.Configuration
{

    /// <summary>
    ///     Modify the given DMN engine configuration so that the DMN engine can be used
    ///     from the process engine. Note that properties will not be overridden if they
    ///     are set on the configuration, except the transform handler for the decision table.
    ///     
    /// </summary>
    public class DmnEngineConfigurationBuilder
    {
        protected internal readonly DefaultDmnEngineConfiguration DmnEngineConfiguration;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnHistoryEventProducer DmnHistoryEventProducerRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal ExpressionManager ExpressionManagerRenamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IHistoryLevel HistoryLevelRenamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnScriptEngineResolver ScriptEngineResolverRenamed;

        /// <summary>
        ///     Creates a new builder to modify the given DMN engine configuration.
        /// </summary>
        public DmnEngineConfigurationBuilder(DefaultDmnEngineConfiguration dmnEngineConfiguration)
        {
            EnsureUtil.EnsureNotNull("dmnEngineConfiguration", dmnEngineConfiguration);

            this.DmnEngineConfiguration = dmnEngineConfiguration;
        }

        public virtual DmnEngineConfigurationBuilder HistoryLevel(IHistoryLevel historyLevel)
        {
            HistoryLevelRenamed = historyLevel;

            return this;
        }

        public virtual DmnEngineConfigurationBuilder DmnHistoryEventProducer(
            IDmnHistoryEventProducer dmnHistoryEventProducer)
        {
            DmnHistoryEventProducerRenamed = dmnHistoryEventProducer;

            return this;
        }

        public virtual DmnEngineConfigurationBuilder ScriptEngineResolver(IDmnScriptEngineResolver scriptEngineResolver)
        {
            ScriptEngineResolverRenamed = scriptEngineResolver;

            return this;
        }

        public virtual DmnEngineConfigurationBuilder ExpressionManager(ExpressionManager expressionManager)
        {
            ExpressionManagerRenamed = expressionManager;

            return this;
        }

        /// <summary>
        ///     Modify the given DMN engine configuration and return it.
        /// </summary>
        public virtual DefaultDmnEngineConfiguration Build()
        {
            var decisionEvaluationListeners = CreateCustomPostDecisionEvaluationListeners();
            DmnEngineConfiguration.CustomPostDecisionEvaluationListeners = decisionEvaluationListeners;

            // override the decision table handler
            var dmnTransformer = DmnEngineConfiguration.Transformer;
            dmnTransformer.ElementTransformHandlerRegistry.addHandler(typeof (IDefinitions),new DecisionRequirementsDefinitionTransformHandler());
            dmnTransformer.ElementTransformHandlerRegistry.addHandler(typeof (IDecision), new DecisionDefinitionHandler());

            // do not override the script engine resolver if set
            if (DmnEngineConfiguration.ScriptEngineResolver == null)
            {
                //EnsureUtil.EnsureNotNull("scriptEngineResolver", scriptEngineResolver_Renamed);

               // dmnEngineConfiguration.ScriptEngineResolver = scriptEngineResolver_Renamed;
            }

            // do not override the el provider if set
            if (DmnEngineConfiguration.ElProvider == null)
            {
                EnsureUtil.EnsureNotNull("expressionManager", ExpressionManagerRenamed);

                var elProvider = new ProcessEngineElProvider(ExpressionManagerRenamed);
                DmnEngineConfiguration.ElProvider = elProvider;
            }

            return DmnEngineConfiguration;
        }

        protected internal virtual IList<IDmnDecisionEvaluationListener> CreateCustomPostDecisionEvaluationListeners()
        {
            EnsureUtil.EnsureNotNull("dmnHistoryEventProducer", DmnHistoryEventProducerRenamed);
            // note that the history level may be null - see CAM-5165

            var historyDecisionEvaluationListener =
                new HistoryDecisionEvaluationListener(DmnHistoryEventProducerRenamed, HistoryLevelRenamed);
            
            //TODO Dmn自定义Listener
            var customPostDecisionEvaluationListeners = DmnEngineConfiguration.CustomPostDecisionEvaluationListeners;
            customPostDecisionEvaluationListeners.Add(new MetricsDecisionEvaluationListener());
            customPostDecisionEvaluationListeners.Add(historyDecisionEvaluationListener);

            return customPostDecisionEvaluationListeners;
        }
    }
}