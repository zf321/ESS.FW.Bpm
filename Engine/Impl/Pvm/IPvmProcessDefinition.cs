namespace ESS.FW.Bpm.Engine.Impl.Pvm
{
    /// <summary>
    ///      
    /// </summary>
    public interface IPvmProcessDefinition : IReadOnlyProcessDefinition
    {
        string DeploymentId { get; }

        IPvmProcessInstance CreateProcessInstance();

        IPvmProcessInstance CreateProcessInstance(string businessKey);

        IPvmProcessInstance CreateProcessInstance(string businessKey, string caseInstanceId);
    }
}