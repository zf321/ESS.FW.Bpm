using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace Engine.Tests.Standalone.Pvm.Activities
{
    public class While : IActivityBehavior
    {
        private readonly int _from;
        private readonly int _to;
        private readonly string _variableName;

        public While(string variableName, int from, int to)
        {
            this._variableName = variableName;
            this._from = from;
            this._to = to;
        }

        public void Execute(IActivityExecution execution)
        {
            var more = execution.Activity.FindOutgoingTransition("more");
            var done = execution.Activity.FindOutgoingTransition("done");

            var value = (int?) execution.GetVariable(_variableName);

            if (value == null)
            {
                execution.SetVariable(_variableName, _from);
                execution.LeaveActivityViaTransition(more);
            }
            else
            {
                value = value + 1;

                if (value < _to)
                {
                    execution.SetVariable(_variableName, value);
                    execution.LeaveActivityViaTransition(more);
                }
                else
                {
                    execution.LeaveActivityViaTransition(done);
                }
            }
        }
    }
}