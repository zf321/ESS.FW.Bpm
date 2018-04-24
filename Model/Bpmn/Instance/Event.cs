using System.Collections.Generic;


namespace ESS.FW.Bpm.Model.Bpmn.instance
{
    /// <summary>
	/// The BPMN event element
	/// 
	/// 
	/// 
	/// </summary>
	public interface IEvent : IFlowNode, IInteractionNode
    {

        ICollection<IProperty> Properties { get; }

        //IBpmnShape DiagramElement {get;}
    }

}