using System;

namespace Engine.Tests
{
    /// <summary>
    ///     Annotation for a test method or class to create and delete a deployment around a test method.
    ///     <para>Usage - Example 1 (method-level annotation):</para>
    ///     <pre>
    ///         package org.Example;
    ///         ..
    ///         public class ExampleTest {
    ///         &#64;Deployment
    ///         public void testForADeploymentWithASingleResource() {
    ///         // a deployment will be available in the engine repository
    ///         // containing the single resource
    ///         <b>org/example/ExampleTest.TestForADeploymentWithASingleResource.bpmn20.xml</b>
    ///         }
    ///         &#64;Deployment(resources = {
    ///         "org/example/processOne.bpmn20.xml",
    ///         "org/example/processTwo.bpmn20.xml",
    ///         "org/example/some.other.Resource" })
    ///         public void testForADeploymentWithASingleResource() {
    ///         // a deployment will be available in the engine repository
    ///         // containing the three resources
    ///         }
    ///     </pre>
    ///     <para>Usage - Example 2 (class-level annotation):</para>
    ///     <pre>
    ///         package org.Example;
    ///         ..
    ///         &#64;Deployment
    ///         public class ExampleTest2 {
    ///         public void testForADeploymentWithASingleResource() {
    ///         // a deployment will be available in the engine repository
    ///         // containing the single resource <b>org/example/ExampleTest2.bpmn20.xml</b>
    ///         }
    ///         &#64;Deployment(resources = "org/example/process.bpmn20.xml")
    ///         public void testForADeploymentWithASingleResource() {
    ///         // the method-level annotation overrides the class-level annotation
    ///         }
    ///     </pre>
    /// </summary>
    public class DeploymentAttribute : Attribute
    {
        /// <summary>
        ///     Specify resources that make up the process definition.
        /// </summary>
        public string[] Resources = new string[0];

        public DeploymentAttribute()
        {
        }
        public DeploymentAttribute(string[] resources)
        {
            this.Resources = resources;
        }
        public DeploymentAttribute(string resources)
        {
            this.Resources = new string[]{resources};
        }
    }
}