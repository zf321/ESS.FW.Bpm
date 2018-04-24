
using ESS.FW.Bpm.Model.Bpmn.instance;

namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class TransactionBuilder : AbstractTransactionBuilder//<TransactionBuilder>
	{

	  protected internal TransactionBuilder(IBpmnModelInstance modelInstance, ITransaction element) : base(modelInstance, element)
	  {
	  }

	}

}