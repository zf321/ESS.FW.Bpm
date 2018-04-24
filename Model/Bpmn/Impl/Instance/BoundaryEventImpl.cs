

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
    /// The BPMN boundaryEvent element
    /// 
    /// 
    /// </summary>
    public class BoundaryEventImpl : CatchEventImpl, IBoundaryEvent
    {

        protected internal static IAttribute/*<bool>*/ CancelActivityAttribute;
        protected internal static IAttributeReference AttachedToRefAttribute;//IAttributeReference<IActivity>

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBoundaryEvent>(/*typeof(IBoundaryEvent),*/ BpmnModelConstants.BpmnElementBoundaryEvent)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ICatchEvent))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            CancelActivityAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmnAttributeCancelActivity)
                .DefaultValue(true).Build();

            AttachedToRefAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeAttachedToRef)
                .Required()
                .QNameAttributeReference<IActivity>(/*typeof(IActivity)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBoundaryEvent>
        {
            public virtual IBoundaryEvent NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BoundaryEventImpl(instanceContext);
            }
        }

        public BoundaryEventImpl(ModelTypeInstanceContext context) : base(context)
        {
        }

        public new BoundaryEventBuilder Builder()
        {
            base.Builder();
            return new BoundaryEventBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual bool CancelActivity
        {
            get => CancelActivityAttribute.GetValue<Boolean>(this);
            set => CancelActivityAttribute.SetValue(this, value);
        }

        public virtual IActivity AttachedTo
        {
            get => AttachedToRefAttribute.GetReferenceTargetElement<IActivity>(this);
            set => AttachedToRefAttribute.SetReferenceTargetElement(this, value);
        }

        #region not exist in java source

        public override bool CamundaAsyncAfter
        {
            get
            {
                throw new System.NotSupportedException("'asyncAfter' is not supported for 'Boundary Events' right now.");
            }
            set
            {
                throw new System.NotSupportedException("'asyncAfter' is not supported for 'Boundary Events' right now.");
            }
        }


        public override bool CamundaAsyncBefore
        {
            get
            {
                throw new System.NotSupportedException("'asyncBefore' is not supported for 'Boundary Events' right now.");
            }
            set
            {
                throw new System.NotSupportedException("'asyncBefore' is not supported for 'Boundary Events' right now.");
            }
        }


        public override bool CamundaExclusive
        {
            get
            {
                throw new System.NotSupportedException("'exclusive' is not supported for 'Boundary Events' right now.");
            }
            set
            {
                throw new System.NotSupportedException("'exclusive' is not supported for 'Boundary Events' right now.");
            }
        }

        #endregion


    }
}