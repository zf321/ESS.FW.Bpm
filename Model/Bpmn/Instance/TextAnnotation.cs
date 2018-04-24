
namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN 2.0 textAnnotation element
	/// 
	/// @author Filip Hrisafov
	/// </summary>
	public interface ITextAnnotation : IArtifact
	{

	  string TextFormat {get;set;}


	  IText Text {get;set;}


	}

}