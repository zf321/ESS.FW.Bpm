using System;
using System.Collections.Generic;
using System.Threading;
using ESS.FW.Bpm.Engine.Application;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Core.Instance;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.Variable.Serializer;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Common.Extensions;

namespace ESS.FW.Bpm.Engine.context.Impl
{
    /// <summary>
    /// </summary>
    public class Context
    {
        
        //protected internal static ThreadLocal<Stack<CommandContext>> commandContextThreadLocal = new ThreadLocal<Stack<CommandContext>>(() => new Stack<CommandContext>(), true);
        private  static ThreadLocal<Stack<CommandContext>> _commandContextThreadLocal = new ThreadLocal<Stack<CommandContext>>();
        //protected internal static ThreadLocal<Stack<CommandInvocationContext>> commandInvocationContextThreadLocal = new ThreadLocal<Stack<CommandInvocationContext>>(() => new Stack<CommandInvocationContext>(), true);
        private static ThreadLocal<Stack<CommandInvocationContext>> _commandInvocationContextThreadLocal =  new ThreadLocal<Stack<CommandInvocationContext>>();
        //protected internal static ThreadLocal<Stack<ProcessEngineConfigurationImpl>> processEngineConfigurationStackThreadLocal =  new ThreadLocal<Stack<ProcessEngineConfigurationImpl>>(() => new Stack<ProcessEngineConfigurationImpl>(), true);
        private static ThreadLocal<Stack<ProcessEngineConfigurationImpl>> _processEngineConfigurationStackThreadLocal = new ThreadLocal<Stack<ProcessEngineConfigurationImpl>>();
        //protected internal static ThreadLocal<Stack<CoreExecutionContext<ExecutionEntity>>> executionContextStackThreadLocal = new ThreadLocal<Stack<CoreExecutionContext<ExecutionEntity>>>(() => new Stack<CoreExecutionContext<ExecutionEntity>>(), true);
        private static ThreadLocal<Stack<CoreExecutionContext/*<CoreExecution>*/>> _executionContextStackThreadLocal = new ThreadLocal<Stack<CoreExecutionContext/*<CoreExecution>*/>>();
        //protected internal static ThreadLocal<JobExecutorContext> jobExecutorContextThreadLocal = new ThreadLocal<JobExecutorContext>(true);
        private static ThreadLocal<JobExecutorContext> _jobExecutorContextThreadLocal = new ThreadLocal<JobExecutorContext>();
        //protected internal static ThreadLocal<Stack<IProcessApplicationReference>> processApplicationContext = new ThreadLocal<Stack<IProcessApplicationReference>>(() => new Stack<IProcessApplicationReference>(), true);
        private static ThreadLocal<Stack<IProcessApplicationReference>> _processApplicationContext = new ThreadLocal<Stack<IProcessApplicationReference>>();

        public static CommandContext CommandContext
        {
            get
            {
                var stack = GetStack(_commandContextThreadLocal);
                return stack.IsEmpty() ? null : stack.Peek();
            }
            set => GetStack(_commandContextThreadLocal).Push(value);
        }

        public static void RemoveCommandContext()
        {
            GetStack(_commandContextThreadLocal).Pop();
        }

        public static CommandInvocationContext CommandInvocationContext
        {
            get
            {
                var stack = GetStack(_commandInvocationContextThreadLocal);
                return stack.IsEmpty() ? null : stack.Peek();
            }
            set => GetStack(_commandInvocationContextThreadLocal).Push(value);
        }

        public static void RemoveCommandInvocationContext()
        {
            GetStack(_commandInvocationContextThreadLocal).Pop();
        }

        public static ProcessEngineConfigurationImpl ProcessEngineConfiguration
        {
            get
            {
                var stack = GetStack(_processEngineConfigurationStackThreadLocal);
                return stack.Count == 0 ? null : stack.Peek();
            }
            set => GetStack(_processEngineConfigurationStackThreadLocal).Push(value);
        }

        public static void RemoveProcessEngineConfiguration()
        {
            GetStack(_processEngineConfigurationStackThreadLocal).Pop();
        }

        public static BpmnExecutionContext BpmnExecutionContext => (BpmnExecutionContext)CoreExecutionContext;

        //public static CaseExecutionContext CaseExecutionContext => (CaseExecutionContext) CoreExecutionContext;


        public static CoreExecutionContext CoreExecutionContext
        {
            get
            {
                var stack = GetStack(_executionContextStackThreadLocal);
                if (stack == null || stack.IsEmpty())
                    return null;
                return stack.Peek();
            }
        }

        public static void SetExecutionContext(ExecutionEntity execution)
        {            
            GetStack(_executionContextStackThreadLocal).Push(new BpmnExecutionContext(execution));
        }

