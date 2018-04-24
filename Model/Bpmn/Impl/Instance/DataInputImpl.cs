using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class DataInputImpl : ItemAwareElementImpl, IDataInput
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<bool>*/ IsCollectionAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDataInput>(/*typeof(IDataInput),*/ BpmnModelConstants.BpmnElementDataInput)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IItemAwareElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            IsCollectionAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsCollection).DefaultValue(false).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDataInput>
        {
            public virtual IDataInput NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DataInputImpl(instanceContext);
            }
        }

        public DataInputImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get
            {
                return NameAttribute.GetValue<String>(this);
            }

            set
            {
                NameAttribute.SetValue(this, value);
            }
        }


        public virtual bool Collection
        {
            get
            {
                return IsCollectionAttribute.GetValue<Boolean>(this);
            }

            set
            {
                IsCollectionAttribute.SetValue(this, value);
            }
        }

    }

}