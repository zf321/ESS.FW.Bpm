using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Xml.impl.util
{

    using DomDocumentImpl = DomDocumentImpl;
    using DomElementImpl = DomElementImpl;

    /// <summary>
    /// Helper methods which abstract some gruesome DOM specifics.
    /// It does not provide synchronization when invoked in parallel with
    /// the same objects.
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public sealed class DomUtil
    {

        /// <summary>
        /// A <seealso cref="INodeListFilter"/> allows to filter a <seealso cref="NodeList"/>,
        /// retaining only elements in the list which match the filter.
        /// </summary>
        /// <seealso cref= DomUtil#filterNodeList(NodeList, NodeListFilter) </seealso>
        public interface INodeListFilter
        {

            /// <summary>
            /// Test if node matches the filter
            /// </summary>
            /// <param name="node"> the node to match </param>
            /// <returns> true if the filter does match the node, false otherwise </returns>
            bool Matches(XmlNode node);

        }

        /// <summary>
        /// Filter retaining only Nodes of type <seealso cref="INode#ELEMENT_NODE"/>
        /// 
        /// </summary>
        public class ElementNodeListFilter : INodeListFilter
        {

            public virtual bool Matches(XmlNode node)
            {
                //TODO bool matches(XmlNode node) 源码为1
                return node.NodeType == XmlNodeType.Element;// == XmlNode.ELEMENT_NODE;
                //return false;
            }

        }

        /// <summary>
        /// Filters <seealso cref="XmlElement"/> by their nodeName + namespaceUri
        /// 
        /// </summary>
        public class ElementByNameListFilter : ElementNodeListFilter
        {

            internal readonly string LocalName;
            internal readonly string NamespaceUri;

            /// <param name="localName"> the local name to filter for </param>
            /// <param name="namespaceUri"> the namespaceUri to filter for </param>
            public ElementByNameListFilter(string localName, string namespaceUri)
            {
                this.LocalName = localName;
                this.NamespaceUri = namespaceUri;
            }

            public override bool Matches(XmlNode node)
            {
                return base.Matches(node) && LocalName.Equals(node.LocalName) && NamespaceUri.Equals(node.NamespaceURI);
            }

        }

        public class ElementByTypeListFilter : ElementNodeListFilter
        {

            internal readonly Type Type;
            internal readonly IModelInstance Model;

            public ElementByTypeListFilter(Type type, IModelInstance modelInstance)
            {
                this.Type = type;
                this.Model = modelInstance;
            }
            //TODO Input底层可能又问题
            public override bool Matches(XmlNode node)
            {
                if (!base.Matches(node))
                {
                    return false;
                }
                IModelElementInstance modelElement = ModelUtil.GetModelElement(new DomElementImpl((XmlElement)node), Model);
                return Type.IsAssignableFrom(modelElement.GetType());
            }
        }

        /// <summary>
        /// Allows to apply a <seealso cref="INodeListFilter"/> to a <seealso cref="NodeList"/>. This allows to remove all elements from a node list which do not match the Filter.
        /// </summary>
        /// <param name="nodeList"> the <seealso cref="XmlNodeList"/> to filter </param>
        /// <param name="filter"> the <seealso cref="INodeListFilter"/> to apply to the <seealso cref="NodeList"/> </param>
        /// <returns> the List of all Nodes which match the filter </returns>
        public static IList<IDomElement> FilterNodeList(XmlNodeList nodeList, INodeListFilter filter)
        {

            IList<IDomElement> filteredList = new List<IDomElement>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlNode node = nodeList.Item(i);
                if (filter.Matches(node))
                {
                    filteredList.Add(new DomElementImpl((XmlElement)node));
                }
            }

            return filteredList;

        }

        /// <summary>
        /// Filters a <seealso cref="NodeList"/> retaining all elements
        /// </summary>
        /// <param name="nodeList">  the the <seealso cref="NodeList"/> to filter </param>
        /// <returns> the list of all elements </returns>
        public static IList<IDomElement> FilterNodeListForElements(XmlNodeList nodeList)
        {
            return FilterNodeList(nodeList, new ElementNodeListFilter());
        }

        /// <summary>
        /// Filter a <seealso cref="NodeList"/> retaining all elements with a specific name
        /// 
        /// </summary>
        /// <param name="nodeList"> the <seealso cref="NodeList"/> to filter </param>
        /// <param name="namespaceUri"> the namespace for the elements </param>
        /// <param name="localName"> the local element name to filter for </param>
        /// <returns> the List of all Elements which match the filter </returns>
        public static IList<IDomElement> FilterNodeListByName(XmlNodeList nodeList, string namespaceUri, string localName)
        {
            return FilterNodeList(nodeList, new ElementByNameListFilter(localName, namespaceUri));
        }

        /// <summary>
        /// Filter a <seealso cref="NodeList"/> retaining all elements with a specific type
        /// 
        /// </summary>
        /// <param name="nodeList">  the <seealso cref="NodeList"/> to filter </param>
        /// <param name="modelInstance">  the model instance </param>
        /// <param name="type">  the type class to filter for </param>
        /// <returns> the list of all Elements which match the filter </returns>
        public static IList<IDomElement> FilterNodeListByType(XmlNodeList nodeList, IModelInstance modelInstance, Type type)
        {
            return FilterNodeList(nodeList, new ElementByTypeListFilter(type, modelInstance));
        }

        //	  public class DomErrorHandler : ErrorHandler
        //	  {

        ////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
        //		//internal static readonly Logger LOGGER = Logger.getLogger(typeof(DomErrorHandler).FullName);

        //		internal virtual string getParseExceptionInfo(SAXParseException spe)
        //		{
        //		  return "URI=" + spe.SystemId + " Line=" + spe.LineNumber + ": " + spe.Message;
        //		}

        //		public virtual void warning(XmlException spe)
        //		{
        //		  //LOGGER.warning(getParseExceptionInfo(spe));
        //		}

        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        ////ORIGINAL LINE: public void error(org.xml.sax.SAXParseException spe) throws org.xml.sax.SAXException
        //		public virtual void error(SAXParseException spe)
        //		{
        //		  string message = "Error: " + getParseExceptionInfo(spe);
        //		  throw new SAXException(message);
        //		}

        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        ////ORIGINAL LINE: public void fatalError(org.xml.sax.SAXParseException spe) throws org.xml.sax.SAXException
        //		public virtual void fatalError(SAXParseException spe)
        //		{
        //		  string message = "Fatal Error: " + getParseExceptionInfo(spe);
        //		  throw new SAXException(message);
        //		}
        //	  }

        /// <summary>
        /// Get an empty DOM document
        /// </summary>
        /// <param name="documentBuilderFactory"> the factory to build to DOM document </param>
        /// <returns> the new empty document </returns>
        /// <exception cref="ModelParseException"> if unable to create a new document </exception>
        public static IDomDocument GetEmptyDocument()
        {
            try
            {
                //DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
                return new DomDocumentImpl(new XmlDocument());
            }
            catch (XmlException e)
            {
                throw new ModelParseException("Unable to create a new document", e);
            }
        }

        /// <summary>
        /// Create a new DOM document from the input stream
        /// </summary>
        /// <param name="documentBuilderFactory"> the factory to build to DOM document </param>
        /// <param name="inputStream"> the input stream to parse </param>
        /// <returns> the new DOM document </returns>
        /// <exception cref="ModelParseException"> if a parsing or IO error is triggered </exception>
        public static IDomDocument ParseInputStream(System.IO.Stream inputStream)
        {

            try
            {
                //DocumentBuilder documentBuilder = documentBuilderFactory.newDocumentBuilder();
                //documentBuilder.ErrorHandler = new DomErrorHandler();
                XmlDocument doc = new XmlDocument();
                doc.Load(inputStream);
                return new DomDocumentImpl(doc);
            }
            //catch (XmlParserConfigurationException e)
            //{
            //  throw new ModelParseException("ParserConfigurationException while parsing input stream", e);

            //}
            catch (XmlException e)
            {
                throw new ModelParseException("SAXException while parsing input stream", e);

            }
            catch (IOException e)
            {
                throw new ModelParseException("IOException while parsing input stream", e);

            }
        }

    }

}