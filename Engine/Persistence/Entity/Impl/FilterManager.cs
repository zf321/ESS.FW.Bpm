using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{

    /// <summary>
    /// 
    /// </summary>
    [Component]
    public class FilterManager : AbstractManagerNet<FilterEntity>, IFilterManager
    {
        public FilterManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }
        private string ANY = "*";
        public virtual IFilter CreateNewFilter(string resourceType)
        {
            CheckAuthorization(Permissions.Create,Resources.Filter, ANY);
            return new FilterEntity(resourceType);
        }

        public virtual IFilter InsertOrUpdateFilter(IFilter filter)
        {
            throw new NotImplementedException();
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: org.camunda.bpm.engine.impl.AbstractQuery<?, ?> query = filter.getQuery();
            //AbstractQuery<> ?, ?> query = filter.Query;
            //query.validate(QueryValidators.StoredQueryValidator<>.get());

            //if (filter.Id == null)
            //{
            //    checkAuthorization(CREATE, FILTER, ANY);
            //    DbEntityManager.Insert((FilterEntity) filter);
            //    CreateDefaultAuthorizations(filter);
            //}
            //else
            //{
            //    checkAuthorization(UPDATE, FILTER, filter.Id);
            //    DbEntityManager.Merge((FilterEntity) filter);
            //}

            //return filter;
        }

        public virtual void DeleteFilter(string filterId)
        {
            throw new NotImplementedException();
            //checkAuthorization(DELETE, FILTER, filterId);

            //FilterEntity filter = FindFilterByIdInternal(filterId);
            //ensureNotNull("No filter found for filter id '" + filterId + "'", "filter", filter);

            //// delete all authorizations for this filter id
            //deleteAuthorizations(FILTER, filterId);
            //// delete the filter itself
            //DbEntityManager.Delete(filter);
        }

        public virtual FilterEntity FindFilterById(string filterId)
        {
            throw new NotImplementedException();
            //ensureNotNull("Invalid filter id", "filterId", filterId);
            //checkAuthorization(READ, FILTER, filterId);
            //return FindFilterByIdInternal(filterId);
        }

        protected internal virtual FilterEntity FindFilterByIdInternal(string filterId)
        {
            //return DbEntityManager.SelectById< FilterEntity>(typeof(FilterEntity), filterId);
            return Get(filterId);
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.Util.List<org.camunda.bpm.engine.filter.Filter> findFiltersByQueryCriteria(org.camunda.bpm.engine.impl.filter.FilterQueryImpl filterQuery)
        //public virtual IList<IFilter> FindFiltersByQueryCriteria(FilterQueryImpl filterQuery)
        //{
        //    throw new NotImplementedException();
        //    //configureQuery(filterQuery, FILTER);
        //    //return DbEntityManager.selectList("selectFilterByQueryCriteria", filterQuery);
        //}

        //public virtual long FindFilterCountByQueryCriteria(FilterQueryImpl filterQuery)
        //{
        //    throw new NotImplementedException();
        //    //configureQuery(filterQuery, FILTER);
        //    //return (long?) DbEntityManager.selectOne("selectFilterCountByQueryCriteria", filterQuery);
        //}

        // authorization utils /////////////////////////////////

        protected internal virtual void CreateDefaultAuthorizations(IFilter filter)
        {
            if (AuthorizationEnabled)
            {
                throw new NotImplementedException();
                //saveDefaultAuthorizations(ResourceAuthorizationProvider.newFilter(filter));
            }
        }
    }
}