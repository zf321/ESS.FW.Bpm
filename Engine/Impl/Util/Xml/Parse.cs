using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ESS.FW.Bpm.Engine.Impl.Util.IO;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Util.xml
{
    //
    //using DefaultHandler = org.xml.sax.helpers.DefaultHandler;


    /// <summary>
    ///      
    /// </summary>
    public class Parse /*: DefaultHandler*/
    {
        private const string JaxpSchemaSource = "http://java.sun.com/xml/jaxp/properties/schemaSource";
        private const string JaxpSchemaLanguage = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";
        private const string W3CXmlSchema = "http://www.w3.org/2001/XMLSchema";

        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;
        /// <summary>
        /// 解析发生错误的error
        /// </summary>
        protected internal IList<Problem> Errors = new List<Problem>();
        //JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string NameRenamed;

        //private static readonly string NEW_LINE = System.getProperty("line.separator");

        protected internal Parser Parser;
        protected internal Element rootElement = null;
        protected internal string schemaResource;
        protected internal IStreamSource streamSource;
        protected internal IList<Problem> Warnings = new List<Problem>();

        public Parse(Parser parser)
        {
            this.Parser = parser;
        }

        /// <summary>
        ///     xml源写入入口
        /// </summary>
        protected internal virtual IStreamSource StreamSource
        {
            set
            {
                if (streamSource != null)
                    throw Log.MultipleSourcesException(streamSource, value);
                streamSource = value;
            }
        }

        public virtual Element RootElement
        {
            get { return rootElement; }
        }

        public virtual IList<Problem> Problems
        {
            get { return Errors; }
        }

        /// <summary>
        ///     初始化需要!加载配置文件基础协议版本
        /// </summary>
        public virtual string SchemaResource
        {
            set
            {
                //SAXParserFactory saxParserFactory = parser.SaxParserFactory;
                //saxParserFactory.NamespaceAware = true;
                //saxParserFactory.Validating = true;
                //try
                //{
                //  saxParserFactory.setFeature("http://xml.org/sax/features/namespace-prefixes", true);
                //}
                //catch (Exception e)
                //{
                //  LOG.unableToSetSchemaResource(e);
                //}
                schemaResource = value;
            }
        }

        public virtual Parse SetName(string name)
        {
            NameRenamed = name;
            return this;
        }

        public virtual Parse SourceInputStream(Stream inputStream)
        {
            if (NameRenamed == null)
                SetName("inputStream");
            StreamSource = new InputStreamSource(inputStream);
            return this;
        }

        public Parse Deployment(DeploymentEntity deployment)
        {
            //this.del
            return this;

        }

        // public virtual Parse sourceResource(string resource)
        // {
        //return sourceResource(resource, null);
        // }

        public virtual Parse SourceUrl(Uri url)
        {
            if (ReferenceEquals(NameRenamed, null))
                SetName(url.ToString());
            //StreamSource = new UrlStreamSource(url);
            return this;
        }

        public virtual Parse SourceUrl(string url)
        {
            //try
            //{
            return SourceUrl(new Uri(url));
            //}
            //catch (MalformedURLException e)
            //{
            //  throw LOG.malformedUrlException(url, e);
            //}
        }

        public virtual Parse SourceResource(string resource)
        {
            if (ReferenceEquals(NameRenamed, null))
                SetName(resource);
            StreamSource = new ResourceStreamSource(resource);
            return this;
        }

        public virtual Parse SourceString(string @string)
        {
            if (ReferenceEquals(NameRenamed, null))
                SetName("string");
            StreamSource = new StringStreamSource(@string);
            return this;
        }

        public virtual Parse Execute()
        {
            try
            {
                //var inputStream = streamSource.InputStream;

                // if (string.ReferenceEquals(schemaResource, null))
                // { // must be done before parser is created
                //parser.SaxParserFactory.NamespaceAware = false;
                //parser.SaxParserFactory.Validating = false;
                // }
                //parser.XmlParser.SourceInputStream(streamSource.InputStream);
                // SAXParser saxParser = parser.SaxParser;
                // if (!string.ReferenceEquals(schemaResource, null))
                // {
                //saxParser.setProperty(JAXP_SCHEMA_LANGUAGE, W3C_XML_SCHEMA);
                //saxParser.setProperty(JAXP_SCHEMA_SOURCE, schemaResource);
                // }
                //saxParser.parse(inputStream, new ParseHandler(this));
            }
            catch (System.Exception e)
            {
                throw Log.ParsingFailureException(NameRenamed, e);
            }

            return this;
        }

        // public virtual void addError(SAXParseException e)
        // {
        //errors.Add(new Problem(e, name_Renamed));
        // }

        public virtual void AddError(string errorMessage, Element element)
        {
            Errors.Add(new Problem(errorMessage, NameRenamed, element));
        }

        public virtual void AddError(BpmnParseException e)
        {
            Errors.Add(new Problem(e, NameRenamed));
        }

        public virtual bool HasErrors()
        {
            return (Errors != null) && (Errors.Count > 0);
        }

        // public virtual void addWarning(SAXParseException e)
        // {
        //warnings.Add(new Problem(e, name_Renamed));
        // }

        public virtual void AddWarning(string errorMessage, Element element)
        {
            Warnings.Add(new Problem(errorMessage, NameRenamed, element));
        }

        public virtual bool HasWarnings()
        {
            return (Warnings != null) && (Warnings.Count > 0);
        }

        public virtual void LogWarnings()
        {
            var builder = new StringBuilder();
            foreach (var warning in Warnings)
            {
                builder.Append("\n* ");
                builder.Append(warning);
            }
            Log.LogParseWarnings(builder.ToString());
        }

        public virtual void ThrowExceptionForErrors()
        {
            var strb = new StringBuilder();
            foreach (var error in Errors)
            {
                strb.Append("\n* ");
                strb.Append(error);
            }
            throw Log.ExceptionDuringParsing(strb.ToString());
        }
    }
}