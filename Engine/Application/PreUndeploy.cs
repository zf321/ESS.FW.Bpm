﻿using System;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>
    ///         Annotation that can be placed on a method of a
    ///         <seealso cref="AbstractProcessApplication ProcessApplication" /> class.
    ///     </para>
    ///     <para>
    ///         The method will be invoked before the process application is undeployed.
    ///     </para>
    ///     <para>
    ///         <strong>LIMITATION:</strong> the annotation must be placed on a method of the same class carrying the
    ///         <code>{@literal @}ProcessApplication</code> annotation. Methods of superclasses are not detected.
    ///     </para>
    ///     <para>
    ///         <strong>NOTE:</strong> A process application class must only define a single
    ///         <code>{@literal @}PostDeploy</code>
    ///         MethodInfo.
    ///     </para>
    ///     <para>
    ///         <strong>NOTE:</strong> if the {@literal @}PostDeploy method throws an exception, the exception is logged but
    ///         the container will still undeploy the application.
    ///     </para>
    ///     <h2>Basic Usage example:</h2>
    ///     <pre>
    ///         {@literal @}ProcessApplication("My Process Application")
    ///         public class MyProcessApplication extends ServletProcessApplication {
    ///         {@literal @}PreUndeploy
    ///         public void cleanup(ProcessEngine processEngine) {
    ///         ...
    ///         }
    ///         }
    ///     </pre>
    ///     <para>
    ///         A method annotated with <code>{@literal @}PreUndeploy</code> may additionally take the following set of
    ///         parameters, in any oder:
    ///         <ul>
    ///             <li>
    ///                 <seealso cref="IProcessApplicationInfo" />: the <seealso cref="IProcessApplicationInfo" /> object for
    ///                 this process application is injected
    ///             </li>
    ///             <li><seealso cref="IProcessEngine" /> the default process engine is injected</li>
    ///             <li>
    ///                 {@code List
    ///                 <ProcessEngine>
    ///                     } all process engines to which this process application has performed deployments are
    ///                     injected.
    ///             </li>
    ///         </ul>
    ///         
    ///     </para>
    /// </summary>
    /// <seealso cref= PostDeploy
    /// </seealso>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class PreUndeploy : Attribute
    {
    }
}