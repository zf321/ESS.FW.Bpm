using ESS.FW.Bpm.Model.Bpmn;

namespace Engine.Tests.Api.Runtime.Migration.Models.Builder
{
    public class DefaultExternalTaskModelBuilder
    {
        public const string DefaultProcessKey = "Process";
        public const string DefaultExternalTaskName = "externalTask";
        public const string DefaultExternalTaskType = "external";
        public const string DefaultTopic = "foo";
        public const int DefaultPriority = 1;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string ExternalTaskNameRenamed = DefaultExternalTaskName;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string ExternalTaskTypeRenamed = DefaultExternalTaskType;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal int? PriorityRenamed = DefaultPriority;

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string ProcessKeyRenamed = DefaultProcessKey;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal string TopicRenamed = DefaultTopic;

        public static DefaultExternalTaskModelBuilder CreateDefaultExternalTaskModel()
        {
            return new DefaultExternalTaskModelBuilder();
        }

        public virtual DefaultExternalTaskModelBuilder ProcessKey(string processKey)
        {
            ProcessKeyRenamed = processKey;
            return this;
        }

        public virtual DefaultExternalTaskModelBuilder ExternalTaskName(string externalTaskName)
        {
            ExternalTaskNameRenamed = externalTaskName;
            return this;
        }

        public virtual DefaultExternalTaskModelBuilder ExternalTaskType(string externalTaskType)
        {
            ExternalTaskTypeRenamed = externalTaskType;
            return this;
        }

        public virtual DefaultExternalTaskModelBuilder Topic(string topic)
        {
            TopicRenamed = topic;
            return this;
        }

        public virtual DefaultExternalTaskModelBuilder Priority(int? priority)
        {
            PriorityRenamed = priority;
            return this;
        }

        public virtual IBpmnModelInstance Build()
        {
            return ProcessModels.NewModel(ProcessKeyRenamed)
                .StartEvent()
                .ServiceTask(ExternalTaskNameRenamed)
                .CamundaType(ExternalTaskTypeRenamed)
                .CamundaTopic(TopicRenamed)
                .CamundaTaskPriority(PriorityRenamed.ToString())
                .EndEvent()
                .Done();
        }
    }
}