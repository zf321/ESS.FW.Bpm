using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;
using ESS.FW.Bpm.Model.Bpmn.impl.instance.di;
using ESS.FW.Bpm.Model.Xml.impl.type.reference;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{
    
    /// <summary>
    /// The BPMNDI BPMNShape element
    /// 
    /// 
    /// </summary>
    public class BpmnShapeImpl : LabeledShapeImpl, IBpmnShape
    {

        protected internal static IAttributeReference BpmnElementAttribute;//IAttributeReference<IBaseElement>
        protected internal static IAttribute/*<bool>*/ IsHorizontalAttribute;
        protected internal static IAttribute/*<bool>*/ IsExpandedAttribute;
        protected internal static IAttribute/*<bool>*/ IsMarkerVisibleAttribute;
        protected internal static IAttribute/*<bool>*/ IsMessageVisibleAttribute;
        protected internal static IAttribute/*<ParticipantBandKind>*/ ParticipantBandKindAttribute;
        protected internal static IAttributeReference ChoreographyActivityShapeAttribute;//IAttributeReference<IBpmnShape>
        protected internal static IChildElement/*<IBpmnLabel>*/ BpmnLabelChild;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnShape>(/*typeof(IBpmnShape),*/ BpmnModelConstants.BpmndiElementBpmnShape)
                .NamespaceUri(BpmnModelConstants.BpmndiNs)
                .ExtendsType(typeof(ILabeledShape))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            BpmnElementAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeBpmnElement)
                .QNameAttributeReference<IBaseElement>(/*typeof(IBaseElement)*/)
                .Build();

            IsHorizontalAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmndiAttributeIsHorizontal).Build();

            IsExpandedAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmndiAttributeIsExpanded).Build();

            IsMarkerVisibleAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmndiAttributeIsMarkerVisible).Build();

            IsMessageVisibleAttribute = typeBuilder.BooleanAttribute(BpmnModelConstants.BpmndiAttributeIsMessageVisible).Build();

            ParticipantBandKindAttribute = typeBuilder.EnumAttribute<ParticipantBandKind>(BpmnModelConstants.BpmndiAttributeParticipantBandKind/*, typeof(ParticipantBandKind)*/).Build();

            ChoreographyActivityShapeAttribute = typeBuilder
                .StringAttribute(BpmnModelConstants.BpmndiAttributeChoreographyActivityShape)
                .QNameAttributeReference<IBpmnShape>(/*typeof(IBpmnShape)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            BpmnLabelChild = sequenceBuilder.Element<IBpmnLabel>(/*typeof(IBpmnLabel)*/).Build/*<IBpmnLabel>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnShape>
        {
            public virtual IBpmnShape NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new BpmnShapeImpl(instanceContext);
            }
        }

        public BpmnShapeImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBaseElement BpmnElement
        {
            get =>  BpmnElementAttribute.GetReferenceTargetElement<IBaseElement>(this);
            set => BpmnElementAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual bool Horizontal
        {
            get => IsHorizontalAttribute.GetValue<Boolean>(this);
            set => IsHorizontalAttribute.SetValue(this, value);
        }


        public virtual bool Expanded
        {
            get => IsExpandedAttribute.GetValue<Boolean>(this);
            set => IsExpandedAttribute.SetValue(this, value);
        }


        public virtual bool MarkerVisible
        {
            get => IsMarkerVisibleAttribute.GetValue<Boolean>(this);
            set => IsMarkerVisibleAttribute.SetValue(this, value);
        }


        public virtual bool MessageVisible
        {
            get => IsMessageVisibleAttribute.GetValue<Boolean>(this);
            set => IsMessageVisibleAttribute.SetValue(this, value);
        }


        public virtual ParticipantBandKind ParticipantBandKind
        {
            get => ParticipantBandKindAttribute.GetValue<ParticipantBandKind>(this);
            set => ParticipantBandKindAttribute.SetValue(this, value);
        }


        public virtual IBpmnShape ChoreographyActivityShape
        {
            get =>  ChoreographyActivityShapeAttribute.GetReferenceTargetElement<IBpmnShape>(this);
            set => ChoreographyActivityShapeAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IBpmnLabel BpmnLabel
        {
            get => (IBpmnLabel)BpmnLabelChild.GetChild(this);
            set => BpmnLabelChild.SetChild(this, value);
        }

        //public override Model.Bpmn.instance.di.IExtension Extension
        //{
        //    get { throw new System.NotImplementedException(); }
        //    set { throw new System.NotImplementedException(); }
        //}
    }
}