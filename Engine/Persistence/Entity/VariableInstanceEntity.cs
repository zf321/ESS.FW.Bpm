using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Core.Variable;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Runtime;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Value;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class VariableInstanceEntity : IDbEntity, IVariableInstance, ICoreVariableInstance, IValueFields,
        IDbEntityLifecycleAware, ITypedValueUpdateListener, IHasDbRevision, INameable
    {
        private const long SerialVersionUid = 1L;


        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        private readonly bool _instanceFieldsInitialized;

        // transient properties
        [NotMapped]
        private ExecutionEntity _execution;

        protected internal ByteArrayField byteArrayField;

        protected internal string configuration;

        protected internal string gaseExecutionId;

        /// <summary>
        ///     临时
        ///     Determines whether this variable is stored in the data base.
        /// </summary>
        protected internal bool isTransient;


        protected internal TypedValueField typedValueField;


        //public virtual string GetSerializerName()
        //{
        //    return typedValueField.SerializerName;
        //}

        //public virtual string GetErrorMessage()
        //{
        //    return typedValueField.ErrorMessage;
        //}
        protected string variableScopeId;

        //SQL映射的默认构造函数 Default constructor for SQL mapping
        public VariableInstanceEntity()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            typedValueField.AddImplicitUpdateListener(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isTransient">临时</param>
        public VariableInstanceEntity(string name, ITypedValue value, bool isTransient) : this()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            Name = name;
            this.isTransient = isTransient;
            typedValueField.SetValue(value);
        }
        #region EF字段
        public string Id { get; set; }
        public int Revision { get; set; }
        public string SerializerName
        {
            get { return typedValueField.SerializerName == null ? "SerializerName=null【调试】" : typedValueField.SerializerName; }
            set { typedValueField.SerializerName = value; }
        }
        public string Name { get; set; }

        public string ProcessInstanceId { get; set; }
        public string ExecutionId { get; set; }
        public string CaseInstanceId { get; set; }
        public string CaseExecutionId { get; set; }
        public string TaskId { get; set; }
        public string ActivityInstanceId { get; set; }
        public string ActivityId { get; set; }
        public bool IsActive { get; set; }
        public bool IsConcurrencyScope { get; set; }
        public string ByteArrayId { get { return byteArrayField.ByteArrayId; } set { byteArrayField.ByteArrayId = value; } }
        public double? DoubleValue { get; set; }
        public string TextValue { get; set; }
        public string TextValue2 { get; set; }
        public long? LongValue { get; set; }
        public long SequenceCounter { get; set; } = 1;
        /// <summary>
        ///     <para>
        ///         Determines whether this variable is supposed to be a local variable
        ///         in case of concurrency in its scope. This affects
        ///     </para>
        ///     <ul>
        ///         <li>
        ///             tree expansion (not evaluated yet by the engine)
        ///             <li>
        ///                 activity instance IDs of variable instances: concurrentLocal
        ///                 variables always receive the activity instance id of their execution
        ///                 (which may not be the scope execution), while non-concurrentLocal variables
        ///                 always receive the activity instance id of their scope (which is set in the
        ///                 parent execution)
        ///     </ul>
        ///     <para>
        ///         In the future, this field could be used for restoring the variable distribution
        ///         when the tree is expanded/compacted multiple times.
        ///         On expansion, the goal would be to keep concurrentLocal variables always with
        ///         their concurrent replacing executions while non-concurrentLocal variables
        ///         stay in the scope execution
        ///     </para>
        /// </summary>
        public bool IsConcurrentLocal { get; set; }
        public string TenantId { get; set; }
        [NotMapped]//[ForeignKey("ByteArrayValueId")]
        public virtual ResourceEntity ResourceEntity
        {
            get
            {
                return byteArrayField.ResourceEntity;
            }
            set
            {
                byteArrayField.ByteArrayId = value.Id;
            }
        }
        [NotMapped]//[ForeignKey("ProcessInstanceId")]
        public virtual ExecutionEntity ProcessInstanceExecution { get; set; }
        [NotMapped]
        [JsonIgnore]
        public virtual ExecutionEntity Execution
        {
            get
            {
                EnsureExecutionInitialized();
                return _execution;
            }
            set
            {
                this._execution = value;

                if (value == null)
                {
                    ExecutionId = null;
                    ProcessInstanceId = null;
                    TenantId = null;
                }
                else
                {
                    SetExecutionId(value.Id);
                    ProcessInstanceId = value.ProcessInstanceId;
                    TenantId = value.TenantId;
                }
            }
        }
        [NotMapped]
        [JsonIgnore]
        public virtual TaskEntity Task
        {
            get
            {
                if (TaskId != null)
                    return Context.CommandContext.TaskManager.FindTaskById(TaskId);
                return null;
            }
            set
            {
                if (value != null)
                {
                    this.TaskId = value.Id;
                    this.TenantId = value.TenantId;
                    var r = value.GetExecution();
                    if (r != null)
                    {
                        Execution = r;
                    }
                    //if (task.GetCaseExecution() != null)
                    //{
                    //    //setCaseExecution(task.getCaseExecution());
                    //}
                }
                else
                {
                    this.TaskId = null;
                    this.TenantId = null;
                    //SetExecution(null);
                    Execution = null;
                    //setCaseExecution(null);
                }
            }
        }
        #endregion
        public string VariableScopeId
        {
            get
            {
                if (TaskId != null)
                    return TaskId;

                if (ExecutionId != null)
                    return ExecutionId;

                return CaseExecutionId;
            }
            set { variableScopeId = value; }
        }


        public virtual void Delete()
        {
            // clear value
            ClearValueFields();

            if (!isTransient)
                Context.CommandContext.VariableInstanceManager.Delete(this);
        }

        public virtual ITypedValue GetTypedValue(bool deserializeValue)
        {
            return typedValueField.GetTypedValue(deserializeValue);
        }
        public void SetValue(ITypedValue value)
        {
            if (GetIsTransient())
                throw Log.UpdateTransientVariableException(GetName());

            // clear value fields
            ClearValueFields();

            typedValueField.SetValue(value);
        }

        public virtual void IncrementSequenceCounter()
        {
            SequenceCounter++;
        }

        /// <returns>
        ///     <code>true</code>, if the variable is transient. A transient
        ///     variable is not stored in the data base.
        /// </returns>
        public virtual bool GetIsTransient()
        {
            return isTransient;
        }

        public virtual object GetPersistentState()
        {
            IDictionary<string, object> persistentState = new Dictionary<string, object>();
            if (typedValueField.SerializerName != null)
                persistentState["serializerName"] = typedValueField.SerializerName;
            if (LongValue != null)
                persistentState["longValue"] = LongValue;
            if (DoubleValue != null)
                persistentState["doubleValue"] = DoubleValue;
            if (TextValue != null)
                persistentState["textValue"] = TextValue;
            if (TextValue2 != null)
                persistentState["textValue2"] = TextValue2;
            if (byteArrayField.ByteArrayId != null)
                persistentState["byteArrayValueId"] = byteArrayField.ByteArrayId;

            persistentState["sequenceCounter"] = GetSequenceCounter();
            persistentState["concurrentLocal"] = IsConcurrentLocal;
            persistentState["executionId"] = ExecutionId;
            persistentState["taskId"] = TaskId;
            persistentState["caseExecutionId"] = CaseExecutionId;
            persistentState["caseInstanceId"] = CaseInstanceId;
            persistentState["tenantId"] = TenantId;
            persistentState["processInstanceId"] = ProcessInstanceId;

            return persistentState;
        }

        // entity lifecycle /////////////////////////////////////////////////////////

        public virtual void PostLoad()
        {
            // make sure the serializer is initialized
            typedValueField.PostLoad();
        }

        public int RevisionNext
        {
            get { return Revision + 1; }
        }


        public void OnImplicitValueUpdate(ITypedValue updatedValue)
        {
            var targetProcessApplication = GetContextProcessApplication();
            if (targetProcessApplication != null)
                Context.ExecuteWithinProcessApplication<object>(() =>
                    {
                        GetVariableScope()
                            .SetVariableLocal(Name, updatedValue);
                        return null;
                    },
                    targetProcessApplication, new InvocationContext(Execution));
            else
                GetVariableScope()
                    .SetVariableLocal(Name, updatedValue);
        }

        public virtual byte[] ByteArrayValue
        {
            get { return byteArrayField.GetByteArrayValue(); }
            set
            {
                // avoid setting a byte array value for a transient variable because this
                // would create and insert an entity in the data base
                if (!isTransient)
                    byteArrayField.SetByteArrayValue(value);
            }
        }

        // case execution ///////////////////////////////////////////////////////////
        //package org.camunda.bpm.engine.impl.cmmn.entity.runtime
        //public virtual CaseExecutionEntity GetCaseExecution()
        //{
        //    if (CaseExecutionId != null)
        //    {
        //        return context.Impl.Context.CommandContext.CaseExecutionManager.findCaseExecutionById(caseExecutionId);
        //    }
        //    return null;
        //}

        // getters and setters //////////////////////////////////////////////////////

        public string TypeName { get; }


        public object Value
        {
            get
            {
                return typedValueField.GetValue();
            }
        }
        public ITypedValue TypedValue { get; }
        //public string ProcessInstanceId { get; set; }
        //public string Name { get; set; }

        //public string ExecutionId
        //{
        //    get { return executionId; }
        //    set { executionId = value; }
        //}

        //public string CaseInstanceId
        //{
        //    get { return caseInstanceId; }
        //    set { caseInstanceId = value; }
        //}

        //public string CaseExecutionId { get; set; }

        //public string TaskId
        //{
        //    get { return taskId; }
        //    set { taskId = value; }
        //}

        //public string ActivityInstanceId { get; set; }
        public string ErrorMessage { get; }


        public virtual string GetTextValue()
        {
            return TextValue;
        }

        public virtual long? GetLongValue()
        {
            return LongValue;
        }

        public virtual void SetLongValue(long? longValue)
        {
            LongValue = longValue;
        }

        public virtual double? GetDoubleValue()
        {
            return DoubleValue;
        }

        public virtual void SetDoubleValue(double? doubleValue)
        {
            DoubleValue = doubleValue;
        }

        public virtual void SetTextValue(string textValue)
        {
            TextValue = textValue;
        }

        public virtual string GetName()
        {
            return Name;
        }

        //public virtual TypedValueSerializer<?> getSerializer()
        //{
        //    return typedValueField.Serializer;
        //}

        public virtual string GetTextValue2()
        {
            return TextValue2;
        }

        public virtual void SetTextValue2(string textValue2)
        {
            TextValue2 = textValue2;
        }

        private void InitializeInstanceFields()
        {
            byteArrayField = new ByteArrayField(this);
            typedValueField = new TypedValueField(this, true);
        }

        public static VariableInstanceEntity CreateAndInsert(string name, ITypedValue value)
        {
            var variableInstance = Create(name, value, false);
            Insert(variableInstance);

            return variableInstance;
        }

        public static void Insert(VariableInstanceEntity variableInstance)
        {
            Context.CommandContext.VariableInstanceManager.Add(variableInstance);
        }

        public static VariableInstanceEntity Create(string name, ITypedValue value, bool isTransient)
        {
            return new VariableInstanceEntity(name, value, isTransient);
        }

        // lazy initialized relations ///////////////////////////////////////////////


        public virtual void SetExecutionId(string executionId)
        {
            ExecutionId = executionId;
        }

        public virtual void SetCaseInstanceId(string caseInstanceId)
        {
            CaseInstanceId = caseInstanceId;
        }

        public virtual void SetCaseExecutionId(string caseExecutionId)
        {
            CaseExecutionId = caseExecutionId;
        }

        //public virtual void SetCaseExecution(CaseExecutionEntity caseExecution)
        //{
        //    if (caseExecution != null)
        //    {
        //        this.CaseInstanceId = caseExecution.GetCaseInstanceId();
        //        this.CaseExecutionId = caseExecution.Id;
        //        this.TenantId = caseExecution.TenantId;
        //    }
        //    else
        //    {
        //        this.CaseInstanceId = null;
        //        this.CaseExecutionId = null;
        //        this.TenantId = null;
        //    }
        //}

        // byte array value /////////////////////////////////////////////////////////

        // i couldn't find a easy readable way to extract the common byte array value logic
        // into a common class.  therefor it's duplicated in VariableInstanceEntity,
        // HistoricVariableInstance and HistoricDetailVariableInstanceUpdateEntity

        public virtual string GetByteArrayValueId()
        {
            return byteArrayField.ByteArrayId;
        }

        public virtual void SetByteArrayValueId(string byteArrayValueId)
        {
            byteArrayField.ByteArrayId = byteArrayValueId;
        }

        protected internal virtual void DeleteByteArrayValue()
        {
            byteArrayField.DeleteByteArrayValue();
        }

        // type /////////////////////////////////////////////////////////////////////


        public virtual ITypedValue GetTypedValue()
        {
            return typedValueField.GetTypedValue();
        }

        public virtual void ClearValueFields()
        {
            LongValue = null;
            DoubleValue = null;
            TextValue = null;
            TextValue2 = null;
            typedValueField.Clear();

            if (byteArrayField.ByteArrayId != null)
            {
                DeleteByteArrayValue();
                SetByteArrayValueId(null);
            }
        }

        public virtual string GetTypeName()
        {
            return typedValueField.TypeName;
        }

        // execution ////////////////////////////////////////////////////////////////

        protected internal virtual void EnsureExecutionInitialized()
        {
            if (_execution == null && ExecutionId != null)
                _execution = Context.CommandContext.ExecutionManager.FindExecutionById(ExecutionId);
        }

        //public virtual ExecutionEntity GetExecution()
        //{
        //    EnsureExecutionInitialized();
        //    return _execution;
        //}
        /// <summary>
        ///     为EF提供导航属性
        /// </summary>

        //public virtual void setSerializer<T1>(ITypedValueSerializer<T1> serializer) where T1 : ITypedValue
        //{
        //    typedValueField.SerializerName = serializer.GetName();
        //}

        //public virtual void SetSerializerName(string type)
        //{
        //    typedValueField.SerializerName = type;
        //}
        public virtual void SetTask(TaskEntity task)
        {
            if (task != null)
            {
                TaskId = task.Id;
                TenantId = task.TenantId;

                if (task.GetExecution() != null)
                {
                    //SetExecution(task.GetExecution());
                    var t = Execution;
                }
                //if (task.GetCaseExecution() != null)
                //{
                //    //setCaseExecution(task.getCaseExecution());
                //}
            }
            else
            {
                TaskId = null;
                TenantId = null;
                //SetExecution(null);
                Execution = null;
                //setCaseExecution(null);
            }
        }

        public virtual string GetVariableScopeId()
        {
            if (TaskId != null)
                return TaskId;

            if (ExecutionId != null)
                return ExecutionId;

            return CaseExecutionId;
        }

        protected internal virtual IVariableScope GetVariableScope()
        {
            if (TaskId != null)
                return GetTask();
            if (ExecutionId != null)
                //return GetExecution();
                return Execution;
            if (CaseExecutionId != null)
                throw new NotImplementedException();
            return null;
        }

        protected internal virtual TaskEntity GetTask()
        {
            if (TaskId != null)
                return Context.CommandContext.TaskManager.FindTaskById(TaskId);
            return null;
        }

        //sequence counter ///////////////////////////////////////////////////////////

        public virtual long GetSequenceCounter()
        {
            return SequenceCounter;
        }

        public virtual void SetSequenceCounter(long sequenceCounter)
        {
            SequenceCounter = sequenceCounter;
        }


        public virtual bool GetIsConcurrentLocal()
        {
            return IsConcurrentLocal;
        }

        public virtual void SetConcurrentLocal(bool isConcurrentLocal)
        {
            IsConcurrentLocal = isConcurrentLocal;
        }

        protected internal virtual IProcessApplicationReference GetContextProcessApplication()
        {
            if (TaskId != null)
                return ProcessApplicationContextUtil.GetTargetProcessApplication(GetTask());
            if (ExecutionId != null)
                return null;
            if (CaseExecutionId != null)
                return null;
            return null;
        }

        public override string ToString()
        {
            return GetType()
                       .Name + "[id=" + Id + ", revision=" + Revision + ", name=" + Name + ", processInstanceId=" +
                   ProcessInstanceId + ", executionId=" + ExecutionId + ", caseInstanceId=" + CaseInstanceId +
                   ", caseExecutionId=" + CaseExecutionId + ", taskId=" + TaskId + ", activityInstanceId=" +
                   ActivityInstanceId + ", tenantId=" + TenantId + ", longValue=" + LongValue + ", doubleValue=" +
                   DoubleValue + ", textValue=" + TextValue + ", textValue2=" + TextValue2 + ", byteArrayValueId=" +
                   GetByteArrayValueId() + ", configuration=" + configuration + ", isConcurrentLocal=" +
                   IsConcurrentLocal + "]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + (Id == null ? 0 : Id.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (VariableInstanceEntity)obj;
            if (Id == null)
            {
                if (other.Id != null)
                    return false;
            }
            else if (!Id.Equals(other.Id))
            {
                return false;
            }
            return true;
        }

        /// <param name="isTransient">
        ///     <code>true</code>, if the variable is not stored in the data base.
        ///     Default is <code>false</code>.
        /// </param>
        public virtual void SetTransient(bool isTransient)
        {
            this.isTransient = isTransient;
        }

        public virtual string GetTenantId()
        {
            return TenantId;
        }

        public virtual void SetTenantId(string tenantId)
        {
            TenantId = tenantId;
        }

    }
}