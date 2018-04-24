using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    public class KnowledgeSourceImpl : DrgElementImpl, IKnowledgeSource
    {

        protected internal static IAttribute/*<string>*/ LocationUriAttribute;

        protected internal static IChildElementCollection/*<IAuthorityRequirement>*/ AuthorityRequirementCollection;
        protected internal static IChildElement/*<IType>*/ TypeChild;
        protected internal static IElementReference OwnerRef;//IElementReference<IOrganizationUnit, IOwnerReference>

        public KnowledgeSourceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string LocationUri
        {
            get => LocationUriAttribute.GetValue<String>(this);
            set => LocationUriAttribute.SetValue(this, value);
        }


        public virtual ICollection<IAuthorityRequirement> AuthorityRequirement => AuthorityRequirementCollection.Get<IAuthorityRequirement>(this);

        public virtual IType Type
        {
            get => (IType)TypeChild.GetChild(this);
            set => TypeChild.SetChild(this, value);
        }


        public virtual IOrganizationUnit Owner
        {
            get => (IOrganizationUnit)OwnerRef.GetReferenceTargetElement(this);
            set => OwnerRef.SetReferenceTargetElement(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IKnowledgeSource>(/*typeof(IKnowledgeSource),*/ DmnModelConstants.DmnElementKnowledgeSource).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDrgElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            LocationUriAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeLocationUri).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            AuthorityRequirementCollection = sequenceBuilder.ElementCollection<IAuthorityRequirement>(/*typeof(IAuthorityRequirement)*/).Build/*<IAuthorityRequirement>*/();

            TypeChild = sequenceBuilder.Element<IType>(/*typeof(IType)*/).Build/*<IType>*/();

            OwnerRef = sequenceBuilder
                .Element<IOwnerReference>(/*typeof(IOwnerReference)*/)
                .UriElementReference<IOrganizationUnit>(/*typeof(IOrganizationUnit)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IKnowledgeSource>
        {
            public virtual IKnowledgeSource NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new KnowledgeSourceImpl(instanceContext);
            }
        }

    }

}