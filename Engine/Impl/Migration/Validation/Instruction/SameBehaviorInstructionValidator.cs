using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.Bpmn.Behavior;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instruction
{
    public class SameBehaviorInstructionValidator : IMigrationInstructionValidator
    {
        public static readonly IList<ISet<Type>> _EquivalentBehaviors = new List<ISet<Type>>();

        protected internal IDictionary<Type, ISet<Type>> EquivalentBehaviors = new Dictionary<Type, ISet<Type>>();

        static SameBehaviorInstructionValidator()
        {
        //    EquivalentBehaviors.Add(CollectionUtil.AsHashSet(typeof(CallActivityBehavior),
        //        typeof(CaseCallActivityBehavior)));

        //    EquivalentBehaviors.Add(CollectionUtil.AsHashSet(typeof(SubProcessActivityBehavior),
                //typeof(EventSubProcessActivityBehavior)));
        }

        public SameBehaviorInstructionValidator() : this(_EquivalentBehaviors)
        {
        }

        public SameBehaviorInstructionValidator(IList<ISet<Type>> equivalentBehaviors)
        {
            //foreach (var equivalenceClass in equivalentBehaviors)
            //    foreach (var clazz in equivalenceClass)
            //        this.EquivalentBehaviors[clazz] = equivalenceClass;
        }

        public virtual void Validate(IValidatingMigrationInstruction instruction,
            ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
        {
            var sourceActivity = instruction.SourceActivity;
            var targetActivity = instruction.TargetActivity;

            var sourceBehaviorClass = sourceActivity.ActivityBehavior.GetType();
            var targetBehaviorClass = targetActivity.ActivityBehavior.GetType();

            if (!SameBehavior(sourceBehaviorClass, targetBehaviorClass))
                report.AddFailure("Activities have incompatible types " + "(" + sourceBehaviorClass.Name +
                                  " is not compatible with " + targetBehaviorClass.Name + ")");
        }

        protected internal virtual bool SameBehavior(Type sourceBehavior, Type targetBehavior)
        {
            if (sourceBehavior == targetBehavior)
                return true;
            var equivalentBehaviors = this.EquivalentBehaviors[sourceBehavior];
            if (equivalentBehaviors != null)
                return equivalentBehaviors.Contains(targetBehavior);
            return false;
        }
    }
}