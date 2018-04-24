using System;
using System.Collections.Generic;
using System.Reflection;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.Scripting;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Repository;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Javax.Script;

namespace ESS.FW.Bpm.Engine.Application
{
    


    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractProcessApplication : IProcessApplicationInterface
    {

        private static ProcessApplicationLogger LOG = ProcessEngineLogger.ProcessApplicationLogger;

        protected internal ELResolver processApplicationElResolver;
        protected internal ObjectELResolver processApplicationObjectELResolver;
        protected internal ProcessApplicationScriptEnvironment processApplicationScriptEnvironment;

        protected internal IVariableSerializers variableSerializers;

        protected internal bool isDeployed = false;

        // deployment /////////////////////////////////////////////////////

        public virtual void Deploy()
        {
            if (isDeployed)
            {
                LOG.AlreadyDeployed();
            }
            else
            {
                // deploy the application
                //RuntimeContainerDelegate.INSTANCE.Get().DeployProcessApplication(this);
                isDeployed = true;
            }
        }

        public virtual void Undeploy()
        {
            if (!isDeployed)
            {
                LOG.NotDeployed();
            }
            else
            {
                // delegate stopping of the process application to the runtime container.
                //RuntimeContainerDelegate.INSTANCE.Get().UndeployProcessApplication(this);
                isDeployed = false;
            }
        }

        public virtual void CreateDeployment(string processArchiveName, IDeploymentBuilder deploymentBuilder)
        {
            // default implementation does nothing
        }

        // Runtime ////////////////////////////////////////////

        public virtual string Name
        {
            get
            {
                Type processApplicationClass = this.GetType();
                string name = null;

                ProcessApplicationAttribute annotation = processApplicationClass.GetCustomAttribute<ProcessApplicationAttribute>();
                if (annotation != null)
                {
                    name = annotation.Value;

                    if (string.ReferenceEquals(name, null) || name.Length == 0)
                    {
                        name = annotation.Name;
                    }
                }


                if (string.ReferenceEquals(name, null) || name.Length == 0)
                {
                    name = AutodetectProcessApplicationName();
                }

                return name;
            }
        }

        /// <summary>
        /// Override this method to autodetect an application name in case the
        /// <seealso cref="ProcessApplicationAttribute"/> annotation was used but without parameter.
        /// </summary>
        protected internal abstract string AutodetectProcessApplicationName();
        
        public virtual T Execute<T>(Func<T> callable)
        {
            //ClassLoader originalClassloader = ClassLoaderUtil.ContextClassloader;
            //ClassLoader processApplicationClassloader = ProcessApplicationClassloader;

            try
            {
                //ClassLoaderUtil.ContextClassloader = processApplicationClassloader;

                return callable.Invoke();
                
            }
            catch (System.Exception e)
            {
                throw LOG.ProcessApplicationExecutionException(e);
            }
            finally
            {
                //ClassLoaderUtil.ContextClassloader = originalClassloader;
            }
        }
        public virtual T Execute<T>(Func<T> callable, InvocationContext invocationContext)
        {
            // allows to hook into the invocation
            return Execute(callable);
        }


        //public virtual ClassLoader ProcessApplicationClassloader
        //{
        //    get
        //    {
        //        // the default implementation uses the classloader that loaded
        //        // the application-provided subclass of this class.
        //        return ClassLoaderUtil.GetClassloader(this.GetType());
        //    }
        //}

        public virtual IProcessApplicationInterface RawObject
        {
            get
            {
                return this;
            }
        }

        public virtual IDictionary<string, string> Properties
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        public virtual ELResolver ElResolver
        {
            get
            {
                if (processApplicationElResolver == null)
                {
                    lock (this)
                    {
                        if (processApplicationElResolver == null)
                        {
                            processApplicationElResolver = InitProcessApplicationElResolver();
                        }
                    }
                }
                return processApplicationElResolver;

            }
        }

        public virtual ObjectELResolver ObjectELResolver
        {
            get
            {
                if (processApplicationObjectELResolver == null)
                {
                    lock (this)
                    {
                        if (processApplicationObjectELResolver == null)
                        {
                            processApplicationObjectELResolver = new ObjectELResolver();
                        }
                    }
                }
                return processApplicationObjectELResolver;
            }
        }

        /// <summary>
        /// <para>Initializes the process application provided ElResolver. This implementation uses the
        /// Java SE <seealso cref="ServiceLoader"/> facilities for resolving implementations of <seealso cref="ProcessApplicationElResolver"/>.</para>
        /// <para>
        /// </para>
        /// <para>If you want to provide a custom implementation in your application, place a file named
        /// <code>META-INF/org.camunda.bpm.application.ProcessApplicationElResolver</code> inside your application
        /// which contains the fully qualified classname of your implementation. Or simply override this method.</para>
        /// </summary>
        /// <returns> the process application ElResolver. </returns>
        protected internal virtual ELResolver InitProcessApplicationElResolver()
        {

            return DefaultElResolverLookup.LookupResolver();

        }

        public virtual IDelegateListener<IBaseDelegateExecution> ExecutionListener
        {
            get
            {
                return null;
            }
        }

        public virtual ITaskListener TaskListener
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// see <seealso cref="ProcessApplicationScriptEnvironment#getScriptEngineForName(String, boolean)"/>
        /// </summary>
        public virtual IScriptEngine GetScriptEngineForName(string name, bool cache)
        {
            return ProcessApplicationScriptEnvironment.GetScriptEngineForName(name, cache);
        }

        /// <summary>
        /// see <seealso cref="ProcessApplicationScriptEnvironment#getEnvironmentScripts()"/>
        /// </summary>
        public virtual IDictionary<string, IList<ExecutableScript>> EnvironmentScripts
        {
            get
            {
                throw new NotImplementedException();
                //return ProcessApplicationScriptEnvironment.EnvironmentScripts;
            }
        }

        protected internal virtual ProcessApplicationScriptEnvironment ProcessApplicationScriptEnvironment
        {
            get
            {
                if (processApplicationScriptEnvironment == null)
                {
                    lock (this)
                    {
                        if (processApplicationScriptEnvironment == null)
                        {
                            processApplicationScriptEnvironment = new ProcessApplicationScriptEnvironment(this);
                        }
                    }
                }
                return processApplicationScriptEnvironment;
            }
        }

        public virtual IVariableSerializers VariableSerializers
        {
            get
            {
                return variableSerializers;
            }
            set
            {
                this.variableSerializers = value;
            }
        }

        public abstract  IProcessApplicationReference Reference { get; }
    }
}
