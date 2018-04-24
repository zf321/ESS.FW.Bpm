using ESS.FW.Bpm.Engine.Delegate;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{
    
    /// <summary>
	/// 
	/// 
	/// </summary>
	public class GetDeserializedValueDelegate : IJavaDelegate
	{
        [Test]
	  public virtual void Execute(IBaseDelegateExecution Execution)
	  {

		JavaSerializable dataObject = (JavaSerializable) Execution.GetVariable("varName");

		Assert.NotNull(dataObject);
		Assert.AreEqual("foo", dataObject.Property);

	  }

	}

}