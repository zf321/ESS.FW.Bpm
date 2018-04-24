using ESS.FW.Bpm.Engine.Dmn.engine.impl.spi.el;
using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.EL
{
    public class JuelExpression : IELExpression
    {
        protected ValueExpression expression;
        protected JuelElContextFactory elContextFactory;

        public JuelExpression(ValueExpression expression, JuelElContextFactory elContextFactory)
        {
            this.expression = expression;
            this.elContextFactory = elContextFactory;
        }

        public Object GetValue(IVariableContext variableContext)
        {
            ELContext elContext = elContextFactory.CreateElContext(variableContext);
            return expression.GetValue(elContext);
        }

        public override String ToString()
        {
            return "JuelExpression{" +
              "expression=" + expression +
              ", elContextFactory=" + elContextFactory +
              '}';
        }
    }
}
