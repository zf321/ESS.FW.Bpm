using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;

namespace ESS.FW.Bpm.Engine.Application
{

    

    /// <summary>
    /// <para>A Process Application is an ordinary Java Application that uses the camunda process engine for
    /// BPM and Worklow functionality. Most such applications will start their own process engine (or use
    /// a process engine provided by the runtime container), deploy some BPMN 2.0 process definitions and
    /// interact with process instances derived from these process definitions. Since most process applications
    /// perform very similar bootstrapping, deployment and runtime tasks, we generalized this functionality.
    /// The concept is similar to the javax.ws.rs.core.Application class in JAX-RS: adding the process
    /// application class allows you to bootstrap and configure the provided services.</para>
    /// 
    /// <para>Adding a ProcessApplication class to your Java Application provides your applications with the
    /// following services:
    /// 
    /// <ul>
    /// <li><strong>Bootstrapping</strong> embedded process engine(s) or looking up container managed process engine(s).
    /// You can define multiple process engines in a file named processes.xml which is added to your application.
    /// The ProcessApplication class makes sure this file is picked up and the defined process engines are started
    /// and stopped as the application is deployed / undeployed.</li>
    /// <li><strong>Automatic deployment</strong> of classpath BPMN 2.0 resources. You can define multiple deployments
    /// (process archives) in the processes.xml file. The process application class makes sure the deployments
    /// are performed upon deployment of your application. Scanning your application for process definition
    /// resource files (engine in *.bpmn20.xml or *.bpmn) is supported as well.</li>
    /// <li><strong>Classloading & Thread context switching:</strong> Resolution of application-local Java Delegate Implementations and Beans in case of a
    /// multi-application deployment. The process application class allows your java application to
    /// expose your local Java Delegate implementations or Spring / CDI beans to a shared, container managed
    /// process engine. This way you can start a single process engine that dispatches to multiple process
    /// applications that can be (re-)deployed independently.</li>
    /// </ul>
    /// </para>
    /// 
    /// <para>Transforming an existing Java Application into a Process Application is easy and non-intrusive.
    /// You simply have to add:
    /// <ul>
    /// <li>A Process Application class: The Process Application class constitutes the interface between
    /// your application and the process engine. There are different base classes you can extent to reflect
    /// different environments (e.g. Servlet vs. EJB Container):
    /// <ul>
    ///  <li> <seealso cref="ServletProcessApplication"/>: To be used for Process Applications is a Servlet Container like Apache Tomcat.</li>
    ///  <li> <seealso cref="EjbProcessApplication"/>: To be used in a Java EE application server like JBoss, Glassfish or WebSphere Application Server.</li>
    ///  <li> <seealso cref="EmbeddedProcessApplication"/>: To be used when embedding the process engine is an ordinary Java SE application.</li>
    ///  <li> org.camunda.bpm.engine.spring.application.SpringProcessApplication: To be used for bootstrapping the process application from a Spring Application Context.</li>
    /// </ul>
    /// </li>
    /// <li>A processes.xml file to META-INF: The deployment descriptor file allows to provide a declarative
    /// configuration of the deployment(s) this process application makes to the process engine. It can be
    /// empty and serve as simple marker file - but it must be present.</li>
    /// </ul>
    /// </para>
    /// 
    /// 
    /// </summary>
    public interface IProcessApplicationInterface
    {

        /// <summary>
        /// <para>Deploy this process application into the runtime container.</para>
        /// 
        /// <strong>NOTE:</strong> on some containers (like JBoss AS 7) the deployment of
        /// the process application is performed asynchronously and via introspection at deployment
        /// time. This means that there is no guarantee that the process application is fully
        /// deployed after this method returns.
        /// 
        /// <para>If you need a post deployment hook, use the {@literal @}<seealso cref="PostDeploy"/>
        /// annotation.</para>
        /// </summary>
        void Deploy();

        /// <summary>
        /// <para>Undeploy this process application from the runtime container.</para>
        /// 
        /// <para>If your application needs to be notified of the undeployment,
        /// add a {@literal @}<seealso cref="PreUndeploy"/> method to your subclass.</para>
        /// </summary>
        void Undeploy();

        /// <returns> the name of this process application </returns>
        string Name { get; }

        /// <summary>
        /// <para>Returns a globally sharable reference to this process application. This reference may be safely passed
        /// to the process engine. And other applications.</para>
        /// </summary>
        /// <returns> a globally sharable reference to this process application. </returns>
        IProcessApplicationReference Reference { get; }

