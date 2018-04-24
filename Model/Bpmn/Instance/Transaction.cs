
namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// 
	/// 
	/// </summary>
	public interface ITransaction : ISubProcess
	{

	  TransactionMethod Method {get;set;}

    }

}