using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Dmn.engine.@delegate;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.metrics;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.transform;
using ESS.FW.Bpm.Engine.Dmn.engine.spi;
using ESS.FW.Bpm.Model.Dmn.impl;
using ESS.FW.Bpm.Engine.Dmn.Feel;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.EL;

namespace ESS.FW.Bpm.Engine.Dmn.engine.impl
{
    public class DefaultDmnEngineConfiguration : DmnEngineConfiguration
    {
        public const string FEEL_EXPRESSION_LANGUAGE_ALTERNATIVE = "feel";
        public const string JUEL_EXPRESSION_LANGUAGE = "juel";

        public static readonly string FEEL_EXPRESSION_LANGUAGE = DmnModelConstants.FeelNs;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IDmnDecisionEvaluationListener> customPostDecisionEvaluationListeners_Renamed =
            new List<IDmnDecisionEvaluationListener>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IDmnDecisionTableEvaluationListener> customPostDecisionTableEvaluationListeners_Renamed
            = new List<IDmnDecisionTableEvaluationListener>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IDmnDecisionEvaluationListener> customPreDecisionEvaluationListeners_Renamed =
            new List<IDmnDecisionEvaluationListener>();

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IList<IDmnDecisionTableEvaluationListener> customPreDecisionTableEvaluationListeners_Renamed =
            new List<IDmnDecisionTableEvaluationListener>();

        // Decision evaluation listeners
        protected internal IList<IDmnDecisionEvaluationListener> decisionEvaluationListeners;
        protected internal IList<IDmnDecisionTableEvaluationListener> decisionTableEvaluationListeners;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string defaultInputEntryExpressionLanguage_Renamed = FEEL_EXPRESSION_LANGUAGE;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string defaultInputExpressionExpressionLanguage_Renamed = JUEL_EXPRESSION_LANGUAGE;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string defaultLiteralExpressionLanguage_Renamed = JUEL_EXPRESSION_LANGUAGE;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string defaultOutputEntryExpressionLanguage_Renamed = JUEL_EXPRESSION_LANGUAGE;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IELProvider elProvider_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnEngineMetricCollector engineMetricCollector_Renamed;
        protected internal IFeelEngine feelEngine;
////JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IFeelEngineFactory feelEngineFactory_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnScriptEngineResolver scriptEngineResolver_Renamed;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal IDmnTransformer transformer_Renamed = new DefaultDmnTransformer();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.Util.Collection<? extends org.camunda.bpm.dmn.engine.delegate.DmnDecisionEvaluationListener> getDefaultDmnDecisionEvaluationListeners()
        protected internal virtual ICollection<IDmnDecisionEvaluationListener> DefaultDmnDecisionEvaluationListeners
        {
            get
            {
                IList<IDmnDecisionEvaluationListener> defaultListeners = new List<IDmnDecisionEvaluationListener>();

                if (engineMetricCollector_Renamed is IDmnDecisionEvaluationListener)
                    defaultListeners.Add((IDmnDecisionEvaluationListener) engineMetricCollector_Renamed);
                else
                    defaultListeners.Add(new DmnEngineMetricCollectorWrapper(engineMetricCollector_Renamed));

                return defaultListeners;
            }
        }

        public override IDmnEngineMetricCollector EngineMetricCollector
        {
            get { return engineMetricCollector_Renamed; }
            set { engineMetricCollector_Renamed = value; }
        }

        public override IList<IDmnDecisionTableEvaluationListener> CustomPreDecisionTableEvaluationListeners
        {
            get { return customPreDecisionTableEvaluationListeners_Renamed; }
            set { customPreDecisionTableEvaluationListeners_Renamed = value; }
        }

        public override IList<IDmnDecisionTableEvaluationListener> CustomPostDecisionTableEvaluationListeners
        {
            get { return customPostDecisionTableEvaluationListeners_Renamed; }
            set { customPostDecisionTableEvaluationListeners_Renamed = value; }
        }

