using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class DecisionServiceImpl : NamedElementImpl, IDecisionService
    {

        protected internal static IElementReferenceCollection OutputDecisionRefCollection;//IElementReferenceCollection<IDecision, IOutputDecisionReference>
        protected internal static IElementReferenceCollection EncapsulatedDecisionRefCollection;//IElementReferenceCollection<IDecision, IEncapsulatedDecisionReference>
        protected internal static IElementReferenceCollection InputDecisionRefCollection;//IElementReferenceCollection<IDecision, INputDecisionReference>
        protected internal static IElementReferenceCollection InputDataRefCollection;//IElementReferenceCollection<INputData, INputDataReference>

        public DecisionServiceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDecision> OutputDecisions => OutputDecisionRefCollection.GetReferenceTargetElements<IDecision>(this);

        public virtual ICollection<IDecision> EncapsulatedDecisions => EncapsulatedDecisionRefCollection.GetReferenceTargetElements<IDecision>(this);

        public virtual ICollection<IDecision> InputDecisions => InputDecisionRefCollection.GetReferenceTargetElements<IDecision>(this);

        public virtual ICollection<INputData> InputData => InputDataRefCollection.GetReferenceTargetElements<INputData>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IDecisionService>(/*typeof(IDecisionService),*/ DmnModelConstants.DmnElementDecisionService)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns)
                    .ExtendsType(typeof(INamedElement))
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            OutputDecisionRefCollection = sequenceBuilder
                .ElementCollection<IOutputDecisionReference>(/*typeof(IOutputDecisionReference)*/)
                .Required()
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            EncapsulatedDecisionRefCollection = sequenceBuilder
                .ElementCollection<IEncapsulatedDecisionReference>(/*typeof(IEncapsulatedDecisionReference)*/)
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            InputDecisionRefCollection = sequenceBuilder
                .ElementCollection<INputDecisionReference>(/*typeof(INputDecisionReference)*/)
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            InputDataRefCollection = sequenceBuilder
                .ElementCollection<INputDataReference>(/*typeof(INputDataReference)*/)
                .UriElementReferenceCollection<INputData>(/*typeof(INputData)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IDecisionService>
        {
            public virtual IDecisionService NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new DecisionServiceImpl(instanceContext);
            }
        }

    }

}