//using System;
//using ESS.FW.Bpm.Engine.History;
//using ESS.FW.Bpm.Engine.History.Impl.Event;
//using ESS.FW.Bpm.Engine.Impl;
//using ESS.FW.Bpm.Engine.Impl.DB;
//using ESS.FW.Bpm.Engine.Impl.DB.EntityManager;
//using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
//using ESS.FW.Bpm.Engine.Variable.Serializer;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Bpm.Engine.Variable.Value;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace ESS.FW.Bpm.Engine.Persistence.Entity
//{

//    /// <summary>
//    /// 
//    /// </summary>
//    [Obsolete("Ê¹ÓÃHistoricVariableUpdateEventEntity",true)]
//    public class HistoricDetailVariableInstanceUpdateEntity : HistoricDetailEventEntity, IValueFields, IHistoricVariableUpdate, IDbEntityLifecycleAware
//    {
//        private bool _instanceFieldsInitialized = false;

//        public HistoricDetailVariableInstanceUpdateEntity()
//        {
//            if (!_instanceFieldsInitialized)
//            {
//                InitializeInstanceFields();
//                _instanceFieldsInitialized = true;
//            }
//        }

//        private void InitializeInstanceFields()
//        {
//            typedValueField = new TypedValueField(this, false);
//            //throw new NotImplementedException();
//            byteArrayField = new ByteArrayField(this);
//        }


//        private const long SerialVersionUid = 1L;
//        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

//        protected internal TypedValueField typedValueField;

//        protected internal ByteArrayField byteArrayField;

//        public virtual object Value
//        {
//            get
//            {
//                return typedValueField.GetValue();
//            }
//        }
//        public virtual ITypedValue TypedValue
//        {
//            get
//            {
//                return typedValueField.TypedValue;
//            }
//        }

//        public virtual ITypedValue GetTypedValue(bool deserializeValue)
//        {
//            return typedValueField.GetTypedValue(deserializeValue);
//        }

//        public override void Delete()
//        {
//            throw new NotImplementedException();
//            //DbEntityManager dbEntityManger = context.Impl.Context.CommandContext.DbEntityManager;

//            //dbEntityManger.Delete(this);

//            byteArrayField.DeleteByteArrayValue();
//        }

//        //JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//        //ORIGINAL LINE: public org.camunda.bpm.engine.impl.Variable.serializer.TypedValueSerializer<?> getSerializer()
//        //public virtual TypedValueSerializer?> Serializer
//        //{
//        // get
//        // {
//        //return typedValueField.Serializer;
//        // }
//        //}

//        public virtual string ErrorMessage
//        {
//            get
//            {
//                return typedValueField.ErrorMessage;
//            }
//        }
//        [Column("BYTEARRAY_ID")]
//        public  string ByteArrayId
//        {
//            get
//            {
//                return byteArrayField.ByteArrayId;
//            }
//            set
//            {
//                byteArrayField.ByteArrayId = value;
//            }
//        }
//        [Column("VAR_TYPE")]
//        public  string SerializerName
//        {
//            get
//            {
//                return typedValueField.SerializerName;
//            }
//            set
//            {
//                typedValueField.SerializerName = value;
//            }
//        }

//        public virtual string ByteArrayValueId
//        {
//            get
//            {
//                return byteArrayField.ByteArrayId;
//            }
//        }
//        [NotMapped]
//        public virtual byte[] ByteArrayValue
//        {
//            get
//            {
//                return byteArrayField.GetByteArrayValue();
//            }
//            set
//            {
//                byteArrayField.SetByteArrayValue(value);
//            }
//        }


//        public virtual string Name
//        {
//            get
//            {
//                return VariableName;
//            }
//        }

//        // entity lifecycle /////////////////////////////////////////////////////////

//        public virtual void PostLoad()
//        {
//            // make sure the serializer is initialized
//            typedValueField.PostLoad();
//        }

//        // getters and setters //////////////////////////////////////////////////////

//        public virtual string TypeName
//        {
//            get
//            {
//                return typedValueField.TypeName;
//            }
//        }

//        public virtual string VariableTypeName
//        {
//            get
//            {
//                return TypeName;
//            }
//        }

//        //public virtual DateTime Time
//        //{
//        //    get
//        //    {
//        //        return TimeStamp;
//        //    }
//        //}

//        #region ÒÆ³ýÒ»²ã¼Ì³Ð
//        // getter / setters ////////////////////////////
//        //[Column("VAR_TYPE")]
//        //public virtual string SerializerName { get; set; }

//        [Column("NAME")]
//        public virtual string VariableName { get; set; }

//        [Column("LONG_")]
//        public virtual long? LongValue { get; set; }

//        [Column("DOUBLE")]
//        public virtual double? DoubleValue { get; set; }

//        [Column("TEXT")]
//        public virtual string TextValue { get; set; }

//        [Column("TEXT2")]
//        public virtual string TextValue2 { get; set; }

//        [NotMapped]
//        public virtual byte[] ByteValue { get; set; }

//        [Column("REV")]
//        public virtual int Revision { get; set; }

//        //[Column("BYTEARRAY_ID")]
//        //public virtual string ByteArrayId { set; get; }

//        [Column("VAR_INST_ID")]
//        public virtual string VariableInstanceId { get; set; }

//        [NotMapped]
//        public virtual string ScopeActivityInstanceId { get; set; }
//        #endregion

//        public override string ToString()
//        {
//            return this.GetType().Name + "[variableName=" + VariableName + ", variableInstanceId=" + VariableInstanceId + ", revision=" + Revision + ", serializerName=" + SerializerName + ", longValue=" + LongValue + ", doubleValue=" + DoubleValue + ", textValue=" + TextValue + ", textValue2=" + TextValue2 + ", byteArrayId=" + ByteArrayId + ", activityInstanceId=" + ActivityInstanceId + ", eventType=" + EventType + ", executionId=" + ExecutionId + ", id=" + Id + ", processDefinitionId=" + ProcessInstanceId + ", processInstanceId=" + ProcessInstanceId + ", taskId=" + TaskId + ", timestamp=" + TimeStamp + "]";
//        }




//    }

//}