        //public static void SetExecutionContext(CaseExecutionEntity execution)
        //{
        //    GetStack(_executionContextStackThreadLocal).Push(new CaseExecutionContext(execution));
        //}

        public static void RemoveExecutionContext()
        {
            var stack = GetStack(_executionContextStackThreadLocal);
            if (stack.Count > 0)
                stack.Pop();
        }

        /// <summary>
        ///     获取队列，如果为NULL给空集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="threadLocal"></param>
        /// <returns></returns>
        protected internal static Stack<T> GetStack<T>(ThreadLocal<Stack<T>> threadLocal)
        {
            var stack = threadLocal.Value;
            if (stack == null)
            {
                stack = new Stack<T>();
                threadLocal.Value = stack;
                //threadLocal.Values.Add(stack);
            }
            return stack;
        }

        public static JobExecutorContext JobExecutorContext
        {
            get => _jobExecutorContextThreadLocal.Value;
            set => _jobExecutorContextThreadLocal.Value = value;
            //set => jobExecutorContextThreadLocal.Values.Add(value);
        }

        public static void RemoveJobExecutorContext()
        {
            _jobExecutorContextThreadLocal.Value = null;
        }


        public static IProcessApplicationReference CurrentProcessApplication
        {
            get
            {
                var stack = GetStack(_processApplicationContext);
                return stack.IsEmpty() ? null : stack.Peek();
            }
            set => GetStack(_processApplicationContext).Push(value);
        }

        public static void RemoveCurrentProcessApplication()
        {
            GetStack(_processApplicationContext).Pop();
        }
        
        public static T ExecuteWithinProcessApplication<T>(Func<T> callback, IProcessApplicationReference processApplicationReference)
        {
            return ExecuteWithinProcessApplication(callback, processApplicationReference, null);
        }

        public static T ExecuteWithinProcessApplication<T>(Func<T> callback,  IProcessApplicationReference processApplicationReference, InvocationContext invocationContext)
        {
            var paName = processApplicationReference.Name;
            try
            {
                var processApplication = processApplicationReference.ProcessApplication;
                CurrentProcessApplication = processApplicationReference;

                try
                {
                    //// wrap callback
                    //var wrappedCallback = new ProcessApplicationClassloaderInterceptor<T>(callback);
                    //// execute wrapped callback
                    return processApplication.Execute(callback, invocationContext);
                }
                catch (System.Exception e)
                {
                    // unwrap exception
                    if (e.InnerException != null && e.InnerException is System.Exception)
                        throw e.InnerException;
                    else
                        throw new ProcessEngineException("Unexpected exeption while executing within process application ", e);
                }
                finally
                {
                    RemoveCurrentProcessApplication();
                }
            }
            catch (ProcessApplicationUnavailableException e)
            {
                throw new ProcessEngineException("Cannot switch to process application '" + paName + "' for execution: " + e.Message, e);
            }
        }
        

        #region Serializers

        protected internal static IList<ITypedValueSerializer> customPreVariableSerializers;
        protected internal static IList<ITypedValueSerializer> customPostVariableSerializers;
        protected internal static IVariableSerializers variableSerializers;

        /// <summary>
        ///     Init 序列化 variableSerializers
        /// </summary>
        protected internal static void InitSerialization()
        {
            if (variableSerializers == null)
            {
                variableSerializers = new DefaultVariableSerializers();

                if (customPreVariableSerializers != null)
                    foreach (var customVariableType in customPreVariableSerializers)
                        variableSerializers.AddSerializer(customVariableType);

                //register built-in serializers
                variableSerializers.AddSerializer(new NullValueSerializer());
                variableSerializers.AddSerializer(new StringValueSerializer());
                variableSerializers.AddSerializer(new BooleanValueSerializer());
                variableSerializers.AddSerializer(new ShortValueSerializer());
                variableSerializers.AddSerializer(new IntegerValueSerializer());
                variableSerializers.AddSerializer(new LongValueSerlializer());
                variableSerializers.AddSerializer(new DateValueSerializer());
                variableSerializers.AddSerializer(new DoubleValueSerializer());
                variableSerializers.AddSerializer(new ByteArrayValueSerializer());
                variableSerializers.AddSerializer(new ObjectSerializer());
                variableSerializers.AddSerializer(new FileValueSerializer());

                if (customPostVariableSerializers != null)
                    foreach (var customVariableType in customPostVariableSerializers)
                        variableSerializers.AddSerializer(customVariableType);
            }
        }

        public static IVariableSerializers VariableSerializers
        {
            get
            {
                InitSerialization();
                return variableSerializers;
            }
        }

        #endregion
    }
}