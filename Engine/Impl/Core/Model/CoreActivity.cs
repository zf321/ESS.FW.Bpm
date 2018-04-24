using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Delegate;
using ESS.FW.Bpm.Engine.Impl.Core.Variable.mapping;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.FW.Bpm.Engine.Impl.Pvm.Delegate;

namespace ESS.FW.Bpm.Engine.Impl.Core.Model
{
    /// <summary>
    ///     
    ///     
    /// </summary>
    [Serializable]
    public abstract class CoreActivity : CoreModelElement
    {
        private const long SerialVersionUid = 1L;

        protected internal IoMapping ioMapping;

        public CoreActivity(string id) : base(id)
        {
        }

        [NotMapped]
        public abstract IList<CoreActivity> Activities { get; }
        //[NotMapped]
        //public abstract ICoreActivityBehavior<IBaseDelegateExecution> ActivityBehavior { get; set; }
        [NotMapped]
        public abstract IActivityBehavior ActivityBehavior { get; set; }
        [NotMapped]
        public virtual IoMapping IoMapping
        {
            get { return ioMapping; }
            set { ioMapping = value; }
        }

        /// <summary>
        ///     重写Activities为null返回null
        ///     searches for the activity recursively
        /// </summary>
        public virtual CoreActivity FindActivity(string activityId)
        {
            var localActivity = GetChildActivity(activityId);
            if (localActivity != null)
                return localActivity;
            if (Activities != null)
                foreach (var activity in Activities)
                {
                    var nestedActivity = activity.FindActivity(activityId);
                    if (nestedActivity != null)
                        return nestedActivity;
                }

            return null;
        }

        public virtual CoreActivity CreateActivity()
        {
            return CreateActivity(null);
        }

        /// <summary>
        ///     searches for the activity locally
        /// </summary>
        public abstract CoreActivity GetChildActivity(string activityId);

        public abstract CoreActivity CreateActivity(string activityId);


        public override string ToString()
        {
            return "Activity(" + id + ")";
        }
    }
}