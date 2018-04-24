using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.task.@delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.task.listener
{

    /// <summary>
    ///     
    /// </summary>
    public class DelegateExpressionTaskListener : ITaskListener
    {
        protected internal IExpression Expression;

        public DelegateExpressionTaskListener(IExpression expression, IList<FieldDeclaration> fieldDeclarations)
        {
            this.Expression = expression;
            FieldDeclarations = fieldDeclarations;
        }

        /// <summary>
        ///     returns the expression text for this task listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }

        public virtual IList<FieldDeclaration> FieldDeclarations { get; }

        public virtual void Notify(IDelegateTask delegateTask)
        {
            // Note: we can't cache the result of the expression, because the
            // execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'

            IVariableScope variableScope = delegateTask.Execution;
            if (variableScope == null)
                variableScope = delegateTask.CaseExecution;

            var @delegate = Expression.GetValue(variableScope);
            ClassDelegateUtil.ApplyFieldDeclaration(FieldDeclarations, @delegate);

            if (@delegate is ITaskListener)
                try
                {
                    Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                        new TaskListenerInvocation((ITaskListener) @delegate, delegateTask));
                }
                catch (System.Exception e)
                {
                    throw new ProcessEngineException("Exception while invoking TaskListener: " + e.Message, e);
                }
            else
                throw new ProcessEngineException("Delegate expression " + Expression +
                                                 " did not resolve to an implementation of " + typeof(ITaskListener));
        }
    }
}