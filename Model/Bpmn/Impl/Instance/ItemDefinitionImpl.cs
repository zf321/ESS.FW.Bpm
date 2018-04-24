using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ItemDefinitionImpl : RootElementImpl, IItemDefinition
    {

        protected internal static IAttribute/*<string>*/ StructureRefAttribute;
        protected internal static IAttribute/*<bool>*/ IsCollectionAttribute;
        protected internal static IAttribute/*<ItemKind>*/ ItemKindAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IItemDefinition>(/*typeof(IItemDefinition),*/ BpmnModelConstants.BpmnElementItemDefinition)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IRootElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            StructureRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeStructureRef).Build();

            IsCollectionAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsCollection).DefaultValue(false).Build();

            ItemKindAttribute = typeBuilder.EnumAttribute<ItemKind>(BpmnModelConstants.BpmnAttributeItemKind/*, typeof(ItemKind)*/)
                .DefaultValue(ItemKind.Information)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IItemDefinition>
        {
            public virtual IItemDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ItemDefinitionImpl(instanceContext);
            }
        }

        public ItemDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string StructureRef
        {
            get { return StructureRefAttribute.GetValue<String>(this); }
            set { StructureRefAttribute.SetValue(this, value); }
        }


        public virtual bool Collection
        {
            get { return IsCollectionAttribute.GetValue<Boolean>(this); }
            set { IsCollectionAttribute.SetValue(this, value); }
        }


        public virtual ItemKind ItemKind
        {
            get { return ItemKindAttribute.GetValue<ItemKind>(this); }
            set { ItemKindAttribute.SetValue(this, value); }
        }

    }

}