        public override IList<IDmnDecisionEvaluationListener> CustomPreDecisionEvaluationListeners
        {
            get { return customPreDecisionEvaluationListeners_Renamed; }
            set { customPreDecisionEvaluationListeners_Renamed = value; }
        }

        public override IList<IDmnDecisionEvaluationListener> CustomPostDecisionEvaluationListeners
        {
            get { return customPostDecisionEvaluationListeners_Renamed; }
            set { customPostDecisionEvaluationListeners_Renamed = value; }
        }

        /// <summary>
        ///     The list of decision table evaluation listeners of the configuration. Contains
        ///     the pre, default and post decision table evaluation listeners. Is set during
        ///     the build of an engine.
        /// </summary>
        /// <returns> the list of decision table evaluation listeners </returns>
        public virtual IList<IDmnDecisionTableEvaluationListener> DecisionTableEvaluationListeners
        {
            get { return decisionTableEvaluationListeners; }
        }

        /// <summary>
        ///     The list of decision evaluation listeners of the configuration. Contains
        ///     the pre, default and post decision evaluation listeners. Is set during
        ///     the build of an engine.
        /// </summary>
        /// <returns> the list of decision table evaluation listeners </returns>
        public virtual IList<IDmnDecisionEvaluationListener> DecisionEvaluationListeners
        {
            get { return decisionEvaluationListeners; }
        }

        /// <returns> the script engine resolver </returns>
        public virtual IDmnScriptEngineResolver ScriptEngineResolver
        {
            get { return scriptEngineResolver_Renamed; }
            set { scriptEngineResolver_Renamed = value; }
        }

        /// <returns> the el provider </returns>
        public virtual IELProvider ElProvider
        {
            get { return elProvider_Renamed; }
            set { elProvider_Renamed = value; }
        }

        /// <returns> the factory is used to create a <seealso cref="FeelEngine" /> </returns>
        public virtual IFeelEngineFactory FeelEngineFactory
        {
            get { return feelEngineFactory_Renamed; }
            set
            {
                feelEngineFactory_Renamed = value;
                feelEngine = null; // clear cached FEEL engine
            }
        }

        ///// <summary>
        /////     The feel engine used by the engine. Is initialized during the build of
        /////     the engine.
        ///// </summary>
        ///// <returns> the feel engine </returns>
        public virtual IFeelEngine FeelEngine
        {
            get { return feelEngine; }
        }

        /// <returns> the default expression language for input expressions </returns>
        public virtual string DefaultInputExpressionExpressionLanguage
        {
            get { return defaultInputExpressionExpressionLanguage_Renamed; }
            set { defaultInputExpressionExpressionLanguage_Renamed = value; }
        }

        /// <returns> the default expression language for input entries </returns>
        public virtual string DefaultInputEntryExpressionLanguage
        {
            get { return defaultInputEntryExpressionLanguage_Renamed; }
            set { defaultInputEntryExpressionLanguage_Renamed = value; }
        }

        /// <returns> the default expression language for output entries </returns>
        public virtual string DefaultOutputEntryExpressionLanguage
        {
            get { return defaultOutputEntryExpressionLanguage_Renamed; }
            set { defaultOutputEntryExpressionLanguage_Renamed = value; }
        }

        /// <returns> the default expression language for literal expressions </returns>
        public virtual string DefaultLiteralExpressionLanguage
        {
            get { return defaultLiteralExpressionLanguage_Renamed; }
            set { defaultLiteralExpressionLanguage_Renamed = value; }
        }

        /// <returns> the DMN transformer </returns>
        public virtual IDmnTransformer Transformer
        {
            get { return transformer_Renamed; }
            set { transformer_Renamed = value; }
        }

        public override IDmnEngine buildEngine()
        {
            init();
            return new DefaultDmnEngine(this);
        }

