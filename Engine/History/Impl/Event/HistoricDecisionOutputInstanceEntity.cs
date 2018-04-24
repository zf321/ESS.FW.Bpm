using System;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricDecisionOutputInstanceEntity : HistoryEvent, IHistoricDecisionOutputInstance , IValueFields
    {
        private const long SerialVersionUid = 1L;
        private readonly bool _instanceFieldsInitialized;

        protected internal ByteArrayField byteArrayField;

        protected internal TypedValueField typedValueField;

        public HistoricDecisionOutputInstanceEntity()
        {
            if (!_instanceFieldsInitialized)
                _instanceFieldsInitialized = true;
        }

        public virtual ITypedValue TypedValue
        {
            get { return typedValueField.TypedValue; }
        }

        public virtual string Name
        {
            get { return VariableName; }
        }

        public virtual string TextValue { get; set; }


        public virtual string TextValue2 { get; set; }


        public virtual long? LongValue { get; set; }


        public virtual double? DoubleValue { get; set; }


        public virtual string ByteArrayId
        {
            get { return byteArrayField.ByteArrayId; }
            set { byteArrayField.ByteArrayId = value; }
        }


        public virtual byte[] ByteArrayValue
        {
            get { return byteArrayField.GetByteArrayValue(); }
            set { byteArrayField.SetByteArrayValue(value); }
        }

        public virtual string SerializerName
        {
            get { return typedValueField.SerializerName; }
            set { typedValueField.SerializerName = value; }
        }


        public virtual string TenantId { get; set; }

        public virtual string DecisionInstanceId { get; set; }

        public virtual string ClauseId { get; set; }

        public virtual string ClauseName { get; set; }

        public virtual string RuleId { get; set; }

        public virtual int? RuleOrder { get; set; }


        public virtual string VariableName { get; set; }

        [NotMapped]
        public virtual string TypeName
        {
            get { return typedValueField.TypeName; }
            set { typedValueField.SerializerName = value; }
        }
        public virtual string ErrorMessage
        {
            get { return typedValueField.ErrorMessage; }
        }

        public object Value
        {
            get
            {
                return GetValue();
            }
            
        }



        public string GetTextValue()
        {
            return TextValue;
        }

        public void SetTextValue(string textValue)
        {
            this.TextValue = textValue;
        }

        public string GetTextValue2()
        {
            return TextValue2;
        }

        public void SetTextValue2(string textValue)
        {
            this.TextValue2 = textValue;
        }

        public long? GetLongValue()
        {
            return LongValue;
        }

        public void SetLongValue(long? longValue)
        {
            this.LongValue = longValue;
        }

        public double? GetDoubleValue()
        {
            return DoubleValue;
        }

        public void SetDoubleValue(double? doubleValue)
        {
            this.DoubleValue = doubleValue;
        }

        public byte[] GetByteArrayValue()
        {
            return ByteArrayValue;
        }

        public void SetByteArrayValue(byte[] byteArrayValue)
        {
            ByteArrayValue = byteArrayValue;
        }

        public string GetName()
        {
            return Name;
        }

        private void InitializeInstanceFields()
        {
            byteArrayField = new ByteArrayField(this);
            typedValueField = new TypedValueField(this, false);
        }


        public virtual object GetValue()
        {
            return typedValueField.GetValue();
        }

        public virtual ITypedValue getTypedValue(bool deserializeValue)
        {
            return typedValueField.GetTypedValue(deserializeValue);
        }


        //public virtual void setValue(ITypedValue typedValue)
        //{
        //    typedValueField.setValue(typedValue);
        //}


        public virtual void Delete()
        {
            byteArrayField.DeleteByteArrayValue();

            Context.CommandContext.GetDbEntityManager<HistoricDecisionOutputInstanceEntity>().Delete(this);//.DbEntityManager.delete(this);
        }
    }
}