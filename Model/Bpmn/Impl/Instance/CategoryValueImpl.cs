using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CategoryValueImpl : BaseElementImpl, ICategoryValue
    {

        protected internal static IAttribute/*<string>*/ ValueAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICategoryValue>(/*typeof(ICategoryValue),*/ BpmnModelConstants.BpmnElementCategoryValue)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ValueAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeValue).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICategoryValue>
        {
            public virtual ICategoryValue NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CategoryValueImpl(instanceContext);
            }
        }

        public CategoryValueImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Value
        {
            get
            {
                return ValueAttribute.GetValue<String>(this);
            }

            set
            {
                ValueAttribute.SetValue(this, value);
            }
        }

    }

}