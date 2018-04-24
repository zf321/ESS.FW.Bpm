using ESS.FW.Bpm.Engine.Impl.Javax.EL;
using ESS.FW.Bpm.Engine.Impl.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Dmn.Feel.Impl.Juel.El
{
    public class FeelFunctionMapper:FunctionMapper
    {
        public static readonly FeelEngineLogger LOG= FeelLogger.ENGINE_LOGGER;
        //protected internal static readonly SimpleDateFormat FEEL_DATE_AND_TIME_FORMAT;//"yyyy-MM-dd'T'HH:mm:ss"
        public const string JUEL_DATE_AND_TIME_METHOD = "dateAndTime";
        protected internal static readonly IDictionary<string, MethodInfo> methods;

        public FeelFunctionMapper()
        {
        }

        public override MethodInfo ResolveFunction(string prefix, string localName)
        {
            return methods.ContainsKey(localName)? methods[localName]:null;
        }

        protected internal static MethodInfo GetMethod(string name, params Type[] parameterTypes)
        {
            try
            {
                return ReflectUtil.GetMethod(typeof(FeelFunctionMapper), name, parameterTypes);
            }
            //TODO 反射异常类型
            catch (System.Exception var3)
            {
                throw LOG.unableToFindMethod(var3, name, parameterTypes);
            }
        }

        public static DateTime ParseDateAndTime(string dateAndTimeString)
        {
            try
            {
                //TODO 自定义时间格式
                //SimpleDateFormat clonedDateFormat = (SimpleDateFormat)FEEL_DATE_AND_TIME_FORMAT.clone();
                //return clonedDateFormat.parse(dateAndTimeString);
                return DateTime.Parse(dateAndTimeString);
            }
            catch (/*ParseException*/FormatException var2)
            {
                throw LOG.invalidDateAndTimeFormat(dateAndTimeString, var2);
            }
        }

    }
}
