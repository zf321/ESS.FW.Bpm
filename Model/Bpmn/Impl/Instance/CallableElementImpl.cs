using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;
using System;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class CallableElementImpl : RootElementImpl, ICallableElement
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IElementReferenceCollection SupportedInterfaceRefCollection;//IElementReferenceCollection<INterface, SupportedInterfaceRef>
        protected internal static IChildElement/*<IOSpecification>*/ IoSpecificationChild;
        protected internal static IChildElementCollection/*<IOBinding>*/ IoBindingCollection;

        public new static void RegisterType(ModelBuilder bpmnModelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = bpmnModelBuilder.DefineType<ICallableElement>(/*typeof(ICallableElement),*/ BpmnModelConstants.BpmnElementCallableElement)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IRootElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            SupportedInterfaceRefCollection = sequenceBuilder.ElementCollection<SupportedInterfaceRef>(/*typeof(SupportedInterfaceRef)*/)
                    .QNameElementReferenceCollection<INterface>(/*typeof(INterface)*/)
                    .Build();

            IoSpecificationChild = sequenceBuilder.Element<IOSpecification>(/*typeof(IOSpecification)*/).Build/*<IOSpecification>*/();

            IoBindingCollection = sequenceBuilder.ElementCollection<IOBinding>(/*typeof(IOBinding)*/).Build/*<IOBinding>*/();

            typeBuilder.Build();
        }

        public CallableElementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IModelElementInstance>
        {
            public virtual IModelElementInstance NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new CallableElementImpl(instanceContext);
            }
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }

        public virtual ICollection<INterface> SupportedInterfaces => SupportedInterfaceRefCollection.GetReferenceTargetElements<INterface>(this);

        public virtual IOSpecification IoSpecification
        {
            get => (IOSpecification)IoSpecificationChild.GetChild(this);
            set => IoSpecificationChild.SetChild(this, value);
        }

        public virtual ICollection<IOBinding> IoBindings => IoBindingCollection.Get<IOBinding>(this);
    }

}