﻿using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    public class LaneImpl : BaseElementImpl, ILane
    {

        protected internal static IAttribute/*<string>*/ NameAttribute;
        protected internal static IAttributeReference PartitionElementRefAttribute;//IAttributeReference<PartitionElement>
        protected internal static IChildElement/*<PartitionElement>*/ partitionElementChild;
        protected internal static IElementReferenceCollection FlowNodeRefCollection;//IElementReferenceCollection<IFlowNode, FlowNodeRef>
        protected internal static IChildElement/*<ChildLaneSet>*/ ChildLaneSetChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ILane>(/*typeof(ILane),*/ BpmnModelConstants.BpmnElementLane)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

            PartitionElementRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributePartitionElementRef)
                .QNameAttributeReference<PartitionElement>(/*typeof(PartitionElement)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            partitionElementChild = sequenceBuilder.Element<PartitionElement>(/*typeof(PartitionElement)*/).Build/*<PartitionElement>*/();

            FlowNodeRefCollection = sequenceBuilder
                .ElementCollection<FlowNodeRef>(/*typeof(FlowNodeRef)*/)
                .IdElementReferenceCollection<IFlowNode>(/*typeof(IFlowNode)*/)
                .Build();

            ChildLaneSetChild = sequenceBuilder.Element<ChildLaneSet>(/*typeof(ChildLaneSet)*/).Build/*<ChildLaneSet>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ILane>
        {
            public virtual ILane NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new LaneImpl(instanceContext);
            }
        }

        public LaneImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual string Name
        {
            get => NameAttribute.GetValue<String>(this);
            set => NameAttribute.SetValue(this, value);
        }


        public virtual PartitionElement PartitionElement
        {
            get => PartitionElementRefAttribute.GetReferenceTargetElement<PartitionElement>(this);
            set => PartitionElementRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual PartitionElement PartitionElementChild
        {
            get => (PartitionElement)partitionElementChild.GetChild(this);
            set => partitionElementChild.SetChild(this, value);
        }


        public virtual ICollection<IFlowNode> FlowNodeRefs => FlowNodeRefCollection.GetReferenceTargetElements<IFlowNode>(this);

        public virtual ChildLaneSet ChildLaneSet
        {
            get => (ChildLaneSet)ChildLaneSetChild.GetChild(this);
            set => ChildLaneSetChild.SetChild(this, value);
        }

    }

}