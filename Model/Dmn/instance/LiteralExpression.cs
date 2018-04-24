

namespace ESS.FW.Bpm.Model.Dmn.instance
{

	public interface ILiteralExpression : IExpression
	{

	  string ExpressionLanguage {get;set;}


	  IText Text {get;set;}


	  IMportedValues ImportValues {get;set;}


	}

}