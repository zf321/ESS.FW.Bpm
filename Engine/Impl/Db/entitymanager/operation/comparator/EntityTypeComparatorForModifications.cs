using System;
using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Batch;
using ESS.FW.Bpm.Engine.Dmn.Impl.Entity.Repository;
using ESS.FW.Bpm.Engine.Management;



namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation.Comparator
{
    /// <summary>
    ///     Compares operations by Entity type.
    ///     
    /// </summary>
    public class EntityTypeComparatorForModifications : IComparer<Type>
    {
        public static readonly IDictionary<Type, int> TypeOrder = new Dictionary<Type, int>();

        static EntityTypeComparatorForModifications()
        {
            TypeOrder[typeof (IBatch)] = 1;

            // 2
            //TypeOrder[typeof (TenantEntity)] = 2;
            //TypeOrder[typeof (GroupEntity)] = 2;
            //TypeOrder[typeof (UserEntity)] = 2;
            //TypeOrder[typeof (ResourceEntity)] = 2;
            //TypeOrder[typeof (TaskEntity)] = 2;
            TypeOrder[typeof (IJobDefinition)] = 2;

            // 3
            //TypeOrder[typeof (ExecutionEntity)] = 3;
            //TypeOrder[typeof (CaseExecutionEntity)] = 3;

            //// 4
            //TypeOrder[typeof (ProcessDefinitionEntity)] = 4;
            //TypeOrder[typeof (CaseDefinitionEntity)] = 4;
            TypeOrder[typeof (DecisionDefinitionEntity)] = 4;
            TypeOrder[typeof (DecisionRequirementsDefinitionEntity)] = 4;
            //TypeOrder[typeof (ResourceEntity)] = 4;

            //// 5
            //TypeOrder[typeof (DeploymentEntity)] = 5;
        }

        public virtual int Compare(Type firstEntityType, Type secondEntityType)
        {
            if (firstEntityType == secondEntityType)
            {
                return 0;
            }

            int? firstIndex = TypeOrder[firstEntityType];
            int? secondIndex = TypeOrder[secondEntityType];

            // unknown type happens before / after everything else
            if (firstIndex == null)
            {
                firstIndex = int.MaxValue;
            }
            if (secondIndex == null)
            {
                secondIndex = int.MaxValue;
            }

            var result = firstIndex.Value.CompareTo(secondIndex);
            if (result == 0)
            {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
                return firstEntityType.FullName.CompareTo(secondEntityType.FullName);
            }
            return result;
        }
    }
}