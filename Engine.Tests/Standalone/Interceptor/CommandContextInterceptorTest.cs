using System;
using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Interceptor
{
    /// <summary>
    /// </summary>
    [TestFixture]
    public class CommandContextInterceptorTest : PluggableProcessEngineTestCase
    {
        private class CommandAnonymousInnerClass : ICommand<object>
        {
            private readonly CommandContextInterceptorTest _outerInstance;

            public CommandAnonymousInnerClass(CommandContextInterceptorTest outerInstance)
            {
                _outerInstance = outerInstance;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                throw new InvalidOperationException("here i come!");
            }
        }

        private class CommandAnonymousInnerClass2 : ICommand<object>
        {
            private readonly ExceptionThrowingCmd _innerCommand1;
            private readonly ExceptionThrowingCmd _innerCommand2;
            private readonly CommandContextInterceptorTest _outerInstance;

            public CommandAnonymousInnerClass2(CommandContextInterceptorTest outerInstance,
                ExceptionThrowingCmd innerCommand1, ExceptionThrowingCmd innerCommand2)
            {
                _outerInstance = outerInstance;
                _innerCommand1 = innerCommand1;
                _innerCommand2 = innerCommand2;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;

                commandExecutor.Execute(_innerCommand1);
                commandExecutor.Execute(_innerCommand2);

                return null;
            }
        }

        private class CommandAnonymousInnerClass3 : ICommand<object>
        {
            private readonly ExceptionThrowingCmd _innerCommand;
            private readonly CommandContextInterceptorTest _outerInstance;

            public CommandAnonymousInnerClass3(CommandContextInterceptorTest outerInstance,
                ExceptionThrowingCmd innerCommand)
            {
                _outerInstance = outerInstance;
                _innerCommand = innerCommand;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                var commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;

                try
                {
                    commandExecutor.Execute(_innerCommand);
                    Assert.Fail("exception expected to pop up during execution of inner command");
                }
                catch (IdentifiableRuntimeException)
                {
                    // happy path
                    Assert.IsNull(Context.CommandInvocationContext.Throwable,
                        "the exception should not have been propagated to this command's context");
                }

                return null;
            }
        }

        protected internal class ExceptionThrowingCmd : ICommand<object>
        {
            private readonly CommandContextInterceptorTest _outerInstance;

            protected internal System.Exception ExceptionToThrow;


            protected internal bool Executed;

            public ExceptionThrowingCmd(CommandContextInterceptorTest outerInstance, System.Exception e)
            {
                _outerInstance = outerInstance;
                Executed = false;
                ExceptionToThrow = e;
            }

            public virtual object Execute(CommandContext commandContext)
            {
                Executed = true;
                throw ExceptionToThrow;
            }
        }

        protected internal class IdentifiableRuntimeException : System.Exception
        {
            internal const long SerialVersionUid = 1L;
            private readonly CommandContextInterceptorTest _outerInstance;
            protected internal int Id;

            public IdentifiableRuntimeException(CommandContextInterceptorTest outerInstance, int id)
            {
                _outerInstance = outerInstance;
                Id = id;
            }
        }

        [Test]
        public virtual void TestCommandContextGetCurrentAfterException()
        {
            try
            {
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass(this));

                Assert.Fail("expected exception");
            }
            catch (InvalidOperationException)
            {
                // OK
            }

            Assert.IsNull(Context.CommandContext);
        }

        [Test]
        public virtual void TestCommandContextNestedFailingCommands()
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand1 = new ExceptionThrowingCmd(new IdentifiableRuntimeException(1));
            var innerCommand1 = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 1));
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand2 = new ExceptionThrowingCmd(new IdentifiableRuntimeException(2));
            var innerCommand2 = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 2));

            try
            {
                processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass2(this,
                    innerCommand1, innerCommand2));

                Assert.Fail("Exception expected");
            }
            catch (IdentifiableRuntimeException e)
            {
                Assert.AreEqual(1, e.Id);
            }

            Assert.True(innerCommand1.Executed);
            Assert.IsFalse(innerCommand2.Executed);
        }

        [Test]
        public virtual void TestCommandContextNestedTryCatch()
        {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ExceptionThrowingCmd innerCommand = new ExceptionThrowingCmd(new IdentifiableRuntimeException(1));
            var innerCommand = new ExceptionThrowingCmd(this, new IdentifiableRuntimeException(this, 1));

            processEngineConfiguration.CommandExecutorTxRequired.Execute(new CommandAnonymousInnerClass3(this,
                innerCommand));
        }
    }
}