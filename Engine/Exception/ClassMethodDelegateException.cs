using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Exceptions
{
    public class ClassMethodDelegateException:System.Exception
    {
        public ClassMethodDelegateException(string msg, System.Exception ex) : base(msg,ex)
        {
        }
    }
}
