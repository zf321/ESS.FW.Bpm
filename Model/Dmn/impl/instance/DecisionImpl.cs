using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class DecisionImpl : DrgElementImpl, IDecision
    {

        protected internal static IChildElement/*<IQuestion>*/ QuestionChild;
        protected internal static IChildElement/*<IAllowedAnswers>*/ AllowedAnswersChild;
        protected internal static IChildElement/*<IVariable>*/ VariableChild;
        protected internal static IChildElementCollection/*<INformationRequirement>*/ InformationRequirementCollection;
        protected internal static IChildElementCollection/*<IKnowledgeRequirement>*/ KnowledgeRequirementCollection;
        protected internal static IChildElementCollection/*<IAuthorityRequirement>*/ AuthorityRequirementCollection;
        protected internal static IChildElementCollection/*<ISupportedObjectiveReference>*/ SupportedObjectiveChildElementCollection;
        protected internal static IElementReferenceCollection ImpactedPerformanceIndicatorRefCollection;//IElementReferenceCollection<IPerformanceIndicator, IMpactedPerformanceIndicatorReference>
        protected internal static IElementReferenceCollection DecisionMakerRefCollection;//IElementReferenceCollection<IOrganizationUnit, IDecisionMakerReference>
        protected internal static IElementReferenceCollection DecisionOwnerRefCollection;//IElementReferenceCollection<IOrganizationUnit, IDecisionOwnerReference>
        protected internal static IChildElementCollection/*<IUsingProcessReference>*/ UsingProcessCollection;
        protected internal static IChildElementCollection/*<IUsingTaskReference>*/ UsingTaskCollection;
        protected internal static IChildElement/*<IExpression>*/ ExpressionChild;

        public DecisionImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IQuestion Question
        {
            get => (IQuestion)QuestionChild.GetChild(this);
            set => QuestionChild.SetChild(this, value);
        }


        public virtual IAllowedAnswers AllowedAnswers
        {
            get => (IAllowedAnswers)AllowedAnswersChild.GetChild(this);
            set => AllowedAnswersChild.SetChild(this, value);
        }


        public virtual IVariable Variable
        {
            get => (IVariable)VariableChild.GetChild(this);
            set => VariableChild.SetChild(this, value);
        }


        public virtual ICollection<INformationRequirement> InformationRequirements => InformationRequirementCollection.Get<INformationRequirement>(this);

        public virtual ICollection<IKnowledgeRequirement> KnowledgeRequirements => KnowledgeRequirementCollection.Get<IKnowledgeRequirement>(this);

        public virtual ICollection<IAuthorityRequirement> AuthorityRequirements => AuthorityRequirementCollection.Get<IAuthorityRequirement>(this);

        public virtual ICollection<ISupportedObjectiveReference> SupportedObjectiveReferences => SupportedObjectiveChildElementCollection.Get<ISupportedObjectiveReference>(this);

        public virtual ICollection<IPerformanceIndicator> ImpactedPerformanceIndicators => ImpactedPerformanceIndicatorRefCollection.GetReferenceTargetElements<IPerformanceIndicator>(this);

        public virtual ICollection<IOrganizationUnit> DecisionMakers => DecisionMakerRefCollection.GetReferenceTargetElements<IOrganizationUnit>(this);

        public virtual ICollection<IOrganizationUnit> DecisionOwners => DecisionOwnerRefCollection.GetReferenceTargetElements<IOrganizationUnit>(this);

        public virtual ICollection<IUsingProcessReference> UsingProcessReferences => UsingProcessCollection.Get<IUsingProcessReference>(this);

        public virtual ICollection<IUsingTaskReference> UsingTaskReferences => UsingTaskCollection.Get<IUsingTaskReference>(this);

        public virtual IExpression Expression
        {
            get => (IExpression)ExpressionChild.GetChild(this);
            set => ExpressionChild.SetChild(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDecision>(/*typeof(IDecision),*/ DmnModelConstants.DmnElementDecision)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IDrgElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            QuestionChild = sequenceBuilder.Element<IQuestion>(/*typeof(IQuestion)*/).Build/*<IQuestion>*/();

            AllowedAnswersChild = sequenceBuilder.Element<IAllowedAnswers>(/*typeof(IAllowedAnswers)*/).Build/*<IAllowedAnswers>*/();

            VariableChild = sequenceBuilder.Element<IVariable>(/*typeof(IVariable)*/).Build/*<IVariable>*/();

            InformationRequirementCollection = sequenceBuilder.ElementCollection<INformationRequirement>(/*typeof(INformationRequirement)*/).Build/*<INformationRequirement>*/();

            KnowledgeRequirementCollection = sequenceBuilder.ElementCollection<IKnowledgeRequirement>(/*typeof(IKnowledgeRequirement)*/).Build/*<IKnowledgeRequirement>*/();

            AuthorityRequirementCollection = sequenceBuilder.ElementCollection<IAuthorityRequirement>(/*typeof(IAuthorityRequirement)*/).Build/*<IAuthorityRequirement>*/();

            SupportedObjectiveChildElementCollection = sequenceBuilder.ElementCollection<ISupportedObjectiveReference>(/*typeof(ISupportedObjectiveReference)*/).Build/*<ISupportedObjectiveReference>*/();

            ImpactedPerformanceIndicatorRefCollection = sequenceBuilder
                .ElementCollection<IMpactedPerformanceIndicatorReference>(/*typeof(IMpactedPerformanceIndicatorReference)*/)
                .UriElementReferenceCollection<IPerformanceIndicator>(/*typeof(IPerformanceIndicator)*/)
                .Build();

            DecisionMakerRefCollection = sequenceBuilder
                .ElementCollection<IDecisionMakerReference>(/*typeof(IDecisionMakerReference)*/)
                .UriElementReferenceCollection<IOrganizationUnit>(/*typeof(IOrganizationUnit)*/)
                .Build();

            DecisionOwnerRefCollection = sequenceBuilder
                .ElementCollection<IDecisionOwnerReference>(/*typeof(IDecisionOwnerReference)*/)
                .UriElementReferenceCollection<IOrganizationUnit>(/*typeof(IOrganizationUnit)*/)
                .Build();

            UsingProcessCollection = sequenceBuilder
                .ElementCollection<IUsingProcessReference>(/*typeof(IUsingProcessReference)*/)
                .Build/*<IUsingProcessReference>*/();

            UsingTaskCollection = sequenceBuilder
                .ElementCollection<IUsingTaskReference>(/*typeof(IUsingTaskReference)*/)
                .Build/*<IUsingTaskReference>*/();

            ExpressionChild = sequenceBuilder
                .Element<IExpression>(/*typeof(IExpression)*/)
                .Build/*<IExpression>*/();

            typeBuilder.Build();
        }

        public int CamundaHistoryTimeToLive { get; set; }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDecision>
        {
            public virtual IDecision NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DecisionImpl(instanceContext);
            }
        }
    }
}