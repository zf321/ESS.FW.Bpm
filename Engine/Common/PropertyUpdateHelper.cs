using ESS.Shared.Entities.Bpm;
using ESS.FW.Bpm.Engine.Impl;
using ESS.FW.Bpm.Engine.Impl.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Common
{
    public class PropertyUpdateHelper
    {
        /// <summary>
        /// 更新Entity除id外所有属性值
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="db"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TSource Update<TSource, TModel>(TSource db, TModel model) where TSource : IDbEntity where TModel : IDbEntity
        {
            if (db.Id != model.Id)
            {
                throw new System.Exception($"id不相同,db:{typeof(TSource).Name} id:{db.Id} model:{typeof(TModel).Name} id:{model.Id}");
            }
            Type dbT = typeof(TSource);
            Type modelT = typeof(TModel);
            foreach (PropertyInfo item in dbT.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (item.Name == "Id") continue;
                PropertyInfo modelProperty = modelT.GetProperty(item.Name, BindingFlags.Public | BindingFlags.Instance);
                if (modelProperty != null && item.GetMethod != null && item.SetMethod != null && modelProperty.GetMethod != null)
                {
                    var resultVal = modelProperty.GetValue(model);
                    if (resultVal != null)
                        item.SetValue(db, modelProperty.GetValue(model));
                }

            }
            return db;
        }
        /// <summary>
        /// 更新Entity除id外所有属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T UpDate<T>(T db, T model) where T : IDbEntity
        {
            if (db == null)
            {
                EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;
                Log.LogDebug("更新数据失败", string.Format("EF缓存/数据库中找不到{0} Id:{1}", typeof(T).Name, model.Id));
                throw new System.Exception(string.Format("更新数据失败 EF缓存/数据库中找不到{0} Id:{1}", typeof(T).Name, model.Id));
            }
            if (db.Id != model.Id) throw new System.Exception($"id不同 {typeof(T).Name} db.id:{db.Id} model.id{model.Id}");
            Type t = typeof(T);
            foreach (PropertyInfo item in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (item.Name == "Id") continue;
                if (item.GetMethod != null && item.SetMethod != null)
                {
                    try
                    {
                        var resultVal = item.GetValue(model);
                        if (resultVal != null)
                            item.SetValue(db, item.GetValue(model));
                    }
                    catch (ArgumentException e)
                    {
                        throw new ArgumentException($"Update异常:类型 {t.Name } 属性 {item.Name}", e);
                    }
                }
            }
            return db;
        }
    }
}
