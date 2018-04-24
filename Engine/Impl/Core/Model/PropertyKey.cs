namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     Key of a typed property.
    /// </summary>
    /// <param name="T">
    ///     the type of the property value
    ///     
    /// </param>
    /// <seealso cref=
    /// <seealso cref="PropertyListKey" />
    /// ,
    /// <seealso cref="PropertyMapKey" />
    /// </seealso>
    public class PropertyKey<T>
    {
        protected internal readonly string name;

        public PropertyKey(string name)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get { return name; }
        }

        public override string ToString()
        {
            return "PropertyKey [name=" + name + "]";
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + (ReferenceEquals(name, null) ? 0 : name.GetHashCode());
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
            var other = (PropertyKey<object>) obj;
            if (ReferenceEquals(name, null))
            {
                if (!ReferenceEquals(other.name, null))
                    return false;
            }
            else if (!name.Equals(other.name))
            {
                return false;
            }
            return true;
        }
    }
}