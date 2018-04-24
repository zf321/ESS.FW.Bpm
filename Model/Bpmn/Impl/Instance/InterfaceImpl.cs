using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class InterfaceImpl : RootElementImpl, INterface
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttribute/*<string>*/ ImplementationRefAttribute;
        protected internal static IChildElementCollection/*<IOperation>*/ OperationCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INterface>(/*typeof(INterface),*/ BpmnModelConstants.BpmnElementInterface)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IRootElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Required().Build();

            ImplementationRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeImplementationRef).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            OperationCollection = sequenceBuilder.ElementCollection<IOperation>(/*typeof(IOperation)*/).Required().Build/*<IOperation>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INterface>
        {
            public virtual INterface NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InterfaceImpl(instanceContext);
            }
        }

        public InterfaceImpl(ModelTypeInstanceContext context) : base(context)
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

        public virtual ICollection<IOperation> Operations => OperationCollection.Get<IOperation>(this);
    }
}