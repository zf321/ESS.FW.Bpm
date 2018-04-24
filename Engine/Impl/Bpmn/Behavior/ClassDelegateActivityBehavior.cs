using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{
    /// <summary>
    ///     Helper class for bpmn constructs that allow class delegation.
    ///     This class will lazily instantiate the referenced classes when needed at runtime.
    /// </summary>
    public class ClassDelegateActivityBehavior : AbstractBpmnActivityBehavior
    {
        protected internal new static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        protected internal string ClassName;
        protected internal IList<FieldDeclaration> FieldDeclarations;

        public ClassDelegateActivityBehavior(string className, IList<FieldDeclaration> fieldDeclarations)
        {
            ClassName = className;
            FieldDeclarations = fieldDeclarations;
        }

        public ClassDelegateActivityBehavior(Type clazz, IList<FieldDeclaration> fieldDeclarations)
            : this(clazz.FullName, fieldDeclarations)
        {
        }

        // Activity Behavior
        public override void Execute(IActivityExecution execution)
        {
            ExecuteWithErrorPropagation(execution, () => GetActivityBehaviorInstance(execution)
                .Execute(execution));
        }

        // Signallable activity behavior
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

        protected internal virtual void DoSignal(IActivityExecution execution, string signalName, object signalData)
        {
            var activityBehaviorInstance = GetActivityBehaviorInstance(execution);

            if (activityBehaviorInstance is CustomActivityBehavior)
            {
                var behavior = (CustomActivityBehavior) activityBehaviorInstance;
                var @delegate = behavior.DelegateActivityBehavior;

                if (!(@delegate is ISignallableActivityBehavior))
                    throw Log.IncorrectlyUsedSignalException(typeof(ISignallableActivityBehavior).FullName);
            }
            ExecuteWithErrorPropagation(execution,
                () =>
                    ((ISignallableActivityBehavior) activityBehaviorInstance).Signal(execution, signalName, signalData));
        }

        protected internal virtual IActivityBehavior GetActivityBehaviorInstance(IActivityExecution execution)
        {
            var delegateInstance = ClassDelegateUtil.InstantiateDelegate(ClassName, FieldDeclarations);

            if (delegateInstance is IActivityBehavior)
                return new CustomActivityBehavior((IActivityBehavior) delegateInstance);
            if (delegateInstance is IJavaDelegate)
                return new ServiceTaskJavaDelegateActivityBehavior((IJavaDelegate) delegateInstance);
            throw Log.MissingDelegateParentClassException(delegateInstance.GetType()
                .FullName, typeof(IJavaDelegate).FullName, typeof(IActivityBehavior).FullName);
        }


    }
}