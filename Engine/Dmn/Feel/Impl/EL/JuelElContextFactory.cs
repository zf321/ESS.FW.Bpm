using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Juel;
using ESS.FW.Bpm.Engine.Variable.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.EL
{
    public class JuelElContextFactory
    {
        protected ELResolver resolver;

        public JuelElContextFactory(ELResolver resolver)
        {
            this.resolver = resolver;
        }

        public ELContext CreateElContext(IVariableContext variableContext)
        {
            SimpleContext elContext = new SimpleContext(resolver);
            elContext.PutContext(typeof(IVariableContext), variableContext);
            return elContext;
        }
    }
}
