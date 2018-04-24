using System;
using ESS.FW.Bpm.Engine.Variable.Value;
using ESS.FW.Bpm.Engine.Persistence.Entity.Util;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.context.Impl;

namespace ESS.FW.Bpm.Engine.History.Impl.Event
{

    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class HistoricDecisionInputInstanceEntity : HistoryEvent, IHistoricDecisionInputInstance, Variable.Serializer.IValueFields
    {
        private const long SerialVersionUid = 1L;
        private readonly bool _instanceFieldsInitialized;

        //protected internal string DecisionInstanceId;

        //protected internal long? LongValue;

        //protected internal string TenantId;
        //protected internal string TextValue;
        //protected internal string TextValue2;

        public HistoricDecisionInputInstanceEntity()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }

        public virtual string Name
        {
            get
            {
                // used for save a byte value
                return ClauseId;
            }
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
        [NotMapped]
        protected internal ByteArrayField byteArrayField
        {
            get { return new ByteArrayField((INameable)this); }
            set { this.ClauseId = value.NameProvider.Name; }
        }
        protected internal TypedValueField typedValueField;

        public virtual string DecisionInstanceId { get; set; }


        public virtual string ClauseId { get; set; }


        public virtual string ClauseName { get; set; }


        public virtual string TypeName
        {
            get { return typedValueField.TypeName; }
            set { typedValueField.SerializerName = value; }
        }

        public virtual ITypedValue TypedValue
        {
            get { return typedValueField.TypedValue; }
        }

        public virtual string ErrorMessage
        {
            get { return typedValueField.ErrorMessage; }
        }

        public object Value
        {
            get { throw new NotImplementedException(); }
        }
        

        private void InitializeInstanceFields()
        {
            byteArrayField = new ByteArrayField(this);
            typedValueField = new TypedValueField(this, false);
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
            return byteArrayField.GetByteArrayValue();
        }

        public void SetByteArrayValue(byte[] byteArrayValue)
        {
            this.byteArrayField.SetByteArrayValue(byteArrayValue);
        }

        public string GetName()
        {
            return Name;
        }

        public void Delete()
        {
            byteArrayField.DeleteByteArrayValue();
            Context.CommandContext.GetDbEntityManager<HistoricDecisionInputInstanceEntity>().Delete(this);
        }


        //public virtual void setValue(ITypedValue typedValue)
        //}
        //    return typedValueField.getTypedValue(deserializeValue);
        //{

        public virtual ITypedValue GetTypedValue(bool deserializeValue)
        {
            return (ITypedValue)typedValueField.GetValue();
        }
            


        //public virtual object getValue()
    }
}