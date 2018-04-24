using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable.Value;
using NUnit.Framework;

namespace Engine.Tests.Api.Variables
{
    

    /// <summary>
	/// 
	/// 
	/// </summary>
	[TestFixture] public class GetSerializedValueDelegate : IJavaDelegate
	{

        [Test]
        public virtual void Execute(IBaseDelegateExecution execution)
	  {

		ITypedValue typedValue = execution.GetVariableTyped<ITypedValue>("varName", false);
		Assert.NotNull(typedValue);

	  }

	}

}