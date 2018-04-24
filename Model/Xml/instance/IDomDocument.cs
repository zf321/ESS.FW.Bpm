using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ESS.FW.Bpm.Model.Xml.instance
{
    public interface IDomDocument
    {

        /// <summary>
        /// Returns the root element of the document.
        /// </summary>
        /// <returns> the root element or null if non exists </returns>
        IDomElement RootElement { get; set; }


        /// <summary>
        /// Creates a new element in the dom document.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the new element </param>
        /// <param name="localName">  the localName of the new element </param>
        /// <returns> the new DOM element </returns>
        IDomElement CreateElement(string namespaceUri, string localName);

        /// <summary>
        /// Gets an element by its id.
        /// </summary>
        /// <param name="id">  the id to search for </param>
        /// <returns> the element or null if no such element exists </returns>
        IDomElement GetElementById(string id);

        /// <summary>
        /// Gets all elements with the namespace and name.
        /// </summary>
        /// <param name="namespaceUri">  the element namespaceURI to search for </param>
        /// <param name="localName">  the element name to search for </param>
        /// <returns> the list of matching elements </returns>
        IList<IDomElement> GetElementsByNameNs(string namespaceUri, string localName);

        /// <summary>
        /// Returns a new <seealso cref="DOMSource"/> of the document.
        /// 
        /// Note that a <seealso cref="DOMSource"/> wraps the underlying <seealso cref="Document"/> which is
        /// not thread-safe. Multiple DOMSources of the same document should be synchronized
        /// by the calling application.
        /// </summary>
        /// <returns> the new <seealso cref="DOMSource"/> </returns>
        //DOMSource DomSource { get; }
        XmlDocument DomSource { get; }
        

        /// <summary>
        /// Registers a new namespace with a generic prefix.
        /// </summary>
        /// <param name="namespaceUri">  the namespaceUri of the new namespace </param>
        /// <returns> the used prefix </returns>
        string RegisterNamespace(string namespaceUri);

        /// <summary>
        /// Registers a new namespace for the prefix.
        /// </summary>
        /// <param name="prefix">  the prefix of the new namespace </param>
        /// <param name="namespaceUri">  the namespaceUri of the new namespace </param>
        void RegisterNamespace(string prefix, string namespaceUri);

        /// <summary>
        /// Clones the DOM document.
        /// </summary>
        /// <returns> the cloned DOM document </returns>
        IDomDocument Clone();

    }

}