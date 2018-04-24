

using System;
using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    public class InputClauseImpl : DmnElementImpl, IInputClause
	{

	  protected internal static IChildElement/*<IInputExpression>*/ InputExpressionChild;
	  protected internal static IChildElement/*<INputValues>*/ InputValuesChild;

	  // camunda extensions
	  protected internal static IAttribute/*<string>*/ CamundaInputVariableAttribute;

	  public InputClauseImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual IInputExpression InputExpression
	  {
		  get => (IInputExpression)InputExpressionChild.GetChild(this);
	      set => InputExpressionChild.SetChild(this, value);
	  }


	  public virtual INputValues InputValues
	  {
		  get => (INputValues)InputValuesChild.GetChild(this);
	      set => InputValuesChild.SetChild(this, value);
	  }


	  // camunda extensions

	  public virtual string CamundaInputVariable
	  {
		  get { return CamundaInputVariableAttribute.GetValue<String>(this); }
		  set { CamundaInputVariableAttribute.SetValue(this, value); }
	  }



	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IInputClause>(/*typeof(IInputClause),*/ DmnModelConstants.DmnElementInputClause)
                .NamespaceUri(DmnModelConstants.Dmn11Ns)
                .ExtendsType(typeof(IDmnElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		InputExpressionChild = sequenceBuilder.Element<IInputExpression>(/*typeof(IInputExpression)*/).Required().Build/*<IInputExpression>*/();

		InputValuesChild = sequenceBuilder.Element<INputValues>(/*typeof(INputValues)*/).Build/*<INputValues>*/();

		// camunda extensions

		CamundaInputVariableAttribute = typeBuilder.StringAttribute(DmnModelConstants.CamundaAttributeInputVariable).Namespace(DmnModelConstants.CamundaNs).Build();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IInputClause>
	  {
		  public virtual IInputClause NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new InputClauseImpl(instanceContext);
		  }
	  }

	}

}