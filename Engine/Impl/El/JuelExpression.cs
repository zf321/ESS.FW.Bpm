using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Impl.EL
{
    /// <summary>
    ///     Expression implementation backed by a Antlr <seealso cref="Javax.EL.ValueExpression" />.
    ///     
    ///     
    /// </summary>
    public class JuelExpression : IExpression
    {
        protected internal ExpressionManager ExpressionManager;

        protected internal ValueExpression ValueExpression;

        public JuelExpression(ValueExpression valueExpression, ExpressionManager expressionManager,
            string expressionText)
        {
            this.ValueExpression = valueExpression;
            this.ExpressionManager = expressionManager;
            this.ExpressionText = expressionText;
        }

        public virtual object GetValue(IVariableScope variableScope)
        {
            //TODO 表达式相关
            return GetValue(variableScope, null);
        }

        public virtual object GetValue(IVariableScope variableScope, IBaseDelegateExecution contextExecution)
        {
            var elContext = ExpressionManager.GetElContext(variableScope);
            try
            {
                var invocation = new ExpressionGetInvocation(ValueExpression, elContext, contextExecution);
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
                return invocation.InvocationResult;
            }
            catch (PropertyNotFoundException pnfe)
            {
                throw new ProcessEngineException(
                    "Unknown property used in expression: " + ExpressionText + ". Cause: " + pnfe.Message, pnfe);
            }
            catch (MethodNotFoundException mnfe)
            {
                throw new ProcessEngineException(
                    "Unknown method used in expression: " + ExpressionText + ". Cause: " + mnfe.Message, mnfe);
            }
            catch (ELException ele)
            {
                throw new ProcessEngineException(
                    "Error while evaluating expression: " + ExpressionText + ". Cause: " + ele.Message, ele);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(
                    "Error while evaluating expression: " + ExpressionText + ". Cause: " + e.Message, e);
            }
        }

        public virtual void SetValue(object value, IVariableScope variableScope)
        {
            SetValue(value, variableScope, null);
        }

        public virtual void SetValue(object value, IVariableScope variableScope, IBaseDelegateExecution contextExecution)
        {
            var elContext = ExpressionManager.GetElContext(variableScope);
            try
            {
                var invocation = new ExpressionSetInvocation(ValueExpression, elContext, value, contextExecution);
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(invocation);
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException(
                    "Error while evaluating expression: " + ExpressionText + ". Cause: " + e.Message, e);
            }
        }

        public virtual bool LiteralText => ValueExpression.IsLiteralText;

        public virtual string ExpressionText { get; }

        public override string ToString()
        {
            if (ValueExpression != null)
                return ValueExpression.ExpressionString;
            return base.ToString();
        }
        
    }
}