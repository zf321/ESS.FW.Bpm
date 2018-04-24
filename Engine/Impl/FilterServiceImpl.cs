using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ESS.FW.Bpm.Engine.Filter;
using ESS.FW.Bpm.Engine.Impl.Cmd;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Query;
using ESS.FW.Bpm.Model.Bpmn;
using ESS.FW.Bpm.Model.Xml.instance;

namespace ESS.FW.Bpm.Engine.Impl
{
    /// <summary>
    ///     
    /// </summary>
    public class FilterServiceImpl : ServiceImpl, IFilterService
    {
        public virtual IFilter NewTaskFilter()
        {
            return CommandExecutor.Execute(new CreateFilterCmd(EntityTypes.Task));
        }

        public virtual IFilter NewTaskFilter(string filterName)
        {
            return NewTaskFilter().SetName(filterName);
        }

        public virtual IQueryable<IFilter> CreateFilterQuery(Expression<Func<FilterEntity, bool>> expression)
        {
            return CommandExecutor.Execute(new CreateQueryCmd<FilterEntity>(expression));
        }

        public virtual IQueryable<IFilter> CreateTaskFilterQuery()
        {
            Expression<Func<FilterEntity, bool>> expression = c => c.ResourceType == EntityTypes.Task;
            return CommandExecutor.Execute(new CreateQueryCmd<FilterEntity>(expression));
        }

        public virtual IFilter SaveFilter(IFilter filter)
        {
            return CommandExecutor.Execute(new SaveFilterCmd(filter));
        }

        public virtual IFilter GetFilter(string filterId)
        {
            return CommandExecutor.Execute(new GetFilterCmd(filterId));
        }

        public virtual void DeleteFilter(string filterId)
        {
            CommandExecutor.Execute(new DeleteFilterCmd(filterId));
        }
        
        public virtual T SingleResult<T>(string filterId)
        {
            return (T) CommandExecutor.Execute(new ExecuteFilterSingleResultCmd(filterId));
        }

        public virtual long? Count(string filterId)
        {
            return CommandExecutor.Execute(new ExecuteFilterCountCmd(filterId));
        }

        List<T> IFilterService.List<T>(string filterId)
        {
            throw new NotImplementedException();
        }

        public long? Count<T1>(string filterId, IQuery<T1> extendingQuery) where T1 : IModelElementInstance
        {
            throw new NotImplementedException();
        }
        
        public virtual IList<T> List<T>(string filterId)
        {
            return (IList<T>) CommandExecutor.Execute(new ExecuteFilterListCmd(filterId));
        }
        
        //public virtual IList<T> List<T, TQ>(string filterId, TQ extendingQuery) /*where TQ : IQuery<object, object>*/
        //{
        //    return (IList<T>) CommandExecutor.Execute(new ExecuteFilterListCmd(filterId, extendingQuery));
        //}
        
        public virtual IList<T> ListPage<T>(string filterId, int firstResult, int maxResults)
        {
            return (IList<T>) CommandExecutor.Execute(new ExecuteFilterListPageCmd(filterId, firstResult, maxResults));
        }
        
        //public virtual IList<T> ListPage<T, TQ>(string filterId, TQ extendingQuery, int firstResult, int maxResults)
        //    //where TQ : IQuery<object, object>
        //{
        //    return
        //        (IList<T>)
        //        CommandExecutor.Execute(new ExecuteFilterListPageCmd(filterId, extendingQuery, firstResult,
        //            maxResults));
        //}
        
        //public virtual T SingleResult<T, TQ>(string filterId, TQ extendingQuery) where TQ : IQuery<object, object>
        //{
        //    return (T) CommandExecutor.Execute(new ExecuteFilterSingleResultCmd(filterId, extendingQuery));
        //}

        //public virtual long? Count(string filterId, IQuery<object, object> extendingQuery)
        //{
        //    return CommandExecutor.Execute(new ExecuteFilterCountCmd(filterId, extendingQuery));
        //}
    }
}