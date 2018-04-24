using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class AbstractTypedValue<T> : ITypedValue
    {
        private const long SerialVersionUid = 1L;

        protected internal IValueType type;

        protected internal T value;

        public AbstractTypedValue(T value, IValueType type)
        {
            this.value = value;
            this.type = type;
        }

        public virtual object Value
        {
            get { return value; }
        }

        public virtual IValueType Type
        {
            get {
                if (type == null && Value != null)
                {
                    return ((ITypedValue)value).Type;
                }
                return type;
            }
        }

        public override string ToString()
        {
            return "Value '" + value + "' of type '" + type + "'";
        }
    }
}