using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    public class ElementCollectionImpl : NamedElementImpl, IElementCollection
    {

        protected internal static IElementReferenceCollection DrgElementRefCollection;//IElementReferenceCollection<IDrgElement, IDrgElementReference>

        public ElementCollectionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDrgElement> DrgElements => DrgElementRefCollection.GetReferenceTargetElements<IDrgElement>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IElementCollection>(/*typeof(IElementCollection),*/ DmnModelConstants.DmnElementElementCollection).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(INamedElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DrgElementRefCollection = sequenceBuilder
                .ElementCollection<IDrgElementReference>(/*typeof(IDrgElementReference)*/)
                .UriElementReferenceCollection<IDrgElement>(/*typeof(IDrgElement)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IElementCollection>
        {
            public virtual IElementCollection NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ElementCollectionImpl(instanceContext);
            }
        }

    }

}