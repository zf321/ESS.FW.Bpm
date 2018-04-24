using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    public interface ITaskReport
    {
        /// <summary>
        ///     Select a list with the number of tasks per group
        /// </summary>
        IList<ITaskCountByCandidateGroupResult> TaskCountByCandidateGroup();
    }
}