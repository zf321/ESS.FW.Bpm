using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    /// <summary>
    ///     封装特性集合的数据模型(树状结构)
    ///     Represents one XML element.
    ///     
    /// </summary>
    public class Element
    {

        /// <summary>
        ///     特性集合 camunda:asyncAfter="true" 其中camunda在 xmlns:camunda="http://camunda.org/schema/1.0/bpmn"定义
        ///     实际值：http://camunda.org/schema/1.0/bpmn:asyncAfter
        ///     key格式:uri:attributName
        ///     如果namespace为空,key为attributeName
        /// </summary>
        protected internal IDictionary<string, Attribute> AttributeMap = new Dictionary<string, Attribute>();

        /// <summary>
        ///     列号
        /// </summary>
        protected internal int column;
        
        /// <summary>
        ///     包含的element集合
        /// </summary>
        protected internal IList<Element> ElementsRenamed = new List<Element>();

        /// <summary>
        ///     行号
        /// </summary>
        protected internal int line;

        public XmlNode RootXmlNode;

        /// <summary>
        ///     名称
        /// </summary>
        protected internal string tagName;

        protected internal StringBuilder text = new StringBuilder();

        /// <summary>
        ///     资源地址
        /// </summary>
        protected internal string uri;

        /// <summary>
        ///     Element构造函数
        /// </summary>
        /// <param name="uri">资源地址</param>
        /// <param name="localName">本地名称(uri有值时tagname为此值)</param>
        /// <param name="qName">q名称(uri为空时tagname为此值)</param>
        /// <param name="attributes">特性集合,可以为null</param>
        /// <param name="locator">定位可以为null</param>
        public Element(string uri, string localName, string qName, IAttributes attributes, ILocator locator)
        {
            this.uri = uri;
            tagName = string.IsNullOrEmpty(uri) ? qName : localName;

            if (attributes != null)
                for (var i = 0; i < attributes.GetLength(); i++)
                {

                    var attributeUri = attributes.GetUri(i);
                    var name = string.IsNullOrEmpty(attributeUri) ? attributes.GetQName(i) : attributes.GetLocalName(i);
                    var value = attributes.GetValue(i);
                    AttributeMap[ComposeMapKey(attributeUri, name)] = new Attribute(name, value, attributeUri);
                }

            if (locator != null)
            {
                line = locator.GetLineNumber(); //.LineNumber;
                column = locator.GetColumnNumber(); //.ColumnNumber;
            }
        }

        /// <summary>
        ///     重写构造函数(如果url为空，过滤掉prefix)
        /// </summary>
        /// <param name="xmlNode"></param>
        public Element(XmlNode xmlNode, ILocator locator = null)
        {
            uri = xmlNode.NamespaceURI; //.BaseURI;
            tagName = string.IsNullOrEmpty(uri) ? xmlNode.Name/*.Replace(xmlNode.Prefix + ":", "")*/ : xmlNode.LocalName;
            RootXmlNode = xmlNode;
            if (xmlNode.Attributes != null)
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    var attributeUri = attribute.NamespaceURI;//.BaseURI;
                    var name = string.IsNullOrEmpty(attributeUri)
                        ? attribute.Name//.Replace(attribute.Prefix + ":", "")
                        : attribute.LocalName;
                    var value = attribute.Value;
                    AttributeMap[ComposeMapKey(attributeUri, name)] = new Attribute(name, value, attributeUri);
                }
            if (locator != null)
            {
                line = locator.GetLineNumber();
                column = locator.GetColumnNumber();
            }
            if (xmlNode.ChildNodes.Count > 0)
                foreach (XmlNode item in xmlNode.ChildNodes)
                    ElementsRenamed.Add(new Element(item, null));
        }

        /// <summary>
        ///     自定义构造函数
        /// </summary>
        /// <param name="xmlSource"></param>
        /// <param name="locator"></param>
        public Element(string xmlSource, ILocator locator = null) : this(GetXmlFromString(xmlSource), locator)
        {
        }

        public Element(Stream inputStream, ILocator locator = null) : this(GetXmlFromStream(inputStream), locator)
        {
        }

        /// <summary>
        ///     资源地址
        /// </summary>
        public virtual string Uri
        {
            get { return uri; }
        }

        /// <summary>
        ///     名称
        /// </summary>
        public virtual string TagName
        {
            get { return tagName; }
        }

        /// <summary>
        ///     行号
        /// </summary>
        public virtual int Line
        {
            get { return line; }
        }

        /// <summary>
        ///     列号
        /// </summary>
        public virtual int Column
        {
            get { return column; }
        }

        public virtual string Text
        {
            get { return RootXmlNode.InnerText.ToString(); }
        }

        private static XmlNode GetXmlFromString(string xmlSource)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlSource);
            return xmlDoc.DocumentElement;
        }

        private static XmlNode GetXmlFromStream(Stream stream)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(stream);
            return xmldoc.DocumentElement;
        }

        /// <summary>
        ///     根据tagName搜索elements_Renamed集合内的uri为空的tagName集合
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public virtual IList<Element> Elements(string tagName)
        {
            return ElementsNS((string)null, tagName);
        }

        public virtual IList<Element> ElementsNS(Namespace nameSpace, string tagName)
        {
            IList<Element> elementsNS = ElementsNS(nameSpace.NamespaceUri, tagName);
            if (elementsNS.Count == 0 && nameSpace.HasAlternativeUri())
            {
                elementsNS = ElementsNS(nameSpace.AlternativeUri, tagName);
            }
            return elementsNS;
        }
        /// <summary>
        ///     查找元素
        /// </summary>
        /// <param name="nameSpaceUri"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        protected internal virtual IList<Element> ElementsNS(string nameSpaceUri, string tagName)
        {
            IList<Element> selectedElements = new List<Element>();
            foreach (var element in ElementsRenamed)
                if (tagName.Equals(element.TagName))
                    if (ReferenceEquals(nameSpaceUri, null) ||
                        (!ReferenceEquals(nameSpaceUri, null) && nameSpaceUri.Equals(element.Uri)))
                        selectedElements.Add(element);
            return selectedElements;
        }

        /// <summary>
        ///     查找单个,有多个会异常
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public virtual Element element(string tagName)
        {
            return ElementNs(new Namespace(null), tagName);
        }

        /// <summary>
        ///     查找单个,有多个会异常
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public virtual Element ElementNs(Namespace nameSpace, string tagName)
        {
            var elements = ElementsNS(nameSpace.NamespaceUri, tagName);
            if ((elements.Count == 0) && nameSpace.HasAlternativeUri())
                elements = ElementsNS(nameSpace.AlternativeUri, tagName);
            if (elements.Count == 0)
                return null;
            if (elements.Count > 1)
                throw new ProcessEngineException("Parsing exception: multiple elements with tag name " + tagName +
                                                 " found");
            return elements[0];
        }

        /// <summary>
        ///     添加到elements_Renamed子集中
        /// </summary>
        /// <param name="element"></param>
        public virtual void Add(Element element)
        {
            ElementsRenamed.Add(element);
        }

        /// <summary>
        ///     attributeMap中根据key获取value
        /// </summary>
        /// <param name="name"></param>
        /// <returns>没有为null</returns>
        public virtual string GetAttributeValue(string name)
        {
            if (AttributeMap.ContainsKey(name))
                return AttributeMap[name].Value;
            return null;
        }

        /// <summary>
        ///     获取attribute所有key
        /// </summary>
        /// <returns></returns>
        public virtual ICollection<string> GetAllAttributeKeys()
        {
            return AttributeMap.Keys;
        }
        [System.Obsolete]
        public virtual string AttributeNs(Namespace @namespace, string name)
        {
            var attribute = GetAttributeValue(ComposeMapKey(@namespace.NamespaceUri, name));
            if (ReferenceEquals(attribute, null) && @namespace.HasAlternativeUri())
                attribute = GetAttributeValue(ComposeMapKey(@namespace.AlternativeUri, name));
            return attribute;
        }

        /// <summary>
        ///     根据key获取value
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="defaultValue">手动指定的默认值</param>
        /// <returns>如果找不到对应的key返回指定的默认指</returns>
        public virtual string GetAttributeValue(string name, string defaultValue)
        {
            if (AttributeMap.ContainsKey(name))
                return AttributeMap[name].Value;
            return defaultValue;
        }

        public virtual string GetAttributeNS(Namespace @namespace, string name)
        {
            string attribute = GetAttributeValue(ComposeMapKey(@namespace.NamespaceUri, name));
            if ((attribute== null) && @namespace.HasAlternativeUri())
            {
                attribute = GetAttributeValue(ComposeMapKey(@namespace.AlternativeUri, name));
            }
            return attribute;
        }

        public virtual string GetAttributeNS(Namespace @namespace, string name, string defaultValue)
        {
            string attribute = GetAttributeValue(ComposeMapKey(@namespace.NamespaceUri, name));
            if ((attribute == null) && @namespace.HasAlternativeUri())
            {
                attribute = GetAttributeValue(ComposeMapKey(@namespace.AlternativeUri, name));
            }
            if (attribute == null)
            {
                return defaultValue;
            }
            return attribute;
        }

        /// <summary>
        ///     生成mapKey
        /// </summary>
        /// <param name="attributeUri">资源地址</param>
        /// <param name="attributeName">名称</param>
        /// <returns>格式:资源地址:名称</returns>
        protected internal virtual string ComposeMapKey(string attributeUri, string attributeName)
        {
            var strb = new StringBuilder();
            if (!string.IsNullOrEmpty(attributeUri))
            {
                strb.Append(attributeUri);
                strb.Append(":");
            }
            strb.Append(attributeName);
            return strb.ToString();
        }

        /// <summary>
        ///     获取所有Element
        /// </summary>
        /// <returns></returns>
        public virtual IList<Element> GetAllElement()
        {
            return ElementsRenamed;
        }

        /// <summary>
        ///     重写Tostring
        /// </summary>
        /// <returns>格式:<tagName...</returns>
        public override string ToString()
        {
            return "<" + tagName + "...";
        }

        /// <summary>
        ///     插入文本
        ///     Due to the nature of SAX parsing, sometimes the characters of an element
        ///     are not processed at once. So instead of a setText operation, we need
        ///     to have an appendText operation.
        /// </summary>
        public virtual void AppendText(string text)
        {
            this.text.Append(text);
        }

        /// <summary>
        ///     递归获取所有key为id的Attribute值
        ///     allows to recursively collect the ids of all elements in the tree.
        /// </summary>
        public virtual void CollectIds(IList<string> ids)
        {
            if (AttributeMap.ContainsKey("id"))
            {
                ids.Add(GetAttributeValue("id"));
                foreach (var child in ElementsRenamed)
                    child.CollectIds(ids);
            }
            else
            {
                foreach (var child in ElementsRenamed)
                    child.CollectIds(ids);
            }
        }
    }
}