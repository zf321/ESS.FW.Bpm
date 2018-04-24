using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Xml.impl.instance
{

    /// <summary>
	/// 
	/// </summary>
	public class DomDocumentImpl : IDomDocument
    {

        public const string GenericNsPrefix = "ns";

        private readonly XmlDocument _document;

        public DomDocumentImpl(XmlDocument document)
        {
            this._document = document;
        }

        public virtual IDomElement RootElement
        {
            get
            {
                lock (_document)
                {
                    XmlElement documentElement = _document.DocumentElement;
                    if (documentElement != null)
                    {
                        return new DomElementImpl(documentElement);
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            set
            {
                lock (_document)
                {
                    XmlElement documentElement = _document.DocumentElement;
                    XmlElement newDocumentElement = ((DomElementImpl)value).XmlElement;
                    if (documentElement != null)
                    {
                        _document.ReplaceChild(newDocumentElement, documentElement);
                    }
                    else
                    {
                        _document.AppendChild(newDocumentElement);
                    }
                }
            }
        }


        public virtual IDomElement CreateElement(string namespaceUri, string localName)
        {
            lock (_document)
            {
                XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
                XmlElement element = _document.CreateElement(xmlQName.PrefixedName, xmlQName.NamespaceUri);
                return new DomElementImpl(element);
            }
        }

        public virtual IDomElement GetElementById(string id)
        {
            lock (_document)
            {
                XmlNode xml = _document.DocumentElement.SelectSingleNode(string.Format("//*[@id='{0}']", id));
                return new DomElementImpl((XmlElement)xml);
            }
        }
        //TODO 重写document.getElementById
        //private void GetElementByIdExt(string id, XmlNode node, ref XmlElement element)
        //{
        //    if (element != null) return;
        //    if (element == null && node.Attributes != null)
        //    {
        //        foreach (XmlAttribute item in node.Attributes)
        //        {
        //            if (item.Name == "id" && item.Value == id)
        //            {
        //                element = item.OwnerElement;
        //                return;
        //            }
        //        }
        //    }
        //    if (element == null)
        //    {
        //        GetElementByIdExt_2(id, node, ref element);
        //    }
        //    //return element;
        //}
        //private void GetElementByIdExt_2(string id, XmlNode node, ref XmlElement element)
        //{
        //    if (element == null)
        //    {
        //        if (node.ChildNodes.Count > 0)
        //        {
        //            foreach (XmlNode item in node.ChildNodes)
        //            {
        //                if (element != null) return;
        //                if (element == null)
        //                {
        //                    GetElementByIdExt(id, item, ref element);
        //                }

        //            }
        //        }
        //    }
        //}

        public virtual IList<IDomElement> GetElementsByNameNs(string namespaceUri, string localName)
        {
            lock (_document)
            {
                XmlNodeList elementsByTagNameNs = _document.GetElementsByTagName(localName, namespaceUri);
                return DomUtil.FilterNodeListByName(elementsByTagNameNs, namespaceUri, localName);
            }
        }

        public virtual XmlDocument DomSource
        {
            get
            {
                //return new XmlDocument(_document);
                return _document;
            }
        }

        public virtual string RegisterNamespace(string namespaceUri)
        {
            lock (_document)
            {
                IDomElement rootElement = RootElement;
                if (rootElement != null)
                {
                    return rootElement.RegisterNamespace(namespaceUri);
                }
                else
                {
                    throw new ModelException("Unable to define a new namespace without a root document element");
                }
            }
        }

        public virtual void RegisterNamespace(string prefix, string namespaceUri)
        {
            lock (_document)
            {
                IDomElement rootElement = RootElement;
                if (rootElement != null)
                {
                    rootElement.RegisterNamespace(prefix, namespaceUri);
                }
                else
                {
                    throw new ModelException("Unable to define a new namespace without a root document element");
                }
            }
        }

        protected internal virtual string UnusedGenericNsPrefix
        {
            get
            {
                lock (_document)
                {
                    XmlElement documentElement = _document.DocumentElement;
                    if (documentElement == null)
                    {
                        return GenericNsPrefix + "0";
                    }
                    else
                    {
                        for (int i = 0; i < int.MaxValue; i++)
                        {
                            if (!documentElement.HasAttribute(/*XMLNS_ATTRIBUTE_NS_URI,*/ GenericNsPrefix + i))
                            {
                                return GenericNsPrefix + i;
                            }
                        }
                        throw new ModelException("Unable to find an unused namespace prefix");
                    }
                }
            }
        }

        public virtual IDomDocument Clone()
        {
            lock (_document)
            {
                return new DomDocumentImpl((XmlDocument)_document.CloneNode(true));
            }
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            DomDocumentImpl that = (DomDocumentImpl)o;
            return _document.Equals(that._document);
        }

        public override int GetHashCode()
        {
            return _document.GetHashCode();
        }
    }

}