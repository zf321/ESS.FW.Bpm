//using System.Collections.Generic;
//using System.Linq;
//using ESS.FW.Bpm.Engine.Externaltask;
//using ESS.FW.Bpm.Engine.Impl.Cmd;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;

//namespace ESS.FW.Bpm.Engine.Impl.Externaltask
//{
//    /// <summary>
//    ///     
//    ///     
//    /// </summary>
//    public class ExternalTaskQueryTopicBuilderImpl : IExternalTaskQueryTopicBuilder
//    {
//        protected internal ICommandExecutor CommandExecutor;

//        protected internal TopicFetchInstruction CurrentInstruction;

//        protected internal IDictionary<string, TopicFetchInstruction> Instructions;
//        protected internal int MaxTasks;

//        /// <summary>
//        ///     Indicates that priority is enabled.
//        /// </summary>
//        protected internal bool UsePriority;

//        protected internal string WorkerId;

//        public ExternalTaskQueryTopicBuilderImpl(ICommandExecutor commandExecutor, string workerId, int maxTasks,
//            bool usePriority)
//        {
//            this.CommandExecutor = commandExecutor;
//            this.WorkerId = workerId;
//            this.MaxTasks = maxTasks;
//            this.UsePriority = usePriority;
//            Instructions = new Dictionary<string, TopicFetchInstruction>();
//        }

//        public virtual IList<ILockedExternalTask> Execute()
//        {
//            SubmitCurrentInstruction();
//            return CommandExecutor.Execute(new FetchExternalTasksCmd(WorkerId, MaxTasks, Instructions, UsePriority));
//        }

//        //public virtual IExternalTaskQueryTopicBuilder Topic(string topicName, long lockDuration)
//        //{
//        //    SubmitCurrentInstruction();
//        //    CurrentInstruction = new TopicFetchInstruction(topicName, lockDuration);
//        //    return this;
//        //}

//        //public virtual IExternalTaskQueryTopicBuilder Variables(params string[] variables)
//        //{
//        //    // don't use plain  since this returns an instance of a different list class
//        //    // that is private and may mess mybatis queries up
//        //    if (variables != null)
//        //        CurrentInstruction.VariablesToFetch = new List<string>(variables);
//        //    return this;
//        //}

//        //public virtual IExternalTaskQueryTopicBuilder Variables(IList<string> variables)
//        //{
//        //    CurrentInstruction.VariablesToFetch = variables;
//        //    return this;
//        //}

//        //public virtual IExternalTaskQueryTopicBuilder EnableCustomObjectDeserialization()
//        //{
//        //    CurrentInstruction.DeserializeVariables = true;
//        //    return this;
//        //}

//        protected internal virtual void SubmitCurrentInstruction()
//        {
//            if (CurrentInstruction != null)
//                Instructions[CurrentInstruction.TopicName] = CurrentInstruction;
//        }
//    }
//}