using System.Xml;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.impl.parser;
using ESS.FW.Bpm.Model.Xml.impl.util;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Bpmn.impl
{
    
    public class BpmnParser : AbstractModelParser
    {
        private const string JaxpSchemaSource = "http://java.sun.com/xml/jaxp/properties/schemaSource";
        private const string JaxpSchemaLanguage = "http://java.sun.com/xml/jaxp/properties/schemaLanguage";

        private const string W3CXmlSchema = "http://www.w3.org/2001/XMLSchema";

        public BpmnParser():base()
        {
            //this.schemaFactory = SchemaFactory.newInstance<T>(W3C_XML_SCHEMA);
            AddSchema(BpmnModelConstants.Bpmn20Ns,CreateSchema(BpmnModelConstants.Bpmn20SchemaLocation));
        }


        protected internal override void ConfigureFactory()
        {
            //dbf.setAttribute(JaxpSchemaLanguage, W3CXmlSchema);
            //dbf.setAttribute(JaxpSchemaSource, ReflectUtil.getResource(BpmnModelConstants.Bpmn20SchemaLocation, typeof(BpmnParser).ClassLoader).ToString());
            base.ConfigureFactory(/*dbf*/);
        }

        protected internal override IModelInstance CreateModelInstance(IDomDocument document)
        {
            return new BpmnModelInstanceImpl((ModelImpl)Bpmn.Instance.BpmnModel, Bpmn.Instance.BpmnModelBuilder, document);
        }

        public override IModelInstance ParseModelFromStream(System.IO.Stream inputStream)
        {
            return (BpmnModelInstanceImpl)base.ParseModelFromStream(inputStream);
        }

        public override IModelInstance EmptyModel => (BpmnModelInstanceImpl)base.EmptyModel;
    }
}