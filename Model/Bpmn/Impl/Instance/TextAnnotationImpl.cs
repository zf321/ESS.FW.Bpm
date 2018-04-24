

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class TextAnnotationImpl : ArtifactImpl, ITextAnnotation
    {

        protected internal static IAttribute/*<string>*/ TextFormatAttribute;
        protected internal static IChildElement/*<IText>*/ TextChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITextAnnotation>(/*typeof(ITextAnnotation),*/ BpmnModelConstants.BpmnElementTextAnnotation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IArtifact))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TextFormatAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeTextFormat).DefaultValue("text/plain").Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TextChild = sequenceBuilder.Element<IText>(/*typeof(IText)*/).Build/*<IText>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITextAnnotation>
        {
            public virtual ITextAnnotation NewInstance(ModelTypeInstanceContext context)
            {
                return new TextAnnotationImpl(context);
            }
        }

        public TextAnnotationImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string TextFormat
        {
            get => TextFormatAttribute.GetValue<String>(this);
            set => TextFormatAttribute.SetValue(this, value);
        }


        public virtual IText Text
        {
            get => (IText)TextChild.GetChild(this);
            set => TextChild.SetChild(this, value);
        }
    }
}