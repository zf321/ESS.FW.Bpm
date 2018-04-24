using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.builder;
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

    public class MultiInstanceLoopCharacteristicsImpl : LoopCharacteristicsImpl, IMultiInstanceLoopCharacteristics
    {

        protected internal static IAttribute/*<bool>*/ IsSequentialAttribute;
        protected internal static IAttribute/*<MultiInstanceFlowCondition>*/ BehaviorAttribute;
        protected internal static IAttributeReference OneBehaviorEventRefAttribute;//IAttributeReference<IEventDefinition>
        protected internal static IAttributeReference NoneBehaviorEventRefAttribute;//IAttributeReference<IEventDefinition>
        protected internal static IChildElement/*<ILoopCardinality>*/ LoopCardinalityChild;
        protected internal static IElementReference LoopDataInputRefChild;//IElementReference<IDataInput, LoopDataInputRef>
        protected internal static IElementReference LoopDataOutputRefChild;//IElementReference<IDataOutput, LoopDataOutputRef>
        protected internal static IChildElement/*<INputDataItem>*/ InputDataItemChild;
        protected internal static IChildElement/*<IOutputDataItem>*/ OutputDataItemChild;
        protected internal static IChildElementCollection/*<IComplexBehaviorDefinition>*/ ComplexBehaviorDefinitionCollection;
        protected internal static IChildElement/*<ICompletionCondition>*/ CompletionConditionChild;
        protected internal static IAttribute/*<bool>*/ camundaAsyncAfter;
        protected internal static IAttribute/*<bool>*/ camundaAsyncBefore;
        protected internal static IAttribute/*<bool>*/ camundaExclusive;
        protected internal static IAttribute/*<string>*/ camundaCollection;
        protected internal static IAttribute/*<string>*/ camundaElementVariable;

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IMultiInstanceLoopCharacteristics>(/*typeof(IMultiInstanceLoopCharacteristics),*/ BpmnModelConstants.BpmnElementMultiInstanceLoopCharacteristics)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(ILoopCharacteristics))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            IsSequentialAttribute = typeBuilder
                .BooleanAttribute(BpmnModelConstants.BpmnElementIsSequential)
                .DefaultValue(false)
                .Build();

            BehaviorAttribute = typeBuilder
                .EnumAttribute<MultiInstanceFlowCondition>(BpmnModelConstants.BpmnElementBehavior/*, typeof(MultiInstanceFlowCondition)*/)
                .DefaultValue(MultiInstanceFlowCondition.All)
                .Build();

            OneBehaviorEventRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnElementOneBehaviorEventRef)
                .QNameAttributeReference<IEventDefinition>(/*typeof(IEventDefinition)*/)
                .Build();

            NoneBehaviorEventRefAttribute =typeBuilder
                .StringAttribute(BpmnModelConstants.BpmnElementNoneBehaviorEventRef)
                .QNameAttributeReference<IEventDefinition>(/*typeof(IEventDefinition)*/)
                .Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            LoopCardinalityChild = sequenceBuilder
                .Element<ILoopCardinality>(/*typeof(ILoopCardinality)*/)
                .Build/*<ILoopCardinality>*/();

            LoopDataInputRefChild = sequenceBuilder
                .Element<LoopDataInputRef>(/*typeof(LoopDataInputRef)*/)
                .QNameElementReference<IDataInput>(/*typeof(IDataInput)*/)
                .Build();

            LoopDataOutputRefChild = sequenceBuilder
                .Element<LoopDataOutputRef>(/*typeof(LoopDataOutputRef)*/)
                .QNameElementReference<IDataOutput>(/*typeof(IDataOutput)*/)
                .Build();

            OutputDataItemChild = sequenceBuilder
                .Element<IOutputDataItem>(/*typeof(IOutputDataItem)*/)
                .Build/*<IOutputDataItem>*/();

            InputDataItemChild = sequenceBuilder
                .Element<INputDataItem>(/*typeof(INputDataItem)*/)
                .Build/*<INputDataItem>*/();

            ComplexBehaviorDefinitionCollection = sequenceBuilder
                .ElementCollection<IComplexBehaviorDefinition>(/*typeof(IComplexBehaviorDefinition)*/)
                .Build/*<IComplexBehaviorDefinition>*/();

            CompletionConditionChild = sequenceBuilder
                .Element<ICompletionCondition>(/*typeof(ICompletionCondition)*/)
                .Build/*<ICompletionCondition>*/();

            camundaAsyncAfter = typeBuilder
                .BooleanAttribute(BpmnModelConstants.CamundaAttributeAsyncAfter)
                .Namespace(BpmnModelConstants.CamundaNs).DefaultValue(false)
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

            camundaCollection = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeCollection)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            camundaElementVariable = typeBuilder
                .StringAttribute(BpmnModelConstants.CamundaAttributeElementVariable)
                .Namespace(BpmnModelConstants.CamundaNs)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IMultiInstanceLoopCharacteristics>
        {
            public virtual IMultiInstanceLoopCharacteristics NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new MultiInstanceLoopCharacteristicsImpl(instanceContext);
            }
        }

        public MultiInstanceLoopCharacteristicsImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public MultiInstanceLoopCharacteristicsBuilder Builder()
        {
            return new MultiInstanceLoopCharacteristicsBuilder((IBpmnModelInstance)modelInstance, this);
        }

        public virtual ILoopCardinality LoopCardinality
        {
            get => (ILoopCardinality)LoopCardinalityChild.GetChild(this);
            set => LoopCardinalityChild.SetChild(this, value);
        }


        public virtual IDataInput LoopDataInputRef
        {
            get => (IDataInput)LoopDataInputRefChild.GetReferenceTargetElement(this);
            set => LoopDataInputRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual IDataOutput LoopDataOutputRef
        {
            get => (IDataOutput)LoopDataOutputRefChild.GetReferenceTargetElement(this);
            set => LoopDataOutputRefChild.SetReferenceTargetElement(this, value);
        }


        public virtual INputDataItem InputDataItem
        {
            get => (INputDataItem)InputDataItemChild.GetChild(this);
            set => InputDataItemChild.SetChild(this, value);
        }


        public virtual IOutputDataItem OutputDataItem
        {
            get => (IOutputDataItem)OutputDataItemChild.GetChild(this);
            set => OutputDataItemChild.SetChild(this, value);
        }


        public virtual ICollection<IComplexBehaviorDefinition> ComplexBehaviorDefinitions => ComplexBehaviorDefinitionCollection.Get<IComplexBehaviorDefinition>(this);

        public virtual ICompletionCondition CompletionCondition
        {
            get => (ICompletionCondition)CompletionConditionChild.GetChild(this);
            set => CompletionConditionChild.SetChild(this, value);
        }


        public virtual bool Sequential
        {
            get => IsSequentialAttribute.GetValue<Boolean>(this);
            set => IsSequentialAttribute.SetValue(this, value);
        }


        public virtual MultiInstanceFlowCondition Behavior
        {
            get => BehaviorAttribute.GetValue<MultiInstanceFlowCondition>(this);
            set => BehaviorAttribute.SetValue(this, value);
        }


        public virtual IEventDefinition OneBehaviorEventRef
        {
            get => OneBehaviorEventRefAttribute.GetReferenceTargetElement<IEventDefinition>(this);
            set => OneBehaviorEventRefAttribute.SetReferenceTargetElement(this, value);
        }


        public virtual IEventDefinition NoneBehaviorEventRef
        {
            get => NoneBehaviorEventRefAttribute.GetReferenceTargetElement<IEventDefinition>(this);
            set => NoneBehaviorEventRefAttribute.SetReferenceTargetElement(this, value);
        }


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


        public virtual string CamundaCollection
        {
            get => camundaCollection.GetValue<String>(this);
            set => camundaCollection.SetValue(this, value);
        }


        public virtual string CamundaElementVariable
        {
            get => camundaElementVariable.GetValue<String>(this);
            set => camundaElementVariable.SetValue(this, value);
        }
    }
}