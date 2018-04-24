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
    public class ErrorImpl : RootElementImpl, IError
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ ErrorCodeAttribute;

        protected internal static IAttributeReference StructureRefAttribute;//IAttributeReference<IItemDefinition>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IError>(/*typeof(IError),*/ BpmnModelConstants.BpmnElementError)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeName)
                .Build();

            ErrorCodeAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeErrorCode)
                .Build();

            StructureRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeStructureRef)
                .QNameAttributeReference<IItemDefinition>(/*typeof(IItemDefinition)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IError>
        {
            public virtual IError NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ErrorImpl(instanceContext);
            }
        }

        public ErrorImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual string ErrorCode
        {
            get => ErrorCodeAttribute.GetValue<String>(this);
            set => ErrorCodeAttribute.SetValue(this, value);
        }


        public virtual IItemDefinition Structure
        {
            get => StructureRefAttribute.GetReferenceTargetElement<IItemDefinition>(this);
            set => StructureRefAttribute.SetReferenceTargetElement(this, value);
        }

    }

}