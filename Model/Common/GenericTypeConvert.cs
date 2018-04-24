using System.Collections.Generic;

namespace ESS.FW.Bpm.Model.Common
{
    public class GenericTypeConvert
    {
        /// <summary>
        /// IList<T>转换工具(创建了新对象，影响性能，用于Java源码转换不好实现的地方)
        /// </summary>
        /// <typeparam name="TSource">数据源，一般是implClass</typeparam>
        /// <typeparam name="TOut">基类，一般是接口</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<TOut> Convert<TSource, TOut>(IList<TSource> source)where TSource:TOut
        {
            IList<TOut> result = new List<TOut>();
            foreach(var item in source)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
