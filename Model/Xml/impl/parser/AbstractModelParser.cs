using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;


namespace ESS.FW.Bpm.Model.Xml.impl.parser
{

    //public abstract class AbstractModelParser
    //{

    //    private readonly DocumentBuilderFactory documentBuilderFactory;
    //    protected internal SchemaFactory schemaFactory;
    //    protected internal IDictionary<string, Schema> schemas = new Dictionary<string, Schema>();

    //    protected internal AbstractModelParser()
    //    {
    //        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
    //        configureFactory(dbf);
    //        this.documentBuilderFactory = dbf;
    //    }

    //    /// <summary>
    //    /// allows subclasses to configure the <seealso cref="DocumentBuilderFactory"/>. </summary>
    //    /// <param name="dbf"> the factory to configure </param>
    //    protected internal virtual void configureFactory(DocumentBuilderFactory dbf)
    //    {
    //        dbf.Validating = true;
    //        dbf.IgnoringComments = false;
    //        //dbf.IgnoringElementContentWhitespace = false;
    //        dbf.NamespaceAware = true;
    //        protectAgainstXxeAttacks(dbf);
    //    }

    //    /// <summary>
    //    /// Configures the DocumentBuilderFactory in a way, that it is protected against XML External Entity Attacks.
    //    /// If the implementing parser does not support one or multiple features, the failed feature is ignored.
    //    /// The parser might not protected, if the feature assignment fails.
    //    /// </summary>
    //    /// <seealso cref= <a href="https://www.owasp.org/index.php/XML_External_Entity_(XXE)_Prevention_Cheat_Sheet">OWASP Information of XXE attacks</a>
    //    /// </seealso>
    //    /// <param name="dbf"> The factory to configure. </param>
    //    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //    //ORIGINAL LINE: private void protectAgainstXxeAttacks(final javax.xml.parsers.DocumentBuilderFactory dbf)
    //    private void protectAgainstXxeAttacks(DocumentBuilderFactory dbf)
    //    {
    //        try
    //        {
    //            dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
    //        }
    //        catch (ParserConfigurationException)
    //        {
    //        }

    //        try
    //        {
    //            dbf.setFeature("http://apache.org/xml/features/disallow-doctype-decl", true);
    //        }
    //        catch (ParserConfigurationException)
    //        {
    //        }

    //        try
    //        {
    //            dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
    //        }
    //        catch (ParserConfigurationException)
    //        {
    //        }

    //        dbf.XIncludeAware = false;
    //        dbf.ExpandEntityReferences = false;
    //    }

    //    public virtual ModelInstance parseModelFromStream(System.IO.Stream inputStream)
    //    {
    //        DomDocument document = null;

    //        lock (documentBuilderFactory)
    //        {
    //            document = DomUtil.parseInputStream(documentBuilderFactory, inputStream);
    //        }

    //        validateModel(document);
    //        return createModelInstance(document);

    //    }

    //    public virtual ModelInstance EmptyModel
    //    {
    //        get
    //        {
    //            DomDocument document = null;

    //            lock (documentBuilderFactory)
    //            {
    //                document = DomUtil.GetEmptyDocument(documentBuilderFactory);
    //            }

    //            return createModelInstance(document);
    //        }
    //    }

    //    /// <summary>
    //    /// Validate DOM document
    //    /// </summary>
    //    /// <param name="document"> the DOM document to validate </param>
    //    public virtual void validateModel(DomDocument document)
    //    {

    //        Schema schema = getSchema(document);

    //        if (schema == null)
    //        {
    //            return;
    //        }

    //        Validator validator = schema.newValidator();
    //        try
    //        {
    //            lock (document)
    //            {
    //                validator.validate(document.DomSource);
    //            }
    //        }
    //        catch (IOException e)
    //        {
    //            throw new ModelValidationException("Error during DOM document validation", e);
    //        }
    //        catch (SAXException e)
    //        {
    //            throw new ModelValidationException("DOM document is not valid", e);
    //        }
    //    }

    //    protected internal virtual Schema getSchema(DomDocument document)
    //    {
    //        DomElement rootElement = document.RootElement;
    //        string namespaceURI = rootElement.NamespaceURI;
    //        return schemas[namespaceURI];
    //    }

