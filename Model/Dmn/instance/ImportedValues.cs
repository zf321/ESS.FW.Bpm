

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface IMportedValues : IMport
	{

	  string ExpressionLanguage {get;set;}


	  IMportedElement ImportedElement {get;set;}


	}

}