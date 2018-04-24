using System;

namespace Engine.Tests
{
    /// <summary>
    ///     Annotation for a test method or class to specify the required history level.
    ///     If the current history level of the process engine is lower than the
    ///     specified one then the test method is skipped.
    ///     <para>Usage:</para>
    ///     <pre>
    ///         package org.Example;
    ///         ..
    ///         public class ExampleTest {
    ///         &#64;RequiredHistoryLevel(ProcessEngineConfiguration.HistoryActivity)
    ///         public void testWithHistory() {
    ///         // test something with the history service (e.g. variables)
    ///         }
    ///     </pre>
    /// </summary>
    public class RequiredHistoryLevelAttribute : Attribute
    {
        /// <summary>
        ///     The required history level.
        /// </summary>
        public string value;
        
        public RequiredHistoryLevelAttribute(string value)
        {
            this.value = value;
        }
    }
}