using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Autofac.Features.AttributeFilters;

using ESS.FW.Bpm.Engine.Impl.Cfg;
using ESS.FW.Bpm.Engine.Task;
using Microsoft.Extensions.Logging;
using ESS.FW.DataAccess;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Impl
{
    /// <summary>
    ///     
    /// </summary>
    [Component]
    public class CommentManager : AbstractHistoricManagerNet<CommentEntity>, ICommentManager
    {
        public CommentManager(DbContext dbContex, ILoggerFactory loggerFactory, IDGenerator idGenerator) : base(dbContex, loggerFactory, idGenerator)
        {
        }

        public override void Delete(CommentEntity dbEntity)
        {
            CheckHistoryEnabled();
            base.Delete(dbEntity);
        }

        public void Insert(CommentEntity dbEntity)
        {
            CheckHistoryEnabled();
            Add(dbEntity);
        }

        public virtual IList<IComment> FindCommentsByTaskId(string taskId)
        {
            CheckHistoryEnabled();
            //return ListExt.ConvertToListT<IComment>(DbEntityManager.SelectList("selectCommentsByTaskId", taskId));
            return Find(m => m.TaskId == taskId).ToList().Cast<IComment>().ToList();
        }

        public virtual IList<IEvent> FindEventsByTaskId(string taskId)
        {
            CheckHistoryEnabled();

            //ListQueryParameterObject query = new ListQueryParameterObject();
            //query.Parameter = taskId;
            //query.OrderingProperties.Add(new QueryOrderingProperty(new QueryPropertyImpl("TIME_"), Direction.Descending));

            //return ListExt.ConvertToListT<IEvent>(DbEntityManager.SelectList("selectEventsByTaskId", query)) ;
            return Find(m => m.TaskId == taskId)
                .OrderByDescending(m => m.Time)
                .ToList().Cast<IEvent>().ToList();
        }

        public virtual void DeleteCommentsByTaskId(string taskId)
        {
            CheckHistoryEnabled();
            //DbEntityManager.Delete(typeof(CommentEntity), "deleteCommentsByTaskId", taskId);
            Delete(m => m.TaskId == taskId);
        }

        public virtual IList<IComment> FindCommentsByProcessInstanceId(string processInstanceId)
        {
            CheckHistoryEnabled();
            //return ListExt.ConvertToListT<IComment>(DbEntityManager.SelectList("selectCommentsByProcessInstanceId", processInstanceId)) ;
            return Find(m => m.ProcessInstanceId == processInstanceId).ToList().Cast<IComment>().ToList();
        }

        public virtual CommentEntity FindCommentByTaskIdAndCommentId(string taskId, string commentId)
        {
            CheckHistoryEnabled();

            //IDictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters["taskId"] = taskId;
            //parameters["id"] = commentId;

            //return (CommentEntity) DbEntityManager.SelectOne("selectCommentByTaskIdAndCommentId", parameters);
            return First(m => m.TaskId == taskId && m.Id == commentId);
        }
    }
}