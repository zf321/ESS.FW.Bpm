using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Type;
using ESS.FW.Bpm.Engine.Variable.Value;

namespace ESS.FW.Bpm.Engine.Impl.Variable.listener
{
    /// <summary>
    ///     
    /// </summary>
    public class DelegateCaseVariableInstanceImpl : IDelegateCaseVariableInstance
    {
        protected internal string activityInstanceId;
        protected internal string caseExecutionId;
        protected internal string caseInstanceId;
        protected internal string errorMessage;

        protected internal string executionId;
        protected internal string name;
        protected internal string processInstanceId;
        protected internal string taskId;
        protected internal ITypedValue value;

        // fields copied from variable instance
        protected internal string VariableId;
        

        /// <summary>
        ///     Currently not part of public interface.
        /// </summary>
        public virtual IDelegateCaseExecution ScopeExecution { get; set; }

        public virtual string EventName { get; set; }


        public virtual IDelegateCaseExecution SourceExecution { get; set; }


        //// methods delegated to wrapped variable ////

        public virtual string Id
        {
            get { return VariableId; }
        }

        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
        }

        public virtual string ExecutionId
        {
            get { return executionId; }
        }

        public virtual string CaseInstanceId
        {
            get { return caseInstanceId; }
        }

        public virtual string CaseExecutionId
        {
            get { return caseExecutionId; }
        }

        public virtual string TaskId
        {
            get { return taskId; }
        }

        public virtual string ActivityInstanceId
        {
            get { return activityInstanceId; }
        }

        public virtual string ErrorMessage
        {
            get { return errorMessage; }
        }

        public virtual string TenantId { get; set; }


        public virtual string TypeName
        {
            get
            {
                if (value != null)
                    return value.ToString();
                return null;
            }
        }

        public virtual string Name
        {
            get { return name; }
        }

        public virtual object Value
        {
            get
            {
                if (value != null)
                    return value;
                return null;
            }
        }

        public virtual ITypedValue TypedValue
        {
            get { return value; }
        }

        public virtual IProcessEngineServices ProcessEngineServices
        {
            get { return Context.ProcessEngineConfiguration.ProcessEngine; }
        }
        

        public static DelegateCaseVariableInstanceImpl FromVariableInstance(IVariableInstance variableInstance)
        {
            var delegateInstance = new DelegateCaseVariableInstanceImpl();
            delegateInstance.VariableId = variableInstance.Id;
            delegateInstance.processInstanceId = variableInstance.ProcessInstanceId;
            delegateInstance.executionId = variableInstance.ExecutionId;
            delegateInstance.caseExecutionId = variableInstance.CaseExecutionId;
            delegateInstance.caseInstanceId = variableInstance.CaseInstanceId;
            delegateInstance.taskId = variableInstance.TaskId;
            delegateInstance.activityInstanceId = variableInstance.ActivityInstanceId;
            delegateInstance.TenantId = variableInstance.TenantId;
            delegateInstance.errorMessage = variableInstance.ErrorMessage;
            delegateInstance.name = variableInstance.Name;
            delegateInstance.value = variableInstance.TypedValue;

            return delegateInstance;
        }
    }
}