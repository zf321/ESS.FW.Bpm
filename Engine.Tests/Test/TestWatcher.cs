//using System;
//using System.Collections.Generic;
//using NUnit.Framework;

//namespace ESS.FW.Bpm.Engine.Tests.Test
//{
    
//    /// <summary>
//    /// TestWatcher is a base class for Rules that take note of the testing
//    /// action, without modifying it. For example, this class will keep a log of each
//    /// passing and failing test:
//    /// 
//    /// <pre>
//    /// public static class WatchmanTest {
//    ///  private static String watchedLog;
//    /// 
//    ///  &#064;Rule
//    ///  public TestWatcher watchman= new TestWatcher() {
//    ///      &#064;Override
//    ///      protected void failed(Throwable e, Description description) {
//    ///          watchedLog+= description + &quot;\n&quot;
//    ///      }
//    /// 
//    ///      &#064;Override
//    ///      protected void succeeded(Description description) {
//    ///          watchedLog+= description + &quot; &quot; + &quot;success!\n&quot;
//    ///         }
//    ///     };
//    /// 
//    ///  &#064;Test
//    ///  public void fails() {
//    ///      fail();
//    ///  }
//    /// 
//    ///  &#064;Test
//    ///  public void succeeds() {
//    ///     }
//    /// }
//    /// </pre>
//    /// 
//    /// </summary>
//    public abstract class TestWatcher : ITestRule
//    {
//        public virtual Action Apply(Action @base, Description description)
//        {
//            return () =>
//            {
//                IList<Exception> errors = new List<Exception>();

//                StartingQuietly(description, errors);
//                try
//                {
//                    @base();
//                    SucceededQuietly(description, errors);
//                }
//                catch (AccessViolationException e)
//                {
//                    errors.Add(e);
//                    SkippedQuietly(e, description, errors);
//                }
//                catch (Exception t)
//                {
//                    errors.Add(t);
//                    FailedQuietly(t, description, errors);
//                }
//                finally
//                {
//                    FinishedQuietly(description, errors);
//                }

//                Assume.That(errors, Is.Empty);
//            };
//        }
        

//        private void SucceededQuietly(Description description, IList<Exception> errors)
//        {
//            try
//            {
//                Succeeded(description);
//            }
//            catch (Exception t)
//            {
//                errors.Add(t);
//            }
//        }

//        private void FailedQuietly(Exception t, Description description, IList<Exception> errors)
//        {
//            try
//            {
//                Failed(t, description);
//            }
//            catch (Exception t1)
//            {
//                errors.Add(t1);
//            }
//        }

//        private void SkippedQuietly(AccessViolationException e, Description description, IList<Exception> errors)
//        {
//            try
//            {
//                Skipped(e, description);
//            }
//            catch (Exception t)
//            {
//                errors.Add(t);
//            }
//        }

//        private void StartingQuietly(Description description, IList<Exception> errors)
//        {
//            try
//            {
//                Starting(description);
//            }
//            catch (Exception t)
//            {
//                errors.Add(t);
//            }
//        }

//        private void FinishedQuietly(Description description, IList<Exception> errors)
//        {
//            try
//            {
//                Finished(description);
//            }
//            catch (Exception t)
//            {
//                errors.Add(t);
//            }
//        }

//        /// <summary>
//        /// Invoked when a test succeeds
//        /// </summary>
//        protected internal virtual void Succeeded(Description description)
//        {
//        }

//        /// <summary>
//        /// Invoked when a test fails
//        /// </summary>
//        protected internal virtual void Failed(Exception e, Description description)
//        {
//        }

//        /// <summary>
//        /// Invoked when a test is skipped due to a failed assumption.
//        /// </summary>
//        protected internal virtual void Skipped(AccessViolationException e, Description description)
//        {
//        }

//        /// <summary>
//        /// Invoked when a test is about to start
//        /// </summary>
//        protected internal virtual void Starting(Description description)
//        {
//        }

//        /// <summary>
//        /// Invoked when a test method finishes (whether passing or failing)
//        /// </summary>
//        protected internal virtual void Finished(Description description)
//        {
//        }
//    }
//}
