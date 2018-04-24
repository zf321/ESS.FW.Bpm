using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.History.Impl.Event;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using HistoricDetailEventEntity = ESS.FW.Bpm.Engine.History.Impl.Event.HistoricDetailEventEntity;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache
{
    /// <summary>
    ///     Provides the keys used by <seealso cref="DbEntityCache" /> for organizing the different
    ///     <seealso cref="IDbEntity" /> types.
    ///     Especially for polymorphic types, it is important that they are accessible in the cache under one
    ///     common key such that querying the cache with a superclass or with a subclass both return the cached
    ///     entities.
    ///     
    /// </summary>
    public class DbEntityCacheKeyMapping
    {
        protected internal IDictionary<Type, Type> EntityCacheKeys;

        public DbEntityCacheKeyMapping()
        {
            EntityCacheKeys = new Dictionary<Type, Type>();
        }

        public virtual Type GetEntityCacheKey(Type entityType)
        {
            Type entityCacheKey = null;
            if (EntityCacheKeys.ContainsKey(entityType))
            {
                entityCacheKey = EntityCacheKeys[entityType];
            }
            if (entityCacheKey == null)
                return entityType;

            return entityCacheKey;
        }

        public virtual void RegisterEntityCacheKey(Type entityType, Type cacheKey)
        {
            EntityCacheKeys[entityType] = cacheKey;
        }
        /// <summary>
        /// 数据实体类型配置信息
        /// </summary>
        /// <returns></returns>
        public static DbEntityCacheKeyMapping DefaultEntityCacheKeyMapping()
        {
            var mapping = new DbEntityCacheKeyMapping();
            // subclasses of JobEntity
            mapping.RegisterEntityCacheKey(typeof(MessageEntity), typeof(JobEntity));
            mapping.RegisterEntityCacheKey(typeof (TimerEntity), typeof (JobEntity));
            // subclasses of HistoricDetailEventEntity
            mapping.RegisterEntityCacheKey(typeof (HistoricFormPropertyEntity), typeof (HistoricDetailEventEntity));
            mapping.RegisterEntityCacheKey(typeof(HistoricFormPropertyEventEntity), typeof(HistoricDetailEventEntity));
            mapping.RegisterEntityCacheKey(typeof(HistoricVariableUpdateEventEntity),
                typeof(HistoricDetailEventEntity));
            mapping.RegisterEntityCacheKey(typeof (HistoricVariableUpdateEventEntity),typeof (HistoricDetailEventEntity));
            return mapping;
        }

        public static DbEntityCacheKeyMapping EmptyMapping()
        {
            return new DbEntityCacheKeyMapping();
        }
    }
}