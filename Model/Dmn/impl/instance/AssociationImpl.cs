

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class AssociationImpl : ArtifactImpl, IAssociation
    {
        protected internal static IAttribute/*<AssociationDirection>*/ AssociationDirectionAttribute;
        //protected internal static Attribute<int> associationDirectionAttribute;

        //protected internal static IElementReference<IDmnElement, ISourceRef> SourceRef;
        //protected internal static IElementReference<IDmnElement, ITargetRef> TargetRef;
        protected internal static IElementReference SourceRef;//IElementReference<IDmnElement, ISourceRef>
        protected internal static IElementReference TargetRef;//IElementReference<IDmnElement, ITargetRef>

        public AssociationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual AssociationDirection AssociationDirection
        {
            get => AssociationDirectionAttribute.GetValue<AssociationDirection>(this);
            set => AssociationDirectionAttribute.SetValue(this, value);
        }


        public virtual IDmnElement Source
        {
            get => (IDmnElement)SourceRef.GetReferenceTargetElement(this);
            set => SourceRef.SetReferenceTargetElement(this, value);
        }


        public virtual IDmnElement Target
        {
            get => (IDmnElement)TargetRef.GetReferenceTargetElement(this);
            set => TargetRef.SetReferenceTargetElement(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAssociation>(/*typeof(IAssociation),*/ DmnModelConstants.DmnElementAssociation)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IArtifact))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            AssociationDirectionAttribute = typeBuilder.EnumAttribute<AssociationDirection>(DmnModelConstants.DmnAttributeAssociationDirection/*, typeof(AssociationDirection)*/)
                .DefaultValue(AssociationDirection.None)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            SourceRef = sequenceBuilder.Element<ISourceRef>(/*typeof(ISourceRef)*/).Required().UriElementReference<IDmnElement>(/*typeof(IDmnElement)*/).Build();

            TargetRef = sequenceBuilder.Element<ITargetRef>(/*typeof(ITargetRef)*/).Required().UriElementReference<IDmnElement>(/*typeof(IDmnElement)*/).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAssociation>
        {
            public virtual IAssociation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new AssociationImpl(instanceContext);
            }
        }

    }

}