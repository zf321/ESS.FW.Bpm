namespace Engine.Tests.Api.Runtime.Migration.Util
{
    /// <summary>
    /// </summary>
    public interface IMigratingBpmnEventTriggerAssert
    {
        void AssertEventTriggerMigrated(MigrationTestRule migrationContext, string targetActivityId);
    }
}