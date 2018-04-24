

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl.instance;
using ESS.FW.Bpm.Model.Xml.type;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN11_NS;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.camunda.bpm.model.dmn.impl.DmnModelConstants.DMN_ELEMENT_ARTIFACT;

    using ModelBuilder = ModelBuilder;
    using ModelTypeInstanceContext = ModelTypeInstanceContext;

    public abstract class ArtifactImpl : DmnElementImpl, IArtifact
	{

	  public ArtifactImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	  public static void RegisterType(ModelBuilder modelBuilder)
	  {
		IModelElementTypeBuilder typeBuilder = modelBuilder.DefineType<IArtifact>(/*typeof(IArtifact),*/ DmnModelConstants.DmnElementArtifact).NamespaceUri(DmnModelConstants.Dmn11Ns).ExtendsType(typeof(IDmnElement)).AbstractType();

		typeBuilder.Build();
	  }

	}

}