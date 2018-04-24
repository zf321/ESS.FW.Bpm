

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Bpmn.impl.instance
{
    /// <summary>
	/// The BPMN artifact element
	/// 
	/// 
	/// </summary>
	public abstract class ArtifactImpl : BaseElementImpl, IArtifact
	{

	  public new static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IArtifact>(/*typeof(IArtifact),*/BpmnModelConstants.BpmnElementArtifact)
                .NamespaceUri(BpmnModelConstants.Bpmn20Ns)
                .ExtendsType(typeof(IBaseElement))
                .AbstractType();
		typeBuilder.Build();
	  }

	  public ArtifactImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }
	}

}