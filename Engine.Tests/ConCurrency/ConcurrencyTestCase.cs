using System;
using System.Collections.Generic;
using System.Threading;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.ConCurrency
{
    /// <summary>
    /// </summary>
    public abstract class ConcurrencyTestCase : PluggableProcessEngineTestCase
    {
        private IList<ControllableCommand<object>> _controllableCommands;

        [SetUp]
        public virtual void SetUp()
        {
            _controllableCommands = new List<ControllableCommand<object>>();
        }

        [TearDown]
        public override void TearDown()
        {
            // wait for all spawned threads to end
            foreach (var controllableCommand in _controllableCommands)
            {
                ThreadControl threadControl = controllableCommand.Monitor;
                threadControl.executingThread.Interrupt();
                threadControl.executingThread.Join();
            }

            // clear the test thread's interruption state
            Thread.CurrentThread.Interrupt();
        }

        protected internal virtual ThreadControl ExecuteControllableCommand<T>(ControllableCommand<T> command)
        {
            var controlThread = Thread.CurrentThread;

            var thread = new Thread(() =>
            {
                try
                {
                    processEngineConfiguration.CommandExecutorTxRequired.Execute(command);
                }
                catch (System.Exception e)
                {
                    command.Monitor.Exception = e;
                    controlThread.Interrupt();
                    throw e;
                }
            });

            // Todo: 泛型参数到object的转型
            _controllableCommands.Add(command as ControllableCommand<object>);
            command.Monitor.executingThread = thread;

            thread.Start();

            return command.Monitor;
        }

        public abstract class ControllableCommand<T> : ICommand<T>
        {
            protected internal readonly ThreadControl Monitor;

            protected ControllableCommand()
            {
                Monitor = new ThreadControl();
            }

            protected ControllableCommand(ThreadControl threadControl)
            {
                Monitor = threadControl;
            }

            public abstract T Execute(CommandContext commandContext);
            
        }

        public class ThreadControl
        {
            private volatile bool _syncAvailable = false;
            protected internal Thread executingThread;
            private volatile bool _reportFailure;
            private volatile System.Exception _exception;
            private bool _IgnoreSync = false;


            public ThreadControl()
            {
            }

            public ThreadControl(Thread executingThread)
            {
                this.executingThread = executingThread;
            }

            public virtual void WaitForSync()
            {
                WaitForSync(Int32.MaxValue);
            }

            public virtual void WaitForSync(int timeout)
            {
                lock (this)
                {
                    if (_exception != null)
                        if (_reportFailure)
                            return;
                        else
                            Assert.Fail();
                    try
                    {
                        if (!_syncAvailable)
                        {
                            try
                            {
                                Monitor.Wait(this, timeout);
                            }
                            catch (System.Exception e)
                            {
                                if (!_reportFailure || _exception == null)
                                    Assert.Fail("unexpected interruption");
                            }
                        }
                    }
                    finally
                    {
                        _syncAvailable = false;
                    }
                }
            }

            public virtual void WaitUntilDone()
            {
                WaitUntilDone(false);
            }

            public virtual void WaitUntilDone(bool ignoreUpcomingSyncs)
            {
                _IgnoreSync = ignoreUpcomingSyncs;
                MakeContinue();
                Join();
            }

            public virtual void Join()
            {
                try
                {
                    executingThread.Join();
                }
                catch (ThreadInterruptedException ex)
                {
                    if (!_reportFailure || _exception == null)
                    {
                        Assert.Fail("Unexpected interruption");
                    }
                }
            }

            public virtual void Sync()
            {
                lock (this)
                {
                    if (_IgnoreSync)
                        return;

                    _syncAvailable = true;
                    try
                    {
                        Monitor.PulseAll(this);
                        Monitor.Wait(this);
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        if (!_reportFailure || _exception == null)
                        {
                            Assert.Fail("Unexpected interruption");
                        }
                    }
                }
            }

            public virtual void MakeContinue()
            {
                lock (this)
                {
                    if (_exception != null)
                        Assert.Fail();
                    Monitor.PulseAll(this);
                }
            }

            public virtual void MakeContinueAndWaitForSync()
            {
                MakeContinue();
                WaitForSync();
            }

            public virtual void ReportInterrupts()
            {
                _reportFailure = true;
            }

            public virtual void IgnoreFutureSyncs()
            {
                _IgnoreSync = true;
            }

            public virtual System.Exception Exception
            {
                set
                {
                    lock (this)
                    {
                        _exception = value;
                    }
                }
                get => _exception;
            }

        }
    }
}