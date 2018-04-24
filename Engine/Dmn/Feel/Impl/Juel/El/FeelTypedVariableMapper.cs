using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;
using ESS.FW.Bpm.Engine.Variable.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class FeelTypedVariableMapper: VariableMapper
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        protected internal ExpressionFactory expressionFactory;
        protected internal IVariableContext variableContext;

        public FeelTypedVariableMapper(ExpressionFactory expressionFactory, IVariableContext variableContext)
        {
            this.expressionFactory = expressionFactory;
            this.variableContext = variableContext;
        }

        public override ValueExpression ResolveVariable(string variable)
        {
            if (this.variableContext.ContainsVariable(variable))
            {
                object value = this.UnpackVariable(variable);
                return this.expressionFactory.CreateValueExpression(value, typeof(object));
            }
            else
            {
                throw LOG.unknownVariable(variable);
            }
        }

        public override ValueExpression SetVariable(string variable, ValueExpression expression)
        {
            throw LOG.variableMapperIsReadOnly();
        }

        public virtual object UnpackVariable(string variable)
        {
            ITypedValue valueTyped = this.variableContext.Resolve(variable);
            return valueTyped != null ? valueTyped.Value: null;
        }


    }
}
