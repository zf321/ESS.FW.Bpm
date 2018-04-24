

using System;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.attribute;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    public class PropertyImpl : ItemAwareElementImpl, IProperty
	{

	  protected internal static IAttribute/*<string>*/ NameAttribute;

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IProperty>(/*typeof(IProperty),*/ BpmnModelConstants.BpmnElementProperty)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IItemAwareElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		NameAttribute = typeBuilder.StringAttribute(BpmnModelConstants.BpmnAttributeName).Build();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IProperty>
	  {
		  public virtual IProperty NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new PropertyImpl(instanceContext);
		  }
	  }

	  public PropertyImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual string Name
	  {
		  get { return NameAttribute.GetValue<String>(this); }
		  set { NameAttribute.SetValue(this, value); }
	  }


	}

}