using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{
    /// <summary>
    ///     
    /// </summary>
    public class CaseVariableListenerInvocation : DelegateInvocation
    {
        protected internal IDelegateCaseVariableInstance VariableInstance;

        protected internal ICaseVariableListener VariableListenerInstance;

        public CaseVariableListenerInvocation(ICaseVariableListener variableListenerInstance,
            IDelegateCaseVariableInstance variableInstance) : this(variableListenerInstance, variableInstance, null)
        {
        }

        public CaseVariableListenerInvocation(ICaseVariableListener variableListenerInstance,
            IDelegateCaseVariableInstance variableInstance, IBaseDelegateExecution contextExecution)
            : base(contextExecution, null)
        {
            this.VariableListenerInstance = variableListenerInstance;
            this.VariableInstance = variableInstance;
        }
        
        protected internal override void Invoke()
        {
            VariableListenerInstance.Notify(VariableInstance);
        }
    }
}