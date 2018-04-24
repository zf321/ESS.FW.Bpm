
using System;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class SequenceFlowImpl : FlowElementImpl, ISequenceFlow
    {

        protected internal static IAttributeReference SourceRefAttribute;//IAttributeReference<IFlowNode>
        protected internal static IAttributeReference TargetRefAttribute;//IAttributeReference<IFlowNode>
        protected internal static IAttribute/*<bool>*/ IsImmediateAttribute;
        protected internal static IChildElement/*<IConditionExpression>*/ ConditionExpressionCollection;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ISequenceFlow>(/*typeof(ISequenceFlow),*/ BpmnModelConstants.BpmnElementSequenceFlow)
                    .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                    .ExtendsType(typeof(IFlowElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            SourceRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeSourceRef)
                .Required()
                .IdAttributeReference<IFlowNode>(/*typeof(IFlowNode)*/)
                .Build();

            TargetRefAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnAttributeTargetRef)
                .Required()
                .IdAttributeReference<IFlowNode>(/*typeof(IFlowNode)*/)
                .Build();

            IsImmediateAttribute = typeBuilder
                .BooleanAttribute(BpmnModelConstants.BpmnAttributeIsImmediate)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ConditionExpressionCollection = sequenceBuilder
                .Element<IConditionExpression>(/*typeof(IConditionExpression)*/)
                .Build/*<IConditionExpression>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ISequenceFlow>
        {
            public virtual ISequenceFlow NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SequenceFlowImpl(instanceContext);
            }
        }

        public SequenceFlowImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public SequenceFlowBuilder Builder()
        {
            return new SequenceFlowBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual IFlowNode Source
        {
            get => SourceRefAttribute.GetReferenceTargetElement<IFlowNode>(this);
            set => SourceRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IFlowNode Target
        {
            get => TargetRefAttribute.GetReferenceTargetElement<IFlowNode>(this);
            set => TargetRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual bool Immediate
        {
            get => IsImmediateAttribute.GetValue<Boolean>(this);
            set => IsImmediateAttribute.SetValue(this, value);
        }


        public virtual IConditionExpression ConditionExpression
        {
            get => (IConditionExpression)ConditionExpressionCollection.GetChild(this);
            set => ConditionExpressionCollection.SetChild(this, value);
        }


        public virtual void RemoveConditionExpression()
        {
            ConditionExpressionCollection.RemoveChild(this);
        }
        

        public new IBpmnEdge DiagramElement => (IBpmnEdge) base.DiagramElement;
    }

}