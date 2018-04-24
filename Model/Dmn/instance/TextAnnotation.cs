

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface ITextAnnotation : IArtifact
	{

	  string TextFormat {get;set;}


	  IText Text {get;set;}


	}

}