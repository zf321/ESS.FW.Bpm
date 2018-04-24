namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public interface IMigratingBpmnEventTrigger : IBpmnEventTrigger, IMigratingBpmnEventTriggerAssert
    {
        /// <summary>
        ///     Returns a new trigger that triggers the same event in the context of a different activity (e.g. because the
        ///     activity has changed during migration)
        /// </summary>
        IMigratingBpmnEventTrigger InContextOf(string activityId);
    }
}