using System.Linq;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Cmd;

namespace Engine.Tests.Api.Runtime.Migration
{
    public class MigrateProcessInstanceDelegate : IJavaDelegate
    {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.Camunda.bpm.Engine.Delegate.IDelegateExecution execution) throws Exception
        public virtual void Execute(IBaseDelegateExecution execution)
        {
            var repoService = ((IDelegateExecution) execution).ProcessEngineServices.RepositoryService;
            var targetDefinition = repoService.CreateProcessDefinitionQuery()
                /*.LatestVersion()*/
                .First();

            var migrationCommand = new SetProcessDefinitionVersionCmd(
                ((IDelegateExecution) execution).ProcessInstanceId,
                targetDefinition.Version);

            ESS.FW.Bpm.Engine.context.Impl.Context.ProcessEngineConfiguration.CommandExecutorTxRequired.Execute(migrationCommand);
        }
    }
}