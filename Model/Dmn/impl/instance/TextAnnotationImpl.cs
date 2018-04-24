using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class TextAnnotationImpl : ArtifactImpl, ITextAnnotation
    {

        protected internal static IAttribute/*<string>*/ TextFormatAttribute;

        protected internal static IChildElement/*<IText>*/ TextChild;

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


        public TextAnnotationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITextAnnotation>(/*typeof(ITextAnnotation),*/ DmnModelConstants.DmnElementTextAnnotation).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IArtifact)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TextFormatAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTextFormat).DefaultValue("text/plain").Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TextChild = sequenceBuilder.Element<IText>(/*typeof(IText)*/).Build/*<IText>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITextAnnotation>
        {
            public virtual ITextAnnotation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new TextAnnotationImpl(instanceContext);
            }
        }

    }

}