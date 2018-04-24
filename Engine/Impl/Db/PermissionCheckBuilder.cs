using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public class PermissionCheckBuilder
    {
        protected internal IList<PermissionCheck> atomicChecks = new List<PermissionCheck>();
        protected internal IList<CompositePermissionCheck> CompositeChecks = new List<CompositePermissionCheck>();
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
        protected internal bool disjunctive_Renamed = true;

        protected internal PermissionCheckBuilder parent;

        public PermissionCheckBuilder()
        {
        }

        public PermissionCheckBuilder(PermissionCheckBuilder parent)
        {
            this.parent = parent;
        }

        public virtual IList<PermissionCheck> AtomicChecks
        {
            get { return atomicChecks; }
        }

        public virtual PermissionCheckBuilder Disjunctive()
        {
            disjunctive_Renamed = true;
            return this;
        }

        public virtual PermissionCheckBuilder Conjunctive()
        {
            disjunctive_Renamed = false;
            return this;
        }

        public virtual PermissionCheckBuilder AtomicCheck(Resources resource, string queryParam, Permissions permission)
        {
            var permCheck = new PermissionCheck();
            permCheck.Resource = resource;
            permCheck.ResourceIdQueryParam = queryParam;
            permCheck.Permission = permission;
            atomicChecks.Add(permCheck);

            return this;
        }

        public virtual PermissionCheckBuilder AtomicCheckForResourceId(Resources resource, string resourceId,
            Permissions permission)
        {
            var permCheck = new PermissionCheck();
            permCheck.Resource = resource;
            permCheck.ResourceId = resourceId;
            permCheck.Permission = permission;
            atomicChecks.Add(permCheck);

            return this;
        }

        public virtual PermissionCheckBuilder Composite()
        {
            return new PermissionCheckBuilder(this);
        }

        public virtual PermissionCheckBuilder Done()
        {
            parent.CompositeChecks.Add(Build());
            return parent;
        }

        public virtual CompositePermissionCheck Build()
        {
            Validate();

            var permissionCheck = new CompositePermissionCheck(disjunctive_Renamed);
            permissionCheck.AtomicChecks = atomicChecks;
            permissionCheck.CompositeChecks = CompositeChecks;

            return permissionCheck;
        }

        protected internal virtual void Validate()
        {
            if ((atomicChecks.Count > 0) && (CompositeChecks.Count > 0))
                throw new ProcessEngineException(
                    "Mixed authorization checks of atomic and composite permissions are not supported");
        }
    }
}