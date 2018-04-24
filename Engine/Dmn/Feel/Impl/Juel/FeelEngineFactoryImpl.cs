using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using org.camunda.bpm.dmn.feel.impl.juel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelEngineFactoryImpl: IFeelEngineFactory
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        public const int DEFAULT_EXPRESSION_CACHE_SIZE = 1000;
        protected internal readonly IFeelEngine feelEngine;
        protected internal readonly int expressionCacheSize;
        protected internal readonly IList<IFeelToJuelFunctionTransformer> customFunctionTransformers;

        public FeelEngineFactoryImpl() : this(1000)
        {
        }

        public FeelEngineFactoryImpl(int expressionCacheSize) : this(expressionCacheSize, new List<IFeelToJuelFunctionTransformer>())
        {
        }

        public FeelEngineFactoryImpl(IList<IFeelToJuelFunctionTransformer> customFunctionTransformers) : this(1000, customFunctionTransformers)
        {
        }

        public FeelEngineFactoryImpl(int expressionCacheSize, IList<IFeelToJuelFunctionTransformer> customFunctionTransformers)
        {
            this.expressionCacheSize = expressionCacheSize;
            this.customFunctionTransformers = customFunctionTransformers;
            this.feelEngine = this.CreateFeelEngine();
        }

        public virtual IFeelEngine CreateInstance()
        {
            return this.feelEngine;
        }

        protected internal virtual IFeelEngine CreateFeelEngine()
        {
            IFeelToJuelTransform transform = this.CreateFeelToJuelTransform();
            ExpressionFactory expressionFactory = this.CreateExpressionFactory();
            IElContextFactory elContextFactory = this.CreateElContextFactory();
            ICache<TransformExpressionCacheKey, string> transformExpressionCache = this.CreateTransformExpressionCache();
            return new FeelEngineImpl(transform, expressionFactory, elContextFactory, transformExpressionCache);
        }

        protected internal virtual IFeelToJuelTransform CreateFeelToJuelTransform()
        {
            FeelToJuelTransformImpl transformer = new FeelToJuelTransformImpl();
            var i = this.customFunctionTransformers.GetEnumerator();

            while (i.MoveNext())
			{
                IFeelToJuelFunctionTransformer functionTransformer = (IFeelToJuelFunctionTransformer)i.Current;
                transformer.AddCustomFunctionTransformer(functionTransformer);
            }

            return transformer;
        }

        protected internal virtual ExpressionFactory CreateExpressionFactory()
        {
            Bpm.Engine.Impl.Core.Model.Properties properties = new Bpm.Engine.Impl.Core.Model.Properties();
            properties.properties.Add("camundafeel.javax.el.cacheSize", Convert.ToString(this.expressionCacheSize));

            try
            {
                return new ExpressionFactoryImpl(properties, this.CreateTypeConverter());
            }
            catch (ELException var3)
            {
                throw LOG.unableToInitializeFeelEngine(var3);
            }
        }

        protected internal virtual FeelTypeConverter CreateTypeConverter()
        {
            return new FeelTypeConverter();
        }

        protected internal virtual IElContextFactory CreateElContextFactory()
        {
            FeelElContextFactory factory = new FeelElContextFactory();
            var i = this.customFunctionTransformers.GetEnumerator();

            while (i.MoveNext())
			{
                IFeelToJuelFunctionTransformer functionTransformer = (IFeelToJuelFunctionTransformer)i.Current;
                factory.AddCustomFunction(functionTransformer.GetName(), functionTransformer.GetMethod());
            }

            return factory;
        }

        protected internal virtual ICache<TransformExpressionCacheKey, string> CreateTransformExpressionCache()
        {
            return new ConcurrentLruCache<TransformExpressionCacheKey, string>(this.expressionCacheSize);
        }


    }
}
