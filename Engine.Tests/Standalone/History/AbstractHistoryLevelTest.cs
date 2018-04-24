using ESS.FW.Bpm.Engine.History.Impl;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using NUnit.Framework;

namespace Engine.Tests.Standalone.History
{

    [TestFixture]
    public class AbstractHistoryLevelTest
	{


	  public class MyHistoryLevel : AbstractHistoryLevel
	  {

		public override int Id
		{
			get
			{
			  return 4711;
			}
		}

		public override string Name
		{
			get
			{
			  return "myName";
			}
		}

		public override bool IsHistoryEventProduced(HistoryEventTypes eventType, object entity)
		{
		  return false;
		}
	  }

        [Test]
        public virtual void EnsureCorrectToString()
	  {
		Assert.That((new MyHistoryLevel()).ToString(), Is.EqualTo("MyHistoryLevel(name=myName, id=4711)"));
	  }
	}
}