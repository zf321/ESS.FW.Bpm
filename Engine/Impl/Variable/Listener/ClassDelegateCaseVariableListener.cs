using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{

    /// <summary>
    ///     
    /// </summary>
    public class ClassDelegateCaseVariableListener : ClassDelegate, ICaseVariableListener
    {
        public ClassDelegateCaseVariableListener(string className, IList<FieldDeclaration> fieldDeclarations)
            : base(className, fieldDeclarations)
        {
        }

        public ClassDelegateCaseVariableListener(Type clazz, IList<FieldDeclaration> fieldDeclarations)
            : base(clazz, fieldDeclarations)
        {
        }

        protected internal virtual ICaseVariableListener VariableListenerInstance
        {
            get
            {
                object delegateInstance = ClassDelegateUtil.InstantiateDelegate(className, fieldDeclarations);
                if (delegateInstance is ICaseVariableListener)
                {
                    return (ICaseVariableListener)delegateInstance;
                }
                throw new ProcessEngineException(delegateInstance.GetType().FullName + " doesn't implement " +
                                                 typeof(ICaseVariableListener));
            }
        }
        
        public virtual void Notify(IDelegateCaseVariableInstance variableInstance)
        {
            var variableListenerInstance = VariableListenerInstance;

            Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(
                new CaseVariableListenerInvocation(variableListenerInstance, variableInstance));
        }
    }
}