    //    protected internal virtual void addSchema(string namespaceURI, Schema schema)
    //    {
    //        schemas[namespaceURI] = schema;
    //    }

    //protected internal virtual Schema createSchema(string location, ClassLoader classLoader)
    //{
    //    URL cmmnSchema = ReflectUtil.GetResource(location, classLoader);
    //    try
    //    {
    //        return schemaFactory.newSchema(cmmnSchema);
    //    }
    //    catch (SAXException)
    //    {
    //        throw new ModelValidationException("Unable to parse schema:" + cmmnSchema);
    //    }
    //}

    //    protected internal abstract ModelInstance createModelInstance(DomDocument document);

    //}

    public abstract class AbstractModelParser
    {
        //private readonly DocumentBuilderFactory documentBuilderFactory;
        //protected SchemaFactory schemaFactory;
        protected internal IDictionary<string, XmlSchema> schemas = new Dictionary<string, XmlSchema>();

        private object syncObj=new object();

        protected internal AbstractModelParser()
        {
            //DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
            //configureFactory(dbf);
            //this.documentBuilderFactory = dbf;
            ConfigureFactory();
        }

        protected internal virtual void ConfigureFactory()
        {
            //dbf.setValidating(true);
            //dbf.setIgnoringComments(false);
            //dbf.setIgnoringElementContentWhitespace(false);
            //dbf.setNamespaceAware(true);
            //protectAgainstXxeAttacks(dbf);
        }

        //private void ProtectAgainstXxeAttacks()
        //{
            //try
            //{
            //    dbf.setFeature("http://xml.org/sax/features/external-general-entities", false);
            //}
            //catch (ParserConfigurationException ignored)
            //{
            //}

            //try
            //{
            //    dbf.setFeature("http://apache.org/xml/features/disallow-doctype-decl", true);
            //}
            //catch (ParserConfigurationException ignored)
            //{
            //}

            //try
            //{
            //    dbf.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
            //}
            //catch (ParserConfigurationException ignored)
            //{
            //}

            //dbf.setXIncludeAware(false);
            //dbf.setExpandEntityReferences(false);
        //}

        public virtual IModelInstance ParseModelFromStream(System.IO.Stream inputStream)
        {
            IDomDocument document = null;

            lock(syncObj) {
                document = DomUtil.ParseInputStream(inputStream);
            }

            ValidateModel(document);
            return CreateModelInstance(document);
        }

        public virtual IModelInstance EmptyModel
        {
            get {
                IDomDocument document = null;

                lock(syncObj) {
                    document = DomUtil.GetEmptyDocument();
                }

                return CreateModelInstance(document);
            }
        }

        public virtual void ValidateModel(IDomDocument document)
        {
            XmlSchema schema = GetSchema(document);

            if (schema == null)
            {
                return;
            }

            //Validator validator = schema.newValidator();
            try
            {
                lock(document) {
                    //validator.validate(document.getDomSource());
                }
            }
            catch (IOException e)
            {
                throw new ModelValidationException("Error during DOM document validation", e);
            }
            catch (XmlSchemaException e)
            {
                throw new ModelValidationException("DOM document is not valid", e);
            }
        }


        protected internal virtual XmlSchema GetSchema(IDomDocument document)
        {
            IDomElement rootElement = document.RootElement;
            String namespaceURI = rootElement.NamespaceUri;
            return schemas[namespaceURI];
        }

        protected internal virtual void AddSchema(string namespaceURI, XmlSchema schema)
        {
            schemas.Add(namespaceURI, schema);
        }

        protected internal virtual XmlSchema CreateSchema(string location)
        {
            Uri cmmnSchema = ReflectUtil.GetResource(location);
            try
            {
                return new XmlSchema() {SourceUri = cmmnSchema.AbsolutePath};
                //return schemaFactory.newSchema(cmmnSchema);
            }
            catch (XmlSchemaException)
            {
                throw new ModelValidationException("Unable to parse schema:" + cmmnSchema);
            }
        }

        protected internal abstract IModelInstance CreateModelInstance(IDomDocument document);
    }

}