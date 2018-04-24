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
    public abstract class FlowElementImpl : BaseElementImpl, IFlowElement
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IChildElement/*<IAuditing>*/ AuditingChild;
        protected internal static IChildElement/*<IMonitoring>*/ MonitoringChild;
        protected internal static IElementReferenceCollection CategoryValueRefCollection;//IElementReferenceCollection<ICategoryValue, CategoryValueRef>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {

            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFlowElement>(/*typeof(IFlowElement),*/ BpmnModelConstants.BpmnElementFlowElement)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IBaseElement))
                    .AbstractType();

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            AuditingChild = sequenceBuilder.Element<IAuditing>(/*typeof(IAuditing)*/).Build/*<IAuditing>*/();

            MonitoringChild = sequenceBuilder.Element<IMonitoring>(/*typeof(IMonitoring)*/).Build/*<IMonitoring>*/();

            CategoryValueRefCollection = sequenceBuilder
                .ElementCollection<CategoryValueRef>(/*typeof(CategoryValueRef)*/)
                .QNameElementReferenceCollection<ICategoryValue>(/*typeof(ICategoryValue)*/)
                .Build();

            typeBuilder.Build();
        }

        public FlowElementImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual IAuditing Auditing
        {
            get => (IAuditing)AuditingChild.GetChild(this);
            set => AuditingChild.SetChild(this, value);
        }


        public virtual IMonitoring Monitoring
        {
            get => (IMonitoring)MonitoringChild.GetChild(this);
            set => MonitoringChild.SetChild(this, value);
        }


        public virtual ICollection<ICategoryValue> CategoryValueRefs => CategoryValueRefCollection.GetReferenceTargetElements<ICategoryValue>(this);
    }

}