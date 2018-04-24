using ESS.FW.Bpm.Engine.Impl;
using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.exception;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Util;
using Newtonsoft.Json;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{ 
   

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public  class FilterEntity : IFilter, IDbEntity, IHasDbRevision, IDbEntityLifecycleAware
    {

        private const long SerialVersionUid = 1L;
        protected internal static readonly EnginePersistenceLogger Log = ProcessEngineLogger.PersistenceLogger;

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public final static java.Util.Map<String, org.camunda.bpm.engine.impl.json.JsonObjectConverter<?>> queryConverter = new java.Util.HashMap<String, org.camunda.bpm.engine.impl.json.JsonObjectConverter<?>>();
        //public static readonly IDictionary<string, JsonObjectConverter> QueryConverter = new Dictionary<string, JsonObjectConverter>();

        static FilterEntity()
        {
            //QueryConverter[EntityTypes.Task] = new JsonTaskQueryConverter();
        }

        protected internal string id;
        protected internal string resourceType;
        protected internal string name;
        protected internal string owner;
        //protected internal AbstractQuery query;
        protected internal IDictionary<string, object> properties;
        protected internal int revision = 0;

        public  FilterEntity()
        {

        }

        public FilterEntity(string resourceType)
        {
            ResourceType = resourceType;
            QueryInternal = "{}";
        }

        public virtual string Id
        {
            set
            {
                this.id = value;
            }
            get
            {
                return id;
            }
        }


        public virtual IFilter SetResourceType(string resourceType)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Filter resource type must not be null or empty", "resourceType", resourceType);
            EnsureUtil.EnsureNull(typeof(NotValidException), "Cannot overwrite filter resource type", "resourceType", this.resourceType);

            this.resourceType = resourceType;
            return this;
        }

        public virtual string ResourceType
        {
            get
            {
                return resourceType;
            }
            set
            {
                this.resourceType = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        public virtual IFilter SetName(string name)
        {
            EnsureUtil.EnsureNotEmpty(typeof(NotValidException), "Filter name must not be null or empty", "name", name);
            this.name = name;
            return this;
        }

        public virtual string Owner
        {
            get
            {
                return owner;
            }
            set
            {
                this.owner = value;
            }
        }

        public virtual IFilter SetOwner(string owner)
        {
            this.owner = owner;
            return this;
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> T getQuery()
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> T getQuery()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
        //public virtual T GetQuery<T>() where T : org.camunda.bpm.engine.query.Query<?, ?>
        //{
        //    get
        //    {
        //        return (T) Query;
        //    }
        //}

        public virtual string QueryInternal
        {
            get
            {
                throw new NotImplementedException();
                //JsonObjectConverter<object> converter = Converter;
                //return converter.toJson(Query);
            }
            set
            {
                throw new NotImplementedException();
                //ensureNotNull(typeof(NotValidException), "query", value);
                //JsonObjectConverter<object> converter = Converter;
                ////JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
                ////ORIGINAL LINE: this.query = (org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) converter.toObject(new org.camunda.bpm.engine.impl.Util.json.JSONObject(value));
                //this.Query = (AbstractQuery<?, ?>) converter.toObject(new JSONObject(value));
            }
        }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: public <T extends org.camunda.bpm.engine.query.Query<?, ?>> org.camunda.bpm.engine.filter.Filter setQuery(T query)
//        public virtual IFilter SetQuery<T>(T query) where T : org.camunda.bpm.engine.query.Query<?, ?>
//        {
//            EnsureUtil.EnsureNotNull(typeof(NotValidException), "query", query);
////JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
////ORIGINAL LINE: this.query = (org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query;
//            this.Query = (AbstractQuery<?, ?>) query;
//            return this;
//        }


        public virtual IDictionary<string, object> Properties
        {
            get
            {
                
                if (properties != null)
                {
                    //return JsonUtil.jsonObjectAsMap(new JSONObject(properties));
                    return properties;
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual string PropertiesInternal
        {
            get
            {
                
                //return (new JSONObject(properties)).ToString();
                return JsonConvert.SerializeObject(properties);
            }
            set
            {
                
                //if (value != null)
                //{
                //    JSONObject jsonObject = new JSONObject(value);
                //    this.properties = JsonUtil.jsonObjectAsMap(jsonObject);
                //}
                //else
                //{
                //    this.properties = null;
                //}
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.properties = null;
                }
                properties = JsonConvert.DeserializeObject<IDictionary<string, object>>(value);
            }
        }

        public virtual IFilter SetProperties(IDictionary<string, object> properties)
        {
            this.properties = properties;
            return this;
        }


        public virtual int Revision
        {
            get
            {
                return revision;
            }
            set
            {
                this.revision = value;
            }
        }


        public virtual int RevisionNext
        {
            get
            {
                return revision + 1;
            }
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> org.camunda.bpm.engine.filter.Filter extend(T extendingQuery)
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.camunda.bpm.engine.query.Query<?, ?>> org.camunda.bpm.engine.filter.Filter extend(T extendingQuery)
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//        public virtual IFilter Extend<T>(T extendingQuery) where T : org.camunda.bpm.engine.query.Query<?, ?>
//        {
//            ensureNotNull(typeof(NotValidException), "extendingQuery", extendingQuery);

//            if (!extendingQuery.GetType().Equals(Query.GetType()))
//            {
////JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
//                throw Log.queryExtensionException(Query.GetType().FullName, extendingQuery.GetType().FullName);
//            }

//            FilterEntity copy = CopyFilter();
//            copy.Query = Query.Extend(extendingQuery);

//            return copy;
//        }

////JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
////ORIGINAL LINE: @SuppressWarnings("unchecked") protected <T> org.camunda.bpm.engine.impl.json.JsonObjectConverter<T> getConverter()
//        protected internal virtual JsonObjectConverter<T> GetConverter<T>()
//        {
//            get
//            {
//                JsonObjectConverter<T> converter = (JsonObjectConverter<T>) QueryConverter[resourceType];
//                if (converter != null)
//                {
//                    return converter;
//                }
//                else
//                {
//                    throw Log.UnsupportedResourceTypeException(resourceType);
//                }
//            }
//        }

        public virtual object GetPersistentState()
        {
                IDictionary<string, object> persistentState = new Dictionary<string, object>();
                persistentState["name"] = this.name;
                persistentState["owner"] = this.owner;
                //persistentState["query"] = this.Query;
                persistentState["properties"] = this.properties;
                return persistentState;
        }

        protected internal virtual FilterEntity CopyFilter()
        {
            FilterEntity copy = new FilterEntity(ResourceType);
            //copy.Name = Name;
            //copy.Owner = Owner;
            copy.QueryInternal = QueryInternal;
            copy.PropertiesInternal = PropertiesInternal;
            return copy;
        }

        public virtual void PostLoad()
        {
            throw new NotImplementedException();
            //if (Query != null)
            //{
            //    Query.AddValidator(QueryValidators.StoredQueryValidator<>.get());
            //}

        }

    }

}