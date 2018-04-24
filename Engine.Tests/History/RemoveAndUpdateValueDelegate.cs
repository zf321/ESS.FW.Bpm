using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Variable.Value.Impl;

namespace Engine.Tests.History
{


    /// <summary>
    /// 
    /// 
    /// </summary>
    [Serializable]
    public class RemoveAndUpdateValueDelegate : IJavaDelegate
    {

        private const long serialVersionUID = 1L;

        public const string NEW_ELEMENT = "new element";

        public virtual void Execute(IBaseDelegateExecution execution)
        {
            IList<string> list;
            var test = execution.GetVariable("listVar");
            //TODO 待优化
            if (test is UntypedValueImpl)
            {
                list = (IList<string>)(((UntypedValueImpl)test).Value);
            }
            else
            {
                list = (IList<string>)execution.GetVariable("listVar");
            }
            execution.RemoveVariable("listVar");
            // implicitly update the previous list, should update the variable value
            list.Add(NEW_ELEMENT);
        }

    }

}