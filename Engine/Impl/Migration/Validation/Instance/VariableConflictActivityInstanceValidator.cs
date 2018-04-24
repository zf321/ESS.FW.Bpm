using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Impl.migration.instance;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.migration.validation.instance
{
    /// <summary>
    ///     Validates that when an activity instance has a variable with the same name twice (as a scope execution variable and
    ///     a
    ///     a concurrent variable parent execution variable), no situation occurs in which either one is overwritten.
    ///     
    /// </summary>
    public class VariableConflictActivityInstanceValidator : IMigratingActivityInstanceValidator
    {
        public virtual void Validate(MigratingActivityInstance migratingInstance,
            MigratingProcessInstance migratingProcessInstance,
            MigratingActivityInstanceValidationReportImpl instanceReport)
        {
            var sourceScope = migratingInstance.SourceScope;
            var targetScope = migratingInstance.TargetScope;

            if (migratingInstance.Migrates())
            {
                var becomesNonScope = sourceScope.IsScope && !targetScope.IsScope;
                if (becomesNonScope)
                {
                    var dependentVariablesByName = GetMigratingVariableInstancesByName(migratingInstance);
                    foreach (var variableName in dependentVariablesByName.Keys)
                        if (dependentVariablesByName[variableName].Count > 1)
                            instanceReport.AddFailure("The variable '" + variableName +
                                                      "' exists in both, this scope and " +
                                                      "concurrent local in the parent scope. " +
                                                      "Migrating to a non-scope activity would overwrite one of them.");
                }
            }
        }

        protected internal virtual IDictionary<string, IList<MigratingVariableInstance>>
            GetMigratingVariableInstancesByName(MigratingActivityInstance activityInstance)
        {
            IDictionary<string, IList<MigratingVariableInstance>> result =
                new Dictionary<string, IList<MigratingVariableInstance>>();

            foreach (var migratingInstance in activityInstance.MigratingDependentInstances)
                if (migratingInstance is MigratingVariableInstance)
                {
                    var migratingVariableInstance = (MigratingVariableInstance) migratingInstance;
                    CollectionUtil.AddToMapOfLists(result, migratingVariableInstance.VariableName,
                        migratingVariableInstance);
                }

            return result;
        }
    }
}