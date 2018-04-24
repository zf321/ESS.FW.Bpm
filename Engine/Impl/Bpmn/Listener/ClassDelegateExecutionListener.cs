using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Listener
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ClassDelegateExecutionListener : ClassDelegate, IDelegateListener<IBaseDelegateExecution>
    {
        protected internal static readonly BpmnBehaviorLogger Log = ProcessEngineLogger.BpmnBehaviorLogger;

        public ClassDelegateExecutionListener(string className, IList<FieldDeclaration> fieldDeclarations)
            : base(className, fieldDeclarations)
        {
        }

        public ClassDelegateExecutionListener(Type clazz, IList<FieldDeclaration> fieldDeclarations)
            : base(clazz, fieldDeclarations)
        {
        }

        protected internal virtual IDelegateListener<IBaseDelegateExecution> ExecutionListenerInstance
        {
            get
            {
                object delegateInstance = ClassDelegateUtil.InstantiateDelegate(className, fieldDeclarations);
                if (delegateInstance is IDelegateListener<IBaseDelegateExecution>)
                {
                    return (IDelegateListener<IBaseDelegateExecution>)delegateInstance;
                }
                if (delegateInstance is IJavaDelegate)
                {
                    return new ServiceTaskJavaDelegateActivityBehavior((IJavaDelegate)delegateInstance);
                }
                throw Log.MissingDelegateParentClassException(delegateInstance.GetType().FullName,
                    typeof (IDelegateListener<IBaseDelegateExecution>).FullName, typeof (IJavaDelegate).FullName);
            }
        }

        // Execution listener
        public virtual void Notify(IBaseDelegateExecution execution)
        {
            var executionListenerInstance = ExecutionListenerInstance;

            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                new ExecutionListenerInvocation(executionListenerInstance, (IDelegateExecution) execution));
        }
    }
}