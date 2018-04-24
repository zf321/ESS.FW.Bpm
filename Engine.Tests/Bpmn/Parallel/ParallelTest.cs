using NUnit.Framework;

namespace Engine.Tests.Bpmn.Parallel
{

	using PluggableProcessEngineTestCase = PluggableProcessEngineTestCase;


	/// <summary>
	/// 
	/// </summary>
	public class ParallelTest : PluggableProcessEngineTestCase
	{

        [Test]
        [Deployment]
        public virtual void testParallel()
	  {
		runtimeService.StartProcessInstanceByKey("myProc");
	  }
	}

}