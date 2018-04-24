using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.instance.bpmndi;
using ESS.FW.Bpm.Model.Bpmn.instance.di;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;
using ESS.FW.Bpm.Model.Xml.type.child;
using DiagramImpl = ESS.FW.Bpm.Model.Bpmn.impl.instance.di.DiagramImpl;



namespace ESS.FW.Bpm.Model.Bpmn.impl.instance.bpmndi
{
    

    /// <summary>
    /// The BPMNDI BPMNDiagram element
    /// 
    /// 
    /// </summary>
    public class BpmnDiagramImpl : DiagramImpl, IBpmnDiagram
	{

	  protected internal static IChildElement/*<IBpmnPlane>*/ BpmnPlaneChild;
	  protected internal static IChildElementCollection/*<IBpmnLabelStyle>*/ BpmnLabelStyleCollection;

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IBpmnDiagram>(/*typeof(IBpmnDiagram), */BpmnModelConstants.BpmndiElementBpmnDiagram)
                .NamespaceUri(BpmnModelConstants.BpmndiNs)
                .ExtendsType(typeof(IDiagram))
                .InstanceProvider(new ModelTypeInstanceProviderAnonymousInnerClass());

		ISequenceBuilder sequenceBuilder = typeBuilder.Sequence();

		BpmnPlaneChild = sequenceBuilder.Element<IBpmnPlane>(/*typeof(IBpmnPlane)*/).Required().Build/*<IBpmnPlane>*/();

		BpmnLabelStyleCollection = sequenceBuilder.ElementCollection<IBpmnLabelStyle>(/*typeof(IBpmnLabelStyle)*/).Build/*<IBpmnLabelStyle>*/();

		typeBuilder.Build();
	  }

	  private class ModelTypeInstanceProviderAnonymousInnerClass : IModelTypeInstanceProvider<IBpmnDiagram>
	  {
		  public virtual IBpmnDiagram NewInstance(ModelTypeInstanceContext instanceContext)
		  {
			return new BpmnDiagramImpl(instanceContext);
		  }
	  }

	  public BpmnDiagramImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public virtual IBpmnPlane BpmnPlane
	  {
		  get => (IBpmnPlane)BpmnPlaneChild.GetChild(this);
	      set => BpmnPlaneChild.SetChild(this, value);
	  }


	  public virtual ICollection<IBpmnLabelStyle> BpmnLabelStyles => BpmnLabelStyleCollection.Get<IBpmnLabelStyle>(this);
	}

}