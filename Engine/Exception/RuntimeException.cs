using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Exception
{
    public class RuntimeException:System.Exception
    {
        public RuntimeException() : base()
        {

        }
        public RuntimeException(string msg) : base(msg)
        {

        }
        public RuntimeException(string msg,System.Exception ex) : base(msg, ex)
        {

        }
    }
}
