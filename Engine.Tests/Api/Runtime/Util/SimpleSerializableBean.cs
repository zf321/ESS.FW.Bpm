using System;

namespace Engine.Tests.Api.Runtime.Util
{
    [Serializable]
    public class SimpleSerializableBean
    {
        private const long serialVersionUID = 1L;

        protected internal int intProperty;

        public SimpleSerializableBean()
        {
        }

        public SimpleSerializableBean(int intProperty)
        {
            this.intProperty = intProperty;
        }

        public virtual int IntProperty
        {
            get { return intProperty; }
            set { intProperty = value; }
        }


        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime * result + intProperty;
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
            var other = (SimpleSerializableBean) obj;
            if (intProperty != other.intProperty)
                return false;
            return true;
        }
    }
}