using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DocumentationImpl : BpmnModelElementInstanceImpl, IDocumentation
    {

        protected internal static IAttribute/*<string>*/ IdAttribute;
        protected internal static IAttribute/*<string>*/ TextFormatAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDocumentation>(/*typeof(IDocumentation),*/ BpmnModelConstants.BpmnElementDocumentation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            IdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeId).IdAttribute().Build();

            TextFormatAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTextFormat).DefaultValue("text/plain").Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDocumentation>
        {
            public virtual IDocumentation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DocumentationImpl(instanceContext);
            }
        }

        public DocumentationImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public override string Id
        {
            get
            {
                return IdAttribute.GetValue<String>(this);
            }
            set
            {
                IdAttribute.SetValue(this, value);
            }
        }

        public virtual string TextFormat
        {
            get
            {
                return TextFormatAttribute.GetValue<String>(this);
            }
            set
            {
                TextFormatAttribute.SetValue(this, value);
            }
        }


    }

}