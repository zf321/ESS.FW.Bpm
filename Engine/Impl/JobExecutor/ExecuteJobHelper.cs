using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using System;

namespace ESS.FW.Bpm.Engine.Impl.JobExecutor
{
    public class ExecuteJobHelper
    {
        private static readonly JobExecutorLogger Log = ProcessEngineLogger.JobExecutorLogger;

        public static void ExecuteJob(string jobId, ICommandExecutor commandExecutor)
        {
            var jobFailureCollector = new JobFailureCollector(jobId);

            ExecuteJob(jobId, commandExecutor, jobFailureCollector,new ExecuteJobsCmd(jobId, jobFailureCollector));
        }

        public static void ExecuteJob(string nextJobId, ICommandExecutor commandExecutor, JobFailureCollector jobFailureCollector, ICommand<object> cmd)
        {
            try
            {
                commandExecutor.Execute(cmd);
            }
            catch (ProcessEngineException exception)
            {
                HandleJobFailure(nextJobId, jobFailureCollector, exception);
                throw;
            }
            catch (System.Exception exception)
            {
                HandleJobFailure(nextJobId, jobFailureCollector, exception);
                // wrap the exception and throw it to indicate the ExecuteJobCmd failed
                throw Log.WrapJobExecutionFailure(jobFailureCollector, exception);
            }
            finally
            {
                InvokeJobListener(commandExecutor, jobFailureCollector);
            }
        }

        protected internal static void InvokeJobListener(ICommandExecutor commandExecutor, JobFailureCollector jobFailureCollector)
        {
            if (!string.IsNullOrEmpty((jobFailureCollector.jobId)))
                if (jobFailureCollector.Failure != null)
                {
                    // the failed job listener is responsible for decrementing the retries and logging the exception to the DB.
                    var failedJobListener = CreateFailedJobListener(commandExecutor, jobFailureCollector.Failure, jobFailureCollector.JobId);

                    OptimisticLockingException exception = CallFailedJobListenerWithRetries(commandExecutor, failedJobListener);
                    if (exception != null)
                    {
                        throw exception;
                    }
                }
                else
                {
                    var successListener = CreateSuccessfulJobListener(commandExecutor);
                    commandExecutor.Execute(successListener);
                }
        }

        private static OptimisticLockingException CallFailedJobListenerWithRetries(ICommandExecutor commandExecutor, FailedJobListener failedJobListener)
        {
            try
            {
                commandExecutor.Execute(failedJobListener);
                return null;
            }
            catch (OptimisticLockingException ex)
            {
                failedJobListener.IncrementCountRetries();
                if (failedJobListener.GetRetriesLeft() > 0)
                {
                    return CallFailedJobListenerWithRetries(commandExecutor, failedJobListener);
                }
                return ex;
            }
        }

        protected internal static void HandleJobFailure(string nextJobId, JobFailureCollector jobFailureCollector,
            System.Exception exception)
        {
            Log.ExceptionWhileExecutingJob(nextJobId, exception);

            jobFailureCollector.Failure = exception;
        }


        protected internal static FailedJobListener CreateFailedJobListener(ICommandExecutor commandExecutor,
            System.Exception exception, string jobId)
        {
            return new FailedJobListener(commandExecutor, jobId, exception);
        }

        protected internal static SuccessfulJobListener CreateSuccessfulJobListener(ICommandExecutor commandExecutor)
        {
            return new SuccessfulJobListener();
        }
    }
}