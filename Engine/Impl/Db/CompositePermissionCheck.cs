using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    /// <summary>
    ///     
    /// </summary>
    public class CompositePermissionCheck
    {
        protected internal IList<PermissionCheck> atomicChecks = new List<PermissionCheck>();

        protected internal IList<CompositePermissionCheck> compositeChecks = new List<CompositePermissionCheck>();

        protected internal bool disjunctive;

        public CompositePermissionCheck() : this(true)
        {
        }

        public CompositePermissionCheck(bool disjunctive)
        {
            this.disjunctive = disjunctive;
        }

        public virtual IList<PermissionCheck> AtomicChecks
        {
            set { atomicChecks = value; }
            get { return atomicChecks; }
        }

        public virtual IList<CompositePermissionCheck> CompositeChecks
        {
            set { compositeChecks = value; }
            get { return compositeChecks; }
        }

        /// <summary>
        ///     conjunctive else
        /// </summary>
        public virtual bool Disjunctive
        {
            get { return disjunctive; }
        }

        public virtual IList<PermissionCheck> AllPermissionChecks
        {
            get
            {
                IList<PermissionCheck> allChecks = new List<PermissionCheck>();

                ((List<PermissionCheck>) allChecks).AddRange(atomicChecks);

                foreach (var compositePermissionCheck in compositeChecks)
                    ((List<PermissionCheck>) allChecks).AddRange(compositePermissionCheck.AllPermissionChecks);

                return allChecks;
            }
        }

        public virtual void AddAtomicCheck(PermissionCheck permissionCheck)
        {
            atomicChecks.Add(permissionCheck);
        }

        public virtual void AddCompositeCheck(CompositePermissionCheck subCheck)
        {
            compositeChecks.Add(subCheck);
        }


        public virtual void Clear()
        {
            compositeChecks.Clear();
            atomicChecks.Clear();
        }
    }
}