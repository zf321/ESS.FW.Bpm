using System;

namespace ESS.FW.Bpm.Engine.Application
{
    /// <summary>
    ///     <para>
    ///         Annotation that can be placed on a method of a
    ///         <seealso cref="AbstractProcessApplication ProcessApplication" /> class.
    ///     </para>
    ///     <para>
    ///         The method will be invoked after the process application has been successfully deployed, meaning that
    ///         <ul>
    ///             <li>
    ///                 If the process application defines one or more <seealso cref="IProcessEngine" />, all process
    ///                 engines have been successfully started and can be looked up.
    ///             </li>
    ///             <li>
    ///                 If the process application defines one or more ProcessArchvies (deployments), all deployments have
    ///                 completed successfully.
    ///             </li>
    ///         </ul>
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
    ///         <strong>NOTE:</strong> if the {@literal @}PostDeploy method throws an exception, the deployment of the process
    ///         application will
    ///         be rolled back, all process engine deployments will be removed and all process engines defined by this
    ///         application will be stopped.
    ///     </para>
    ///     <h2>Basic Usage example:</h2>
    ///     <pre>
    ///         {@literal @}ProcessApplication("My Process Application")
    ///         public class MyProcessApplication extends ServletProcessApplication {
    ///         {@literal @}PostDeploy
    ///         public void startProcess(ProcessEngine processEngine) {
    ///         processEngine.getRuntimeService()
    ///         .startProcessInstanceByKey("invoiceProcess");
    ///         }
    ///         }
    ///     </pre>
    ///     <para>
    ///         A method annotated with <code>{@literal @}PostDeploy</code> may additionally take the following set of
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
    /// <seealso cref= PreUndeploy
    /// </seealso>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class PostDeploy : Attribute
    {
    }
}