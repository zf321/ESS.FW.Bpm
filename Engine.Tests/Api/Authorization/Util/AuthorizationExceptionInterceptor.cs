using System;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.Api.Authorization.Util
{
    /// <summary>
    /// </summary>
    public class AuthorizationExceptionInterceptor : CommandInterceptor
    {
        protected internal int Count;

        //private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

        protected internal bool IsActive;
        protected internal AuthorizationException lastException;

        public virtual AuthorizationException LastException
        {
            get { return lastException; }
        }

        public override T Execute<T>(ICommand<T> command)
        {
            try
            {
                Count++; // only catch exception if we are at the top of the command stack
                // (there may be multiple nested command invocations and we need
                // to prevent that this intercepter swallows an exception)
                var result = Next.Execute(command);
                Count--;
                return result;
            }
            catch (AuthorizationException e)
            {
                Count--;
                if (Count == 0 && IsActive)
                {
                    lastException = e;
                    Console.WriteLine("Caught authorization exception; storing for Assertion in test", e);
                }
                else
                {
                    throw e;
                }
            }
            return default(T);
        }

        public virtual void Reset()
        {
            lastException = null;
            Count = 0;
        }

        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }
        
    }
}