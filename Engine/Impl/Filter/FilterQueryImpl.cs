//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Filter;
//using ESS.FW.Bpm.Engine.Impl.Interceptor;
//using ESS.FW.Bpm.Engine.Impl.Util;

//namespace ESS.FW.Bpm.Engine.Impl.Filter
//{
////JAVA TO C# CONVERTER TODO ITask: This Java 'import static' statement cannot be converted to C#:
////	import static org.camunda.bpm.engine.impl.Util.EnsureUtil.ensureNotNull;

//    /// <summary>
//    ///     
//    /// </summary>
//    [Serializable]
//    public class FilterQueryImpl : AbstractQuery<IQueryable<IFilter>,IFilter>, IQueryable<IFilter>
//    {
//        private const long SerialVersionUid = 1L;
////JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
//        protected internal string FilterIdRenamed;
//        protected internal string Name;
//        protected internal string NameLike;
//        protected internal string Owner;
//        protected internal string ResourceType;

//        public FilterQueryImpl()
//        {
//        }

//        public FilterQueryImpl(ICommandExecutor commandExecutor) : base(commandExecutor)
//        {
//        }

//        public virtual IQueryable<IFilter> FilterId(string filterId)
//        {
//            EnsureUtil.EnsureNotNull("filterId", filterId);
//            FilterIdRenamed = filterId;
//            return this;
//        }

//        public virtual IQueryable<IFilter> FilterResourceType(string resourceType)
//        {
//            EnsureUtil.EnsureNotNull("resourceType", resourceType);
//            this.ResourceType = resourceType;
//            return this;
//        }

//        public virtual IQueryable<IFilter> FilterName(string name)
//        {
//            EnsureUtil.EnsureNotNull("name", name);
//            this.Name = name;
//            return this;
//        }

//        public virtual IQueryable<IFilter> FilterNameLike(string nameLike)
//        {
//            EnsureUtil.EnsureNotNull("nameLike", nameLike);
//            this.NameLike = nameLike;
//            return this;
//        }

//        public virtual IQueryable<IFilter> FilterOwner(string owner)
//        {
//            EnsureUtil.EnsureNotNull("owner", owner);
//            this.Owner = owner;
//            return this;
//        }

//        public virtual IQueryable<IFilter> OrderByFilterId()
//        {
//            return OrderBy(FilterQueryPropertyFields.FilterId);
//        }

//        public virtual IQueryable<IFilter> OrderByFilterResourceType()
//        {
//            return OrderBy(FilterQueryPropertyFields.ResourceType);
//        }

//        public virtual IQueryable<IFilter> OrderByFilterName()
//        {
//            return OrderBy(FilterQueryPropertyFields.Name);
//        }

//        public virtual IQueryable<IFilter> OrderByFilterOwner()
//        {
//            return OrderBy(FilterQueryPropertyFields.Owner);
//        }

//        public override IList<IFilter> ExecuteList(CommandContext commandContext, Page page)
//        {
//            //return commandContext.FilterManager.findFiltersByQueryCriteria(this);
//            return null;
//        }

//        public override long ExecuteCount(CommandContext commandContext)
//        {
//            //return commandContext.FilterManager.findFilterCountByQueryCriteria(this);
//            return 0;
//        }
//    }
//}