using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Model.Common
{
    public static class BpmExpression
    {
        public static IQueryable<T> ParameterSelect<T>(this IQueryable<T> source, string key,string value)
        {
            return source.Where(GetParameterExpression<T>(key,value));
        }
        public static IQueryable<T> ParameterSelect<T>(this IQueryable<T> source,IDictionary<string,string> dic)
        {
            foreach (var item in dic)
            {
                source = source.Where(GetParameterExpression<T>(item.Key, item.Value));
            }
            return source;
        }
        public static  Expression<Func<T,bool>> GetParameterExpression<T>(string key,string value)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");
            Expression field = Expression.Property(parameterExpression, typeof(T).GetProperty(key));
            Expression constantExpression = ConstantExpression.Constant(value, typeof(string));
            MethodCallExpression equals = Expression.Call(field, typeof(T).GetMethod("Equals"), new Expression[] { constantExpression });
            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(equals, new ParameterExpression[]
            {
                    parameterExpression
            });
            return lambda;
        }
    }
}
