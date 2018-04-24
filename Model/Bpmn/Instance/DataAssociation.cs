using System.Collections.Generic;
using ESS.FW.Bpm.Model.Bpmn.impl.instance;



namespace ESS.FW.Bpm.Model.Bpmn.instance
{

	using Transformation = Transformation;

    /// <summary>
	/// The BPMN dataAssociation element
	/// 
	/// 
	/// </summary>
	public interface IDataAssociation : IBaseElement
    {

        ICollection<IItemAwareElement> Sources { get; }

        IItemAwareElement Target { get; set; }


        IFormalExpression GetTransformation();

        void SetTransformation(Transformation transformation);

        ICollection<IAssignment> Assignments { get; }

        //new IBpmnEdge DiagramElement { get; }
    }
}