using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;

namespace ESS.FW.Bpm.Engine.Dmn.Impl.EL
{
    /// <summary>
    ///     
    /// </summary>
    public class ProcessEngineElExpression : IELExpression
    {
        protected internal readonly ValueExpression ValueExpression;

        public ProcessEngineElExpression(ValueExpression expression)
        {
            ValueExpression = expression;
        }

        public virtual object GetValue(IVariableContext variableContext)
        {
            if (Context.CommandContext == null)
                throw new ProcessEngineException(
                    "Expression can only be evaluated inside the context of the process engine");

            var context = Context.ProcessEngineConfiguration.ExpressionManager.CreateElContext(variableContext);

            return ValueExpression.GetValue(context);
        }
    }
}