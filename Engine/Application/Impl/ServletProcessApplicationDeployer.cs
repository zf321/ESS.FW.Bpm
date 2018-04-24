using ESS.FW.Bpm.Engine.Impl;

namespace ESS.FW.Bpm.Engine.Application.Impl
{
    /// <summary>
    ///     <para>
    ///         This class is an implementation of <seealso cref="ServletContainerInitializer" /> and
    ///         is notified whenever a subclass of <seealso cref="ServletProcessApplication" /> annotated
    ///         with the <seealso cref="ProcessApplicationAttribute" /> annotation is deployed. In such an event,
    ///         we automatically add the class as <seealso cref="ServletContextListener" /> to the
    ///         <seealso cref="ServletContext" />.
    ///     </para>
    ///     <para><strong>NOTE:</strong> Only works with Servlet 3.0 or better.</para>
    ///     
    /// </summary>
//JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @HandlesTypes(ProcessApplication.class) public class ServletProcessApplicationDeployer implements javax.servlet.ServletContainerInitializer
    public class ServletProcessApplicationDeployer //: ServletContainerInitializer
    {
        private static readonly ProcessApplicationLogger Log = ProcessEngineLogger.ProcessApplicationLogger;

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void onStartup(java.Util.Set<Class> c, javax.servlet.ServletContext ctx) throws javax.servlet.ServletException
//        public virtual void onStartup(ISet<Type> c, ServletContext ctx)
//        {
//            if (c == null || c.Count == 0)
//            {
//                // skip deployments that do not carry a PA
//                return;
//            }

//            if (c.Contains(typeof (ProcessApplication)))
//            {
//                // this is a workaround for a bug in WebSphere-8.5 who
//                // ships the annotation itself as part of the discovered classes.

//                // copy into a fresh Set as we don't know if the original Set is mutable or immutable.
//                c = new HashSet<Type>(c);

//                // and now remove the annotation itself.
//                c.remove(typeof (ProcessApplication));
//            }


//            string contextPath = ctx.ContextPath;
//            if (c.Count > 1)
//            {
//                // a deployment must only contain a single PA
//                throw LOG.multiplePasException(c, contextPath);
//            }
//            if (c.Count == 1)
//            {
//                Type paClass = c.GetEnumerator().next();

//                // validate whether it is a legal Process Application
//                if (!paClass.IsSubclassOf(typeof (AbstractProcessApplication)))
//                {
//                    throw LOG.paWrongTypeException(paClass);
//                }

//                // add it as listener if it's a ServletProcessApplication
//                if (paClass.IsSubclassOf(typeof (ServletProcessApplication)))
//                {
//                    LOG.detectedPa(paClass);
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                    ctx.addListener(paClass.FullName);
//                }
//            }
//            else
//            {
//                LOG.servletDeployerNoPaFound(contextPath);
//            }
//        }
    }
}