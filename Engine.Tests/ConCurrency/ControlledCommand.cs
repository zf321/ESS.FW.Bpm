

using ESS.FW.Bpm.Engine.Impl.Interceptor;

namespace Engine.Tests.ConCurrency
{
    
	/// <summary>
	/// 
	/// </summary>
	public class ControlledCommand<T> : ICommand<T>
	{

	  protected internal ControllableThread controllableThread;
	  protected internal ICommand<T> command;

	  public ControlledCommand(ControllableThread controllableThread, ICommand<T> command)
	  {
		this.controllableThread = controllableThread;
		this.command = command;
	  }

	  public virtual T Execute(CommandContext commandContext)
	  {
		T result = command.Execute(commandContext);
		controllableThread.returnControlToTestThreadAndWait();
		return result;
	  }
	}
}