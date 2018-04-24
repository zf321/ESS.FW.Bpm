using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.task.@delegate;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.task.listener
{

    /// <summary>
    ///     
    /// </summary>
    public class ClassDelegateTaskListener : ClassDelegate, ITaskListener
    {
        public ClassDelegateTaskListener(string className, IList<FieldDeclaration> fieldDeclarations)
            : base(className, fieldDeclarations)
        {
        }

        public ClassDelegateTaskListener(Type clazz, IList<FieldDeclaration> fieldDeclarations)
            : base(clazz, fieldDeclarations)
        {
        }

        protected internal virtual ITaskListener TaskListenerInstance
        {
            get
            {
                object delegateInstance = ClassDelegateUtil.InstantiateDelegate(className, fieldDeclarations);

                if (delegateInstance is ITaskListener)
                {
                    return (ITaskListener)delegateInstance;
                }
                throw new ProcessEngineException(delegateInstance.GetType().FullName + " doesn't implement " +
                                                 typeof(ITaskListener));
            }
        }

        public virtual void Notify(IDelegateTask delegateTask)
        {
            var taskListenerInstance = TaskListenerInstance;
            try
            {
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                    new TaskListenerInvocation(taskListenerInstance, delegateTask));
            }
            catch (System.Exception e)
            {
                throw new ProcessEngineException("Exception while invoking TaskListener: " + e.Message, e);
            }
        }
    }
}