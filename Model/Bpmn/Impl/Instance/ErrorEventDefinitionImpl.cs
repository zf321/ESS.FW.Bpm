

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
    public class ErrorEventDefinitionImpl : EventDefinitionImpl, IErrorEventDefinition
    {

        protected internal static IAttributeReference ErrorRefAttribute;//IAttributeReference<IError>
        protected internal static IAttribute/*<string>*/ CamundaErrorCodeVariableAttribute;
        protected internal static IAttribute/*<string>*/ CamundaErrorMessageVariableAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IErrorEventDefinition>(/*typeof(IErrorEventDefinition),*/ BpmnModelConstants.BpmnElementErrorEventDefinition)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IEventDefinition))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ErrorRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeErrorRef)
                .QNameAttributeReference<IError>(/*typeof(IError)*/)
                .Build();

            CamundaErrorCodeVariableAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeErrorCodeVariable)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            CamundaErrorMessageVariableAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeErrorMessageVariable)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IErrorEventDefinition>
        {

            public virtual IErrorEventDefinition NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ErrorEventDefinitionImpl(instanceContext);
            }
        }

        public ErrorEventDefinitionImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual IError Error
        {
            get => ErrorRefAttribute.GetReferenceTargetElement<IError>(this);
            set => ErrorRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual string CamundaErrorCodeVariable
        {
            set => CamundaErrorCodeVariableAttribute.SetValue(this, value);
            get => CamundaErrorCodeVariableAttribute.GetValue<String>(this);
        }


        public virtual string CamundaErrorMessageVariable
        {
            set => CamundaErrorMessageVariableAttribute.SetValue(this, value);
            get => CamundaErrorMessageVariableAttribute.GetValue<String>(this);
        }

    }

}