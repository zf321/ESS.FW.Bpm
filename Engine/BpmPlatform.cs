namespace ESS.FW.Bpm.Engine
{
    /// <summary>
    ///     <para>Provides access to the camunda BPM platform services.</para>
    ///     
    /// </summary>
    public sealed class BpmPlatform
    {
        public const string JndiNamePrefix = "java:global";
        public const string AppJndiName = "camunda-bpm-platform";
        public const string ModuleJndiName = "process-engine";

        public const string ProcessEngineServiceName = "ProcessEngineService!org.camunda.bpm.ProcessEngineService";

        public const string ProcessApplicationServiceName =
            "ProcessApplicationService!org.camunda.bpm.ProcessApplicationService";

        public static readonly string ProcessEngineServiceJndiName = JndiNamePrefix + "/" + AppJndiName + "/" +
                                                                         ModuleJndiName + "/" +
                                                                         ProcessEngineServiceName;

        public static readonly string ProcessApplicationServiceJndiName = JndiNamePrefix + "/" + AppJndiName +
                                                                              "/" + ModuleJndiName + "/" +
                                                                              ProcessApplicationServiceName;

        public static IProcessEngineService ProcessEngineService { get; }// = RuntimeContainerDelegate_Fields.INSTANCE.get().ProcessEngineService;

        public static IProcessApplicationService ProcessApplicationService { get; } //= RuntimeContainerDelegate_Fields.INSTANCE.get().ProcessApplicationService;

        public static IProcessEngine DefaultProcessEngine { get; } = ProcessEngineService.DefaultProcessEngine;
    }
}