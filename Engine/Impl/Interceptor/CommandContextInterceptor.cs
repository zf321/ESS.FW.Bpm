using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;

namespace ESS.FW.Bpm.Engine.Impl.Interceptor
{
    /// <summary>
    ///     <para>
    ///         CommandContext拦截器
    ///         Interceptor used for opening the <seealso cref="CommandContext" /> and
    ///         <seealso cref="CommandInvocationContext" />.
    ///     </para>
    ///     <para>
    ///         Since 7.1, this interceptor will not always open a new command context but instead reuse an existing
    ///         command context if possible. This is required for supporting process engine public API access from
    ///         delegation code (see <seealso cref="IProcessEngineServicesAware" />.). However, for every command, a new
    ///         command invocation context is created. While a command context holds resources that are
    ///         shared between multiple commands, such as database sessions, a command invocation context holds
    ///         resources specific for a single command.
    ///     </para>
    ///     <para>
    ///         The interceptor will check whether an open command context exists. If true, it will reuse the
    ///         command context. If false, it will open a new one. We will always push the context to the
    ///         <seealso cref="context" /> stack. So ins some situations, you will see the same context being pushed to the
    ///         sack
    ///         multiple times. The rationale is that the size of  the stack should allow you to determine whether
    ///         you are currently running an 'inner' command or an 'outer' command as well as your current stack size.
    ///         Existing code may rely on this behavior.
    ///     </para>
    ///     <para>
    ///         The interceptor can be configured using the property <seealso cref="#alwaysOpenNew" />.
    ///         If this property is set to true, we will always open a new context regardless whether there already
    ///         exists an active context or not. This is required for properly supporting REQUIRES_NEW semantics for
    ///         commands run through the
    ///         <seealso cref="ProcessEngineConfigurationImpl#getCommandInterceptorsTxRequiresNew()" />
    ///         chain. In that context the 'inner' command must be able to succeed / fail independently from the
    ///         'outer' command.
    ///     </para>
    ///     / / / commandcontext拦截器
    /// / / <见CREF =“commandinvocationcontext”/>。
    ////
    ////自7.1起，此拦截器将不总是打开一个新的命令上下文，而是重用现有的命令上下文。
    //如果可能，命令上下文。这是从授权代码支持流程引擎公共API访问要求（见<<又见CREF =“iprocessengineservicesaware”/>。）。但是，对于每个命令，都创建了一个新的命令调用上下文。虽然命令上下文保存在多个命令（如数据库会话）之间共享的资源，但命令调用上下文保存针对单个命令的特定资源。< /第三段>
    //<副>
    //拦截器将检查是否存在打开命令上下文。如果为true，它将重用命令上下文。如果FALSE，它将打开一个新的。我们将始终把context对<<又见CREF =“context”/>堆栈。所以插件，某些情况下，你会看到同样的情况下被推到这袋多次。理由是堆栈的大小应该允许您确定当前是否运行了“内部”命令或“外部”命令以及当前堆栈大小。
    /// / /拦截器可以配置使用属性<见CREF =“# alwaysopennew”/>。如果将此属性设置为true，无论是否已经存在活动上下文，我们总是打开一个新上下文。这是需要适当的支持requires_new语义命令贯穿<<又见CREF =“processengineconfigurationimpl # getcommandinterceptorstxrequiresnew()”/>
    ///链。在这种情况下，“内部”命令必须能够独立于“外部”命令成功/失败。
    /// </summary>
    public class CommandContextInterceptor : CommandInterceptor
    {
        private static readonly CommandLogger Log = ProcessEngineLogger.CmdLogger;

        /// <summary>
        ///     if true, we will always open a new command context
        /// </summary>
        protected internal bool AlwaysOpenNew;


        public CommandContextInterceptor(CommandContextFactory commandContextFactory,
            ProcessEngineConfigurationImpl processEngineConfiguration)
        {
            this.CommandContextFactory = commandContextFactory;
            this.ProcessEngineConfiguration = processEngineConfiguration;
        }

        public CommandContextInterceptor(CommandContextFactory commandContextFactory,
            ProcessEngineConfigurationImpl processEngineConfiguration, bool alwaysOpenNew)
            : this(commandContextFactory, processEngineConfiguration)
        {
            this.AlwaysOpenNew = alwaysOpenNew;
        }

        public virtual CommandContextFactory CommandContextFactory { get; set; }


        public virtual ProcessEngineConfigurationImpl ProcessEngineConfiguration { get; protected internal set; }

        public virtual ProcessEngineConfigurationImpl ProcessEngineContext
        {
            set { ProcessEngineConfiguration = value; }
        }

        public override T Execute<T>(ICommand<T> command) 
        {
            CommandContext context = null;

            if (!AlwaysOpenNew)
            {
                // check whether we can reuse the command context
                var existingCommandContext = Context.CommandContext;
                if ((existingCommandContext != null) && IsFromSameEngine(existingCommandContext))
                    context = existingCommandContext;
            }
            var openNew = context == null;
            var commandObj = command as ICommand<object>;
            var commandInvocationContext = new CommandInvocationContext(commandObj);
            Context.CommandInvocationContext = commandInvocationContext;

            try
            {
                if (openNew)
                {
                    Log.DebugOpeningNewCommandContext();
                    context = CommandContextFactory.CreateCommandContext();
                }
                else
                {
                    Log.DebugReusingExistingCommandContext();
                }

                Context.CommandContext = context;
                Context.ProcessEngineConfiguration = ProcessEngineConfiguration;
                
                // delegate to next interceptor in chain
                return Next.Execute(command);
            }
            //// 取消catch 方便debug
            catch (System.Exception e)
            {
                commandInvocationContext.TrySetThrowable(e);
                throw;
            }
            finally
            {
                try
                {
                    if (openNew)
                    {
                        Log.ClosingCommandContext();
                        context.Close(commandInvocationContext);
                    }
                    else
                    {
                        commandInvocationContext.Rethrow();
                    }
                }
                finally
                {
                    Context.RemoveCommandInvocationContext();
                    Context.RemoveCommandContext();
                    Context.RemoveProcessEngineConfiguration();
                }
            }
        }

        protected internal virtual bool IsFromSameEngine(CommandContext existingCommandContext)
        {
            return ProcessEngineConfiguration == existingCommandContext.ProcessEngineConfiguration;
        }
    }
}