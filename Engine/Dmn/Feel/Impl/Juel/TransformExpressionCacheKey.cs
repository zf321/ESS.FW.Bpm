using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class TransformExpressionCacheKey
    {
        protected String expression;
        protected String inputName;

        public TransformExpressionCacheKey(String expression, String inputName)
        {
            this.expression = expression;
            this.inputName = inputName;
        }
        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + ((expression == null) ? 0 : expression.GetHashCode());
            result = prime * result + ((inputName == null) ? 0 : inputName.GetHashCode());
            return result;
        }


        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            TransformExpressionCacheKey other = (TransformExpressionCacheKey)obj;
            if (expression == null)
            {
                if (other.expression != null)
                    return false;
            }
            else if (!expression.Equals(other.expression))
                return false;
            if (inputName == null)
            {
                if (other.inputName != null)
                    return false;
            }
            else if (!inputName.Equals(other.inputName))
                return false;
            return true;
        }


        public override String ToString()
        {
            return "TransformExpressionCacheKey [expression=" + expression + ", inputName=" + inputName + "]";
        }
    }
}
