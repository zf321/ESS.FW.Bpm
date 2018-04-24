using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;
using System.Linq;

namespace ESS.FW.Bpm.Model.Xml.impl.instance
{
    /// <summary>
    /// 
    /// </summary>
    public class DomElementImpl : IDomElement
    {
        public static string XmlnsAttributeNsUri = "http://www.w3.org/2000/xmlns/";
        private const string ModelElementKey = "camunda.modelElementRef";

        private XmlElement _element;
        private XmlDocument _document;

        public DomElementImpl(XmlElement node)
        {
            this._element = node;
            this._document = node.OwnerDocument;
        }
        //public DomElementImpl(XmlNode node) : base(node.Prefix, node.LocalName, node.NamespaceURI, node.OwnerDocument)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.LoadXml(node.OuterXml);
        //    XmlElement element = doc.DocumentElement;
        //    this._element = element;
        //    this._document = node.OwnerDocument;
        //}

        public XmlElement XmlElement
        {
            get
            {
                return _element;
            }
            set
            {
                this._element = value;
                this._document = value.OwnerDocument;
            }
        }

        public virtual string NamespaceUri
        {
            get
            {
                lock (_document)
                {
                    return _element.NamespaceURI;
                }
            }
        }

        public virtual string LocalName
        {
            get
            {
                lock (_document)
                {
                    return _element.LocalName;
                }
            }
        }

        public virtual string Prefix
        {
            get
            {
                lock (_document)
                {
                    return _element.Prefix;
                }
            }
        }

