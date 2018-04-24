using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.impl.parser;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Dmn.impl
{
    public class DmnParser : AbstractModelParser
    {

        private const string JaxpSchemaSource = "http://java.sun.com/xml/jaxp/properties/schemaSource";
        private const string JaxpSchemaLanguage = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";

        private const string W3CXmlSchema = "http://www.w3.org/2001/XMLSchema";

        public DmnParser()
        {
            //this.schemaFactory = SchemaFactory.newInstance(W3C_XML_SCHEMA);
            AddSchema(DmnModelConstants.Dmn11Ns, CreateSchema(DmnModelConstants.Dmn11SchemaLocation));
            AddSchema(DmnModelConstants.Dmn11AlternativeNs, CreateSchema(DmnModelConstants.Dmn11AlternativeSchemaLocation));
        }

        //protected internal override void configureFactory(DocumentBuilderFactory dbf)
        //{
        //    dbf.setAttribute(JAXP_SCHEMA_LANGUAGE, W3C_XML_SCHEMA);
        //    dbf.setAttribute(JAXP_SCHEMA_SOURCE, new string[] { ReflectUtil.getResource(DmnModelConstants.DMN_11_SCHEMA_LOCATION, typeof(DmnParser)).ToString(), ReflectUtil.getResource(DmnModelConstants.DMN_11_ALTERNATIVE_SCHEMA_LOCATION, typeof(DmnParser)).ToString() });
        //    base.configureFactory(dbf);
        //}
        protected internal override IModelInstance CreateModelInstance(IDomDocument document)
        {
            //var dmnMode = Dmn.INSTANCE.DmnModel as ModelImpl;
            //var dmnBuilder = Dmn.INSTANCE.DmnModelBuilder;
            //var t = new DmnModelInstanceImpl(dmnMode, dmnBuilder, document);
            return new DmnModelInstanceImpl((ModelImpl)Dmn.Instance.DmnModel, Dmn.Instance.DmnModelBuilder, document);
        }
        public override /*DmnModelInstanceImpl*/IModelInstance ParseModelFromStream(System.IO.Stream inputStream)
        {
            try
            {
                //var IModel = base.parseModelFromStream(inputStream);
                //var model = IModel as DmnModelInstanceImpl;
                return /*(DmnModelInstanceImpl)*/base.ParseModelFromStream(inputStream);
            }
            catch (ModelParseException e)
            {
                throw new DmnModelException("Unable to parse model", e);
            }
        }
        public override IModelInstance EmptyModel
        {
            get
            {
                return (DmnModelInstanceImpl)base.EmptyModel;
            }
        }
    }
}