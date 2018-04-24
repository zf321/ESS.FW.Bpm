//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace ESS.FW.Bpm.Engine.Tests.Test
//{
    

//    /// <summary>
//    /// The RuleChain rule allows ordering of TestRules. You create a
//    /// {@code RuleChain} with <seealso cref="#outerRule(TestRule)"/> and subsequent calls of
//    /// <seealso cref="#around(TestRule)"/>:
//    /// 
//    /// <pre>
//    /// public static class UseRuleChain {
//    /// 	&#064;Rule
//    /// 	public RuleChain chain= RuleChain
//    /// 	                       .outerRule(new LoggingRule("outer rule")
//    /// 	                       .around(new LoggingRule("middle rule")
//    /// 	                       .around(new LoggingRule("inner rule");
//    /// 
//    /// 	&#064;Test
//    /// 	public void example() {
//    /// 		assertTrue(true);
//    ///     }
//    /// }
//    /// </pre>
//    /// 
//    /// writes the log
//    /// 
//    /// <pre>
//    /// starting outer rule
//    /// starting middle rule
//    /// starting inner rule
//    /// finished inner rule
//    /// finished middle rule
//    /// finished outer rule
//    /// </pre>
//    /// 
//    /// </summary>
//    public class RuleChain : ITestRule
//    {
//        private static readonly RuleChain EMPTY_CHAIN = new RuleChain(new List<ITestRule>());

//        private IList<ITestRule> rulesStartingWithInnerMost;

//        /// <summary>
//        /// Returns a {@code RuleChain} without a <seealso cref="TestRule"/>. This method may
//        /// be the starting point of a {@code RuleChain}.
//        /// </summary>
//        /// <returns> a {@code RuleChain} without a <seealso cref="TestRule"/>. </returns>
//        public static RuleChain EmptyRuleChain()
//        {
//            return EMPTY_CHAIN;
//        }

//        /// <summary>
//        /// Returns a {@code RuleChain} with a single <seealso cref="TestRule"/>. This method
//        /// is the usual starting point of a {@code RuleChain}.
//        /// </summary>
//        /// <param name="outerRule"> the outer rule of the {@code RuleChain}. </param>
//        /// <returns> a {@code RuleChain} with a single <seealso cref="TestRule"/>. </returns>
//        public static RuleChain OuterRule(ITestRule outerRule)
//        {
//            return EmptyRuleChain().Around(outerRule);
//        }

//        private RuleChain(IList<ITestRule> rules)
//        {
//            this.rulesStartingWithInnerMost = rules;
//        }

//        /// <summary>
//        /// Create a new {@code RuleChain}, which encloses the {@code nextRule} with
//        /// the rules of the current {@code RuleChain}.
//        /// </summary>
//        /// <param name="enclosedRule"> the rule to enclose. </param>
//        /// <returns> a new {@code RuleChain}. </returns>
//        public virtual RuleChain Around(ITestRule enclosedRule)
//        {
//            IList<ITestRule> rulesOfNewChain = new List<ITestRule>();
//            rulesOfNewChain.Add(enclosedRule);
//            ((List<ITestRule>)rulesOfNewChain).AddRange(rulesStartingWithInnerMost);
//            return new RuleChain(rulesOfNewChain);
//        }

//        /// <summary>
//        /// {@inheritDoc}
//        /// </summary>
//        public virtual Action Apply(Action @base, Description description)
//        {
//            foreach (ITestRule each in rulesStartingWithInnerMost)
//            {
//                @base = each.Apply(@base, description);
//            }
//            return @base;
//        }
//    }
//}
