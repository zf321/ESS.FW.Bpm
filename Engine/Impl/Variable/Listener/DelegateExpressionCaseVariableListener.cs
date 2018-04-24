using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{

    /// <summary>
    ///     
    /// </summary>
    public class DelegateExpressionCaseVariableListener : ICaseVariableListener
    {
        protected internal IExpression Expression;

        public DelegateExpressionCaseVariableListener(IExpression expression, IList<FieldDeclaration> fieldDeclarations)
        {
            this.Expression = expression;
            FieldDeclarations = fieldDeclarations;
        }

        /// <summary>
        ///     returns the expression text for this execution listener. Comes in handy if you want to
        ///     check which listeners you already have.
        /// </summary>
        public virtual string ExpressionText
        {
            get { return Expression.ExpressionText; }
        }

        public virtual IList<FieldDeclaration> FieldDeclarations { get; }
        
        public virtual void Notify(IDelegateCaseVariableInstance variableInstance)
        {
            var @delegate = Expression.GetValue(variableInstance.SourceExecution);
            ClassDelegateUtil.ApplyFieldDeclaration(FieldDeclarations, @delegate);

            if (@delegate is ICaseVariableListener)
            {
                var listenerInstance = (ICaseVariableListener) @delegate;
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                    new CaseVariableListenerInvocation(listenerInstance, variableInstance));
            }
            else
            {
                throw new ProcessEngineException("Delegate expression " + Expression +
                                                 " did not resolve to an implementation of " +
                                                 typeof(ICaseVariableListener));
            }
        }
    }
}