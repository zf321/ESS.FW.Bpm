using System;
using System.Collections.Generic;
using System.Xml;



namespace ESS.FW.Bpm.Model.Xml.instance
{


    /// <summary>
    /// Encapsulates <seealso cref="XmlElement"/>. Implementations of this interface must be thread-safe.
    /// 
    /// 
    /// </summary>
    public interface IDomElement
    {

        /// <summary>
        /// Returns the namespace URI for this element.
        /// </summary>
        /// <returns> the namespace URI </returns>
        string NamespaceUri { get; }

        XmlElement XmlElement { get; set; }

        /// <summary>
        /// Returns the local name of this element.
        /// </summary>
        /// <returns> the local name </returns>
        string LocalName { get; }

        /// <summary>
        /// Returns the prefix of this element.
        /// </summary>
        /// <returns> the prefix </returns>
        string Prefix { get; }

        /// <summary>
        /// Returns the DOM document which contains this element.
        /// </summary>
        /// <returns> the DOM document or null if the element itself is a document </returns>
        IDomDocument Document { get; }

        /// <summary>
        /// Returns the root element of the document which contains this element.
        /// </summary>
        /// <returns> the root element of the document or null if non exists </returns>
        IDomElement RootElement { get; }

        /// <summary>
        /// Returns the parent element of this element.
        /// </summary>
        /// <returns> the parent element or null if not part of a tree </returns>
        IDomElement ParentElement { get; }

        /// <summary>
        /// Returns all child elements of this element.
        /// </summary>
        /// <returns> the list of child elements </returns>
        IList<IDomElement> ChildElements { get; }

        /// <summary>
        /// Returns all child elements of this element with a specific namespace + name
        /// </summary>
        /// <returns> the list of child elements </returns>
        IList<IDomElement> GetChildElementsByNameNs(string namespaceUris, string elementName);
        IDomElement GetChildElementsById(IDomElement element, string id);
        /// <summary>
        /// Returns all child elements of this element with specific namespaces + name.
        /// </summary>
        /// <returns> the list of child elements </returns>
        IList<IDomElement> GetChildElementsByNameNs(ISet<string> namespaceUris, string elementName);

        /// <summary>
        /// Returns all child elements of this element with a specific type.
        /// </summary>
        /// <returns> the list of child elements matching the type </returns>
        IList<IDomElement> GetChildElementsByType(IModelInstance modelInstance, Type elementType);

        /// <summary>
        /// Replaces a child element with a new element.
        /// </summary>
        /// <param name="newChildDomElement">  the new child element </param>
        /// <param name="existingChildDomElement">  the existing child element </param>
        /// <exception cref="ModelException"> if the child cannot be replaced </exception>
        void ReplaceChild(IDomElement newChildDomElement, IDomElement existingChildDomElement);

        /// <summary>
        /// Removes a child element of this element.
        /// </summary>
        /// <param name="domElement">  the child element to remove </param>
        /// <returns> true if the child element was removed otherwise false </returns>
        bool RemoveChild(IDomElement domElement);

        /// <summary>
        /// Appends the element to the child elements of this element.
        /// </summary>
        /// <param name="childElement">  the element to append </param>
        void AppendChild(IDomElement childElement);

        /// <summary>
        /// Inserts the new child element after another child element. If the child element to
        /// insert after is null the new child element will be inserted at the beginning.
        /// </summary>
        /// <param name="elementToInsert">  the new element to insert </param>
        /// <param name="insertAfter">  the existing child element to insert after or null </param>
        void InsertChildElementAfter(IDomElement elementToInsert, IDomElement insertAfter);

        /// <summary>
        /// Checks if this element has a attribute under the namespace of this element.
        /// </summary>
        /// <param name="localName">  the name of the attribute </param>
        /// <returns> true if the attribute exists otherwise false </returns>
        bool HasAttribute(string localName);

        /// <summary>
        /// Checks if this element has a attribute with the given namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <param name="localName">  the name of the attribute </param>
        /// <returns> true if the attribute exists otherwise false </returns>
        bool HasAttribute(string namespaceUri, string localName);

        /// <summary>
        /// Returns the attribute value for the namespace of this element.
        /// </summary>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <returns> the value of the attribute or the empty string </returns>
        string GetAttribute(string attributeName);

        /// <summary>
        /// Returns the attribute value for the given namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <param name="localName">  the name of the attribute </param>
        /// <returns> the value of the attribute or the empty string </returns>
        string GetAttribute(string namespaceUri, string localName);

        /// <summary>
        /// Sets the attribute value for the namespace of this element.
        /// </summary>
        /// <param name="localName">  the name of the attribute </param>
        /// <param name="value">  the value to set </param>
        void SetAttribute(string localName, string value);

        /// <summary>
        /// Sets the attribute value for the given namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <param name="localName">  the name of the attribute </param>
        /// <param name="value">  the value to set </param>
        void SetAttribute(string namespaceUri, string localName, string value);

        /// <summary>
        /// Sets the value of a id attribute for the namespace of this element.
        /// </summary>
        /// <param name="localName">  the name of the attribute </param>
        /// <param name="value">  the value to set </param>
        void SetIdAttribute(string localName, string value);

        /// <summary>
        /// Sets the value of a id attribute for the given namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <param name="localName">  the name of the attribute </param>
        /// <param name="value">  the value to set </param>
        void SetIdAttribute(string namespaceUri, string localName, string value);

        /// <summary>
        /// Removes the attribute for the namespace of this element.
        /// </summary>
        /// <param name="localName">  the name of the attribute </param>
        void RemoveAttribute(string localName);

        /// <summary>
        /// Removes the attribute for the given namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <param name="localName">  the name of the attribute </param>
        void RemoveAttribute(string namespaceUri, string localName);

        /// <summary>
        /// Gets the text content of this element all its descendants.
        /// </summary>
        /// <returns> the text content </returns>
        string TextContent { get; set; }


        /// <summary>
        /// Adds a CDATA section to this element.
        /// </summary>
        /// <param name="textContent">  the CDATA content to set </param>
        void AddCDataSection(string data);

        /// <summary>
        /// Returns the <seealso cref="ModelElementInstance"/> which is associated with this element.
        /// </summary>
        /// <returns> the <seealso cref="ModelElementInstance"/> or null if non is associated </returns>
        IModelElementInstance ModelElementInstance { get; set; }


        /// <summary>
        /// Adds a new namespace with a generated prefix to this element.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <returns> the generated prefix for the new namespace </returns>
        string RegisterNamespace(string namespaceUri);

        /// <summary>
        /// Adds a new namespace with prefix to this element.
        /// </summary>
        /// <param name="prefix">  the prefix of the namespace </param>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        void RegisterNamespace(string prefix, string namespaceUri);

        /// <summary>
        /// Returns the prefix of the namespace starting from this node upwards.
        /// The default namespace has the prefix {@code null}.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the namespace </param>
        /// <returns> the prefix or null if non is defined </returns>
        string LookupPrefix(string namespaceUri);
    }

}