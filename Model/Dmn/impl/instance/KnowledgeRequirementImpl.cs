using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class KnowledgeRequirementImpl : DmnModelElementInstanceImpl, IKnowledgeRequirement
    {

        protected internal static IElementReference RequiredKnowledgeRef;//IElementReference<IBusinessKnowledgeModel, IRequiredKnowledgeReference>

        public KnowledgeRequirementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
        {
        }

        public virtual IBusinessKnowledgeModel RequiredKnowledge
        {
            get => (IBusinessKnowledgeModel)RequiredKnowledgeRef.GetReferenceTargetElement(this);
            set => RequiredKnowledgeRef.SetReferenceTargetElement(this, value);
        }


        public new static void RegisterType(ModelBuilder modelBuilder)
        {
            IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IKnowledgeRequirement>(/*typeof(IKnowledgeRequirement),*/ DmnModelConstants.DmnElementKnowledgeRequirement)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

            ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

            RequiredKnowledgeRef = sequenceBuilder
                .Element<IRequiredKnowledgeReference>(/*typeof(IRequiredKnowledgeReference)*/)
                .Required()
                .UriElementReference<IBusinessKnowledgeModel>(/*typeof(IBusinessKnowledgeModel)*/)
                .Build();

            typeBuilder.Build();
        }

        private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IKnowledgeRequirement>
        {
            public virtual IKnowledgeRequirement NewInstance(ModelTypeInstanceContext instanceContext)
            {
                return new KnowledgeRequirementImpl(instanceContext);
            }
        }
    }
}