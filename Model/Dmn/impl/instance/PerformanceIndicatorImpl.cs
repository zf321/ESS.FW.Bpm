using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

    public class PerformanceIndicatorImpl : BusinessContextElementImpl, IPerformanceIndicator
    {

        protected internal static IElementReferenceCollection ImpactingDecisionRefCollection;//IElementReferenceCollection<IDecision, IMpactingDecisionReference>

        public PerformanceIndicatorImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual ICollection<IDecision> ImpactingDecisions => ImpactingDecisionRefCollection.GetReferenceTargetElements<IDecision>(this);

        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IPerformanceIndicator>(/*typeof(IPerformanceIndicator),*/ DmnModelConstants.DmnElementPerformanceIndicator)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IBusinessContextElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            ImpactingDecisionRefCollection = sequenceBuilder
                .ElementCollection<IMpactingDecisionReference>(/*typeof(IMpactingDecisionReference)*/)
                .UriElementReferenceCollection<IDecision>(/*typeof(IDecision)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IPerformanceIndicator>
        {
            public virtual IPerformanceIndicator NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new PerformanceIndicatorImpl(instanceContext);
            }
        }

    }

}