        public virtual IDomDocument Document
        {
            get
            {
                lock (_document)
                {
                    XmlDocument ownerDocument = _element.OwnerDocument;
                    if (ownerDocument != null)
                    {
                        return new DomDocumentImpl(ownerDocument);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public virtual IDomElement RootElement
        {
            get
            {
                lock (_document)
                {
                    IDomDocument document = Document;
                    if (document != null)
                    {
                        return document.RootElement;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public virtual IDomElement ParentElement
        {
            get
            {
                lock (_document)
                {
                    XmlNode parentNode = _element.ParentNode;
                    if (parentNode != null && parentNode is XmlElement)
                    {
                        //return new DomElementImpl((XmlElement)parentNode);
                        return new DomElementImpl((XmlElement)parentNode);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public virtual IList<IDomElement> ChildElements
        {
            get
            {
                lock (_document)
                {
                    XmlNodeList childNodes = _element.ChildNodes;
                    return DomUtil.FilterNodeListForElements(childNodes);
                }
            }
        }
        public virtual IDomElement GetChildElementsById(IDomElement element, string id)
        {
            lock (_document)
            {
                if (id == null) return null;
                IDomElement r = element.ChildElements.FirstOrDefault(m => m.GetAttribute("id") == id);
                if (r != null)
                {
                    return r;
                }
                return null;
            }
        }
        public virtual IList<IDomElement> GetChildElementsByNameNs(string namespaceUri, string elementName)
        {
            lock (_document)
            {
                XmlNodeList childNodes = _element.ChildNodes;
                return DomUtil.FilterNodeListByName(childNodes, namespaceUri, elementName);
            }
        }

        public virtual IList<IDomElement> GetChildElementsByNameNs(ISet<string> namespaceUris, string elementName)
        {
            IList<IDomElement> result = new List<IDomElement>();
            foreach (string @namespace in namespaceUris)
            {
                if (!string.ReferenceEquals(@namespace, null))
                {
                    ((List<IDomElement>)result).AddRange(GetChildElementsByNameNs(@namespace, elementName));
                }
            }
            return result;
        }

        public virtual IList<IDomElement> GetChildElementsByType(IModelInstance modelInstance, Type elementType)
        {
            lock (_document)
            {
                XmlNodeList childNodes = _element.ChildNodes;
                return DomUtil.FilterNodeListByType(childNodes, modelInstance, elementType);
            }
        }

        public virtual void ReplaceChild(IDomElement newChildDomElement, IDomElement existingChildDomElement)
        {
            lock (_document)
            {
                XmlElement newElement = ((DomElementImpl)newChildDomElement).XmlElement;
                XmlElement existingElement = ((DomElementImpl)existingChildDomElement).XmlElement;
                try
                {
                    _element.ReplaceChild(newElement, existingElement);
                }
                catch (XmlException e)
                {
                    throw new ModelException("Unable to replace child <" + existingElement + "> of element <" + _element + "> with element <" + newElement + ">", e);
                }
            }
        }

        public virtual bool RemoveChild(IDomElement childDomElement)
        {
            lock (_document)
            {
                XmlElement childElement = ((DomElementImpl)childDomElement).XmlElement;
                try
                {
                    _element.RemoveChild(childElement);
                    return true;
                }
                catch (XmlException)
                {
                    return false;
                }
            }
        }

        public virtual void AppendChild(IDomElement childDomElement)
        {
            lock (_document)
            {
                XmlElement childElement = ((DomElementImpl)childDomElement).XmlElement;
                _element.AppendChild(childElement);
            }
        }

        public virtual void InsertChildElementAfter(IDomElement elementToInsert, IDomElement insertAfter)
        {
            lock (_document)
            {
                XmlElement xmlElement = ((DomElementImpl)elementToInsert).XmlElement;
                //XmlNode newElement= _document.ImportNode(xmlElement, true);
                // find node to insert before
                XmlNode insertBeforeNode;
                if (insertAfter == null)
                {
                    insertBeforeNode = _element.FirstChild;
                }
                else
                {
                    insertBeforeNode = ((DomElementImpl)insertAfter).XmlElement.NextSibling;
                }

                // insert before node or append if no node was found
                if (insertBeforeNode != null)
                {
                    _element.InsertBefore(xmlElement, insertBeforeNode);
                }
                else
                {
                    _element.AppendChild(xmlElement);
                    //elementToInsert.XmlElement = xml;
                }
            }
        }

        public virtual bool HasAttribute(string localName)
        {
            return HasAttribute(null, localName);
        }

        public virtual bool HasAttribute(string namespaceUri, string localName)
        {
            lock (_document)
            {
                return _element.HasAttribute(namespaceUri, localName);
            }
        }

        public virtual string GetAttribute(string attributeName)
        {
            return GetAttribute(null, attributeName);
        }


        public virtual string GetAttribute(string namespaceUri, string localName)
        {
            lock (_document)
            {
                XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
                string value;
                if (xmlQName.HasLocalNamespace())
                {
                    //value = element.GetAttribute( xmlQName.LocalName, null);
                    value = _element.GetAttribute(xmlQName.LocalName, xmlQName.NamespaceUri);
                }
                else
                {
                    value = _element.GetAttribute(xmlQName.LocalName);
                }
                if (value.Length == 0)
                {
                    return null;
                }
                else
                {
                    return value;
                }
            }
        }

        public virtual void SetAttribute(string localName, string value)
        {
            SetAttribute(null, localName, value);
        }

        public virtual void SetAttribute(string namespaceUri, string localName, string value)
        {
            SetAttribute(namespaceUri, localName, value, false);
        }

        private void SetAttribute(string namespaceUri, string localName, string value, bool isIdAttribute)
        {
            lock (_document)
            {
                XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
                if (xmlQName.HasLocalNamespace())
                {
                    if (_element.HasAttribute(xmlQName.LocalName))
                    {
                        _element.Attributes.RemoveNamedItem(xmlQName.LocalName);
                    }
                    _element.SetAttribute(xmlQName.LocalName, null, value);
                    //if (isIdAttribute)
                    //{
                    //    _element.SetAttribute(xmlQName.LocalName, null);
                    //}
                }
                else
                {
                    if (_element.HasAttribute(xmlQName.PrefixedName))
                    {
                        _element.Attributes.RemoveNamedItem(xmlQName.PrefixedName);
                    }
                    _element.SetAttribute(xmlQName.PrefixedName, xmlQName.NamespaceUri, value);
                    //if (isIdAttribute)
                    //{
                    //    _element.SetAttribute(xmlQName.LocalName, xmlQName.NamespaceUri, null);
                    //}
                }
            }
        }

        public virtual void SetIdAttribute(string localName, string value)
        {
            SetIdAttribute(NamespaceUri, localName, value);
        }

        public virtual void SetIdAttribute(string namespaceUri, string localName, string value)
        {
            SetAttribute(namespaceUri, localName, value, true);
        }

        public virtual void RemoveAttribute(string localName)
        {
            RemoveAttribute(NamespaceUri, localName);
        }

        public virtual void RemoveAttribute(string namespaceUri, string localName)
        {
            lock (_document)
            {
                XmlQName xmlQName = new XmlQName(this, namespaceUri, localName);
                if (xmlQName.HasLocalNamespace())
                {
                    _element.RemoveAttribute(xmlQName.LocalName, null);
                }
                else
                {
                    _element.RemoveAttribute(xmlQName.LocalName, xmlQName.NamespaceUri);
                }
            }
        }

        public virtual string TextContent
        {
            get
            {
                lock (_document)
                {
                    return _element.InnerText;
                }
            }
            set
            {
                lock (_document)
                {
                    _element.InnerText = value;
                }
            }
        }


        public virtual void AddCDataSection(string data)
        {
            lock (_document)
            {
                var cdataSection = _document.CreateCDataSection(data);
                _element.AppendChild(cdataSection);
            }
        }
        //TODO 把ModelElementInstance集成到document里面
        public virtual IModelElementInstance ModelElementInstance { get; set; }
        //{
        //    get
        //    {
        //        lock (document)
        //        {
        //            //TODO 调用了java底层API方法element.getUserData
        //            //document.OwnerDocument.
        //            //return (IModelElementInstance)element.getUserData(MODEL_ELEMENT_KEY);
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        lock (document)
        //        {

        //            //element.setUserData(MODEL_ELEMENT_KEY, value, null);
        //        }
        //    }
        //}

        public virtual string RegisterNamespace(string namespaceUri)
        {
            lock (_document)
            {
                string lookupPrefix = this.LookupPrefix(namespaceUri);
                if (string.ReferenceEquals(lookupPrefix, null))
                {
                    // check if a prefix is known
                    string prefix = XmlQName.KnownPrefixes[namespaceUri];
                    // check if prefix is not already used
                    if (!string.ReferenceEquals(prefix, null) && RootElement != null && RootElement.HasAttribute(XmlnsAttributeNsUri, prefix))
                    {
                        prefix = null;
                    }
                    if (string.ReferenceEquals(prefix, null))
                    {
                        // generate prefix
                        prefix = ((DomDocumentImpl)Document).UnusedGenericNsPrefix;
                    }
                    RegisterNamespace(prefix, namespaceUri);
                    return prefix;
                }
                else
                {
                    return lookupPrefix;
                }
            }
        }

        public virtual void RegisterNamespace(string prefix, string namespaceUri)
        {
            lock (_document)
            {
                //element.setAttributeNS(XMLNS_ATTRIBUTE_NS_URI, XMLNS_ATTRIBUTE + ":" + prefix, namespaceUri);
            }
        }


        public virtual string LookupPrefix(string namespaceUri)
        {
            lock (_document)
            {
                return _element.GetPrefixOfNamespace(namespaceUri);
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

            DomElementImpl that = (DomElementImpl)o;
            return _element.Equals(that._element);
        }

        public override int GetHashCode()
        {
            return _element.GetHashCode();
        }
    }
}