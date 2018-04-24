using System;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Variable.Serializer;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricVariableUpdateEventEntity : HistoricDetailEventEntity, IHistoricVariableUpdate, IDbEntityLifecycleAware, IValueFields
    {
        private bool _instanceFieldsInitialized = false;

        public HistoricVariableUpdateEventEntity()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        private void InitializeInstanceFields()
        {
            typedValueField = new TypedValueField(this, false);
            //throw new NotImplementedException();
            byteArrayField = new ByteArrayField(this);
        }
        private const long SerialVersionUid = 1L;
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        protected internal TypedValueField typedValueField;

        protected internal ByteArrayField byteArrayField;
        // getter / setters ////////////////////////////
        [Column("VAR_TYPE")]
        public virtual string SerializerName
        {
            get
            {
                return typedValueField.SerializerName;
            }
            set
            {
                typedValueField.SerializerName = value;
            }
        }

        [Column("NAME")]
        public virtual string VariableName { get; set; }

        [Column("LONG_")]
        public virtual long? LongValue { get; set; }

        [Column("DOUBLE")]
        public virtual double? DoubleValue { get; set; }

        [Column("TEXT")]
        public virtual string TextValue { get; set; }

        [Column("TEXT2")]
        public virtual string TextValue2 { get; set; }

        [NotMapped]
        public virtual byte[] ByteValue { get { return byteArrayField.GetByteArrayValue(); } set { byteArrayField.SetByteArrayValue(value); } }

        [Column("REV")]
        public virtual int Revision { get; set; }

        [Column("BYTEARRAY_ID")]
        public virtual string ByteArrayId
        {
            get
            {
                return byteArrayField.ByteArrayId;
            }
            set
            {
                byteArrayField.ByteArrayId = value;
            }
        }

        [Column("VAR_INST_ID")]
        public virtual string VariableInstanceId { get; set; }


        [NotMapped]
        public virtual string ScopeActivityInstanceId { get; set; }
        public virtual ITypedValue TypedValue
        {
            get
            {
                return typedValueField.TypedValue;
            }
        }
        public virtual string ErrorMessage
        {
            get
            {
                return typedValueField.ErrorMessage;
            }
        }
        public virtual string Name
        {
            get
            {
                return VariableName;
            }
        }
        public virtual object Value
        {
            get
            {
                return typedValueField.GetValue();
            }
        }
        public virtual void PostLoad()
        {
            // make sure the serializer is initialized
            typedValueField.PostLoad();
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual string TypeName
        {
            get
            {
                return typedValueField.TypeName;
            }
        }

        public virtual string VariableTypeName
        {
            get
            {
                return TypeName;
            }
        }

        [NotMapped]
        public virtual byte[] ByteArrayValue
        {
            get
            {
                return byteArrayField.GetByteArrayValue();
            }
            set
            {
                byteArrayField.SetByteArrayValue(value);
            }
        }

        public override string ToString()
        {
            return GetType().Name + "[variableName=" + VariableName + ", variableInstanceId=" + VariableInstanceId +
                   ", revision=" + Revision + ", serializerName=" + SerializerName + ", longValue=" + LongValue +
                   ", doubleValue=" + DoubleValue + ", textValue=" + TextValue + ", textValue2=" + TextValue2 +
                   ", byteArrayId=" + ByteArrayId + ", activityInstanceId=" + ActivityInstanceId +
                   ", scopeActivityInstanceId=" + ScopeActivityInstanceId + ", eventType=" + EventType +
                   ", executionId=" + ExecutionId + ", id=" + Id + ", processDefinitionId=" + ProcessInstanceId +
                   ", processInstanceId=" + ProcessInstanceId + ", taskId=" + TaskId + ", timestamp=" + TimeStamp +
                   ", tenantId=" + TenantId + "]";
        }
    }
}