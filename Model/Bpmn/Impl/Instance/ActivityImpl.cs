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

    /// <summary>
    /// The BPMN activity element
    /// 
    /// 
    /// </summary>
    public abstract class ActivityImpl : FlowNodeImpl, IActivity
    {

        protected internal static IAttribute/*<bool>*/ IsForCompensationAttribute;
        protected internal static IAttribute/*<int?>*/ StartQuantityAttribute;
        protected internal static IAttribute/*<int?>*/ CompletionQuantityAttribute;
        protected internal static IAttributeReference DefaultAttribute;//IAttributeReference<ISequenceFlow>
        protected internal static IChildElement/*<IOSpecification>*/ IoSpecificationChild;
        protected internal static IChildElementCollection/*<IProperty>*/ PropertyCollection;
        protected internal static IChildElementCollection/*<IDataInputAssociation>*/ DataInputAssociationCollection;
        protected internal static IChildElementCollection/*<IDataOutputAssociation>*/ DataOutputAssociationCollection;
        protected internal static IChildElementCollection/*<IResourceRole>*/ ResourceRoleCollection;
        protected internal static IChildElement/*<ILoopCharacteristics>*/ LoopCharacteristicsChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IActivity>(/*typeof(IActivity),*/ BpmnModelConstants.BpmnElementActivity)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFlowNode))
                .AbstractType();

            IsForCompensationAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeIsForCompensation).DefaultValue(false).Build();

            StartQuantityAttribute = typeBuilder.IntegerAttribute(BpmnModelConstants.BpmnAttributeStartQuantity).DefaultValue(1).Build();

            

            CompletionQuantityAttribute = typeBuilder.IntegerAttribute(BpmnModelConstants.BpmnAttributeCompletionQuantity).DefaultValue(1).Build();

            //IAttributeReference<ISequenceFlow>
            DefaultAttribute = (IAttributeReference) typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeDefault)
                .IdAttributeReference<ISequenceFlow>(/*typeof(ISequenceFlow)*/).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            IoSpecificationChild = sequenceBuilder.Element<IOSpecification>(/*typeof(IOSpecification)*/).Build/*<IOSpecification>*/();

            PropertyCollection = sequenceBuilder.ElementCollection<IProperty>(/*typeof(IProperty)*/).Build/*<IProperty>*/();

            DataInputAssociationCollection = sequenceBuilder.ElementCollection<IDataInputAssociation>(/*typeof(IDataInputAssociation)*/).Build/*<IDataInputAssociation>*/();

            DataOutputAssociationCollection = sequenceBuilder.ElementCollection<IDataOutputAssociation>(/*typeof(IDataOutputAssociation)*/).Build/*<IDataOutputAssociation>*/();

            ResourceRoleCollection = sequenceBuilder.ElementCollection<IResourceRole>(/*typeof(IResourceRole)*/).Build/*<IResourceRole>*/();

            LoopCharacteristicsChild = sequenceBuilder.Element<ILoopCharacteristics>(/*typeof(ILoopCharacteristics)*/).Build/*<ILoopCharacteristics>*/();

            typeBuilder.Build();
        }

        public ActivityImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual bool ForCompensation
        {
            get => IsForCompensationAttribute.GetValue<Boolean>(this);
            set => IsForCompensationAttribute.SetValue(this, value);
        }


        public virtual int StartQuantity
        {
            get => StartQuantityAttribute.GetValue<Int32?>(this).GetValueOrDefault();
            set => StartQuantityAttribute.SetValue(this, value);
        }


        public virtual int CompletionQuantity
        {
            get
            {
                return CompletionQuantityAttribute.GetValue<Int32?>(this).GetValueOrDefault();
            }
            set
            {
                CompletionQuantityAttribute.SetValue(this, value);
            }
        }


        public virtual ISequenceFlow Default
        {
            get => /*(ISequenceFlow)*/ DefaultAttribute.GetReferenceTargetElement<ISequenceFlow>(this);
            set => DefaultAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IOSpecification IoSpecification
        {
            get
            {
                return (IOSpecification)IoSpecificationChild.GetChild(this);
            }
            set
            {
                IoSpecificationChild.SetChild(this, value);
            }
        }


        public virtual ICollection<IProperty> Properties
        {
            get
            {
                return PropertyCollection.Get<IProperty>(this);
            }
        }

        public virtual ICollection<IDataInputAssociation> DataInputAssociations
        {
            get
            {
                return DataInputAssociationCollection.Get<IDataInputAssociation>(this);
            }
        }

        public virtual ICollection<IDataOutputAssociation> DataOutputAssociations
        {
            get
            {
                return DataOutputAssociationCollection.Get<IDataOutputAssociation>(this);
            }
        }

        public virtual ICollection<IResourceRole> ResourceRoles
        {
            get
            {
                return ResourceRoleCollection.Get<IResourceRole>(this);
            }
        }

        public virtual ILoopCharacteristics LoopCharacteristics
        {
            get
            {
                return (ILoopCharacteristics)LoopCharacteristicsChild.GetChild(this);
            }
            set
            {
                LoopCharacteristicsChild.SetChild(this, value);
            }
        }
    }
}