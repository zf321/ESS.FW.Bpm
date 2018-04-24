//using System.Collections.Generic;

//namespace ESS.FW.Bpm.Engine.Externaltask
//{
//    /// <summary>
//    ///     
//    /// </summary>
//    public interface IExternalTaskQueryTopicBuilder : IQueryable<IExternalTask>Builder
//    {
//        /// <summary>
//        ///     Define variables to fetch with all tasks for the current topic. Calling
//        ///     this method multiple times overrides the previously specified variables.
//        /// </summary>
//        /// <param name="variables"> the variable names to fetch, if null all variables will be fetched </param>
//        /// <returns> this builder </returns>
//        IQueryable<IExternalTask>TopicBuilder Variables(params string[] variables);

//        /// <summary>
//        ///     Define variables to fetch with all tasks for the current topic. Calling
//        ///     this method multiple times overrides the previously specified variables.
//        /// </summary>
//        /// <param name="variables"> the variable names to fetch, if null all variables will be fetched </param>
//        /// <returns> this builder </returns>
//        IQueryable<IExternalTask>TopicBuilder Variables(IList<string> variables);

//        /// <summary>
//        ///     Enable deserialization of variable values that are custom objects. By default, the query
//        ///     will not attempt to deserialize the value of these variables.
//        /// </summary>
//        /// <returns> this builder </returns>
//        IQueryable<IExternalTask>TopicBuilder EnableCustomObjectDeserialization();
//    }
//}