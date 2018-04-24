using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class DecisionTableImpl : ExpressionImpl, IDecisionTable
    {

        protected internal static IAttribute/*<HitPolicy>*/ HitPolicyAttribute;
        protected internal static IAttribute/*<BuiltinAggregator>*/ AggregationAttribute;
        protected internal static IAttribute/*<DecisionTableOrientation>*/ PreferredOrientationAttribute;
        protected internal static IAttribute/*<string>*/ OutputLabelAttribute;

        protected internal static IChildElementCollection/*<IInput>*/ InputCollection;
        protected internal static IChildElementCollection/*<IOutput>*/ OutputCollection;
        protected internal static IChildElementCollection/*<IRule>*/ RuleCollection;

        public DecisionTableImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual HitPolicy HitPolicy
        {
            get => HitPolicyAttribute.GetValue<HitPolicy>(this);
            set => HitPolicyAttribute.SetValue(this, value);
        }


        public virtual BuiltinAggregator Aggregation
        {
            get => AggregationAttribute.GetValue<BuiltinAggregator>(this);
            set => AggregationAttribute.SetValue(this, value);
        }


        public virtual DecisionTableOrientation PreferredOrientation
        {
            get => PreferredOrientationAttribute.GetValue<DecisionTableOrientation>(this);
            set => PreferredOrientationAttribute.SetValue(this, value);
        }


        public virtual string OutputLabel
        {
            get => OutputLabelAttribute.GetValue<String>(this);
            set => OutputLabelAttribute.SetValue(this, value);
        }


        public virtual ICollection<IInput> Inputs => InputCollection.Get<IInput>(this);

        public virtual ICollection<IOutput> Outputs => OutputCollection.Get<IOutput>(this);

        public virtual ICollection<IRule> Rules => RuleCollection.Get<IRule>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDecisionTable>(/*typeof(IDecisionTable),*/ DmnModelConstants.DmnElementDecisionTable).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IExpression)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            HitPolicyAttribute = typeBuilder.NamedEnumAttribute<HitPolicy>(DmnModelConstants.DmnAttributeHitPolicy/*, typeof(HitPolicy)*/).DefaultValue(HitPolicy.Unique).Build();

            AggregationAttribute = typeBuilder.EnumAttribute<BuiltinAggregator>(DmnModelConstants.DmnAttributeAggregation/*, typeof(BuiltinAggregator)*/).Build();

            PreferredOrientationAttribute = typeBuilder.NamedEnumAttribute<DecisionTableOrientation>(DmnModelConstants.DmnAttributePreferredOrientation/*, typeof(DecisionTableOrientation)*/).DefaultValue(DecisionTableOrientation.RuleAsRow).Build();

            OutputLabelAttribute = typeBuilder.StringAttribute(DmnModelConstants.DmnAttributeOutputLabel).Build();

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            InputCollection = sequenceBuilder.ElementCollection<IInput>(/*typeof(IInput)*/).Build/*<IInput>*/();

            OutputCollection = sequenceBuilder.ElementCollection<IOutput>(/*typeof(IOutput)*/).Required().Build/*<IOutput>*/();

            RuleCollection = sequenceBuilder.ElementCollection<IRule>(/*typeof(IRule)*/).Build/*<IRule>*/();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDecisionTable>
        {
            public virtual IDecisionTable NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DecisionTableImpl(instanceContext);
            }
        }
    }
}