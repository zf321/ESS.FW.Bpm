
namespace ESS.FW.Bpm.Model.Xml.impl.util
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class QName
    {

        private readonly string _qualifier;
        private readonly string _localName;

        public QName(string localName) : this(null, localName)
        {
        }

        public QName(string qualifier, string localName)
        {
            this._localName = localName;
            this._qualifier = qualifier;
        }

        public virtual string Qualifier
        {
            get
            {
                return _qualifier;
            }
        }

        public virtual string LocalName
        {
            get
            {
                return _localName;
            }
        }

        public static QName ParseQName(string identifier)
        {
            string qualifier;
            string localName;

            string[] split = identifier.Split(':');
            if (split.Length == 2)
            {
                qualifier = split[0];
                localName = split[1];
            }
            else
            {
                qualifier = null;
                localName = split[0];
            }

            return new QName(qualifier, localName);
        }

        public override string ToString()
        {
            return Combine(_qualifier, _localName);
        }

        public static string Combine(string qualifier, string localName)
        {
            if (string.IsNullOrEmpty(qualifier) || qualifier.Length == 0)
            {
                return localName;
            }
            else
            {
                return qualifier + ":" + localName;
            }
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((string.IsNullOrEmpty(_localName)) ? 0 : _localName.GetHashCode());
            result = prime * result + ((string.IsNullOrEmpty(_qualifier)) ? 0 : _qualifier.GetHashCode());
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
            QName other = (QName)obj;
            if (string.IsNullOrEmpty(_localName))
            {
                if (!string.IsNullOrEmpty(other._localName))
                {
                    return false;
                }
            }
            else if (_localName != other._localName)
            {
                return false;
            }
            if (string.IsNullOrEmpty(_qualifier))
            {
                if (!string.IsNullOrEmpty(other._qualifier))
                {
                    return false;
                }
            }
            else if (_qualifier != other._qualifier)
            {
                return false;
            }
            return true;
        }
    }
}