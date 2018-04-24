using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     <seealso cref="IActivityBehavior" /> used when 'delegateExpression' is used
    ///     for a serviceTask.
    /// </summary>
    public class ServiceTaskDelegateExpressionActivityBehavior : TaskActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;
        private readonly IList<FieldDeclaration> _fieldDeclarations;

        private readonly IExpression _expression;

        public ServiceTaskDelegateExpressionActivityBehavior(IExpression expression,
            IList<FieldDeclaration> fieldDeclarations)
        {
            _expression = expression;
            _fieldDeclarations = fieldDeclarations;
        }

        public override void Signal(IActivityExecution execution, string signalName, object signalData)
        {
            var targetProcessApplication =
                ProcessApplicationContextUtil.GetTargetProcessApplication((ExecutionEntity) execution);
            if (ProcessApplicationContextUtil.RequiresContextSwitch(targetProcessApplication))
                Context.ExecuteWithinProcessApplication<object>(() =>
                    {
                        Signal(execution, signalName, signalData);
                        return null;
                    }, targetProcessApplication,
                    new InvocationContext(execution));
            else
                DoSignal(execution, signalName, signalData);
        }

        public virtual void DoSignal(IActivityExecution execution, string signalName, object signalData)
        {
            var @delegate = _expression.GetValue(execution);
            ClassDelegateUtil.ApplyFieldDeclaration(_fieldDeclarations, @delegate);
            var activityBehaviorInstance = GetActivityBehaviorInstance(execution, @delegate);

            if (activityBehaviorInstance is CustomActivityBehavior)
            {
                var behavior = (CustomActivityBehavior) activityBehaviorInstance;
                var delegateActivityBehavior = behavior.DelegateActivityBehavior;

                if (!(delegateActivityBehavior is ISignallableActivityBehavior))
                {
                    // legacy behavior: do nothing when it is not a signallable activity behavior
                }
            }
            ExecuteWithErrorPropagation(execution,
                () =>
                    ((ISignallableActivityBehavior) activityBehaviorInstance).Signal(execution, signalName, signalData));
        }


        protected override void PerformExecution(IActivityExecution execution)
        {
            ExecuteWithErrorPropagation(execution,
                () =>
                {
                    // Note: we can't cache the result of the expression, because the
                    // execution can change: eg. delegateExpression='${mySpringBeanFactory.randomSpringBean()}'
                    var @delegate = _expression.GetValue(execution);
                    if(@delegate is ESS.FW.Bpm.Engine.Variable.Value.Impl.UntypedValueImpl)
                    {
                        var del = (Engine.Variable.Value.Impl.UntypedValueImpl)@delegate;
                        var r = del.Value;
                        ClassDelegateUtil.ApplyFieldDeclaration(_fieldDeclarations, r);

                        if (r is IActivityBehavior)
                        {
                            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                                new ActivityBehaviorInvocation((IActivityBehavior)r, execution));
                        }
                        else if (r is IJavaDelegate)
                        {
                            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                                new JavaDelegateInvocation((IJavaDelegate)r, execution));
                            Leave(execution);
                        }
                        else
                        {
                            throw Log.ResolveDelegateExpressionException(_expression, typeof(IActivityBehavior),
                                typeof(IJavaDelegate));
                        }
                        return;
                    }



                    ClassDelegateUtil.ApplyFieldDeclaration(_fieldDeclarations, @delegate);

                    if (@delegate is IActivityBehavior)
                    {
                        Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                            new ActivityBehaviorInvocation((IActivityBehavior) @delegate, execution));
                    }
                    else if (@delegate is IJavaDelegate)
                    {
                        Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                            new JavaDelegateInvocation((IJavaDelegate) @delegate, execution));
                        Leave(execution);
                    }
                    else
                    {
                        throw Log.ResolveDelegateExpressionException(_expression, typeof(IActivityBehavior),
                            typeof(IJavaDelegate));
                    }
                }
            );
        }


        protected internal virtual IActivityBehavior GetActivityBehaviorInstance(IActivityExecution execution,
            object delegateInstance)
        {
            if (delegateInstance is IActivityBehavior)
                return new CustomActivityBehavior((IActivityBehavior) delegateInstance);
            if (delegateInstance is IJavaDelegate)
                return new ServiceTaskJavaDelegateActivityBehavior((IJavaDelegate) delegateInstance);
            throw Log.MissingDelegateParentClassException(delegateInstance.GetType()
                    .FullName,
                typeof(IJavaDelegate).FullName, typeof(IActivityBehavior).FullName);
        }
        
    }
}