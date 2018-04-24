namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     Key of a map property.
    /// </summary>
    /// <param name="K"> the type of keys maintained by the map </param>
    /// <param name="V">
    ///     the type of mapped values
    ///     
    /// </param>
    public class PropertyMapKey<TK, TV>
    {
        protected internal readonly string name;
        protected internal bool allowOverwrite = true;

        public PropertyMapKey(string name) : this(name, true)
        {
        }

        public PropertyMapKey(string name, bool allowOverwrite)
        {
            this.name = name;
            this.allowOverwrite = allowOverwrite;
        }

        public virtual string Name => name;

        public virtual bool AllowsOverwrite => allowOverwrite;

        public override string ToString()
        {
            return "PropertyMapKey [name=" + name + "]";
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
            var other = (PropertyMapKey<object, object>) obj;
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