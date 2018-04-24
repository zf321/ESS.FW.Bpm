using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cfg.Auth;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Event;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.task;
using ESS.FW.Bpm.Engine.Impl.task.@delegate;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Bpmn.instance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Engine.exception;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{




    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TaskEntity : AbstractVariableScope, Task.ITask, IDelegateTask, IDbEntity, IHasDbRevision, ICommandContextListener, IVariablesProvider
    {
        private bool _instanceFieldsInitialized = false;

        private void InitializeInstanceFields()
        {

            _variableStore = new VariableStore(this, new TaskEntityReferencer(this));

        }


        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        public const string DeleteReasonCompleted = "completed";
        public const string DeleteReasonDeleted = "deleted";

        private const long SerialVersionUid = 1L;

        protected internal string owner;
        protected internal string assignee;
        protected internal DelegationState delegationState;

        protected internal string parentTaskId;
        [NonSerialized]
        protected internal TaskEntity parentTask;

        protected internal string name;
        protected internal string description;
        protected internal int priority = TaskFields.PriorityNormal;
        protected internal DateTime? dueDate;
        protected internal DateTime? followUpDate;
        protected internal string tenantId;

        protected internal bool IsIdentityLinksInitialized = false;

        [NonSerialized]
        [NotMapped]
        [JsonIgnore]
        protected internal ExecutionEntity execution;

        [NonSerialized]
        [NotMapped]
        [JsonIgnore]
        protected internal ExecutionEntity processInstance;


        //[NonSerialized]
        //protected internal CaseExecutionEntity caseExecution;

        protected internal string caseInstanceId;

        // taskDefinition
        [NonSerialized]
        protected internal TaskDefinition taskDefinition;
        protected internal string taskDefinitionKey;

        protected internal bool IsDeleted;
        protected internal string deleteReason;

        protected internal bool IsFormKeyInitialized = false;
        protected internal string formKey;

        [NonSerialized]
        [NotMapped]
        private VariableStore _variableStore;


        [NonSerialized]
        protected internal bool SkipCustomListeners = false;

        /// <summary>
        /// contains all changed properties of this entity
        /// </summary>
        //[NonSerialized]
        protected internal IDictionary<string, PropertyChange> propertyChanges = new Dictionary<string, PropertyChange>();

        [NonSerialized]
        protected internal IList<PropertyChange> IdentityLinkChanges = new List<PropertyChange>();

        // name references of tracked properties
        public const string ASSIGNEE = "assignee";
        public const string Delegation = "delegation";
        public const string delete = "delete";
        public const string DESCRIPTION = "description";
        public const string DUE_DATE = "dueDate";
        public const string FOLLOW_UP_DATE = "followUpDate";
        public const string NAME = "name";
        public const string OWNER = "owner";
        public const string PARENT_TASK = "parentTask";
        public const string PRIORITY = "priority";
        public const string CASE_INSTANCE_ID = "caseInstanceId";

        public TaskEntity() : this(null)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public TaskEntity(string taskId)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            this.Id = taskId;
        }

        /// <summary>
        /// creates and initializes a new persistent task. </summary>
        public static TaskEntity CreateAndInsert(IVariableScope execution)
        {
            TaskEntity task = Create();

            if (execution is ExecutionEntity)
            {
                ExecutionEntity executionEntity = (ExecutionEntity)execution;
                task.SetExecution(executionEntity);
                task.SkipCustomListeners = executionEntity.SkipCustomListeners;
                task.SaveTask(executionEntity);
                return task;

            }
            //else if (execution is CaseExecutionEntity)
            //{
            //    task.SetCaseExecution((IDelegateCaseExecution)execution);
            //}

            task.Insert(null);
            return task;
        }
        public void Insert(ExecutionEntity execution)
        {
            EnsureParentTaskActive();
            PropagateExecutionTenantId(execution);
            PropagateParentTaskTenantId();
            Context.CommandContext.TaskManager.InsertTask(this);
            if (execution != null)
            {
                execution.AddTask(this);
            }
        }

        public virtual void SaveTask(ExecutionEntity execution)
        {
            EnsureParentTaskActive();
            PropagateExecutionTenantId(execution);
            PropagateParentTaskTenantId();
            ITaskManager taskManager = Context.CommandContext.TaskManager;
            TaskEntity task = null;
            if (!string.IsNullOrEmpty(this.Id))
            {
                task = taskManager.FindTaskById(this.Id);
            }
            if (task != null)
                taskManager.UpdateTask(this);
            else
                taskManager.InsertTask(this);

            if (execution != null)
            {
                execution.AddTask(this);
            }
        }

        protected internal virtual void PropagateExecutionTenantId(ExecutionEntity execution)
        {
            if (execution != null)
            {
                TenantId = execution.TenantId;
            }
        }

        protected internal virtual void PropagateParentTaskTenantId()
        {
            if (parentTaskId != null)
            {

                TaskEntity parentTask = context.Impl.Context.CommandContext.TaskManager.FindTaskById(parentTaskId);

                if (tenantId != null && !TenantIdIsSame(parentTask))
                {
                    throw Log.CannotSetDifferentTenantIdOnSubtask(parentTaskId, parentTask.TenantId, tenantId);
                }

                TenantId = parentTask.TenantId;
            }
        }

        public virtual void Update()
        {
            //throw new NotImplementedException();
            EnsureParentTaskActive();
            PropagateExecutionTenantId(execution);
            PropagateParentTaskTenantId();

            CommandContext commandContext = context.Impl.Context.CommandContext;
            ITaskManager taskManager = commandContext.TaskManager;


            if (execution != null)
            {
                taskManager.UpdateTask(this);
            }
            EnsureTenantIdNotChanged();

            RegisterCommandContextCloseListener();

            //CommandContext commandContext = context.Impl.Context.CommandContext;
            //DbEntityManager dbEntityManger = commandContext.DbEntityManager;
            context.Impl.Context.CommandContext.Merge<TaskEntity>(this);
            ////dbEntityManger.Merge(this);
            //throw new NotImplementedException();
            ////commandContext.TaskManager.Merge(this);
        }

        protected internal virtual void EnsureTenantIdNotChanged()
        {
            TaskEntity persistentTask = context.Impl.Context.CommandContext.TaskManager.FindTaskById(Id);

            if (persistentTask != null)
            {

                bool changed = !TenantIdIsSame(persistentTask);

                if (changed)
                {
                    throw Log.CannotChangeTenantIdOfTask(Id, persistentTask.tenantId, tenantId);
                }
            }
        }

        protected internal virtual bool TenantIdIsSame(TaskEntity otherTask)
        {
            string otherTenantId = otherTask.TenantId;

            if (otherTenantId == null)
            {
                return tenantId == null;
            }
            else
            {
                return otherTenantId.Equals(tenantId);
            }
        }

        /// <summary>
        /// new task.  Embedded state and create time will be initialized.
        /// But this task still will have to be persisted with
        /// TransactionContext
        ///     .getCurrent()
        ///     .getPersistenceSession()
        ///     .insert(task);
        /// </summary>
        public static TaskEntity Create()
        {
            TaskEntity task = new TaskEntity();
            task.IsIdentityLinksInitialized = true;
            task.CreateTime = ClockUtil.CurrentTime;
            return task;
        }

        public virtual void Complete()
        {
            if (TaskListenerFields.EventnameComplete.Equals(this.EventName) || TaskListenerFields.EventnameDelete.Equals(this.EventName))
            {
                throw Log.InvokeTaskListenerException(new System.Exception("invalid task state"));
            }
            // if the task is associated with a case
            // execution then call complete on the
            // associated case execution. The case
            // execution handles the completion of
            // the task.
            if (CaseExecutionId != null)
            {
                //GetCaseExecution().manualComplete();
                throw new NotImplementedException();
            }

            // in the other case:

            // ensure the the Task is not suspended
            EnsureTaskActive();

            // trigger TaskListener.complete event
            FireEvent(TaskListenerFields.EventnameComplete);

            // delete will clear property
            var executionId = ExecutionId;
            // delete the task

            Context.CommandContext.TaskManager.DeleteTask(this, TaskEntity.DeleteReasonCompleted, false, SkipCustomListeners);
            // if the task is associated with a
            // execution (and not a case execution)
            // then call signal an the associated
            // execution.
            if (executionId != null)
            {
                ExecutionEntity execution = GetExecution(executionId);
                execution.RemoveTask(this);
                execution.Signal(null, null);
            }
        }

        public virtual void CaseExecutionCompleted()
        {
            throw new NotImplementedException();
            // ensure the the Task is not suspended
            //          EnsureTaskActive();

            //// trigger TaskListener.complete event
            //FireEvent(TaskListener.EVENTNAME_COMPLETE);

            //// delete the task
            //Context.CommandContext.TaskManager.DeleteTask(this, TaskEntity.DeleteReasonCompleted, false, false);
        }

        public virtual void Delete(string deleteReason, bool cascade)
        {
            this.deleteReason = deleteReason;
            FireEvent(TaskListenerFields.EventnameDelete);

            Context.CommandContext.TaskManager.DeleteTask(this, deleteReason, cascade, SkipCustomListeners);

            if (ExecutionId != null)
            {
                ExecutionEntity execution = GetExecution();
                execution.RemoveTask(this);
            }
        }

        public virtual void Delete(string deleteReason, bool cascade, bool skipCustomListeners)
        {
            this.SkipCustomListeners = skipCustomListeners;
            Delete(deleteReason, cascade);
        }

        public void Delegate(string userId)
        {
            DelegationState = DelegationState.Pending;
            if (Owner == null)
            {
                Owner = Assignee;
            }
            Assignee = userId;
        }

        public virtual void Resolve()
        {
            DelegationState = DelegationState.Resolved;
            Assignee = this.owner;
        }

        public object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            persistentState["assignee"] = this.assignee;
            persistentState["owner"] = this.owner;
            persistentState["name"] = this.name;
            persistentState["priority"] = this.priority;
            if (ExecutionId != null)
            {
                persistentState["executionId"] = this.ExecutionId;
            }
            if (ProcessDefinitionId != null)
            {
                persistentState["processDefinitionId"] = this.ProcessDefinitionId;
            }
            if (CaseExecutionId != null)
            {
                persistentState["CaseExecutionId"] = this.CaseExecutionId;
            }
            if (caseInstanceId != null)
            {
                persistentState["caseInstanceId"] = this.caseInstanceId;
            }
            if (CaseDefinitionId != null)
            {
                persistentState["CaseDefinitionId"] = this.CaseDefinitionId;
            }
            if (CreateTime != null)
            {
                persistentState["createTime"] = this.CreateTime;
            }
            if (description != null)
            {
                persistentState["description"] = this.description;
            }
            if (dueDate != null)
            {
                persistentState["dueDate"] = this.dueDate;
            }
            if (followUpDate != null)
            {
                persistentState["followUpDate"] = this.followUpDate;
            }
            if (parentTaskId != null)
            {
                persistentState["parentTaskId"] = this.parentTaskId;
            }
            if (delegationState != DelegationState.None)
            {
                persistentState["delegationState"] = this.delegationState;
            }
            if (tenantId != null)
            {
                persistentState["tenantId"] = this.tenantId;
            }

            persistentState["suspensionState"] = this.SuspensionState;

            return persistentState;
        }

        public int RevisionNext
        {
            get
            {
                return Revision + 1;
            }
        }

        protected internal virtual void EnsureParentTaskActive()
        {
            if (parentTaskId != null)
            {
                TaskEntity parentTask = context.Impl.Context.CommandContext.TaskManager.FindTaskById(parentTaskId);

                EnsureUtil.EnsureNotNull(typeof(NullValueException), "Parent task with id '" + parentTaskId + "' does not exist", "parentTask", parentTask);

                if (parentTask.SuspensionState == SuspensionStateFields.Suspended.StateCode)
                {
                    throw Log.SuspendedEntityException("parent task", Id);
                }
            }
        }

        protected internal virtual void EnsureTaskActive()
        {
            if (SuspensionState == SuspensionStateFields.Suspended.StateCode)
            {
                throw Log.SuspendedEntityException("task", Id);
            }
        }

        [JsonIgnore]
        public IUserTask BpmnModelElementInstance
        {
            get
            {
                IBpmnModelInstance bpmnModelInstance = BpmnModelInstance;
                if (bpmnModelInstance != null)
                {
                    IModelElementInstance modelElementInstance = bpmnModelInstance.GetModelElementById(taskDefinitionKey);
                    try
                    {
                        return (IUserTask)modelElementInstance;
                    }
                    catch (System.InvalidCastException e)
                    {
                        IModelElementType elementType = modelElementInstance.ElementType;
                        throw Log.CastModelInstanceException(modelElementInstance, "UserTask", elementType.TypeName, elementType.TypeNamespace, e);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        [JsonIgnore]
        public IBpmnModelInstance BpmnModelInstance
        {
            get
            {
                if (ProcessDefinitionId != null)
                {
                    return Context.ProcessEngineConfiguration.DeploymentCache.FindBpmnModelInstanceForProcessDefinition(ProcessDefinitionId);

                }
                else
                {
                    return null;

                }
            }
        }

        IFlowElement IBpmnModelExecutionContext.BpmnModelElementInstance
        {
            get { return BpmnModelElementInstance; }
        }

        // variables ////////////////////////////////////////////////////////////////


        //protected internal override IVariableInstanceFactory VariableInstanceFactory
        //{
        //    get
        //    {
        //        return (IVariableInstanceFactory)VariableInstanceEntityFactory.Instance;
        //    }
        //}
        [NotMapped]
        protected override IList<IVariableInstanceLifecycleListener<ICoreVariableInstance>> VariableInstanceLifecycleListeners
        {
            get
            {
                return new List<IVariableInstanceLifecycleListener<ICoreVariableInstance>>()
                {
                    VariableInstanceEntityPersistenceListener.Instance,
                    VariableInstanceSequenceCounterListener.Instance,
                    VariableInstanceHistoryListener.Instance
                };
            }
        }

        public override void DispatchEvent(VariableEvent variableEvent)
        {
            if (Execution != null && variableEvent.VariableInstance.TaskId == null)
            {
                execution.HandleConditionalEventOnVariableChange(variableEvent);
            }
        }
        public ICollection<ICoreVariableInstance> ProvideVariables()
        {
            var entities = Context.CommandContext.VariableInstanceManager.FindVariableInstancesByTaskId(Id);
            return entities.Cast<ICoreVariableInstance>().ToList();
        }
        [JsonIgnore]
        [NotMapped]
        public override AbstractVariableScope ParentVariableScope
        {
            get
            {

                if (GetExecution() != null)
                {
                    return execution;
                }
                //if (GetCaseExecution() != null)
                //{
                //    return caseExecution;
                //}
                if (ParentTask != null)
                {
                    return parentTask;
                }
                return null;
            }
        }
        [NotMapped]
        public override string VariableScopeKey
        {
            get
            {
                return "task";
            }
        }

        // execution ////////////////////////////////////////////////////////////////
        [NotMapped]
        public virtual TaskEntity ParentTask
        {
            get
            {
                if (parentTask == null && parentTaskId != null)
                {
                    this.parentTask = context.Impl.Context.CommandContext.TaskManager.FindTaskById(parentTaskId);
                }
                return parentTask;
            }
        }
        public ExecutionEntity GetExecution()
        {
            return GetExecution(ExecutionId);
        }
        public ExecutionEntity GetExecution(string executionId)
        {
            if ((Execution == null) && (executionId != null))
            {
                this.execution = context.Impl.Context.CommandContext.ExecutionManager.FindExecutionById(executionId);
            }
            return execution;
        }

        public virtual void SetExecution(IActivityExecution execution)
        {
            if (execution != null)
            {

                this.execution = (ExecutionEntity)execution;
                this.ExecutionId = this.Execution.Id;
                this.ProcessInstanceId = this.Execution.ProcessInstanceId;
                this.ProcessDefinitionId = this.Execution.ProcessDefinitionId;
                // get the process instance
                ExecutionEntity instance = (ExecutionEntity)this.Execution.ProcessInstance;
                if (instance != null)
                {
                    // set case instance id on this task
                    this.caseInstanceId = instance.CaseInstanceId;
                }

            }
            else
            {
                this.execution = null;
                this.ExecutionId = null;
                this.ProcessInstanceId = null;
                this.ProcessDefinitionId = null;
                this.caseInstanceId = null;
            }
        }

        // case execution ////////////////////////////////////////////////////////////////

        //public CaseExecutionEntity GetCaseExecution()
        //{
        //    EnsureCaseExecutionInitialized();
        //    return caseExecution;
        //}

        protected internal virtual void EnsureCaseExecutionInitialized()
        {
            if ((CaseExecution == null) && (CaseExecutionId != null))
            {
                throw new NotImplementedException();
                //CaseExecution = Context.CommandContext.CaseExecutionManager.findCaseExecutionById(CaseExecutionId);
            }
        }

        public virtual void SetCaseExecution(IDelegateCaseExecution caseExecution)
        {
            if (caseExecution != null)
            {

                //this.caseExecution = (CaseExecutionEntity)caseExecution;
                this.CaseExecutionId = this.CaseExecution.Id;
                this.caseInstanceId = this.CaseExecution.CaseInstanceId;
                this.CaseDefinitionId = this.CaseExecution.CaseDefinitionId;

            }
            else
            {
                //this.caseExecution = null;
                this.CaseExecutionId = null;
                this.caseInstanceId = null;
                this.CaseDefinitionId = null;
            }
        }

        public string CaseExecutionId { get; set; }

        [NotMapped]
        public string CaseInstanceId
        {
            get
            {
                return caseInstanceId;
            }
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(CASE_INSTANCE_ID, this.caseInstanceId, value);
                this.caseInstanceId = value;
            }
        }


        /* plain setter for persistence */
        public virtual string CaseInstanceIdWithoutCascade
        {
            get
            {
                return caseInstanceId;
            }
            set
            {
                this.caseInstanceId = value;
            }
        }

        //public virtual CaseDefinitionEntity CaseDefinition
        //{
        //    get
        //    {
        //        if (CaseDefinitionId != null)
        //        {
        //             return context.Impl.Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedCaseDefinitionById(CaseDefinitionId);
        //        }
        //        return null;
        //    }
        //}

        public string CaseDefinitionId { get; set; }


        // task assignment //////////////////////////////////////////////////////////

        public virtual IdentityLinkEntity AddIdentityLink(string userId, string groupId, string type)
        {
            EnsureTaskActive();

            IdentityLinkEntity identityLink = NewIdentityLink(userId, groupId, type);
            identityLink.Insert();
            IdentityLinks.Add(identityLink);

            FireAddIdentityLinkAuthorizationProvider(type, userId, groupId);
            return identityLink;
        }

        public virtual void FireIdentityLinkHistoryEvents(string userId, string groupId, string type, HistoryEventTypes historyEventType)
        {
            IdentityLinkEntity identityLinkEntity = NewIdentityLink(userId, groupId, type);
            identityLinkEntity.FireHistoricIdentityLinkEvent(historyEventType);
        }

        public virtual IdentityLinkEntity NewIdentityLink(string userId, string groupId, string type)
        {
            IdentityLinkEntity identityLinkEntity = new IdentityLinkEntity();
            identityLinkEntity.Task = (this);
            identityLinkEntity.UserId = userId;
            identityLinkEntity.GroupId = groupId;
            identityLinkEntity.Type = type;
            identityLinkEntity.TenantId = TenantId;
            return identityLinkEntity;
        }

        public virtual void DeleteIdentityLink(string userId, string groupId, string type)
        {
            EnsureTaskActive();

            IList<IdentityLinkEntity> identityLinks = Context.CommandContext.IdentityLinkManager.FindIdentityLinkByTaskUserGroupAndType(Id, userId, groupId, type);

            foreach (IdentityLinkEntity identityLink in identityLinks)
            {
                FireDeleteIdentityLinkAuthorizationProvider(type, userId, groupId);
                identityLink.Delete();
            }
        }

        public virtual void DeleteIdentityLinks()
        {
            IList<IdentityLinkEntity> identityLinkEntities = IdentityLinks;
            foreach (IdentityLinkEntity identityLinkEntity in identityLinkEntities)
            {
                FireDeleteIdentityLinkAuthorizationProvider(identityLinkEntity.Type, identityLinkEntity.UserId, identityLinkEntity.GroupId);
                identityLinkEntity.Delete();
            }
            IsIdentityLinksInitialized = false;
        }

        [JsonIgnore]
        public ISet<IIdentityLink> Candidates
        {
            get
            {
                ISet<IIdentityLink> potentialOwners = new HashSet<IIdentityLink>();
                foreach (IdentityLinkEntity identityLinkEntity in IdentityLinks)
                {
                    if (IdentityLinkType.Candidate.Equals(identityLinkEntity.Type))
                    {
                        potentialOwners.Add(identityLinkEntity);
                    }
                }
                return potentialOwners;
            }
        }

        public void AddCandidateUser(string userId)
        {
            AddIdentityLink(userId, null, IdentityLinkType.Candidate);
        }

        public void AddCandidateUsers(ICollection<string> candidateUsers)
        {
            foreach (string candidateUser in candidateUsers)
            {
                AddCandidateUser(candidateUser);
            }
        }

        public void AddCandidateGroup(string groupId)
        {
            AddIdentityLink(null, groupId, IdentityLinkType.Candidate);
        }

        public void AddCandidateGroups(ICollection<string> candidateGroups)
        {
            foreach (string candidateGroup in candidateGroups)
            {
                AddCandidateGroup(candidateGroup);
            }
        }

        public void AddGroupIdentityLink(string groupId, string identityLinkType)
        {
            AddIdentityLink(null, groupId, identityLinkType);
        }

        public void AddUserIdentityLink(string userId, string identityLinkType)
        {
            AddIdentityLink(userId, null, identityLinkType);
        }

        public void DeleteCandidateGroup(string groupId)
        {
            DeleteGroupIdentityLink(groupId, IdentityLinkType.Candidate);
        }

        public void DeleteCandidateUser(string userId)
        {
            DeleteUserIdentityLink(userId, IdentityLinkType.Candidate);
        }

        public void DeleteGroupIdentityLink(string groupId, string identityLinkType)
        {
            if (groupId != null)
            {
                DeleteIdentityLink(null, groupId, identityLinkType);
            }
        }

        public void DeleteUserIdentityLink(string userId, string identityLinkType)
        {
            if (userId != null)
            {
                DeleteIdentityLink(userId, null, identityLinkType);
            }
        }

        public virtual IList<IdentityLinkEntity> IdentityLinks { get; set; } = new List<IdentityLinkEntity>();

        [NotMapped]
        [JsonIgnore]
        public virtual IDictionary<string, object> ActivityInstanceVariables
        {
            get
            {
                if (Execution != null)
                {
                    return Execution.Variables;
                }
                return new Dictionary<string, object>();
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual IDictionary<string, object> ExecutionVariables
        {
            set
            {
                AbstractVariableScope scope = ParentVariableScope;
                if (scope != null)
                {
                    scope.Variables = value;
                }
            }
        }

        public override string ToString()
        {
            return "Task[" + Id + "]";
        }

        // special setters //////////////////////////////////////////////////////////
        public string Name
        {
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(NAME, this.name, value);
                this.name = value;
            }
            get
            {
                return name;
            }
        }

        /* plain setter for persistence */
        public virtual string NameWithoutCascade
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }
        [NotMapped]
        public string Description
        {
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(DESCRIPTION, this.description, value);
                this.description = value;
            }
            get
            {
                return description;
            }
        }

        /* plain setter for persistence */
        public virtual string DescriptionWithoutCascade
        {
            get
            {
                return description;
            }
            set
            {
                this.description = value;
            }
        }
        [NotMapped]
        public string Assignee
        {
            set
            {
                EnsureTaskActive();
                RegisterCommandContextCloseListener();

                string oldAssignee = this.assignee;
                if (value == null && oldAssignee == null)
                {
                    return;
                }

                AddIdentityLinkChanges(IdentityLinkType.Assignee, oldAssignee, value);
                PropertyChanged(ASSIGNEE, oldAssignee, value);
                this.assignee = value;

                CommandContext commandContext = context.Impl.Context.CommandContext;
                // if there is no command context, then it means that the user is calling the
                // setAssignee outside a service method.  E.g. while creating a new task.
                if (commandContext != null)
                {
                    FireEvent(TaskListenerFields.EventnameAssignment);
                    //if (commandContext.DbEntityManager.Contains(this))
                    //{
                    FireAssigneeAuthorizationProvider(oldAssignee, value);
                    FireHistoricIdentityLinks();
                    //}
                }
            }
            get
            {
                return assignee;
            }
        }

        /* plain setter for persistence */
        public virtual string AssigneeWithoutCascade
        {
            get
            {
                return assignee;
            }
            set
            {
                this.assignee = value;
            }
        }
        [NotMapped]
        public string Owner
        {
            set
            {
                EnsureTaskActive();
                RegisterCommandContextCloseListener();

                string oldOwner = this.owner;
                if (value == null && oldOwner == null)
                {
                    return;
                }

                AddIdentityLinkChanges(IdentityLinkType.Owner, oldOwner, value);
                PropertyChanged(OWNER, oldOwner, value);
                this.owner = value;

                CommandContext commandContext = context.Impl.Context.CommandContext;
                // if there is no command context, then it means that the user is calling the
                // setOwner outside a service method.  E.g. while creating a new task.
                if (commandContext != null/* && commandContext.DbEntityManager.Contains(this)*/)
                {
                    FireOwnerAuthorizationProvider(oldOwner, value);
                    this.FireHistoricIdentityLinks();
                }

            }
            get
            {
                return owner;
            }
        }

        /* plain setter for persistence */
        public virtual string OwnerWithoutCascade
        {
            get
            {
                return owner;
            }
            set
            {
                this.owner = value;
            }
        }
        [NotMapped]
        public DateTime? DueDate
        {
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(DUE_DATE, this.dueDate, value);
                this.dueDate = value;
            }
            get
            {
                return dueDate;
            }
        }

        public virtual DateTime? DueDateWithoutCascade
        {
            get
            {
                return dueDate;
            }
            set
            {
                this.dueDate = value;
            }
        }
        [NotMapped]
        public int Priority
        {
            set
            {

                RegisterCommandContextCloseListener();
                PropertyChanged(PRIORITY, this.priority, value);
                this.priority = value;
            }
            get
            {

                return priority;
            }
        }

        public virtual int PriorityWithoutCascade
        {
            get
            {
                return Priority;
            }
            set
            {

                this.priority = value;
            }
        }
        [NotMapped]
        public string ParentTaskId
        {
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(PARENT_TASK, this.parentTaskId, value);
                this.parentTaskId = value;
            }
            get
            {
                return parentTaskId;
            }
        }

        public virtual string ParentTaskIdWithoutCascade
        {
            get
            {
                return parentTaskId;
            }
            set
            {
                this.parentTaskId = value;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual string TaskDefinitionKeyWithoutCascade
        {
            get
            {
                return taskDefinitionKey;
            }
            set
            {
                this.taskDefinitionKey = value;
            }
        }
        public virtual void FireEvent()
        {
            PropertyChange assigneePropertyChange = propertyChanges.ContainsKey(ASSIGNEE) ? propertyChanges[ASSIGNEE] : null;
            if (assigneePropertyChange != null)
            {
                FireEvent(TaskListenerFields.EventnameAssignment);
            }
        }
        public virtual void FireEvent(string taskEventName)
        {
            IList<ITaskListener> taskEventListeners = GetListenersForEvent(taskEventName);

            if (taskEventListeners != null)
            {
                foreach (ITaskListener taskListener in taskEventListeners)
                {
                    CoreExecution execution = GetExecution();
                    //if (execution == null)
                    //{
                    //    execution = GetCaseExecution();
                    //}

                    if (execution != null)
                    {
                        EventName = taskEventName;
                    }
                    try
                    {
                        TaskListenerInvocation listenerInvocation = new TaskListenerInvocation(taskListener, this, execution);
                        context.Impl.Context.ProcessEngineConfiguration.DelegateInterceptor.HandleInvocation(listenerInvocation);
                    }
                    catch (System.Exception e)
                    {
                        throw Log.InvokeTaskListenerException(e);
                    }
                }
            }
        }

        protected internal virtual IList<ITaskListener> GetListenersForEvent(string @event)
        {
            TaskDefinition resolvedTaskDefinition = TaskDefinition;
            if (resolvedTaskDefinition != null)
            {
                if (SkipCustomListeners)
                {
                    return resolvedTaskDefinition.GetBuiltinTaskListeners(@event);
                }
                else
                {
                    return resolvedTaskDefinition.GetTaskListeners(@event);
                }

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tracks a property change. Therefore the original and new value are stored in a map.
        /// It tracks multiple changes and if a property finally is changed back to the original
        /// value, then the change is removed.
        /// </summary>
        /// <param name="propertyName"> </param>
        /// <param name="orgValue"> </param>
        /// <param name="newValue"> </param>
        protected internal virtual void PropertyChanged(string propertyName, object orgValue, object newValue)
        {
            if (propertyChanges.ContainsKey(propertyName)) // update an existing change to save the original value
            {
                object oldOrgValue = propertyChanges[propertyName].OrgValue;
                if ((oldOrgValue == null && newValue == null) || (oldOrgValue != null && oldOrgValue.Equals(newValue))) // remove this change -  change back to null
                {
                    propertyChanges.Remove(propertyName);
                }
                else
                {
                    propertyChanges[propertyName].NewValue = newValue;
                }
            } // save this change
            else
            {
                if ((orgValue == null && newValue != null) || (orgValue != null && newValue == null) || (orgValue != null && !orgValue.Equals(newValue))) // value change -  value to null -  null to value
                {
                    propertyChanges[propertyName] = new PropertyChange(propertyName, orgValue, newValue);
                }
            }
        }

        // authorizations ///////////////////////////////////////////////////////////

        public virtual void FireAuthorizationProvider()
        {
            PropertyChange assigneePropertyChange = propertyChanges.ContainsKey(ASSIGNEE) ? propertyChanges[ASSIGNEE] : null;
            if (assigneePropertyChange != null)
            {
                string oldAssignee = assigneePropertyChange.OrgValueString;
                string newAssignee = assigneePropertyChange.NewValueString;
                FireAssigneeAuthorizationProvider(oldAssignee, newAssignee);
            }

            PropertyChange ownerPropertyChange = propertyChanges.ContainsKey(OWNER) ? propertyChanges[OWNER] : null;
            if (ownerPropertyChange != null)
            {
                string oldOwner = ownerPropertyChange.OrgValueString;
                string newOwner = ownerPropertyChange.NewValueString;
                FireOwnerAuthorizationProvider(oldOwner, newOwner);
            }
        }

        protected internal virtual void FireAssigneeAuthorizationProvider(string oldAssignee, string newAssignee)
        {
            FireAuthorizationProvider(ASSIGNEE, oldAssignee, newAssignee);
        }

        protected internal virtual void FireOwnerAuthorizationProvider(string oldOwner, string newOwner)
        {
            FireAuthorizationProvider(OWNER, oldOwner, newOwner);
        }

        protected internal virtual void FireAuthorizationProvider(string property, string oldValue, string newValue)
        {
            if (AuthorizationEnabled && CaseExecutionId == null)
            {
                IResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

                AuthorizationEntity[] authorizations = null;
                if (ASSIGNEE.Equals(property))
                {
                    authorizations = provider.NewTaskAssignee(this, oldValue, newValue);
                }
                else if (OWNER.Equals(property))
                {
                    authorizations = provider.NewTaskOwner(this, oldValue, newValue);
                }

                SaveAuthorizations(authorizations);
            }
        }

        protected internal virtual void FireAddIdentityLinkAuthorizationProvider(string type, string userId, string groupId)
        {
            if (AuthorizationEnabled && CaseExecutionId == null)
            {
                IResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

                AuthorizationEntity[] authorizations = null;
                if (userId != null)
                {
                    authorizations = provider.NewTaskUserIdentityLink(this, userId, type);
                }
                else if (groupId != null)
                {
                    authorizations = provider.NewTaskGroupIdentityLink(this, groupId, type);
                }

                SaveAuthorizations(authorizations);
            }
        }

        protected internal virtual void FireDeleteIdentityLinkAuthorizationProvider(string type, string userId, string groupId)
        {
            if (AuthorizationEnabled && CaseExecutionId == null)
            {
                IResourceAuthorizationProvider provider = ResourceAuthorizationProvider;

                AuthorizationEntity[] authorizations = null;
                if (userId != null)
                {
                    authorizations = provider.DeleteTaskUserIdentityLink(this, userId, type);
                }
                else if (groupId != null)
                {
                    authorizations = provider.DeleteTaskGroupIdentityLink(this, groupId, type);
                }

                DeleteAuthorizations(authorizations);
            }
        }

        protected internal virtual IResourceAuthorizationProvider ResourceAuthorizationProvider
        {
            get
            {
                ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
                return processEngineConfiguration.ResourceAuthorizationProvider;
            }
        }

        protected internal virtual void SaveAuthorizations(AuthorizationEntity[] authorizations)
        {
            CommandContext commandContext = Context.CommandContext;
            ITaskManager taskManager = commandContext.TaskManager;
            //taskManager.SaveDefaultAuthorizations(authorizations);
        }

        protected internal virtual void DeleteAuthorizations(AuthorizationEntity[] authorizations)
        {
            CommandContext commandContext = Context.CommandContext;
            ITaskManager taskManager = commandContext.TaskManager;
            //taskManager.DeleteDefaultAuthorizations(authorizations);
        }

        protected internal virtual bool AuthorizationEnabled
        {
            get
            {
                return context.Impl.Context.ProcessEngineConfiguration.AuthorizationEnabled;
            }
        }

        // modified getters and setters /////////////////////////////////////////////
        [NotMapped]
        [JsonIgnore]
        public virtual TaskDefinition TaskDefinition
        {
            set
            {
                this.taskDefinition = value;
                this.taskDefinitionKey = value.Key;
            }
            get
            {
                if (taskDefinition == null && taskDefinitionKey != null)
                {

                    IDictionary<string, TaskDefinition> taskDefinitions = null;
                    if (ProcessDefinitionId != null)
                    {
                        ProcessDefinitionEntity processDefinition = Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(ProcessDefinitionId);

                        taskDefinitions = processDefinition.TaskDefinitions;

                    }
                    //else
                    //{
                    //    CaseDefinitionEntity caseDefinition = Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedCaseDefinitionById(caseDefinitionId);

                    //    taskDefinitions = caseDefinition.TaskDefinitions;ProcessDefinition
                    //}

                    taskDefinition = taskDefinitions[taskDefinitionKey];
                }
                return taskDefinition;
            }
        }


        // getters and setters //////////////////////////////////////////////////////

        public string Id { get; set; }


        public int Revision { get; set; }






        public DateTime CreateTime { get; set; }


        public string ExecutionId { get; set; }

        public string ProcessInstanceId { get; set; }

        [JsonIgnore]
        public virtual ProcessDefinitionEntity ProcessDefinition
        {
            get
            {
                //throw new NotImplementedException();
                if (ProcessDefinitionId != null)
                {
                    return Context.CommandContext.ProcessDefinitionManager.Get(ProcessDefinitionId);
                    //return Context.ProcessEngineConfiguration.DeploymentCache.FindDeployedProcessDefinitionById(processDefinitionId);
                }
                return null;
            }
        }

        public string ProcessDefinitionId { get; set; }

        public virtual void InitializeFormKey()
        {
            IsFormKeyInitialized = true;
            if (taskDefinitionKey != null)
            {
                TaskDefinition taskDefinition = TaskDefinition;
                if (taskDefinition != null)
                {
                    Delegate.IExpression k = taskDefinition.FormKey;
                    if (k != null)
                    {
                        this.formKey = (string)k.GetValue(this);
                    }
                }
            }
        }
        [JsonIgnore]
        public string FormKey
        {
            get
            {
                if (!IsFormKeyInitialized)
                {
                    throw Log.UninitializedFormKeyException();
                }
                return formKey;
            }
        }


        //[NotMapped]
        public string TaskDefinitionKey
        {
            get
            {
                return taskDefinitionKey;
            }
            set
            {
                if ((value == null && this.taskDefinitionKey != null) || (value != null && !value.Equals(this.taskDefinitionKey)))
                {
                    this.taskDefinition = null;
                    this.formKey = null;
                    this.IsFormKeyInitialized = false;
                }

                this.taskDefinitionKey = value;
            }
        }

        public IDelegateExecution Execution
        {
            get { return execution; }
        }
        public IDelegateCaseExecution CaseExecution { get; }

        [NotMapped]
        public string EventName { get; set; }
        public virtual ExecutionEntity GetProcessInstance()
        {
            return processInstance;
        }
        public virtual void SetProcessInstance(ExecutionEntity processInstance)
        {
            this.processInstance = processInstance;
        }
        [NotMapped]
        public DelegationState DelegationState
        {
            get
            {
                return delegationState;
            }
            set
            {
                PropertyChanged(Delegation, this.delegationState, value);
                this.delegationState = value;
            }
        }

        public virtual DelegationState DelegationStateWithoutCascade
        {
            set
            {
                this.delegationState = value;
            }
        }

        public virtual string DelegationStateString
        {
            get
            {
                return (delegationState != null ? delegationState.ToString() : null);
            }
            set
            {
                if (value == null)
                {
                    DelegationStateWithoutCascade = DelegationState.Pending;
                }
                else
                {
                    DelegationStateWithoutCascade = (DelegationState)Enum.Parse(DelegationState.GetType(), value);
                }
            }
        }

        [NotMapped]
        public virtual bool Deleted
        {
            get
            {
                return IsDeleted;
            }
            set
            {
                PropertyChanged(delete, this.IsDeleted, value);
                this.IsDeleted = value;
            }
        }

        public string DeleteReason
        {
            get
            {
                return deleteReason;
            }
        }


        public virtual int SuspensionState { get; set; }= SuspensionStateFields.Active.StateCode;
        public bool Suspended
        {
            get
            {
                return SuspensionState == SuspensionStateFields.Suspended.StateCode;
            }
        }
        [NotMapped]
        public DateTime? FollowUpDate
        {
            get
            {
                return followUpDate;
            }
            set
            {
                RegisterCommandContextCloseListener();
                PropertyChanged(FOLLOW_UP_DATE, this.followUpDate, value);
                this.followUpDate = value;
            }
        }

        public string TenantId
        {
            get
            {
                return tenantId;
            }
            set
            {
                this.tenantId = value;
            }
        }




        public virtual DateTime? FollowUpDateWithoutCascade
        {
            get
            {
                return followUpDate;
            }
            set
            {
                this.followUpDate = value;
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual ICollection<ICoreVariableInstance> VariablesInternal
        {
            get
            {

                return VariableStore.Variables;
            }
        }

        public void OnCommandContextClose(CommandContext commandContext)
        {

            //if (commandContext.DbEntityManager.IsDirty(this))
            //{
            //    commandContext.HistoricTaskInstanceManager.UpdateHistoricTaskInstance(this);
            //}
        }


        public void OnCommandFailed(CommandContext commandContext, System.Exception t)
        {
            // ignore
        }

        protected internal virtual void RegisterCommandContextCloseListener()
        {
            CommandContext commandContext = context.Impl.Context.CommandContext;
            if (commandContext != null)
            {
                commandContext.RegisterCommandContextListener(this);
            }
        }

        public virtual IDictionary<string, PropertyChange> PropertyChanges
        {
            get
            {
                return propertyChanges;
            }
        }

        public virtual void CreateHistoricTaskDetails(string operation)
        {

            CommandContext commandContext = Context.CommandContext;
            if (commandContext != null)
            {
                IList<PropertyChange> values = new List<PropertyChange>(propertyChanges.Values);
                commandContext.OperationLogManager.LogTaskOperations(operation, this, values);
                FireHistoricIdentityLinks();
                propertyChanges.Clear();
            }
        }

        public virtual void FireHistoricIdentityLinks()
        {
            foreach (PropertyChange propertyChange in IdentityLinkChanges)
            {
                string oldValue = propertyChange.OrgValueString;
                string propertyName = propertyChange.PropertyName;
                if (oldValue != null)
                {
                    FireIdentityLinkHistoryEvents(oldValue, null, propertyName, HistoryEventTypes.IdentityLinkDelete);
                }
                string newValue = propertyChange.NewValueString;
                if (newValue != null)
                {
                    FireIdentityLinkHistoryEvents(newValue, null, propertyName, HistoryEventTypes.IdentityLinkAdd);
                }
            }
            IdentityLinkChanges.Clear();
        }
        [JsonIgnore]
        public IProcessEngineServices ProcessEngineServices
        {
            get
            {
                return context.Impl.Context.ProcessEngineConfiguration.ProcessEngine;
            }
        }

        protected internal override IVariableInstanceFactory<ICoreVariableInstance> VariableInstanceFactory
        {
            get
            {
                return VariableInstanceEntityFactory.Instance;
            }
        }



        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((Id == null) ? 0 : Id.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() != obj.GetType())
            {
                return false;
            }
            TaskEntity other = (TaskEntity)obj;
            if (Id == null)
            {
                if (other.Id != null)
                {
                    return false;
                }
            }
            else if (!Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

        public virtual void ExecuteMetrics(string metricsName)
        {
            ProcessEngineConfigurationImpl processEngineConfiguration = context.Impl.Context.ProcessEngineConfiguration;
            if (processEngineConfiguration.MetricsEnabled)
            {
                processEngineConfiguration.MetricsRegistry.MarkOccurrence(Metrics.ActivtyInstanceStart);
            }
        }

        public virtual void AddIdentityLinkChanges(string type, string oldProperty, string newProperty)
        {
            IdentityLinkChanges.Add(new PropertyChange(type, oldProperty, newProperty));
        }


        //public virtual ExecutionEntity ExecutionEf
        //{
        //    get
        //    {
        //        return execution;
        //    }
        //    set
        //    {
        //        this.execution = value;
        //    }
        //}
        //public virtual ExecutionEntity ProcessInstanceExecution { get; set; }
        [JsonIgnore]
        [NotMapped]
        protected internal override VariableStore VariableStore => _variableStore;
    }

}