namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public interface IBpmnEventTrigger
    {
        void Trigger(string ProcessInstanceId);
    }
}