using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Variable.Value;


namespace ESS.FW.Bpm.Engine.Variable.Type.Impl
{
    /// <summary>
	/// 
	/// </summary>
	[Serializable]
    public abstract class AbstractValueTypeImpl : IValueType
    {
        public abstract ITypedValue CreateValue(object value, IDictionary<string, object> valueInfo);
        public abstract IDictionary<string, object> GetValueInfo(ITypedValue typedValue);
        public abstract bool IsPrimitiveValueType { get; }

        private const long SerialVersionUid = 1L;

        protected internal string name;

        public AbstractValueTypeImpl(string name)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
        }

        public override string ToString()
        {
            return name;
        }

        public virtual bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public virtual IValueType Parent
        {
            get
            {
                return null;
            }
        }

        public virtual bool CanConvertFromTypedValue(ITypedValue typedValue)
        {
            return false;
        }

        public virtual ITypedValue ConvertFromTypedValue(ITypedValue typedValue)
        {
            throw UnsupportedConversion(typedValue.Type);
        }

        protected internal virtual System.ArgumentException UnsupportedConversion(IValueType typeToConvertTo)
        {
            return new System.ArgumentException("The type " + Name + " supports no conversion from type: " + typeToConvertTo.Name);
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((string.ReferenceEquals(name, null)) ? 0 : name.GetHashCode());
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
            AbstractValueTypeImpl other = (AbstractValueTypeImpl)obj;
            if (string.ReferenceEquals(name, null))
            {
                if (!string.ReferenceEquals(other.name, null))
                {
                    return false;
                }
            }
            else if (!name.Equals(other.name))
            {
                return false;
            }
            return true;
        }

    }

}