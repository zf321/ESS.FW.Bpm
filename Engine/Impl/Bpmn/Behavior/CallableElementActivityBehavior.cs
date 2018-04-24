using System;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Model;
using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior
{

    /// <summary>
    ///     
    /// </summary>
    public abstract class CallableElementActivityBehavior : AbstractBpmnActivityBehavior,ISubProcessActivityBehavior
    {
        protected internal CallableElement callableElement;

        /// <summary>
        ///     The class name of the delegated variable mapping, which should be used.
        /// </summary>
        protected internal string ClassName;

        /// <summary>
        ///     The expression which identifies the delegation for the variable mapping.
        /// </summary>
        protected internal IExpression Expression;

        protected internal string[] VariablesFilter =
        {
            MultiInstanceActivityBehavior.NumberOfInstances,
            MultiInstanceActivityBehavior.NumberOfActiveInstances,
            MultiInstanceActivityBehavior.NumberOfCompletedInstances
        };

        public CallableElementActivityBehavior()
        {
        }

        public CallableElementActivityBehavior(string className)
        {
            this.ClassName = className;
        }

        public CallableElementActivityBehavior(IExpression expression)
        {
            this.Expression = expression;
        }

        public virtual CallableElement CallableElement
        {
            get { return callableElement; }
            set { callableElement = value; }
        }

        protected internal virtual BaseCallableElement.CallableElementBinding Binding
        {
            get { return CallableElement.Binding; }
        }

        protected internal virtual bool LatestBinding
        {
            get { return CallableElement.LatestBinding; }
        }

        protected internal virtual bool DeploymentBinding
        {
            get { return CallableElement.DeploymentBinding; }
        }

        protected internal virtual bool VersionBinding
        {
            get { return CallableElement.VersionBinding; }
        }

        protected internal virtual IDelegateVariableMapping GetDelegateVariableMapping(object instance)
        {
            if (instance is IDelegateVariableMapping)
                return (IDelegateVariableMapping) instance;
            throw Log.MissingDelegateVariableMappingParentClassException(instance.GetType().FullName,
                typeof(IDelegateVariableMapping).FullName);
        }

        protected internal virtual object InstantiateDelegateClass(IActivityExecution execution)
        {
            object @delegate = null;
            if (Expression != null)
                @delegate = Expression.GetValue(execution);
            else if (!ReferenceEquals(ClassName, null))
                @delegate = ClassDelegateUtil.InstantiateDelegate(ClassName, null);
            return @delegate;
        }
        
        public override void Execute(IActivityExecution execution)
        {
            var variables = GetInputVariables(execution);
            
            IDelegateVariableMapping varMapping = ResolveDelegation(execution);
            if (varMapping != null)
            {
                InvokeVarMappingDelegation(new DelegateInvocationAnonymousInnerClass(this, execution, variables, varMapping));
            }

            var businessKey = GetBusinessKey(execution);
            StartInstance(execution, variables, businessKey);
        }
        
        public virtual void PassOutputVariables(IActivityExecution execution, IVariableScope subInstance)
        {
            // only data. no control flow available on this execution.
            IVariableMap variables = FilterVariables(GetOutputVariables(subInstance));
            var localVariables = GetOutputVariablesLocal(subInstance);

            execution.Variables = variables;
            execution.VariablesLocal = localVariables;

            IDelegateVariableMapping varMapping = ResolveDelegation(execution);
            if (varMapping != null)
            {
                InvokeVarMappingDelegation(new DelegateInvocationAnonymousInnerClass2(this, execution, subInstance, varMapping));
            }
        }

        protected internal virtual void InvokeVarMappingDelegation(DelegateInvocation delegation)
        {
            try
            {
                Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(delegation);
            }
            catch (System.Exception ex)
            {
                throw new ProcessEngineException(ex);
            }
        }

        protected internal virtual IVariableMap FilterVariables(IVariableMap variables)
        {
            if (variables != null)
                foreach (var key in VariablesFilter)
                    variables.Remove(key);
            return variables;
        }
        
        public virtual void Completed(IActivityExecution execution)
        {
            // only control flow. no sub instance data available
            Leave(execution);
        }


        protected internal virtual string GetBusinessKey(IActivityExecution execution)
        {
            return CallableElement.GetBusinessKey(execution);
        }

        protected internal virtual IVariableMap GetInputVariables(IActivityExecution callingExecution)
        {
            return CallableElement.GetInputVariables(callingExecution);
        }

        protected internal virtual IVariableMap GetOutputVariables(IVariableScope calledElementScope)
        {
            return CallableElement.GetOutputVariables(calledElementScope);
        }

        protected internal virtual IVariableMap GetOutputVariablesLocal(IVariableScope calledElementScope)
        {
            return CallableElement.GetOutputVariablesLocal(calledElementScope);
        }

        protected internal virtual int? GetVersion(IActivityExecution execution)
        {
            return CallableElement.GetVersion(execution);
        }

        protected internal virtual string GetDeploymentId(IActivityExecution execution)
        {
            return CallableElement.DeploymentId;
        }

        protected internal abstract void StartInstance(IActivityExecution execution, IVariableMap variables,
            string businessKey);

        protected internal virtual IDelegateVariableMapping ResolveDelegation(IActivityExecution execution)
        {
            object @delegate = ResolveDelegateClass(execution);
            return @delegate != null ? GetDelegateVariableMapping(@delegate) : null;
        }
        
        public virtual object ResolveDelegateClass(IActivityExecution execution)
        {
            IProcessApplicationReference targetProcessApplication = ProcessApplicationContextUtil.GetTargetProcessApplication((ExecutionEntity)execution);
            if (ProcessApplicationContextUtil.RequiresContextSwitch(targetProcessApplication))
            {
                return Context.ExecuteWithinProcessApplication(
                    () => ResolveDelegateClass(execution), targetProcessApplication,
                    new InvocationContext(execution));
            }
            else
            {
                return InstantiateDelegateClass(execution);
            }
        }
        

        private class DelegateInvocationAnonymousInnerClass : DelegateInvocation
        {
            private readonly CallableElementActivityBehavior _outerInstance;

            private readonly IActivityExecution _execution;
            private readonly IVariableMap _variables;
            private readonly IDelegateVariableMapping _varMapping;

            public DelegateInvocationAnonymousInnerClass(CallableElementActivityBehavior outerInstance,
                IActivityExecution execution, IVariableMap variables, IDelegateVariableMapping varMapping)
                : base(execution, null)
            {
                this._outerInstance = outerInstance;
                this._execution = execution;
                this._variables = variables;
                this._varMapping = varMapping;
            }
            
            protected internal override void Invoke()
            {
                _varMapping.MapInputVariables(_execution, _variables);
            }
        }

        private class DelegateInvocationAnonymousInnerClass2 : DelegateInvocation
        {
            private readonly CallableElementActivityBehavior _outerInstance;

            private readonly IActivityExecution _execution;
            private readonly IVariableScope _subInstance;
            private readonly IDelegateVariableMapping _varMapping;

            public DelegateInvocationAnonymousInnerClass2(CallableElementActivityBehavior outerInstance,
                IActivityExecution execution, IVariableScope subInstance, IDelegateVariableMapping varMapping)
                : base(execution, null)
            {
                this._outerInstance = outerInstance;
                this._execution = execution;
                this._subInstance = subInstance;
                this._varMapping = varMapping;
            }
            
            protected internal override void Invoke()
            {
                _varMapping.MapOutputVariables(_execution, _subInstance);
            }
        }
    }
}