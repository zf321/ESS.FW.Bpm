using System.Threading;

namespace Engine.Tests.ConCurrency
{
    // Todo: System.Threading.Thread封闭类无法继承
    public class ControllableThread //: System.Threading.Thread
    {

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        public ControllableThread() : base()
        {
            //Name = generateName();
        }

        public ControllableThread(ThreadStart runnable) //: base(runnable)
        {
            //Name = generateName();
        }

        protected internal virtual string generateName()
        {
            //JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.GetName method:
            string className = this.GetType().FullName;
            int dollarIndex = className.LastIndexOf('$');
            return className.Substring(dollarIndex + 1);
        }

        public virtual void startAndWaitUntilControlIsReturned()
        {
            //lock (this)
            //{
            //    Debug.WriteLine("test thread will start " + Name + " and wait till it returns control");
            //    start();
            //    try
            //    {
            //        Monitor.Wait(this);
            //    }
            //    catch (InterruptedException e)
            //    {
            //        Console.WriteLine(e.ToString());
            //        Console.Write(e.StackTrace);
            //    }
            //}
        }

        public virtual void returnControlToTestThreadAndWait()
        {
            //lock (this)
            //{
            //    Debug.WriteLine(TestContext.CurrentContext.Test.Name + " will notify test thread and till test thread proceeds this thread");
            //    Monitor.Pulse(this);
            //    try
            //    {
            //        Monitor.Wait(this);
            //    }
            //    catch (InterruptedException e)
            //    {
            //        Console.WriteLine(e.ToString());
            //        Console.Write(e.StackTrace);
            //    }
            //}
        }

        public virtual void returnControlToControllableThreadAndWait()
        {
            lock (this)
            {
                // just for understanding the test case
                returnControlToTestThreadAndWait();
            }
        }

        public virtual void proceedAndWaitTillDone()
        {
            //lock (this)
            //{
            //    Debug.WriteLine("test thread will notify " + Name + " and wait until it completes");
            //    Monitor.Pulse(this);
            //    try
            //    {
            //        join();
            //    }
            //    catch (InterruptedException e)
            //    {
            //        Console.WriteLine(e.ToString());
            //        Console.Write(e.StackTrace);
            //    }
            //}
        }
    }

}