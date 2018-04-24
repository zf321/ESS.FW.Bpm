
 

using ESS.FW.Bpm.Engine.Persistence.Entity;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     Represents the command to set the priority of an existing external ITask.
    ///      
    /// </summary>
    public class SetExternalTaskPriorityCmd : ExternalTaskCmd
    {
        /// <summary>
        ///     The priority that should set on the external ITask.
        /// </summary>
        protected internal long Priority;

        public SetExternalTaskPriorityCmd(string externalTaskId, long priority) : base(externalTaskId)
        {
            this.Priority = priority;
        }

        protected internal override object Execute(ExternalTaskEntity externalTask)
        {
            externalTask.Priority = int.Parse(Priority.ToString());
            return null;
        }

        protected internal override void ValidateInput()
        {
        }
    }
}

