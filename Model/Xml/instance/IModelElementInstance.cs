using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Xml.type;


namespace ESS.FW.Bpm.Model.Xml.instance
{
    /// <summary>
    /// An instance of a <seealso cref="IModelElementType"/>
    /// 
    /// 
    /// 
    /// </summary>
    public interface IModelElementInstance
    {
        string Id { get; set; }
        /// <summary>
        /// Returns the represented DOM <seealso cref="DomElement"/>.
        /// </summary>
        /// <returns> the DOM element </returns>
        IDomElement DomElement { get; /*set;*/ }

        /// <summary>
        /// Returns the model instance which contains this type instance.
        /// </summary>
        /// <returns> the model instance </returns>

        IModelInstance ModelInstance { get; }
        /// <summary>
        /// Returns the parent element of this.
        /// </summary>
        /// <returns> the parent element </returns>
        IModelElementInstance ParentElement { get; }

        /// <summary>
        /// Returns the element type of this.
        /// </summary>
        /// <returns> the element type </returns>
        IModelElementType ElementType { get; }

        /// <summary>
        /// Returns the attribute value for the attribute name.
        /// </summary>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <returns> the value of the attribute </returns>
        string GetAttributeValue(string attributeName);

        /// <summary>
        /// Sets the value by name of a non-ID attribute.
        /// </summary>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <param name="xmlValue">  the value to set </param>
        void SetAttributeValue(string attributeName, string xmlValue);

        /// <summary>
        /// Sets attribute value by name.
        /// </summary>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <param name="xmlValue">  the value to set </param>
        /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
        void SetAttributeValue(string attributeName, string xmlValue, bool isIdAttribute);

        /// <summary>
        /// Removes attribute by name.
        /// </summary>
        /// <param name="attributeName">  the name of the attribute </param>
        void RemoveAttribute(string attributeName);

        /// <summary>
        /// Returns the attribute value for the given attribute name and namespace URI.
        /// </summary>
        /// <param name="namespaceUri">  the namespace URI of the attribute </param>
        /// <param name="attributeName">  the attribute name of the attribute </param>
        /// <returns> the value of the attribute </returns>
        string GetAttributeValueNs(string namespaceUri, string attributeName);

        /// <summary>
        /// Sets the value by name and namespace of a non-ID attribute.
        /// </summary>
        /// <param name="namespaceUri">  the namespace URI of the attribute </param>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <param name="xmlValue">  the XML value to set </param>
        void SetAttributeValueNs(string namespaceUri, string attributeName, string xmlValue);

        /// <summary>
        /// Sets the attribute value by name and namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespace URI of the attribute </param>
        /// <param name="attributeName">  the name of the attribute </param>
        /// <param name="xmlValue">  the XML value to set </param>
        /// <param name="isIdAttribute">  true if the attribute is an ID attribute, false otherwise </param>
        void SetAttributeValueNs(string namespaceUri, string attributeName, string xmlValue, bool isIdAttribute);

        /// <summary>
        /// Removes the attribute by name and namespace.
        /// </summary>
        /// <param name="namespaceUri">  the namespace URI of the attribute </param>
        /// <param name="attributeName">  the name of the attribute </param>
        void RemoveAttributeNs(string namespaceUri, string attributeName);

        /// <summary>
        /// Returns the text content of the DOM element without leading and trailing spaces. For
        /// raw text content see <seealso cref="ModelElementInstanceImpl#getRawTextContent()"/>.
        /// </summary>
        /// <returns> text content of underlying DOM element with leading and trailing whitespace trimmed </returns>
        string TextContent { get; set; }

        /// <summary>
        /// Returns the raw text content of the DOM element including all whitespaces.
        /// </summary>
        /// <returns> raw text content of underlying DOM element </returns>
        string RawTextContent { get; }


        /// <summary>
        /// Replaces this element with a new element and updates references.
        /// </summary>
        /// <param name="newElement">  the new element to replace with </param>
        void ReplaceWithElement(IModelElementInstance newElement);

        /// <summary>
        /// Returns a child element with the given name or 'null' if no such element exists
        /// </summary>
        /// <param name="namespaceUri"> the local name of the element </param>
        /// <param name="elementName"> the namespace of the element </param>
        /// <returns> the child element or null. </returns>
        IModelElementInstance GetUniqueChildElementByNameNs(string namespaceUri, string elementName);

        /// <summary>
        /// Returns a child element with the given type
        /// </summary>
        /// <param name="elementType">  the type of the element </param>
        /// <returns> the child element or null </returns>
        IModelElementInstance GetUniqueChildElementByType(Type elementType);

        /// <summary>
        /// Adds or replaces a child element by name. Replaces an existing Child XmlElement with the same name
        /// or adds a new child if no such element exists.
        /// </summary>
        /// <param name="newChild"> the child to add </param>
        IModelElementInstance UniqueChildElementByNameNs { set; }

        /// <summary>
        /// Replace an existing child element with a new child element. Changes the underlying DOM element tree.
        /// </summary>
        /// <param name="existingChild"> the child element to replace </param>
        /// <param name="newChild"> the new child element </param>
        void ReplaceChildElement(IModelElementInstance existingChild, IModelElementInstance newChild);

        /// <summary>
        /// Adds a new child element to the children of this element. The child
        /// is inserted at the correct position of the allowed child types.
        /// Updates the underlying DOM element tree.
        /// </summary>
        /// <param name="newChild"> the new child element </param>
        /// <exception cref="ModelException"> if the new child type is not an allowed child type </exception>
        void AddChildElement(IModelElementInstance newChild);

        /// <summary>
        /// Removes the child element from this.
        /// </summary>
        /// <param name="child">  the child element to remove </param>
        /// <returns> true if the child element could be removed </returns>
        bool RemoveChildElement(IModelElementInstance child);

        /// <summary>
        /// Return all child elements of a given type
        /// </summary>
        /// <param name="childElementType"> the child element type to search for </param>
        /// <returns> a collection of elements of the given type </returns>
        IList<IModelElementInstance> GetChildElementsByType(Type childElementType);

        /// <summary>
        /// Return all child elements of a given type
        /// </summary>
        /// <param name="childElementClass">  the class of the child element type to search for </param>
        /// <returns> a collection of elements to the given type </returns>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        IList<T> GetChildElementsByType<T>(System.Type childElementClass) where T : IModelElementInstance;

        /// <summary>
        /// Inserts the new element after the given element or at the beginning if the given element is null.
        /// </summary>
        /// <param name="elementToInsert">  the new element to insert </param>
        /// <param name="insertAfterElement">  the element to insert after or null to insert at first position </param>
        void InsertElementAfter(IModelElementInstance elementToInsert, IModelElementInstance insertAfterElement);

        /// <summary>
        /// Execute updates after the element was inserted as a replacement of another element.
        /// </summary>
        void UpdateAfterReplacement();
    }
}