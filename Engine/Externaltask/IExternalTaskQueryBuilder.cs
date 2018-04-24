using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Externaltask
{
    /// <summary>
    ///     
    /// </summary>
    public interface IExternalTaskQueryBuilder
    {
        /// <summary>
        ///     Specifies that tasks of a topic should be fetched and locked for
        ///     a certain amount of time
        /// </summary>
        /// <param name="topicName"> the name of the topic </param>
        /// <param name="lockDuration">
        ///     the duration in milliseconds for which tasks should be locked;
        ///     begins at the time of fetching
        ///     @return
        /// </param>
        //IQueryable<IExternalTask>TopicBuilder Topic(string topicName, long lockDuration);

        /// <summary>
        ///     Performs the fetching. Locks candidate tasks of the given topics
        ///     for the specified duration.
        /// </summary>
        /// <returns>
        ///     fetched external tasks that match the topic and that can be
        ///     successfully locked
        /// </returns>
        IList<ILockedExternalTask> Execute();
    }
}