namespace ESS.FW.Bpm.Engine.Impl.Bpmn.Parser
{
    /// <summary>
    ///     .
    /// </summary>
    public class BpmnParseLogger : ProcessEngineLogger
    {
        // LOGGING

        public virtual void ParsingElement(string elementType, string elementId)
        {
            LogDebug("001", "Parsing element from type '{0}' with id '{1}'", elementType, elementId);
        }

        public virtual void IgnoringNonExecutableProcess(string elementId)
        {
            LogInfo("002",
                "Ignoring non-executable process with id '{0}'. Set the attribute isExecutable=\"true\" to deploy " +
                "this process.", elementId);
        }

        public virtual void MissingIsExecutableAttribute(string elementId)
        {
            LogInfo("003",
                "Process with id '{0}' has no attribute isExecutable. Better set the attribute explicitly, " +
                "especially to be compatible with future engine versions which might change the default behavior.",
                elementId);
        }

        public virtual void ParsingFailure(System.Exception cause)
        {
            LogError("004", "Unexpected Exception with message: {0} ", cause.Message);
        }

        // EXCEPTIONS

        public virtual ProcessEngineException ParsingProcessException(System.Exception cause)
        {
            return new ProcessEngineException(ExceptionMessage("009", "Error while parsing process:"+GetAllMessageFromException(cause)), cause);
        }

        public virtual void ExceptionWhileGeneratingProcessDiagram(System.Exception t)
        {
            LogError("010", "Error while generating process diagram, image will not be stored in repository", t);
        }

        public virtual ProcessEngineException MessageEventSubscriptionWithSameNameExists(string resourceName,
            string eventName)
        {
            throw new ProcessEngineException(ExceptionMessage("011",
                "Cannot deploy process definition '{0}': there already is a message event subscription for the message with name '{1}'.",
                resourceName, eventName));
        }
    }
}