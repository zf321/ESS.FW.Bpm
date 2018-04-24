using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelSyntaxException : FeelException
    {
        protected String feelExpression;
        protected String description;
        public FeelSyntaxException(String message, String feelExpression, String description):base(message)
        {
            this.feelExpression = feelExpression;
            this.description = description;
        }
        public FeelSyntaxException(String message, String feelExpression, String description, System.Exception cause):base(message,cause)
        {
            this.feelExpression = feelExpression;
            this.description = description;
        }
        public String GetFeelExpression()
        {
            return this.feelExpression;
        }

        public String GetDescription()
        {
            return this.description;
        }
    }
}
