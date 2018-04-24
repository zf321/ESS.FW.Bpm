using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelMissingFunctionException : FeelException
    {
        protected String function;
        public FeelMissingFunctionException(String message, String function) : base(message)
        {
            this.function = function;
        }
        public FeelMissingFunctionException(String message, System.Exception cause, String function):base(message,cause)
        {
            this.function = function;
        }
        public String GetFunction()
        {
            return this.function;
        }
    }
}
