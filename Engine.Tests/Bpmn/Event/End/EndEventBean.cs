using System;
using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.Event.End
{
    
	[Serializable]
	public class EndEventBean
	{

	  private const long serialVersionUID = 1L;

	  public virtual IJavaDelegate GetJavaDelegate()
	  {
			return new DummyServiceTask();
	  }

	}

}