

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class InformationRequirementImpl : DmnModelElementInstanceImpl, INformationRequirement
    {

        protected internal static IElementReference RequiredDecisionRef;//IElementReference<IDecision, IRequiredDecisionReference>
        protected internal static IElementReference RequiredInputRef;//IElementReference<INputData, IRequiredInputReference>

        public InformationRequirementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IDecision RequiredDecision
        {
            get => (IDecision)RequiredDecisionRef.GetReferenceTargetElement(this);
            set => RequiredDecisionRef.SetReferenceTargetElement(this, value);
        }


        public virtual INputData RequiredInput
        {
            get => (INputData)RequiredInputRef.GetReferenceTargetElement(this);
            set => RequiredInputRef.SetReferenceTargetElement(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<INformationRequirement>(/*typeof(INformationRequirement),*/ DmnModelConstants.DmnElementInformationRequirement)
                    .NamespaceUri(DmnModelConstants.Dmn11Ns)
                    .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            RequiredDecisionRef = sequenceBuilder
                .Element<IRequiredDecisionReference>(/*typeof(IRequiredDecisionReference)*/)
                .UriElementReference<IDecision>(/*typeof(IDecision)*/)
                .Build();

            RequiredInputRef = sequenceBuilder
                .Element<IRequiredInputReference>(/*typeof(IRequiredInputReference)*/)
                .UriElementReference<INputData>(/*typeof(INputData)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<INformationRequirement>
        {
            public virtual INformationRequirement NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new InformationRequirementImpl(instanceContext);
            }
        }

    }

}