        public virtual void init()
        {
            initMetricCollector();
            initDecisionTableEvaluationListener();
            initDecisionEvaluationListener();
            InitScriptEngineResolver();
            InitElProvider();
            InitFeelEngine();
        }

        protected internal virtual void initMetricCollector()
        {
            if (engineMetricCollector_Renamed == null)
                engineMetricCollector_Renamed = new DefaultEngineMetricCollector();
        }

        protected internal virtual void initDecisionTableEvaluationListener()
        {
            IList<IDmnDecisionTableEvaluationListener> listeners = new List<IDmnDecisionTableEvaluationListener>();
            if ((customPreDecisionTableEvaluationListeners_Renamed != null) &&
                (customPreDecisionTableEvaluationListeners_Renamed.Count > 0))
                ((List<IDmnDecisionTableEvaluationListener>) listeners).AddRange(
                    customPreDecisionTableEvaluationListeners_Renamed);

            if ((customPostDecisionTableEvaluationListeners_Renamed != null) &&
                (customPostDecisionTableEvaluationListeners_Renamed.Count > 0))
                ((List<IDmnDecisionTableEvaluationListener>) listeners).AddRange(
                    customPostDecisionTableEvaluationListeners_Renamed);
            decisionTableEvaluationListeners = listeners;
        }

        protected internal virtual void initDecisionEvaluationListener()
        {
            IList<IDmnDecisionEvaluationListener> listeners = new List<IDmnDecisionEvaluationListener>();
            if ((customPreDecisionEvaluationListeners_Renamed != null) &&
                (customPreDecisionEvaluationListeners_Renamed.Count > 0))
                ((List<IDmnDecisionEvaluationListener>) listeners).AddRange(customPreDecisionEvaluationListeners_Renamed);

            ((List<IDmnDecisionEvaluationListener>) listeners).AddRange(DefaultDmnDecisionEvaluationListeners);

            if ((customPostDecisionEvaluationListeners_Renamed != null) &&
                (customPostDecisionEvaluationListeners_Renamed.Count > 0))
                ((List<IDmnDecisionEvaluationListener>) listeners).AddRange(customPostDecisionEvaluationListeners_Renamed);
            decisionEvaluationListeners = listeners;
        }

        protected internal virtual void InitElProvider()
        {
            if (elProvider_Renamed == null)
            {
                elProvider_Renamed = new JuelElProvider();
            }
        }

        protected internal virtual void InitScriptEngineResolver()
        {
            if (scriptEngineResolver_Renamed == null)
            {
                //scriptEngineResolver_Renamed = new DefaultScriptEngineResolver();
            }
        }

        protected internal virtual void InitFeelEngine()
        {
            if (feelEngineFactory_Renamed == null)
            {
                feelEngineFactory_Renamed = new FeelEngineFactoryImpl();
            }

            if (feelEngine == null)
            {
                feelEngine = feelEngineFactory_Renamed.CreateInstance();
            }
        }


        public override DmnEngineConfiguration engineMetricCollector(IDmnEngineMetricCollector engineMetricCollector)
        {
            EngineMetricCollector = engineMetricCollector;
            return this;
        }


        public override DmnEngineConfiguration customPreDecisionTableEvaluationListeners(
            IList<IDmnDecisionTableEvaluationListener> decisionTableEvaluationListeners)
        {
            CustomPreDecisionTableEvaluationListeners = decisionTableEvaluationListeners;
            return this;
        }


        public override DmnEngineConfiguration customPostDecisionTableEvaluationListeners(
            IList<IDmnDecisionTableEvaluationListener> decisionTableEvaluationListeners)
        {
            CustomPostDecisionTableEvaluationListeners = decisionTableEvaluationListeners;
            return this;
        }


        public override DmnEngineConfiguration customPreDecisionEvaluationListeners(
            IList<IDmnDecisionEvaluationListener> decisionEvaluationListeners)
        {
            CustomPreDecisionEvaluationListeners = decisionEvaluationListeners;
            return this;
        }


