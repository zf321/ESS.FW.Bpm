

using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
    /// The BPMN receiveTask element
    /// 
    /// 
    /// </summary>
    public class ReceiveTaskImpl : TaskImpl, IReceiveTask
    {

        protected internal static IAttribute/*<string>*/ ImplementationAttribute;
        protected internal static IAttribute/*<bool>*/ InstantiateAttribute;
        protected internal static IAttributeReference MessageRefAttribute;//IAttributeReference<IMessage> 
        protected internal static IAttributeReference OperationRefAttribute;//IAttributeReference<IOperation>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IReceiveTask>(/*typeof(IReceiveTask),*/ BpmnModelConstants.BpmnElementReceiveTask)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ITask))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ImplementationAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImplementation).DefaultValue("##WebService").Build();

            InstantiateAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeInstantiate).DefaultValue(false).Build();

            MessageRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeMessageRef)
                .QNameAttributeReference<IMessage>(/*typeof(IMessage)*/)
                .Build();

            OperationRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeOperationRef)
                .QNameAttributeReference<IOperation>(/*typeof(IOperation)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IReceiveTask>
        {
            public virtual IReceiveTask NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new ReceiveTaskImpl(instanceContext);
            }
        }

        public ReceiveTaskImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new ReceiveTaskBuilder Builder()
        {
            return new ReceiveTaskBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual string Implementation
        {
            get => ImplementationAttribute.GetValue<String>(this);
            set => ImplementationAttribute.SetValue(this, value);
        }
        
        public virtual bool Instantiate
        {
            get => InstantiateAttribute.GetValue<Boolean>(this);
            set => InstantiateAttribute.SetValue(this, value);
        }

        public virtual IMessage Message
        {
            get => MessageRefAttribute.GetReferenceTargetElement<IMessage>(this);
            set => MessageRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IOperation Operation
        {
            get => OperationRefAttribute.GetReferenceTargetElement<IOperation>(this);
            set => OperationRefAttribute.SetReferenceTargetElement(this, value);
        }

    }
}