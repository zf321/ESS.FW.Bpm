using ESS.FW.Bpm.Engine.Delegate;

namespace Engine.Tests.Bpmn.ServiceTask.Util
{

	/// <summary>
	/// 
	/// </summary>
	public class ToUpperCaseSetterInjected : IJavaDelegate
	{

	  private IExpression _text;
	  private bool setterInvoked = false;

	  public virtual void execute(IDelegateExecution execution)
	  {

		
	  }

        public void Execute(IBaseDelegateExecution execution)
        {
            if (!setterInvoked)
            {
                throw new System.Exception("Setter was not invoked");
            }
            execution.SetVariable("setterVar", ((string)_text.GetValue(execution)).ToUpper());
        }

        public virtual IExpression Text
	  {
		  set
		  {
			setterInvoked = true;
			this._text = value;
		  }
	  }

	}

}