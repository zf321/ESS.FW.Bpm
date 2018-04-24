

using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;

namespace ESS.FW.Bpm.Model.Bpmn
{
    /// <summary>
	/// <para>A BPMN 2.0 Model</para>
	/// 
	/// 
	/// 
	/// </summary>
	public interface IBpmnModelInstance : IModelInstance
	{

	  /// <returns> the <seealso cref="Definitions"/>, root element of the Bpmn Model.
	  ///  </returns>
	  IDefinitions Definitions {get;set;}


	  /// <summary>
	  /// Copies the BPMN model instance but not the model. So only the wrapped DOM document is cloned.
	  /// Changes of the model are persistent between multiple model instances.
	  /// </summary>
	  /// <returns> the new BPMN model instance </returns>
	  new IBpmnModelInstance Clone();

	}

}