
using ESS.FW.Bpm.Engine.Common.Cache;
using ESS.FW.Bpm.Engine.Dmn.Feel;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El;
using ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.Transform;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;

namespace org.camunda.bpm.dmn.feel.impl.juel
{
    public class FeelEngineImpl : IFeelEngine
    {

        public static readonly FeelEngineLogger LOG = FeelLogger.ENGINE_LOGGER;

        protected internal IFeelToJuelTransform transform;
        protected internal ExpressionFactory expressionFactory;
        protected internal IElContextFactory elContextFactory;
        protected internal ICache<TransformExpressionCacheKey, string> transformExpressionCache;

        public FeelEngineImpl(IFeelToJuelTransform transform, ExpressionFactory expressionFactory, IElContextFactory elContextFactory, ICache<TransformExpressionCacheKey, string> transformExpressionCache)
        {
            this.transform = transform;
            this.expressionFactory = expressionFactory;
            this.elContextFactory = elContextFactory;
            this.transformExpressionCache = transformExpressionCache;
        }

        public virtual T EvaluateSimpleExpression<T>(string simpleExpression, IVariableContext variableContext)
        {
            throw LOG.simpleExpressionNotSupported();
        }

        public virtual bool EvaluateSimpleUnaryTests(string simpleUnaryTests, string inputName, IVariableContext variableContext)
        {
            try
            {
                ELContext elContext = CreateContext(variableContext);
                ValueExpression valueExpression = TransformSimpleUnaryTests(simpleUnaryTests, inputName, elContext);
                return (bool)valueExpression.GetValue(elContext);
            }
            catch (FeelMissingFunctionException e)
            {
                throw LOG.UnknownFunction(simpleUnaryTests, e);
            }
            catch (FeelMissingVariableException e)
            {
                if (inputName.Equals(e.GetVariable()))
                {
                    throw LOG.unableToEvaluateExpressionAsNotInputIsSet(simpleUnaryTests, e);
                }
                else
                {
                    throw LOG.unknownVariable(simpleUnaryTests, e);
                }
            }
            catch (FeelConvertException e)
            {
                throw LOG.unableToConvertValue(simpleUnaryTests, e);
            }
            catch (ELException e)
            {
                if (e.InnerException is FeelMethodInvocationException)
                {
                    throw LOG.unableToInvokeMethod(simpleUnaryTests, (FeelMethodInvocationException)e.InnerException);
                }
                else
                {
                    throw LOG.unableToEvaluateExpression(simpleUnaryTests, e);
                }
            }
        }

        protected internal virtual ELContext CreateContext(IVariableContext variableContext)
        {
            return elContextFactory.CreateContext(expressionFactory, variableContext);
        }

        protected internal virtual ValueExpression TransformSimpleUnaryTests(string simpleUnaryTests, string inputName, ELContext elContext)
        {

            string juelExpression = transformToJuelExpression(simpleUnaryTests, inputName);

            try
            {
                return expressionFactory.CreateValueExpression(elContext, juelExpression, typeof(object));
            }
            catch (ELException e)
            {
                throw LOG.invalidExpression(simpleUnaryTests, e);
            }
        }

        protected internal virtual string transformToJuelExpression(string simpleUnaryTests, string inputName)
        {

            TransformExpressionCacheKey cacheKey = new TransformExpressionCacheKey(simpleUnaryTests, inputName);
            string juelExpression = transformExpressionCache.Get(cacheKey);

            if (juelExpression == null)
            {
                juelExpression = transform.TransformSimpleUnaryTests(simpleUnaryTests, inputName);
                transformExpressionCache.Put(cacheKey, juelExpression);
            }
            return juelExpression;
        }

    }
}