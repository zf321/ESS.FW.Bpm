using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Variable.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public interface IElContextFactory
    {
        /**
   * Create a { @link ELContext } for the given { @link ExpressionFactory }
        and {@link VariableContext}.
   *
   * @param expressionFactory the { @link ExpressionFactory }
    to use
   * @param variableContext the { @link VariableContext }
    to use
   * @return the {@link ELContext}
instance
*/
        ELContext CreateContext(ExpressionFactory expressionFactory, IVariableContext variableContext);
    }
}
