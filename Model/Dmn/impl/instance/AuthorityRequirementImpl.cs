using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using ESS.FW.Bpm.Model.Xml.type.reference;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class AuthorityRequirementImpl : DmnModelElementInstanceImpl, IAuthorityRequirement
	{

	  protected internal static IElementReference RequiredDecisionRef;//IElementReference<IDecision, IRequiredDecisionReference>
        protected internal static IElementReference RequiredInputRef;//IElementReference<INputData, IRequiredInputReference>
        protected internal static IElementReference RequiredAuthorityRef;//IElementReference<IKnowledgeSource, IRequiredAuthorityReference>

        public AuthorityRequirementImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
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


	  public virtual IKnowledgeSource RequiredAuthority
	  {
		  get => (IKnowledgeSource)RequiredAuthorityRef.GetReferenceTargetElement(this);
	      set => RequiredAuthorityRef.SetReferenceTargetElement(this, value);
	  }


	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAuthorityRequirement>(/*typeof(IAuthorityRequirement),*/ DmnModelConstants.DmnElementAuthorityRequirement)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		RequiredDecisionRef = sequenceBuilder.Element<IRequiredDecisionReference>(/*typeof(IRequiredDecisionReference)*/).UriElementReference<IDecision>(/*typeof(IDecision)*/).Build();

		RequiredInputRef = sequenceBuilder.Element<IRequiredInputReference>(/*typeof(IRequiredInputReference)*/).UriElementReference<INputData>(/*typeof(INputData)*/).Build();

		RequiredAuthorityRef = sequenceBuilder.Element<IRequiredAuthorityReference>(/*typeof(IRequiredAuthorityReference)*/).UriElementReference<IKnowledgeSource>(/*typeof(IKnowledgeSource)*/).Build();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAuthorityRequirement>
	  {
		  public virtual IAuthorityRequirement NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new AuthorityRequirementImpl(instanceContext);
		  }
	  }

	}

}