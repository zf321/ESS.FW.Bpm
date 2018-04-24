using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Exception
{
    public class RejectedExecutionException:System.Exception
    {
        public RejectedExecutionException(string msg,System.Exception e) : base(msg, e)
        {

        }
    }
}
