

using ESS.FW.Bpm.Model.Bpmn.instance.dc;

namespace ESS.FW.Bpm.Model.Bpmn.instance.di
{
    /// <summary>
	/// The DI Shape element
	/// 
	/// 
	/// </summary>
	public interface IShape : INode
	{

	  IBounds Bounds {get;set;}


	}

}