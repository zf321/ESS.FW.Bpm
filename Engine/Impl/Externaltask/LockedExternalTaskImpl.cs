using System;
using System.Collections.Generic;


using ESS.FW.Bpm.Engine.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable;


namespace ESS.FW.Bpm.Engine.Impl.Externaltask
{
    /// <summary>
    ///     
    /// </summary>
    public class LockedExternalTaskImpl : ILockedExternalTask
    {
        protected internal string activityId;
        protected internal string activityInstanceId;
        protected internal string errorDetails;
        protected internal string errorMessage;
        protected internal string executionId;

        protected internal string id;
        protected internal DateTime lockExpirationTime;
        protected internal long priority;
        protected internal string processDefinitionId;
        protected internal string processDefinitionKey;
        protected internal string processInstanceId;
        protected internal int? retries;
        protected internal string tenantId;
        protected internal string topicName;
        protected internal VariableMapImpl variables;
        protected internal string workerId;

        public virtual string Id
        {
            get { return id; }
        }

        public virtual string TopicName
        {
            get { return topicName; }
        }

        public virtual string WorkerId
        {
            get { return workerId; }
        }

        public virtual DateTime LockExpirationTime
        {
            get { return lockExpirationTime; }
        }

        public virtual int? Retries
        {
            get { return retries; }
        }

        public virtual string ErrorMessage
        {
            get { return errorMessage; }
        }

        public virtual string ProcessInstanceId
        {
            get { return processInstanceId; }
        }

        public virtual string ExecutionId
        {
            get { return executionId; }
        }

        public virtual string ActivityId
        {
            get { return activityId; }
        }

        public virtual string ActivityInstanceId
        {
            get { return activityInstanceId; }
        }

        public virtual string ProcessDefinitionId
        {
            get { return processDefinitionId; }
        }

        public virtual string ProcessDefinitionKey
        {
            get { return processDefinitionKey; }
        }

        public virtual string TenantId
        {
            get { return tenantId; }
        }

        public virtual IVariableMap Variables
        {
            get { return variables; }
        }

        public virtual string ErrorDetails
        {
            get { return errorDetails; }
        }

        public virtual long Priority
        {
            get { return priority; }
        }

        /// <summary>
        ///     Construct representation of locked ExternalTask from corresponding entity.
        ///     During mapping variables will be collected,during collection variables will not be deserialized
        ///     and scope will not be set to local.
        /// </summary>
        /// <seealso cref=
        /// <seealso
        ///     cref="AbstractVariableScope#collectVariables(VariableMapImpl, Collection, boolean, boolean)" />
        /// </seealso>
        /// <param name="externalTaskEntity"> - source persistent entity to use for fields </param>
        /// <param name="variablesToFetch">
        ///     - list of variable names to fetch, if null then all variables will be fetched
        /// </param>
        /// <returns>
        ///     object with all fields copied from the ExternalTaskEntity, error details fetched from the
        ///     database and variables attached
        /// </returns>
        public static LockedExternalTaskImpl FromEntity(ExternalTaskEntity externalTaskEntity,
            IList<string> variablesToFetch, bool deserializeVariables)
        {
            var result = new LockedExternalTaskImpl();
            result.id = externalTaskEntity.Id;
            result.topicName = externalTaskEntity.TopicName;
            result.workerId = externalTaskEntity.WorkerId;
            result.lockExpirationTime = externalTaskEntity.LockExpirationTime;
            result.retries = externalTaskEntity.Retries;
            result.errorMessage = externalTaskEntity.ErrorMessage;
            result.errorDetails = externalTaskEntity.ErrorDetails;

            result.processInstanceId = externalTaskEntity.ProcessInstanceId;
            result.executionId = externalTaskEntity.ExecutionId;
            result.activityId = externalTaskEntity.ActivityId;
            result.activityInstanceId = externalTaskEntity.ActivityInstanceId;
            result.processDefinitionId = externalTaskEntity.ProcessDefinitionId;
            result.processDefinitionKey = externalTaskEntity.ProcessDefinitionKey;
            result.tenantId = externalTaskEntity.TenantId;
            result.priority = externalTaskEntity.Priority;

            ExecutionEntity execution = externalTaskEntity.Execution;
            result.variables = new VariableMapImpl();
            execution.CollectVariables(result.variables, variablesToFetch, false, deserializeVariables);

            return result;
        }
    }
}