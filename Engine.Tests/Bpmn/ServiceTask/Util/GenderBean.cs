using System;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{
	[Serializable]
	public class GenderBean
	{

	  private const long serialVersionUID = 1L;

	  public virtual string getGenderString(string gender)
	  {
		return "Your gender is: " + gender;
	  }
	}

}