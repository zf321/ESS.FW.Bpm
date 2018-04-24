

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class ResourceParameterImpl : BaseElementImpl, IResourceParameter
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference TypeAttribute;//IAttributeReference<IItemDefinition>
        protected internal static IAttribute/*<bool>*/ IsRequiredAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IResourceParameter>(/*typeof(IResourceParameter),*/ BpmnModelConstants.BpmnElementResourceParameter)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            TypeAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeType)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            IsRequiredAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsRequired).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IResourceParameter>
        {
            public virtual IResourceParameter NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ResourceParameterImpl(instanceContext);
            }
        }

        public ResourceParameterImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Type
        {
            get => TypeAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => TypeAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual bool Required
        {
            get => IsRequiredAttribute.GetValue<Boolean>(this);
            set => IsRequiredAttribute.SetValue(this, value);
        }
    }
}