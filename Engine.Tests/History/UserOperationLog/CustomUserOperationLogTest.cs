using System;
using System.Linq;
using Engine.Tests.Util;
using ESS.FW.Bpm.Engine;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Impl.oplog;
using NUnit.Framework;

namespace Engine.Tests.History.UserOperationLog
{

    

    [TestFixture]
    public class CustomUserOperationLogTest
	{

		public const string USER_ID = "demo";

//JAVA TO C# CONVERTER TODO Resources.Task: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static util.ProcessEngineBootstrapRule bootstrapRule = new util.ProcessEngineBootstrapRule("resources/history/useroperationlog/enable.legacy.User.operation.log.Camunda.cfg.xml");
		public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule("resources/history/useroperationlog/enable.legacy.User.operation.log.Camunda.cfg.xml");


		private static readonly string TASK_ID = Guid.NewGuid().ToString();

		private ICommandExecutor commandExecutor;
		private IHistoryService historyService;

        [SetUp]
        public virtual void setUp()
		{
			commandExecutor = ((ProcessEngineConfigurationImpl)bootstrapRule.ProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired;
			historyService = bootstrapRule.ProcessEngine.HistoryService;
		}
        [Test]
		public virtual void testDoNotOverwriteUserId()
		{
			commandExecutor.Execute(new CommandAnonymousInnerClass(this));

			// and check its there
			Assert.That(historyService.CreateUserOperationLogQuery(c=>c.Id == TASK_ID).First().UserId, Is.EqualTo("kermit"));
		}

		private class CommandAnonymousInnerClass : ICommand<object>
		{
			private readonly CustomUserOperationLogTest outerInstance;

			public CommandAnonymousInnerClass(CustomUserOperationLogTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public object Execute(final org.Camunda.bpm.Engine.impl.interceptor.CommandContext commandContext)
			public  object Execute(CommandContext commandContext)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.oplog.UserOperationLogContext userOperationLogContext = new org.Camunda.bpm.Engine.impl.oplog.UserOperationLogContext();
				UserOperationLogContext userOperationLogContext = new UserOperationLogContext();
				userOperationLogContext.UserId = "kermit";

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.Camunda.bpm.Engine.impl.oplog.UserOperationLogContextEntry entry = new org.Camunda.bpm.Engine.impl.oplog.UserOperationLogContextEntry("foo", "bar");
				UserOperationLogContextEntry entry = new UserOperationLogContextEntry("foo", "bar");
				//entry.PropertyChanges = (new PropertyChange(null, null, null));
				entry.TaskId = TASK_ID;
				userOperationLogContext.AddEntry(entry);

				commandContext.OperationLogManager.LogUserOperations(userOperationLogContext);
				return null;
			}
		}
	}

}