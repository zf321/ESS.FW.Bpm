using System;
using ESS.FW.Bpm.Model.Bpmn.instance.camunda;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.camunda
{
    public class CamundaValueImpl : BpmnModelElementInstanceImpl, ICamundaValue
	{

	  protected internal static IAttribute/*<string>*/ CamundaIdAttribute;
	  protected internal static IAttribute/*<string>*/ CamundaNameAttribute;

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<ICamundaValue>(/*typeof(ICamundaValue),*/ BpmnModelConstants.CamundaElementValue)
                .NamespaceUri(BpmnModelConstants.CamundaNs)
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		CamundaIdAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeId).Namespace(BpmnModelConstants.CamundaNs).Build();

		CamundaNameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.CamundaAttributeName).Namespace(BpmnModelConstants.CamundaNs).Build();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<ICamundaValue>
	  {
		  public virtual ICamundaValue NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new CamundaValueImpl(instanceContext);
		  }
	  }

	  public CamundaValueImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string CamundaId
	  {
		  get { return CamundaIdAttribute.GetValue<String>(this); }
		  set { CamundaIdAttribute.SetValue(this, value); }
	  }


	  public virtual string CamundaName
	  {
		  get { return CamundaNameAttribute.GetValue<String>(this); }
		  set { CamundaNameAttribute.SetValue(this, value); }
	  }

	}

}