using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Listener
{
    /// <summary>
    /// </summary>
    public class DelegateExpressionExecutionListener : IDelegateListener<IBaseDelegateExecution>
    {
        protected internal static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        private readonly IList<FieldDeclaration> _fieldDeclarations;

        protected internal IExpression Expression;

        public DelegateExpressionExecutionListener(IExpression expression, IList<FieldDeclaration> fieldDeclarations)
        {
            this.Expression = expression;
            this._fieldDeclarations = fieldDeclarations;
        }

        /// <summary>
        ///     returns the expression text for this execution listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }
        
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            // Note: we can't cache the result of the expression, because the
            // execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
            var @delegate = Expression.GetValue(execution);
            ClassDelegateUtil.ApplyFieldDeclaration(_fieldDeclarations, @delegate);

            if (@delegate is IDelegateListener<IBaseDelegateExecution>)
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                    new ExecutionListenerInvocation((IDelegateListener<IBaseDelegateExecution>) @delegate, (IDelegateExecution) execution));
            else if (@delegate is IJavaDelegate)
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                    new JavaDelegateInvocation((IJavaDelegate) @delegate, (IDelegateExecution) execution));
            else
                throw Log.ResolveDelegateExpressionException(Expression, typeof(IDelegateListener<IBaseDelegateExecution>),
                    typeof(IJavaDelegate));
        }
    }
}