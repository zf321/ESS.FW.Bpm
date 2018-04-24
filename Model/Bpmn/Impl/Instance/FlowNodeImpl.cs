using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{

    /// <summary>
    /// The BPMN flowNode element
    /// 
    /// 
    /// </summary>
    public abstract class FlowNodeImpl : FlowElementImpl, IFlowNode
    {

        protected internal static IElementReferenceCollection IncomingCollection;//IElementReferenceCollection<ISequenceFlow, Incoming>
        protected internal static IElementReferenceCollection OutgoingCollection;//IElementReferenceCollection<ISequenceFlow, Outgoing>

        /// <summary>
        /// Camunda Attributes </summary>
        protected internal static IAttribute/*<bool>*/ camundaAsyncAfter;
        protected internal static IAttribute/*<bool>*/ camundaAsyncBefore;
        protected internal static IAttribute/*<bool>*/ camundaExclusive;
        protected internal static IAttribute/*<string>*/ camundaJobPriority;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IFlowNode>(/*typeof(IFlowNode),*/ BpmnModelConstants.BpmnElementFlowNode)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IFlowElement))
                .AbstractType();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            IncomingCollection = sequenceBuilder
                .ElementCollection<Incoming>(/*typeof(Incoming)*/)
                .QNameElementReferenceCollection<ISequenceFlow>(/*typeof(ISequenceFlow)*/)
                .Build();

            OutgoingCollection = sequenceBuilder
                .ElementCollection<Outgoing>(/*typeof(Outgoing)*/)
                .QNameElementReferenceCollection<ISequenceFlow>(/*typeof(ISequenceFlow)*/)
                .Build();

            camundaAsyncAfter = typeBuilder
                .BooleanAttribute(BpmnModelConstants.CamundaAttributeAsyncAfter)
                .Namespace(BpmnModelConstants.CamundaNs)
                .DefaultValue(false)
                .Build();

            camundaAsyncBefore = typeBuilder
                .BooleanAttribute(BpmnModelConstants.CamundaAttributeAsyncBefore)
                .Namespace(BpmnModelConstants.CamundaNs)
                .DefaultValue(false)
                .Build();

            camundaExclusive = typeBuilder
                .BooleanAttribute(BpmnModelConstants.CamundaAttributeExclusive)
                .Namespace(BpmnModelConstants.CamundaNs)
                .DefaultValue(true)
                .Build();

            camundaJobPriority = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeJobPriority)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        public FlowNodeImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public virtual AbstractFlowNodeBuilder<IFlowNode> Builder()
        {
            throw new BpmnModelException("No builder implemented for type " + ElementType.TypeNamespace + ":" + ElementType.TypeName);
        }
        
        public new virtual void UpdateAfterReplacement()
        {
            base.UpdateAfterReplacement();
            ICollection<IReference> incomingReferences = GetIncomingReferencesByType(typeof(ISequenceFlow));
            foreach (IReference reference in incomingReferences)
            {
                foreach (IModelElementInstance sourceElement in reference.FindReferenceSourceElements(this))
                {
                    string referenceIdentifier = reference.GetReferenceIdentifier(sourceElement);

                    if (referenceIdentifier != null && referenceIdentifier.Equals(Id) && reference is IAttributeReference)
                    {
                        string attributeName = ((IAttributeReference)reference).ReferenceSourceAttribute.AttributeName;
                        if (attributeName.Equals(BpmnModelConstants.BpmnAttributeSourceRef))
                        {
                            Outgoing.Add((ISequenceFlow)sourceElement);
                        }
                        else if (attributeName.Equals(BpmnModelConstants.BpmnAttributeTargetRef))
                        {
                            Incoming.Add((ISequenceFlow)sourceElement);
                        }
                    }
                }
            }
        }

        public virtual ICollection<ISequenceFlow> Incoming => IncomingCollection.GetReferenceTargetElements<ISequenceFlow>(this);

        public virtual ICollection<ISequenceFlow> Outgoing => OutgoingCollection.GetReferenceTargetElements<ISequenceFlow>(this);

        public virtual IQuery<IFlowNode> PreviousNodes
        {
            get
            {
                ICollection<IFlowNode> previousNodes = new HashSet<IFlowNode>();
                foreach (ISequenceFlow sequenceFlow in Incoming)
                {
                    previousNodes.Add(sequenceFlow.Source);
                }
                return new QueryImpl<IFlowNode>(previousNodes);
            }
        }

        public virtual IQuery<IFlowNode> SucceedingNodes
        {
            get
            {
                ICollection<IFlowNode> succeedingNodes = new HashSet<IFlowNode>();
                foreach (ISequenceFlow sequenceFlow in Outgoing)
                {
                    succeedingNodes.Add(sequenceFlow.Target);
                }
                return new QueryImpl<IFlowNode>(succeedingNodes);
            }
        }

        /// <summary>
        /// Camunda Attributes </summary>

        public virtual bool CamundaAsyncBefore
        {
            get => camundaAsyncBefore.GetValue<Boolean>(this);
            set => camundaAsyncBefore.SetValue(this, value);
        }


        public virtual bool CamundaAsyncAfter
        {
            get => camundaAsyncAfter.GetValue<Boolean>(this);
            set => camundaAsyncAfter.SetValue(this, value);
        }


        public virtual bool CamundaExclusive
        {
            get => camundaExclusive.GetValue<Boolean>(this);
            set => camundaExclusive.SetValue(this, value);
        }


        public virtual string CamundaJobPriority
        {
            get => camundaJobPriority.GetValue<String>(this);
            set => camundaJobPriority.SetValue(this, value);
        }

    }

}