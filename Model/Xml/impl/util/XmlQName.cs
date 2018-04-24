using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.instance;



namespace ESS.FW.Bpm.Model.Xml.impl.util
{
    /// <summary>
	/// 
	/// </summary>
	public class XmlQName
    {
        public static string XmlnsAttributeNsUri =
        "http://www.w3.org/2000/xmlns/";
        public static readonly IDictionary<string, string> KnownPrefixes;
        static XmlQName()
        {
            KnownPrefixes = new Dictionary<string, string>();
            KnownPrefixes["http://www.camunda.com/fox"] = "fox";
            KnownPrefixes["http://activiti.org/bpmn"] = "camunda";
            KnownPrefixes["http://camunda.org/schema/1.0/bpmn"] = "camunda";
            KnownPrefixes["http://www.omg.org/spec/BPMN/20100524/MODEL"] = "bpmn2";
            KnownPrefixes["http://www.omg.org/spec/BPMN/20100524/DI"] = "bpmndi";
            KnownPrefixes["http://www.omg.org/spec/DD/20100524/DI"] = "di";
            KnownPrefixes["http://www.omg.org/spec/DD/20100524/DC"] = "dc";
            KnownPrefixes[XmlnsAttributeNsUri] = "";
        }

        protected internal IDomElement RootElement;
        protected internal IDomElement Element;

        protected internal string localName;
        protected internal string namespaceUri;
        protected internal string Prefix;

        public XmlQName(IDomDocument document, string namespaceUri, string localName) : this(document, null, namespaceUri, localName)
        {
        }

        public XmlQName(IDomElement element, string namespaceUri, string localName) : this(element.Document, element, namespaceUri, localName)
        {
        }

        public XmlQName(IDomDocument document, IDomElement element, string namespaceUri, string localName)
        {
            this.RootElement = document.RootElement;
            this.Element = element;
            this.localName = localName;
            this.namespaceUri = namespaceUri;
            this.Prefix = null;
        }

        public virtual string NamespaceUri
        {
            get
            {
                return namespaceUri;
            }
        }

        public virtual string LocalName
        {
            get
            {
                return localName;
            }
        }

        public virtual string PrefixedName
        {
            get
            {
                if (string.ReferenceEquals(Prefix, null))
                {
                    lock (this)
                    {
                        if (string.ReferenceEquals(Prefix, null))
                        {
                            this.Prefix = DeterminePrefixAndNamespaceUri();
                        }
                    }
                }
                return QName.Combine(Prefix, localName);
            }
        }

        public virtual bool HasLocalNamespace()
        {
            if (Element != null)
            {
                return Element.NamespaceUri == namespaceUri;
            }
            else
            {
                return false;
            }
        }

        private string DeterminePrefixAndNamespaceUri()
        {
            if (!string.ReferenceEquals(namespaceUri, null))
            {
                if (RootElement != null && namespaceUri.Equals(RootElement.NamespaceUri))
                {
                    // global namespaces do not have a prefix or namespace URI
                    return null;
                }
                else
                {
                    // lookup for prefix
                    string lookupPrefix = this.LookupPrefix();
                    if (string.ReferenceEquals(lookupPrefix, null) && RootElement != null)
                    {
                        // if no prefix is found we generate a new one
                        // search for known prefixes
                        string knownPrefix = KnownPrefixes[namespaceUri];
                        if (string.ReferenceEquals(knownPrefix, null))
                        {
                            // generate namespace
                            return RootElement.RegisterNamespace(namespaceUri);
                        }
                        else if (knownPrefix.Length == 0)
                        {
                            // ignored namespace
                            return null;
                        }
                        else
                        {
                            // register known prefix
                            RootElement.RegisterNamespace(knownPrefix, namespaceUri);
                            return knownPrefix;
                        }
                    }
                    else
                    {
                        return lookupPrefix;
                    }
                }
            }
            else
            {
                // no namespace so no prefix
                return null;
            }
        }

        private string LookupPrefix()
        {
            if (!string.ReferenceEquals(namespaceUri, null))
            {
                string lookupPrefix = null;
                if (Element != null)
                {
                    lookupPrefix = Element.LookupPrefix(namespaceUri);
                }
                else if (RootElement != null)
                {
                    lookupPrefix = RootElement.LookupPrefix(namespaceUri);
                }
                return lookupPrefix;
            }
            else
            {
                return null;
            }
        }

    }

}