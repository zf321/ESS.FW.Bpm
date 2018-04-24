using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Util;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Util
{

	/// <summary>
	/// </summary>
	[TestFixture]
	public class CompareUtilTest
	{
        
        [Test]
	  public virtual void TestDateNotInAnAscendingOrder()
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(2015, 3, 15);
		DateTime first = calendar;
		calendar = new DateTime(2015, 8, 15);
		DateTime second = calendar;
		DateTime nullDate = DateTime.MinValue;
		Assert.That(CompareUtil.AreNotInAscendingOrder(DateTime.MinValue, first, DateTime.MinValue, second), Is.EqualTo(false));
		Assert.That(CompareUtil.AreNotInAscendingOrder(DateTime.MinValue, first, DateTime.MinValue, first), Is.EqualTo(false));
		Assert.That(CompareUtil.AreNotInAscendingOrder(DateTime.MinValue, second, DateTime.MinValue, first), Is.EqualTo(true));
		Assert.That(CompareUtil.AreNotInAscendingOrder(nullDate, nullDate, nullDate), Is.EqualTo(false));

		Assert.That(CompareUtil.AreNotInAscendingOrder(first, second), Is.EqualTo(false));
		Assert.That(CompareUtil.AreNotInAscendingOrder(first, first), Is.EqualTo(false));
		Assert.That(CompareUtil.AreNotInAscendingOrder(second, first), Is.EqualTo(true));
	  }

        [Test]
        public virtual void TestIsNotContainedIn()
	  {
		string element = "test";
		string[] values = new string[] {"test", "test1", "test2"};
		string[] values2 = new string[] {"test1", "test2"};
		string[] nullValues = null;
		IList<string> nullList = null;

		Assert.That(CompareUtil.ElementIsNotContainedInArray(element, values), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInArray(element, values2), Is.EqualTo(true));
		Assert.That(CompareUtil.ElementIsNotContainedInArray(null, values), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInArray(null, nullValues), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInArray(element, nullValues), Is.EqualTo(false));

		Assert.That(CompareUtil.ElementIsNotContainedInList(element, values), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInList(element,values2), Is.EqualTo(true));
		Assert.That(CompareUtil.ElementIsNotContainedInList(null, values), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInList(null, nullList), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsNotContainedInList(element, nullList), Is.EqualTo(false));
	  }

        [Test]
        public virtual void TestIsContainedIn()
	  {
		string element = "test";
		string[] values = new string[] {"test", "test1", "test2"};
		string[] values2 = new string[] {"test1", "test2"};
		string[] nullValues = null;
		IList<string> nullList = null;

		Assert.That(CompareUtil.ElementIsContainedInArray(element, values), Is.EqualTo(true));
		Assert.That(CompareUtil.ElementIsContainedInArray(element, values2), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInArray(null, values), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInArray(null, nullValues), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInArray(element, nullValues), Is.EqualTo(false));

		Assert.That(CompareUtil.ElementIsContainedInList(element, (values)), Is.EqualTo(true));
		Assert.That(CompareUtil.ElementIsContainedInList(element, (values2)), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInList(null, (values)), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInList(null, nullList), Is.EqualTo(false));
		Assert.That(CompareUtil.ElementIsContainedInList(element, nullList), Is.EqualTo(false));
	  }
	}

}