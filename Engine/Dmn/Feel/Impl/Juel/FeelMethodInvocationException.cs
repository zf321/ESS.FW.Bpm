using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelMethodInvocationException:FeelException
    {
        protected String method;
        protected String[] parameters;

        public FeelMethodInvocationException(String message, String method,params string[] parameters):base(message)
        {
            this.method = method;
            this.parameters = parameters;
        }

        public FeelMethodInvocationException(String message, System.Exception cause, String method, params string[] parameters):base(message,cause)
        {
            this.method = method;
            this.parameters = parameters;
        }

        public String GetMethod()
        {
            return this.method;
        }

        public String[] GetParameters()
        {
            return this.parameters;
        }
    }
}
