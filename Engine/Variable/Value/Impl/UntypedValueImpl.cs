using System;
using ESS.FW.Bpm.Engine.Variable.Type;

namespace ESS.FW.Bpm.Engine.Variable.Value.Impl
{
    /// <summary>
    ///     Used when the type of an object has not been specified by the user and
    ///     needs to be autodetected.
    /// </summary>
    [Serializable]
    public class UntypedValueImpl : ITypedValue
    {
        private const long SerialVersionUid = 1L;

        protected internal object value;

        public UntypedValueImpl(object @object)
        {
            value = @object;
        }

        public virtual object Value
        {
            get { return value; }
        }

        public virtual IValueType Type
        {
            get
            {
                // no type
                return null;
            }
        }

        public override string ToString()
        {
            return "Untyped value '" + value + "'";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + (value == null ? 0 : value.GetHashCode());
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
            var other = (UntypedValueImpl) obj;
            if (value == null)
            {
                if (other.value != null)
                    return false;
            }
            else if (!value.Equals(other.value))
            {
                return false;
            }
            return true;
        }
    }
}