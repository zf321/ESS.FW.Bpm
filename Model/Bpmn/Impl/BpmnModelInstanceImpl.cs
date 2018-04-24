

using ESS.FW.Bpm.Model.Bpmn.impl.instance;
using ESS.FW.Bpm.Model.Bpmn.instance;
using ESS.FW.Bpm.Model.Xml;
using ESS.FW.Bpm.Model.Xml.impl;
using ESS.FW.Bpm.Model.Xml.instance;
using ModelInstanceImpl = ESS.FW.Bpm.Model.Xml.impl.ModelInstanceImpl;

namespace ESS.FW.Bpm.Model.Bpmn.impl
{
    

    /// <summary>
	/// <para>The Bpmn Model</para>
	/// 
	/// 
	/// </summary>
	public class BpmnModelInstanceImpl : ModelInstanceImpl, IBpmnModelInstance
	{

	  public BpmnModelInstanceImpl(ModelImpl model, ModelBuilder modelBuilder, IDomDocument document) 
            : base(model, modelBuilder, document)
	  {
	  }

	  public virtual IDefinitions Definitions
	  {
		  get
		  {
			return (DefinitionsImpl) DocumentElement;
		  }
		  set
		  {
			DocumentElement = value;
		  }
	  }


	  public virtual IBpmnModelInstance Clone()
	  {
		return new BpmnModelInstanceImpl(model, ModelBuilder, document.Clone());
	  }
        
    }

}