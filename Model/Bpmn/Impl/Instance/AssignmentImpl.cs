

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{   
    /// <summary>
    /// The BPMN assignment element
    /// 
    /// 
    /// </summary>
    public class AssignmentImpl : BaseElementImpl, IAssignment
	{

	  protected internal static IChildElement/*<From>*/ FromChild;
	  protected internal static IChildElement/*<To>*/ ToChild;

	  public new  static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IAssignment>(/*typeof(IAssignment),*/ BpmnModelConstants.BpmnElementAssignment)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		FromChild = sequenceBuilder.Element<From>(/*typeof(From)*/).Required().Build/*<From>*/();

		ToChild = sequenceBuilder.Element<To>(/*typeof(To)*/).Required().Build/*<To>*/();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IAssignment>
	  {
		  public virtual IAssignment NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new AssignmentImpl(instanceContext);
		  }
	  }

	  public AssignmentImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual From From
	  {
		  get => (From)FromChild.GetChild(this);
	      set => FromChild.SetChild(this, value);
	  }


	  public virtual To To
	  {
		  get => (To)ToChild.GetChild(this);
	      set => ToChild.SetChild(this, value);
	  }

	}

}