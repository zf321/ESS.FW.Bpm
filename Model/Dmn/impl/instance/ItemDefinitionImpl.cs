using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class ItemDefinitionImpl : NamedElementImpl, ITemDefinition
    {

        protected internal static IAttribute/*<string>*/ TypeLanguageAttribute;
        protected internal static IAttribute/*<bool>*/ IsCollectionAttribute;

        protected internal static IChildElement/*<ITypeRef>*/ TypeRefChild;
        protected internal static IChildElement/*<IAllowedValues>*/ AllowedValuesChild;
        protected internal static IChildElementCollection/*<ITemComponent>*/ ItemComponentCollection;

        public ItemDefinitionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string TypeLanguage
        {
            get => TypeLanguageAttribute.GetValue<String>(this);
            set => TypeLanguageAttribute.SetValue(this, value);
        }


        public virtual bool Collection
        {
            get => IsCollectionAttribute.GetValue<Boolean>(this);
            set => IsCollectionAttribute.SetValue(this, value);
        }


        public virtual ITypeRef TypeRef
        {
            get => (ITypeRef)TypeRefChild.GetChild(this);
            set => TypeRefChild.SetChild(this, value);
        }


        public virtual IAllowedValues AllowedValues
        {
            get => (IAllowedValues)AllowedValuesChild.GetChild(this);
            set => AllowedValuesChild.SetChild(this, value);
        }

        public virtual ICollection<ITemComponent> ItemComponents => ItemComponentCollection.Get<ITemComponent>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ITemDefinition>(/*typeof(ITemDefinition),*/ DmnModelConstants.DmnElementItemDefinition).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(INamedElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TypeLanguageAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeTypeLanguage).Build();

            IsCollectionAttribute = typeBuilder.BooleanAttribute(DmnModelConstants.DmnAttributeIsCollection).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            TypeRefChild = sequenceBuilder.Element<ITypeRef>(/*typeof(ITypeRef)*/).Build/*<ITypeRef>*/();

            AllowedValuesChild = sequenceBuilder.Element<IAllowedValues>(/*typeof(IAllowedValues)*/).Build/*<IAllowedValues>*/();

            ItemComponentCollection = sequenceBuilder.ElementCollection<ITemComponent>(/*typeof(ITemComponent)*/).Build/*<ITemComponent>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ITemDefinition>
        {
            public virtual ITemDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ItemDefinitionImpl(instanceContext);
            }
        }

    }

}