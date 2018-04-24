using System;
using ESS.FW.Bpm.Engine.Application.Impl;
using ESS.FW.Bpm.Engine.Common.Logging;
using ESS.FW.Bpm.Engine.Dmn.Impl;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Impl.Core;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Digest;
using ESS.FW.Bpm.Engine.Impl.Externaltask;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.JobExecutor;
using ESS.FW.Bpm.Engine.Impl.migration;
using ESS.FW.Bpm.Engine.Impl.Metrics;
using ESS.FW.Bpm.Engine.Impl.Plugin;
using ESS.FW.Bpm.Engine.Impl.Pvm;
using ESS.FW.Bpm.Engine.Impl.Util;
using ESS.FW.Bpm.Engine.Impl.Scripting;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     .
    /// </summary>
    [System.Serializable]
    public class ProcessEngineLogger : BaseLogger
    {
        public const string ProjectCode = "ENGINE";

        public static readonly ProcessEngineLogger Instance =
            CreateLogger<ProcessEngineLogger>(typeof(ProcessEngineLogger),
                ProjectCode, "org.camunda.bpm.engine", "00");

        public static readonly BpmnParseLogger BpmnParseLogger = CreateLogger<BpmnParseLogger>(
            typeof(BpmnParseLogger),
            ProjectCode, "org.camunda.bpm.engine.bpmn.parser", "01");

        public static readonly BpmnBehaviorLogger BpmnBehaviorLogger =
            CreateLogger<BpmnBehaviorLogger>(typeof(BpmnBehaviorLogger), ProjectCode,
                "org.camunda.bpm.engine.bpmn.behavior",
                "02");

        public static readonly EnginePersistenceLogger PersistenceLogger =
            CreateLogger<EnginePersistenceLogger>(typeof(EnginePersistenceLogger), ProjectCode,
                "org.camunda.bpm.engine.persistence",
                "03");

        //public static readonly CmmnTransformerLogger CMMN_TRANSFORMER_LOGGER = BaseLogger.createLogger(typeof(CmmnTransformerLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.transformer", "04");

        //public static readonly CmmnBehaviorLogger CMNN_BEHAVIOR_LOGGER = BaseLogger.createLogger(typeof(CmmnBehaviorLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.behavior", "05");

        //public static readonly CmmnOperationLogger CMMN_OPERATION_LOGGER = BaseLogger.createLogger(typeof(CmmnOperationLogger), PROJECT_CODE, "org.camunda.bpm.engine.cmmn.Operation", "06");

        public static readonly ProcessApplicationLogger ProcessApplicationLogger =
            CreateLogger<ProcessApplicationLogger>(typeof(ProcessApplicationLogger), ProjectCode,
                "org.camunda.bpm.application", "07");

        //public static readonly ContainerIntegrationLogger ContainerIntegrationLogger =
        //    CreateLogger<ContainerIntegrationLogger>(typeof(ContainerIntegrationLogger), ProjectCode,
        //        "org.camunda.bpm.container", "08");

        public static readonly EngineUtilLogger UtilLogger = CreateLogger<EngineUtilLogger>(typeof(EngineUtilLogger),
            ProjectCode, "org.camunda.bpm.engine.Util", "09");

        public static readonly TransactionLogger TxLogger = CreateLogger<TransactionLogger>(typeof(TransactionLogger),
            ProjectCode, "org.camunda.bpm.engine.tx", "11");

        public static readonly ConfigurationLogger ConfigLogger = CreateLogger<ConfigurationLogger>(
            typeof(ConfigurationLogger), ProjectCode, "org.camunda.bpm.engine.cfg", "12");

        public static readonly CommandLogger CmdLogger = CreateLogger<CommandLogger>(typeof(CommandLogger),
            ProjectCode,
            "org.camunda.bpm.engine.cmd", "13");

        public static readonly JobExecutorLogger JobExecutorLogger =
            CreateLogger<JobExecutorLogger>(typeof(JobExecutorLogger), ProjectCode,
                "org.camunda.bpm.engine.jobexecutor", "14");

        //public static readonly TestLogger TEST_LOGGER = BaseLogger.createLogger<>(typeof(TestLogger), PROJECT_CODE,
        //    "org.camunda.bpm.engine.test", "15");

        public static readonly ContextLogger ContextLogger = CreateLogger<ContextLogger>(typeof(ContextLogger),
            ProjectCode, "org.camunda.bpm.engine.context", "16");

        public static readonly CoreLogger CoreLogger = CreateLogger<CoreLogger>(typeof(CoreLogger), ProjectCode,
            "org.camunda.bpm.engine.core", "17");

        public static readonly MetricsLogger MetricsLogger = CreateLogger<MetricsLogger>(typeof(MetricsLogger),
            ProjectCode, "org.camunda.bpm.engine.metrics", "18");

        public static readonly AdministratorAuthorizationPluginLogger AdminPluginLogger =
            CreateLogger<AdministratorAuthorizationPluginLogger>(typeof(AdministratorAuthorizationPluginLogger),
                ProjectCode,
                "org.camunda.bpm.engine.plugin.admin", "19");

        public static readonly PvmLogger PvmLogger = CreateLogger<PvmLogger>(typeof(PvmLogger), ProjectCode,
            "org.camunda.bpm.engine.Pvm", "20");

        public static readonly ScriptLogger SCRIPT_LOGGER = BaseLogger.CreateLogger<ScriptLogger>(typeof(ScriptLogger), ProjectCode,
            "org.camunda.bpm.engine.script", "21");

        public static readonly DecisionLogger DecisionLogger = CreateLogger<DecisionLogger>(typeof(DecisionLogger),
            ProjectCode, "org.camunda.bpm.engine.dmn", "22");

        public static readonly MigrationLogger MigrationLogger = CreateLogger<MigrationLogger>(typeof(MigrationLogger),
            ProjectCode, "org.camunda.bpm.engine.migration", "23");

        public static readonly ExternalTaskLogger ExternalTaskLogger =
            CreateLogger<ExternalTaskLogger>(typeof(ExternalTaskLogger), ProjectCode,
                "org.camunda.bpm.engine.externaltask",
                "24");

        public static readonly SecurityLogger SecurityLogger = CreateLogger<SecurityLogger>(typeof(SecurityLogger),
            ProjectCode, "org.camunda.bpm.engine.security", "25");

        public virtual void ProcessEngineCreated(string name)
        {
            LogInfo("001", "Process Engine {0} created.", name);
        }

        public virtual void ProcessEngineAlreadyInitialized()
        {
            LogInfo("002", "Process engine already initialized");
        }

        public virtual void InitializingProcessEngineForResource(Uri resourceUrl)
        {
            LogInfo("003", "Initializing process engine for resource {0}", resourceUrl);
        }

        public virtual void InitializingProcessEngine(string name)
        {
            LogInfo("004", "Initializing process engine {0}", name);
        }

        public virtual void ExceptionWhileInitializingProcessengine(System.Exception e)
        {
            LogError("005", "Exception while initializing process engine {0}", e.Message, e);
        }

        public virtual void ExceptionWhileClosingProcessEngine(string @string, System.Exception e)
        {
            LogError("006", "Exception while closing process engine {0}", @string, e);
        }

        public virtual void ProcessEngineClosed(string name)
        {
            LogInfo("007", "Process Engine {0} closed", name);
        }
    }
}