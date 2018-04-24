

namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	/// <summary>
	/// The BPMN ioBinding element
	/// 
	/// 
	/// </summary>
	public interface IOBinding : IBaseElement
	{

	  IOperation Operation {get;set;}


	  IDataInput InputData {get;set;}


	  IDataOutput OutputData {get;set;}

	}

}