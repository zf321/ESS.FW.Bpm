

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataStoreImpl : RootElementImpl, IDataStore
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<int?>*/ CapacityAttribute;
        protected internal static IAttribute/*<bool>*/ IsUnlimitedAttribute;
        protected internal static IAttributeReference ItemSubjectRefAttribute;//IAttributeReference<IItemDefinition>
        protected internal static IChildElement/*<IDataState>*/ DataStateChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataStore>(/*typeof(IDataStore),*/ BpmnModelConstants.BpmnElementDataStore)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            CapacityAttribute = typeBuilder.IntegerAttribute(BpmnModelConstants.BpmnAttributeCapacity).Build();

            IsUnlimitedAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsUnlimited).DefaultValue(true).Build();

            ItemSubjectRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeItemSubjectRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DataStateChild = sequenceBuilder.Element<IDataState>(/*typeof(IDataState)*/).Build/*<IDataState>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataStore>
        {
            public virtual IDataStore NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataStoreImpl(instanceContext);
            }
        }

        public DataStoreImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }

        public virtual int? Capacity
        {
            get => CapacityAttribute.GetValue<Int32?>(this).GetValueOrDefault();
            set => CapacityAttribute.SetValue(this, value);
        }

        public virtual bool Unlimited
        {
            get => IsUnlimitedAttribute.GetValue<Boolean>(this);
            set => IsUnlimitedAttribute.SetValue(this, value);
        }

        public virtual IItemDefinition ItemSubject
        {
            get => ItemSubjectRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => ItemSubjectRefAttribute.SetReferenceTargetElement(this, value);
        }

        public virtual IDataState DataState
        {
            get => (IDataState)DataStateChild.GetChild(this);
            set => DataStateChild.SetChild(this, value);
        }
    }
}