using ESS.FW.Bpm.Engine.Impl.Delegate;
using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Form.Impl.Handler
{
    /// <summary>
    ///     
    /// </summary>
    public class CreateTaskFormInvocation : DelegateInvocation
    {
        protected internal TaskEntity ITask;

        protected internal ITaskFormHandler TaskFormHandler;

        public CreateTaskFormInvocation(ITaskFormHandler taskFormHandler, TaskEntity ITask) : base(null, null)
        {
            this.TaskFormHandler = taskFormHandler;
            this.ITask = ITask;
        }

//JAVA TO C# CONVERTER WARNING: MethodInfo 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void invoke() throws Exception
        protected internal override void Invoke()
        {
            InvocationResult = TaskFormHandler.CreateTaskForm(ITask);
        }
    }
}