using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelMissingVariableException:FeelException
    {
        private  String variable;
        public FeelMissingVariableException(String message, String variable):base(message)
        {
            this.variable = variable;
        }

        public FeelMissingVariableException(String message, System.Exception cause, String variable):base(message,cause)
        {
            this.variable = variable;
        }

        public String GetVariable()
        {
            return this.variable;
        }
    }
}
