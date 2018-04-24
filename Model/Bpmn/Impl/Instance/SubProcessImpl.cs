using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class SubProcessImpl : ActivityImpl, ISubProcess
    {

        protected internal static IAttribute/*<bool>*/ TriggeredByEventAttribute;
        protected internal static IChildElementCollection/*<ILaneSet>*/ LaneSetCollection;
        protected internal static IChildElementCollection/*<IFlowElement>*/ FlowElementCollection;
        protected internal static IChildElementCollection/*<IArtifact>*/ ArtifactCollection;

        /// <summary>
        /// camunda extensions </summary>
        protected internal static IAttribute/*<bool>*/ CamundaAsyncAttribute;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ISubProcess>(/*typeof(ISubProcess),*/ BpmnModelConstants.BpmnElementSubProcess)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IActivity))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            TriggeredByEventAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeTriggeredByEvent).DefaultValue(false).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            LaneSetCollection = sequenceBuilder.ElementCollection<ILaneSet>(/*typeof(ILaneSet)*/).Build/*<ILaneSet>*/();

            FlowElementCollection = sequenceBuilder.ElementCollection<IFlowElement>(/*typeof(IFlowElement)*/).Build/*<IFlowElement>*/();

            ArtifactCollection = sequenceBuilder.ElementCollection<IArtifact>(/*typeof(IArtifact)*/).Build/*<IArtifact>*/();            

            CamundaAsyncAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.CamundaAttributeAsync).Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false).Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ISubProcess>
        {
            public virtual ISubProcess NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new SubProcessImpl(instanceContext);
            }
        }

        public SubProcessImpl(ModelTypeInstanceContext context) : base(context)
        {
        }
        
        public new SubProcessBuilder Builder()
        {
            return new SubProcessBuilder((IBpmnModelInstance)modelInstance, this);
        }
        

        public virtual bool TriggeredByEvent
        {
            get => TriggeredByEventAttribute.GetValue<Boolean>(this);
            set => TriggeredByEventAttribute.SetValue(this, value);
        }

        public virtual ICollection<ILaneSet> LaneSets => LaneSetCollection.Get<ILaneSet>(this);

        public virtual ICollection<IFlowElement> FlowElements => FlowElementCollection.Get<IFlowElement>(this);

        public virtual ICollection<IArtifact> Artifacts => ArtifactCollection.Get<IArtifact>(this);

        /// <summary>
        /// camunda extensions </summary>

        /// @deprecated use isCamundaAsyncBefore() instead. 
        [Obsolete("use isCamundaAsyncBefore() instead.")]
        public virtual bool CamundaAsync
        {
            get => CamundaAsyncAttribute.GetValue<Boolean>(this);
            set => CamundaAsyncAttribute.SetValue(this, value);
        }
    }
}