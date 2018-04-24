using System;
using ESS.FW.Bpm.Model.Bpmn.instance;


namespace ESS.FW.Bpm.Model.Bpmn.builder
{
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class AbstractTransactionBuilder : AbstractSubProcessBuilder
	{

	  protected internal AbstractTransactionBuilder(IBpmnModelInstance modelInstance, ITransaction element) 
            : base(modelInstance, element)
	  {
	  }

	  public virtual AbstractTransactionBuilder Method(TransactionMethod method)
	  {
            ((ITransaction)element).Method = method;
            return this;
	  }


	}

}