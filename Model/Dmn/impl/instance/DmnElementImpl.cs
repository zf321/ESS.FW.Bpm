using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public abstract class DmnElementImpl : DmnModelElementInstanceImpl, IDmnElement
    {

        protected internal static IAttribute/*<string>*/ IdAttribute;
        protected internal static IAttribute/*<string>*/ LabelAttribute;

        protected internal static IChildElement/*<IDescription>*/ DescriptionChild;
        protected internal static IChildElement/*<IExtensionElements>*/ ExtensionElementsChild;

        public DmnElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public override string Id
        {
            get => IdAttribute.GetValue<String>(this);
            set => IdAttribute.SetValue(this, value);
        }


        public virtual string Label
        {
            get => LabelAttribute.GetValue<String>(this);
            set => LabelAttribute.SetValue(this, value);
        }


        public virtual IDescription Description
        {
            get => (IDescription)DescriptionChild.GetChild(this);
            set => DescriptionChild.SetChild(this, value);
        }


        public virtual IExtensionElements ExtensionElements
        {
            get => (IExtensionElements)ExtensionElementsChild.GetChild(this);
            set => ExtensionElementsChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDmnElement>(/*typeof(IDmnElement),*/ DmnModelConstants.DmnElement).NamespaceUri(DmnModelConstants.Dmn11Ns).AbstractType();

            IdAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeId).IdAttribute().Build();

            LabelAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeLabel).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DescriptionChild = sequenceBuilder.Element<IDescription>(/*typeof(IDescription)*/).Build/*<IDescription>*/();

            ExtensionElementsChild = sequenceBuilder.Element<IExtensionElements>(/*typeof(IExtensionElements)*/).Build/*<IExtensionElements>*/();

            typeBuilder.Build();
        }
    }
}