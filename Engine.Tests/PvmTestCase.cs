using System;
using NUnit.Framework;

namespace Engine.Tests
{
    /// <summary>
    /// </summary>
    public class PvmTestCase// : TestCase
    {
        /// <summary>
        ///     Asserts if the provided text is part of some text.
        /// </summary>
        public virtual void AssertTextPresent(string expected, string actual)
        {
            if (ReferenceEquals(actual, null) || (actual.IndexOf(expected, StringComparison.Ordinal) == -1))
                throw new AssertionException("expected presence of [" + expected + "], but was [" + actual + "]");
                //throw new Exception("expected presence of [" + expected + "], but was [" + actual + "]");
        }

        /// <summary>
        ///     Asserts if the provided text is part of some text, ignoring any uppercase characters
        /// </summary>
        public virtual void AssertTextPresentIgnoreCase(string expected, string actual)
        {
            AssertTextPresent(expected.ToLower(), actual.ToLower());
        }

        // {

        // public virtual object defaultManualActivation()
        //IExpression expression = new FixedValue(true);
        //CaseControlRuleImpl caseControlRule = new CaseControlRuleImpl(expression);
        //return caseControlRule;
        // }
    }
}