using System;

namespace ESS.FW.Bpm.Engine.History
{
    /// <summary>
    ///     Log entry about an operation performed by a user. This is used for logging
    ///     actions such as creating a new ITask, completing a ITask,
    ///     canceling a process instance, ...
    ///     <h2>Operation Type</h2>
    ///     <para>
    ///         The type of the operation which has been performed. A user may create a new ITask,
    ///         complete a ITask, delegate a tasks, etc... Check this class for a list of built-in
    ///         operation type constants.
    ///     </para>
    ///     <h2>Entity Type</h2>
    ///     <para>
    ///         The type of the entity on which the operation was performed. Operations may be
    ///         performed on tasks, attachments, ...
    ///     </para>
    ///     <h2>Affected Entity Criteria</h2>
    ///     <para>
    ///         The methods that reference other entities (except users), such as <seealso cref="#getProcessInstanceId()" />
    ///         or <seealso cref="#getProcessDefinitionId()" />, describe which entities were affected
    ///         by the operation and represent restriction criteria.
    ///         A <code>null</code> return value of any of those methods means that regarding
    ///         this criterion, any entity was affected.
    ///     </para>
    ///     <para>
    ///         For example, if an operation suspends all process instances that belong to a certain
    ///         process definition id, one operation log entry is created.
    ///         Its return value for the method <seealso cref="#getProcessInstanceId()" /> is <code>null</code>,
    ///         while <seealso cref="#getProcessDefinitionId()" /> returns an id. Thus, the return values
    ///         of these methods can be understood as selection criteria of instances of the entity type
    ///         that were affected by the operation.
    ///     </para>
    ///     <h2>Additional Considerations</h2>
    ///     <para>
    ///         The event describes which user has requested out the operation and the time
    ///         at which the operation was performed. Furthermore, one operation can result in multiple
    ///         <seealso cref="IUserOperationLogEntry" /> entities whicha are linked by the value of the
    ///         <seealso cref="#getOperationId()" /> method.
    ///     </para>
    ///     
    ///     
    /// </summary>
    public interface IUserOperationLogEntry
    {
        /// @deprecated Please use
        /// <seealso cref="EntityTypes#ITask" />
        /// instead. 
        /// @deprecated Please use
        /// <seealso cref="EntityTypes#IDENTITY_LINK" />
        /// instead. 
        /// @deprecated Please use
        /// <seealso cref="EntityTypes#ATTACHMENT" />
        /// instead.
        /// <summary>
        ///     The unique identifier of this log entry.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Deployment reference
        /// </summary>
        string DeploymentId { get; }

        /// <summary>
        ///     Process definition reference.
        /// </summary>
        string ProcessDefinitionId { get; }

        /// <summary>
        ///     Key of the process definition this log entry belongs to; <code>null</code> means any.
        /// </summary>
        string ProcessDefinitionKey { get; }

        /// <summary>
        ///     Process instance reference.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     Execution reference.
        /// </summary>
        string ExecutionId { get; }

        /// <summary>
        ///     Case definition reference.
        /// </summary>
        string CaseDefinitionId { get; }

        /// <summary>
        ///     Case instance reference.
        /// </summary>
        string CaseInstanceId { get; }

        /// <summary>
        ///     Case execution reference.
        /// </summary>
        string CaseExecutionId { get; }

        /// <summary>
        ///     ITask instance reference.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     Job instance reference.
        /// </summary>
        string JobId { get; }

        /// <summary>
        ///     Job definition reference.
        /// </summary>
        string JobDefinitionId { get; }

        /// <summary>
        ///     Batch reference.
        /// </summary>
        string BatchId { get; }

        /// <summary>
        ///     The User who performed the operation
        /// </summary>
        string UserId { get; }

        /// <summary>
        ///     Timestamp of this change.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        ///     The unique identifier of this operation.
        ///     If an operation modifies multiple properties, multiple <seealso cref="IUserOperationLogEntry" /> instances will be
        ///     created with a common operationId. This allows grouping multiple entries which are part of a composite operation.
        /// </summary>
        string OperationId { get; }

        /// <summary>
        ///     Type of this operation, like create, assign, claim and so on.
        /// </summary>
        /// <seealso cref= # OPERATION_TYPE_ASSIGN and other fields beginning with OPERATION_TYPE
        /// </seealso>
        string OperationType { get; }

        /// <summary>
        ///     The type of the entity on which this operation was executed.
        /// </summary>
        /// <seealso cref= # ENTITY_TYPE_TASK and other fields beginning with ENTITY_TYPE
        /// </seealso>
        string EntityType { get; }

        /// <summary>
        ///     The property changed by this operation.
        /// </summary>
        string Property { get; }

        /// <summary>
        ///     The original value of the property.
        /// </summary>
        string OrgValue { get; }

        /// <summary>
        ///     The new value of the property.
        /// </summary>
        string NewValue { get; }
    }

    public static class UserOperationLogEntryFields
    {
        public const string OperationTypeAssign = "Assign";
        public const string OperationTypeClaim = "Claim";
        public const string OperationTypeComplete = "Complete";
        public const string OperationTypeCreate = "Create";
        public const string OperationTypeDelegate = "Delegate";
        public const string OperationTypeDelete = "Delete";
        public const string OperationTypeResolve = "Resolve";
        public const string OperationTypeSetOwner = "SetOwner";
        public const string OperationTypeSetPriority = "SetPriority";
        public const string OperationTypeUpdate = "Update";
        public const string OperationTypeActivate = "Activate";
        public const string OperationTypeSuspend = "Suspend";
        public const string OperationTypeMigrate = "Migrate";
        public const string OperationTypeAddUserLink = "AddUserLink";
        public const string OperationTypeDeleteUserLink = "DeleteUserLink";
        public const string OperationTypeAddGroupLink = "AddGroupLink";
        public const string OperationTypeDeleteGroupLink = "DeleteGroupLink";
        public const string OperationTypeAddAttachment = "AddAttachment";
        public const string OperationTypeDeleteAttachment = "DeleteAttachment";
        public const string OperationTypeSuspendJobDefinition = "SuspendJobDefinition";
        public const string OperationTypeActivateJobDefinition = "ActivateJobDefinition";
        public const string OperationTypeSuspendProcessDefinition = "SuspendProcessDefinition";
        public const string OperationTypeActivateProcessDefinition = "ActivateProcessDefinition";
        public const string OperationTypeModifyProcessInstance = "ModifyProcessInstance";
        public const string OperationTypeSuspendJob = "SuspendJob";
        public const string OperationTypeActivateJob = "ActivateJob";
        public const string OperationTypeSetJobRetries = "SetJobRetries";
        public const string OperationTypeSetVariable = "SetVariable";
        public const string OperationTypeRemoveVariable = "RemoveVariable";
        public const string OperationTypeModifyVariable = "ModifyVariable";
        public const string OperationTypeSuspendBatch = "SuspendBatch";
        public const string OperationTypeActivateBatch = "ActivateBatch";
        public static string OperationTypeRestartProcessInstance = "RestartProcessInstance";

    }
}