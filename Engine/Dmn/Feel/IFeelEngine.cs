using ESS.FW.Bpm.Engine.Variable.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel
{
    public interface IFeelEngine
    {
        /**
   * Evaluate a FEEL simple expression
   *
   * @param simpleExpression the simple expression to evaluate
   * @param variableContext the variable context which are available
   * @param <T> the expected return type
   * @return the result of the simple expression
   *
   * @throws FeelException
   *           if the expression cannot be evaluated
   */
        T EvaluateSimpleExpression<T>(String simpleExpression, IVariableContext variableContext);

        /**
         * Evaluate a FEEL simple unary tests expression
         *
         * @param simpleUnaryTests the simple unary tests expression to evaluate
         * @param inputName the name of the variable which is tested
         * @param variableContext the variable context are available
         * @return the result of the simple unary tests expression
         *
         * @throws FeelException
         *           if the expression cannot be evaluated
         */
        bool EvaluateSimpleUnaryTests(String simpleUnaryTests, String inputName, IVariableContext variableContext);

    }
}