        public override DmnEngineConfiguration customPostDecisionEvaluationListeners(
            IList<IDmnDecisionEvaluationListener> decisionEvaluationListeners)
        {
            CustomPostDecisionEvaluationListeners = decisionEvaluationListeners;
            return this;
        }


        /// <summary>
        ///     Set the script engine resolver which is used by the engine to get
        ///     an instance of a script engine to evaluated expressions.
        /// </summary>
        /// <param name="scriptEngineResolver"> the script engine resolver </param>
        /// <returns> this </returns>
        public virtual DefaultDmnEngineConfiguration scriptEngineResolver(IDmnScriptEngineResolver scriptEngineResolver)
        {
            ScriptEngineResolver = scriptEngineResolver;
            return this;
        }


        /// <summary>
        ///     Set the el provider which is used by the engine to
        ///     evaluate an el expression.
        /// </summary>
        /// <param name="elProvider"> the el provider </param>
        /// <returns> this </returns>
        public virtual DefaultDmnEngineConfiguration elProvider(IELProvider elProvider)
        {
            ElProvider = elProvider;
            return this;
        }


        /// <summary>
        ///     Set the factory to create a <seealso cref="FeelEngine" />
        /// </summary>
        /// <param name="feelEngineFactory"> the feel engine factory </param>
        /// <returns> this </returns>
        //public virtual DefaultDmnEngineConfiguration feelEngineFactory(FeelEngineFactory feelEngineFactory)
        //{
        //    FeelEngineFactory = feelEngineFactory;
        //    return this;
        //}


        /// <summary>
        ///     Set the default expression language which is used to evaluate input expressions.
        ///     It is used for all input expressions which do not have a expression
        ///     language set.
        /// </summary>
        /// <param name="expressionLanguage"> the default expression language for input expressions </param>
        /// <returns> this configuration </returns>
        public virtual DefaultDmnEngineConfiguration defaultInputExpressionExpressionLanguage(string expressionLanguage)
        {
            DefaultInputExpressionExpressionLanguage = expressionLanguage;
            return this;
        }


        /// <summary>
        ///     Set the default expression language which is used to evaluate input entries.
        ///     It is used for all input entries which do not have a expression
        ///     language set.
        /// </summary>
        /// <param name="expressionLanguage"> the default expression language for input entries </param>
        /// <returns> this configuration </returns>
        public virtual DefaultDmnEngineConfiguration defaultInputEntryExpressionLanguage(string expressionLanguage)
        {
            DefaultInputEntryExpressionLanguage = expressionLanguage;
            return this;
        }


        /// <summary>
        ///     Set the default expression language which is used to evaluate output entries.
        ///     It is used for all output entries which do not have a expression
        ///     language set.
        /// </summary>
        /// <param name="expressionLanguage"> the default expression language for output entries </param>
        /// <returns> this configuration </returns>
        public virtual DefaultDmnEngineConfiguration defaultOutputEntryExpressionLanguage(string expressionLanguage)
        {
            DefaultOutputEntryExpressionLanguage = expressionLanguage;
            return this;
        }


        /// <summary>
        ///     Set the default expression language which is used to evaluate literal expressions.
        ///     It is used for all literal expressions which do not have a expression
        ///     language set.
        /// </summary>
        /// <param name="expressionLanguage"> the default expression language for literal expressions </param>
        /// <returns> this configuration </returns>
        public virtual DefaultDmnEngineConfiguration defaultLiteralExpressionLanguage(string expressionLanguage)
        {
            DefaultLiteralExpressionLanguage = expressionLanguage;
            return this;
        }


        /// <summary>
        ///     Set the DMN transformer used to transform the DMN model.
        /// </summary>
        /// <param name="transformer"> the DMN transformer </param>
        /// <returns> this </returns>
        public virtual DefaultDmnEngineConfiguration transformer(IDmnTransformer transformer)
        {
            Transformer = transformer;
            return this;
        }
    }
}