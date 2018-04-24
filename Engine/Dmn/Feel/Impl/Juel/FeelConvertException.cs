using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel
{
    public class FeelConvertException:FeelException
    {
        protected Object value;
        protected Type type;
        public FeelConvertException(String message, Object value, Type type):base(message)
        {
            this.value = value;
            this.type = type;
        }

        public FeelConvertException(String message, Object value,Type type,System.Exception cause):base(message,cause)
        {
            this.value = value;
            this.type = type;
        }

        public FeelConvertException(String message, FeelConvertException convertException):base(message, convertException)
        {
            this.value = convertException.GetValue();
            this.type = convertException.GetType();
        }

        public Object GetValue()
        {
            return value;
        }

        public Type Type
        {
            get { return type; }
            
        }
    }
}
