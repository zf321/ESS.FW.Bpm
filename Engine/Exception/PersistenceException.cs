using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Exception
{
    public class PersistenceException:RuntimeException
    {
        public PersistenceException()
        {
        }

        public PersistenceException(String var1):base(var1)
        {
        }

        public PersistenceException(String var1, System.Exception var2):base(var1,var2)
        {
        }
    }
}
