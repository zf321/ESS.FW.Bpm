using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.UserTask
{
    /// <summary>
    ///     This is for test case UserTaskTest.TestCompleteAfterParallelGateway
    /// </summary>
    public class UserTaskTestCreateTaskListener : ITaskListener
    {
        private IExpression expression;

        public virtual void Notify(IDelegateTask delegateTask)
        {
            if (expression != null && expression.GetValue(delegateTask) != null)
            {
                // get the expression variable
                var exp = expression.GetValue(delegateTask)
                    .ToString();

                // this expression will be evaluated when completing the task
                delegateTask.SetVariableLocal("validationRule", exp);
            }
        }
    }
}