using System.Collections.Generic;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;



namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    
    public class BusinessKnowledgeModelImpl : DrgElementImpl, IBusinessKnowledgeModel
	{

	  protected internal static IChildElement/*<IEncapsulatedLogic>*/ EncapsulatedLogicChild;
	  protected internal static IChildElement/*<IVariable>*/ VariableChild;
	  protected internal static IChildElementCollection/*<IKnowledgeRequirement>*/ KnowledgeRequirementCollection;
	  protected internal static IChildElementCollection/*<IAuthorityRequirement>*/ AuthorityRequirementCollection;

	  public BusinessKnowledgeModelImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual IEncapsulatedLogic EncapsulatedLogic
	  {
		  get => (IEncapsulatedLogic)EncapsulatedLogicChild.GetChild(this);
	      set => EncapsulatedLogicChild.SetChild(this, value);
	  }


	  public virtual IVariable Variable
	  {
		  get => (IVariable)VariableChild.GetChild(this);
	      set => VariableChild.SetChild(this, value);
	  }


	  public virtual ICollection<IKnowledgeRequirement> KnowledgeRequirement => KnowledgeRequirementCollection.Get<IKnowledgeRequirement>(this);

	    public virtual ICollection<IAuthorityRequirement> AuthorityRequirement => AuthorityRequirementCollection.Get<IAuthorityRequirement>(this);

	    public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBusinessKnowledgeModel>(/*typeof(IBusinessKnowledgeModel),*/ DmnModelConstants.DmnElementBusinessKnowledgeModel).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDrgElement)).InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		EncapsulatedLogicChild = sequenceBuilder.Element<IEncapsulatedLogic>(/*typeof(IEncapsulatedLogic)*/).Build/*<IEncapsulatedLogic>*/();

		VariableChild = sequenceBuilder.Element<IVariable>(/*typeof(IVariable)*/).Build/*<IVariable>*/();

		KnowledgeRequirementCollection = sequenceBuilder.ElementCollection<IKnowledgeRequirement>(/*typeof(IKnowledgeRequirement)*/).Build/*<IKnowledgeRequirement>*/();

		AuthorityRequirementCollection = sequenceBuilder.ElementCollection<IAuthorityRequirement>(/*typeof(IAuthorityRequirement)*/).Build/*<IAuthorityRequirement>*/();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBusinessKnowledgeModel>
	  {
		  public virtual IBusinessKnowledgeModel NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BusinessKnowledgeModelImpl(instanceContext);
		  }
	  }

	}

}