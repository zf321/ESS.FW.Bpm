

using ESS.FW.Bpm.Model.Dmn.instance;
using ESS.FW.Bpm.Model.Xml.impl.instance;

namespace ESS.FW.Bpm.Model.Dmn.impl.instance
{

	public abstract class DmnModelElementInstanceImpl : ModelElementInstanceImpl, IDmnModelElementInstance
	{

	  public DmnModelElementInstanceImpl(ModelTypeInstanceContext instanceContext) : base(instanceContext)
	  {
	  }

	}

}