using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    public class OrganizationUnitImpl : BusinessContextElementImpl, IOrganizationUnit
    {

        protected internal static IElementReferenceCollection DecisionDecisionMadeRefCollection;//IElementReferenceCollection<IDecision, IDecisionMadeReference>
        protected internal static IElementReferenceCollection DecisionDecisionOwnedRefCollection;//IElementReferenceCollection<IDecision, IDecisionOwnedReference>

        public OrganizationUnitImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDecision> DecisionsMade => DecisionDecisionMadeRefCollection.GetReferenceTargetElements<IDecision>(this);

        public virtual ICollection<IDecision> DecisionsOwned => DecisionDecisionOwnedRefCollection.GetReferenceTargetElements<IDecision>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IOrganizationUnit>(/*typeof(IOrganizationUnit),*/ DmnModelConstants.DmnElementOrganizationUnit)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IBusinessContextElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            DecisionDecisionMadeRefCollection = sequenceBuilder
                .ElementCollection<IDecisionMadeReference>(/*typeof(IDecisionMadeReference)*/)
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            DecisionDecisionOwnedRefCollection = sequenceBuilder
                .ElementCollection<IDecisionOwnedReference>(/*typeof(IDecisionOwnedReference)*/)
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IOrganizationUnit>
        {
            public virtual IOrganizationUnit NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new OrganizationUnitImpl(instanceContext);
            }
        }

    }

}