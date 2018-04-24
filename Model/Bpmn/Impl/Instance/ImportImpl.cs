using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ImportImpl : BpmnModelElementInstanceImpl, IMport
    {

        protected internal static IAttribute/*<string>*/ NamespaceAttribute;
        protected internal static IAttribute/*<string>*/ LocationAttribute;
        protected internal static IAttribute/*<string>*/ ImportTypeAttribute;

        public new static void RegisterType(ModelBuilder bpmnModelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = bpmnModelBuilder.DefineType<IMport>(/*typeof(IMport), */BpmnModelConstants.BpmnElementImport)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NamespaceAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeNamespace).Required().Build();

            LocationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeLocation).Required().Build();

            ImportTypeAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImportType).Required().Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMport>
        {
            public virtual IMport NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ImportImpl(instanceContext);
            }
        }

        public ImportImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Namespace
        {
            get
            {
                return NamespaceAttribute.GetValue<String>(this);
            }
            set
            {
                NamespaceAttribute.SetValue(this, value);
            }
        }


        public virtual string Location
        {
            get
            {
                return LocationAttribute.GetValue<String>(this);
            }
            set
            {
                LocationAttribute.SetValue(this, value);
            }
        }


        public virtual string ImportType
        {
            get
            {
                return ImportTypeAttribute.GetValue<String>(this);
            }
            set
            {
                ImportTypeAttribute.SetValue(this, value);
            }
        }


    }

}