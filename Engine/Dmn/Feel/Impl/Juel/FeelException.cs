using ESS.FW.Bpm.Engine.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelException:RuntimeException
    {
        public FeelException(string msg) : base(msg)
        {

        }
        public FeelException(string msg,System.Exception ex) : base(msg, ex)
        {

        }
    }
}
