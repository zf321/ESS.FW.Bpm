using ESS.Shared.Entities.Bpm;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Cache
{


    /// <summary>
    ///     A simple first level cache for <seealso cref="IDbEntity" />.
    ///     
    /// </summary>
    public class DbEntityCache
    {
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

        /// <summary>
        ///     The cache itself: maps entity types (classes) to maps indexed by id (primary key).
        ///     The motivation for indexing by type (class) is
        ///     a) multiple entities of different types could have the same value as primary key. In the
        ///     process engine, TaskEntity and HistoricTaskEntity have the same id value.
        ///     b) performance (?)
        /// </summary>
        protected internal IDictionary<Type, IDictionary<string, CachedDbEntity>> CachedEntites =
            new Dictionary<Type, IDictionary<string, CachedDbEntity>>();

        protected internal DbEntityCacheKeyMapping CacheKeyMapping;

        public DbEntityCache()
        {
            CacheKeyMapping = DbEntityCacheKeyMapping.EmptyMapping();
            Log.LogDebug("缓存无参构造函数", "DbEntityCacheKeyMapping.EmptyMapping");
        }

        public DbEntityCache(DbEntityCacheKeyMapping cacheKeyMapping)
        {
            this.CacheKeyMapping = cacheKeyMapping;
            string test = string.Empty;
            foreach (var item in CacheKeyMapping.EntityCacheKeys)
            {
                test += $"key:{item.Key.Name},value:{item.Value.Name};";
            }
            Log.LogDebug("缓存构造函数", test);
        }
        public void Clear()
        {
            CachedEntites.Clear();
        }

        public virtual IList<CachedDbEntity> CachedEntities
        {
            get
            {
                IList<CachedDbEntity> result = new List<CachedDbEntity>();
                foreach (var typeCache in CachedEntites.Values)
                    ((List<CachedDbEntity>)result).AddRange(typeCache.Values);
                return result;
            }
        }

        /// <summary>
        ///     Sets an object to a deleted state. It will not be removed from the cache but
        ///     transition to one of the DELETED states, depending on it's current state.
        /// </summary>
        /// <param name="dbEntity"> the object to mark deleted. </param>
        public virtual IDbEntity Deleted
        {
            set
            {
                var cachedEntity = GetCachedEntity(value);
                if (cachedEntity != null)
                {
                    if (cachedEntity.EntityState == DbEntityState.Transient)
                        cachedEntity.EntityState = DbEntityState.DeletedTransient;
                    else if (cachedEntity.EntityState == DbEntityState.Persistent)
                        cachedEntity.EntityState = DbEntityState.DeletedPersistent;
                    else if (cachedEntity.EntityState == DbEntityState.Merged)
                        cachedEntity.EntityState = DbEntityState.DeletedMerged;
                }
                else
                {
                    // put a deleted merged into the cache
                    var cachedDbEntity = new CachedDbEntity();
                    cachedDbEntity.Entity = value;
                    cachedDbEntity.EntityState = DbEntityState.DeletedMerged;
                    PutInternal(cachedDbEntity);
                }
            }
        }

        /// <summary>
        ///     get an object from the cache
        /// </summary>
        /// <param name="type"> the type of the object </param>
        /// <param name="id"> the id of the object </param>
        /// <returns> the object or 'null' if the object is not in the cache </returns>
        /// <exception cref="ProcessEngineException"> if an object for the given id can be found but is of the wrong type. </exception>
        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.impl.db.DbEntity> T get(Class<T> type, String id)
        public virtual T Get<T>(Type type, string id) where T : IDbEntity
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(type);
            var cachedDbEntity = GetCachedEntity(cacheKey, id);
            if (cachedDbEntity != null)
            {
                var dbEntity = cachedDbEntity.Entity;
                try
                {
                    return (T)dbEntity;
                }
                catch (InvalidCastException e)
                {
                    //throw e;
                    throw Log.EntityCacheLookupException<T>(type, id, dbEntity.GetType(), e);
                }
            }
            return default(T);
        }
        public virtual T Get<T>(string id) where T : IDbEntity
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(typeof(T));
            var cachedDbEntity = GetCachedEntity(cacheKey, id);
            if (cachedDbEntity != null)
            {
                var dbEntity = cachedDbEntity.Entity;
                try
                {
                    return (T)dbEntity;
                }
                catch (InvalidCastException e)
                {
                    //throw e;
                    throw Log.EntityCacheLookupException<T>(typeof(T), id, dbEntity.GetType(), e);
                }
            }
            return default(T);
        }
        //JAVA TO C# CONVERTER TODO ITask: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.impl.db.DbEntity> java.Util.List<T> getEntitiesByType(Class<T> type)
        public virtual IList<T> GetEntitiesByType<T>(Type type) where T : IDbEntity
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(type);
            var entities = CachedEntites.ContainsKey(cacheKey) ? CachedEntites[cacheKey] : null;
            IList<T> result = new List<T>();
            if (entities == null)
                return new List<T>();
            foreach (var cachedEntity in entities.Values)
                if (type != cacheKey)
                {
                    // if the cacheKey of this type differs from the actual type,
                    // not all cached entites with the key should be returned.
                    // Then we only add those entities whose type matches the argument type.
                    if (type.IsAssignableFrom(cachedEntity.GetType()))
                        result.Add((T)cachedEntity.Entity);
                }
                else
                {
                    result.Add((T)cachedEntity.Entity);
                }
            return result;
        }
        public virtual IList<T> GetEntitiesByType<T>() where T : IDbEntity
        {
            Type type = typeof(T);
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(type);
            var entities = CachedEntites.ContainsKey(cacheKey) ? CachedEntites[cacheKey] : null;
            IList<T> result = new List<T>();
            if (entities == null)
                return new List<T>();
            foreach (var cachedEntity in entities.Values)
                if (type != cacheKey)
                {
                    // if the cacheKey of this type differs from the actual type,
                    // not all cached entites with the key should be returned.
                    // Then we only add those entities whose type matches the argument type.
                    if (type.IsAssignableFrom(cachedEntity.GetType()))
                        result.Add((T)cachedEntity.Entity);
                }
                else
                {
                    result.Add((T)cachedEntity.Entity);
                }
            return result;
        }

        /// <summary>
        ///     Looks up an entity in the cache.
        /// </summary>
        /// <param name="type"> the type of the object </param>
        /// <param name="id"> the id of the CachedEntity to lookup </param>
        /// <returns> the cached entity or null if the entity does not exist. </returns>
        public virtual CachedDbEntity GetCachedEntity(Type type, string id)
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(type);
            var entitiesByType = CachedEntites.ContainsKey(cacheKey) ? CachedEntites[cacheKey] : null;
            if (entitiesByType != null)
                return entitiesByType.ContainsKey(id) ? entitiesByType[id] : null;
            return null;
        }
        /// <summary>
        /// 直接返回T（CachedDbEntity.Entity） 而不是CachedDbEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetCachedEntity<T>(string id) where T : class, IDbEntity
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(typeof(T));
            var entitiesByType = CachedEntites.ContainsKey(cacheKey) ? CachedEntites[cacheKey] : null;
            if (entitiesByType != null)
                return entitiesByType.ContainsKey(id) ? entitiesByType[id].Entity as T : null;
            return null;
        }

        /// <summary>
        ///     Looks up an entity in the cache.
        /// </summary>
        /// <param name="dbEntity"> the entity for which the CachedEntity should be looked up </param>
        /// <returns> the cached entity or null if the entity does not exist. </returns>
        public virtual CachedDbEntity GetCachedEntity(IDbEntity dbEntity)
        {
            return GetCachedEntity(dbEntity.GetType(), dbEntity.Id);
        }

        /// <summary>
        ///     Put a new, <seealso cref="DbEntityState#TRANSIENT" /> object into the cache.
        /// </summary>
        /// <param name="e"> the object to put into the cache </param>
        public virtual void PutTransient(IDbEntity e)
        {
            var cachedDbEntity = new CachedDbEntity();
            cachedDbEntity.Entity = e;
            cachedDbEntity.EntityState = DbEntityState.Transient;
            PutInternal(cachedDbEntity);
        }

        /// <summary>
        ///     Put a <seealso cref="DbEntityState#PERSISTENT" /> object into the cache.
        /// </summary>
        /// <param name="e"> the object to put into the cache </param>
        public virtual void PutPersistent(IDbEntity e)
        {
            var cachedDbEntity = new CachedDbEntity();
            cachedDbEntity.Entity = e;
            cachedDbEntity.EntityState = DbEntityState.Persistent;
            cachedDbEntity.DetermineEntityReferences();
            cachedDbEntity.MakeCopy();

            PutInternal(cachedDbEntity);
        }

        /// <summary>
        ///     Put a <seealso cref="DbEntityState#MERGED" /> object into the cache.
        /// </summary>
        /// <param name="e"> the object to put into the cache </param>
        public virtual void PutMerged(IDbEntity e)
        {
            var cachedDbEntity = new CachedDbEntity();
            cachedDbEntity.Entity = e;
            cachedDbEntity.EntityState = DbEntityState.Merged;
            cachedDbEntity.DetermineEntityReferences();
            // no copy required

            PutInternal(cachedDbEntity);
        }

        protected internal virtual void PutInternal(CachedDbEntity entityToAdd)
        {
            var type = entityToAdd.Entity.GetType();
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(type);

            IDictionary<string, CachedDbEntity> map = CachedEntites.ContainsKey(cacheKey) ? CachedEntites[cacheKey] : null;

            if (map == null)
            {
                map = new Dictionary<string, CachedDbEntity>();
                CachedEntites[cacheKey] = map;
            }
            if (entityToAdd.Entity.Id == null)
            {
                entityToAdd.Entity.Id = context.Impl.Context.CommandContext.Scope.Resolve<IDGenerator>().NextId;
            }
            
            // check whether this object is already present in the cache
            var existingCachedEntity = map.ContainsKey(entityToAdd.Entity.Id) ? map[entityToAdd.Entity.Id] : null;
            if (existingCachedEntity == null)
                map[entityToAdd.Entity.Id] = entityToAdd;
            else
                switch (entityToAdd.EntityState)
                {
                    case DbEntityState.Transient:
                        // cannot put TRANSIENT entity if entity with same id already exists in cache.
                        if (existingCachedEntity.EntityState == DbEntityState.Transient)
                            throw Log.EntityCacheDuplicateEntryException("TRANSIENT", entityToAdd.Entity.Id,
                                entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);
                        throw Log.AlreadyMarkedEntityInEntityCacheException(entityToAdd.Entity.Id,
                            entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);

                    case DbEntityState.Persistent:
                        if (existingCachedEntity.EntityState == DbEntityState.Persistent)
                        {
                            // use new entity state, replacing the existing one.
                            map[entityToAdd.Entity.Id] = entityToAdd;
                            break;
                        }
                        if ((existingCachedEntity.EntityState == DbEntityState.DeletedPersistent) ||
                            (existingCachedEntity.EntityState == DbEntityState.DeletedMerged))
                            break;

                        // otherwise fail:
                        throw Log.EntityCacheDuplicateEntryException("PERSISTENT", entityToAdd.Entity.Id,
                            entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);

                    case DbEntityState.Merged:
                        if ((existingCachedEntity.EntityState == DbEntityState.Persistent) ||
                            (existingCachedEntity.EntityState == DbEntityState.Merged))
                        {
                            // use new entity state, replacing the existing one.
                            map[entityToAdd.Entity.Id] = entityToAdd;
                            break;
                        }
                        if ((existingCachedEntity.EntityState == DbEntityState.DeletedPersistent) ||
                            (existingCachedEntity.EntityState == DbEntityState.DeletedMerged))
                            break;

                        // otherwise fail:
                        throw Log.EntityCacheDuplicateEntryException("MERGED", entityToAdd.Entity.Id,
                            entityToAdd.Entity.GetType(), existingCachedEntity.EntityState);

                    default:
                        // deletes are always added
                        map[entityToAdd.Entity.Id] = entityToAdd;
                        break;
                }
        }

        /// <summary>
        ///     Remove an entity from the cache
        /// </summary>
        /// <param name="e">
        ///     the entity to remove
        ///     @return
        /// </param>
        public virtual bool Remove(IDbEntity e)
        {
            var cacheKey = CacheKeyMapping.GetEntityCacheKey(e.GetType());
            var typeMap = CachedEntites[cacheKey];
            if (typeMap != null)
                return typeMap.Remove(e.Id) != null;
            return false;
        }

        /// <param name="cachedDbEntity"> </param>
        public virtual void Remove(CachedDbEntity cachedDbEntity)
        {
            Remove(cachedDbEntity.Entity);
        }

        /// <summary>
        ///     Allows checking whether the provided entity is present in the cache
        /// </summary>
        /// <param name="dbEntity"> the entity to check </param>
        /// <returns> true if the the provided entity is present in the cache </returns>
        public virtual bool Contains(IDbEntity dbEntity)
        {
            return GetCachedEntity(dbEntity) != null;
        }

        /// <summary>
        ///     Allows checking whether the provided entity is present in the cache
        ///     and is <seealso cref="DbEntityState#PERSISTENT" />.
        /// </summary>
        /// <param name="dbEntity"> the entity to check </param>
        /// <returns>
        ///     true if the provided entity is present in the cache and is
        ///     <seealso cref="DbEntityState#PERSISTENT" />.
        /// </returns>
        public virtual bool IsPersistent(IDbEntity dbEntity)
        {
            var cachedDbEntity = GetCachedEntity(dbEntity);
            if (cachedDbEntity == null)
                return false;
            return cachedDbEntity.EntityState == DbEntityState.Persistent;
        }

        /// <summary>
        ///     Allows checking whether the provided entity is present in the cache
        ///     and is marked to be deleted.
        /// </summary>
        /// <param name="dbEntity"> the entity to check </param>
        /// <returns>
        ///     true if the provided entity is present in the cache and is
        ///     marked to be deleted
        /// </returns>
        public virtual bool IsDeleted(IDbEntity dbEntity)
        {
            var cachedDbEntity = GetCachedEntity(dbEntity);
            if (cachedDbEntity == null)
                return false;
            return (cachedDbEntity.EntityState == DbEntityState.DeletedMerged) ||
                   (cachedDbEntity.EntityState == DbEntityState.DeletedPersistent) ||
                   (cachedDbEntity.EntityState == DbEntityState.DeletedTransient);
        }

        /// <summary>
        ///     Allows checking whether the provided entity is present in the cache
        ///     and is <seealso cref="DbEntityState#TRANSIENT" />.
        /// </summary>
        /// <param name="dbEntity"> the entity to check </param>
        /// <returns>
        ///     true if the provided entity is present in the cache and is
        ///     <seealso cref="DbEntityState#TRANSIENT" />.
        /// </returns>
        public virtual bool IsTransient(IDbEntity dbEntity)
        {
            var cachedDbEntity = GetCachedEntity(dbEntity);
            if (cachedDbEntity == null)
                return false;
            return cachedDbEntity.EntityState == DbEntityState.Transient;
        }
    }
}