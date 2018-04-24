using System;
using NUnit.Framework;

namespace Engine.Tests.Util
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class AssertUtil
    {
        /// <summary>
        ///     Drop milliseconds since older MySQL versions cannot store them
        /// </summary>
        [Test]
        public static void AssertEqualsSecondPrecision(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Ticks/1000L, actual.Ticks/1000L, "expected " + expected + " but got " + actual);
        }
    }
}