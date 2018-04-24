using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Common.Components;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricVariableInstanceEntity : IValueFields, IHistoricVariableInstance, IDbEntity, IHasDbRevision,
        IDbEntityLifecycleAware
    {
        public static string StateCreated = "CREATED";
        public static string StateDeleted = "DELETED";
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
        private readonly bool _instanceFieldsInitialized;

        protected internal ByteArrayField byteArrayField;

        protected internal TypedValueField typedValueField;

        public HistoricVariableInstanceEntity()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public HistoricVariableInstanceEntity(HistoricVariableUpdateEventEntity historyEvent)
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
            UpdateFromEvent(historyEvent);
        }
        private void InitializeInstanceFields()
        {
            byteArrayField = new ByteArrayField(this);
            typedValueField = new TypedValueField(this, false);
        }

        public virtual ITypedValue TypedValue
        {
            get { return typedValueField.TypedValue; }
        }
        public virtual ITypedValueSerializer Serializer
        {
            get
            {
                return typedValueField.Serializer;
            }
        }


        public virtual string ByteArrayId
        {
            get { return byteArrayField.ByteArrayId; }
            set {
                byteArrayField.ByteArrayId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    if(context.Impl.Context.CommandContext != null)
                    {
                        typedValueField.SetValue(new UntypedValueImpl(Context.CommandContext.ByteArrayManager.Get(value).Bytes));
                    }
                    else
                    {
                        //TODO ÐÂ½¨ÁËscope
                        using (var scope = ObjectContainer.Current.BeginLifetimeScope())
                        {
                            IByteArrayManager db = scope.Resolve<IByteArrayManager>();
                            //initValue = ;
                            SetByteArrayValue(db.Get(value));
                            //using (MemoryStream ms=new MemoryStream(db.Get(value).Bytes))
                            //{
                            //    BinaryFormatter formatter = new BinaryFormatter();
                            //    initValue = formatter.Deserialize(ms);
                            //}
                            //typedValueField.SetValue(new UntypedValueImpl(db.Get(value).Bytes));
                        }
                    }
                }
            }
        }

        // getters and setters //////////////////////////////////////////////////////
        public virtual string SerializerName
        {
            get { return typedValueField.SerializerName; }
            set { typedValueField.SerializerName = value; }
        }

        public virtual object GetPersistentState()
        {
            IList<object> state = new List<object>(8);
            state.Add(SerializerName);
            state.Add(TextValue);
            state.Add(TextValue2);
            state.Add(DoubleValue);
            state.Add(LongValue);
            state.Add(ProcessDefinitionId);
            state.Add(ProcessDefinitionKey);
            state.Add(ByteArrayId);
            return state;
        }


        // entity lifecycle /////////////////////////////////////////////////////////

        public virtual void PostLoad()
        {
            // make sure the serializer is initialized
            typedValueField.PostLoad();
        }

        public virtual int RevisionNext
        {
            get { return Revision + 1; }
        }

        public virtual int Revision { get; set; }
        
        public virtual object Value
        {
            get {
                return typedValueField.GetValue();
            }
        }

        
        public virtual string TypeName
        {
            get { return typedValueField.TypeName; }
        }
        
        public virtual string VariableTypeName
        {
            get { return TypeName; }
        }

        public virtual string VariableName
        {
            get { return Name; }
        }

        public virtual string Id { get; set; }
        public string State { get; set; } = "CREATED";

        public virtual string ProcessDefinitionKey { get; set; }


        public virtual string ProcessDefinitionId { get; set; }


        public virtual string ProcessInstanceId { get; set; }


        public virtual string TaskId { get; set; }


        public virtual string ExecutionId { get; set; }


        [Obsolete]
        public virtual string ActivtyInstanceId
        {
            get { return ActivityInstanceId; }
        }

        public virtual string ActivityInstanceId { get; set; }


        public virtual string CaseDefinitionKey { get; set; }


        public virtual string CaseDefinitionId { get; set; }


        public virtual string CaseInstanceId { get; set; }


        public virtual string CaseExecutionId { get; set; }


        public virtual string ErrorMessage
        {
            get { return typedValueField.ErrorMessage; }
        }

        public virtual string TenantId { get; set; }
        [NotMapped]
        public DateTime? StartTime { get; set; }
        [NotMapped]
        public DateTime? EndTime { get; set; }



        public virtual byte[] ByteArrayValue
        {
            get { return byteArrayField.GetByteArrayValue(); }
            set { byteArrayField.SetByteArrayValue(value); }
        }


        public virtual string Name { get; set; }


        public virtual long? LongValue { get; set; }


        public virtual double? DoubleValue { get; set; }


        public virtual string TextValue { get; set; }


        public virtual string TextValue2 { get; set; }

        

        public virtual void UpdateFromEvent(HistoricVariableUpdateEventEntity historyEvent)
        {
            Id = historyEvent.VariableInstanceId;
            ProcessDefinitionKey = historyEvent.ProcessDefinitionKey;
            ProcessDefinitionId = historyEvent.ProcessDefinitionId;
            ProcessInstanceId = historyEvent.ProcessInstanceId;
            TaskId = historyEvent.TaskId;
            ExecutionId = historyEvent.ExecutionId;
            ActivityInstanceId = historyEvent.ScopeActivityInstanceId;
            TenantId = historyEvent.TenantId;
            CaseDefinitionKey = historyEvent.CaseDefinitionKey;
            CaseDefinitionId = historyEvent.CaseDefinitionId;
            CaseInstanceId = historyEvent.CaseInstanceId;
            CaseExecutionId = historyEvent.CaseExecutionId;
            Name = historyEvent.VariableName;
            LongValue = historyEvent.LongValue;
            DoubleValue = historyEvent.DoubleValue;
            TextValue = historyEvent.TextValue;
            TextValue2 = historyEvent.TextValue2;

            SerializerName = historyEvent.SerializerName;

            byteArrayField.DeleteByteArrayValue();

            if (historyEvent.ByteValue != null)
                ByteArrayValue=(historyEvent.ByteValue);
        }

        public virtual void Delete()
        {
            byteArrayField.DeleteByteArrayValue();

            Context.CommandContext.HistoricVariableInstanceManager.Delete(this);
        }

        public virtual ITypedValue GetTypedValue(bool deserializeValue)
        {
            return typedValueField.GetTypedValue(deserializeValue);
        }


        public virtual void SetByteArrayValue(ResourceEntity byteArrayValue)
        {
            byteArrayField.SetByteArrayValue(byteArrayValue);
        }

        public override string ToString()
        {
            return GetType()
                       .Name + "[id=" + Id + ", processDefinitionKey=" + ProcessDefinitionKey + ", processDefinitionId=" +
                   ProcessDefinitionId + ", processInstanceId=" + ProcessInstanceId + ", taskId=" + TaskId +
                   ", executionId=" + ExecutionId + ", tenantId=" + TenantId + ", activityInstanceId=" +
                   ActivityInstanceId + ", caseDefinitionKey=" + CaseDefinitionKey + ", caseDefinitionId=" +
                   CaseDefinitionId + ", caseInstanceId=" + CaseInstanceId + ", caseExecutionId=" + CaseExecutionId +
                   ", name=" + Name + ", revision=" + Revision + ", serializerName=" + SerializerName + ", longValue=" +
                   LongValue + ", doubleValue=" + DoubleValue + ", textValue=" + TextValue + ", textValue2=" +
                   TextValue2 + ", byteArrayId=" + ByteArrayId + "]";
        }
        
    }
}