using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using NUnit.Framework;

namespace Engine.Tests.Standalone.Interceptor
{

    [TestFixture]
    public class CommandInvocationContextTest : PluggableProcessEngineTestCase
	{

	  /// <summary>
	  /// Test that the command invocation context always holds the correct command;
	  /// in outer commands as well as nested commands.
	  /// </summary>
	  public virtual void TestGetCurrentCommand()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: impl.Interceptor.Command<?> outerCommand = new SelfAssertingCommand(new SelfAssertingCommand(null));
		ICommand<object> outerCommand = new SelfAssertingCommand(this, new SelfAssertingCommand(this, null));

		processEngineConfiguration.CommandExecutorTxRequired.Execute(outerCommand);
	  }

	  protected internal class SelfAssertingCommand : ICommand<object>
	  {
		  private readonly CommandInvocationContextTest _outerInstance;


		protected internal ICommand<object> InnerCommand;

		public SelfAssertingCommand(CommandInvocationContextTest outerInstance, ICommand<object> innerCommand)
		{
			this._outerInstance = outerInstance;
		  this.InnerCommand = innerCommand;
		}

		public virtual object Execute(CommandContext commandContext)
		{
		  Assert.AreEqual(this, Context.CommandInvocationContext.Command);

		  if (InnerCommand != null)
		  {
			ICommandExecutor commandExecutor = Context.ProcessEngineConfiguration.CommandExecutorTxRequired;
			commandExecutor.Execute(InnerCommand);

			// should still be correct after command invocation
			Assert.AreEqual(this, Context.CommandInvocationContext.Command);
		  }

		  return null;
		}

	  }

	}

}