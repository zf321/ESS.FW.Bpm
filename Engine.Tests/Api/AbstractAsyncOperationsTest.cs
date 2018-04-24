using System.Collections.Generic;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.History;
using ESS.FW.Bpm.Engine.Runtime;
using NUnit.Framework;

namespace Engine.Tests.Api
{
    
    

	/// <summary>
	///
	/// </summary>
	public abstract class AbstractAsyncOperationsTest: ProcessEngineTestRule
    {
		private bool InstanceFieldsInitialized = false;

		public AbstractAsyncOperationsTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			//testRule = new ProcessEngineTestRule(engineRule);
		}


	  public const string OneTaskProcess = "oneTaskProcess";
	  public const string TESTING_INSTANCE_DELETE = "testing instance Delete";

	  protected internal IRuntimeService runtimeService;
	  protected internal IManagementService managementService;
	  protected internal IHistoryService historyService;

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;
        
        public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

	  protected internal virtual void executeSeedJob(IBatch batch)
	  {
		string seedJobDefinitionId = batch.SeedJobDefinitionId;
		IJob seedJob = managementService.CreateJobQuery()
                //.JobDefinitionId(seedJobDefinitionId)
                .First();
		Assert.NotNull(seedJob);
		managementService.ExecuteJob(seedJob.Id);
	  }

	  /// <summary>
	  /// Execute all batch jobs of batch once and collect exceptions during job execution.
	  /// </summary>
	  /// <param name="batch"> the batch for which the batch jobs should be executed </param>
	  /// <returns> the catched exceptions of the batch job executions, is empty if non where thrown </returns>
	  protected internal virtual IList<System.Exception> executeBatchJobs(IBatch batch)
	  {
		string batchJobDefinitionId = batch.BatchJobDefinitionId;
		IList<IJob> batchJobs = managementService.CreateJobQuery()
                //.JobDefinitionId(batchJobDefinitionId)
                .ToList();
		Assert.IsFalse(batchJobs.Count == 0);

		IList<System.Exception> catchedExceptions = new List<System.Exception>();

		foreach (IJob batchJob in batchJobs)
		{
		  try
		  {
			managementService.ExecuteJob(batchJob.Id);
		  }
		  catch (System.Exception e)
		  {
			catchedExceptions.Add(e);
		  }
		}

		return catchedExceptions;
	  }

	  protected internal virtual IList<string> startTestProcesses(int numberOfProcesses)
	  {
		List<string> ids = new List<string>();

		for (int i = 0; i < numberOfProcesses; i++)
		{
		  ids.Add(runtimeService.StartProcessInstanceByKey(OneTaskProcess).ProcessInstanceId);
		}

		return ids;
	  }

	  protected internal virtual void AssertHistoricTaskDeletionPresent(IList<string> processIds, string deleteReason, ProcessEngineTestRule testRule)
	  {
		if (!testRule.HistoryLevelNone)
		{

		  foreach (string processId in processIds)
		  {
			IHistoricTaskInstance historicTaskInstance = historyService.CreateHistoricTaskInstanceQuery(p=>p.ProcessInstanceId== processId)
                        //.ProcessInstanceId(processId)
                        .First();

			Assert.That(historicTaskInstance.DeleteReason, Is.EqualTo(deleteReason));
		  }
		}
	  }

	  protected internal virtual void AssertHistoricBatchExists(ProcessEngineTestRule testRule)
	  {
		if (testRule.HistoryLevelFull)
		{
		  Assert.That(historyService.CreateHistoricBatchQuery().Count(), Is.EqualTo(1L));
		}
	  }

	}

}