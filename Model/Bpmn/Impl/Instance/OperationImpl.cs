using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class OperationImpl : BaseElementImpl, IOperation
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ ImplementationRefAttribute;
        protected internal static IElementReference InMessageRefChild;//IElementReference<IMessage, InMessageRef>
        protected internal static IElementReference OutMessageRefChild;//IElementReference<IMessage, OutMessageRef>
        protected internal static IElementReferenceCollection ErrorRefCollection;//IElementReferenceCollection<IError, ErrorRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOperation>(/*typeof(IOperation),*/ BpmnModelConstants.BpmnElementOperation)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Required().Build();

            ImplementationRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnElementImplementationRef).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InMessageRefChild = sequenceBuilder
                .Element<InMessageRef>(/*typeof(InMessageRef)*/)
                .Required()
                .QNameElementReference<IMessage>(/*typeof(IMessage)*/)
                .Build();

            OutMessageRefChild = sequenceBuilder
                .Element<OutMessageRef>(/*typeof(OutMessageRef)*/)
                .QNameElementReference<IMessage>(/*typeof(IMessage)*/)
                .Build();

            ErrorRefCollection = sequenceBuilder
                .ElementCollection<ErrorRef>(/*typeof(ErrorRef)*/)
                .QNameElementReferenceCollection<IError>(/*typeof(IError)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOperation>
        {
            public virtual IOperation NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OperationImpl(instanceContext);
            }
        }

        public OperationImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual string ImplementationRef
        {
            get => ImplementationRefAttribute.GetValue<String>(this);
            set => ImplementationRefAttribute.SetValue(this, value);
        }


        public virtual IMessage InMessage
        {
            get => (IMessage)InMessageRefChild.GetReferenceTargetElement(this);
            set => InMessageRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual IMessage OutMessage
        {
            get => (IMessage)OutMessageRefChild.GetReferenceTargetElement(this);
            set => OutMessageRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual ICollection<IError> Errors => ErrorRefCollection.GetReferenceTargetElements<IError>(this);
    }
}