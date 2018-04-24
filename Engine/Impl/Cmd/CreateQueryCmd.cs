using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Impl.Interceptor;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using ESS.FW.Bpm.Engine.Task;
using ESS.FW.DataAccess;
using System.Collections.Generic;
using Autofac.Features.Metadata;

using ESS.FW.DataAccess.EF;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Impl.Cmd
{
    /// <summary>
    ///     
    /// </summary>
    [Serializable]
    public class CreateQueryCmd<T> : ICommand<IQueryable<T>> where T : class, IDbEntity, new()
    {
        private const long SerialVersionUid = 1L;

        protected internal Expression<Func<T, bool>> Expression;

        public CreateQueryCmd(Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                this.Expression = arg => true;
            else
            {
                this.Expression = expression;
            }
        }

        public IQueryable<T> Execute(CommandContext commandContext)
        {
            // Todo: CommandContext.Scope.Resolve<IEnumerable<Meta<IRepository<T,string>>>>()  Resolve不出来对象。Register & Resolve不匹配
            //var reps = commandContext.Scope.Resolve<IEnumerable<Meta<IRepository<T, string>>>>().ToList();
            //var rep = reps.FirstOrDefault(c => c.Metadata.Any(d => d.Key == "DbType" && d.Value is DbTypes.Bpm))?.Value;

            //if(!RelationshipManager.Contain(typeof(T)))
            //    {
            //    RelationshipManager.Register(commandContext.DbContext, typeof(T));
            //}
            //var relations = RelationshipManager.GetEntityProperties(typeof(T));
            //foreach(var r in relations)
            //{
            //    rep.Include(r.PropertyName);
            //}


            //return rep?.Find(Expression);
            try
            {
                IQueryable<T> rep = commandContext.DbContext.Set<T>();
                //if (!ESS.FW.DataAccess.EF.RelationshipManager.Contain(typeof(T)))
                //{
                //    ESS.FW.DataAccess.EF.RelationshipManager.Register(commandContext.DbContext, typeof(T));
                //}
                //var relations = ESS.FW.DataAccess.EF.RelationshipManager.GetEntityProperties(typeof(T));
                //foreach (var r in relations)
                //{
                //    rep = rep.Include(r.PropertyName);
                //}


                return rep?.Where(Expression);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}