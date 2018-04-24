using System;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class SyncFuture<TV> //: Future<V>
    {
        private readonly System.Exception _e;

        private readonly TV _result;

        public SyncFuture(TV result)
        {
            this._result = result;
        }

        public SyncFuture(System.Exception e)
        {
            this._e = e;
        }

        public virtual bool Cancelled
        {
            get { return false; }
        }

        public virtual bool Done
        {
            get { return true; }
        }

        public virtual bool Cancel(bool mayInterruptIfRunning)
        {
            return false;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get() throws InterruptedException, java.Util.concurrent.ExecutionException
        public virtual TV Get()
        {
            if (_e == null)
                return _result;
            return default(TV);
            //throw new ExecutionException(e);
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public V get(long timeout, java.Util.concurrent.TimeUnit unit) throws InterruptedException, java.Util.concurrent.ExecutionException, java.Util.concurrent.TimeoutException
        //public virtual V get(long timeout, TimeUnit unit)
        //{
        //    return get();

        //}
    }
}