        /// <summary>
        /// Since <seealso cref="#getReference()"/> may return a proxy object, this method returs the actual, unproxied object and is
        /// meant to be called from the <seealso cref="#execute(Callable)"/> method. (ie. from a Callable implementation passed to
        /// the method.).
        /// </summary>
        IProcessApplicationInterface RawObject { get; }

        /// <summary>
        /// The default implementation simply modifies the Context <seealso cref="ClassLoader"/>
        /// </summary>
        /// <param name="callable"> to be executed "within" the context of this process application. </param>
        /// <returns> the result of the callback </returns>
        //JAVA TO C# CONVERTER TODO TASK: The following line could not be converted:
        T Execute<T>(Func<T> callable);

        /// <summary>
        /// Is invoked instead of <seealso cref="#execute(Callable)"/> if a context is available.
        /// The default implementation simply forward the call to
        /// <seealso cref="#execute(Callable)"/>. A custom implementation can override the method
        /// to hook into the invocation.
        /// </summary>
        /// <param name="callable"> to be executed "within" the context of this process application. </param>
        /// <param name="context"> of the current invocation, can be <code>null</code> </param>
        /// <returns> the result of the callback </returns>
        T Execute<T>(Func<T> callable, InvocationContext context);

        /// <summary>
        /// <para>Override this method to provide an environment-specific <seealso cref="ClassLoader"/> to be used by the process
        /// engine for loading resources from the process application</para>
        /// 
        /// <para><strong>NOTE: the process engine must <em>never</em> cache any references to this <seealso cref="ClassLoader"/>
        /// or to classes obtained through this <seealso cref="ClassLoader"/>.</strong></para>
        /// </summary>
        /// <returns> the <seealso cref="ClassLoader"/> that can be used to load classes and resources from this process application. </returns>
        //ClassLoader ProcessApplicationClassloader { get; }

        /// <summary>
        /// <para>override this method in order to provide a map of properties.</para>
        /// 
        /// <para>The properties are made available globally through the <seealso cref="ProcessApplicationService"/></para>
        /// </summary>
        /// <seealso cref= ProcessApplicationService </seealso>
        /// <seealso cref= ProcessApplicationInfo#getProperties() </seealso>
        IDictionary<string, string> Properties { get; }

        /// <summary>
        /// <para>This allows the process application to provide a custom ElResolver to the process engine.</para>
        /// 
        /// <para>The process engine will use this ElResolver whenever it is executing a
        /// process in the context of this process application.</para>
        /// 
        /// <para>The process engine must only call this method from Callable implementations passed
        /// to <seealso cref="#execute(Callable)"/></para>
        /// </summary>
        ELResolver ElResolver { get; }

        /// <summary>
        /// <para>Returns an instance of <seealso cref="Engine.Impl.Javax.EL.ObjectELResolver"/> that a process application caches.</para>
        /// <para>Has to be managed by the process application since <seealso cref="Engine.Impl.Javax.EL.ObjectELResolver"/> keeps
        /// hard references to classes in a cache.</para>
        /// </summary>
        ObjectELResolver ObjectELResolver { get; }

        /// <summary>
        /// <para>Override this method in order to programmatically add resources to the
        /// deployment created by this process application.</para>
        /// 
        /// <para>This method is invoked at deployment time once for each process archive
        /// deployed by this process application.</para>
        /// 
        /// <para><strong>NOTE:</strong> this method must NOT call the <seealso cref="DeploymentBuilder#deploy()"/>
        /// method.</para>
        /// </summary>
        /// <param name="deploymentBuilder"> the <seealso cref="DeploymentBuilder"/> used to construct the deployment. </param>
        /// <param name="processArchiveName"> the name of the processArchive which is currently being deployed. </param>
        void CreateDeployment(string processArchiveName, IDeploymentBuilder deploymentBuilder);


        /// <summary>
        /// <para>Allows the process application to provide an <seealso cref="ExecutionListener"/> which is notified about
        /// all execution events in all of the process instances deployed by this process application.</para>
        /// 
        /// <para>If this method returns 'null', the process application is not notified about execution events.</para>
        /// </summary>
        /// <returns> an <seealso cref="ExecutionListener"/> or null. </returns>
        IDelegateListener<IBaseDelegateExecution> ExecutionListener { get; }

        /// <summary>
        /// <para>Allows the process application to provide a <seealso cref="TaskListener"/> which is notified about
        /// all Task events in all of the process instances deployed by this process application.</para>
        /// 
        /// <para>If this method returns 'null', the process application is not notified about Task events.</para>
        /// </summary>
        /// <returns> a <seealso cref="TaskListener"/> or null. </returns>
        ITaskListener TaskListener { get; }

    }
}
