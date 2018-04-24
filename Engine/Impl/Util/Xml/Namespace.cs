namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///     
    /// </summary>
    public class Namespace
    {
        private readonly string _alternativeUri;

        private readonly string _namespaceUri;

        public Namespace(string namespaceUri) : this(namespaceUri, null)
        {
        }

        /// <summary>
        ///     Creates a namespace with an alternative uri.
        /// </summary>
        /// <param name="namespaceUri"> </param>
        /// <param name="alternativeUri"> </param>
        public Namespace(string namespaceUri, string alternativeUri)
        {
            this._namespaceUri = namespaceUri;
            this._alternativeUri = alternativeUri;
        }

        public virtual string NamespaceUri
        {
            get { return _namespaceUri; }
        }

        public virtual string AlternativeUri
        {
            get { return _alternativeUri; }
        }

        /// <summary>
        ///     If a namespace has changed over time it could feel responsible for handling
        ///     the older one.
        ///     @return
        /// </summary>
        public virtual bool HasAlternativeUri()
        {
            return AlternativeUri != null;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + (ReferenceEquals(_namespaceUri, null) ? 0 : _namespaceUri.GetHashCode());
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
            var other = (Namespace) obj;
            if (ReferenceEquals(_namespaceUri, null))
            {
                if (!ReferenceEquals(other._namespaceUri, null))
                    return false;
            }
            else if (!_namespaceUri.Equals(other._namespaceUri))
            {
                return false;
            }
            return true;
        }
    }
}